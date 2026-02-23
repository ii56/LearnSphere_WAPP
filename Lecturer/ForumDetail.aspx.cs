using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class ForumDetail : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int postId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
                Response.Redirect("~/Login.aspx");

            if (!int.TryParse(Request.QueryString["postid"], out postId))
                Response.Redirect("Forums.aspx");

            if (Request.QueryString["delete"] != null)
            {
                int deletePostId;

                if (int.TryParse(Request.QueryString["delete"], out deletePostId))
                {
                    DeletePost(deletePostId);
                    return;
                }
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadQuestion();
                LoadAnswers();
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

        private void DeletePost(int postId)
        {
            int currentUserId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
            UPDATE ForumPost
            SET deletiontime = GETDATE()
            WHERE postid = @postid
            AND userid = @userid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@postid", postId);
                cmd.Parameters.AddWithValue("@userid", currentUserId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return;
                }
            }

            Response.Redirect("ForumDetail.aspx?postid=" + Request.QueryString["postid"] +
                              "&courseid=" + Request.QueryString["courseid"]);
        }

        private void LoadQuestion()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                            SELECT p.*, u.uname, u.ProfileImage
                            FROM ForumPost p
                            INNER JOIN [User] u ON p.userid = u.userid
                            WHERE p.postid = @postid
                            AND p.deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@postid", postId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblQuestionTitle.Text = reader["title"].ToString();
                    lblQuestionContent.Text = reader["content"].ToString();
                    lblQuestionUser.Text = reader["uname"].ToString();
                    lblQuestionDate.Text = Convert.ToDateTime(reader["creationtime"]).ToString("dd MMM yyyy");

                    lblUpvotes.Text = "▲ " + reader["upvotes"];
                    lblDownvotes.Text = " ▼ " + reader["downvotes"];

                    imgQuestionUser.ImageUrl = GetProfileImage(reader["ProfileImage"]);

                    litTags.Text = FormatTags(reader["tags"]);
                }
            }
        }

        private void LoadAnswers()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                            SELECT p.*, u.uname, u.ProfileImage
                            FROM ForumPost p
                            INNER JOIN [User] u ON p.userid = u.userid
                            WHERE p.parentid = @postid
                            AND p.deletiontime IS NULL
                            ORDER BY p.creationtime ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@postid", postId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptAnswers.DataSource = dt;
                rptAnswers.DataBind();
            }
        }

        protected string FormatTags(object tagObj)
        {
            if (tagObj == null) return "";

            string[] tags = tagObj.ToString().Split(',');
            string result = "";

            foreach (string tag in tags)
                result += $"<span>{tag.Trim()}</span>";

            return result;
        }

        protected string GetProfileImage(object imageObj)
        {
            if (imageObj == null || imageObj == DBNull.Value)
                return ResolveUrl("~/images/default-user.png");

            return ResolveUrl(imageObj.ToString());
        }

        protected void btnAnswer_Click(object sender, EventArgs e)
        {
            Response.Redirect("Answer.aspx?postid=" + postId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        protected string ShowDeleteButton(object postUserIdObj, object postIdObj)
        {
            if (Session["userid"] == null)
                return "";

            int currentUserId = Convert.ToInt32(Session["userid"]);
            int postUserId = Convert.ToInt32(postUserIdObj);
            int postId = Convert.ToInt32(postIdObj);

            if (currentUserId == postUserId)
            {
                return $"<a href='ForumDetail.aspx?delete={postId}&postid={Request.QueryString["postid"]}' class='btn-delete'>Delete</a>";
            }

            return "";
        }
    }
}