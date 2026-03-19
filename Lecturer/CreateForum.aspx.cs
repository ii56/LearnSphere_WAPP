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
    public partial class CreateForum : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int courseId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure user is logged in and is a lecturer
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Validate course ID from query string
            if (!int.TryParse(Request.QueryString["courseid"], out courseId))
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            // Verify lecturer owns this course
            if (!IsLecturerCourseOwner(courseId))
            {
                Response.Redirect("Forums.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
            }
        }



        private bool IsLecturerCourseOwner(int courseId)
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Course WHERE courseid=@courseid AND ownerid=@userid";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userId;

                conn.Open();

                int result = (int)cmd.ExecuteScalar();

                return result > 0;
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



        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                string title = txtTitle.Text.Trim();
                string description = txtDescription.Text.Trim();
                string tags = txtTags.Text.Trim();

                // Required validation
                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                {
                    return;
                }

                // Length validation
                if (title.Length > 100)
                {
                    return;
                }

                if (description.Length > 1000)
                {
                    return;
                }

                if (tags.Length > 200)
                {
                    return;
                }

                // XSS protection
                title = Server.HtmlEncode(title);
                description = Server.HtmlEncode(description);
                tags = Server.HtmlEncode(tags);



                // Check for duplicate forum
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string duplicateQuery = @"
                        SELECT COUNT(*) 
                        FROM CourseForum 
                        WHERE courseid=@courseid AND title=@title";

                    SqlCommand checkCmd = new SqlCommand(duplicateQuery, conn);

                    checkCmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;
                    checkCmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = title;

                    conn.Open();

                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        return;
                    }
                }



                // Insert forum
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"
                        INSERT INTO CourseForum
                        (courseid, createdby, title, description, tags)
                        VALUES
                        (@courseid, @createdby, @title, @description, @tags)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;
                    cmd.Parameters.Add("@createdby", SqlDbType.Int).Value = Convert.ToInt32(Session["userid"]);
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = title;
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar, 1000).Value = description;
                    cmd.Parameters.Add("@tags", SqlDbType.NVarChar, 200).Value = tags;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Created Forum (CourseID: " + courseId + ")");
                }

                Response.Redirect("Forums.aspx");
            }
            catch
            {
                // Prevent database error exposure
            }
        }



        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("~/Login.aspx");
        }
    }
}