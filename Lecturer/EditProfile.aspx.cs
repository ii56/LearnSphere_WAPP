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
                if (Session["profileImage"] != null)
                {
                    imgSidebarProfile.Src = ResolveUrl(Session["profileImage"].ToString());
                }
                else
                {
                    imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                }

                LoadSidebarProfileImage();
                LoadProfile();

                LoadRoleOptions();
                LoadVerificationHistory();
            }
        }
        private string GetCurrentRoleFromDB()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = "SELECT usertype FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }

        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();
                object result = cmd.ExecuteScalar();

                imgSidebarProfile.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        private void LoadProfile()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT uname, fname, lname, email, age, gender, description, ProfileImage
                                 FROM [User]
                                 WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
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
                    ddlGender.SelectedValue = reader["gender"].ToString();
                    txtDescription.Text = reader["description"]?.ToString();
                }
            }
        }

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

            // ROLE VALIDATION (VERY IMPORTANT)
            string currentRole = GetCurrentRoleFromDB();
            string requestedRole = ddlRequestedRole.SelectedValue;

            bool valid = false;

            if (currentRole == "General" &&
                (requestedRole == "Student" || requestedRole == "Lecturer"))
                valid = true;

            else if (currentRole == "Lecturer" && requestedRole == "Admin")
                valid = true;

            else if (currentRole == "Admin" && requestedRole == "SuperAdmin")
                valid = true;

            if (!valid)
            {
                lblVerificationMsg.Text = "Invalid role request.";
                return;
            }

            // CHECK EXISTING PENDING REQUEST
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string check = @"
SELECT COUNT(*) 
FROM VerificationRequest
WHERE userid=@uid 
AND requestedrole=@role
AND status='Pending'";

                SqlCommand checkCmd = new SqlCommand(check, con);
                checkCmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                checkCmd.Parameters.Add("@role", SqlDbType.NVarChar).Value = requestedRole;
                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    lblVerificationMsg.Text = "You already have a pending request.";
                    return;
                }
            }

            // -------- SAVE FILE --------
            string folder = Server.MapPath("~/Uploads/VerificationDocs/");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + ".pdf";
            string fullPath = Path.Combine(folder, fileName);

            fuVerificationDoc.SaveAs(fullPath);

            string relPath = "~/Uploads/VerificationDocs/" + fileName;

            // -------- INSERT REQUEST --------
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
            INSERT INTO VerificationRequest
            (userid, currentrole, requestedrole, documentpath, status, requesttime)
            VALUES
            (@uid, @current, @requested, @doc, 'Pending', GETDATE())";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@current", SqlDbType.NVarChar).Value = currentRole;
                cmd.Parameters.Add("@requested", SqlDbType.NVarChar).Value = requestedRole;
                cmd.Parameters.Add("@doc", SqlDbType.NVarChar).Value = relPath;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblVerificationMsg.ForeColor = System.Drawing.Color.Green;
            lblVerificationMsg.Text = "Verification request sent successfully.";

            LoadVerificationHistory();
        }

        private void LoadVerificationHistory()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
            SELECT requestedrole, status, requesttime
            FROM VerificationRequest
            WHERE userid=@uid
            ORDER BY requesttime DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;

                con.Open();
                rptVerificationHistory.DataSource = cmd.ExecuteReader();
                rptVerificationHistory.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string fname = txtFirstName.Text.Trim();
                string lname = txtLastName.Text.Trim();
                string desc = txtDescription.Text.Trim();
                string password = txtPassword.Text;

                // 🔒 VALIDATION
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

                // XSS PROTECTION
                fname = Server.HtmlEncode(fname);
                lname = Server.HtmlEncode(lname);
                email = Server.HtmlEncode(email);
                desc = Server.HtmlEncode(desc);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query;

                    if (!string.IsNullOrEmpty(password))
                    {
                        if (password.Length < 6)
                        {
                            lblMessage.Text = "Password must be at least 6 characters.";
                            return;
                        }

                        string hashed = BCrypt.Net.BCrypt.HashPassword(password);

                        query = @"UPDATE [User]
                                  SET fname=@fname, lname=@lname, email=@email,
                                      age=@age, gender=@gender, description=@desc,
                                      pwd=@pwd
                                  WHERE userid=@id";

                        SqlCommand cmd = new SqlCommand(query, con);
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
                        query = @"UPDATE [User]
                                  SET fname=@fname, lname=@lname, email=@email,
                                      age=@age, gender=@gender, description=@desc
                                  WHERE userid=@id";

                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@fname", SqlDbType.NVarChar, 50).Value = fname;
                        cmd.Parameters.Add("@lname", SqlDbType.NVarChar, 50).Value = lname;
                        cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email;
                        cmd.Parameters.Add("@age", SqlDbType.Int).Value = age;
                        cmd.Parameters.Add("@gender", SqlDbType.NVarChar, 10).Value = ddlGender.SelectedValue;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;

                        cmd.ExecuteNonQuery();
                    }

                    // 🔒 IMAGE UPLOAD SECURITY
                    if (fuProfileImage.HasFile)
                    {
                        string ext = Path.GetExtension(fuProfileImage.FileName).ToLower();

                        if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                        {
                            lblUploadMessage.Text = "Only JPG/PNG allowed.";
                            return;
                        }

                        if (fuProfileImage.PostedFile.ContentLength > 3 * 1024 * 1024)
                        {
                            lblUploadMessage.Text = "Max size 3MB.";
                            return;
                        }

                        // Validate actual image
                        try
                        {
                            using (var img = System.Drawing.Image.FromStream(fuProfileImage.PostedFile.InputStream))
                            {
                                // valid image
                            }
                        }
                        catch
                        {
                            lblUploadMessage.Text = "Invalid image file.";
                            return;
                        }

                        string folder = Server.MapPath("~/Profile_pictures/");
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        string fileName = userId + ".jpg";
                        string path = Path.Combine(folder, fileName);

                        using (var img = System.Drawing.Image.FromStream(fuProfileImage.PostedFile.InputStream))
                        {
                            img.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }

                        string relPath = "~/Profile_pictures/" + fileName;

                        SqlCommand imgCmd = new SqlCommand(
                            "UPDATE [User] SET ProfileImage=@img WHERE userid=@id", con);

                        imgCmd.Parameters.Add("@img", SqlDbType.NVarChar).Value = relPath;
                        imgCmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                        imgCmd.ExecuteNonQuery();

                        Session["profileImage"] = relPath;
                    }
                }

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Profile updated successfully.";
            }
            catch
            {
                lblMessage.Text = "Error updating profile.";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}