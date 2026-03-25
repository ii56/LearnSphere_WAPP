using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using BCrypt.Net;

namespace LearnSphere_WAPP
{
    public partial class Login : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

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

        // Standard username + password login
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Fetch hash by username only
                SqlCommand cmd = new SqlCommand(@"
                    SELECT userid, uname, usertype, fname, pwd
                    FROM [User]
                    WHERE LOWER(uname) = LOWER(@uname) AND status = 'Active'", con);
                cmd.Parameters.AddWithValue("@uname", uname.Text.Trim());

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string storedHash = reader["pwd"].ToString();
                    bool valid = BCrypt.Net.BCrypt.Verify(pwd.Text.Trim(), storedHash);

                    if (valid)
                    {
                        SetSession(reader);
                        string role = reader["usertype"].ToString();
                        reader.Close();
                        LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Login");
                        RedirectUser(role);
                    }
                    else
                    {
                        errMsg.Text = "Invalid username or password.";
                    }
                }
                else
                {
                    errMsg.Text = "Invalid username or password.";
                }
            }
        }

        // Google Sign-In: looks up the username by the Google email address,
        // pre-fills the username field and shows a note — the user still enters their password
        protected void btnGoogleLoginTrigger_Click(object sender, EventArgs e)
        {
            string googleEmail = hfGoogleUsername.Value.Trim();

            if (string.IsNullOrEmpty(googleEmail))
            {
                errMsg.Text = "Could not retrieve your Google account details. Please try again.";
                return;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT uname FROM [User] WHERE email=@email AND status='Active'", con);
                cmd.Parameters.AddWithValue("@email", googleEmail);
                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // Pre-fill the username field so the user only needs to enter their password
                    uname.Text = result.ToString();
                    pnlGoogleNote.Visible = true;
                }
                else
                {
                    errMsg.Text = "No account found for this Google email. Please register first.";
                }
            }
        }

        // Stores user info in session after a successful login
        private void SetSession(SqlDataReader reader)
        {
            Session["userid"] = reader["userid"];
            Session["uname"] = reader["uname"];
            Session["usertype"] = reader["usertype"];
            Session["fname"] = reader["fname"];
        }

        private void RedirectUser(string role)
        {
            switch (role)
            {
                case "SuperAdmin":
                case "Admin": Syslog.action(int.Parse(Session["userid"].ToString()), "Login"); Response.Redirect("~/Admin/AdminDashboard.aspx"); break;
                case "Lecturer": Syslog.action(int.Parse(Session["userid"].ToString()), "Login"); Response.Redirect("~/Lecturer/LecturerDashboard.aspx"); break;
                case "Student": Syslog.action(int.Parse(Session["userid"].ToString()), "Login"); Response.Redirect("~/Student/StudentDashboard.aspx"); break;
                case "General": Syslog.action(int.Parse(Session["userid"].ToString()), "Login"); Response.Redirect("~/GeneralUser/GeneralDashboard.aspx"); break;
                default: Response.Redirect("Login.aspx"); break;
            }
        }
    }
}