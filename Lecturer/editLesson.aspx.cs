using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class editLesson : System.Web.UI.Page
    {
        string connStr = ConfigurationManager
            .ConnectionStrings["LearnSphereDB"].ConnectionString;

        int lessonId;
        int moduleId;
        int courseId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!int.TryParse(Request.QueryString["courseid"], out courseId))
                Response.Redirect("ViewCourses.aspx");

            int.TryParse(Request.QueryString["lessonid"], out lessonId);

            int.TryParse(Request.QueryString["moduleid"], out moduleId);

            if (!IsPostBack)
            {
                if (lessonId > 0)
                {
                    lblModuleName.Text = "Edit Lesson";
                    btnUpdateModule.Text = "Update and Continue";
                    LoadSidebarProfileImage();
                    LoadLesson();
                }
                else
                {
                    lblModuleName.Text = "Add Lesson";
                    btnUpdateModule.Text = "Confirm Addition";
                    LoadSidebarProfileImage();
                    LoadModuleName();
                }
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

        private void LoadModuleName()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT modulename
                    FROM Module
                    WHERE moduleid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", moduleId);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                    lblModuleName.Text += " - " + result.ToString();
            }
        }

        private void LoadLesson()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                            SELECT l.lessontitle,
                                   l.description,
                                   l.duration,
                                   m.modulename,
                                   l.moduleid
                            FROM Lesson l
                            INNER JOIN Module m ON l.moduleid = m.moduleid
                            WHERE l.lessonid = @id
                            AND l.deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", lessonId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtLessonTitle.Text = reader["lessontitle"].ToString();
                    txtLessonDesc.Text = reader["description"].ToString();
                    txtDuration.Text = reader["duration"].ToString();
                    lblModuleName.Text += " - " + reader["modulename"].ToString();
                    moduleId = Convert.ToInt32(reader["moduleid"]);
                }
            }
        }

        protected void btnUpdateModule_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                int currentLessonId = lessonId;

                if (lessonId > 0)
                {
                    string query = @"
                UPDATE Lesson
                SET lessontitle = @title,
                    description = @desc,
                    duration = @duration
                WHERE lessonid = @id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@title", txtLessonTitle.Text);
                    cmd.Parameters.AddWithValue("@desc", txtLessonDesc.Text);
                    cmd.Parameters.AddWithValue("@duration",
                        string.IsNullOrEmpty(txtDuration.Text)
                        ? (object)DBNull.Value
                        : Convert.ToInt32(txtDuration.Text));
                    cmd.Parameters.AddWithValue("@id", lessonId);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string query = @"
                INSERT INTO Lesson
                (moduleid, lessontitle, description, duration, ordernumber, creationtime)
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
                    GETDATE()
                )";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@moduleid", moduleId);
                    cmd.Parameters.AddWithValue("@title", txtLessonTitle.Text);
                    cmd.Parameters.AddWithValue("@desc", txtLessonDesc.Text);
                    cmd.Parameters.AddWithValue("@duration",
                        string.IsNullOrEmpty(txtDuration.Text)
                        ? (object)DBNull.Value
                        : Convert.ToInt32(txtDuration.Text));

                    currentLessonId = (int)cmd.ExecuteScalar();
                }

                if (!string.IsNullOrWhiteSpace(txtVideoUrl.Text))
                {
                    string checkVideoQuery = @"
        SELECT materialid
        FROM Material
        WHERE lessonid=@lessonid
        AND videourl IS NOT NULL";

                    SqlCommand checkVideoCmd = new SqlCommand(checkVideoQuery, con);
                    checkVideoCmd.Parameters.AddWithValue("@lessonid", currentLessonId);

                    object existingVideo = checkVideoCmd.ExecuteScalar();

                    if (existingVideo != null)
                    {
                        // UPDATE existing video
                        string updateVideoQuery = @"
            UPDATE Material
            SET videourl=@videourl,
                uploadtime=GETDATE()
            WHERE materialid=@id";

                        SqlCommand updateVideoCmd = new SqlCommand(updateVideoQuery, con);
                        updateVideoCmd.Parameters.AddWithValue("@videourl", txtVideoUrl.Text.Trim());
                        updateVideoCmd.Parameters.AddWithValue("@id", existingVideo);
                        updateVideoCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string insertVideoQuery = @"
            INSERT INTO Material
            (clickcount, filetype, lessonid, fileurl, videourl)
            VALUES
            (0, 'URL', @lessonid, NULL, @videourl)";

                        SqlCommand insertVideoCmd = new SqlCommand(insertVideoQuery, con);
                        insertVideoCmd.Parameters.AddWithValue("@lessonid", currentLessonId);
                        insertVideoCmd.Parameters.AddWithValue("@videourl", txtVideoUrl.Text.Trim());
                        insertVideoCmd.ExecuteNonQuery();
                    }
                }

                if (fuLessonFile.HasFile)
                {
                    string fileName = System.IO.Path.GetFileName(fuLessonFile.FileName);
                    string extension = System.IO.Path.GetExtension(fileName).ToLower();

                    string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".ppt", ".pptx" };

                    if (!allowedExtensions.Contains(extension))
                    {
                        lblMessage.Text = "Invalid file type.";
                        return;
                    }

                    string newFileName = Guid.NewGuid().ToString() + extension;
                    string folderPath = Server.MapPath("~/Uploads/LessonMaterials/");

                    if (!System.IO.Directory.Exists(folderPath))
                    {
                        System.IO.Directory.CreateDirectory(folderPath);
                    }

                    string savePath = System.IO.Path.Combine(folderPath, newFileName);

                    fuLessonFile.SaveAs(savePath);

                    string fileUrl = "~/Uploads/LessonMaterials/" + newFileName;

                    string checkFileQuery = @"
        SELECT materialid, fileurl
        FROM Material
        WHERE lessonid=@lessonid
        AND fileurl IS NOT NULL";

                    SqlCommand checkFileCmd = new SqlCommand(checkFileQuery, con);
                    checkFileCmd.Parameters.AddWithValue("@lessonid", currentLessonId);

                    SqlDataReader reader = checkFileCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int materialId = Convert.ToInt32(reader["materialid"]);
                        string oldFileUrl = reader["fileurl"].ToString();

                        reader.Close();

 
                        string oldPath = Server.MapPath(oldFileUrl);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);

    
                        string updateFileQuery = @"
            UPDATE Material
            SET fileurl=@fileurl,
                filetype=@filetype,
                uploadtime=GETDATE()
            WHERE materialid=@id";

                        SqlCommand updateFileCmd = new SqlCommand(updateFileQuery, con);
                        updateFileCmd.Parameters.AddWithValue("@fileurl", fileUrl);
                        updateFileCmd.Parameters.AddWithValue("@filetype", extension);
                        updateFileCmd.Parameters.AddWithValue("@id", materialId);
                        updateFileCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        reader.Close();

                        string insertFileQuery = @"
            INSERT INTO Material
            (clickcount, filetype, lessonid, fileurl, videourl)
            VALUES
            (0, @filetype, @lessonid, @fileurl, NULL)";

                        SqlCommand insertFileCmd = new SqlCommand(insertFileQuery, con);
                        insertFileCmd.Parameters.AddWithValue("@lessonid", currentLessonId);
                        insertFileCmd.Parameters.AddWithValue("@fileurl", fileUrl);
                        insertFileCmd.Parameters.AddWithValue("@filetype", extension);
                        insertFileCmd.ExecuteNonQuery();
                    }
                }
            }

            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}