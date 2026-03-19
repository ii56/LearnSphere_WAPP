using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using BCrypt.Net;

namespace LearnSphere_WAPP.Student
{
    public partial class StudentProfile : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null) { Response.Redirect("~/Login.aspx"); return; }

            if (!IsPostBack)
                LoadProfile();
        }

        private void LoadProfile()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(
                    "SELECT uname, email, fname, lname, age, gender, creationtime, ProfileImage FROM [User] WHERE userid = @uid", con))
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
                            string gender = reader["gender"] != null ? reader["gender"].ToString().Trim() : "";
                            int age = Convert.ToInt32(reader["age"]);
                            DateTime joined = Convert.ToDateTime(reader["creationtime"]);
                            string profileImg = reader["ProfileImage"] != DBNull.Value ? reader["ProfileImage"].ToString() : "";

                            // header
                            lblHeaderName.Text = fname;
                            lblAvatarInitial.Text = fname.Substring(0, 1).ToUpper();
                            Session["fname"] = fname;

                            // hero section
                            lblFullName.Text = fname + " " + lname;
                            lblEmail.Text = email;
                            lblHeroInitial.Text = fname.Substring(0, 1).ToUpper();

                            // view mode labels
                            lblFname.Text = fname;
                            lblLname.Text = lname;
                            lblEmailView.Text = email;
                            lblAge.Text = age.ToString();
                            lblGender.Text = gender;
                            lblUsername.Text = uname;
                            lblJoined.Text = joined.ToString("MMMM dd, yyyy");

                            // edit mode fields
                            txtFname.Text = fname;
                            txtLname.Text = lname;
                            txtEmail.Text = email;
                            txtAge.Text = age.ToString();
                            if (gender == "Male") ddlGender.SelectedValue = "Male";
                            else if (gender == "Female") ddlGender.SelectedValue = "Female";

                            // profile picture
                            lblUploadInitial.Text = fname.Substring(0, 1).ToUpper();
                            if (!string.IsNullOrEmpty(profileImg))
                            {
                                imgProfile.ImageUrl = ResolveUrl(profileImg);
                                pnlProfilePic.Visible = true;
                                pnlProfileInitial.Visible = false;
                                imgUploadPreview.ImageUrl = ResolveUrl(profileImg);
                                pnlUploadPreview.Visible = true;
                                pnlUploadInitial.Visible = false;
                            }
                            else
                            {
                                pnlProfilePic.Visible = false;
                                pnlProfileInitial.Visible = true;
                                pnlUploadPreview.Visible = false;
                                pnlUploadInitial.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            pnlView.Visible = false;
            pnlEdit.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlView.Visible = true;
            pnlEdit.Visible = false;
            LoadProfile();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // validate inputs
            if (string.IsNullOrWhiteSpace(txtFname.Text) || string.IsNullOrWhiteSpace(txtLname.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtAge.Text))
            {
                ShowMessage("Please fill in all fields.", false);
                return;
            }

            int ageVal;
            if (!int.TryParse(txtAge.Text, out ageVal) || ageVal < 1 || ageVal > 120)
            {
                ShowMessage("Please enter a valid age.", false);
                return;
            }

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE [User] SET fname=@fname, lname=@lname, email=@email, age=@age, gender=@gender
                    WHERE userid=@uid", con))
                {
                    cmd.Parameters.AddWithValue("@fname", txtFname.Text.Trim());
                    cmd.Parameters.AddWithValue("@lname", txtLname.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@age", ageVal);
                    cmd.Parameters.AddWithValue("@gender", ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            // update session
            Session["fname"] = txtFname.Text.Trim();

            pnlView.Visible = true;
            pnlEdit.Visible = false;
            ShowMessage("Profile updated successfully!", true);
            LoadProfile();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fuProfilePic.HasFile)
            {
                ShowMessage("Please select a file to upload.", false);
                return;
            }

            // check file type
            string ext = Path.GetExtension(fuProfilePic.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif")
            {
                ShowMessage("Only JPG, PNG, and GIF files are allowed.", false);
                return;
            }

            // check file size (max 2MB)
            if (fuProfilePic.PostedFile.ContentLength > 2 * 1024 * 1024)
            {
                ShowMessage("File size must be less than 2MB.", false);
                return;
            }

            try
            {
                int userId = Convert.ToInt32(Session["userid"]);
                string fileName = "profile_" + userId + "_" + DateTime.Now.Ticks + ext;
                string folderPath = Server.MapPath("~/Profile_pictures/");

                // create folder if it doesnt exist
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, fileName);
                fuProfilePic.SaveAs(filePath);

                string dbPath = "~/Profile_pictures/" + fileName;

                // update database
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

                ShowMessage("Profile picture updated!", true);
                LoadProfile();
            }
            catch (Exception ex)
            {
                ShowMessage("Error uploading: " + ex.Message, false);
            }
        }

        protected void btnChangePwd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPwd.Text) || string.IsNullOrWhiteSpace(txtNewPwd.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPwd.Text))
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

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // get current stored hash
                string storedHash = "";
                using (SqlCommand cmd = new SqlCommand("SELECT pwd FROM [User] WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    storedHash = cmd.ExecuteScalar().ToString();
                }

                // verify current password
                if (!BCrypt.Net.BCrypt.Verify(txtCurrentPwd.Text, storedHash))
                {
                    ShowMessage("Current password is incorrect.", false);
                    return;
                }

                // hash new password and save
                string newHash = BCrypt.Net.BCrypt.HashPassword(txtNewPwd.Text);
                using (SqlCommand cmd = new SqlCommand("UPDATE [User] SET pwd = @pwd WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@pwd", newHash);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            // clear the fields
            txtCurrentPwd.Text = "";
            txtNewPwd.Text = "";
            txtConfirmPwd.Text = "";

            ShowMessage("Password changed successfully!", true);
        }

        private void ShowMessage(string text, bool success)
        {
            lblMessage.Text = text;
            lblMessage.CssClass = success ? "alert alert-success" : "alert alert-error";
            lblMessage.Visible = true;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}