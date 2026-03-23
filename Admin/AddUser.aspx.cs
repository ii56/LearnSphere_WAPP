using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace LearnSphere_WAPP.Admin
{
    public partial class AddUser : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
            }

            if (!IsPostBack)
            {

            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            string uname = txtUname.Text.Trim();
            string email = txtEmail.Text.Trim();
            string fname = txtFname.Text.Trim();
            string lname = txtLname.Text.Trim();
            int age = int.Parse(txtAge.Text);
            string gender = dropdownGender.SelectedValue;

            string pwd = txtPwd.Text.Trim();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pwd);

            string query = @"INSERT INTO [User]
    (uname, email, pwd, fname, lname, age, gender, creationtime, usertype, status)
    VALUES
    (@uname, @email, @pwd, @fname, @lname, @age, @gender, GETDATE(), 'General', 'Active')";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@uname", uname);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pwd", hashedPassword);
                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@lname", lname);
                cmd.Parameters.AddWithValue("@age", age);
                cmd.Parameters.AddWithValue("@gender", gender);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            lblMessage.Text = "✅ User added successfully!";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }

        protected void cvUsername_ServerValidate(object source, ServerValidateEventArgs args)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM [User] WHERE uname=@uname";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@uname", SqlDbType.NVarChar, 50).Value = args.Value.Trim();

                int count = (int)cmd.ExecuteScalar();

                args.IsValid = count == 0;
            }
        }
    }
}