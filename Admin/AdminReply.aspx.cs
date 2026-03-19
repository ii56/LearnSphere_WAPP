using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminReply : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int questionId;
        int forumId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("../Login.aspx");
                return;
            }

            if (!int.TryParse(Request.QueryString["postid"], out questionId))
            {
                Response.Redirect("AdminForums.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadQuestion();
            }
        }



        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string imagePath = result.ToString();
                    sidebarImg.Src = ResolveUrl(imagePath);
                }
                else
                {
                    sidebarImg.Src = ResolveUrl("~/images/default-user.png");
                }
            }
        }



        private void LoadQuestion()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT title, content, forumid
                    FROM ForumPost
                    WHERE postid = @postid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@postid", SqlDbType.Int).Value = questionId;

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblQuestionTitle.Text = Server.HtmlEncode(reader["title"].ToString());
                    lblQuestionContent.Text = Server.HtmlEncode(reader["content"].ToString());

                    forumId = Convert.ToInt32(reader["forumid"]);
                }
                else
                {
                    Response.Redirect("AdminForums.aspx");
                }
            }
        }



        protected void btnPostAnswer_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                string content = txtAnswer.Text.Trim();
                string videoUrl = txtVideoUrl.Text.Trim();

                if (string.IsNullOrWhiteSpace(content))
                {
                    lblMessage.Text = "Answer cannot be empty.";
                    return;
                }

                if (content.Length > 2000)
                {
                    lblMessage.Text = "Answer is too long.";
                    return;
                }

                // XSS protection
                content = Server.HtmlEncode(content);

                if (!string.IsNullOrEmpty(videoUrl))
                {
                    Uri uriResult;

                    if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out uriResult))
                    {
                        lblMessage.Text = "Invalid video URL.";
                        return;
                    }
                }

                int parentPostId;

                if (!int.TryParse(Request.QueryString["postid"], out parentPostId))
                {
                    Response.Redirect("AdminForums.aspx");
                    return;
                }

                int forumId = 0;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string getForumQuery = "SELECT forumid FROM ForumPost WHERE postid = @postid";

                    SqlCommand getCmd = new SqlCommand(getForumQuery, conn);
                    getCmd.Parameters.Add("@postid", SqlDbType.Int).Value = parentPostId;

                    conn.Open();

                    object result = getCmd.ExecuteScalar();

                    if (result == null)
                    {
                        Response.Redirect("AdminForums.aspx");
                        return;
                    }

                    forumId = Convert.ToInt32(result);
                }



                string savedFileUrl = null;
                string savedImageUrl = null;

                // DOCUMENT UPLOAD
                if (fileUploadFile.HasFile)
                {
                    string ext = Path.GetExtension(fileUploadFile.FileName).ToLower();

                    string[] allowedDocs = { ".pdf", ".docx", ".zip" };

                    if (Array.IndexOf(allowedDocs, ext) < 0)
                    {
                        lblMessage.Text = "Invalid document type.";
                        return;
                    }

                    if (fileUploadFile.PostedFile.ContentLength > 5 * 1024 * 1024)
                    {
                        lblMessage.Text = "Document too large (max 5MB).";
                        return;
                    }

                    string newName = Guid.NewGuid().ToString() + ext;

                    string path = Server.MapPath("../Uploads/Documents/" + newName);

                    fileUploadFile.SaveAs(path);

                    savedFileUrl = "../Uploads/Documents/" + newName;
                }



                // IMAGE UPLOAD
                if (fileUploadImage.HasFile)
                {
                    string ext = Path.GetExtension(fileUploadImage.FileName).ToLower();

                    string[] allowedImages = { ".jpg", ".jpeg", ".png" };

                    if (Array.IndexOf(allowedImages, ext) < 0)
                    {
                        lblMessage.Text = "Invalid image type.";
                        return;
                    }

                    if (fileUploadImage.PostedFile.ContentLength > 3 * 1024 * 1024)
                    {
                        lblMessage.Text = "Image too large (max 3MB).";
                        return;
                    }

                    string newName = Guid.NewGuid().ToString() + ext;

                    string path = Server.MapPath("../Uploads/Images/" + newName);

                    fileUploadImage.SaveAs(path);

                    savedImageUrl = "../Uploads/Images/" + newName;
                }



                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string insertQuery = @"
                        INSERT INTO ForumPost
                        (forumid, userid, parentid, title, content, videourl)
                        VALUES
                        (@forumid, @userid, @parentid, NULL, @content, @videourl)";

                    SqlCommand cmd = new SqlCommand(insertQuery, conn);

                    cmd.Parameters.Add("@forumid", SqlDbType.Int).Value = forumId;
                    cmd.Parameters.Add("@userid", SqlDbType.Int).Value = Convert.ToInt32(Session["userid"]);
                    cmd.Parameters.Add("@parentid", SqlDbType.Int).Value = parentPostId;
                    cmd.Parameters.Add("@content", SqlDbType.NVarChar, 2000).Value = content;

                    if (string.IsNullOrEmpty(videoUrl))
                        cmd.Parameters.Add("@videourl", SqlDbType.NVarChar).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add("@videourl", SqlDbType.NVarChar).Value = videoUrl;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Make comment to Post (PostID: " + parentPostId + ")");
                Response.Redirect("AdminForumDetails.aspx?postid=" + parentPostId);
            }
            catch
            {
                lblMessage.Text = "An error occurred while posting the comment.";
            }
        }



        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminForumDetails.aspx?postid=" + Request.QueryString["postid"] + "&courseid=" + Request.QueryString["courseid"] );
        }



        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("../Login.aspx");
        }
    }
}