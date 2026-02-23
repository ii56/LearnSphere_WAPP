using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class question : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int courseId;
        int forumId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
                Response.Redirect("~/Login.aspx");

            if (!int.TryParse(Request.QueryString["courseid"], out courseId))
                Response.Redirect("Forums.aspx");

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                GetForumId();
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", userId);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string imagePath = result.ToString();
                    Session["profileImage"] = imagePath;
                    imgSidebarProfile.Src = ResolveUrl(imagePath);
                }
                else
                {
                    imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                }
            }
        }

        private void GetForumId()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT forumid FROM CourseForum WHERE courseid = @courseid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@courseid", courseId);

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
            string title = txtTitle.Text.Trim();
            string content = txtContent.Text.Trim();
            string tags = txtTags.Text.Trim();
            string videoUrl = txtVideoUrl.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                lblMessage.Text = "Title and Content are required.";
                return;
            }

            GetForumId();

            int userId = Convert.ToInt32(Session["userid"]);
            int newPostId = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string insertQuery = @"
                            INSERT INTO ForumPost
                            (forumid, userid, parentid, title, content, tags, videourl)
                            OUTPUT INSERTED.postid
                            VALUES
                            (@forumid, @userid, NULL, @title, @content, @tags, @videourl)";

                SqlCommand cmd = new SqlCommand(insertQuery, conn);

                cmd.Parameters.AddWithValue("@forumid", forumId);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@content", content);
                cmd.Parameters.AddWithValue("@tags",
                    string.IsNullOrEmpty(tags) ? (object)DBNull.Value : tags);
                cmd.Parameters.AddWithValue("@videourl",
                    string.IsNullOrEmpty(videoUrl) ? (object)DBNull.Value : videoUrl);

                newPostId = (int)cmd.ExecuteScalar();

                if (fileUploadFile.HasFile)
                {
                    string extension = Path.GetExtension(fileUploadFile.FileName).ToLower();
                    string[] allowedFiles = { ".pdf", ".docx", ".zip" };

                    if (!allowedFiles.Contains(extension))
                    {
                        lblMessage.Text = "Invalid document file type.";
                        return;
                    }

                    string folder = Server.MapPath("~/Forum_materials/questions/files/");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = newPostId + "_" + userId + extension;
                    string fullPath = Path.Combine(folder, fileName);

                    fileUploadFile.SaveAs(fullPath);

                    string relativePath = "~/Forum_materials/questions/files/" + fileName;

                    SqlCommand updateCmd = new SqlCommand(
                        "UPDATE ForumPost SET fileurl=@fileurl WHERE postid=@postid", conn);

                    updateCmd.Parameters.AddWithValue("@fileurl", relativePath);
                    updateCmd.Parameters.AddWithValue("@postid", newPostId);
                    updateCmd.ExecuteNonQuery();
                }

                if (fileUploadImage.HasFile)
                {
                    string extension = Path.GetExtension(fileUploadImage.FileName).ToLower();
                    string[] allowedImages = { ".jpg", ".jpeg", ".png" };

                    if (!allowedImages.Contains(extension))
                    {
                        lblMessage.Text = "Invalid image file type.";
                        return;
                    }

                    string folder = Server.MapPath("~/Forum_materials/questions/images/");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = newPostId + "_" + userId + extension;
                    string fullPath = Path.Combine(folder, fileName);

                    fileUploadImage.SaveAs(fullPath);

                    string relativePath = "~/Forum_materials/questions/images/" + fileName;

                    SqlCommand updateCmd = new SqlCommand(
                        "UPDATE ForumPost SET imageurl=@imageurl WHERE postid=@postid", conn);

                    updateCmd.Parameters.AddWithValue("@imageurl", relativePath);
                    updateCmd.Parameters.AddWithValue("@postid", newPostId);
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
            Response.Redirect("~/Login.aspx");
        }
    }
}