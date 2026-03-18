using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminForumDetails : System.Web.UI.Page
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
                Response.Redirect("../Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);

            // 🔐 VALIDATE QUERY
            if (!int.TryParse(Request.QueryString["postid"], out postId) || postId <= 0)
            {
                Response.Redirect("AdminForums.aspx");
                return;
            }

            // 🔐 AUTHORIZATION (CRITICAL)
            if (!CanAccessPost(postId, userId))
            {
                Response.Redirect("AdminForums.aspx");
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
                lnkBack.NavigateUrl = "AdminViewForums.aspx?courseid=" + courseId;
            }
            else
            {
                lnkBack.NavigateUrl = "AdminForums.aspx";
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

                sidebarImg.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("../images/default-user.png");
            }
        }

        private void LoadQuestion()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"
SELECT 
    p.postid,
    p.title,
    p.content,
    p.tags,
    p.creationtime,
    p.userid,
    u.uname,
    u.ProfileImage,

    ISNULL(SUM(CASE WHEN v.votetype = 1 THEN 1 ELSE 0 END), 0) AS upvotes,
    ISNULL(SUM(CASE WHEN v.votetype = -1 THEN 1 ELSE 0 END), 0) AS downvotes

FROM ForumPost p
INNER JOIN [User] u ON p.userid = u.userid
LEFT JOIN ForumVote v ON p.postid = v.postid

WHERE p.postid = @id
AND p.deletiontime IS NULL

GROUP BY 
    p.postid,
    p.title,
    p.content,
    p.tags,
    p.creationtime,
    p.userid,
    u.uname,
    u.ProfileImage";

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

                        likeCount.InnerText = reader["upvotes"].ToString();
                        dislikeCount.InnerText = reader["downvotes"].ToString();

                        imgQuestionUser.ImageUrl = GetProfileImage(reader["ProfileImage"]);
                        litTags.Text = FormatTags(reader["tags"]);
                    }
                    else
                    {
                        Response.Redirect("AdminForums.aspx");
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
SELECT 
    p.postid,
    p.content,
    p.creationtime,
    p.userid,
    u.uname,
    u.ProfileImage,

    ISNULL(SUM(CASE WHEN v.votetype = 1 THEN 1 ELSE 0 END), 0) AS upvotes,
    ISNULL(SUM(CASE WHEN v.votetype = -1 THEN 1 ELSE 0 END), 0) AS downvotes

FROM ForumPost p
INNER JOIN [User] u ON p.userid = u.userid
LEFT JOIN ForumVote v ON p.postid = v.postid

WHERE p.parentid = @postid
AND p.deletiontime IS NULL

GROUP BY 
    p.postid,
    p.content,
    p.creationtime,
    p.userid,
    u.uname,
    u.ProfileImage

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
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
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
                return ResolveUrl("../images/default-user.png");

            return ResolveUrl(imageObj.ToString());
        }

        // 🔐 DELETE HANDLER (POST ONLY)
        protected void rptAnswers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int pid = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "DeletePost")
            {
                DeletePost(pid);
            }
            else if (e.CommandName == "LikeAnswer")
            {
                HandleVote(pid, 1);
            }
            else if (e.CommandName == "DislikeAnswer")
            {
                HandleVote(pid, -1);
            }

            LoadAnswers();
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
            Response.Redirect("AdminReply.aspx?postid=" + postId + "&courseid=" + Request.QueryString["courseid"]);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("../Login.aspx");
        }

        // 🔐 CHECK IF USER OWNS POST
        protected bool IsOwner(object postUserIdObj)
        {
            if (postUserIdObj == null) return false;

            int postUserId = Convert.ToInt32(postUserIdObj);
            return postUserId == userId;
        }

        private void HandleVote(int postId, int voteType)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string checkQuery = "SELECT votetype FROM ForumVote WHERE postid=@pid AND userid=@uid";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@pid", postId);
                checkCmd.Parameters.AddWithValue("@uid", userId);

                object existingVote = checkCmd.ExecuteScalar();

                if (existingVote != null)
                {
                    int currentVote = Convert.ToInt32(existingVote);

                    if (currentVote == voteType)
                    {
                        // remove vote
                        SqlCommand del = new SqlCommand(
                            "DELETE FROM ForumVote WHERE postid=@pid AND userid=@uid", conn);
                        del.Parameters.AddWithValue("@pid", postId);
                        del.Parameters.AddWithValue("@uid", userId);
                        del.ExecuteNonQuery();
                    }
                    else
                    {
                        // switch vote
                        SqlCommand upd = new SqlCommand(
                            "UPDATE ForumVote SET votetype=@type WHERE postid=@pid AND userid=@uid", conn);
                        upd.Parameters.AddWithValue("@type", voteType);
                        upd.Parameters.AddWithValue("@pid", postId);
                        upd.Parameters.AddWithValue("@uid", userId);
                        upd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // insert
                    SqlCommand ins = new SqlCommand(
                        "INSERT INTO ForumVote(postid, userid, votetype) VALUES(@pid, @uid, @type)", conn);
                    ins.Parameters.AddWithValue("@pid", postId);
                    ins.Parameters.AddWithValue("@uid", userId);
                    ins.Parameters.AddWithValue("@type", voteType);
                    ins.ExecuteNonQuery();
                }
            }
        }

        protected void Vote_Command(object sender, CommandEventArgs e)
        {
            int postId;

            // ✅ SAFE parsing (no crash)
            if (!int.TryParse(e.CommandArgument?.ToString(), out postId))
            {
                lblMessage.Text = "Invalid post ID.";
                return;
            }

            if (e.CommandName == "LikeQuestion")
                HandleVote(postId, 1);
            else if (e.CommandName == "DislikeQuestion")
                HandleVote(postId, -1);

            LoadQuestion();
        }

        protected void btnLikeQuestion_Click(object sender, EventArgs e)
        {
            HandleVote(postId, 1);
            LoadQuestion();
        }

        protected void btnDislikeQuestion_Click(object sender, EventArgs e)
        {
            HandleVote(postId, -1);
            LoadQuestion();
        }
    }
}