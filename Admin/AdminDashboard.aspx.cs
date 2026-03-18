using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
            }

            if (!Page.IsPostBack)
            {
                LoadSidebarProfileImage();
                lblWelcome.Text = "Welcome " + Session["uname"];
                con.Open();

                lblTotalUsers.Text = new SqlCommand("SELECT COUNT(*) FROM [User] where not status = 'Deleted'", con).ExecuteScalar().ToString();
                lblTotalStudents.Text = new SqlCommand("SELECT COUNT(*) FROM [User] where usertype = 'Student' and not status = 'Deleted'", con).ExecuteScalar().ToString();
                lblTotalLecturers.Text = new SqlCommand("SELECT COUNT(*) FROM [User] where usertype = 'Lecturer' and not status = 'Deleted'", con).ExecuteScalar().ToString();
                lblTotalCourses.Text = new SqlCommand("SELECT COUNT(*) FROM Course", con).ExecuteScalar().ToString();
                lblTotalForums.Text = new SqlCommand("SELECT COUNT(*) FROM ForumPost", con).ExecuteScalar().ToString();
                lecturersVal.Text = new SqlCommand("SELECT COUNT(*) FROM [User] where status = 'Pending' and usertype = 'Lecturer'", con).ExecuteScalar().ToString();
                studentsVal.Text = new SqlCommand("SELECT COUNT(*) FROM [User] where status = 'Pending' and usertype = 'Student'", con).ExecuteScalar().ToString();
                coursesVal.Text = new SqlCommand("SELECT COUNT(*) FROM course where status = 'Pending'", con).ExecuteScalar().ToString();

                con.Close();
            }
        }
        public void LoadSidebarProfileImage()
        {
            string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", Session["userid"]);

            con.Open();

            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                string imagePath = result.ToString();
                sidebarImg.Src = ResolveUrl(imagePath);
            }
            else
            {
                sidebarImg.Src = ResolveUrl("~/images/default-user.png");
            }
            con.Close();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }
    }
}