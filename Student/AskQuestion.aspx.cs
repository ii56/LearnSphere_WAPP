using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class AskQuestion : System.Web.UI.Page
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

            // need forumid to post to the right forum
            if (Request.QueryString["forumid"] == null)
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            forumId = Convert.ToInt32(Request.QueryString["forumid"]);

            // set back links
            linkBack.HRef = "Questions.aspx?forumid=" + forumId;
            linkCancel.HRef = "Questions.aspx?forumid=" + forumId;

            string displayName = GetDisplayName();
            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int userId = Convert.ToInt32(Session["userid"]);
            string title = txtTitle.Text.Trim();
            string content = txtContent.Text.Trim();
            string tags = txtTags.Text.Trim();

            // basic check just in case validators dont catch it
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                lblMessage.Text = "Please fill in the title and details.";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query = @"INSERT INTO ForumPost (forumid, userid, title, content, tags, creationtime)
                                     VALUES (@forumid, @userid, @title, @content, @tags, @now)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@forumid", forumId);
                        cmd.Parameters.AddWithValue("@userid", userId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@content", content);
                        cmd.Parameters.AddWithValue("@tags", string.IsNullOrEmpty(tags) ? (object)DBNull.Value : tags);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.ExecuteNonQuery();

                        LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Added a question to forum (ForumID: " + forumId + ")");
                    }
                }

                // go back to questions page after posting
                Response.Redirect("Questions.aspx?forumid=" + forumId);
            }
            catch
            {
                lblMessage.Text = "Something went wrong. Please try again.";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
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