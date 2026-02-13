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

                    string query = @"SELECT * FROM [User] WHERE uname=@uname AND usertype=@usertype AND status=1";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@uname", uname.Text);
                    cmd.Parameters.AddWithValue("@usertype", usertype.SelectedValue);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string storedHash = reader["pwd"].ToString();

                        if (BCrypt.Net.BCrypt.Verify(pwd.Text, storedHash))
                        {
                            Session["userid"] = reader["userid"];
                            Session["uname"] = reader["uname"];
                            Session["usertype"] = reader["usertype"];

                            RedirectUser(usertype.SelectedValue);
                        }
                        else
                        {
                            errMsg.Text = "Invalid password.";
                        }
                    }
                    else
                    {
                        errMsg.Text = "User not found or inactive.";
                    }
                }
            }
        }

        private void RedirectUser(string role)
        {
            switch (role)
            {
                case "Admin":
                    Response.Redirect("AdminDashboard.aspx");
                    break;

                case "Lecturer":
                    Response.Redirect("LecturerDashboard.aspx");
                    break;

                case "Student":
                    Response.Redirect("StudentDashboard.aspx");
                    break;

                case "Public":
                    Response.Redirect("PublicHome.aspx");
                    break;
            }
        }
    }
}