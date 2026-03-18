using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class EditProfile : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

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

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT uname, fname, lname, email, age, gender, description, ProfileImage FROM [User] WHERE userid = @id";
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

                        string imgPath = reader["ProfileImage"] != DBNull.Value ? reader["ProfileImage"].ToString() : "~/images/default-user.png";
                        imgSidebarProfile.Src = ResolveUrl(imgPath);
                        imgLargePreview.Src = ResolveUrl(imgPath);
                        Session["profileImage"] = imgPath;
                    }
                }
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

                    SqlCommand cmd = new SqlCommand(updateSql, con);
                    cmd.Parameters.AddWithValue("@fname", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@lname", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtAge.Text));
                    cmd.Parameters.AddWithValue("@gender", ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@desc", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@id", userId);

                    if (!string.IsNullOrEmpty(txtPassword.Text))
                        cmd.Parameters.AddWithValue("@pwd", BCrypt.Net.BCrypt.HashPassword(txtPassword.Text));

                    cmd.ExecuteNonQuery();

                    if (fuProfileImage.HasFile)
                    {
                        SaveProfileImage(userId, con);
                    }
                }

                lblMessage.Text = "Profile updated successfully!";
                lblMessage.ForeColor = System.Drawing.Color.Green;
                LoadProfileData();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
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

                    // Logic for Resizing
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
                new SqlCommand($"UPDATE [User] SET ProfileImage='{relativePath}' WHERE userid={userId}", con).ExecuteNonQuery();
            }
        }

        protected void btnUploadVerification_Click(object sender, EventArgs e)
        {
            if (fuVerificationDoc.HasFile && Path.GetExtension(fuVerificationDoc.FileName).ToLower() == ".pdf")
            {
                string folder = Server.MapPath("~/Uploads/VerificationDocs/");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + ".pdf";
                fuVerificationDoc.SaveAs(Path.Combine(folder, fileName));

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string q = "INSERT INTO Material (clickcount, filetype, lessonid, userid, fileurl, uploadtime) VALUES (0, 'VERIFICATION', NULL, @uid, @url, GETDATE())";
                    SqlCommand cmd = new SqlCommand(q, con);
                    cmd.Parameters.AddWithValue("@uid", Session["userid"]);
                    cmd.Parameters.AddWithValue("@url", "~/Uploads/VerificationDocs/" + fileName);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                lblVerificationMsg.Text = "Request sent!";
                lblVerificationMsg.ForeColor = System.Drawing.Color.Green;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx");
        }
    }
}