using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class CourseDetails : System.Web.UI.Page
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadUnreadMessagesBadge();
                LoadCourseDetails();
            }
        }

        private void LoadCourseDetails()
        {
            if (Request.QueryString["courseid"] == null || !int.TryParse(Request.QueryString["courseid"], out int courseId))
            {
                Response.Redirect("ViewCourses.aspx"); // Send back if URL is tampered with
                return;
            }

            int currentUserId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    // Query checks details AND if the user has an invoice for it
                    string query = @"
                        SELECT c.coursename, c.description, c.category, c.price, 
                               u.fname + ' ' + u.lname AS InstructorName,
                               CAST(CASE WHEN inv.userid IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS IsEnrolled
                        FROM Course c
                        INNER JOIN [User] u ON c.ownerid = u.userid
                        LEFT JOIN Invoice inv ON c.courseid = inv.courseid AND inv.userid = @UserId
                        WHERE c.courseid = @CourseId AND c.status = 1 AND c.deletiontime IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        cmd.Parameters.AddWithValue("@CourseId", courseId);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblCourseName.Text = reader["coursename"].ToString();
                                litDescription.Text = reader["description"].ToString().Replace("\n", "<br/>");
                                lblCategory.Text = reader["category"].ToString();
                                lblInstructorName.Text = reader["InstructorName"].ToString();

                                decimal price = Convert.ToDecimal(reader["price"]);
                                lblPrice.Text = price == 0 ? "Free" : "$" + price.ToString("0.00");

                                bool isEnrolled = Convert.ToBoolean(reader["IsEnrolled"]);

                                // Dynamic Button State
                                if (isEnrolled)
                                {
                                    btnCourseAction.Text = "Go to Course";
                                    btnCourseAction.CssClass = "btn-goto-large";
                                }
                                else
                                {
                                    btnCourseAction.Text = "Enroll Now";
                                    btnCourseAction.CssClass = "btn-enroll-large";
                                }
                            }
                            else
                            {
                                // Course doesn't exist or isn't published
                                Response.Redirect("ViewCourses.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading course details: " + ex.Message);
                lblMessage.Text = "Error loading course data. Please try again.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnCourseAction_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["courseid"] == null || !int.TryParse(Request.QueryString["courseid"], out int courseId))
                return;

            if (btnCourseAction.Text == "Go to Course")
            {
                // Already enrolled, jump straight into the content
                Response.Redirect($"CourseContent.aspx?id={courseId}");
            }
            else
            {
                // Not enrolled, process the enrollment
                EnrollUserViaInvoice(courseId);
            }
        }

        private void EnrollUserViaInvoice(int courseId)
        {
            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();
                    string query = @"INSERT INTO Invoice (userid, courseid, invoicetime) 
                                     VALUES (@uid, @cid, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@cid", courseId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Change state immediately without full page reload
                btnCourseAction.Text = "Go to Course";
                btnCourseAction.CssClass = "btn-goto-large";

                lblMessage.Text = "Successfully enrolled! Welcome to the course.";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during enrollment: " + ex.Message);
                lblMessage.Text = "Enrollment failed. You might already be enrolled.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        // --- Standard Sidebar Methods ---
        private void LoadSidebarProfileImage()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        imgSidebarProfile.Src = (result != null && result != DBNull.Value) ? ResolveUrl(result.ToString()) : ResolveUrl("~/images/default-user.png");
                    }
                }
            }
            catch { imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png"); }
        }

        private void LoadUnreadMessagesBadge()
        {
            if (Session["unreadCount"] != null && int.TryParse(Session["unreadCount"].ToString(), out int count) && count > 0)
                litUnreadBadge.Text = $"<span class='message-badge'>{count}</span>";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}