using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class editLesson : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int lessonId;
        int moduleId;
        int courseId;
        int userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // AUTHENTICATION
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);

            // VALIDATE QUERY STRINGS
            if (!int.TryParse(Request.QueryString["courseid"], out courseId) || courseId <= 0)
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            int.TryParse(Request.QueryString["lessonid"], out lessonId);
            int.TryParse(Request.QueryString["moduleid"], out moduleId);

            // AUTHORIZATION CHECK (CRITICAL)
            if (!IsCourseOwner(courseId, userId))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (lessonId > 0 && !IsLessonValid(lessonId))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (moduleId > 0 && !IsModuleValid(moduleId))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();

                if (lessonId > 0)
                {
                    lblModuleName.Text = "Edit Lesson";
                    btnUpdateModule.Text = "Update and Continue";
                    LoadLesson();
                }
                else
                {
                    lblModuleName.Text = "Add Lesson";
                    btnUpdateModule.Text = "Confirm Addition";
                    LoadModuleName();
                }
            }
        }

        private bool IsCourseOwner(int courseId, int userId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Course WHERE courseid=@cid AND ownerid=@uid";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool IsModuleValid(int moduleId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Module WHERE moduleid=@mid AND courseid=@cid";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@mid", SqlDbType.Int).Value = moduleId;
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool IsLessonValid(int lessonId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) FROM Lesson l
                                 JOIN Module m ON l.moduleid = m.moduleid
                                 WHERE l.lessonid=@lid AND m.courseid=@cid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@lid", SqlDbType.Int).Value = lessonId;
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid=@id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();
                object result = cmd.ExecuteScalar();

                imgSidebarProfile.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        private void LoadModuleName()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT modulename FROM Module WHERE moduleid=@id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                    lblModuleName.Text += " - " + Server.HtmlEncode(result.ToString());
            }
        }

        private void LoadLesson()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT l.lessontitle, l.description, l.duration, m.modulename, l.moduleid
                                 FROM Lesson l
                                 INNER JOIN Module m ON l.moduleid = m.moduleid
                                 WHERE l.lessonid=@id AND l.deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = lessonId;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtLessonTitle.Text = Server.HtmlEncode(reader["lessontitle"].ToString());
                    txtLessonDesc.Text = Server.HtmlEncode(reader["description"].ToString());
                    txtDuration.Text = reader["duration"].ToString();
                    lblModuleName.Text += " - " + Server.HtmlEncode(reader["modulename"].ToString());
                    moduleId = Convert.ToInt32(reader["moduleid"]);
                }
            }
        }

        protected void btnUpdateModule_Click(object sender, EventArgs e)
        {
            try
            {
                string title = txtLessonTitle.Text.Trim();
                string desc = txtLessonDesc.Text.Trim();
                string videoUrl = txtVideoUrl.Text.Trim();

                if (string.IsNullOrWhiteSpace(title))
                {
                    lblMessage.Text = "Lesson title is required.";
                    return;
                }

                int duration;
                if (!int.TryParse(txtDuration.Text, out duration) || duration < 1 || duration > 600)
                {
                    lblMessage.Text = "Invalid duration.";
                    return;
                }

                title = Server.HtmlEncode(title);
                desc = Server.HtmlEncode(desc);
                videoUrl = Server.HtmlEncode(videoUrl);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    int currentLessonId = lessonId;

                    if (lessonId > 0)
                    {
                        string updateQuery = @"UPDATE Lesson
                                               SET lessontitle=@title, description=@desc, duration=@duration
                                               WHERE lessonid=@id";

                        SqlCommand cmd = new SqlCommand(updateQuery, con);
                        cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = title;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = lessonId;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string insertQuery = @"INSERT INTO Lesson
                            (moduleid, lessontitle, description, duration, ordernumber, creationtime)
                            OUTPUT INSERTED.lessonid
                            VALUES
                            (@moduleid, @title, @desc, @duration,
                            (SELECT ISNULL(MAX(ordernumber),0)+1 FROM Lesson WHERE moduleid=@moduleid AND deletiontime IS NULL),
                            GETDATE())";

                        SqlCommand cmd = new SqlCommand(insertQuery, con);
                        cmd.Parameters.Add("@moduleid", SqlDbType.Int).Value = moduleId;
                        cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = title;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;

                        currentLessonId = (int)cmd.ExecuteScalar();
                    }

                    // FILE UPLOAD SECURITY
                    if (fuLessonFile.HasFile)
                    {
                        if (fuLessonFile.PostedFile.ContentLength > 5 * 1024 * 1024)
                        {
                            lblMessage.Text = "File too large (max 5MB).";
                            return;
                        }

                        string ext = Path.GetExtension(fuLessonFile.FileName).ToLower();
                        string[] allowed = { ".pdf", ".doc", ".docx", ".ppt", ".pptx" };

                        if (Array.IndexOf(allowed, ext) < 0)
                        {
                            lblMessage.Text = "Invalid file type.";
                            return;
                        }

                        string fileName = Guid.NewGuid().ToString() + ext;
                        string folder = Server.MapPath("~/Uploads/LessonMaterials/");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                        string fullPath = Path.Combine(folder, fileName);
                        fuLessonFile.SaveAs(fullPath);
                        LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Updated Lesson (LessonID: " + lessonId + ")");
                    }
                }

                Response.Redirect("editCourse.aspx?courseid=" + courseId);
            }
            catch
            {
                lblMessage.Text = "An error occurred. Please try again.";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}