using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class BrowseCourses : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null || Session["usertype"].ToString() != "General")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfile();
                LoadCourses();
            }
        }

        private void LoadSidebarProfile()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string displayName = "User";
                using (SqlCommand cmd = new SqlCommand("SELECT fname, ProfileImage FROM [User] WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            displayName = reader["fname"].ToString();
                            Session["fname"] = displayName;

                            string imgPath = reader["ProfileImage"] != DBNull.Value ? reader["ProfileImage"].ToString() : "";
                            if (!string.IsNullOrEmpty(imgPath))
                            {
                                imgAvatar.ImageUrl = ResolveUrl(imgPath);
                                imgAvatar.Visible = true;
                                lblAvatarInitial.Visible = false;
                            }
                            else
                            {
                                imgAvatar.Visible = false;
                                lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();
                                lblAvatarInitial.Visible = true;
                            }
                        }
                    }
                }
                lblHeaderName.Text = displayName;
            }
        }

        private void LoadCourses()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = @"
                    SELECT c.courseid, c.coursename, c.description, c.category, c.price,
                           u.fname + ' ' + u.lname AS lecturerName,
                           CASE WHEN e.enrollmentid IS NOT NULL THEN 1 ELSE 0 END AS IsEnrolled
                    FROM Course c
                    INNER JOIN [User] u ON c.ownerid = u.userid
                    LEFT JOIN Enrollment e 
                        ON c.courseid = e.courseid 
                        AND e.userid = @uid 
                        AND e.isactive = 1
                    WHERE c.status = 'Active' AND c.deletiontime IS NULL";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);

                    // Apply Filters
                    if (!string.IsNullOrEmpty(txtSearch.Text))
                    {
                        query += " AND c.coursename LIKE @search";
                        cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text.Trim() + "%");
                    }
                    if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                    {
                        query += " AND c.category = @cat";
                        cmd.Parameters.AddWithValue("@cat", ddlCategory.SelectedValue);
                    }

                    query += " ORDER BY c.creationtime DESC";
                    cmd.CommandText = query;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptCourses.DataSource = dt;
                            rptCourses.DataBind();
                            pnlEmpty.Visible = false;
                        }
                        else
                        {
                            rptCourses.DataSource = null;
                            rptCourses.DataBind();
                            pnlEmpty.Visible = true;
                        }
                    }
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadCourses();
        }

        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                Response.Redirect("CourseDetails.aspx?courseid=" + e.CommandArgument);
                return;
            }

            string[] args = e.CommandArgument.ToString().Split('|');
            if (args.Length < 3) return;

            int courseId = Convert.ToInt32(args[0]);
            string courseName = args[1];
            decimal price = Convert.ToDecimal(args[2]);

            if (e.CommandName == "EnrollFree")
            {
                // Free courses instantly enroll without upgrading the user
                ProcessEnrollment(courseId, 0, courseName);
            }
            else if (e.CommandName == "OpenPayment")
            {
                // Trigger the JS Payment Modal via HiddenField
                hfCourseId.Value = courseId + "|" + courseName + "|" + price;
                LoadCourses(); // Rebind to ensure page renders properly for JS to catch the HiddenField
            }
        }

        protected void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            string courseData = hfCourseId.Value;
            if (string.IsNullOrEmpty(courseData)) return;

            string[] parts = courseData.Split('|');
            int courseId = Convert.ToInt32(parts[0]);
            string courseName = parts[1];
            decimal amount = Convert.ToDecimal(parts[2]);

            // Clear hidden field so modal doesn't re-open
            hfCourseId.Value = "";

            // Paid courses trigger the auto-upgrade
            ProcessEnrollment(courseId, amount, courseName);
        }

        private void ProcessEnrollment(int courseId, decimal amount, string courseName)
        {
            int userId = Convert.ToInt32(Session["userid"]);
            string currentRole = Session["usertype"].ToString();

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // 1. Prevent duplicate enrollment
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM Enrollment WHERE userid=@uid AND courseid=@cid AND isactive=1", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@cid", courseId);
                        if ((int)chk.ExecuteScalar() > 0)
                        {
                            ShowMsg("You are already enrolled in this course.", false);
                            return;
                        }
                    }

                    // 2. Insert Enrollment
                    using (SqlCommand enrollCmd = new SqlCommand("INSERT INTO Enrollment (userid, courseid, enrolldate, isactive) VALUES (@uid, @cid, GETDATE(), 1)", con))
                    {
                        enrollCmd.Parameters.AddWithValue("@uid", userId);
                        enrollCmd.Parameters.AddWithValue("@cid", courseId);
                        enrollCmd.ExecuteNonQuery();
                    }

                    // 3. Insert Invoice (if paid)
                    if (amount > 0)
                    {
                        using (SqlCommand invCmd = new SqlCommand("INSERT INTO Invoice (userid, courseid, amount, overdue, duration, creationtime, deadline) VALUES (@uid, @cid, @amount, 0, 30, GETDATE(), DATEADD(day, 30, GETDATE()))", con))
                        {
                            invCmd.Parameters.AddWithValue("@uid", userId);
                            invCmd.Parameters.AddWithValue("@cid", courseId);
                            invCmd.Parameters.AddWithValue("@amount", amount);
                            invCmd.ExecuteNonQuery();
                        }
                    }

                    // 4. Log Action
                    LearnSphere_WAPP.Syslog.action(userId, $"Enrolled in Course (CourseID: {courseId}, Amount: {amount})");

                    // 5. AUTO-UPGRADE LOGIC (Only if they paid money and are currently 'General')
                    if (amount > 0 && currentRole == "General")
                    {
                        // Upgrade to Student
                        using (SqlCommand roleCmd = new SqlCommand("UPDATE [User] SET usertype = 'Student' WHERE userid = @uid", con))
                        {
                            roleCmd.Parameters.AddWithValue("@uid", userId);
                            roleCmd.ExecuteNonQuery();
                        }



                        Session["usertype"] = "Student";
                        Response.Redirect("~/Student/BrowseCourses.aspx");
                        return;
                    }
                }

                // If it was a free course, stay on the page
                ShowMsg($"Successfully enrolled in {courseName}! You can access it in My Learning.", true);
                LoadCourses();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Enrollment Error: " + ex.Message);
                ShowMsg("An error occurred during enrollment. Please try again.", false);
            }
        }

        private void ShowMsg(string msg, bool success)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = "alert " + (success ? "alert-success" : "alert-error");
            lblMessage.Visible = true;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}