using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class Preview : System.Web.UI.Page
    {
        string connStr = ConfigurationManager
            .ConnectionStrings["LearnSphereDB"]
            .ConnectionString;

        // ══════════════════════════════════════════════════════════════════════
        // PAGE LOAD
        // ══════════════════════════════════════════════════════════════════════
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int courseId;
                if (!int.TryParse(Request.QueryString["courseid"], out courseId))
                {
                    Response.Redirect("ViewCourses.aspx");
                    return;
                }

                if (!IsPublished(courseId))
                {
                    Response.Redirect("ViewCourses.aspx");
                    return;
                }

                LoadCourseHeader(courseId);
                LoadSidebar(courseId);

                if (Request.QueryString["lessonid"] == null)
                {
                    LoadCourseOverview(courseId);
                    RenderProgress(0, GetAllLessonIds(courseId).Count);
                }
                else
                {
                    int lessonId;
                    if (int.TryParse(Request.QueryString["lessonid"], out lessonId))
                        LoadLesson(courseId, lessonId);
                    else
                        LoadCourseOverview(courseId);
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // AUTH: course must be published — matches original exactly
        // ══════════════════════════════════════════════════════════════════════
        private bool IsPublished(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Course
                    WHERE courseid=@id AND status='Active' AND deletiontime IS NULL", con);
                cmd.Parameters.AddWithValue("@id", courseId);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // COURSE HEADER — sets sidebar title
        // ══════════════════════════════════════════════════════════════════════
        private void LoadCourseHeader(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT coursename FROM Course WHERE courseid=@id", con);
                cmd.Parameters.AddWithValue("@id", courseId);
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                    lblCourseName.Text = Server.HtmlEncode(result.ToString());
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // SIDEBAR: modules + nested lessons — matches original exactly
        // ══════════════════════════════════════════════════════════════════════
        private void LoadSidebar(int courseId)
        {
            var modules = new List<dynamic>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand moduleCmd = new SqlCommand(@"
                    SELECT moduleid, modulename FROM Module
                    WHERE courseid=@courseid AND deletiontime IS NULL
                    ORDER BY ordernumber", con);
                moduleCmd.Parameters.AddWithValue("@courseid", courseId);
                SqlDataReader mr = moduleCmd.ExecuteReader();

                while (mr.Read())
                {
                    int moduleId = Convert.ToInt32(mr["moduleid"]);
                    modules.Add(new
                    {
                        moduleid = moduleId,
                        modulename = mr["modulename"].ToString(),
                        Lessons = GetLessons(moduleId)
                    });
                }
            }

            rptModules.DataSource = modules;
            rptModules.DataBind();
        }

        // Matches original GetLessons exactly
        private List<dynamic> GetLessons(int moduleId)
        {
            var lessons = new List<dynamic>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT lessonid, lessontitle FROM Lesson
                    WHERE moduleid=@moduleid AND deletiontime IS NULL
                    ORDER BY ordernumber", con);
                cmd.Parameters.AddWithValue("@moduleid", moduleId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    lessons.Add(new
                    {
                        lessonid = reader["lessonid"],
                        lessontitle = reader["lessontitle"]
                    });
            }
            return lessons;
        }

        // ══════════════════════════════════════════════════════════════════════
        // PROGRESS BAR — renders into sidebar litProgressFill + litProgressText
        // ══════════════════════════════════════════════════════════════════════
        private void RenderProgress(int current, int total)
        {
            int percent = total == 0 ? 0 : (int)(((double)current / total) * 100);

            litProgressFill.Text = $"<div class='progress-fill' style='width:{percent}%'></div>";
            litProgressText.Text = $"{current} / {total} lessons";

            // Also keep session values for page-to-page tracking
            Session["PreviewLessonIndex"] = current;
            Session["PreviewTotalLessons"] = total;
        }

        // ══════════════════════════════════════════════════════════════════════
        // COURSE OVERVIEW — shown when no lessonid param
        // ══════════════════════════════════════════════════════════════════════
        private void LoadCourseOverview(int courseId)
        {
            phOverview.Visible = true;
            phLesson.Visible = false;
            phCompletion.Visible = false;
            phOverview.Controls.Clear();

            int firstLessonId = GetFirstLesson(courseId);

            phOverview.Controls.Add(new Literal
            {
                Text = $@"
                <div class='overview-card'>
                    <div class='overview-banner'>
                        <div class='overview-label'>Course Preview</div>
                        <div class='overview-title'>{Server.HtmlEncode(lblCourseName.Text)}</div>
                        <div class='overview-sub'>This is how your course looks to enrolled students.</div>
                    </div>
                    <p>Use the sidebar on the left to navigate between lessons.
                       Click the button below to start from the first lesson.</p>
                    <a class='continue-btn'
                       href='Preview.aspx?courseid={courseId}&lessonid={firstLessonId}'>
                        Start Course →
                    </a>
                </div>"
            });
        }

        // Matches original GetFirstLesson exactly
        private int GetFirstLesson(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1 l.lessonid
                    FROM Lesson l
                    INNER JOIN Module m ON l.moduleid = m.moduleid
                    WHERE m.courseid=@courseid AND l.deletiontime IS NULL
                    ORDER BY m.ordernumber, l.ordernumber", con);
                cmd.Parameters.AddWithValue("@courseid", courseId);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // LESSON VIEW — matches original LoadLesson exactly
        // ══════════════════════════════════════════════════════════════════════
        private void LoadLesson(int courseId, int lessonId)
        {
            phOverview.Visible = false;
            phLesson.Visible = true;
            phCompletion.Visible = false;
            phLesson.Controls.Clear();

            string title = "";
            string description = "";
            string videoUrl = "";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Load lesson details
                SqlCommand lessonCmd = new SqlCommand(@"
                    SELECT lessontitle, lessondescription
                    FROM Lesson WHERE lessonid=@id", con);
                lessonCmd.Parameters.AddWithValue("@id", lessonId);
                SqlDataReader reader = lessonCmd.ExecuteReader();
                if (reader.Read())
                {
                    title = reader["lessontitle"].ToString();
                    description = reader["lessondescription"]?.ToString() ?? "";
                }
                reader.Close();

                // Load video URL
                SqlCommand videoCmd = new SqlCommand(@"
                    SELECT videourl FROM Material
                    WHERE lessonid=@lessonid AND videourl IS NOT NULL", con);
                videoCmd.Parameters.AddWithValue("@lessonid", lessonId);
                object videoResult = videoCmd.ExecuteScalar();
                if (videoResult != null)
                    videoUrl = ConvertToEmbed(videoResult.ToString());
            }

            // Lesson heading + description
            phLesson.Controls.Add(new Literal
            {
                Text = $@"
                <div class='lesson-card'>
                    <h2>{Server.HtmlEncode(title)}</h2>
                    <p>{Server.HtmlEncode(description)}</p>"
            });

            // Video
            if (!string.IsNullOrEmpty(videoUrl))
            {
                phLesson.Controls.Add(new Literal
                {
                    Text = $@"
                    <div class='video-wrapper'>
                        <iframe src='{videoUrl}' frameborder='0' allowfullscreen></iframe>
                    </div>"
                });
            }

            // Documents
            LoadMaterials(lessonId);

            // Prev / Next navigation + close the lesson-card div
            RenderNavigation(courseId, lessonId);

            phLesson.Controls.Add(new Literal { Text = "</div>" });
        }

        // Matches original LoadMaterials exactly — styled links
        private void LoadMaterials(int lessonId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT fileurl FROM Material
                    WHERE lessonid=@lessonid AND fileurl IS NOT NULL", con);
                cmd.Parameters.AddWithValue("@lessonid", lessonId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                bool hasFiles = false;
                string filesHtml = "<div style='margin-bottom:20px;'>";

                while (reader.Read())
                {
                    hasFiles = true;
                    string fileUrl = reader["fileurl"].ToString();
                    filesHtml += $@"
                        <a class='doc-link' target='_blank' href='{ResolveUrl(fileUrl)}'>
                            📄 Open Document
                        </a><br/>";
                }
                filesHtml += "</div>";

                if (hasFiles)
                    phLesson.Controls.Add(new Literal { Text = filesHtml });
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // NAVIGATION — Prev / Next, matches original RenderNavigation exactly
        // ══════════════════════════════════════════════════════════════════════
        private void RenderNavigation(int courseId, int lessonId)
        {
            List<int> lessonIds = GetAllLessonIds(courseId);
            int index = lessonIds.IndexOf(lessonId);

            // Update progress (current = 1-based position)
            RenderProgress(index + 1, lessonIds.Count);

            if (index >= lessonIds.Count - 1)
            {
                // Last lesson → show completion instead of Next
                ShowCompletion();
                return;
            }

            string navHtml = "<div class='lesson-nav'>";

            if (index > 0)
            {
                navHtml += $"<a class='nav-btn' href='Preview.aspx?courseid={courseId}&lessonid={lessonIds[index - 1]}'>← Previous</a>";
            }
            else
            {
                navHtml += "<span></span>"; // spacer so Next stays right-aligned
            }

            navHtml += $"<a class='nav-btn next' href='Preview.aspx?courseid={courseId}&lessonid={lessonIds[index + 1]}'>Next →</a>";
            navHtml += "</div>";

            phLesson.Controls.Add(new Literal { Text = navHtml });
        }

        // Matches original GetAllLessonIds exactly
        private List<int> GetAllLessonIds(int courseId)
        {
            var ids = new List<int>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT l.lessonid
                    FROM Lesson l
                    INNER JOIN Module m ON l.moduleid = m.moduleid
                    WHERE m.courseid=@courseid AND l.deletiontime IS NULL
                    ORDER BY m.ordernumber, l.ordernumber", con);
                cmd.Parameters.AddWithValue("@courseid", courseId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    ids.Add(Convert.ToInt32(reader["lessonid"]));
            }
            return ids;
        }

        // ══════════════════════════════════════════════════════════════════════
        // COMPLETION SCREEN — matches original ShowCompletion
        // ══════════════════════════════════════════════════════════════════════
        private void ShowCompletion()
        {
            phOverview.Visible = false;
            phLesson.Visible = false;
            phCompletion.Visible = true;
            phCompletion.Controls.Clear();

            phCompletion.Controls.Add(new Literal
            {
                Text = @"
                <div class='completion-card'>
                    <div class='completion-icon'>🎉</div>
                    <h2>Course Complete!</h2>
                    <p>You've reached the end of this course preview.<br/>
                       Everything looks great for your students.</p>
                    <a href='ViewCourses.aspx' class='back-courses-btn'>
                        ← Back to Courses
                    </a>
                </div>"
            });
        }

        // ══════════════════════════════════════════════════════════════════════
        // HELPERS — match originals exactly
        // ══════════════════════════════════════════════════════════════════════

        // Converts YouTube watch URL to embed URL
        private string ConvertToEmbed(string url)
        {
            if (url.Contains("watch?v="))
            {
                string id = url.Split('=')[1];
                return "https://www.youtube.com/embed/" + id;
            }
            return url;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewCourses.aspx");
        }
    }
}