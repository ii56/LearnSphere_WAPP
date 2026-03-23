using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class MyCourses : System.Web.UI.Page
    {
        // 1. Made the connection string 'readonly' since it doesn't change
        private readonly string connString = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 2. Session Security Check
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadUnreadMessagesBadge();
                BindEnrolledCourses();
            }
        }

        private void BindEnrolledCourses()
        {
            int currentUserId = Convert.ToInt32(Session["userid"]);

            // 3. Added Try-Catch block to prevent the app from crashing if the DB goes down
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT c.courseid, c.coursename, c.description, c.category 
                        FROM Course c
                        INNER JOIN Invoice i ON c.courseid = i.courseid
                        WHERE i.userid = @UserId 
                        AND c.status = 'Active' 
                        AND c.deletiontime IS NULL
                        ORDER BY i.creationtime DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                rptMyCourses.DataSource = dt;
                                rptMyCourses.DataBind();
                                rptMyCourses.Visible = true;
                                lblNoCourses.Visible = false;
                            }
                            else
                            {
                                rptMyCourses.Visible = false;
                                lblNoCourses.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error and show a user-friendly message instead of a yellow crash screen
                Debug.WriteLine("Database Error in BindEnrolledCourses: " + ex.Message);
                lblNoCourses.Text = "An error occurred while loading your courses. Please try again later.";
                lblNoCourses.Visible = true;
                rptMyCourses.Visible = false;
            }
        }

        private void LoadSidebarProfileImage()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                    // 4. Wrapped SqlCommand in its own using block for proper disposal
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        con.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            imgSidebarProfile.Src = ResolveUrl(result.ToString());
                        }
                        else
                        {
                            // Explicit fallback just in case the DB field is empty
                            imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading profile image: " + ex.Message);
                imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png"); // Safe fallback on error
            }
        }

        private void LoadUnreadMessagesBadge()
        {
            // 5. Safer parsing: Uses TryParse just in case the session object gets corrupted
            if (Session["unreadCount"] != null && int.TryParse(Session["unreadCount"].ToString(), out int unreadCount) && unreadCount > 0)
            {
                litUnreadBadge.Text = $"<span class='message-badge'>{unreadCount}</span>";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}