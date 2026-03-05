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
    public partial class editPublish : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        int courseId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!int.TryParse(Request.QueryString["courseid"], out courseId) || courseId <= 0)
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCourse();
                LoadModulesAndLessons();
                LoadSidebarProfileImage();
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", userId);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    imgSidebarProfile.Src = ResolveUrl(result.ToString());
                }
                else
                {
                    imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                }
            }
        }
        private void LoadCourse()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT coursename, description, price FROM Course WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblCourseName.Text = reader["coursename"].ToString();
                    lblCourseDesc.Text = reader["description"].ToString();
                    lblCoursePrice.Text = "Price: $" + reader["price"].ToString();
                }
            }
        }

        private void LoadModulesAndLessons()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string moduleQuery = @"SELECT moduleid, modulename
                               FROM Module
                               WHERE courseid=@courseid
                               AND deletiontime IS NULL";

                SqlCommand moduleCmd = new SqlCommand(moduleQuery, con);
                moduleCmd.Parameters.AddWithValue("@courseid", courseId);

                SqlDataReader moduleReader = moduleCmd.ExecuteReader();

                var modules = new List<dynamic>();

                while (moduleReader.Read())
                {
                    modules.Add(new
                    {
                        moduleid = moduleReader["moduleid"],
                        modulename = moduleReader["modulename"],
                        Lessons = new List<dynamic>(),
                        HasExam = false,
                        ExamTitle = "",
                        QuestionCount = 0,
                        TotalMarks = 0
                    });
                }

                moduleReader.Close();


                foreach (var module in modules)
                {

                    // LOAD LESSONS

                    string lessonQuery = @"SELECT lessontitle, duration
                                   FROM Lesson
                                   WHERE moduleid=@moduleid
                                   AND deletiontime IS NULL";

                    SqlCommand lessonCmd = new SqlCommand(lessonQuery, con);
                    lessonCmd.Parameters.AddWithValue("@moduleid", module.moduleid);

                    SqlDataReader lessonReader = lessonCmd.ExecuteReader();

                    while (lessonReader.Read())
                    {
                        module.Lessons.Add(new
                        {
                            lessontitle = lessonReader["lessontitle"],
                            duration = lessonReader["duration"]
                        });
                    }

                    lessonReader.Close();


                    // LOAD MODULE EXAM
                    pnlCourseExam.Visible = false;
                    string examQuery = @"SELECT TOP 1 examid, examtitle
                     FROM Exam
                     WHERE moduleid=@moduleid";

                    SqlCommand examCmd = new SqlCommand(examQuery, con);
                    examCmd.Parameters.AddWithValue("@moduleid", module.moduleid);

                    SqlDataReader examReader = examCmd.ExecuteReader();

                    if (examReader.Read())
                    {
                        module.HasExam = true;
                        module.ExamTitle = examReader["examtitle"].ToString();

                        int examId = Convert.ToInt32(examReader["examid"]);

                        examReader.Close();

                        string statsQuery = @"SELECT 
                        COUNT(*) AS QuestionCount,
                        ISNULL(SUM(marks),0) AS TotalMarks
                      FROM ExamQuestion
                      WHERE examid=@examid";

                        SqlCommand statsCmd = new SqlCommand(statsQuery, con);
                        statsCmd.Parameters.AddWithValue("@examid", examId);

                        SqlDataReader statsReader = statsCmd.ExecuteReader();

                        if (statsReader.Read())
                        {
                            module.QuestionCount = Convert.ToInt32(statsReader["QuestionCount"]);
                            module.TotalMarks = Convert.ToInt32(statsReader["TotalMarks"]);
                        }

                        statsReader.Close();
                    }
                    else
                    {
                        examReader.Close();
                    }

                }


                // LOAD COURSE EXAM
                pnlCourseExam.Visible = false;
                string courseExamQuery = @"SELECT examid, examtitle
                                   FROM Exam
                                   WHERE courseid=@courseid";

                SqlCommand courseExamCmd = new SqlCommand(courseExamQuery, con);
                courseExamCmd.Parameters.AddWithValue("@courseid", courseId);

                SqlDataReader courseExamReader = courseExamCmd.ExecuteReader();

                if (courseExamReader.Read())
                {
                    pnlCourseExam.Visible = true;

                    lblCourseExamTitle.Text = courseExamReader["examtitle"].ToString();

                    int examId = Convert.ToInt32(courseExamReader["examid"]);

                    courseExamReader.Close();

                    string countQuery = "SELECT COUNT(*) FROM ExamQuestion WHERE examid=@examid";

                    SqlCommand countCmd = new SqlCommand(countQuery, con);
                    countCmd.Parameters.AddWithValue("@examid", examId);

                    lblCourseExamQuestions.Text = countCmd.ExecuteScalar().ToString();
                }
                else
                {
                    courseExamReader.Close();
                }


                rptModules.DataSource = modules;
                rptModules.DataBind();
            }
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = "UPDATE Course SET status = 1 WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);
                cmd.ExecuteNonQuery();
            }

            Response.Redirect("LecturerDashboard.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }
    }
}