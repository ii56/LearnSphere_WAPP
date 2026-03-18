using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class Forums : System.Web.UI.Page
    {
        // Get connection string from web.config
        string connString = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Session Check
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadEnrolledCourses();
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null) return;
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connString))
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

        private void LoadEnrolledCourses()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query fetches courses the user is enrolled in, and checks if a forum exists for that course.
                    // Adjust table/column names based on your actual database schema.
                    string query = @"
                        SELECT 
                            c.courseid, 
                            c.coursename, 
                            CAST(CASE WHEN f.courseid IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS HasForum
                        FROM Courses c
                        INNER JOIN Enrollments e ON c.courseid = e.courseid
                        LEFT JOIN Forums f ON c.courseid = f.courseid
                        WHERE e.userid = @userid";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userid", Session["userid"]);

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            gvCourses.DataSource = dt;
                            gvCourses.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the error
                System.Diagnostics.Debug.WriteLine("Error loading courses: " + ex.Message);
            }
        }

        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewForum")
            {
                // Retrieve the courseid passed from the CommandArgument
                string courseId = e.CommandArgument.ToString();

                // Redirect to the actual forum thread/details page and pass the course ID
                Response.Redirect($"ForumDetails.aspx?courseid={courseId}");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session and redirect to login
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}