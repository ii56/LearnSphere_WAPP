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
            if (Session["uname"] == null)
            {
                Response.Redirect("../Login.aspx");
            }

            if (!Page.IsPostBack)
            {
                con.Open();

                lblTotalUsers.Text = new SqlCommand("SELECT COUNT(*) FROM [User]", con).ExecuteScalar().ToString();
                lblTotalStudents.Text = new SqlCommand("SELECT COUNT(*) FROM [User]", con).ExecuteScalar().ToString();
                lblTotalLecturers.Text = new SqlCommand("SELECT COUNT(*) FROM [User]", con).ExecuteScalar().ToString();
                lblTotalCourses.Text = new SqlCommand("SELECT COUNT(*) FROM Course", con).ExecuteScalar().ToString();
                lblTotalForums.Text = new SqlCommand("SELECT COUNT(*) FROM ForumPost", con).ExecuteScalar().ToString();

                con.Close();
            }

            Syslog.action(123, "Dashboard");
        }
    }
}