using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class CourseContent : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null) { Response.Redirect("~/Login.aspx"); return; }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadUnreadMessagesBadge();
                LoadCourseStructure();

                if (Request.QueryString["lessonId"] != null)
                {
                    LoadLesson(Convert.ToInt32(Request.QueryString["lessonId"]));
                }
            }
        }

        private void LoadCourseStructure()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            int courseId = 0;

            if (Request.QueryString["lessonId"] != null)
                courseId = Convert.ToInt32(Request.QueryString["lessonId"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 1. Get Course Name (Using Invoice instead of Enrollment)
                string courseQuery = courseId > 0
                    ? "SELECT courseid, coursename FROM Course WHERE courseid = @cid"
                    : "SELECT TOP 1 c.courseid, c.coursename FROM Invoice i INNER JOIN Course c ON i.courseid = c.courseid WHERE i.userid = @uid";

                using (SqlCommand cmd = new SqlCommand(courseQuery, con))
                {
                    if (courseId > 0) cmd.Parameters.AddWithValue("@cid", courseId);
                    else cmd.Parameters.AddWithValue("@uid", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            courseId = Convert.ToInt32(reader["courseid"]);
                            lblCourseName.Text = reader["coursename"].ToString();
                        }
                        else
                        {
                            Response.Redirect("MyCourses.aspx");
                            return;
                        }
                    }
                }

                // 2. Load Modules & Lessons
                try
                {
                    string query = @"
                        SELECT m.moduleid, m.modulename,
                               l.lessonid, l.lessontitle,
                               CASE WHEN lp.progressid IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
                        FROM Module m
                        LEFT JOIN Lesson l ON m.moduleid = l.moduleid
                        LEFT JOIN LessonProgress lp ON l.lessonid = lp.lessonid AND lp.userid = @uid
                        WHERE m.courseid = @courseid
                        ORDER BY m.ordernumber, l.ordernumber";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                        da.SelectCommand.Parameters.AddWithValue("@courseid", courseId);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var modules = dt.AsEnumerable()
                                .GroupBy(row => new {
                                    ModuleId = row.Field<int>("moduleid"),
                                    ModuleName = row.Field<string>("modulename")
                                })
                                .Select(g => new {
                                    ModuleId = g.Key.ModuleId,
                                    ModuleName = g.Key.ModuleName,
                                    Lessons = g.Where(r => !r.IsNull("lessonid"))
                                        .Select(row => new {
                                            LessonId = row.Field<int>("lessonid"),
                                            LessonTitle = row.Field<string>("lessontitle"),
                                            IsCompleted = row.Field<int>("IsCompleted") == 1
                                        }).ToList()
                                }).ToList();

                            rptModules.DataSource = modules;
                            rptModules.DataBind();
                            pnlNoModules.Visible = false;
                            pnlModules.Visible = true;
                        }
                        else
                        {
                            pnlModules.Visible = false;
                            pnlNoModules.Visible = true;
                        }
                    }
                }
                catch { pnlModules.Visible = false; pnlNoModules.Visible = true; }
            }
        }

        private void LoadLesson(int lessonId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // 1. Load Title and Description
                    using (SqlCommand cmd = new SqlCommand("SELECT lessontitle, lessondescription FROM Lesson WHERE lessonid = @lid", con))
                    {
                        cmd.Parameters.AddWithValue("@lid", lessonId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblLessonTitle.Text = reader["lessontitle"].ToString();
                                lblLessonDesc.Text = reader["lessondescription"]?.ToString();
                            }
                            else return;
                        }
                    }

                    // 2. Load Materials
                    try
                    {
                        using (SqlCommand matCmd = new SqlCommand("SELECT materialid, filetype, fileurl, videourl FROM Material WHERE lessonid = @lid", con))
                        {
                            matCmd.Parameters.AddWithValue("@lid", lessonId);
                            using (SqlDataReader matReader = matCmd.ExecuteReader())
                            {
                                DataTable matTable = new DataTable();
                                matTable.Load(matReader);

                                var videoRow = matTable.AsEnumerable().FirstOrDefault(r => r["filetype"].ToString().ToLower() == "video" && !string.IsNullOrEmpty(r["videourl"]?.ToString()));
                                if (videoRow != null)
                                {
                                    iframeVideo.Attributes["src"] = videoRow["videourl"].ToString();
                                    pnlVideo.Visible = true;
                                    pnlNoVideo.Visible = false;
                                }
                                else
                                {
                                    pnlVideo.Visible = false;
                                    pnlNoVideo.Visible = true;
                                }

                                var fileRows = matTable.AsEnumerable().Where(r => r["filetype"].ToString().ToLower() != "video" && !string.IsNullOrEmpty(r["fileurl"]?.ToString())).ToList();
                                if (fileRows.Count > 0)
                                {
                                    rptMaterials.DataSource = fileRows.CopyToDataTable();
                                    rptMaterials.DataBind();
                                    pnlFiles.Visible = true;
                                }
                                else { pnlFiles.Visible = false; }
                            }
                        }
                    }
                    catch { pnlVideo.Visible = false; pnlNoVideo.Visible = true; pnlFiles.Visible = false; }

                    // 3. Check Completion Status
                    int userId = Convert.ToInt32(Session["userid"]);
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM LessonProgress WHERE userid=@uid AND lessonid=@lid AND iscompleted=1", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@lid", lessonId);
                        bool done = (int)chk.ExecuteScalar() > 0;
                        btnComplete.Enabled = !done;
                        btnComplete.Text = done ? "Already Completed" : "Mark as Completed";
                        btnComplete.CssClass = done ? "btn-complete disabled" : "btn-complete";
                    }

                    pnlLesson.Visible = true;
                    pnlSelectLesson.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading lesson: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["lessonId"] == null) return;
            int lessonId = Convert.ToInt32(Request.QueryString["lessonId"]);
            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM LessonProgress WHERE userid=@uid AND lessonid=@lid", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@lid", lessonId);
                        if ((int)chk.ExecuteScalar() > 0) return;
                    }

                    using (SqlCommand ins = new SqlCommand("INSERT INTO LessonProgress (userid,lessonid,iscompleted,completedtime) VALUES(@uid,@lid,1,@t)", con))
                    {
                        ins.Parameters.AddWithValue("@uid", userId);
                        ins.Parameters.AddWithValue("@lid", lessonId);
                        ins.Parameters.AddWithValue("@t", DateTime.Now);
                        ins.ExecuteNonQuery();
                    }

                    // REMOVED GAMIFICATION/POINTS LOGIC AS REQUESTED

                    lblMessage.Text = "Lesson completed!";
                    lblMessage.CssClass = "status-msg success";
                    lblMessage.Visible = true;
                    btnComplete.Enabled = false;
                    btnComplete.Text = "Already Completed";
                    btnComplete.CssClass = "btn-complete disabled";

                    LoadCourseStructure(); // Refresh sidebar UI
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.CssClass = "status-msg error";
                lblMessage.Visible = true;
            }
        }

        // --- Sidebar Helpers ---
        private void LoadSidebarProfileImage()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", Session["userid"]);
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