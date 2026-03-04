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

namespace LearnSphere_WAPP.Lecturer
{
    public partial class EditProfile : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadProfile();
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", userId);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string imagePath = result.ToString();
                    Session["profileImage"] = imagePath;
                    imgSidebarProfile.Src = ResolveUrl(imagePath);
                }
                else
                {
                    imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                }
            }
        }

        private void LoadProfile()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT uname, fname, lname, email, age, gender, 
                        ProfileImage, description
                 FROM [User] 
                 WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
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

                        if (!reader.IsDBNull(reader.GetOrdinal("ProfileImage")))
                        {
                            string imagePath = reader["ProfileImage"].ToString();

                            Session["profileImage"] = imagePath;
                        }
                    }
                }
            }
            LoadVerificationDocuments();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query;

                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text);

                    query = @"UPDATE [User]
                      SET fname=@fname,
                          lname=@lname,
                          email=@email,
                          age=@age,
                          gender=@gender,
                          description=@description,                          
                          pwd=@pwd
                      WHERE userid=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@pwd", hashedPassword);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@fname", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@lname", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtAge.Text));
                    cmd.Parameters.AddWithValue("@gender", ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text);

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    query = @"UPDATE [User]
                          SET fname=@fname,
                              lname=@lname,
                              email=@email,
                              age=@age,
                              gender=@gender,
                              description=@description
                          WHERE userid=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@fname", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@lname", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtAge.Text));
                    cmd.Parameters.AddWithValue("@gender", ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text);

                    cmd.ExecuteNonQuery();
                }

                if (fuProfileImage.HasFile)
                {
                    string extension = Path.GetExtension(fuProfileImage.FileName).ToLower();

                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        lblUploadMessage.Text = "Only JPG, JPEG or PNG files are allowed.";
                        return;
                    }

                    if (fuProfileImage.PostedFile.ContentLength > 3 * 1024 * 1024)
                    {
                        lblUploadMessage.Text = "File size must be less than 3MB.";
                        return;
                    }

                    string folderPath = Server.MapPath("~/Profile_pictures/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string fileName = userId + ".jpeg";
                    string fullPath = Path.Combine(folderPath, fileName);

                    if (File.Exists(fullPath))
                        File.Delete(fullPath);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        fuProfileImage.PostedFile.InputStream.CopyTo(ms);
                        ms.Position = 0;
                        SaveResizedImageAsJpeg(ms, fullPath);
                    }

                    string relativePath = "~/Profile_pictures/" + fileName;

                    SqlCommand imgCmd = new SqlCommand(
                        "UPDATE [User] SET ProfileImage=@img WHERE userid=@id", con);

                    imgCmd.Parameters.AddWithValue("@img", relativePath);
                    imgCmd.Parameters.AddWithValue("@id", userId);

                    imgCmd.ExecuteNonQuery();

                    Session["profileImage"] = relativePath;
                }
            }

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Profile updated successfully.";
        }

        private void SaveResizedImageAsJpeg(Stream imageStream, string savePath)
        {
            using (var originalImage = System.Drawing.Image.FromStream(imageStream, true, true))
            {
                int maxSize = 300;

                int newWidth = originalImage.Width;
                int newHeight = originalImage.Height;

                if (originalImage.Width > maxSize || originalImage.Height > maxSize)
                {
                    float ratio = Math.Min(
                        (float)maxSize / originalImage.Width,
                        (float)maxSize / originalImage.Height
                    );

                    newWidth = (int)(originalImage.Width * ratio);
                    newHeight = (int)(originalImage.Height * ratio);
                }

                using (var resizedImage = new System.Drawing.Bitmap(newWidth, newHeight))
                using (var graphics = System.Drawing.Graphics.FromImage(resizedImage))
                {
                    graphics.InterpolationMode =
                        System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);

                    resizedImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private void LoadVerificationDocuments()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT fileurl, filetype, uploadtime
                         FROM Material
                         WHERE userid = @id
                         AND filetype = 'VERIFICATION'
                         ORDER BY uploadtime DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", userId);

                con.Open();

                rptVerificationDocs.DataSource = cmd.ExecuteReader();
                rptVerificationDocs.DataBind();
            }
        }



        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        protected void btnUploadVerification_Click(object sender, EventArgs e)
        {
            if (!fuVerificationDoc.HasFile)
            {
                lblVerificationMsg.Text = "Please select a PDF document.";
                return;
            }

            int userId = Convert.ToInt32(Session["userid"]);
            string extension = Path.GetExtension(fuVerificationDoc.FileName).ToLower();

            if (extension != ".pdf")
            {
                lblVerificationMsg.Text = "Only PDF files are allowed.";
                return;
            }

            string folderPath = Server.MapPath("~/Uploads/VerificationDocs/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string newFileName = Guid.NewGuid().ToString() + extension;
            string fullPath = Path.Combine(folderPath, newFileName);

            fuVerificationDoc.SaveAs(fullPath);

            string relativePath = "~/Uploads/VerificationDocs/" + newFileName;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO Material
                         (clickcount, filetype, lessonid, userid, fileurl)
                         VALUES (0, 'VERIFICATION', NULL, @userid, @fileurl)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@fileurl", relativePath);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblVerificationMsg.ForeColor = System.Drawing.Color.Green;
            lblVerificationMsg.Text = "Document uploaded successfully.";

            LoadVerificationDocuments();
        }
    }
}