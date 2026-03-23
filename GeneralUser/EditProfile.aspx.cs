using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using BCrypt.Net;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class EditProfile : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null || Session["usertype"].ToString() != "General")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProfile();
                LoadVerificationHistory();
            }
        }

        private void LoadProfile()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT uname, email, fname, lname, age, gender, creationtime, ProfileImage, Description 
                    FROM [User] WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string fname = reader["fname"].ToString();
                            string lname = reader["lname"].ToString();
                            string email = reader["email"].ToString();
                            string uname = reader["uname"].ToString();
                            string gender = reader["gender"] != DBNull.Value ? reader["gender"].ToString().Trim() : "";
                            string desc = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "";
                            int age = Convert.ToInt32(reader["age"]);
                            DateTime joined = Convert.ToDateTime(reader["creationtime"]);
                            string profileImg = reader["ProfileImage"] != DBNull.Value ? reader["ProfileImage"].ToString() : "";

                            // Header
                            lblHeaderName.Text = fname;
                            lblAvatarInitial.Text = fname.Substring(0, 1).ToUpper();
                            Session["fname"] = fname;

                            // Hero
                            lblFullName.Text = fname + " " + lname;
                            lblEmail.Text = email;
                            lblHeroInitial.Text = fname.Substring(0, 1).ToUpper();

                            // View Panel
                            lblFname.Text = fname;
                            lblLname.Text = lname;
                            lblEmailView.Text = email;
                            lblAge.Text = age.ToString();
                            lblGender.Text = string.IsNullOrEmpty(gender) ? "Not Specified" : gender;
                            lblBio.Text = string.IsNullOrEmpty(desc) ? "No description provided." : desc;
                            lblUsername.Text = uname;
                            lblJoined.Text = joined.ToString("MMMM dd, yyyy");

                            // Edit Panel
                            txtFname.Text = fname;
                            txtLname.Text = lname;
                            txtEmail.Text = email;
                            txtAge.Text = age.ToString();
                            txtDescription.Text = desc;
                            if (ddlGender.Items.FindByValue(gender) != null) ddlGender.SelectedValue = gender;

                            // Profile Picture Elements
                            lblUploadInitial.Text = fname.Substring(0, 1).ToUpper();
                            if (!string.IsNullOrEmpty(profileImg))
                            {
                                string resolvedImg = ResolveUrl(profileImg);

                                imgProfile.ImageUrl = resolvedImg;
                                pnlProfilePic.Visible = true;
                                pnlProfileInitial.Visible = false;

                                imgUploadPreview.ImageUrl = resolvedImg;
                                pnlUploadPreview.Visible = true;
                                pnlUploadInitial.Visible = false;

                                imgHeaderAvatar.ImageUrl = resolvedImg;
                                imgHeaderAvatar.Visible = true;
                                lblAvatarInitial.Visible = false;
                            }
                            else
                            {
                                pnlProfilePic.Visible = false;
                                pnlProfileInitial.Visible = true;
                                pnlUploadPreview.Visible = false;
                                pnlUploadInitial.Visible = true;
                                imgHeaderAvatar.Visible = false;
                                lblAvatarInitial.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        private void LoadVerificationHistory()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT requestedrole, status, requesttime, remarks 
                    FROM VerificationRequest 
                    WHERE userid=@uid 
                    ORDER BY requesttime DESC", con);
                cmd.Parameters.AddWithValue("@uid", userId);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    if (dt.Rows.Count > 0)
                    {
                        rptVerificationHistory.DataSource = dt;
                        rptVerificationHistory.DataBind();
                        lblNoHistory.Visible = false;
                    }
                    else
                    {
                        rptVerificationHistory.DataSource = null;
                        rptVerificationHistory.DataBind();
                        lblNoHistory.Visible = true;
                    }
                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            pnlView.Visible = false;
            pnlEdit.Visible = true;
            lblMessage.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlView.Visible = true;
            pnlEdit.Visible = false;
            lblMessage.Visible = false;
            LoadProfile();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFname.Text) || string.IsNullOrWhiteSpace(txtLname.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtAge.Text))
            {
                ShowMessage("Please fill in all required fields.", false);
                return;
            }

            int ageVal;
            if (!int.TryParse(txtAge.Text, out ageVal) || ageVal < 13 || ageVal > 120)
            {
                ShowMessage("Please enter a valid age (13-120).", false);
                return;
            }

            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowMessage("Invalid email format.", false);
                return;
            }

            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE [User] SET fname=@fname, lname=@lname, email=@email, age=@age, gender=@gender, Description=@desc
                        WHERE userid=@uid", con))
                    {
                        cmd.Parameters.AddWithValue("@fname", Server.HtmlEncode(txtFname.Text.Trim()));
                        cmd.Parameters.AddWithValue("@lname", Server.HtmlEncode(txtLname.Text.Trim()));
                        cmd.Parameters.AddWithValue("@email", Server.HtmlEncode(txtEmail.Text.Trim()));
                        cmd.Parameters.AddWithValue("@age", ageVal);
                        cmd.Parameters.AddWithValue("@gender", ddlGender.SelectedValue);
                        cmd.Parameters.AddWithValue("@desc", Server.HtmlEncode(txtDescription.Text.Trim()));
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.ExecuteNonQuery();

                        LearnSphere_WAPP.Syslog.action(userId, "Updated Personal Information");
                    }
                }

                Session["fname"] = txtFname.Text.Trim();

                pnlView.Visible = true;
                pnlEdit.Visible = false;
                ShowMessage("Profile updated successfully!", true);
                LoadProfile();
            }
            catch (Exception ex)
            {
                ShowMessage("Error saving profile: " + ex.Message, false);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fuProfilePic.HasFile)
            {
                ShowMessage("Please select an image file to upload.", false);
                return;
            }

            string ext = Path.GetExtension(fuProfilePic.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
            {
                ShowMessage("Only JPG and PNG files are allowed.", false);
                return;
            }

            if (fuProfilePic.PostedFile.ContentLength > 3 * 1024 * 1024)
            {
                ShowMessage("File size must be less than 3MB.", false);
                return;
            }

            try
            {
                // Verify it's a real image
                using (var img = System.Drawing.Image.FromStream(fuProfilePic.PostedFile.InputStream)) { }

                int userId = Convert.ToInt32(Session["userid"]);
                string fileName = "profile_" + userId + "_" + DateTime.Now.Ticks + ext;
                string folderPath = Server.MapPath("~/Profile_pictures/");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, fileName);
                fuProfilePic.SaveAs(filePath);

                string dbPath = "~/Profile_pictures/" + fileName;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE [User] SET ProfileImage = @img WHERE userid = @uid", con))
                    {
                        cmd.Parameters.AddWithValue("@img", dbPath);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.ExecuteNonQuery();
                    }
                }

                Session["profileImage"] = dbPath;
                LearnSphere_WAPP.Syslog.action(userId, "Updated Profile Picture");

                ShowMessage("Profile picture updated successfully!", true);
                LoadProfile();
            }
            catch
            {
                ShowMessage("Invalid image file. Please try a different photo.", false);
            }
        }

        protected void btnChangePwd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPwd.Text) || string.IsNullOrWhiteSpace(txtNewPwd.Text) || string.IsNullOrWhiteSpace(txtConfirmPwd.Text))
            {
                ShowMessage("Please fill in all password fields.", false);
                return;
            }

            if (txtNewPwd.Text != txtConfirmPwd.Text)
            {
                ShowMessage("New password and confirmation do not match.", false);
                return;
            }

            if (txtNewPwd.Text.Length < 6)
            {
                ShowMessage("New password must be at least 6 characters.", false);
                return;
            }

            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string storedHash = "";
                    using (SqlCommand cmd = new SqlCommand("SELECT pwd FROM [User] WHERE userid = @uid", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        object res = cmd.ExecuteScalar();
                        if (res != null) storedHash = res.ToString();
                    }

                    if (!BCrypt.Net.BCrypt.Verify(txtCurrentPwd.Text, storedHash))
                    {
                        ShowMessage("Current password is incorrect.", false);
                        return;
                    }

                    string newHash = BCrypt.Net.BCrypt.HashPassword(txtNewPwd.Text);
                    using (SqlCommand cmd = new SqlCommand("UPDATE [User] SET pwd = @pwd WHERE userid = @uid", con))
                    {
                        cmd.Parameters.AddWithValue("@pwd", newHash);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.ExecuteNonQuery();
                        LearnSphere_WAPP.Syslog.action(userId, "Changed Password");
                    }
                }

                txtCurrentPwd.Text = "";
                txtNewPwd.Text = "";
                txtConfirmPwd.Text = "";

                ShowMessage("Password changed successfully!", true);
            }
            catch (Exception ex)
            {
                ShowMessage("Error changing password: " + ex.Message, false);
            }
        }

        protected void btnSendVerification_Click(object sender, EventArgs e)
        {
            if (!fuVerificationDoc.HasFile)
            {
                lblVerificationMsg.Text = "Please upload a supporting document.";
                lblVerificationMsg.CssClass = "alert alert-error";
                return;
            }

            string ext = Path.GetExtension(fuVerificationDoc.FileName).ToLower();
            if (ext != ".pdf")
            {
                lblVerificationMsg.Text = "Only PDF documents are allowed.";
                lblVerificationMsg.CssClass = "alert alert-error";
                return;
            }

            if (fuVerificationDoc.PostedFile.ContentLength > 5 * 1024 * 1024)
            {
                lblVerificationMsg.Text = "File size must be less than 5MB.";
                lblVerificationMsg.CssClass = "alert alert-error";
                return;
            }

            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Check for existing pending request
                    using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM VerificationRequest WHERE userid=@uid AND status='Pending'", con))
                    {
                        checkCmd.Parameters.AddWithValue("@uid", userId);
                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            lblVerificationMsg.Text = "You already have a pending request under review.";
                            lblVerificationMsg.CssClass = "alert alert-error";
                            return;
                        }
                    }

                    string folder = Server.MapPath("~/Uploads/VerificationDocs/");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = "LecturerReq_" + userId + "_" + Guid.NewGuid().ToString().Substring(0, 8) + ".pdf";
                    fuVerificationDoc.SaveAs(Path.Combine(folder, fileName));
                    string dbPath = "~/Uploads/VerificationDocs/" + fileName;

                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO VerificationRequest (userid, currentrole, requestedrole, documentpath, status, requesttime) 
                        VALUES (@uid, 'General', 'Lecturer', @doc, 'Pending', GETDATE())", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@doc", dbPath);
                        cmd.ExecuteNonQuery();

                        LearnSphere_WAPP.Syslog.action(userId, "Submitted Request to become Lecturer");
                    }
                }

                lblVerificationMsg.Text = "Your upgrade request has been submitted successfully!";
                lblVerificationMsg.CssClass = "alert alert-success";
                LoadVerificationHistory();
            }
            catch (Exception ex)
            {
                lblVerificationMsg.Text = "Error submitting request: " + ex.Message;
                lblVerificationMsg.CssClass = "alert alert-error";
            }
        }

        private void ShowMessage(string text, bool success)
        {
            lblMessage.Text = text;
            lblMessage.CssClass = "alert " + (success ? "alert-success" : "alert-error");
            lblMessage.Visible = true;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}