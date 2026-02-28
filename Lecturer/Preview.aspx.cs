using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int courseId;
                if (!int.TryParse(Request.QueryString["courseid"], out courseId))
                    Response.Redirect("ViewCourses.aspx");

                if (!IsPublished(courseId))
                    Response.Redirect("ViewCourses.aspx");

                LoadSidebar(courseId);
                LoadCourseHeader(courseId);

                if (Request.QueryString["lessonid"] == null)
                {
                    LoadCourseOverview(courseId);
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

        private bool IsPublished(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) FROM Course
                                 WHERE courseid=@id
                                 AND status=1
                                 AND deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void LoadCourseHeader(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT coursename FROM Course WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                    lblCourseName.Text = result.ToString();
            }
        }
        private void LoadSidebar(int courseId)
        {
            List<dynamic> modules = new List<dynamic>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string moduleQuery = @"SELECT moduleid, modulename
                                       FROM Module
                                       WHERE courseid=@courseid
                                       AND deletiontime IS NULL
                                       ORDER BY ordernumber";

                SqlCommand moduleCmd = new SqlCommand(moduleQuery, con);
                moduleCmd.Parameters.AddWithValue("@courseid", courseId);

                SqlDataReader moduleReader = moduleCmd.ExecuteReader();

                while (moduleReader.Read())
                {
                    int moduleId = Convert.ToInt32(moduleReader["moduleid"]);

                    modules.Add(new
                    {
                        moduleid = moduleId,
                        modulename = moduleReader["modulename"].ToString(),
                        Lessons = GetLessons(moduleId)
                    });
                }
            }

            rptModules.DataSource = modules;
            rptModules.DataBind();
        }

        private List<dynamic> GetLessons(int moduleId)
        {
            List<dynamic> lessons = new List<dynamic>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT lessonid, lessontitle
                                 FROM Lesson
                                 WHERE moduleid=@moduleid
                                 AND deletiontime IS NULL
                                 ORDER BY ordernumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@moduleid", moduleId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lessons.Add(new
                    {
                        lessonid = reader["lessonid"],
                        lessontitle = reader["lessontitle"]
                    });
                }
            }

            return lessons;
        }

        private void LoadCourseOverview(int courseId)
        {
            phOverview.Visible = true;
            phLesson.Visible = false;
            phCompletion.Visible = false;

            phOverview.Controls.Clear();

            int firstLessonId = GetFirstLesson(courseId);

            phOverview.Controls.Add(new Literal
            {
                Text = "<h2>Course Overview</h2>" +
                       "<p>This is how students will see your course.</p>" +
                       "<a class='continue-btn' href='Preview.aspx?courseid=" +
                       courseId + "&lessonid=" + firstLessonId +
                       "'>Continue →</a>"
            });
        }

        private int GetFirstLesson(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT TOP 1 l.lessonid
                    FROM Lesson l
                    INNER JOIN Module m ON l.moduleid = m.moduleid
                    WHERE m.courseid=@courseid
                    AND l.deletiontime IS NULL
                    ORDER BY m.ordernumber, l.ordernumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                con.Open();
                object result = cmd.ExecuteScalar();

                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

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

                string lessonQuery = @"
                    SELECT lessontitle, lessondescription
                    FROM Lesson
                    WHERE lessonid=@id";

                SqlCommand lessonCmd = new SqlCommand(lessonQuery, con);
                lessonCmd.Parameters.AddWithValue("@id", lessonId);

                SqlDataReader reader = lessonCmd.ExecuteReader();

                if (reader.Read())
                {
                    title = reader["lessontitle"].ToString();
                    description = reader["lessondescription"].ToString();
                }

                reader.Close();

                string videoQuery = @"
                    SELECT videourl
                    FROM Material
                    WHERE lessonid=@lessonid
                    AND videourl IS NOT NULL";

                SqlCommand videoCmd = new SqlCommand(videoQuery, con);
                videoCmd.Parameters.AddWithValue("@lessonid", lessonId);

                object videoResult = videoCmd.ExecuteScalar();
                if (videoResult != null)
                    videoUrl = ConvertToEmbed(videoResult.ToString());
            }

            phLesson.Controls.Add(new Literal
            {
                Text = "<h2>" + title + "</h2>" +
                       "<p>" + description + "</p>"
            });

            if (!string.IsNullOrEmpty(videoUrl))
            {
                phLesson.Controls.Add(new Literal
                {
                    Text = "<iframe width='100%' height='500' src='" +
                           videoUrl +
                           "' frameborder='0' allowfullscreen></iframe>"
                });
            }

            LoadMaterials(lessonId);
            RenderNavigation(courseId, lessonId);
        }

        private void LoadMaterials(int lessonId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT fileurl
                    FROM Material
                    WHERE lessonid=@lessonid
                    AND fileurl IS NOT NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@lessonid", lessonId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string fileUrl = reader["fileurl"].ToString();

                    phLesson.Controls.Add(new Literal
                    {
                        Text = "<div><a target='_blank' href='" +
                               ResolveUrl(fileUrl) +
                               "'>Open Document</a></div>"
                    });
                }
            }
        }

        private void RenderNavigation(int courseId, int lessonId)
        {
            List<int> lessonIds = GetAllLessonIds(courseId);

            int index = lessonIds.IndexOf(lessonId);

            Session["PreviewLessonIndex"] = index + 1;
            Session["PreviewTotalLessons"] = lessonIds.Count;

            RenderProgress(index + 1, lessonIds.Count);

            string navHtml = "<div class='lesson-nav'>";

            if (index > 0)
            {
                navHtml += "<a href='Preview.aspx?courseid=" +
                           courseId + "&lessonid=" +
                           lessonIds[index - 1] +
                           "'>← Previous</a>";
            }

            if (index < lessonIds.Count - 1)
            {
                navHtml += "<a href='Preview.aspx?courseid=" +
                           courseId + "&lessonid=" +
                           lessonIds[index + 1] +
                           "'>Next →</a>";
            }
            else
            {
                ShowCompletion();
                return;
            }

            navHtml += "</div>";

            phLesson.Controls.Add(new Literal { Text = navHtml });
        }

        private List<int> GetAllLessonIds(int courseId)
        {
            List<int> ids = new List<int>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT l.lessonid
                    FROM Lesson l
                    INNER JOIN Module m ON l.moduleid = m.moduleid
                    WHERE m.courseid=@courseid
                    AND l.deletiontime IS NULL
                    ORDER BY m.ordernumber, l.ordernumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ids.Add(Convert.ToInt32(reader["lessonid"]));
                }
            }

            return ids;
        }

        private void ShowCompletion()
        {
            phOverview.Visible = false;
            phLesson.Visible = false;
            phCompletion.Visible = true;

            phCompletion.Controls.Clear();

            phCompletion.Controls.Add(new Literal
            {
                Text = "<h2>🎉 Course Completed!</h2>" +
                       "<a href='ViewCourses.aspx'>Back to Courses</a>"
            });
        }

        private string ConvertToEmbed(string url)
        {
            if (url.Contains("watch?v="))
            {
                string id = url.Split('=')[1];
                return "https://www.youtube.com/embed/" + id;
            }
            return url;
        }

        private void RenderProgress(int current, int total)
        {
            int percent = total == 0 ? 0 :
                (int)(((double)current / total) * 100);

            litProgressBar.Text =
                "<div class='progress-bar'>" +
                "<div class='progress-fill' style='width:" +
                percent + "%'></div></div>";
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewCourses.aspx");
        }
    }
}