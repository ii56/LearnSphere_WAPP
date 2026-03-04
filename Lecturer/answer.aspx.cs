using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class answer : Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int questionId;
        int forumId;
        int parentPostId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
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
                    lblQuestionTitle.Text = reader["title"].ToString();
                    lblQuestionContent.Text = reader["content"].ToString();
                    forumId = Convert.ToInt32(reader["forumid"]);
                }
                else
                {
                    Response.Redirect("Forums.aspx");
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
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

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string insertQuery = @"
            INSERT INTO ForumPost
            (forumid, userid, parentid, title, content, videourl)
            VALUES
            (@forumid, @userid, @parentid, NULL, @content, @videourl)";

                SqlCommand cmd = new SqlCommand(insertQuery, conn);

                cmd.Parameters.AddWithValue("@forumid", forumId);
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt32(Session["userid"]));
                cmd.Parameters.AddWithValue("@parentid", parentPostId);
                cmd.Parameters.AddWithValue("@content", content);
                cmd.Parameters.AddWithValue("@videourl",
                    string.IsNullOrEmpty(videoUrl) ? (object)DBNull.Value : videoUrl);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Response.Redirect("ForumDetail.aspx?postid=" + parentPostId);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewQuestion.aspx?postid=" + questionId);
        }
    }
}