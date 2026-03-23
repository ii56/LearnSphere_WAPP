using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class EditProfile : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        int userId;

        // Redirects unauthenticated users, then loads all profile data on first visit
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadProfile();
                LoadRoleOptions();
                LoadVerificationHistory();
            }
        }

        // Fetches the user's current role directly from the DB rather than trusting the session
        private string GetCurrentRoleFromDB()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT usertype FROM [User] WHERE userid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                con.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }

        // Sets the profile picture in both the header avatar and the hero banner, and keeps the session in sync
        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ProfileImage FROM [User] WHERE userid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                con.Open();
                object result = cmd.ExecuteScalar();

                string resolvedImg = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");

                imgSidebarProfile.Src = resolvedImg;
                imgHeroProfile.Src = resolvedImg;

                if (result != null && result != DBNull.Value)
                    Session["profileImage"] = result.ToString();
            }
        }

        // Fills every form field with the user's current data, including the hero banner email
        private void LoadProfile()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT uname, fname, lname, email, age, gender, description, ProfileImage
                    FROM [User] WHERE userid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtUsername.Text = reader["uname"].ToString();
                    txtFirstName.Text = reader["fname"].ToString();
                    txtLastName.Text = reader["lname"].ToString();
                    txtEmail.Text = reader["email"].ToString();
                    txtAge.Text = reader["age"].ToString();
                    txtDescription.Text = reader["description"]?.ToString();

                    string gender = reader["gender"].ToString();
                    if (ddlGender.Items.FindByValue(gender) != null)
                        ddlGender.SelectedValue = gender;

                    lblHeroEmail.Text = Server.HtmlEncode(reader["email"].ToString());
                }
            }
        }

        // Populates the role upgrade dropdown based on what the current role is allowed to request
        private void LoadRoleOptions()
        {
            string role = Session["usertype"].ToString();
            txtCurrentRole.Text = role;

            ddlRequestedRole.Items.Clear();
            ddlRequestedRole.Items.Add(new ListItem("Select Role", ""));

            if (role == "General")
            {
                ddlRequestedRole.Items.Add("Student");
                ddlRequestedRole.Items.Add("Lecturer");
            }
            else if (role == "Lecturer")
            {
                ddlRequestedRole.Items.Add("Admin");
            }
            else if (role == "Admin")
            {
                ddlRequestedRole.Items.Add("SuperAdmin");
            }
        }

        // Validates the uploaded PDF and requested role, then submits a verification request to the admin
        protected void btnSendVerification_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlRequestedRole.SelectedValue))
            {
                lblVerificationMsg.Text = "Please select a role.";
                return;
            }

            if (!fuVerificationDoc.HasFile)
            {
                lblVerificationMsg.Text = "Please upload a document.";
                return;
            }

            string ext = Path.GetExtension(fuVerificationDoc.FileName).ToLower();
            string mime = fuVerificationDoc.PostedFile.ContentType;

            if (ext != ".pdf" || mime != "application/pdf")
            {
                lblVerificationMsg.Text = "Only valid PDF files allowed.";
                return;
            }

            if (fuVerificationDoc.PostedFile.ContentLength > 5 * 1024 * 1024)
            {
                lblVerificationMsg.Text = "File too large (max 5MB).";
                return;
            }

            string currentRole = GetCurrentRoleFromDB();
            string requestedRole = ddlRequestedRole.SelectedValue;

            // Only allow role transitions that make sense in the hierarchy
            bool valid =
                (currentRole == "General" && (requestedRole == "Student" || requestedRole == "Lecturer")) ||
                (currentRole == "Lecturer" && requestedRole == "Admin") ||
                (currentRole == "Admin" && requestedRole == "SuperAdmin");

            if (!valid)
            {
                lblVerificationMsg.Text = "Invalid role request.";
                return;
            }

            // Block duplicate pending requests for the same role
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM VerificationRequest
                    WHERE userid=@uid AND requestedrole=@role AND status='Pending'", con);
                checkCmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                checkCmd.Parameters.Add("@role", SqlDbType.NVarChar).Value = requestedRole;

                if ((int)checkCmd.ExecuteScalar() > 0)
                {
                    lblVerificationMsg.Text = "You already have a pending request.";
                    return;
                }
            }

            // Save the document with a GUID name to avoid conflicts
            string folder = Server.MapPath("~/Uploads/VerificationDocs/");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + ".pdf";
            fuVerificationDoc.SaveAs(Path.Combine(folder, fileName));
            string relPath = "~/Uploads/VerificationDocs/" + fileName;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO VerificationRequest
                    (userid, currentrole, requestedrole, documentpath, status, requesttime)
                    VALUES (@uid, @current, @requested, @doc, 'Pending', GETDATE())", con);
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@current", SqlDbType.NVarChar).Value = currentRole;
                cmd.Parameters.Add("@requested", SqlDbType.NVarChar).Value = requestedRole;
                cmd.Parameters.Add("@doc", SqlDbType.NVarChar).Value = relPath;
                con.Open();
                cmd.ExecuteNonQuery();
                LearnSphere_WAPP.Syslog.action(userId, "Send Verification Request to Admin");
            }

            lblVerificationMsg.ForeColor = System.Drawing.Color.Green;
            lblVerificationMsg.Text = "Verification request sent successfully.";
            LoadVerificationHistory();
        }

        // Loads the user's past verification requests so they can track their request history
        private void LoadVerificationHistory()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT requestedrole, status, requesttime
                    FROM VerificationRequest
                    WHERE userid=@uid
                    ORDER BY requesttime DESC", con);
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                con.Open();
                rptVerificationHistory.DataSource = cmd.ExecuteReader();
                rptVerificationHistory.DataBind();
            }
        }

        // Validates and saves changes to personal info, optionally updates the password and profile picture
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string fname = txtFirstName.Text.Trim();
                string lname = txtLastName.Text.Trim();
                string desc = txtDescription.Text.Trim();
                string password = txtPassword.Text;

                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    lblMessage.Text = "Invalid email format.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(lname))
                {
                    lblMessage.Text = "Name fields cannot be empty.";
                    return;
                }

                int age;
                if (!int.TryParse(txtAge.Text, out age) || age < 13 || age > 120)
                {
                    lblMessage.Text = "Invalid age.";
                    return;
                }

                fname = Server.HtmlEncode(fname);
                lname = Server.HtmlEncode(lname);
                email = Server.HtmlEncode(email);
                desc = Server.HtmlEncode(desc);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    if (!string.IsNullOrEmpty(password))
                    {
                        if (password.Length < 6)
                        {
                            lblMessage.Text = "Password must be at least 6 characters.";
                            return;
                        }

                        // Hash the new password before storing it
                        string hashed = BCrypt.Net.BCrypt.HashPassword(password);

                        SqlCommand cmd = new SqlCommand(@"
                            UPDATE [User]
                            SET fname=@fname, lname=@lname, email=@email,
                                age=@age, gender=@gender, description=@desc, pwd=@pwd
                            WHERE userid=@id", con);
                        cmd.Parameters.Add("@pwd", SqlDbType.NVarChar).Value = hashed;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@fname", SqlDbType.NVarChar, 50).Value = fname;
                        cmd.Parameters.Add("@lname", SqlDbType.NVarChar, 50).Value = lname;
                        cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email;
                        cmd.Parameters.Add("@age", SqlDbType.Int).Value = age;
                        cmd.Parameters.Add("@gender", SqlDbType.NVarChar, 10).Value = ddlGender.SelectedValue;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // No password change — just update the personal info fields
                        SqlCommand cmd = new SqlCommand(@"
                            UPDATE [User]
                            SET fname=@fname, lname=@lname, email=@email,
                                age=@age, gender=@gender, description=@desc
                            WHERE userid=@id", con);
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@fname", SqlDbType.NVarChar, 50).Value = fname;
                        cmd.Parameters.Add("@lname", SqlDbType.NVarChar, 50).Value = lname;
                        cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email;
                        cmd.Parameters.Add("@age", SqlDbType.Int).Value = age;
                        cmd.Parameters.Add("@gender", SqlDbType.NVarChar, 10).Value = ddlGender.SelectedValue;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.ExecuteNonQuery();
                    }

                    // Profile picture — JPG/PNG only, max 3 MB
                    // Saved using the user ID as the filename so old pictures are automatically overwritten
                    if (fuProfileImage.HasFile)
                    {
                        string imgExt = Path.GetExtension(fuProfileImage.FileName).ToLower();

                        if (imgExt != ".jpg" && imgExt != ".jpeg" && imgExt != ".png")
                        {
                            lblUploadMessage.Text = "Only JPG/PNG allowed.";
                            return;
                        }

                        if (fuProfileImage.PostedFile.ContentLength > 3 * 1024 * 1024)
                        {
                            lblUploadMessage.Text = "Max size 3MB.";
                            return;
                        }

                        // Verify the file is actually a valid image before saving it
                        try
                        {
                            using (var img = System.Drawing.Image.FromStream(fuProfileImage.PostedFile.InputStream)) { }
                        }
                        catch
                        {
                            lblUploadMessage.Text = "Invalid image file.";
                            return;
                        }

                        string imgFolder = Server.MapPath("~/Profile_pictures/");
                        if (!Directory.Exists(imgFolder)) Directory.CreateDirectory(imgFolder);

                        string imgFileName = userId + ".jpg";
                        string imgPath = Path.Combine(imgFolder, imgFileName);

                        using (var img = System.Drawing.Image.FromStream(fuProfileImage.PostedFile.InputStream))
                        {
                            img.Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }

                        string relPath = "~/Profile_pictures/" + imgFileName;

                        SqlCommand imgCmd = new SqlCommand(
                            "UPDATE [User] SET ProfileImage=@img WHERE userid=@id", con);
                        imgCmd.Parameters.Add("@img", SqlDbType.NVarChar).Value = relPath;
                        imgCmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                        imgCmd.ExecuteNonQuery();

                        Session["profileImage"] = relPath;
                        LearnSphere_WAPP.Syslog.action(userId, "Updated Profile");

                        // Refresh both header and hero avatars immediately so the page doesn't look stale
                        string resolved = ResolveUrl(relPath);
                        imgSidebarProfile.Src = resolved;
                        imgHeroProfile.Src = resolved;
                    }
                }

                // Keep the hero banner email in sync with whatever was just saved
                lblHeroEmail.Text = Server.HtmlEncode(email);

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Profile updated successfully.";
            }
            catch
            {
                lblMessage.Text = "Error updating profile.";
            }
        }

        // Clears the session and sends the user back to the login page
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(userId, "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}