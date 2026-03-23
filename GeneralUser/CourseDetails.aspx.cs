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
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            int currentUserId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
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
                                lblPrice.Text = price == 0 ? "Free" : "RM " + price.ToString("0.00");

                                bool isEnrolled = Convert.ToBoolean(reader["IsEnrolled"]);

                                if (isEnrolled)
                                {
                                    btnEnroll.Text = "Go to Course";
                                    btnEnroll.CssClass = "btn-goto-large";
                                }
                                else
                                {
                                    btnEnroll.Text = price == 0 ? "Enroll Free" : "Buy Now";
                                    btnEnroll.CssClass = "btn-enroll-large";
                                }
                            }
                            else
                            {
                                Response.Redirect("ViewCourses.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading course details: " + ex.Message);
            }
        }

        // This method handles the main UI button (Enroll Free vs Buy Now vs Go To Course)
        protected void btnCourseAction_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["courseid"] == null || !int.TryParse(Request.QueryString["courseid"], out int courseId))
                return;

            if (btnEnroll.Text == "Go to Course")
            {
                Response.Redirect($"CourseContent.aspx?id={courseId}");
                return;
            }

            // Fetch course data for enrollment/payment processing
            string courseName = "";
            decimal price = 0;

            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT coursename, price FROM Course WHERE courseid = @cid", con))
                {
                    cmd.Parameters.AddWithValue("@cid", courseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            courseName = reader["coursename"].ToString();
                            price = Convert.ToDecimal(reader["price"]);
                        }
                    }
                }
            }

            if (price == 0)
            {
                // Bypass payment for free courses
                ProcessEnrollment(courseId, price);
            }
            else
            {
                // Trigger Payment Modal
                hfCourseData.Value = courseId + "|" + price;
                string script = $"openModalWithData('{courseName.Replace("'", "\\'")}', '{price}');";
                ClientScript.RegisterStartupScript(this.GetType(), "OpenModal", script, true);
            }
        }

        // This method handles the confirmation from the JS Modal
        protected void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfCourseData.Value)) return;

            string[] parts = hfCourseData.Value.Split('|');
            int courseId = Convert.ToInt32(parts[0]);
            decimal amount = Convert.ToDecimal(parts[1]);

            ProcessEnrollment(courseId, amount);
        }

        // Helper method that modifies the DB, Auto-Upgrades, and Logouts
        private void ProcessEnrollment(int courseId, decimal amount)
        {
            int userId = Convert.ToInt32(Session["userid"]);
            string currentRole = Session["usertype"] != null ? Session["usertype"].ToString() : "General";

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();

                    // 1. Check to prevent double-charging/double-enrolling
                    using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Invoice WHERE userid = @uid AND courseid = @cid", con))
                    {
                        checkCmd.Parameters.AddWithValue("@uid", userId);
                        checkCmd.Parameters.AddWithValue("@cid", courseId);
                        if ((int)checkCmd.ExecuteScalar() > 0) return; // Already enrolled
                    }

                    // 2. Insert Invoice (Payment Record)
                    string query = @"INSERT INTO Invoice (userid, courseid, amount, overdue, duration, creationtime, deadline) 
                                     VALUES (@uid, @cid, @amount, 0, 30, @now, @deadline)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@cid", courseId);
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.Parameters.AddWithValue("@deadline", DateTime.Now.AddDays(30));
                        cmd.ExecuteNonQuery();
                    }

                    // 3. AUTO-UPGRADE LOGIC (General -> Student)
                    if (amount > 0)
                    {
                        // A. Update the User table
                        using (SqlCommand roleCmd = new SqlCommand("UPDATE [User] SET usertype = 'Student' WHERE userid = @uid", con))
                        {
                            roleCmd.Parameters.AddWithValue("@uid", userId);
                            roleCmd.ExecuteNonQuery();
                        }

                        // B. Create the Audit Trail in VerificationRequest (Automatically Approved)
                        string auditQuery = @"
                            INSERT INTO VerificationRequest 
                            (userid, currentrole, requestedrole, documentpath, status, requesttime, reviewedtime, remarks) 
                            VALUES 
                            (@uid, 'General', 'Student', 'System: Auto-Upgrade via Payment', 'Approved', GETDATE(), GETDATE(), 'Automatically upgraded after purchasing first course.')";

                        using (SqlCommand reqCmd = new SqlCommand(auditQuery, con))
                        {
                            reqCmd.Parameters.AddWithValue("@uid", userId);
                            reqCmd.ExecuteNonQuery();
                        }

                        // C. Clear Session and Force Logout
                        Session.Clear();
                        Session.Abandon();

                        // Redirect to login page. You could optionally add a query string here (e.g., ?status=upgraded) 
                        // to show a success message on the login screen.
                        Response.Redirect("~/Login.aspx");
                        return; // Stop execution so it doesn't try to update the UI below
                    }
                }

                // Update UI state (Only happens if they were ALREADY a Student or Lecturer)
                btnEnroll.Text = "Go to Course";
                btnEnroll.CssClass = "btn-goto-large";
                hfCourseData.Value = ""; // clear modal data

                lblMessage.Text = "Payment successful! Welcome to the course.";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during enrollment: " + ex.Message);
                lblMessage.Text = "Enrollment failed. You might already be enrolled.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }


        // --- Sidebar UI Methods ---
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