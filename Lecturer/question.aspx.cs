using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class question : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int courseId;
        int forumId;

        // 🔐 CSRF PROTECTION
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 AUTH CHECK
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // 🔐 VALIDATE courseid
            if (!int.TryParse(Request.QueryString["courseid"], out courseId))
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            // 🔐 VERIFY COURSE OWNERSHIP (IDOR protection)
            if (!IsCourseOwnedByUser(courseId, Convert.ToInt32(Session["userid"])))
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                GetForumId();
            }
        }

        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(Session["userid"]);

                con.Open();
                object result = cmd.ExecuteScalar();

                string imagePath = "~/images/default-user.png";

                if (result != null && result != DBNull.Value)
                {
                    string path = result.ToString();
                    if (path.StartsWith("~/images/"))
                        imagePath = path;
                }

                imgSidebarProfile.Src = ResolveUrl(imagePath);
            }
        }

        // 🔐 VERIFY COURSE OWNERSHIP
        private bool IsCourseOwnedByUser(int courseId, int userId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = "SELECT COUNT(*) FROM Course WHERE courseid=@cid AND ownerid=@uid";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void GetForumId()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT forumid FROM CourseForum WHERE courseid=@courseid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                    forumId = Convert.ToInt32(result);
                else
                    Response.Redirect("Forums.aspx");
            }
        }

        protected void btnPost_Click(object sender, EventArgs e)
        {
            // 🔐 RATE LIMIT (anti-spam)
            if (Session["lastPostTime"] != null)
            {
                DateTime last = (DateTime)Session["lastPostTime"];
                if ((DateTime.Now - last).TotalSeconds < 3)
                {
                    lblMessage.Text = "Please wait before posting again.";
                    return;
                }
            }
            Session["lastPostTime"] = DateTime.Now;

            string title = txtTitle.Text.Trim();
            string content = txtContent.Text.Trim();
            string tags = txtTags.Text.Trim();
            string videoUrl = txtVideoUrl.Text.Trim();

            // 🔐 VALIDATION
            if (title.Length < 3 || title.Length > 150)
            {
                lblMessage.Text = "Invalid title length.";
                return;
            }

            if (content.Length < 10 || content.Length > 2000)
            {
                lblMessage.Text = "Invalid content length.";
                return;
            }

            if (!string.IsNullOrEmpty(tags) && tags.Length > 200)
            {
                lblMessage.Text = "Tags too long.";
                return;
            }

            if (!string.IsNullOrEmpty(videoUrl) && videoUrl.Length > 300)
            {
                lblMessage.Text = "Video URL too long.";
                return;
            }

            GetForumId();

            int userId = Convert.ToInt32(Session["userid"]);
            int newPostId = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // 🔐 INSERT POST
                string insertQuery = @"
                    INSERT INTO ForumPost
                    (forumid, userid, parentid, title, content, tags, videourl)
                    OUTPUT INSERTED.postid
                    VALUES
                    (@forumid, @userid, NULL, @title, @content, @tags, @videourl)";

                SqlCommand cmd = new SqlCommand(insertQuery, conn);

                cmd.Parameters.Add("@forumid", SqlDbType.Int).Value = forumId;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@title", SqlDbType.NVarChar, 150).Value = title;
                cmd.Parameters.Add("@content", SqlDbType.NVarChar, 2000).Value = content;
                cmd.Parameters.Add("@tags", SqlDbType.NVarChar, 200).Value =
                    string.IsNullOrEmpty(tags) ? (object)DBNull.Value : tags;
                cmd.Parameters.Add("@videourl", SqlDbType.NVarChar, 300).Value =
                    string.IsNullOrEmpty(videoUrl) ? (object)DBNull.Value : videoUrl;

                newPostId = (int)cmd.ExecuteScalar();

                // 🔐 FILE UPLOAD (DOCUMENT)
                if (fileUploadFile.HasFile)
                {
                    string ext = Path.GetExtension(fileUploadFile.FileName).ToLower();
                    string mime = fileUploadFile.PostedFile.ContentType;

                    string[] allowedExt = { ".pdf", ".docx", ".zip" };

                    if (Array.IndexOf(allowedExt, ext) < 0)
                    {
                        lblMessage.Text = "Invalid document type.";
                        return;
                    }

                    if (fileUploadFile.PostedFile.ContentLength > 5 * 1024 * 1024)
                    {
                        lblMessage.Text = "File too large (max 5MB).";
                        return;
                    }

                    if (fileUploadFile.FileName.Contains(".."))
                    {
                        lblMessage.Text = "Invalid file name.";
                        return;
                    }

                    string folder = Server.MapPath("~/Forum_materials/questions/files/");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = newPostId + "_" + userId + ext;
                    string fullPath = Path.Combine(folder, fileName);

                    fileUploadFile.SaveAs(fullPath);

                    SqlCommand updateCmd = new SqlCommand(
                        "UPDATE ForumPost SET fileurl=@file WHERE postid=@id", conn);

                    updateCmd.Parameters.Add("@file", SqlDbType.NVarChar, 300)
                        .Value = "~/Forum_materials/questions/files/" + fileName;
                    updateCmd.Parameters.Add("@id", SqlDbType.Int).Value = newPostId;

                    updateCmd.ExecuteNonQuery();
                }

                // 🔐 IMAGE UPLOAD
                if (fileUploadImage.HasFile)
                {
                    string ext = Path.GetExtension(fileUploadImage.FileName).ToLower();
                    string mime = fileUploadImage.PostedFile.ContentType;

                    string[] allowedImg = { ".jpg", ".jpeg", ".png" };

                    if (Array.IndexOf(allowedImg, ext) < 0)
                    {
                        lblMessage.Text = "Invalid image type.";
                        return;
                    }

                    if (!(mime == "image/jpeg" || mime == "image/png"))
                    {
                        lblMessage.Text = "Invalid image MIME type.";
                        return;
                    }

                    if (fileUploadImage.PostedFile.ContentLength > 3 * 1024 * 1024)
                    {
                        lblMessage.Text = "Image too large (max 3MB).";
                        return;
                    }

                    if (fileUploadImage.FileName.Contains(".."))
                    {
                        lblMessage.Text = "Invalid image name.";
                        return;
                    }

                    string folder = Server.MapPath("~/Forum_materials/questions/images/");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = newPostId + "_" + userId + ext;
                    string fullPath = Path.Combine(folder, fileName);

                    fileUploadImage.SaveAs(fullPath);

                    SqlCommand updateCmd = new SqlCommand(
                        "UPDATE ForumPost SET imageurl=@img WHERE postid=@id", conn);

                    updateCmd.Parameters.Add("@img", SqlDbType.NVarChar, 300)
                        .Value = "~/Forum_materials/questions/images/" + fileName;
                    updateCmd.Parameters.Add("@id", SqlDbType.Int).Value = newPostId;

                    updateCmd.ExecuteNonQuery();
                }
            }

            Response.Redirect("ViewForum.aspx?courseid=" + courseId);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewForum.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Redirect("~/Login.aspx");
        }
    }
}