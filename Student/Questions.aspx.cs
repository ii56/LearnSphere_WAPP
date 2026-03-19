using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class Questions : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        private int forumId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // check if forumid was passed in the url
            if (Request.QueryString["forumid"] == null)
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            forumId = Convert.ToInt32(Request.QueryString["forumid"]);

            string displayName = GetDisplayName();
            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

            // set the ask question link to pass the forumid
            linkAskQuestion.HRef = "AskQuestion.aspx?forumid=" + forumId;
            linkAskFirst.HRef = "AskQuestion.aspx?forumid=" + forumId;

            if (!IsPostBack)
            {
                LoadForumInfo();
                LoadQuestions();
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

        private void LoadForumInfo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string query = @"SELECT f.title, f.description, c.coursename
                                     FROM CourseForum f
                                     INNER JOIN Course c ON f.courseid = c.courseid
                                     WHERE f.forumid = @fid AND f.deletiontime IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@fid", forumId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblForumTitle.Text = reader["title"].ToString();
                                lblPageTitle.Text = reader["title"].ToString();
                                lblForumDesc.Text = reader["coursename"].ToString();
                            }
                            else
                            {
                                // forum not found, go back
                                Response.Redirect("Forums.aspx");
                            }
                        }
                    }
                }
            }
            catch
            {
                lblError.Text = "Could not load forum info.";
                lblError.Visible = true;
            }
        }

        private void LoadQuestions()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // get all top level posts (parentid is null = it's a question not a reply)
                    string query = @"
                        SELECT p.postid, p.title, p.content, p.tags, p.creationtime,
                               u.fname, u.lname,
                               (SELECT COUNT(*) FROM ForumPost r WHERE r.parentid = p.postid AND r.deletiontime IS NULL) AS replycount
                        FROM ForumPost p
                        INNER JOIN [User] u ON p.userid = u.userid
                        WHERE p.forumid = @fid AND p.parentid IS NULL AND p.deletiontime IS NULL
                        ORDER BY p.creationtime DESC";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@fid", forumId);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        lblPostCount.Text = dt.Rows.Count + " question" + (dt.Rows.Count != 1 ? "s" : "");

                        if (dt.Rows.Count > 0)
                        {
                            rptQuestions.DataSource = dt;
                            rptQuestions.DataBind();
                            pnlEmpty.Visible = false;
                        }
                        else
                        {
                            rptQuestions.DataSource = null;
                            rptQuestions.DataBind();
                            pnlEmpty.Visible = true;
                        }
                    }
                }
            }
            catch
            {
                lblError.Text = "Could not load questions. Please try again.";
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