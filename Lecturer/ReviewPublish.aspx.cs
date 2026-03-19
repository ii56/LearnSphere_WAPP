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
    public partial class ReviewPublish : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 Strong session validation
            if (Session["userid"] == null || Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (Session["CurrentCourseID"] == null)
            {
                Response.Redirect("CreateCourse.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Step indicator
                ViewState["Step"] = "4";

                LoadSidebarProfileImage();
                LoadCourse();
                LoadModulesAndLessons();
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

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", userId);

                    con.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string imagePath = result.ToString();
                        Session["profileImage"] = imagePath;
                        imgSidebarProfile.Src = ResolveUrl(imagePath);
                    }
                    else
                    {
                        imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                    }
                }
            }
        }

        private void LoadCourse()
        {
            int courseId = Convert.ToInt32(Session["CurrentCourseID"]);
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                // 🔐 Ownership check added
                string query = @"SELECT coursename, description, price 
                 FROM Course 
                 WHERE courseid = @id AND ownerid = @lecturerid";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", courseId);
                    cmd.Parameters.AddWithValue("@lecturerid", userId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // 🔐 XSS Protection
                        lblCourseName.Text = "Course: " + Server.HtmlEncode(reader["coursename"].ToString());
                        lblCourseDesc.Text = "Description: " + Server.HtmlEncode(reader["description"].ToString());
                        lblCoursePrice.Text = "Price: $" + Server.HtmlEncode(reader["price"].ToString());
                    }
                    else
                    {
                        // Unauthorized access protection
                        Response.Redirect("CreateCourse.aspx");
                    }
                }
            }
        }

        private void LoadModulesAndLessons()
        {
            int courseId = Convert.ToInt32(Session["CurrentCourseID"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string moduleQuery = @"SELECT moduleid, modulename 
                                       FROM Module 
                                       WHERE courseid = @courseid
                                       AND deletiontime IS NULL";

                using (SqlCommand moduleCmd = new SqlCommand(moduleQuery, con))
                {
                    moduleCmd.Parameters.AddWithValue("@courseid", courseId);

                    SqlDataReader moduleReader = moduleCmd.ExecuteReader();

                    var modules = new List<dynamic>();

                    while (moduleReader.Read())
                    {
                        modules.Add(new
                        {
                            moduleid = moduleReader["moduleid"],
                            modulename = moduleReader["modulename"],
                            Lessons = new List<dynamic>()
                        });
                    }

                    moduleReader.Close();

                    foreach (var module in modules)
                    {
                        string lessonQuery = @"SELECT lessontitle, duration
                                               FROM Lesson
                                               WHERE moduleid = @moduleid
                                               AND deletiontime IS NULL";

                        using (SqlCommand lessonCmd = new SqlCommand(lessonQuery, con))
                        {
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
                        }
                    }

                    rptModules.DataSource = modules;
                    rptModules.DataBind();
                }
            }
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            if (Session["CurrentCourseID"] == null || Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            int courseId = Convert.ToInt32(Session["CurrentCourseID"]);
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = @"UPDATE Course 
                         SET status = 'Active' 
                         WHERE courseid = @id AND ownerid = @lecturerid";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", courseId);
                    cmd.Parameters.AddWithValue("@lecturerid", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        lblMessage.Text = "❌ Unauthorized action.";
                        return;
                    }
                    LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Publish Course (CourseID: " + courseId + ")");
                }
            }

            Session.Remove("CurrentCourseID");

            // 🔥 REDIRECT TO VIEW COURSES
            Response.Redirect("ViewCourses.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("~/Login.aspx");
        }

        protected void btnBackToLessons_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddLessons.aspx");
        }
    }
}