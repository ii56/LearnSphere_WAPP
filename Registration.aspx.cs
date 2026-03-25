using BCrypt.Net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP
{
    public partial class Registration : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ShowStep(1);
        }

        // ── Step visibility ───────────────────────────────────────────────────
        private void ShowStep(int step)
        {
            pnlStep1.Visible = (step == 1);
            pnlStep2.Visible = (step == 2);
            pnlStep3.Visible = (step == 3);
            lblStep.Text = step.ToString();
        }

        // ── Username uniqueness ───────────────────────────────────────────────
        protected void cvUsername_ServerValidate(object source, ServerValidateEventArgs args)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM [User] WHERE uname=@uname", con);
                cmd.Parameters.Add("@uname", SqlDbType.NVarChar, 50).Value = uname.Text;
                con.Open();
                args.IsValid = (int)cmd.ExecuteScalar() == 0;
            }
        }

        // ── Email uniqueness (new validator on step 2) ────────────────────────
        protected void cvEmail_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Skip if email is coming from Google (already stored in hidden field)
            if (hfIsGoogleSignup.Value == "1") { args.IsValid = true; return; }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM [User] WHERE email=@email", con);
                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email.Text.Trim();
                con.Open();
                args.IsValid = (int)cmd.ExecuteScalar() == 0;
            }
        }

        // ── Google Sign-Up postback trigger ──────────────────────────────────
        // Called when JS fires the hidden button after the Google callback
        protected void btnGoogleRegisterTrigger_Click(object sender, EventArgs e)
        {
            string googleEmail = hfGoogleEmail.Value.Trim();
            string googleFname = hfGoogleFname.Value.Trim();
            string googleLname = hfGoogleLname.Value.Trim();

            if (string.IsNullOrEmpty(googleEmail))
            {
                errMsg.Text = "Could not retrieve your Google account details. Please try again.";
                ShowStep(1);
                return;
            }

            // Check if this Google email is already registered
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand chk = new SqlCommand(
                    "SELECT COUNT(*) FROM [User] WHERE email=@email", con);
                chk.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = googleEmail;
                con.Open();
                if ((int)chk.ExecuteScalar() > 0)
                {
                    errMsg.Text = "This Google account is already registered. Please log in instead.";
                    ShowStep(1);
                    return;
                }
            }

            // Pre-fill step 3 fields with Google data and hide the Google signup button
            fname.Text = googleFname;
            lname.Text = googleLname;
            pnlGoogleSignupBtn.Visible = false;

            // Show the Google email as read-only on step 2 and hide the editable email field
            lblGoogleEmailDisplay.Text = googleEmail;
            pnlEmailReadonly.Visible = true;
            pnlEmailEditable.Visible = false;

            // Jump to step 1 (username choice) — email/name are already stored in hidden fields
            ShowStep(1);
        }

        // ── Step 1 → Step 2 ──────────────────────────────────────────────────
        protected void btnNext1_Click(object sender, EventArgs e)
        {
            Page.Validate("Step1");
            if (!Page.IsValid) return;

            // If coming via Google, skip step 2 email entry — jump straight to step 3
            if (hfIsGoogleSignup.Value == "1")
            {
                ShowStep(2); // Show step 2 briefly so user can set a password
                return;
            }

            ShowStep(2);
        }

        // ── Step 2 → Step 3 ──────────────────────────────────────────────────
        protected void btnNext2_Click(object sender, EventArgs e)
        {
            // For Google users, email validation is skipped — only password matters
            bool isGoogle = hfIsGoogleSignup.Value == "1";

            if (!isGoogle)
            {
                Page.Validate("Step2");
                if (!Page.IsValid) return;

                if (string.IsNullOrWhiteSpace(email.Text))
                {
                    errMsg.Text = "Email is required.";
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(pwd.Text))
            {
                errMsg.Text = "Password is required.";
                return;
            }
            if (pwd.Text != pwd2.Text)
            {
                errMsg.Text = "Passwords do not match.";
                return;
            }

            ViewState["password"] = pwd.Text;
            // Use Google email if available, otherwise use the typed email
            ViewState["email"] = isGoogle ? hfGoogleEmail.Value.Trim() : email.Text.Trim();

            errMsg.Text = "";
            ShowStep(3);
        }

        // ── Back buttons ──────────────────────────────────────────────────────
        protected void btnBack1_Click(object sender, EventArgs e) { ShowStep(1); }
        protected void btnBack2_Click(object sender, EventArgs e) { ShowStep(2); }

        // ── Final Registration ────────────────────────────────────────────────
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fname.Text) ||
                string.IsNullOrWhiteSpace(lname.Text) ||
                string.IsNullOrWhiteSpace(age.Text) ||
                string.IsNullOrWhiteSpace(gender.SelectedValue))
            {
                errMsg.Text = "Please complete all fields.";
                return;
            }

            int ageValue;
            if (!int.TryParse(age.Text, out ageValue) || ageValue < 1 || ageValue > 120)
            {
                errMsg.Text = "Invalid age.";
                return;
            }

            string plainPassword = ViewState["password"]?.ToString();
            string emailValue = ViewState["email"]?.ToString();

            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(emailValue))
            {
                errMsg.Text = "Session expired. Please start registration again.";
                ShowStep(1);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Final duplicate username check
                    SqlCommand chkUser = new SqlCommand(
                        "SELECT COUNT(*) FROM [User] WHERE uname=@uname", con);
                    chkUser.Parameters.Add("@uname", SqlDbType.NVarChar, 50).Value = uname.Text;
                    if ((int)chkUser.ExecuteScalar() > 0)
                    {
                        errMsg.Text = "Username already exists.";
                        ShowStep(1);
                        return;
                    }

                    // Final duplicate email check
                    SqlCommand chkEmail = new SqlCommand(
                        "SELECT COUNT(*) FROM [User] WHERE email=@email", con);
                    chkEmail.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = emailValue;
                    if ((int)chkEmail.ExecuteScalar() > 0)
                    {
                        errMsg.Text = "This email is already registered. Please log in instead.";
                        ShowStep(2);
                        return;
                    }

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO [User]
                        (uname, email, pwd, fname, lname, age, gender, creationtime, deletiontime, usertype, status)
                        VALUES
                        (@uname, @email, @pwd, @fname, @lname, @age, @gender, @creationtime, NULL, @usertype, @status)",
                        con);

                    cmd.Parameters.Add("@uname", SqlDbType.NVarChar, 50).Value = uname.Text.Trim();
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = emailValue;
                    cmd.Parameters.Add("@pwd", SqlDbType.NVarChar).Value = hashedPassword;
                    cmd.Parameters.Add("@fname", SqlDbType.NVarChar, 50).Value = fname.Text.Trim();
                    cmd.Parameters.Add("@lname", SqlDbType.NVarChar, 50).Value = lname.Text.Trim();
                    cmd.Parameters.Add("@age", SqlDbType.Int).Value = ageValue;
                    cmd.Parameters.Add("@gender", SqlDbType.NVarChar, 10).Value = gender.SelectedValue;
                    cmd.Parameters.Add("@creationtime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@usertype", SqlDbType.NVarChar, 20).Value = "General";
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar, 20).Value = "Active";

                    cmd.ExecuteNonQuery();
                }

                Session["RegistrationSuccess"] = "Registration successful! Please log in.";
                Response.Redirect("Login.aspx");
            }
            catch (Exception ex)
            {
                errMsg.Text = "Registration failed: " + ex.Message;
            }
        }
    }
}