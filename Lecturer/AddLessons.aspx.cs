using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class AddLessons : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Role validation
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (Session["CurrentCourseID"] == null)
            {
                Response.Redirect("CreateCourse.aspx");
                return;
            }

            if (Session["CurrentModuleID"] == null)
            {
                Response.Redirect("AddModules.aspx");
                return;
            }

            if (Session["CurrentCourseID"] == null)
            {
                Response.Redirect("ViewCourses.aspx"); // better UX
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadModuleTitle();
                LoadLessons();
                ViewState["Step"] = "3";
            }
        }


        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
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


        private void LoadModuleTitle()
        {
            int moduleId = Convert.ToInt32(Session["CurrentModuleID"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT modulename FROM Module WHERE moduleid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", moduleId);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    lblModuleName.Text = Server.HtmlEncode(result.ToString());
                }
            }
        }


        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                int moduleId = Convert.ToInt32(Session["CurrentModuleID"]);

                // -------------------------
                // INPUT SANITIZATION
                // -------------------------

                string lessonTitle = Server.HtmlEncode(txtLessonTitle.Text.Trim());
                string lessonDesc = Server.HtmlEncode(txtLessonDesc.Text.Trim());
                string videoUrl = Server.HtmlEncode(txtVideoUrl.Text.Trim());

                if (string.IsNullOrWhiteSpace(lessonTitle))
                {
                    lblMessage.Text = "Lesson title is required.";
                    return;
                }

                if (lessonTitle.Length > 100)
                {
                    lblMessage.Text = "Lesson title cannot exceed 100 characters.";
                    return;
                }

                if (lessonDesc.Length > 1000)
                {
                    lblMessage.Text = "Description is too long.";
                    return;
                }


                // -------------------------
                // DURATION VALIDATION
                // -------------------------

                int duration;

                if (!int.TryParse(txtDuration.Text, out duration))
                {
                    lblMessage.Text = "Duration must be a valid number.";
                    return;
                }

                if (duration <= 0 || duration > 600)
                {
                    lblMessage.Text = "Duration must be between 1 and 600 minutes.";
                    return;
                }


                // -------------------------
                // VIDEO URL VALIDATION
                // -------------------------

                if (!string.IsNullOrWhiteSpace(videoUrl))
                {
                    Uri uriResult;

                    if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out uriResult))
                    {
                        lblMessage.Text = "Invalid video URL.";
                        return;
                    }
                }


                int newLessonId;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // -------------------------
                    // INSERT LESSON
                    // -------------------------

                    string lessonQuery = @"
                        INSERT INTO Lesson
                        (moduleid, lessontitle, lessondescription, duration, ordernumber, creationtime, deletiontime)
                        OUTPUT INSERTED.lessonid
                        VALUES
                        (
                            @moduleid,
                            @title,
                            @desc,
                            @duration,
                            (
                                SELECT ISNULL(MAX(ordernumber),0) + 1
                                FROM Lesson
                                WHERE moduleid = @moduleid
                                AND deletiontime IS NULL
                            ),
                            GETDATE(),
                            NULL
                        )";

                    SqlCommand lessonCmd = new SqlCommand(lessonQuery, con);

                    lessonCmd.Parameters.AddWithValue("@moduleid", moduleId);
                    lessonCmd.Parameters.AddWithValue("@title", lessonTitle);
                    lessonCmd.Parameters.AddWithValue("@desc", lessonDesc);
                    lessonCmd.Parameters.AddWithValue("@duration", duration);

                    newLessonId = (int)lessonCmd.ExecuteScalar();


                    // -------------------------
                    // VIDEO MATERIAL
                    // -------------------------

                    if (!string.IsNullOrWhiteSpace(videoUrl))
                    {
                        string videoQuery = @"
                            INSERT INTO Material
                            (clickcount, filetype, lessonid, fileurl, videourl)
                            VALUES
                            (0, 'URL', @lessonid, NULL, @videourl)";

                        SqlCommand videoCmd = new SqlCommand(videoQuery, con);
                        videoCmd.Parameters.AddWithValue("@lessonid", newLessonId);
                        videoCmd.Parameters.AddWithValue("@videourl", videoUrl);

                        videoCmd.ExecuteNonQuery();
                    }


                    // -------------------------
                    // FILE UPLOAD VALIDATION
                    // -------------------------

                    if (fuLessonFile.HasFile)
                    {
                        string extension = Path.GetExtension(fuLessonFile.FileName).ToLower();

                        string[] allowedExtensions =
                        {
                            ".pdf",".doc",".docx",".ppt",".pptx"
                        };

                        if (!allowedExtensions.Contains(extension))
                        {
                            lblMessage.Text = "Invalid file type.";
                            return;
                        }

                        if (fuLessonFile.PostedFile.ContentLength > 5 * 1024 * 1024)
                        {
                            lblMessage.Text = "File must be smaller than 5MB.";
                            return;
                        }

                        string folder = Server.MapPath("~/Uploads/LessonMaterials/");

                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        string newFileName = Guid.NewGuid().ToString() + extension;

                        string savePath = Path.Combine(folder, newFileName);

                        fuLessonFile.SaveAs(savePath);

                        string fileUrl = "~/Uploads/LessonMaterials/" + newFileName;

                        string fileQuery = @"
                            INSERT INTO Material
                            (clickcount, filetype, lessonid, fileurl, videourl)
                            VALUES
                            (0, @filetype, @lessonid, @fileurl, NULL)";

                        SqlCommand fileCmd = new SqlCommand(fileQuery, con);

                        fileCmd.Parameters.AddWithValue("@lessonid", newLessonId);
                        fileCmd.Parameters.AddWithValue("@fileurl", fileUrl);
                        fileCmd.Parameters.AddWithValue("@filetype", extension);

                        fileCmd.ExecuteNonQuery();
                        LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Lesson Added (LessonID: " + newLessonId + ")");
                    }
                }


                // -------------------------
                // RESET FORM
                // -------------------------

                txtLessonTitle.Text = "";
                txtLessonDesc.Text = "";
                txtVideoUrl.Text = "";
                txtDuration.Text = "";

                lblMessage.Text = "Lesson added successfully.";

                LoadLessons();
            }
            catch
            {
                lblMessage.Text = "An error occurred while adding the lesson.";
            }
        }


        private void LoadLessons()
        {
            int moduleId = Convert.ToInt32(Session["CurrentModuleID"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT lessontitle, duration
                                 FROM Lesson
                                 WHERE moduleid = @moduleid
                                 AND deletiontime IS NULL";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@moduleid", moduleId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvLessons.DataSource = dt;
                gvLessons.DataBind();
            }
        }


        protected void btnGoToReview_Click(object sender, EventArgs e)
        {
            Response.Redirect("ReviewPublish.aspx");
        }


        protected void btnBackToModules_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddModules.aspx");
        }


        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}