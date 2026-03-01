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
    public partial class LecturerDashboard : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome, " + Session["uname"];
                LoadSidebarProfileImage();
                LoadDashboardData();
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

        private void LoadDashboardData()
        {
            int lecturerId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmdTotalCourses = new SqlCommand("SELECT COUNT(*) FROM Course WHERE ownerid=@id AND deletiontime IS NULL", con);

                cmdTotalCourses.Parameters.AddWithValue("@id", lecturerId);
                lblTotalCourses.Text = cmdTotalCourses.ExecuteScalar().ToString();

                SqlCommand cmdTotalStudents = new SqlCommand(@"SELECT COUNT(DISTINCT i.userid)
                                                            FROM Invoice i
                                                            INNER JOIN Course c ON i.courseid = c.courseid
                                                            WHERE c.ownerid = @id", con);

                cmdTotalStudents.Parameters.AddWithValue("@id", lecturerId);
                lblTotalStudents.Text = cmdTotalStudents.ExecuteScalar().ToString();

                SqlCommand cmdPaidCourses = new SqlCommand( "SELECT COUNT(*) FROM Course WHERE ownerid=@id AND price > 0 AND deletiontime IS NULL", con);

                cmdPaidCourses.Parameters.AddWithValue("@id", lecturerId);
                lblPaidCourses.Text = cmdPaidCourses.ExecuteScalar().ToString();

                SqlCommand cmdFreeCourses = new SqlCommand( "SELECT COUNT(*) FROM Course WHERE ownerid=@id AND price = 0 AND deletiontime IS NULL", con);

                cmdFreeCourses.Parameters.AddWithValue("@id", lecturerId);
                lblFreeCourses.Text = cmdFreeCourses.ExecuteScalar().ToString();
                SqlDataAdapter da = new SqlDataAdapter(@"SELECT TOP 5
                                        coursename,
                                        category,
                                        price,
                                        creationtime
                                        FROM Course
                                        WHERE ownerid = @id
                                        AND deletiontime IS NULL
                                        AND status = 1
                                        ORDER BY creationtime DESC", con);

                da.SelectCommand.Parameters.AddWithValue("@id", lecturerId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvTopCourses.DataSource = dt;
                gvTopCourses.DataBind();
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