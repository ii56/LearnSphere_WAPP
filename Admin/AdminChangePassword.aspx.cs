using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminChangePassword : System.Web.UI.Page
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome " + Session["uname"];
                LoadSidebarProfileImage();
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
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("~/Login.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtNewPassword.Text == txtNewPassword2.Text)
            {
                var passwordManager = new PasswordManager();
                string hashedPassword = passwordManager.HashPassword(txtNewPassword.Text);

                con.Open();

                string query = "update [User] set pwd = '" + hashedPassword + "' where userid = '" + Session["userid"] + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Update Password");
                Response.Write("<script>alert('Password change successfully'); window.location='AdminEditProfile.aspx';</script>");

            }
        }

        private class PasswordManager
        {
            public string HashPassword(string password)
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
        }
    }
}