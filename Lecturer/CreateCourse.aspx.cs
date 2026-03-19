using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class CreateCourse : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // 🔐 CSRF PROTECTION
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 AUTH CHECK
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
            }
        }

        private int CurrentUserID
        {
            get
            {
                return Convert.ToInt32(Session["userid"]);
            }
        }

        // 🔐 PROFILE IMAGE SAFE LOAD
        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = CurrentUserID;

                con.Open();
                object result = cmd.ExecuteScalar();

                string imagePath = "~/images/default-user.png";

                if (result != null && result != DBNull.Value)
                {
                    string path = result.ToString();
                    if (path.StartsWith("~/images/"))
                        imagePath = path;
                }

                imgSidebarProfile.Src = ResolveUrl(imagePath);
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            // 🔐 RATE LIMIT (anti-spam)
            if (Session["lastCourseCreate"] != null)
            {
                DateTime last = (DateTime)Session["lastCourseCreate"];
                if ((DateTime.Now - last).TotalSeconds < 5)
                {
                    lblMessage.Text = "Please wait before creating another course.";
                    return;
                }
            }
            Session["lastCourseCreate"] = DateTime.Now;

            try
            {
                int ownerId = CurrentUserID;

                // 🔐 VERIFY USER STILL LECTURER (IDOR protection)
                if (!IsValidLecturer(ownerId))
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                // -------- INPUT --------
                string courseName = txtCourseName.Text.Trim();
                string description = txtDescription.Text.Trim();
                string category = ddlCategory.SelectedValue;

                // 🔐 VALIDATION
                if (courseName.Length < 3 || courseName.Length > 100)
                {
                    lblMessage.Text = "Invalid course name length.";
                    return;
                }

                if (description.Length < 10 || description.Length > 1000)
                {
                    lblMessage.Text = "Invalid description length.";
                    return;
                }

                string[] allowedCategories = { "AI", "Machine Learning", "Web Development", "Programming" };
                if (Array.IndexOf(allowedCategories, category) < 0)
                {
                    lblMessage.Text = "Invalid category.";
                    return;
                }

                // -------- PRICE --------
                decimal price;
                if (!decimal.TryParse(txtPrice.Text, out price))
                {
                    lblMessage.Text = "Invalid price.";
                    return;
                }

                if (price < 0 || price > 10000)
                {
                    lblMessage.Text = "Price out of range.";
                    return;
                }

                // -------- GENERATED THUMBNAIL --------
                string thumbnailPath = null;

                if (ViewState["ThumbnailPath"] != null)
                {
                    thumbnailPath = ViewState["ThumbnailPath"].ToString();
                }
                else
                {
                    // fallback → auto-generate if user didn’t click preview
                    string color = txtColor.Text;

                    if (string.IsNullOrWhiteSpace(color))
                        color = "#3b82f6";

                    thumbnailPath = GenerateThumbnail(color);
                }

                // -------- DATABASE --------
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query = @"
                INSERT INTO Course
                (ownerid, coursename, description, price, creationtime, deletiontime, category, thumbnail, status)
                OUTPUT INSERTED.courseid
                VALUES
                (@ownerid, @name, @desc, @price, GETDATE(), NULL, @category, @thumbnail, 0)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.Add("@ownerid", SqlDbType.Int).Value = ownerId;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = courseName;
                    cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 1000).Value = description;
                    cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = price;
                    cmd.Parameters.Add("@category", SqlDbType.NVarChar, 50).Value = category;

                    // 🔥 NEW THUMBNAIL PARAM
                    cmd.Parameters.Add("@thumbnail", SqlDbType.NVarChar, 255).Value = thumbnailPath;

                    int newCourseId = (int)cmd.ExecuteScalar();

                    Session["CurrentCourseID"] = newCourseId;
                }

                Response.Redirect("AddModules.aspx");
            }
            catch (Exception)
            {
                // 🔐 SAFE ERROR MESSAGE
                lblMessage.Text = "Something went wrong. Please try again.";
            }
        }


        private string GenerateThumbnail(string colorHex)
        {
            int width = 500;
            int height = 300;

            // 🔥 Ensure folder exists
            string folderPath = Server.MapPath("~/GeneratedThumbnails/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = "thumb_" + Guid.NewGuid().ToString() + ".png";
            string fullPath = Path.Combine(folderPath, fileName);

            try
            {
                using (Bitmap bmp = new Bitmap(width, height))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Base color
                    Color baseColor = ColorTranslator.FromHtml(colorHex);
                    g.Clear(baseColor);

                    Random rand = new Random();

                    // Draw random shapes
                    for (int i = 0; i < 20; i++)
                    {
                        int x = rand.Next(width);
                        int y = rand.Next(height);
                        int size = rand.Next(30, 120);

                        Color shapeColor = Color.FromArgb(
                            rand.Next(40, 120),
                            255, 255, 255);

                        using (Brush brush = new SolidBrush(shapeColor))
                        {
                            if (rand.Next(2) == 0)
                                g.FillEllipse(brush, x, y, size, size);
                            else
                                g.FillRectangle(brush, x, y, size, size);
                        }
                    }

                    // 🔥 IMPORTANT: Save AFTER drawing & inside using
                    bmp.Save(fullPath, ImageFormat.Png);
                }

                return "~/GeneratedThumbnails/" + fileName;
            }
            catch
            {
                return "~/images/default-course.png"; // fallback
            }
        }

        // 🔐 VERIFY LECTURER EXISTS
        private bool IsValidLecturer(int userId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = "SELECT COUNT(*) FROM [User] WHERE userid=@id AND usertype='Lecturer'";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Redirect("~/Login.aspx");
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            string color = txtColor.Text;

            string path = GenerateThumbnail(color);

            imgPreview.ImageUrl = ResolveUrl(path);

            // store for later saving
            ViewState["ThumbnailPath"] = path;
        }
    }
}