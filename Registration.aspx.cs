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
    public partial class Registration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void cvUsername_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM [User] WHERE uname = @uname";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@uname", uname.Text);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                    args.IsValid = false;
                else
                    args.IsValid = true;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pwd.Text);

                        string query = @"INSERT INTO [User]
                        (uname, email, pwd, fname, lname, age, gender, creationtime, deletiontime, usertype, status)
                        VALUES
                        (@uname, @email, @pwd, @fname, @lname, @age, @gender, @creationtime, NULL, @usertype, @status)";

                        SqlCommand cmd = new SqlCommand(query, con);

                        cmd.Parameters.AddWithValue("@uname", uname.Text);
                        cmd.Parameters.AddWithValue("@email", email.Text);
                        cmd.Parameters.AddWithValue("@pwd", hashedPassword);
                        cmd.Parameters.AddWithValue("@fname", fname.Text);
                        cmd.Parameters.AddWithValue("@lname", lname.Text);
                        cmd.Parameters.AddWithValue("@age", age.Text);
                        cmd.Parameters.AddWithValue("@gender", gender.SelectedValue);
                        cmd.Parameters.AddWithValue("@creationtime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@usertype", usertype.SelectedValue);
                        cmd.Parameters.AddWithValue("@status", 1);

                        cmd.ExecuteNonQuery();

                        Session["RegistrationSuccess"] = "Registration successful! Please login";
                        Response.Redirect("Login.aspx");
                    }
                }
                catch (Exception ex)
                {
                    errMsg.ForeColor = System.Drawing.Color.Red;
                    errMsg.Text = "Registration failed: " + ex.Message;
                }
            }
        }
    }
}