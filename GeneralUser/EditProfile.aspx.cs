using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class EditProfile : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProfileData();
            }
        }

        private void LoadProfileData()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "SELECT uname, fname, lname, email, age, gender, description, ProfileImage FROM [User] WHERE userid = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtUsername.Text = reader["uname"].ToString();
                                txtFirstName.Text = reader["fname"].ToString();
                                txtLastName.Text = reader["lname"].ToString();
                                txtEmail.Text = reader["email"].ToString();
                                txtAge.Text = reader["age"].ToString();
                                ddlGender.SelectedValue = reader["gender"].ToString();
                                txtDescription.Text = reader["description"]?.ToString();

                                string imgPath = reader["ProfileImage"] != DBNull.Value ? reader["ProfileImage"].ToString() : "~/images/default-user.png";
                                imgSidebarProfile.Src = ResolveUrl(imgPath);
                                imgLargePreview.Src = ResolveUrl(imgPath);
                                Session["profileImage"] = imgPath;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading profile: " + ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string updateSql = @"UPDATE [User] SET fname=@fname, lname=@lname, email=@email, 
                                        age=@age, gender=@gender, description=@desc";

                    if (!string.IsNullOrEmpty(txtPassword.Text))
                        updateSql += ", pwd=@pwd";

                    updateSql += " WHERE userid=@id";

                    using (SqlCommand cmd = new SqlCommand(updateSql, con))
                    {
                        cmd.Parameters.AddWithValue("@fname", txtFirstName.Text);
                        cmd.Parameters.AddWithValue("@lname", txtLastName.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);

                        // Handle empty age safely
                        int age = 0;
                        int.TryParse(txtAge.Text, out age);
                        cmd.Parameters.AddWithValue("@age", age);

                        cmd.Parameters.AddWithValue("@gender", ddlGender.SelectedValue);
                        cmd.Parameters.AddWithValue("@desc", txtDescription.Text);
                        cmd.Parameters.AddWithValue("@id", userId);

                        if (!string.IsNullOrEmpty(txtPassword.Text))
                            cmd.Parameters.AddWithValue("@pwd", BCrypt.Net.BCrypt.HashPassword(txtPassword.Text));

                        cmd.ExecuteNonQuery();
                    }

                    if (fuProfileImage.HasFile)
                    {
                        SaveProfileImage(userId, con);
                    }
                }

                lblMessage.Text = "Profile updated successfully!";
                lblMessage.ForeColor = System.Drawing.Color.Green;
                LoadProfileData(); // Refresh UI images
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Profile Save Error: " + ex.Message);
                lblMessage.Text = "Error updating profile. Please try again.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void SaveProfileImage(int userId, SqlConnection con)
        {
            string ext = Path.GetExtension(fuProfileImage.FileName).ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                string folderPath = Server.MapPath("~/Profile_pictures/");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string fileName = userId + ".jpeg";
                string fullPath = Path.Combine(folderPath, fileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    fuProfileImage.PostedFile.InputStream.CopyTo(ms);
                    ms.Position = 0;

                    using (var img = System.Drawing.Image.FromStream(ms))
                    {
                        int max = 300;
                        int w = img.Width, h = img.Height;
                        if (w > max || h > max)
                        {
                            float ratio = Math.Min((float)max / w, (float)max / h);
                            w = (int)(w * ratio); h = (int)(h * ratio);
                        }
                        using (var bmp = new System.Drawing.Bitmap(w, h))
                        using (var g = System.Drawing.Graphics.FromImage(bmp))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.DrawImage(img, 0, 0, w, h);
                            bmp.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                }

                string relativePath = "~/Profile_pictures/" + fileName;
                using (SqlCommand cmd = new SqlCommand("UPDATE [User] SET ProfileImage=@path WHERE userid=@uid", con))
                {
                    cmd.Parameters.AddWithValue("@path", relativePath);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- UPDATED LECTURER VERIFICATION REQUEST METHOD ---
        protected void btnUploadVerification_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["userid"]);
            string currentRole = Session["usertype"] != null ? Session["usertype"].ToString() : "General";

            if (fuVerificationDoc.HasFile && Path.GetExtension(fuVerificationDoc.FileName).ToLower() == ".pdf")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();

                        // 1. SAFETY CHECK: Prevent multiple pending requests
                        using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM VerificationRequest WHERE userid = @uid AND status = 'Pending'", con))
                        {
                            checkCmd.Parameters.AddWithValue("@uid", userId);
                            int pendingCount = (int)checkCmd.ExecuteScalar();

                            if (pendingCount > 0)
                            {
                                lblVerificationMsg.Text = "You already have a pending upgrade request under review.";
                                lblVerificationMsg.ForeColor = System.Drawing.Color.DarkOrange;
                                return;
                            }
                        }

                        // 2. Save the PDF File to the Server
                        string folder = Server.MapPath("~/Uploads/VerificationDocs/");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                        // Use a unique file name to prevent overwriting
                        string fileName = "LecturerReq_" + userId + "_" + Guid.NewGuid().ToString().Substring(0, 8) + ".pdf";
                        string savePath = Path.Combine(folder, fileName);
                        string relativePath = "~/Uploads/VerificationDocs/" + fileName;

                        fuVerificationDoc.SaveAs(savePath);

                        // 3. Insert the Request into the VerificationRequest Table
                        string insertQuery = @"
                            INSERT INTO VerificationRequest (userid, currentrole, requestedrole, documentpath, status, requesttime) 
                            VALUES (@uid, @currentRole, 'Lecturer', @docPath, 'Pending', GETDATE())";

                        using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.Parameters.AddWithValue("@currentRole", currentRole);
                            cmd.Parameters.AddWithValue("@docPath", relativePath);

                            cmd.ExecuteNonQuery();
                        }

                        // 4. Update UI
                        lblVerificationMsg.Text = "Request sent successfully! Please wait for admin approval.";
                        lblVerificationMsg.ForeColor = System.Drawing.Color.Green;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Verification Upload Error: " + ex.Message);
                    lblVerificationMsg.Text = "An error occurred while sending your request. Please try again.";
                    lblVerificationMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblVerificationMsg.Text = "Please upload a valid PDF document.";
                lblVerificationMsg.ForeColor = System.Drawing.Color.Red;
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