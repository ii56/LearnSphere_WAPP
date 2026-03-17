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
        int userId;

        protected void Page_Init(object sender, EventArgs e)
        {
            // 🔐 CSRF Protection
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 AUTHENTICATION
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);

            // 🔐 VALIDATE QUERY
            if (!int.TryParse(Request.QueryString["postid"], out postId) || postId <= 0)
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            // 🔐 AUTHORIZATION (CRITICAL)
            if (!CanAccessPost(postId, userId))
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                SetBackLink();
                LoadQuestion();
                LoadAnswers();
            }
        }

        // 🔐 ACCESS CONTROL
        private bool CanAccessPost(int postId, int userId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM ForumPost p
                    INNER JOIN CourseForum f ON p.forumid = f.forumid
                    WHERE p.postid = @pid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@pid", SqlDbType.Int).Value = postId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void SetBackLink()
        {
            if (int.TryParse(Request.QueryString["courseid"], out int courseId))
            {
                lnkBack.NavigateUrl = "ViewForum.aspx?courseid=" + courseId;
            }
            else
            {
                lnkBack.NavigateUrl = "Forums.aspx";
            }
        }

        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();
                object result = cmd.ExecuteScalar();

                imgSidebarProfile.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        private void LoadQuestion()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"
                        SELECT p.*, u.uname, u.ProfileImage
                        FROM ForumPost p
                        INNER JOIN [User] u ON p.userid = u.userid
                        WHERE p.postid = @id
                        AND p.deletiontime IS NULL";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = postId;

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblQuestionTitle.Text = Server.HtmlEncode(reader["title"].ToString());
                        lblQuestionContent.Text = Server.HtmlEncode(reader["content"].ToString());
                        lblQuestionUser.Text = Server.HtmlEncode(reader["uname"].ToString());
                        lblQuestionDate.Text = Convert.ToDateTime(reader["creationtime"]).ToString("dd MMM yyyy");

                        lblUpvotes.Text = "▲ " + reader["upvotes"].ToString();
                        lblDownvotes.Text = " ▼ " + reader["downvotes"].ToString();

                        imgQuestionUser.ImageUrl = GetProfileImage(reader["ProfileImage"]);
                        litTags.Text = FormatTags(reader["tags"]);
                    }
                    else
                    {
                        Response.Redirect("Forums.aspx");
                    }
                }
            }
            catch
            {
                lblMessage.Text = "Error loading forum.";
            }
        }

        private void LoadAnswers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"
                        SELECT p.*, u.uname, u.ProfileImage
                        FROM ForumPost p
                        INNER JOIN [User] u ON p.userid = u.userid
                        WHERE p.parentid = @postid
                        AND p.deletiontime IS NULL
                        ORDER BY p.creationtime ASC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@postid", SqlDbType.Int).Value = postId;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptAnswers.DataSource = dt;
                    rptAnswers.DataBind();

                    lblNoAnswers.Visible = dt.Rows.Count == 0;
                }
            }
            catch
            {
                lblMessage.Text = "Error loading answers.";
            }
        }

        // 🔐 SAFE TAG RENDERING
        protected string FormatTags(object tagObj)
        {
            if (tagObj == null || tagObj == DBNull.Value)
                return "";

            string[] tags = tagObj.ToString().Split(',');
            string result = "";

            foreach (string tag in tags)
            {
                result += "<span>" + Server.HtmlEncode(tag.Trim()) + "</span>";
            }

            return result;
        }

        protected string GetProfileImage(object imageObj)
        {
            if (imageObj == null || imageObj == DBNull.Value)
                return ResolveUrl("~/images/default-user.png");

            return ResolveUrl(imageObj.ToString());
        }

        // 🔐 DELETE HANDLER (POST ONLY)
        protected void rptAnswers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeletePost")
            {
                int deletePostId = Convert.ToInt32(e.CommandArgument);

                DeletePost(deletePostId);
                LoadAnswers();
            }
        }

        private void DeletePost(int deletePostId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"
                        UPDATE ForumPost
                        SET deletiontime = GETDATE()
                        WHERE postid = @pid
                        AND userid = @uid";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@pid", SqlDbType.Int).Value = deletePostId;
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;

                    con.Open();

                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        lblMessage.Text = "Unauthorized action.";
                    }
                }
            }
            catch
            {
                lblMessage.Text = "Error deleting post.";
            }
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

        // 🔐 CHECK IF USER OWNS POST
        protected bool IsOwner(object postUserIdObj)
        {
            if (postUserIdObj == null) return false;

            int postUserId = Convert.ToInt32(postUserIdObj);
            return postUserId == userId;
        }
    }
}