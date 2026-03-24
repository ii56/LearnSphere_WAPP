using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Student
{
    public partial class LessonViewer : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // Redirects unauthenticated users, sets the header name, then loads the course structure
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null) { Response.Redirect("~/Login.aspx"); return; }

            string displayName = GetDisplayName();
            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

            if (!IsPostBack)
            {
                LoadCourseStructure();

                if (Request.QueryString["lessonId"] != null)
                    LoadLesson(Convert.ToInt32(Request.QueryString["lessonId"]));
                else if (Request.QueryString["examid"] != null)
                    LoadExam(Convert.ToInt32(Request.QueryString["examid"]));
            }
        }

        // Fetches the student's first name from session or DB
        private string GetDisplayName()
        {
            if (Session["fname"] != null && Session["fname"].ToString() != "")
                return Session["fname"].ToString();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT fname FROM [User] WHERE userid=@uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", Convert.ToInt32(Session["userid"]));
                    object result = cmd.ExecuteScalar();
                    if (result != null) { Session["fname"] = result.ToString(); return result.ToString(); }
                }
            }
            return "Student";
        }

        // Builds the sidebar: modules, lessons (with completion status), module exams and the course exam
        private void LoadCourseStructure()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            int courseId = 0;
            if (Request.QueryString["courseid"] != null)
                courseId = Convert.ToInt32(Request.QueryString["courseid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Get the course name — fall back to the student's first enrolled course if no ID given
                string courseQuery = courseId > 0
                    ? "SELECT courseid, coursename FROM Course WHERE courseid=@cid"
                    : "SELECT TOP 1 c.courseid, c.coursename FROM Enrollment e INNER JOIN Course c ON e.courseid=c.courseid WHERE e.userid=@uid AND e.isactive=1";

                using (SqlCommand cmd = new SqlCommand(courseQuery, con))
                {
                    if (courseId > 0) cmd.Parameters.AddWithValue("@cid", courseId);
                    else cmd.Parameters.AddWithValue("@uid", userId);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            courseId = Convert.ToInt32(r["courseid"]);
                            lblCourseName.Text = r["coursename"].ToString();
                        }
                        else { Response.Redirect("MyCourses.aspx"); return; }
                    }
                }

                // Load modules + lessons + lesson completion in one query
                string q = @"
                    SELECT m.moduleid, m.modulename, m.ordernumber AS modorder,
                           l.lessonid, l.lessontitle, l.ordernumber AS lsnorder,
                           CASE WHEN lp.progressid IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
                    FROM Module m
                    LEFT JOIN Lesson l          ON m.moduleid  = l.moduleid  AND l.deletiontime IS NULL
                    LEFT JOIN LessonProgress lp ON l.lessonid  = lp.lessonid AND lp.userid=@uid AND lp.iscompleted=1
                    WHERE m.courseid=@courseid AND m.deletiontime IS NULL
                    ORDER BY m.ordernumber, l.ordernumber";

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(q, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                    da.SelectCommand.Parameters.AddWithValue("@courseid", courseId);
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    pnlModules.Visible = false;
                    pnlNoModules.Visible = true;
                    return;
                }

                // Group lessons under their modules and look up module exams
                var modules = dt.AsEnumerable()
                    .GroupBy(row => new { ModuleId = row.Field<int>("moduleid"), ModuleName = row.Field<string>("modulename") })
                    .Select(g =>
                    {
                        int moduleId = g.Key.ModuleId;

                        // Find module exam if one exists
                        int examId = 0;
                        bool examPassed = false;
                        bool examAttempted = false;

                        using (SqlCommand ec = new SqlCommand(
                            "SELECT examid FROM Exam WHERE moduleid=@mid AND deletiontime IS NULL", con))
                        {
                            ec.Parameters.AddWithValue("@mid", moduleId);
                            object eid = ec.ExecuteScalar();
                            if (eid != null)
                            {
                                examId = Convert.ToInt32(eid);
                                GetExamStatus(con, examId, userId, out examPassed, out examAttempted);
                            }
                        }

                        return new
                        {
                            ModuleId = moduleId,
                            ModuleName = g.Key.ModuleName,
                            ExamId = examId,
                            ExamPassed = examPassed,
                            ExamAttempted = examAttempted,
                            Lessons = g.Where(r => !r.IsNull("lessonid"))
                                .Select(row => new
                                {
                                    LessonId = row.Field<int>("lessonid"),
                                    LessonTitle = row.Field<string>("lessontitle"),
                                    IsCompleted = row.Field<int>("IsCompleted") == 1
                                }).ToList()
                        };
                    }).ToList();

                rptModules.DataSource = modules;
                rptModules.DataBind();
                pnlNoModules.Visible = false;
                pnlModules.Visible = true;

                // Check for a course-level exam and store its state for the sidebar link
                using (SqlCommand ce = new SqlCommand(
                    "SELECT examid FROM Exam WHERE courseid=@cid AND moduleid IS NULL AND deletiontime IS NULL", con))
                {
                    ce.Parameters.AddWithValue("@cid", courseId);
                    object ceid = ce.ExecuteScalar();
                    if (ceid != null)
                    {
                        int ceId = Convert.ToInt32(ceid);
                        bool cePassed = false;
                        bool ceAttempted = false;
                        GetExamStatus(con, ceId, userId, out cePassed, out ceAttempted);

                        ViewState["CourseExamId"] = ceId;
                        ViewState["CourseExamPassed"] = cePassed;
                        ViewState["CourseExamAttempted"] = ceAttempted;

                        // Set the HyperLink properties here where ViewState is accessible
                        hlCourseExam.NavigateUrl = string.Format(
                            "LessonViewer.aspx?courseid={0}&examid={1}", courseId, ceId);

                        string activeCss = (Request.QueryString["examid"] == ceId.ToString()) ? " active" : "";
                        string passedCss = cePassed ? " passed" : "";
                        hlCourseExam.CssClass = "exam-link course-exam" + activeCss + passedCss;

                        string badgeClass = cePassed ? "badge-passed" : ceAttempted ? "badge-failed" : "badge-new";
                        string badgeText = cePassed ? "Passed" : ceAttempted ? "Retry" : "New";
                        lblCourseExamBadge.Text = string.Format(
                            "<span class='exam-badge {0}'>{1}</span>", badgeClass, badgeText);

                        pnlCourseExamLink.Visible = true;
                    }
                }
            }
        }

        // Checks whether the student has passed or merely attempted a given exam
        private void GetExamStatus(SqlConnection con, int examId, int userId, out bool passed, out bool attempted)
        {
            passed = false;
            attempted = false;

            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 er.score, e.totalmarks
                FROM ExamResult er
                INNER JOIN Exam e ON er.examid = e.examid
                WHERE er.examid=@eid AND er.userid=@uid
                ORDER BY er.attempttime DESC", con))
            {
                cmd.Parameters.AddWithValue("@eid", examId);
                cmd.Parameters.AddWithValue("@uid", userId);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        attempted = true;
                        int score = r.IsDBNull(0) ? 0 : r.GetInt32(0);
                        int totalmarks = r.GetInt32(1);
                        passed = totalmarks > 0 && (score * 100 / totalmarks) >= 80;
                    }
                }
            }
        }

        // Loads a lesson's title, description, video and downloadable materials into the content panel
        private void LoadLesson(int lessonId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT lessontitle, lessondescription FROM Lesson WHERE lessonid=@lid", con))
                    {
                        cmd.Parameters.AddWithValue("@lid", lessonId);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return;
                            lblLessonTitle.Text = r["lessontitle"].ToString();
                            lblLessonDesc.Text = r["lessondescription"]?.ToString() ?? "";
                        }
                    }

                    // Load materials — video goes into the iframe, files go into the repeater
                    using (SqlCommand mc = new SqlCommand(
                        "SELECT materialid, filetype, fileurl, videourl FROM Material WHERE lessonid=@lid", con))
                    {
                        mc.Parameters.AddWithValue("@lid", lessonId);
                        DataTable mt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(mc)) da.Fill(mt);

                        // Video: check videourl column first, then filetype='URL'
                        var videoRow = mt.AsEnumerable().FirstOrDefault(r =>
                            !string.IsNullOrEmpty(r["videourl"]?.ToString()));
                        if (videoRow != null)
                        {
                            string raw = videoRow["videourl"].ToString();
                            iframeVideo.Attributes["src"] = ConvertToEmbed(raw);
                            pnlVideo.Visible = true;
                            pnlNoVideo.Visible = false;
                        }
                        else
                        {
                            pnlVideo.Visible = false;
                            pnlNoVideo.Visible = true;
                        }

                        // Downloadable files
                        var fileRows = mt.AsEnumerable()
                            .Where(r => !string.IsNullOrEmpty(r["fileurl"]?.ToString()))
                            .ToList();
                        if (fileRows.Count > 0)
                        {
                            rptMaterials.DataSource = fileRows.CopyToDataTable();
                            rptMaterials.DataBind();
                            pnlFiles.Visible = true;
                        }
                        else { pnlFiles.Visible = false; }
                    }

                    // Show the Complete button — disable it if already completed
                    int userId = Convert.ToInt32(Session["userid"]);
                    using (SqlCommand chk = new SqlCommand(
                        "SELECT COUNT(*) FROM LessonProgress WHERE userid=@uid AND lessonid=@lid AND iscompleted=1", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@lid", lessonId);
                        bool done = (int)chk.ExecuteScalar() > 0;
                        btnComplete.Enabled = !done;
                        btnComplete.Text = done ? "✓ Already Completed" : "✓ Mark as Completed";
                    }
                }

                pnlLesson.Visible = true;
                pnlSelectLesson.Visible = false;
                pnlExam.Visible = false;
                pnlExamResult.Visible = false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading lesson: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        // Renders the A/B/C/D radio option labels for a question — called from the repeater in the ASPX
        // Using a helper avoids multi-line expressions inside <%# %> which the ASPX parser rejects
        protected string RenderOptions(object qid, object a, object b, object c, object d)
        {
            string name = "q_" + qid;
            var sb = new System.Text.StringBuilder();

            sb.AppendFormat(
                "<label class='option-label'><input type='radio' name='{0}' value='A' required /><span class='option-key'>A</span>{1}</label>",
                name, Server.HtmlEncode(a?.ToString() ?? ""));

            sb.AppendFormat(
                "<label class='option-label'><input type='radio' name='{0}' value='B' /><span class='option-key'>B</span>{1}</label>",
                name, Server.HtmlEncode(b?.ToString() ?? ""));

            if (!string.IsNullOrEmpty(c?.ToString()))
                sb.AppendFormat(
                    "<label class='option-label'><input type='radio' name='{0}' value='C' /><span class='option-key'>C</span>{1}</label>",
                    name, Server.HtmlEncode(c.ToString()));

            if (!string.IsNullOrEmpty(d?.ToString()))
                sb.AppendFormat(
                    "<label class='option-label'><input type='radio' name='{0}' value='D' /><span class='option-key'>D</span>{1}</label>",
                    name, Server.HtmlEncode(d.ToString()));

            return sb.ToString();
        }

        // Converts a YouTube watch URL to an embed URL so it works inside an iframe
        private string ConvertToEmbed(string url)
        {
            if (url.Contains("watch?v="))
                return "https://www.youtube.com/embed/" + url.Split('=')[1];
            return url;
        }

        // Loads an exam's questions into the exam panel — or shows the previous result if already attempted
        private void LoadExam(int examId)
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Fetch exam metadata
                DataRow examRow = null;
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT examid, examtitle, totalmarks, exampoints,
                           CASE WHEN moduleid IS NOT NULL THEN 'module' ELSE 'course' END AS examtype
                    FROM Exam WHERE examid=@eid AND deletiontime IS NULL", con))
                {
                    cmd.Parameters.AddWithValue("@eid", examId);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
                    if (dt.Rows.Count == 0) return;
                    examRow = dt.Rows[0];
                }

                // Check if already attempted — show previous result if so
                bool passed, attempted;
                GetExamStatus(con, examId, userId, out passed, out attempted);

                if (attempted)
                {
                    ShowExamResult(con, examId, userId,
                        Convert.ToInt32(examRow["totalmarks"]),
                        examRow["exampoints"] != DBNull.Value ? Convert.ToInt32(examRow["exampoints"]) : 0,
                        examRow["examtitle"].ToString(),
                        examRow["examtype"].ToString(),
                        isNewSubmission: false);
                    return;
                }

                // Load questions for fresh attempt
                DataTable questions = new DataTable();
                using (SqlCommand qCmd = new SqlCommand(@"
                    SELECT questionid, questiontext, optionA, optionB, optionC, optionD, marks
                    FROM ExamQuestion WHERE examid=@eid ORDER BY ordernumber", con))
                {
                    qCmd.Parameters.AddWithValue("@eid", examId);
                    using (SqlDataAdapter da = new SqlDataAdapter(qCmd)) da.Fill(questions);
                }

                // Populate exam header
                bool isCourse = examRow["examtype"].ToString() == "course";
                lblExamTag.Text = string.Format(
                    "<span class='exam-tag {0}'>{1}</span>",
                    isCourse ? "course" : "module",
                    isCourse ? "📋 Course Exam" : "📝 Module Exam");
                lblExamTitle.Text = Server.HtmlEncode(examRow["examtitle"].ToString());
                lblQuestionCount.Text = questions.Rows.Count.ToString();
                lblExamPoints.Text = (examRow["exampoints"] != DBNull.Value ? examRow["exampoints"].ToString() : "0");

                rptQuestions.DataSource = questions;
                rptQuestions.DataBind();

                // Store exam metadata in ViewState so the submit handler can access it
                ViewState["ExamId"] = examId;
                ViewState["ExamTotalMarks"] = Convert.ToInt32(examRow["totalmarks"]);
                ViewState["ExamPoints"] = examRow["exampoints"] != DBNull.Value ? Convert.ToInt32(examRow["exampoints"]) : 0;
                ViewState["ExamTitle"] = examRow["examtitle"].ToString();
                ViewState["ExamType"] = examRow["examtype"].ToString();

                pnlExam.Visible = true;
                pnlLesson.Visible = false;
                pnlExamResult.Visible = false;
                pnlSelectLesson.Visible = false;
            }
        }

        // Marks a lesson complete, awards points, and refreshes the sidebar
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

                    // Guard against double-completion
                    using (SqlCommand chk = new SqlCommand(
                        "SELECT COUNT(*) FROM LessonProgress WHERE userid=@uid AND lessonid=@lid", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@lid", lessonId);
                        if ((int)chk.ExecuteScalar() > 0) return;
                    }

                    // Record completion
                    using (SqlCommand ins = new SqlCommand(
                        "INSERT INTO LessonProgress (userid,lessonid,iscompleted,completedtime) VALUES(@uid,@lid,1,@t)", con))
                    {
                        ins.Parameters.AddWithValue("@uid", userId);
                        ins.Parameters.AddWithValue("@lid", lessonId);
                        ins.Parameters.AddWithValue("@t", DateTime.Now);
                        ins.ExecuteNonQuery();
                    }

                    // Get how many points this lesson awards (falls back to 10 if not set)
                    int pts = 10;
                    using (SqlCommand lp = new SqlCommand(
                        "SELECT ISNULL(lessonpoints, 10) FROM Lesson WHERE lessonid=@lid", con))
                    {
                        lp.Parameters.AddWithValue("@lid", lessonId);
                        object r = lp.ExecuteScalar();
                        if (r != null) pts = Convert.ToInt32(r);
                    }

                    // Add points and update badge in one go
                    AwardPoints(con, userId, pts);
                }

                lblMessage.Text = $"✓ Lesson completed! You earned {10} points.";
                lblMessage.Visible = true;
                btnComplete.Enabled = false;
                btnComplete.Text = "✓ Already Completed";
                LearnSphere_WAPP.Syslog.action(userId, "Completed Lesson (LessonID: " + lessonId + ")");
                LoadCourseStructure();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        // Collects all radio button answers, scores the exam, stores the result and awards points if passed
        protected void btnSubmitExam_Click(object sender, EventArgs e)
        {
            if (ViewState["ExamId"] == null) return;

            int examId = Convert.ToInt32(ViewState["ExamId"]);
            int totalMarks = Convert.ToInt32(ViewState["ExamTotalMarks"]);
            int examPoints = Convert.ToInt32(ViewState["ExamPoints"]);
            int userId = Convert.ToInt32(Session["userid"]);

            // Fetch correct answers from DB
            DataTable correctAnswers = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT questionid, correctanswer, marks FROM ExamQuestion WHERE examid=@eid ORDER BY ordernumber", con))
                {
                    cmd.Parameters.AddWithValue("@eid", examId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(correctAnswers);
                }
            }

            // Read answers from Request.Form — each radio group is named q_{questionid}
            int score = 0;
            bool allAnswered = true;

            foreach (DataRow row in correctAnswers.Rows)
            {
                int questionId = Convert.ToInt32(row["questionid"]);
                string formKey = "q_" + questionId;
                string chosen = Request.Form[formKey];

                if (string.IsNullOrEmpty(chosen))
                {
                    allAnswered = false;
                    continue;
                }

                string correct = row["correctanswer"].ToString().Trim().ToUpper();
                int marks = Convert.ToInt32(row["marks"]);
                if (chosen.Trim().ToUpper() == correct)
                    score += marks;
            }

            if (!allAnswered)
            {
                lblExamError.Text = "Please answer all questions before submitting.";
                lblExamError.Visible = true;
                // Reload the exam so the student can finish answering
                LoadExam(examId);
                return;
            }

            // Store the result and award points if passed
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                using (SqlCommand ins = new SqlCommand(
                    "INSERT INTO ExamResult (examid, userid, score, attempttime) VALUES(@eid,@uid,@score,@t)", con))
                {
                    ins.Parameters.AddWithValue("@eid", examId);
                    ins.Parameters.AddWithValue("@uid", userId);
                    ins.Parameters.AddWithValue("@score", score);
                    ins.Parameters.AddWithValue("@t", DateTime.Now);
                    ins.ExecuteNonQuery();
                }

                // Award exam points only if the student passed (≥80%)
                bool passed = totalMarks > 0 && (score * 100 / totalMarks) >= 80;
                if (passed && examPoints > 0)
                    AwardPoints(con, userId, examPoints);

                LearnSphere_WAPP.Syslog.action(userId, $"Submitted Exam (ExamID: {examId}, Score: {score}/{totalMarks})");

                ShowExamResult(con, examId, userId, totalMarks, examPoints,
                    ViewState["ExamTitle"].ToString(),
                    ViewState["ExamType"].ToString(),
                    isNewSubmission: true,
                    justScoredResult: score);
            }

            LoadCourseStructure();
        }

        // Renders the result banner and stats — works for both a new submission and viewing a past attempt
        private void ShowExamResult(SqlConnection con, int examId, int userId,
            int totalMarks, int examPoints, string examTitle, string examType,
            bool isNewSubmission, int justScoredResult = -1)
        {
            int score;
            if (justScoredResult >= 0)
            {
                score = justScoredResult;
            }
            else
            {
                // Load the most recent attempt from the DB
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT TOP 1 score FROM ExamResult WHERE examid=@eid AND userid=@uid ORDER BY attempttime DESC", con))
                {
                    cmd.Parameters.AddWithValue("@eid", examId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    object r = cmd.ExecuteScalar();
                    score = r != null ? Convert.ToInt32(r) : 0;
                }
            }

            int percent = totalMarks > 0 ? (score * 100 / totalMarks) : 0;
            bool passed = percent >= 80;
            string passClass = passed ? "passed" : "failed";
            string icon = passed ? "🎉" : "😔";
            string title = passed ? "Congratulations! You Passed!" : "Not Quite There Yet";
            string sub = passed
                ? $"You scored {percent}% — above the 80% pass mark."
                : $"You scored {percent}% — you need 80% to pass. Keep studying!";

            pnlResultBanner.Controls.Clear();
            pnlResultBanner.Controls.Add(new System.Web.UI.LiteralControl(
                $@"<div class='result-banner {passClass}'>
                    <div class='result-icon'>{icon}</div>
                    <div class='result-title'>{title}</div>
                    <div class='result-sub'>{sub}</div>
                   </div>"));

            lblResultScore.Text = score.ToString();
            lblResultTotal.Text = totalMarks.ToString();
            lblResultPercent.Text = percent + "%";
            lblResultPoints.Text = passed ? examPoints.ToString() : "0";

            if (isNewSubmission && passed && examPoints > 0)
            {
                lblResultMsg.Text = $"⚡ You earned {examPoints} bonus points for passing this exam!";
                lblResultMsg.Visible = true;
            }

            pnlExamResult.Visible = true;
            pnlExam.Visible = false;
            pnlLesson.Visible = false;
            pnlSelectLesson.Visible = false;
        }

        // Updates the student's total points and recalculates their badge tier
        private void AwardPoints(SqlConnection con, int userId, int points)
        {
            // Only update if a StudentPoints row exists — the gamification system creates it on first login
            using (SqlCommand pts = new SqlCommand(@"
                UPDATE StudentPoints
                SET totalpoints = totalpoints + @pts, lastupdated = @t
                WHERE userid = @uid", con))
            {
                pts.Parameters.AddWithValue("@pts", points);
                pts.Parameters.AddWithValue("@t", DateTime.Now);
                pts.Parameters.AddWithValue("@uid", userId);
                pts.ExecuteNonQuery();
            }

            // Recalculate badge based on the new total
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
        }

        // Clears the session and sends the student back to the login page
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}