using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class Answers : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        private int postId = 0;
        private int forumId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (Request.QueryString["postid"] == null)
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            postId = Convert.ToInt32(Request.QueryString["postid"]);

            string displayName = GetDisplayName();
            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

            if (!IsPostBack)
            {
                LoadQuestion();
                LoadAnswers();
            }
        }

        private string GetDisplayName()
        {
            if (Session["fname"] != null && Session["fname"].ToString() != "")
                return Session["fname"].ToString();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT fname FROM [User] WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", Convert.ToInt32(Session["userid"]));
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        Session["fname"] = result.ToString();
                        return result.ToString();
                    }
                }
            }
            return "Student";
        }

        private void LoadQuestion()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query = @"SELECT p.postid, p.forumid, p.title, p.content, p.upvotes, p.downvotes, p.creationtime,
                                            u.fname, u.lname
                                     FROM ForumPost p
                                     INNER JOIN [User] u ON p.userid = u.userid
                                     WHERE p.postid = @pid AND p.deletiontime IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@pid", postId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                forumId = Convert.ToInt32(reader["forumid"]);
                                linkBack.HRef = "Questions.aspx?forumid=" + forumId;

                                lblTitle.Text = reader["title"].ToString();
                                lblContent.Text = reader["content"].ToString();
                                lblUpvotes.Text = reader["upvotes"].ToString();
                                lblDownvotes.Text = reader["downvotes"].ToString();
                                lblPostDate.Text = Convert.ToDateTime(reader["creationtime"]).ToString("MMM dd, yyyy");

                                string fname = reader["fname"].ToString();
                                string lname = reader["lname"].ToString();
                                lblAuthorName.Text = fname + " " + lname;
                                lblAuthorInitial.Text = fname.Substring(0, 1).ToUpper();
                            }
                            else
                            {
                                Response.Redirect("Forums.aspx");
                            }
                        }
                    }
                }
            }
            catch
            {
                lblError.Text = "Could not load the question.";
                lblError.Visible = true;
            }
        }

        private void LoadAnswers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // get all replies to this post (parentid = postId)
                    string query = @"SELECT p.content, p.creationtime, u.fname, u.lname
                                     FROM ForumPost p
                                     INNER JOIN [User] u ON p.userid = u.userid
                                     WHERE p.parentid = @pid AND p.deletiontime IS NULL
                                     ORDER BY p.creationtime ASC";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@pid", postId);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        lblAnswerCount.Text = dt.Rows.Count.ToString();

                        if (dt.Rows.Count > 0)
                        {
                            rptAnswers.DataSource = dt;
                            rptAnswers.DataBind();
                            pnlNoAnswers.Visible = false;
                        }
                        else
                        {
                            rptAnswers.DataSource = null;
                            rptAnswers.DataBind();
                            pnlNoAnswers.Visible = true;
                        }
                    }
                }
            }
            catch
            {
                lblError.Text = "Could not load answers.";
                lblError.Visible = true;
            }
        }

        protected void btnPostAnswer_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string answer = txtAnswer.Text.Trim();
            if (string.IsNullOrEmpty(answer)) return;

            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // first get the forumid for this post so we can insert properly
                    int fid = 0;
                    using (SqlCommand getfid = new SqlCommand("SELECT forumid FROM ForumPost WHERE postid = @pid", con))
                    {
                        getfid.Parameters.AddWithValue("@pid", postId);
                        fid = Convert.ToInt32(getfid.ExecuteScalar());
                    }

                    // insert as a reply (parentid = postId)
                    string query = @"INSERT INTO ForumPost (forumid, userid, parentid, content, creationtime)
                                     VALUES (@forumid, @userid, @parentid, @content, @now)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@forumid", fid);
                        cmd.Parameters.AddWithValue("@userid", userId);
                        cmd.Parameters.AddWithValue("@parentid", postId);
                        cmd.Parameters.AddWithValue("@content", answer);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.ExecuteNonQuery();
                        LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Post Answer (ForumID: " + fid + ")");
                    }
                }

                txtAnswer.Text = "";
                lblAnswerMsg.Text = "Your answer was posted!";
                lblAnswerMsg.CssClass = "alert alert-success";
                lblAnswerMsg.Visible = true;

                // reload answers
                LoadAnswers();
            }
            catch
            {
                lblAnswerMsg.Text = "Something went wrong. Please try again.";
                lblAnswerMsg.CssClass = "alert alert-danger";
                lblAnswerMsg.Visible = true;
            }
        }

        protected void btnUpvote_Click(object sender, EventArgs e)
        {
            UpdateVote(1);
        }

        protected void btnDownvote_Click(object sender, EventArgs e)
        {
            UpdateVote(-1);
        }

        private void UpdateVote(int voteType)
        {
            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // check if user already voted on this post
                    int existing = 0;
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM ForumVote WHERE postid = @pid AND userid = @uid", con))
                    {
                        chk.Parameters.AddWithValue("@pid", postId);
                        chk.Parameters.AddWithValue("@uid", userId);
                        existing = (int)chk.ExecuteScalar();
                    }

                    if (existing > 0)
                    {
                        // already voted, dont let them vote again
                        lblError.Text = "You already voted on this question.";
                        lblError.Visible = true;
                        return;
                    }

                    // insert vote record
                    using (SqlCommand ins = new SqlCommand(
                        "INSERT INTO ForumVote (postid, userid, votetype, creationtime) VALUES (@pid, @uid, @vtype, @now)", con))
                    {
                        ins.Parameters.AddWithValue("@pid", postId);
                        ins.Parameters.AddWithValue("@uid", userId);
                        ins.Parameters.AddWithValue("@vtype", voteType);
                        ins.Parameters.AddWithValue("@now", DateTime.Now);
                        ins.ExecuteNonQuery();
                    }

                    // update the upvotes or downvotes count on the post
                    string updateCol = voteType == 1 ? "upvotes" : "downvotes";
                    using (SqlCommand upd = new SqlCommand(
                        "UPDATE ForumPost SET " + updateCol + " = " + updateCol + " + 1 WHERE postid = @pid", con))
                    {
                        upd.Parameters.AddWithValue("@pid", postId);
                        upd.ExecuteNonQuery();
                    }
                }

                // reload to show updated count
                LoadQuestion();
            }
            catch
            {
                lblError.Text = "Could not save your vote. Please try again.";
                lblError.Visible = true;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}