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
    public partial class editLesson : System.Web.UI.Page
    {
        string connStr = ConfigurationManager
            .ConnectionStrings["LearnSphereDB"].ConnectionString;

        int lessonId;
        int moduleId;
        int courseId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!int.TryParse(Request.QueryString["courseid"], out courseId))
                Response.Redirect("ViewCourses.aspx");

            int.TryParse(Request.QueryString["lessonid"], out lessonId);

            int.TryParse(Request.QueryString["moduleid"], out moduleId);

            if (!IsPostBack)
            {
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

        private void LoadModuleName()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT modulename
                    FROM Module
                    WHERE moduleid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", moduleId);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                    lblModuleName.Text += " - " + result.ToString();
            }
        }

        private void LoadLesson()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                            SELECT l.lessontitle,
                                   l.description,
                                   l.duration,
                                   m.modulename,
                                   l.moduleid
                            FROM Lesson l
                            INNER JOIN Module m ON l.moduleid = m.moduleid
                            WHERE l.lessonid = @id
                            AND l.deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", lessonId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtLessonTitle.Text = reader["lessontitle"].ToString();
                    txtLessonDesc.Text = reader["description"].ToString();
                    txtDuration.Text = reader["duration"].ToString();
                    lblModuleName.Text += " - " + reader["modulename"].ToString();
                    moduleId = Convert.ToInt32(reader["moduleid"]);
                }
            }
        }

        protected void btnUpdateModule_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                if (lessonId > 0)
                {
                    string query = @"
                                UPDATE Lesson
                                SET lessontitle = @title,
                                    description = @desc,
                                    duration = @duration
                                WHERE lessonid = @id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@title", txtLessonTitle.Text);
                    cmd.Parameters.AddWithValue("@desc", txtLessonDesc.Text);
                    cmd.Parameters.AddWithValue("@duration",
                        string.IsNullOrEmpty(txtDuration.Text)
                        ? (object)DBNull.Value
                        : Convert.ToInt32(txtDuration.Text));
                    cmd.Parameters.AddWithValue("@id", lessonId);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string query = @"
                        INSERT INTO Lesson
                        (moduleid, lessontitle, description, duration, creationtime)
                        VALUES
                        (@moduleid, @title, @desc, @duration, GETDATE())";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@moduleid", moduleId);
                    cmd.Parameters.AddWithValue("@title", txtLessonTitle.Text);
                    cmd.Parameters.AddWithValue("@desc", txtLessonDesc.Text);
                    cmd.Parameters.AddWithValue("@duration",
                        string.IsNullOrEmpty(txtDuration.Text)
                        ? (object)DBNull.Value
                        : Convert.ToInt32(txtDuration.Text));
                    cmd.ExecuteNonQuery();
                }
            }

            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}