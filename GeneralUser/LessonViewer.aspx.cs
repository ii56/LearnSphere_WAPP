using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class LessonViewer : System.Web.UI.Page
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
                LoadSidebarProfileImage();
                LoadCourseStructure();

                if (Request.QueryString["lessonId"] != null)
                {
                    LoadLesson(Convert.ToInt32(Request.QueryString["lessonId"]));
                }
                else if (Request.QueryString["examId"] != null)
                {
                    LoadExamIntro(Convert.ToInt32(Request.QueryString["examId"]));
                }
            }
        }

        private void LoadSidebarProfileImage()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string displayName = "General User";
                using (SqlCommand cmd = new SqlCommand("SELECT fname, ProfileImage FROM [User] WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            displayName = reader["fname"].ToString();
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
                Session["fname"] = displayName;
            }
        }

        private void LoadCourseStructure()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            int courseId=0;

            if (Request.QueryString["courseid"] != null)
                courseId = Convert.ToInt32(Session["courseid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 1. Get Course Name (Ensuring they are enrolled via Enrollment table)
                string courseQuery = courseId > 0
                    ? "SELECT c.courseid, c.coursename FROM Course c INNER JOIN Enrollment e ON c.courseid = e.courseid WHERE c.courseid = @cid AND e.userid = @uid AND e.isactive = 1"
                    : "SELECT TOP 1 c.courseid, c.coursename FROM Enrollment e INNER JOIN Course c ON e.courseid = c.courseid WHERE e.userid = @uid AND e.isactive = 1";

                using (SqlCommand cmd = new SqlCommand(courseQuery, con))
                {
                    if (courseId > 0) cmd.Parameters.AddWithValue("@cid", courseId);
                    cmd.Parameters.AddWithValue("@uid", userId);

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

                // 2. Load Modules, Lessons, and Exams
                try
                {
                    // Fetch Modules and Lessons
                    string lessonQuery = @"
                        SELECT m.moduleid, m.modulename,
                               l.lessonid, l.lessontitle,
                               CASE WHEN lp.progressid IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
                        FROM Module m
                        LEFT JOIN Lesson l ON m.moduleid = l.moduleid AND l.deletiontime IS NULL
                        LEFT JOIN LessonProgress lp ON l.lessonid = lp.lessonid AND lp.userid = @uid
                        WHERE m.courseid = @courseid AND m.deletiontime IS NULL
                        ORDER BY m.ordernumber, l.ordernumber";

                    DataTable dtLessons = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(lessonQuery, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                        da.SelectCommand.Parameters.AddWithValue("@courseid", courseId);
                        da.Fill(dtLessons);
                    }

                    // Fetch Exams (Both Module-level and Course-level)
                    string examQuery = @"
                        SELECT e.examid, e.examtitle, e.moduleid, e.courseid,
                               CASE WHEN er.resultid IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
                        FROM Exam e
                        LEFT JOIN ExamResult er ON e.examid = er.examid AND er.userid = @uid
                        WHERE (e.courseid = @courseid) OR (e.moduleid IN (SELECT moduleid FROM Module WHERE courseid = @courseid))
                        AND e.deletiontime IS NULL";

                    DataTable dtExams = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(examQuery, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                        da.SelectCommand.Parameters.AddWithValue("@courseid", courseId);
                        da.Fill(dtExams);
                    }

                    if (dtLessons.Rows.Count > 0 || dtExams.Rows.Count > 0)
                    {
                        // Group into Modules containing both Lessons and Exams
                        var modules = dtLessons.AsEnumerable()
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
                                    }).ToList(),
                                Exams = dtExams.AsEnumerable()
                                    .Where(eRow => !eRow.IsNull("moduleid") && eRow.Field<int>("moduleid") == g.Key.ModuleId)
                                    .Select(eRow => new {
                                        ExamId = eRow.Field<int>("examid"),
                                        ExamTitle = eRow.Field<string>("examtitle"),
                                        IsCompleted = eRow.Field<int>("IsCompleted") == 1
                                    }).ToList()
                            }).ToList();

                        rptModules.DataSource = modules;
                        rptModules.DataBind();

                        // Bind Course-level Exams (moduleid IS NULL)
                        var courseExams = dtExams.AsEnumerable()
                            .Where(eRow => eRow.IsNull("moduleid"))
                            .Select(eRow => new {
                                ExamId = eRow.Field<int>("examid"),
                                ExamTitle = eRow.Field<string>("examtitle"),
                                IsCompleted = eRow.Field<int>("IsCompleted") == 1
                            }).ToList();

                        if (courseExams.Count > 0)
                        {
                            rptCourseExams.DataSource = courseExams;
                            rptCourseExams.DataBind();
                        }

                        pnlNoModules.Visible = false;
                        pnlModules.Visible = true;
                    }
                    else
                    {
                        pnlModules.Visible = false;
                        pnlNoModules.Visible = true;
                    }
                }
                catch
                {
                    pnlModules.Visible = false;
                    pnlNoModules.Visible = true;
                }
            }
        }

        private void LoadLesson(int lessonId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT lessontitle, lessondescription FROM Lesson WHERE lessonid = @lid", con))
                    {
                        cmd.Parameters.AddWithValue("@lid", lessonId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblLessonTitle.Text = reader["lessontitle"].ToString();
                                lblLessonDesc.Text = reader["lessondescription"] != DBNull.Value ? reader["lessondescription"].ToString().Replace("\n", "<br/>") : "";
                            }
                            else return;
                        }
                    }

                    // Load Materials
                    try
                    {
                        using (SqlCommand matCmd = new SqlCommand("SELECT filetype, fileurl, videourl FROM Material WHERE lessonid = @lid", con))
                        {
                            matCmd.Parameters.AddWithValue("@lid", lessonId);
                            using (SqlDataReader matReader = matCmd.ExecuteReader())
                            {
                                DataTable matTable = new DataTable();
                                matTable.Load(matReader);

                                var videoRow = matTable.AsEnumerable()
                                    .FirstOrDefault(r => r["filetype"].ToString().ToLower() == "video"
                                        || r["filetype"].ToString().ToLower() == "url"
                                        && !string.IsNullOrEmpty(r["videourl"]?.ToString()));

                                if (videoRow != null)
                                {
                                    iframeVideo.Attributes["src"] = ConvertToEmbed(videoRow["videourl"].ToString());
                                    pnlVideo.Visible = true;
                                    pnlNoVideo.Visible = false;
                                }
                                else
                                {
                                    pnlVideo.Visible = false;
                                    pnlNoVideo.Visible = true;
                                }

                                var fileRows = matTable.AsEnumerable()
                                    .Where(r => r["filetype"].ToString().ToLower() != "video" && r["filetype"].ToString().ToLower() != "url"
                                        && !string.IsNullOrEmpty(r["fileurl"]?.ToString()))
                                    .ToList();

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

                    // Gamification Check
                    int userId = Convert.ToInt32(Session["userid"]);
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM LessonProgress WHERE userid=@uid AND lessonid=@lid AND iscompleted=1", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@lid", lessonId);
                        bool done = (int)chk.ExecuteScalar() > 0;
                        btnComplete.Enabled = !done;
                        btnComplete.Text = done ? "Already Completed" : "Mark as Completed (+10 Points)";
                    }

                    pnlLesson.Visible = true;
                    pnlExamIntro.Visible = false;
                    pnlSelectLesson.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading lesson: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        private void LoadExamIntro(int examId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT examtitle, durationminutes, totalmarks FROM Exam WHERE examid = @eid AND deletiontime IS NULL", con))
                    {
                        cmd.Parameters.AddWithValue("@eid", examId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblExamTitle.Text = reader["examtitle"].ToString();
                                lblExamDuration.Text = reader["durationminutes"] != DBNull.Value ? reader["durationminutes"].ToString() : "N/A";
                                lblExamMarks.Text = reader["totalmarks"].ToString();
                            }
                            else return;
                        }
                    }

                    // Check if exam is completed
                    int userId = Convert.ToInt32(Session["userid"]);
                    using (SqlCommand chk = new SqlCommand("SELECT score FROM ExamResult WHERE userid=@uid AND examid=@eid", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@eid", examId);
                        object scoreObj = chk.ExecuteScalar();

                        if (scoreObj != null)
                        {
                            btnStartExam.Enabled = false;
                            btnStartExam.Text = "Exam Completed";
                            lblExamMessage.Text = "You have already completed this exam. Your score: <strong>" + scoreObj.ToString() + " / " + lblExamMarks.Text + "</strong>";
                            lblExamMessage.Visible = true;
                        }
                        else
                        {
                            btnStartExam.Enabled = true;
                            btnStartExam.Text = "Start Exam Now →";
                            lblExamMessage.Visible = false;
                        }
                    }
                }

                pnlLesson.Visible = false;
                pnlExamIntro.Visible = true;
                pnlSelectLesson.Visible = false;
            }
            catch (Exception ex)
            {
                lblExamMessage.Text = "Error loading exam details: " + ex.Message;
                lblExamMessage.Visible = true;
                lblExamMessage.CssClass = "alert-error";
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

                    // Gamification Update
                    using (SqlCommand pts = new SqlCommand(@"
                        UPDATE StudentPoints SET totalpoints = totalpoints + 10, lastupdated = @t WHERE userid = @uid;
                        IF @@ROWCOUNT = 0
                            INSERT INTO StudentPoints (userid, totalpoints, badge, lastupdated) VALUES (@uid, 10, 'Bronze', @t);
                        ", con))
                    {
                        pts.Parameters.AddWithValue("@uid", userId);
                        pts.Parameters.AddWithValue("@t", DateTime.Now);
                        pts.ExecuteNonQuery();
                    }

                    // Update Badge
                    using (SqlCommand bdg = new SqlCommand(@"
                        UPDATE StudentPoints SET badge =
                            CASE WHEN totalpoints >= 600 THEN 'Diamond'
                                 WHEN totalpoints >= 300 THEN 'Gold'
                                 WHEN totalpoints >= 100 THEN 'Silver'
                                 ELSE 'Bronze' END
                        WHERE userid = @uid", con))
                    {
                        bdg.Parameters.AddWithValue("@uid", userId);
                        bdg.ExecuteNonQuery();
                    }

                    lblMessage.Text = "Lesson completed! You earned 10 points!";
                    lblMessage.Visible = true;
                    btnComplete.Enabled = false;
                    btnComplete.Text = "Already Completed";

                    LearnSphere_WAPP.Syslog.action(userId, "Completed Lesson (LessonID: " + lessonId + ")");
                    LoadCourseStructure(); // Refresh sidebar UI dots
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
                lblMessage.CssClass = "alert-error";
            }
        }

        protected void btnStartExam_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["examId"] != null)
            {
                // Redirects to an AttemptExam page (Ensure you build this page subsequently)
                Response.Redirect("AttemptExam.aspx?examid=" + Request.QueryString["examId"]);
            }
        }

        private string ConvertToEmbed(string url)
        {
            if (url.Contains("watch?v="))
            {
                string id = url.Split('=')[1].Split('&')[0];
                return "https://www.youtube.com/embed/" + id;
            }
            return url;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}