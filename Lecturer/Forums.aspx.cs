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

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadCourses();
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

        private void LoadCourses()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"
                        SELECT c.courseid, c.coursename,
                        CASE WHEN f.forumid IS NULL THEN 0 ELSE 1 END AS HasForum
                        FROM Course c
                        LEFT JOIN CourseForum f ON c.courseid = f.courseid
                        WHERE c.ownerid = @uid";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvCourses.DataSource = dt;
                    gvCourses.DataBind();
                }
            }
            catch
            {
                lblMessage.Text = "Error loading courses.";
            }
        }

        // 🔐 OWNERSHIP CHECK
        private bool IsCourseOwner(int courseId)
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

        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null)
                return;

            if (!int.TryParse(e.CommandArgument.ToString(), out int courseId) || courseId <= 0)
                return;

            // 🔐 AUTHORIZATION CHECK
            if (!IsCourseOwner(courseId))
            {
                lblMessage.Text = "Unauthorized action.";
                return;
            }

            switch (e.CommandName)
            {
                case "CreateForum":
                    Response.Redirect("CreateForum.aspx?courseid=" + courseId);
                    break;

                case "ViewForum":
                    Response.Redirect("ViewForum.aspx?courseid=" + courseId);
                    break;

                case "DeleteForum":
                    DeleteForum(courseId);
                    LoadCourses();
                    break;
            }
        }

        private void DeleteForum(int courseId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // 🔐 Ensure forum exists
                    string checkQuery = "SELECT forumid FROM CourseForum WHERE courseid=@cid";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;

                    object forumIdObj = checkCmd.ExecuteScalar();

                    if (forumIdObj == null)
                    {
                        lblMessage.Text = "Forum does not exist.";
                        return;
                    }

                    int forumId = Convert.ToInt32(forumIdObj);

                    // 🔐 SAFE DELETE (soft delete posts)
                    string deletePostsQuery = @"
                        UPDATE ForumPost
                        SET deletiontime = GETDATE()
                        WHERE forumid = @fid";

                    SqlCommand deletePostsCmd = new SqlCommand(deletePostsQuery, con);
                    deletePostsCmd.Parameters.Add("@fid", SqlDbType.Int).Value = forumId;
                    deletePostsCmd.ExecuteNonQuery();

                    // 🔐 DELETE FORUM
                    string deleteForumQuery = "DELETE FROM CourseForum WHERE forumid=@fid";

                    SqlCommand deleteForumCmd = new SqlCommand(deleteForumQuery, con);
                    deleteForumCmd.Parameters.Add("@fid", SqlDbType.Int).Value = forumId;

                    deleteForumCmd.ExecuteNonQuery();

                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = "Forum deleted successfully.";
                }
            }
            catch
            {
                lblMessage.Text = "Error deleting forum.";
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