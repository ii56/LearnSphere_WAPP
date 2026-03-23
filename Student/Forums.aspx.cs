using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class Forums : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // set header name
            string displayName = GetDisplayName();
            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

            if (!IsPostBack)
                LoadForums("");
        }

        private string GetDisplayName()
        {
            if (Session["fname"] != null && Session["fname"].ToString() != "")
                return Session["fname"].ToString();

            // fetch from db if session doesnt have it
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

        private void LoadForums(string searchTerm)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // get all forums and count posts for each one
                    string query = @"
                        SELECT f.forumid, f.title, f.description, f.creationtime,
                               c.coursename,
                               (SELECT COUNT(*) FROM ForumPost fp WHERE fp.forumid = f.forumid AND fp.deletiontime IS NULL) AS postcount
                        FROM CourseForum f
                        INNER JOIN Course c ON f.courseid = c.courseid
                        WHERE f.deletiontime IS NULL";

                    // add search filter if user typed something
                    if (!string.IsNullOrEmpty(searchTerm))
                        query += " AND (f.title LIKE @search OR c.coursename LIKE @search)";

                    query += " ORDER BY f.creationtime DESC";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                            da.SelectCommand.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptForums.DataSource = dt;
                            rptForums.DataBind();
                            pnlEmpty.Visible = false;
                        }
                        else
                        {
                            rptForums.DataSource = null;
                            rptForums.DataBind();
                            pnlEmpty.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Could not load forums. Please try again.";
                lblError.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadForums(txtSearch.Text.Trim());
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadForums("");
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