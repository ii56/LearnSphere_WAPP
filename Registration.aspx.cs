using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCrypt.Net;
using System.Data;

namespace LearnSphere_WAPP
{
    public partial class Registration : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowStep(1);
            }
        }

        // ================= STEP CONTROL =================
        private void ShowStep(int step)
        {
            pnlStep1.Visible = step == 1;
            pnlStep2.Visible = step == 2;
            pnlStep3.Visible = step == 3;

            lblStep.Text = step.ToString();
        }

        // ================= USERNAME VALIDATION =================
        protected void cvUsername_ServerValidate(object source, ServerValidateEventArgs args)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM [User] WHERE uname=@uname";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@uname", SqlDbType.NVarChar, 50).Value = uname.Text.Trim();

                int count = (int)cmd.ExecuteScalar();

                args.IsValid = count == 0;
            }
        }

        // ================= STEP 1 → STEP 2 =================
        protected void btnNext1_Click(object sender, EventArgs e)
        {
            Page.Validate("Step1");

            if (!Page.IsValid)
                return;

            ShowStep(2);
        }

        // ================= STEP 2 → STEP 3 =================
        protected void btnNext2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(email.Text))
            {
                errMsg.Text = "Email is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(pwd.Text))
            {
                errMsg.Text = "Password is required";
                return;
            }

            if (pwd.Text != pwd2.Text)
            {
                errMsg.Text = "Passwords do not match";
                return;
            }

            errMsg.Text = "";
            ShowStep(3);
        }

        // ================= BACK BUTTONS =================
        protected void btnBack1_Click(object sender, EventArgs e)
        {
            ShowStep(1);
        }

        protected void btnBack2_Click(object sender, EventArgs e)
        {
            ShowStep(2);
        }

        // ================= REGISTER =================
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // 🔐 BASIC VALIDATION
            if (string.IsNullOrWhiteSpace(fname.Text) ||
                string.IsNullOrWhiteSpace(lname.Text) ||
                string.IsNullOrWhiteSpace(age.Text) ||
                string.IsNullOrWhiteSpace(gender.SelectedValue))
            {
                errMsg.Text = "Please complete all fields";
                return;
            }

            int ageValue;
            if (!int.TryParse(age.Text, out ageValue) || ageValue < 1 || ageValue > 120)
            {
                errMsg.Text = "Invalid age";
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // 🔥 FINAL USERNAME CHECK (race condition protection)
                    string checkQuery = "SELECT COUNT(*) FROM [User] WHERE uname=@uname";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.Add("@uname", SqlDbType.NVarChar, 50).Value = uname.Text.Trim();

                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        errMsg.Text = "Username already exists!";
                        ShowStep(1);
                        return;
                    }

                    // 🔐 HASH PASSWORD
                    string password = pwd.Text.Trim();
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                    // 🔐 INSERT USER
                    string query = @"INSERT INTO [User]
                    (uname, email, pwd, fname, lname, age, gender, creationtime, deletiontime, usertype, status)
                    VALUES
                    (@uname, @email, @pwd, @fname, @lname, @age, @gender, @creationtime, NULL, @usertype, @status)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.Add("@uname", SqlDbType.NVarChar, 50).Value = uname.Text.Trim();
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email.Text.Trim();
                    cmd.Parameters.Add("@pwd", SqlDbType.NVarChar).Value = hashedPassword;

                    cmd.Parameters.Add("@fname", SqlDbType.NVarChar, 50).Value = fname.Text.Trim();
                    cmd.Parameters.Add("@lname", SqlDbType.NVarChar, 50).Value = lname.Text.Trim();

                    cmd.Parameters.Add("@age", SqlDbType.Int).Value = ageValue;
                    cmd.Parameters.Add("@gender", SqlDbType.NVarChar, 10).Value = gender.SelectedValue;

                    cmd.Parameters.Add("@creationtime", SqlDbType.DateTime).Value = DateTime.Now;

                    // 🔥 DEFAULT ROLE
                    cmd.Parameters.Add("@usertype", SqlDbType.NVarChar, 20).Value = "General";

                    cmd.Parameters.Add("@status", SqlDbType.NVarChar, 20).Value = "Active";

                    cmd.ExecuteNonQuery();

                    // ✅ SUCCESS
                    Session["RegistrationSuccess"] = "Registration successful! Please login.";
                    Response.Redirect("Login.aspx");
                }
            }
            catch (Exception)
            {
                errMsg.Text = "Registration failed. Please try again.";
            }
        }
    }
}