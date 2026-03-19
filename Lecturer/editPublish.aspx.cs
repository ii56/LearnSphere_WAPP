using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace LearnSphere_WAPP.Lecturer
{
    public class ModuleView
    {
        public int moduleid { get; set; }
        public string modulename { get; set; }
        public List<LessonView> Lessons { get; set; }
        public bool HasExam { get; set; }
        public string ExamTitle { get; set; }
        public int QuestionCount { get; set; }
        public int TotalMarks { get; set; }
    }

    public class LessonView
    {
        public string lessontitle { get; set; }
        public int duration { get; set; }
    }

    public partial class editPublish : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int courseId;
        int userId;

        protected void Page_Init(object sender, EventArgs e)
        {
            // 🔐 CSRF Protection
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 AUTHENTICATION
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);

            // 🔐 VALIDATE QUERY
            if (!int.TryParse(Request.QueryString["courseid"], out courseId) || courseId <= 0)
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            // 🔐 AUTHORIZATION (CRITICAL)
            if (!IsCourseOwner(courseId, userId))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadCourse();
                LoadModulesAndLessons();
            }
        }

        // 🔐 CHECK COURSE OWNERSHIP
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

        private void LoadCourse()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT coursename, description, price 
                                 FROM Course 
                                 WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblCourseName.Text = Server.HtmlEncode(reader["coursename"].ToString());
                    lblCourseDesc.Text = Server.HtmlEncode(reader["description"].ToString());
                    lblCoursePrice.Text = "Price: $" + reader["price"].ToString();
                }
                else
                {
                    Response.Redirect("ViewCourses.aspx");
                }
            }
        }

        private void LoadModulesAndLessons()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                List<ModuleView> modules = new List<ModuleView>();

                // MODULES
                SqlCommand moduleCmd = new SqlCommand(
                    @"SELECT moduleid, modulename 
                      FROM Module 
                      WHERE courseid=@courseid AND deletiontime IS NULL",
                    con);

                moduleCmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;

                SqlDataReader moduleReader = moduleCmd.ExecuteReader();

                while (moduleReader.Read())
                {
                    modules.Add(new ModuleView
                    {
                        moduleid = Convert.ToInt32(moduleReader["moduleid"]),
                        modulename = moduleReader["modulename"].ToString(),
                        Lessons = new List<LessonView>()
                    });
                }

                moduleReader.Close();

                foreach (var module in modules)
                {
                    // LESSONS
                    SqlCommand lessonCmd = new SqlCommand(
                        @"SELECT lessontitle, duration 
                          FROM Lesson 
                          WHERE moduleid=@mid AND deletiontime IS NULL",
                        con);

                    lessonCmd.Parameters.Add("@mid", SqlDbType.Int).Value = module.moduleid;

                    SqlDataReader lessonReader = lessonCmd.ExecuteReader();

                    while (lessonReader.Read())
                    {
                        module.Lessons.Add(new LessonView
                        {
                            lessontitle = lessonReader["lessontitle"].ToString(),
                            duration = Convert.ToInt32(lessonReader["duration"])
                        });
                    }

                    lessonReader.Close();
                }

                rptModules.DataSource = modules;
                rptModules.DataBind();
            }
        }

        // 🔐 VALIDATION BEFORE PUBLISH
        private bool IsCourseValidForPublish()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Must have modules
                SqlCommand cmd1 = new SqlCommand(
                    "SELECT COUNT(*) FROM Module WHERE courseid=@id AND deletiontime IS NULL", con);
                cmd1.Parameters.Add("@id", SqlDbType.Int).Value = courseId;

                if ((int)cmd1.ExecuteScalar() == 0)
                    return false;

                // Must have lessons
                SqlCommand cmd2 = new SqlCommand(
                    @"SELECT COUNT(*) FROM Lesson 
                      WHERE moduleid IN (SELECT moduleid FROM Module WHERE courseid=@id)",
                    con);
                cmd2.Parameters.Add("@id", SqlDbType.Int).Value = courseId;

                if ((int)cmd2.ExecuteScalar() == 0)
                    return false;

                return true;
            }
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            try
            {
                // 🔐 VALIDATION
                if (!IsCourseValidForPublish())
                {
                    lblMessage.Text = "Course must contain at least one module and lesson.";
                    return;
                }

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Course SET status='Active' WHERE courseid=@id",
                        con);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;

                    cmd.ExecuteNonQuery();
                }

                Response.Redirect("LecturerDashboard.aspx");
            }
            catch
            {
                lblMessage.Text = "Error publishing course. Please try again.";
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
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