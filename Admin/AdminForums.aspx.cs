using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminForums1 : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadCourses();
            }
        }

        private void LoadSidebarProfileImage()
        {
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
                    sidebarImg.Src = ResolveUrl(imagePath);
                }
                else
                {
                    sidebarImg.Src = ResolveUrl("../images/default-user.png");
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
                    LEFT JOIN CourseForum f ON c.courseid = f.courseid";

                SqlCommand cmd = new SqlCommand(query, conn);

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
            
            if (e.CommandName == "DeleteForum")
            {
                DeleteForum(courseId);
                LoadCourses();
            }
            else if (e.CommandName == "ViewForum")
            {
                Response.Redirect("AdminViewForums.aspx?courseid=" + courseId);
            }
        }

        private void DeleteForum(int courseId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query1 = "SELECT forumid FROM CourseForum WHERE courseid = @courseid";

                SqlCommand cmd1 = new SqlCommand(query1, conn);
                cmd1.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;

                object result = cmd1.ExecuteScalar();

                if (result == null)
                {
                    return;
                }

                int forumId = Convert.ToInt32(result);

                string query2 = "DELETE FROM ForumPost WHERE forumid = @forumid";

                SqlCommand cmd2 = new SqlCommand(query2, conn);
                cmd2.Parameters.Add("@forumid", SqlDbType.Int).Value = forumId;

                cmd2.ExecuteNonQuery();

                string query3 = "DELETE FROM CourseForum WHERE courseid = @courseid";

                SqlCommand cmd3 = new SqlCommand(query3, conn);
                cmd3.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;

                cmd3.ExecuteNonQuery();
            }
        }

        protected void btnLogout_Click1(object sender, EventArgs e)
        {
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }
    }
}