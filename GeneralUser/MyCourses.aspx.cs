using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class MyCourses : System.Web.UI.Page
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

                // Fetch enrolled courses
                string query = @"
                    SELECT c.courseid, c.coursename, c.category, e.enrolldate
                    FROM Enrollment e
                    INNER JOIN Course c ON e.courseid = c.courseid
                    WHERE e.userid = @uid AND e.isactive = 1
                    ORDER BY e.enrolldate DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Calculate real progress for each course
                    dt.Columns.Add("Progress", typeof(int));
                    foreach (DataRow row in dt.Rows)
                    {
                        int courseId = Convert.ToInt32(row["courseid"]);
                        int progress = 0;

                        try
                        {
                            // Count total lessons in this course
                            using (SqlCommand totalCmd = new SqlCommand(@"
                                SELECT COUNT(*) FROM Lesson l
                                INNER JOIN Module m ON l.moduleid = m.moduleid
                                WHERE m.courseid = @cid AND l.deletiontime IS NULL", con))
                            {
                                totalCmd.Parameters.AddWithValue("@cid", courseId);
                                int totalLessons = (int)totalCmd.ExecuteScalar();

                                if (totalLessons > 0)
                                {
                                    // Count completed lessons by this general user
                                    using (SqlCommand doneCmd = new SqlCommand(@"
                                        SELECT COUNT(*) FROM LessonProgress lp
                                        INNER JOIN Lesson l ON lp.lessonid = l.lessonid
                                        INNER JOIN Module m ON l.moduleid = m.moduleid
                                        WHERE m.courseid = @cid AND lp.userid = @uid AND lp.iscompleted = 1 AND l.deletiontime IS NULL", con))
                                    {
                                        doneCmd.Parameters.AddWithValue("@cid", courseId);
                                        doneCmd.Parameters.AddWithValue("@uid", userId);
                                        int completedLessons = (int)doneCmd.ExecuteScalar();
                                        progress = (completedLessons * 100) / totalLessons;
                                    }
                                }
                            }
                        }
                        catch { progress = 0; }

                        row["Progress"] = progress;
                    }

                    if (dt.Rows.Count > 0)
                    {
                        rptCourses.DataSource = dt;
                        rptCourses.DataBind();
                        pnlEmpty.Visible = false;
                        pnlCourses.Visible = true;
                    }
                    else
                    {
                        pnlCourses.Visible = false;
                        pnlEmpty.Visible = true;
                    }
                }
            }
        }

        protected void btnUnenrollConfirm_Click(object sender, EventArgs e)
        {
            string val = hfUnenrollId.Value;
            if (string.IsNullOrEmpty(val)) return;

            int courseId = Convert.ToInt32(val);
            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    // Set enrollment to inactive to preserve history (progress, forum posts, etc.)
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE Enrollment SET isactive = 0 WHERE userid = @uid AND courseid = @cid", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@cid", courseId);
                        cmd.ExecuteNonQuery();
                    }
                }

                hfUnenrollId.Value = "";
                lblMessage.Text = "Successfully unenrolled from the course.";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;

                LearnSphere_WAPP.Syslog.action(userId, "Unenrolled from course (CourseID: " + courseId + ")");

                LoadCourses(); // Refresh UI
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error during unenrollment: " + ex.Message);
                lblMessage.Text = "Failed to unenroll. Please try again.";
                lblMessage.CssClass = "alert alert-danger"; // Uses the red alert class
                lblMessage.Visible = true;
            }
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