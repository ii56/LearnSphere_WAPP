using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class answer : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        int questionId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || Session["usertype"].ToString() != "General")
                Response.Redirect("~/Login.aspx");

            if (!int.TryParse(Request.QueryString["postid"], out questionId))
                Response.Redirect("Forums.aspx");

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadQuestion();
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null) return;
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
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

        private void LoadQuestion()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT p.title, p.content, p.forumid
                    FROM ForumPost p
                    WHERE p.postid = @postid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@postid", questionId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblQuestionTitle.Text = "<strong>" + reader["title"].ToString() + "</strong>";
                    lblQuestionContent.Text = reader["content"].ToString();
                }
                else
                {
                    Response.Redirect("Forums.aspx");
                }
            }
        }

        protected void btnPostAnswer_Click(object sender, EventArgs e)
        {
            string content = txtAnswer.Text.Trim();
            string videoUrl = txtVideoUrl.Text.Trim();

            if (string.IsNullOrEmpty(content))
            {
                lblMessage.Text = "Answer cannot be empty.";
                return;
            }

            int parentPostId;
            if (!int.TryParse(Request.QueryString["postid"], out parentPostId))
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            int forumId = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string getForumQuery = "SELECT forumid FROM ForumPost WHERE postid = @postid";
                SqlCommand getCmd = new SqlCommand(getForumQuery, conn);
                getCmd.Parameters.AddWithValue("@postid", parentPostId);
                conn.Open();
                object result = getCmd.ExecuteScalar();

                if (result == null)
                {
                    Response.Redirect("Forums.aspx");
                    return;
                }
                forumId = Convert.ToInt32(result);
            }

            int userId = Convert.ToInt32(Session["userid"]);
            int newPostId = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string insertQuery = @"
                    INSERT INTO ForumPost
                    (forumid, userid, parentid, title, content, videourl)
                    OUTPUT INSERTED.postid
                    VALUES
                    (@forumid, @userid, @parentid, NULL, @content, @videourl)";

                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@forumid", forumId);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@parentid", parentPostId);
                cmd.Parameters.AddWithValue("@content", content);
                cmd.Parameters.AddWithValue("@videourl", string.IsNullOrEmpty(videoUrl) ? (object)DBNull.Value : videoUrl);

                conn.Open();
                newPostId = (int)cmd.ExecuteScalar();

                // Missing File Upload Logic Added Here!
                if (fileUploadFile.HasFile)
                {
                    string extension = Path.GetExtension(fileUploadFile.FileName).ToLower();
                    string[] allowedFiles = { ".pdf", ".docx", ".zip" };

                    if (!allowedFiles.Contains(extension))
                    {
                        lblMessage.Text = "Invalid document file type.";
                        return;
                    }

                    string folder = Server.MapPath("~/Forum_materials/answers/files/");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = newPostId + "_" + userId + extension;
                    string fullPath = Path.Combine(folder, fileName);
                    fileUploadFile.SaveAs(fullPath);

                    string relativePath = "~/Forum_materials/answers/files/" + fileName;
                    SqlCommand updateCmd = new SqlCommand("UPDATE ForumPost SET fileurl=@fileurl WHERE postid=@postid", conn);
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

                    string folder = Server.MapPath("~/Forum_materials/answers/images/");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = newPostId + "_" + userId + extension;
                    string fullPath = Path.Combine(folder, fileName);
                    fileUploadImage.SaveAs(fullPath);

                    string relativePath = "~/Forum_materials/answers/images/" + fileName;
                    // Adjusted query here if needed based on your DB schema constraints discussed earlier
                    SqlCommand updateCmd = new SqlCommand("UPDATE ForumPost SET imageurl=@imageurl WHERE postid=@postid", conn);
                    updateCmd.Parameters.AddWithValue("@imageurl", relativePath);
                    updateCmd.Parameters.AddWithValue("@postid", newPostId);
                    updateCmd.ExecuteNonQuery();
                }
            }

            Response.Redirect("ForumDetail.aspx?postid=" + parentPostId);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ForumDetail.aspx?postid=" + questionId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}