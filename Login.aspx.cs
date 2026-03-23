using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCrypt.Net;

namespace LearnSphere_WAPP
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["RegistrationSuccess"] != null)
                {
                    errMsg.ForeColor = System.Drawing.Color.Green;
                    errMsg.Text = Session["RegistrationSuccess"].ToString();
                    Session["RegistrationSuccess"] = null;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                // Fetch the stored hash by username only
                string query = @"SELECT userid, uname, usertype, pwd
                                 FROM [User]
                                 WHERE LOWER(uname) = LOWER(@uname)
                                 AND status = 'Active'";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@uname", uname.Text.Trim());
                    cmd.Parameters.AddWithValue("@pwd", pwd.Text.Trim());

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // LOGIN SUCCESS
                        Session["userid"] = reader["userid"];
                        Session["uname"] = reader["uname"];
                        Session["usertype"] = reader["usertype"];

                        RedirectUser(reader["usertype"].ToString());
                    }
                    else
                    {
                        errMsg.Text = "Invalid username or password.";
                    }
                }
            }
        }

        private void RedirectUser(string role)
        {
            switch (role)
            {
                case "SuperAdmin":
                    Response.Redirect("~/Admin/AdminDashboard.aspx");
                    break;

                case "Admin":
                    Response.Redirect("~/Admin/AdminDashboard.aspx");
                    break;

                case "Lecturer":
                    Response.Redirect("~/Lecturer/LecturerDashboard.aspx");
                    break;

                case "Student":
                    Response.Redirect("~/Student/StudentDashboard.aspx");
                    break;

                case "General":
                    Syslog.action(Convert.ToInt32(Session["userid"]), "Log In");
                    Response.Redirect("~/GeneralUser/GeneralDashboard.aspx");
                    break;

                default:
                    Response.Redirect("Login.aspx");
                    break;
            }
        }
    }
}