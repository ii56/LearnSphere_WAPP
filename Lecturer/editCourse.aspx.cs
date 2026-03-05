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
        string connStr = ConfigurationManager
            .ConnectionStrings["LearnSphereDB"].ConnectionString;

        int courseId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!int.TryParse(Request.QueryString["courseid"], out courseId))
            {
                Response.Redirect("ViewCourses.aspx");
            }

            if (!IsPostBack)
            {
                LoadCourseInfo();
                LoadSidebarProfileImage();
                LoadModules();
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
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

        private void LoadCourseInfo()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT coursename, description, price
                                 FROM Course
                                 WHERE courseid=@id
                                 AND deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblCourseName.Text = reader["coursename"].ToString();
                    lblCourseDescription.Text = reader["description"].ToString();
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
                                 WHERE courseid=@courseid
                                 AND deletiontime IS NULL
                                 ORDER BY ordernumber";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@courseid", courseId);

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
                int moduleId = Convert.ToInt32(
                    DataBinder.Eval(e.Item.DataItem, "moduleid"));

                Repeater rptLessons =
                    (Repeater)e.Item.FindControl("rptLessons");

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
                    rptLessons.DataSource = cmd.ExecuteReader();
                    rptLessons.DataBind();
                }
            }
        }

        protected void Module_Command(object sender, CommandEventArgs e)
        {
            int moduleId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditModule")
            {
                Response.Redirect("editModule.aspx?moduleid=" +
                                  moduleId +
                                  "&courseid=" + courseId);
            }

            if (e.CommandName == "DeleteModule")
            {
                DeleteModule(moduleId);
                LoadModules();
            }

            if (e.CommandName == "AddLesson")
            {
                Response.Redirect("editLesson.aspx?moduleid=" +
                                  moduleId +
                                  "&courseid=" + courseId);
            }
        }

        protected void Lesson_Command(object sender, CommandEventArgs e)
        {
            int lessonId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditLesson")
            {
                Response.Redirect("editLesson.aspx?lessonid=" +
                                  lessonId +
                                  "&courseid=" + courseId);
            }

            if (e.CommandName == "DeleteLesson")
            {
                DeleteLesson(lessonId);
                LoadModules();
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
                          WHERE lessonid IN
                          (SELECT lessonid FROM Lesson WHERE moduleid=@id)",
                        con, trans);
                    cmd1.Parameters.AddWithValue("@id", moduleId);
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand(
                        "DELETE FROM Lesson WHERE moduleid=@id",
                        con, trans);
                    cmd2.Parameters.AddWithValue("@id", moduleId);
                    cmd2.ExecuteNonQuery();

                    SqlCommand cmd3 = new SqlCommand(
                        "DELETE FROM Module WHERE moduleid=@id",
                        con, trans);
                    cmd3.Parameters.AddWithValue("@id", moduleId);
                    cmd3.ExecuteNonQuery();

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
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
                    cmd1.Parameters.AddWithValue("@id", lessonId);
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand(
                        "DELETE FROM Lesson WHERE lessonid=@id",
                        con, trans);
                    cmd2.Parameters.AddWithValue("@id", lessonId);
                    cmd2.ExecuteNonQuery();

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        protected void btnReview_Click(object sender, EventArgs e)
        {
            Response.Redirect("editPublish.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

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
                cmd.Parameters.AddWithValue("@courseid", courseId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}