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

        // Entry point — redirect if not a lecturer, otherwise load the page
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

        // Pulls the lecturer's profile picture and sets it in the header avatar
        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ProfileImage FROM [User] WHERE userid = @id", con);
                cmd.Parameters.AddWithValue("@id", userId);

                con.Open();
                object result = cmd.ExecuteScalar();

                imgSidebarProfile.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        // Loads all the stat cards and the recent courses table in one DB connection
        private void LoadDashboardData()
        {
            int lecturerId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Total courses this lecturer owns (excluding soft-deleted ones)
                SqlCommand cmdTotalCourses = new SqlCommand(
                    "SELECT COUNT(*) FROM Course WHERE ownerid=@id AND deletiontime IS NULL", con);
                cmdTotalCourses.Parameters.AddWithValue("@id", lecturerId);
                lblTotalCourses.Text = cmdTotalCourses.ExecuteScalar().ToString();

                // Unique students who have paid for at least one of this lecturer's courses
                SqlCommand cmdTotalStudents = new SqlCommand(@"
                    SELECT COUNT(DISTINCT i.userid)
                    FROM Invoice i
                    INNER JOIN Course c ON i.courseid = c.courseid
                    WHERE c.ownerid = @id", con);
                cmdTotalStudents.Parameters.AddWithValue("@id", lecturerId);
                lblTotalStudents.Text = cmdTotalStudents.ExecuteScalar().ToString();

                // Paid vs free course counts
                SqlCommand cmdPaidCourses = new SqlCommand(
                    "SELECT COUNT(*) FROM Course WHERE ownerid=@id AND price > 0 AND deletiontime IS NULL", con);
                cmdPaidCourses.Parameters.AddWithValue("@id", lecturerId);
                lblPaidCourses.Text = cmdPaidCourses.ExecuteScalar().ToString();

                SqlCommand cmdFreeCourses = new SqlCommand(
                    "SELECT COUNT(*) FROM Course WHERE ownerid=@id AND price = 0 AND deletiontime IS NULL", con);
                cmdFreeCourses.Parameters.AddWithValue("@id", lecturerId);
                lblFreeCourses.Text = cmdFreeCourses.ExecuteScalar().ToString();

                // Top 5 most recently created active courses for the quick-view table
                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT TOP 5 coursename, category, price, creationtime
                    FROM Course
                    WHERE ownerid = @id AND deletiontime IS NULL AND status = 'Active'
                    ORDER BY creationtime DESC", con);
                da.SelectCommand.Parameters.AddWithValue("@id", lecturerId);

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvTopCourses.DataSource = dt;
                gvTopCourses.DataBind();
            }
        }

        // Clears the session and sends the lecturer back to the login page
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}