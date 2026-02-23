using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class Forums : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadCourses();
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

        protected void gvCourses_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LoadCourses()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT c.courseid, c.coursename,
                    CASE WHEN f.forumid IS NULL THEN 0 ELSE 1 END AS HasForum
                    FROM Course c
                    LEFT JOIN CourseForum f ON c.courseid = f.courseid
                    WHERE c.ownerid = @ownerid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ownerid", Session["userid"]);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvCourses.DataSource = dt;
                gvCourses.DataBind();
            }
        }

        protected void gvCourses_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null)
                return;

            int courseId;
            if (!int.TryParse(e.CommandArgument.ToString(), out courseId))
                return;

            if (e.CommandName == "CreateForum")
            {
                Response.Redirect("CreateForum.aspx?courseid=" + courseId);
            }
            else if (e.CommandName == "DeleteForum")
            {
                DeleteForum(courseId);
                LoadCourses();
            }
            else if (e.CommandName == "ViewForum")
            {
                Response.Redirect("ViewForum.aspx?courseid=" + courseId);
            }
        }

        private void DeleteForum(int courseId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM CourseForum WHERE courseid = @courseid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}