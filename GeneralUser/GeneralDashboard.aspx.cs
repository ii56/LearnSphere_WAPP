using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class GeneralDashboard : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"].ToString() != "General")
            {
                Response.Redirect("~/Login.aspx");

            }

            if (!IsPostBack)
            {
                lblUserName.Text = Session["fname"] != null ? Session["fname"].ToString() : Session["uname"].ToString();

                LoadUnreadMessagesBadge();
                LoadSidebarProfileImage();
                BindStudyingCourses();
                BindRecommendedCourses();
            }
            Syslog.action(Convert.ToInt32(Session["userid"]), "Log In");

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
        private void LoadUnreadMessagesBadge()
        {
            if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0)
            {
                litUnreadBadge.Text = $"<span class='message-badge'>{Session["unreadCount"]}</span>";
            }
        }


        private void BindStudyingCourses()
        {
            int currentUserId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT c.courseid, c.coursename, c.description, c.category 
                    FROM Course c
                    INNER JOIN Invoice i ON c.courseid = i.courseid
                    WHERE i.userid = @UserId AND c.status = 'Active' AND c.deletiontime IS NULL";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptStudyingCourses.DataSource = dt;
                            rptStudyingCourses.DataBind();
                            lblNoStudying.Visible = false;
                        }
                        else
                        {
                            rptStudyingCourses.Visible = false;
                            lblNoStudying.Visible = true;
                        }
                    }
                }
            }
        }

        private void BindRecommendedCourses()
        {
            int currentUserId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT TOP 4 courseid, coursename, description, price, category 
                    FROM Course 
                    WHERE status = 'Active' AND deletiontime IS NULL AND price = 0
                    AND courseid NOT IN (SELECT courseid FROM Invoice WHERE userid = @UserId)
                    ORDER BY creationtime DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptRecommendedCourses.DataSource = dt;
                            rptRecommendedCourses.DataBind();
                            lblNoRecommendations.Visible = false;
                        }
                        else
                        {
                            rptRecommendedCourses.Visible = false;
                            lblNoRecommendations.Visible = true;
                        }
                    }
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Syslog.action(Convert.ToInt32(Session["userid"]), "Log Out");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }


    }
}