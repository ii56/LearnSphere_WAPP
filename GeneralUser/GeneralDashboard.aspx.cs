using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class GeneralDashboard : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["userid"] = 9;
            Session["usertype"] = "General";
            // Security check - Must be logged in and must be a General User
            if (Session["userid"] == null || Session["usertype"] == null || Session["usertype"].ToString() != "General")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 1. Fetch User Info (Name and Avatar)
                string displayName = "User";
                using (SqlCommand nameCmd = new SqlCommand("SELECT fname, ProfileImage FROM [User] WHERE userid = @uid", con))
                {
                    nameCmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = nameCmd.ExecuteReader())
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

                lblWelcome.Text = displayName;
                lblHeaderName.Text = displayName;

                // 2. Fetch Notifications (Lecturer Upgrade Request Status)
                using (SqlCommand notifCmd = new SqlCommand(@"
                    SELECT TOP 1 status, remarks 
                    FROM VerificationRequest 
                    WHERE userid = @uid 
                    ORDER BY requesttime DESC", con))
                {
                    notifCmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader nReader = notifCmd.ExecuteReader())
                    {
                        if (nReader.Read())
                        {
                            string status = nReader["status"].ToString();
                            string remarks = nReader["remarks"] != DBNull.Value ? nReader["remarks"].ToString() : "";

                            pnlNotification.Visible = true;

                            if (status == "Pending")
                            {
                                notificationWrapper.Attributes["class"] = "alert-notification pending";
                                litNotificationIcon.Text = "⏳";
                                litNotificationText.Text = "Your request to upgrade to Lecturer is currently under review by an administrator.";
                            }
                            else if (status == "Approved")
                            {
                                notificationWrapper.Attributes["class"] = "alert-notification approved";
                                litNotificationIcon.Text = "✅";
                                litNotificationText.Text = "Your request was approved! Please log out and log back in to access the Lecturer Portal.";
                            }
                            else if (status == "Rejected" || status == "Denied")
                            {
                                notificationWrapper.Attributes["class"] = "alert-notification denied";
                                litNotificationIcon.Text = "❌";
                                litNotificationText.Text = "Your request was denied. " + (string.IsNullOrEmpty(remarks) ? "" : "Reason: " + remarks);
                            }
                        }
                    }
                }

                // 3. Fetch Basic Stats (Courses Enrolled & Completed Lessons)
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Enrollment WHERE userid = @uid AND isactive = 1", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    lblEnrolled.Text = cmd.ExecuteScalar().ToString();
                }

                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM LessonProgress WHERE userid = @uid AND iscompleted = 1", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        lblCompleted.Text = cmd.ExecuteScalar().ToString();
                    }
                }
                catch { lblCompleted.Text = "0"; }

                // 4. Fetch Gamification Data (Points and Badge)
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT totalpoints, badge FROM StudentPoints WHERE userid = @uid", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblPoints.Text = reader["totalpoints"].ToString();
                                lblBadge.Text = reader["badge"] != DBNull.Value ? reader["badge"].ToString() : "Bronze";
                            }
                            else
                            {
                                // Initialize gamification record if it doesn't exist
                                lblPoints.Text = "0";
                                lblBadge.Text = "Bronze";
                                using (SqlCommand ins = new SqlCommand("INSERT INTO StudentPoints (userid, totalpoints, badge, lastupdated) VALUES (@uid, 0, 'Bronze', GETDATE())", con))
                                {
                                    ins.Parameters.AddWithValue("@uid", userId);
                                    ins.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch
                {
                    lblPoints.Text = "0";
                    lblBadge.Text = "Bronze";
                }

                // 5. Load Enrolled Courses with Progress
                string enrolledQuery = @"
                    SELECT c.courseid, c.coursename, c.category, e.enrolldate
                    FROM Course c
                    INNER JOIN Enrollment e ON c.courseid = e.courseid
                    WHERE e.userid = @uid AND e.isactive = 1 AND c.status = 'Active'
                    ORDER BY e.enrolldate DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(enrolledQuery, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                    DataTable dtEnrolled = new DataTable();
                    da.Fill(dtEnrolled);

                    dtEnrolled.Columns.Add("Progress", typeof(int));
                    foreach (DataRow row in dtEnrolled.Rows)
                    {
                        int courseId = Convert.ToInt32(row["courseid"]);
                        int progress = 0;
                        try
                        {
                            using (SqlCommand totalCmd = new SqlCommand("SELECT COUNT(*) FROM Lesson l INNER JOIN Module m ON l.moduleid = m.moduleid WHERE m.courseid = @cid AND l.deletiontime IS NULL", con))
                            {
                                totalCmd.Parameters.AddWithValue("@cid", courseId);
                                int totalLessons = (int)totalCmd.ExecuteScalar();

                                if (totalLessons > 0)
                                {
                                    using (SqlCommand doneCmd = new SqlCommand("SELECT COUNT(*) FROM LessonProgress lp INNER JOIN Lesson l ON lp.lessonid = l.lessonid INNER JOIN Module m ON l.moduleid = m.moduleid WHERE m.courseid = @cid AND lp.userid = @uid AND lp.iscompleted = 1", con))
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

                    if (dtEnrolled.Rows.Count > 0)
                    {
                        rptCourses.DataSource = dtEnrolled;
                        rptCourses.DataBind();
                        pnlCourses.Visible = true;
                        pnlEmptyCourses.Visible = false;
                    }
                    else
                    {
                        pnlCourses.Visible = false;
                        pnlEmptyCourses.Visible = true;
                    }
                }

                // 6. Load Recommended Courses (Top 4 published courses the user is NOT enrolled in)
                string recommendQuery = @"
                    SELECT TOP 4 courseid, coursename, description, category, price
                    FROM Course
                    WHERE status = 'Active' AND deletiontime IS NULL
                    AND courseid NOT IN (SELECT courseid FROM Enrollment WHERE userid = @uid AND isactive = 1)
                    ORDER BY price ASC";

                using (SqlDataAdapter daRec = new SqlDataAdapter(recommendQuery, con))
                {
                    daRec.SelectCommand.Parameters.AddWithValue("@uid", userId);
                    DataTable dtRec = new DataTable();
                    daRec.Fill(dtRec);

                    if (dtRec.Rows.Count > 0)
                    {
                        rptRecommended.DataSource = dtRec;
                        rptRecommended.DataBind();
                        pnlRecommended.Visible = true;
                        pnlEmptyRecommended.Visible = false;
                    }
                    else
                    {
                        pnlRecommended.Visible = false;
                        pnlEmptyRecommended.Visible = true;
                    }
                }
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