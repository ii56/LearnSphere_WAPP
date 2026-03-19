using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class editCourse : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int courseId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // AUTHENTICATION
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // VALIDATE COURSE ID
            if (!int.TryParse(Request.QueryString["courseid"], out courseId) || courseId <= 0)
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            // AUTHORIZATION (CRITICAL)
            if (!IsCourseOwner(courseId, Convert.ToInt32(Session["userid"])))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCourseInfo();
                LoadSidebarProfileImage();
                LoadModules();
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
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
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
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string imagePath = result.ToString();
                    imgSidebarProfile.Src = ResolveUrl(imagePath);
                }
                else
                {
                    imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                }
            }
        }

        private void LoadCourseInfo()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT coursename, description, price 
                                 FROM Course 
                                 WHERE courseid=@id AND deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblCourseName.Text = Server.HtmlEncode(reader["coursename"].ToString());
                    lblCourseDescription.Text = Server.HtmlEncode(reader["description"].ToString());
                    lblCoursePrice.Text = reader["price"].ToString();
                }
                else
                {
                    Response.Redirect("ViewCourses.aspx");
                }
            }
        }

        private void LoadModules()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT moduleid, modulename 
                                 FROM Module 
                                 WHERE courseid=@courseid AND deletiontime IS NULL 
                                 ORDER BY ordernumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;

                con.Open();
                rptModules.DataSource = cmd.ExecuteReader();
                rptModules.DataBind();
            }
        }

        protected void rptModules_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int moduleId;
                if (!int.TryParse(DataBinder.Eval(e.Item.DataItem, "moduleid").ToString(), out moduleId))
                    return;

                Repeater rptLessons = (Repeater)e.Item.FindControl("rptLessons");

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"SELECT lessonid, lessontitle 
                                     FROM Lesson 
                                     WHERE moduleid=@moduleid AND deletiontime IS NULL 
                                     ORDER BY ordernumber";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@moduleid", SqlDbType.Int).Value = moduleId;

                    con.Open();
                    rptLessons.DataSource = cmd.ExecuteReader();
                    rptLessons.DataBind();
                }
            }
        }

        protected void Module_Command(object sender, CommandEventArgs e)
        {
            int moduleId;
            if (!int.TryParse(e.CommandArgument.ToString(), out moduleId))
                return;

            if (!IsModuleValid(moduleId))
                return;

            if (e.CommandName == "EditModule")
            {
                Response.Redirect("editModule.aspx?moduleid=" + Server.UrlEncode(moduleId.ToString()) +
                  "&courseid=" + Server.UrlEncode(courseId.ToString()));
            }
            else if (e.CommandName == "DeleteModule")
            {
                DeleteModule(moduleId);
                LoadModules();
            }
            else if (e.CommandName == "AddLesson")
            {
                Response.Redirect("editLesson.aspx?moduleid=" + moduleId + "&courseid=" + courseId);
            }
        }

        protected void Lesson_Command(object sender, CommandEventArgs e)
        {
            int lessonId;
            if (!int.TryParse(e.CommandArgument.ToString(), out lessonId))
                return;

            if (!IsLessonValid(lessonId))
                return;

            if (e.CommandName == "EditLesson")
            {
                Response.Redirect("editLesson.aspx?lessonid=" + lessonId + "&courseid=" + courseId);
            }
            else if (e.CommandName == "DeleteLesson")
            {
                DeleteLesson(lessonId);
                LoadModules();
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

        private void DeleteModule(int moduleId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    SqlCommand cmd1 = new SqlCommand(
                        @"DELETE FROM Material 
                          WHERE lessonid IN (SELECT lessonid FROM Lesson WHERE moduleid=@id)",
                        con, trans);
                    cmd1.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand(
                        "DELETE FROM Lesson WHERE moduleid=@id",
                        con, trans);
                    cmd2.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;
                    cmd2.ExecuteNonQuery();

                    SqlCommand cmd3 = new SqlCommand(
                        "DELETE FROM Module WHERE moduleid=@id",
                        con, trans);
                    cmd3.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;
                    cmd3.ExecuteNonQuery();

                    trans.Commit();
                    LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Deleted Module (ModuleID: " + moduleId + ")");
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }

        private void DeleteLesson(int lessonId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    SqlCommand cmd1 = new SqlCommand(
                        "DELETE FROM Material WHERE lessonid=@id",
                        con, trans);
                    cmd1.Parameters.Add("@id", SqlDbType.Int).Value = lessonId;
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand(
                        "DELETE FROM Lesson WHERE lessonid=@id",
                        con, trans);
                    cmd2.Parameters.Add("@id", SqlDbType.Int).Value = lessonId;
                    cmd2.ExecuteNonQuery();

                    trans.Commit();
                    LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Deleted Lesson (LessonID: " + lessonId + ")");
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Module_Command(source, e);
        }

        protected void btnReview_Click(object sender, EventArgs e)
        {
            Response.Redirect("editPublish.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            Response.Redirect("editModule.aspx?courseid=" + courseId);
        }

        protected void btnCreateExam_Click(object sender, EventArgs e)
        {
            Response.Redirect("MakeExam.aspx?courseid=" + courseId);
        }

        protected void btnEditExam_Click(object sender, EventArgs e)
        {
            Response.Redirect("MakeExam.aspx?courseid=" + courseId + "&edit=1");
        }

        protected void btnDeleteExam_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Exam WHERE courseid=@courseid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;

                con.Open();
                cmd.ExecuteNonQuery();
                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Deleted Exam (CourseID: " + courseId + ")");
            }
        }
    }
}