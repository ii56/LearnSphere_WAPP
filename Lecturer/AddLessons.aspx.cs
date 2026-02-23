using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
            if (!IsPostBack)
            {
                LoadModuleTitle();
                LoadLessons();
            }
            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                ViewState["Step"] = "3";
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
                    lblModuleName.Text = result.ToString();
                }
            }
        }

        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLessonTitle.Text))
            {
                lblMessage.Text = "Lesson title is required.";
                return;
            }

            int duration = 0;
            if (!int.TryParse(txtDuration.Text, out duration))
            {
                lblMessage.Text = "Duration must be a valid number.";
                return;
            }

            try
            {
                int moduleId = Convert.ToInt32(Session["CurrentModuleID"]);
                int newLessonId = 0;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string lessonQuery = @"
                                        INSERT INTO Lesson
                                        (moduleid, lessontitle, lessondescription, duration, ordernumber, creationtime, deletiontime)
                                        OUTPUT INSERTED.lessonid
                                        VALUES
                                        (@moduleid, @title, @desc, @duration, 1, GETDATE(), NULL)";

                    SqlCommand lessonCmd = new SqlCommand(lessonQuery, con);

                    lessonCmd.Parameters.AddWithValue("@moduleid", moduleId);
                    lessonCmd.Parameters.AddWithValue("@title", txtLessonTitle.Text.Trim());
                    lessonCmd.Parameters.AddWithValue("@desc", txtLessonDesc.Text.Trim());
                    lessonCmd.Parameters.AddWithValue("@duration", duration);

                    newLessonId = (int)lessonCmd.ExecuteScalar();

                    if (!string.IsNullOrWhiteSpace(txtVideoUrl.Text))
                    {
                        string videoQuery = @"
                                            INSERT INTO Material
                                            (clickcount, filetype, lessonid, fileurl, videourl)
                                            VALUES
                                            (0, 'URL', @lessonid, NULL, @videourl)";

                        SqlCommand videoCmd = new SqlCommand(videoQuery, con);
                        videoCmd.Parameters.AddWithValue("@lessonid", newLessonId);
                        videoCmd.Parameters.AddWithValue("@videourl", txtVideoUrl.Text.Trim());

                        videoCmd.ExecuteNonQuery();

                    }

                    if (fuLessonFile.HasFile)
                    {
                        string fileName = System.IO.Path.GetFileName(fuLessonFile.FileName);
                        string extension = System.IO.Path.GetExtension(fileName);
                        string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".ppt", ".pptx" };
                        if (!allowedExtensions.Contains(extension.ToLower()))
                        {
                            lblMessage.Text = "Invalid file type.";
                            return;
                        }

                        string newFileName = Guid.NewGuid().ToString() + extension;
                        string savePath = Server.MapPath("~/Uploads/LessonMaterials/" + newFileName);

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
                    }
                }

                txtLessonTitle.Text = "";
                txtLessonDesc.Text = "";
                txtVideoUrl.Text = "";
                txtDuration.Text = "";

                lblMessage.Text = "Lesson and materials added successfully.";
                LoadLessons();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
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

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        protected void btnGoToReview_Click(object sender, EventArgs e)
        {
            Response.Redirect("ReviewPublish.aspx");
        }

        protected void btnBackToModules_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddModules.aspx");
        }
    }
}