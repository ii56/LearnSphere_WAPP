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
    public partial class ViewForum : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        int courseId;
        int forumId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!int.TryParse(Request.QueryString["courseid"], out courseId))
                Response.Redirect("Forums.aspx");

            if (!IsPostBack)
            {
                LoadForumDetails();
                LoadSidebarProfileImage();
                LoadQuestions();
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

        protected string GetProfileImage(object imageObj)
        {
            if (imageObj == null || imageObj == DBNull.Value)
                return ResolveUrl("~/images/default-user.png");

            string path = imageObj.ToString();

            if (string.IsNullOrEmpty(path))
                return ResolveUrl("~/images/default-user.png");

            return ResolveUrl(path);
        }

        private void LoadForumDetails()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT forumid, title, description, tags
                    FROM CourseForum
                    WHERE courseid = @courseid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    forumId = Convert.ToInt32(reader["forumid"]);
                    lblForumTitle.Text = reader["title"].ToString();
                    lblDescription.Text = reader["description"].ToString();
                    lblTags.Text = reader["tags"]?.ToString();
                }
                else
                {
                    Response.Redirect("Forums.aspx");
                }
            }
        }

        private void LoadQuestions()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                                SELECT p.postid,
       p.title,
       p.content,
       p.tags,
       p.creationtime,
       u.uname,
       u.ProfileImage,

       SUM(CASE WHEN v.votetype = 1 THEN 1 ELSE 0 END) AS upvotes,
       SUM(CASE WHEN v.votetype = -1 THEN 1 ELSE 0 END) AS downvotes

FROM ForumPost p
INNER JOIN CourseForum f ON p.forumid = f.forumid
INNER JOIN [User] u ON p.userid = u.userid
LEFT JOIN ForumVote v ON p.postid = v.postid

WHERE f.courseid = @courseid
AND p.parentid IS NULL
AND p.deletiontime IS NULL

GROUP BY p.postid, p.title, p.content, p.tags, p.creationtime, u.uname, u.ProfileImage

ORDER BY p.creationtime DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptQuestions.DataSource = dt;
                rptQuestions.DataBind();
            }
        }

        protected string FormatTags(object tagObj)
        {
            if (tagObj == null) return "";

            string[] tags = tagObj.ToString().Split(',');

            string result = "";

            foreach (string tag in tags)
            {
                result += $"<span>{tag.Trim()}</span>";
            }

            return result;
        }

        protected void btnAskQuestion_Click(object sender, EventArgs e)
        {
            Response.Redirect("question.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("~/Login.aspx");
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int postId;
            if (!int.TryParse(e.CommandArgument.ToString(), out postId))
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            int voteType = 0;

            if (e.CommandName == "Like")
                voteType = 1;
            else if (e.CommandName == "Dislike")
                voteType = -1;
            else
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // 🔍 Check if user already voted
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
                        // 🔥 Same vote clicked again → REMOVE vote
                        string deleteQuery = "DELETE FROM ForumVote WHERE postid=@pid AND userid=@uid";
                        SqlCommand delCmd = new SqlCommand(deleteQuery, conn);
                        delCmd.Parameters.AddWithValue("@pid", postId);
                        delCmd.Parameters.AddWithValue("@uid", userId);
                        delCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // 🔥 Change vote (like ↔ dislike)
                        string updateQuery = "UPDATE ForumVote SET votetype=@type WHERE postid=@pid AND userid=@uid";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@type", voteType);
                        updateCmd.Parameters.AddWithValue("@pid", postId);
                        updateCmd.Parameters.AddWithValue("@uid", userId);
                        updateCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 🔥 Insert new vote
                    string insertQuery = @"INSERT INTO ForumVote(postid, userid, votetype)
                                  VALUES(@pid, @uid, @type)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@pid", postId);
                    insertCmd.Parameters.AddWithValue("@uid", userId);
                    insertCmd.Parameters.AddWithValue("@type", voteType);
                    insertCmd.ExecuteNonQuery();
                }
            }

            // 🔥 Refresh UI
            LoadQuestions();
        }
    }
}