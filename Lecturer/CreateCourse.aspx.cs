using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class CreateCourse : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // ── Which step is active: 1=details, 2=modules, 3=lessons, 4=review ──
        protected int CurrentStep
        {
            get { return ViewState["CurrentStep"] != null ? (int)ViewState["CurrentStep"] : 1; }
            set { ViewState["CurrentStep"] = value; }
        }

        // ── Helpers for step indicator in .aspx ──────────────────────────────
        protected string StepClass(int step)
        {
            if (step < CurrentStep) return "step done";
            if (step == CurrentStep) return "step active";
            return "step";
        }
        protected string StepIcon(int step)
        {
            return step < CurrentStep ? "✓" : step.ToString();
        }

        // ── Shortcut properties ───────────────────────────────────────────────
        private int CurrentUserID
        {
            get { return Convert.ToInt32(Session["userid"]); }
        }
        private int CurrentCourseID
        {
            get { return Session["CurrentCourseID"] != null ? Convert.ToInt32(Session["CurrentCourseID"]) : 0; }
            set { Session["CurrentCourseID"] = value; }
        }
        private int CurrentModuleID
        {
            get { return Session["CurrentModuleID"] != null ? Convert.ToInt32(Session["CurrentModuleID"]) : 0; }
            set { Session["CurrentModuleID"] = value; }
        }

        // ══════════════════════════════════════════════════════════════════════
        // CSRF PROTECTION
        // ══════════════════════════════════════════════════════════════════════
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        // ══════════════════════════════════════════════════════════════════════
        // PAGE LOAD
        // ══════════════════════════════════════════════════════════════════════
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                ShowStep(1);

                // If returning to an in-progress course, restore details
                if (CurrentCourseID > 0)
                    LoadDraftData(CurrentCourseID);
            }
            else
            {
                ShowStep(CurrentStep);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // STEP SWITCHER
        // ══════════════════════════════════════════════════════════════════════
        private void ShowStep(int step)
        {
            CurrentStep = step;

            pnlCourseDetails.Visible = (step == 1);
            pnlAddModules.Visible = (step == 2);
            pnlAddLessons.Visible = (step == 3);
            pnlReviewPublish.Visible = (step == 4);

            // Update banner text per step
            string[] titles = { "", "Create New Course", "Add Modules", "Add Lessons", "Review & Publish" };
            string[] subs = {
                "",
                "Fill in the details about your course.",
                "Organise your course into modules.",
                "Add lessons to your module.",
                "Review everything before publishing."
            };
            string[] pills = { "", "Step 1 of 4 — Draft", "Step 2 of 4 — Draft", "Step 3 of 4 — Draft", "Step 4 of 4 — Ready" };

            lblBannerTitle.Text = titles[step];
            lblBannerSub.Text = subs[step];
            lblDraftPill.Text = pills[step];
        }

        // ══════════════════════════════════════════════════════════════════════
        // PROFILE IMAGE
        // ══════════════════════════════════════════════════════════════════════
        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ProfileImage FROM [User] WHERE userid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = CurrentUserID;
                con.Open();
                object result = cmd.ExecuteScalar();

                string imagePath = "~/images/default-user.png";
                if (result != null && result != DBNull.Value)
                {
                    string path = result.ToString();
                    if (path.StartsWith("~/images/")) imagePath = path;
                    else imagePath = path; // allow any valid path
                }
                imgSidebarProfile.Src = ResolveUrl(imagePath);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // STEP 1 — COURSE DETAILS  (CreateCourse.aspx.cs)
        // ══════════════════════════════════════════════════════════════════════

        // Restore draft data if returning mid-flow — matches original LoadDraftData
        private void LoadDraftData(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT coursename, description, price, category
                    FROM Course WHERE courseid=@id AND ownerid=@uid", con);
                cmd.Parameters.AddWithValue("@id", courseId);
                cmd.Parameters.AddWithValue("@uid", CurrentUserID);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtCourseName.Text = reader["coursename"].ToString();
                    txtDescription.Text = reader["description"].ToString();
                    txtPrice.Text = reader["price"].ToString();
                    ddlCategory.SelectedValue = reader["category"].ToString();
                }
            }
        }

        // Matches original btnCreate_Click exactly:
        // - Creates new course (OUTPUT INSERTED.courseid, status='Unactive') OR updates existing draft
        // - Then navigates to Step 2
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                string courseName = txtCourseName.Text.Trim();
                string description = txtDescription.Text.Trim();
                string category = ddlCategory.SelectedValue;
                decimal price;
                decimal.TryParse(txtPrice.Text, out price);

                if (string.IsNullOrWhiteSpace(courseName) &&
                    string.IsNullOrWhiteSpace(description) &&
                    string.IsNullOrWhiteSpace(category) && price == 0)
                {
                    ShowMsg(lblCourseMsg, "Please enter course details before proceeding.", false);
                    return;
                }

                if (CurrentCourseID == 0)
                {
                    // Create new draft — matches original INSERT
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO Course
                            (ownerid, coursename, description, price, creationtime, deletiontime, category, status)
                            OUTPUT INSERTED.courseid
                            VALUES
                            (@ownerid, @name, @desc, @price, GETDATE(), NULL, @category, 'Unactive')", con);

                        cmd.Parameters.AddWithValue("@ownerid", CurrentUserID);
                        cmd.Parameters.AddWithValue("@name", courseName);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@category", category);

                        var priceParam = cmd.Parameters.Add("@price", SqlDbType.Decimal);
                        priceParam.Precision = 18; priceParam.Scale = 2; priceParam.Value = price;

                        CurrentCourseID = (int)cmd.ExecuteScalar();
                    }
                }
                else
                {
                    // Update existing draft — matches original UPDATE
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(@"
                            UPDATE Course
                            SET coursename=@name, description=@desc, price=@price, category=@category
                            WHERE courseid=@id AND ownerid=@uid", con);

                        cmd.Parameters.AddWithValue("@id", CurrentCourseID);
                        cmd.Parameters.AddWithValue("@uid", CurrentUserID);
                        cmd.Parameters.AddWithValue("@name", courseName);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@category", category);

                        var priceParam = cmd.Parameters.Add("@price", SqlDbType.Decimal);
                        priceParam.Precision = 18; priceParam.Scale = 2; priceParam.Value = price;

                        cmd.ExecuteNonQuery();
                        LearnSphere_WAPP.Syslog.action(CurrentUserID, "Updated Course draft (CourseID: " + CurrentCourseID + ")");
                    }
                }

                LoadCourseTitle();
                LoadModules();
                ShowStep(2);
            }
            catch (Exception ex)
            {
                ShowMsg(lblCourseMsg, ex.Message, false);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // STEP 2 — ADD MODULES  (AddModules.aspx.cs)
        // ══════════════════════════════════════════════════════════════════════
        protected void btnBackToCourse_Click(object sender, EventArgs e)
        {
            if (CurrentCourseID > 0) LoadDraftData(CurrentCourseID);
            ShowStep(1);
        }

        private void LoadCourseTitle()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT coursename FROM Course WHERE courseid=@id AND ownerid=@uid", con);
                cmd.Parameters.AddWithValue("@id", CurrentCourseID);
                cmd.Parameters.AddWithValue("@uid", CurrentUserID);
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                    lblCourseTitle.Text = Server.HtmlEncode(result.ToString());
            }
        }

        // Matches original btnAddModule_Click exactly:
        // - Validation, XSS encode, duplicate check, ordernumber subquery, INSERT
        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                string moduleName = txtModuleName.Text.Trim();
                string moduleDesc = txtModuleDesc.Text.Trim();

                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    ShowMsg(lblModuleMsg, "Module name is required.", false);
                    return;
                }
                if (moduleName.Length > 100)
                {
                    ShowMsg(lblModuleMsg, "Module name cannot exceed 100 characters.", false);
                    return;
                }
                if (moduleDesc.Length > 1000)
                {
                    ShowMsg(lblModuleMsg, "Description cannot exceed 1000 characters.", false);
                    return;
                }

                // XSS protection — matches original
                moduleName = Server.HtmlEncode(moduleName);
                moduleDesc = Server.HtmlEncode(moduleDesc);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Duplicate module check — matches original
                    SqlCommand check = new SqlCommand(@"
                        SELECT COUNT(*) FROM Module
                        WHERE modulename=@name AND courseid=@courseid AND deletiontime IS NULL", con);
                    check.Parameters.AddWithValue("@name", moduleName);
                    check.Parameters.AddWithValue("@courseid", CurrentCourseID);
                    if ((int)check.ExecuteScalar() > 0)
                    {
                        ShowMsg(lblModuleMsg, "A module with this name already exists.", false);
                        return;
                    }

                    // Insert with ordernumber subquery — matches original exactly
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Module
                        (courseid, modulename, moduledescription, ordernumber, creationtime, deletiontime)
                        VALUES
                        (
                            @courseid, @name, @desc,
                            (SELECT ISNULL(MAX(ordernumber),0)+1 FROM Module WHERE courseid=@courseid AND deletiontime IS NULL),
                            GETDATE(), NULL
                        )", con);
                    cmd.Parameters.AddWithValue("@courseid", CurrentCourseID);
                    cmd.Parameters.AddWithValue("@name", moduleName);
                    cmd.Parameters.AddWithValue("@desc", moduleDesc);
                    cmd.ExecuteNonQuery();

                    LearnSphere_WAPP.Syslog.action(CurrentUserID, "Module Added (ModuleName: " + moduleName + ")");
                }

                txtModuleName.Text = "";
                txtModuleDesc.Text = "";
                ShowMsg(lblModuleMsg, "Module added successfully.", true);
                LoadModules();
            }
            catch
            {
                ShowMsg(lblModuleMsg, "An error occurred while adding the module.", false);
            }
        }

        private void LoadModules()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT moduleid, modulename, moduledescription
                    FROM Module
                    WHERE courseid=@courseid AND deletiontime IS NULL
                    ORDER BY ordernumber", con);
                da.SelectCommand.Parameters.AddWithValue("@courseid", CurrentCourseID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvModules.DataSource = dt;
                gvModules.DataBind();
            }
        }

        // Matches original gvModules_RowCommand:
        // - Security check: module belongs to this course
        // - Sets Session["CurrentModuleID"]
        // - Navigates to AddLessons (now Step 3)
        protected void gvModules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "AddLessons") return;

            int moduleId = Convert.ToInt32(e.CommandArgument);

            // Security check — matches original
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Module
                    WHERE moduleid=@moduleid AND courseid=@courseid", con);
                cmd.Parameters.AddWithValue("@moduleid", moduleId);
                cmd.Parameters.AddWithValue("@courseid", CurrentCourseID);
                con.Open();
                if ((int)cmd.ExecuteScalar() == 0)
                {
                    ShowMsg(lblModuleMsg, "Invalid module selection.", false);
                    return;
                }
            }

            CurrentModuleID = moduleId;
            LoadModuleTitle();
            LoadLessons();
            ShowStep(3);
        }

        // Continue button — validates at least one module exists before proceeding
        protected void btnContinue_Click(object sender, EventArgs e)
        {
            // Must have at least one module and one selected
            if (CurrentModuleID == 0)
            {
                // Auto-pick first module if available
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 1 moduleid FROM Module
                        WHERE courseid=@courseid AND deletiontime IS NULL
                        ORDER BY ordernumber", con);
                    cmd.Parameters.AddWithValue("@courseid", CurrentCourseID);
                    con.Open();
                    object r = cmd.ExecuteScalar();
                    if (r == null)
                    {
                        ShowMsg(lblModuleMsg, "Please add at least one module before continuing.", false);
                        return;
                    }
                    CurrentModuleID = Convert.ToInt32(r);
                }
            }

            LoadModuleTitle();
            LoadLessons();
            ShowStep(3);
        }

        // ══════════════════════════════════════════════════════════════════════
        // STEP 3 — ADD LESSONS  (AddLessons.aspx.cs)
        // ══════════════════════════════════════════════════════════════════════
        protected void btnBackToModules_Click(object sender, EventArgs e)
        {
            LoadCourseTitle();
            LoadModules();
            ShowStep(2);
        }

        private void LoadModuleTitle()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT modulename FROM Module WHERE moduleid=@id", con);
                cmd.Parameters.AddWithValue("@id", CurrentModuleID);
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                    lblModuleName.Text = Server.HtmlEncode(result.ToString());
            }
        }

        // Matches original btnAddLesson_Click exactly:
        // - INPUT SANITIZATION, DURATION, VIDEO URL validation
        // - INSERT Lesson with ordernumber subquery, OUTPUT INSERTED.lessonid
        // - Material table: video URL row + file row
        // - File saved to ~/Uploads/LessonMaterials/ with Guid name
        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                // Input sanitization — matches original
                string lessonTitle = Server.HtmlEncode(txtLessonTitle.Text.Trim());
                string lessonDesc = Server.HtmlEncode(txtLessonDesc.Text.Trim());
                string videoUrl = Server.HtmlEncode(txtVideoUrl.Text.Trim());

                if (string.IsNullOrWhiteSpace(lessonTitle))
                {
                    ShowMsg(lblLessonMsg, "Lesson title is required.", false);
                    return;
                }
                if (lessonTitle.Length > 100)
                {
                    ShowMsg(lblLessonMsg, "Lesson title cannot exceed 100 characters.", false);
                    return;
                }
                if (lessonDesc.Length > 1000)
                {
                    ShowMsg(lblLessonMsg, "Description is too long.", false);
                    return;
                }

                // Duration validation — matches original
                int duration;
                if (!int.TryParse(txtDuration.Text, out duration))
                {
                    ShowMsg(lblLessonMsg, "Duration must be a valid number.", false);
                    return;
                }
                if (duration <= 0 || duration > 600)
                {
                    ShowMsg(lblLessonMsg, "Duration must be between 1 and 600 minutes.", false);
                    return;
                }

                // Lesson points — optional, NULL if blank, 0–10000
                int? lessonPoints = null;
                if (!string.IsNullOrWhiteSpace(txtLessonPoints.Text))
                {
                    int pts;
                    if (!int.TryParse(txtLessonPoints.Text.Trim(), out pts) || pts < 0 || pts > 10000)
                    {
                        ShowMsg(lblLessonMsg, "Points must be a whole number between 0 and 10000.", false);
                        return;
                    }
                    lessonPoints = pts;
                }

                // Video URL validation — matches original
                if (!string.IsNullOrWhiteSpace(videoUrl))
                {
                    Uri uriResult;
                    if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out uriResult))
                    {
                        ShowMsg(lblLessonMsg, "Invalid video URL.", false);
                        return;
                    }
                }

                int newLessonId;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // INSERT Lesson with ordernumber subquery — matches original exactly
                    SqlCommand lessonCmd = new SqlCommand(@"
                        INSERT INTO Lesson
                        (moduleid, lessontitle, lessondescription, duration, ordernumber, creationtime, deletiontime, lessonpoints)
                        OUTPUT INSERTED.lessonid
                        VALUES
                        (
                            @moduleid, @title, @desc, @duration,
                            (SELECT ISNULL(MAX(ordernumber),0)+1 FROM Lesson WHERE moduleid=@moduleid AND deletiontime IS NULL),
                            GETDATE(), NULL, @lessonpoints
                        )", con);
                    lessonCmd.Parameters.AddWithValue("@moduleid", CurrentModuleID);
                    lessonCmd.Parameters.AddWithValue("@title", lessonTitle);
                    lessonCmd.Parameters.AddWithValue("@desc", lessonDesc);
                    lessonCmd.Parameters.AddWithValue("@duration", duration);
                    lessonCmd.Parameters.Add("@lessonpoints", SqlDbType.Int).Value =
                        lessonPoints.HasValue ? (object)lessonPoints.Value : DBNull.Value;
                    newLessonId = (int)lessonCmd.ExecuteScalar();

                    // Video material — matches original
                    if (!string.IsNullOrWhiteSpace(videoUrl))
                    {
                        SqlCommand videoCmd = new SqlCommand(@"
                            INSERT INTO Material (clickcount, filetype, lessonid, fileurl, videourl)
                            VALUES (0, 'URL', @lessonid, NULL, @videourl)", con);
                        videoCmd.Parameters.AddWithValue("@lessonid", newLessonId);
                        videoCmd.Parameters.AddWithValue("@videourl", videoUrl);
                        videoCmd.ExecuteNonQuery();
                    }

                    // File upload — matches original exactly
                    // Extensions: .pdf/.doc/.docx/.ppt/.pptx, max 5MB, Guid name
                    // Saved to ~/Uploads/LessonMaterials/
                    if (fuLessonFile.HasFile)
                    {
                        string extension = Path.GetExtension(fuLessonFile.FileName).ToLower();
                        string[] allowedExts = { ".pdf", ".doc", ".docx", ".ppt", ".pptx" };

                        if (!Array.Exists(allowedExts, x => x == extension))
                        {
                            ShowMsg(lblLessonMsg, "Invalid file type.", false);
                            return;
                        }
                        if (fuLessonFile.PostedFile.ContentLength > 5 * 1024 * 1024)
                        {
                            ShowMsg(lblLessonMsg, "File must be smaller than 5MB.", false);
                            return;
                        }

                        string folder = Server.MapPath("~/Uploads/LessonMaterials/");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                        string newFileName = Guid.NewGuid().ToString() + extension;
                        string savePath = Path.Combine(folder, newFileName);
                        fuLessonFile.SaveAs(savePath);

                        string fileUrl = "~/Uploads/LessonMaterials/" + newFileName;

                        SqlCommand fileCmd = new SqlCommand(@"
                            INSERT INTO Material (clickcount, filetype, lessonid, fileurl, videourl)
                            VALUES (0, @filetype, @lessonid, @fileurl, NULL)", con);
                        fileCmd.Parameters.AddWithValue("@lessonid", newLessonId);
                        fileCmd.Parameters.AddWithValue("@fileurl", fileUrl);
                        fileCmd.Parameters.AddWithValue("@filetype", extension);
                        fileCmd.ExecuteNonQuery();

                        LearnSphere_WAPP.Syslog.action(CurrentUserID, "Lesson Added (LessonID: " + newLessonId + ")");
                    }
                }

                // Reset form — matches original
                txtLessonTitle.Text = "";
                txtLessonDesc.Text = "";
                txtVideoUrl.Text = "";
                txtDuration.Text = "";
                txtLessonPoints.Text = "";

                ShowMsg(lblLessonMsg, "Lesson added successfully.", true);
                LoadLessons();
            }
            catch
            {
                ShowMsg(lblLessonMsg, "An error occurred while adding the lesson.", false);
            }
        }

        private void LoadLessons()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT lessontitle, duration, lessonpoints
                    FROM Lesson
                    WHERE moduleid=@moduleid AND deletiontime IS NULL
                    ORDER BY ordernumber", con);
                da.SelectCommand.Parameters.AddWithValue("@moduleid", CurrentModuleID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvLessons.DataSource = dt;
                gvLessons.DataBind();
            }
        }

        protected void btnGoToReview_Click(object sender, EventArgs e)
        {
            LoadCourseForReview();
            LoadModulesAndLessons();
            ShowStep(4);
        }

        // ══════════════════════════════════════════════════════════════════════
        // STEP 4 — REVIEW & PUBLISH  (ReviewPublish.aspx.cs)
        // ══════════════════════════════════════════════════════════════════════
        protected void btnBackToLessons_Click(object sender, EventArgs e)
        {
            LoadModuleTitle();
            LoadLessons();
            ShowStep(3);
        }

        // Matches original LoadCourse with ownership check + XSS
        private void LoadCourseForReview()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT coursename, description, price
                    FROM Course
                    WHERE courseid=@id AND ownerid=@uid", con);
                cmd.Parameters.AddWithValue("@id", CurrentCourseID);
                cmd.Parameters.AddWithValue("@uid", CurrentUserID);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblCourseName.Text = Server.HtmlEncode(reader["coursename"].ToString());
                    lblCourseDesc.Text = Server.HtmlEncode(reader["description"].ToString());
                    lblCoursePrice.Text = "RM " + Server.HtmlEncode(reader["price"].ToString());
                }
                else
                {
                    // Unauthorized — go back to start
                    ShowStep(1);
                }
            }
        }

        // Matches original LoadModulesAndLessons exactly:
        // List<dynamic> with nested Lessons list for nested repeater
        private void LoadModulesAndLessons()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand moduleCmd = new SqlCommand(@"
                    SELECT moduleid, modulename FROM Module
                    WHERE courseid=@courseid AND deletiontime IS NULL
                    ORDER BY ordernumber", con);
                moduleCmd.Parameters.AddWithValue("@courseid", CurrentCourseID);
                SqlDataReader moduleReader = moduleCmd.ExecuteReader();

                var modules = new List<dynamic>();
                while (moduleReader.Read())
                {
                    modules.Add(new
                    {
                        moduleid = moduleReader["moduleid"],
                        modulename = moduleReader["modulename"],
                        Lessons = new List<dynamic>()
                    });
                }
                moduleReader.Close();

                foreach (var module in modules)
                {
                    SqlCommand lessonCmd = new SqlCommand(@"
                        SELECT lessontitle, duration, lessonpoints FROM Lesson
                        WHERE moduleid=@moduleid AND deletiontime IS NULL
                        ORDER BY ordernumber", con);
                    lessonCmd.Parameters.AddWithValue("@moduleid", module.moduleid);
                    SqlDataReader lessonReader = lessonCmd.ExecuteReader();
                    while (lessonReader.Read())
                    {
                        module.Lessons.Add(new
                        {
                            lessontitle = lessonReader["lessontitle"],
                            duration = lessonReader["duration"],
                            lessonpoints = lessonReader["lessonpoints"]
                        });
                    }
                    lessonReader.Close();
                }

                rptModules.DataSource = modules;
                rptModules.DataBind();
            }
        }

        // Matches original btnPublish_Click exactly:
        // - ownership check with rowsAffected==0 guard
        // - status='Active'
        // - Session.Remove("CurrentCourseID")
        // - Redirect to ViewCourses.aspx
        protected void btnPublish_Click(object sender, EventArgs e)
        {
            if (CurrentCourseID == 0 || Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Course SET status='Active'
                    WHERE courseid=@id AND ownerid=@uid", con);
                cmd.Parameters.AddWithValue("@id", CurrentCourseID);
                cmd.Parameters.AddWithValue("@uid", CurrentUserID);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    ShowMsg(lblPublishMsg, "Unauthorized action.", false);
                    return;
                }
                LearnSphere_WAPP.Syslog.action(CurrentUserID, "Publish Course (CourseID: " + CurrentCourseID + ")");
            }

            Session.Remove("CurrentCourseID");
            Session.Remove("CurrentModuleID");
            Response.Redirect("ViewCourses.aspx");
        }

        // ══════════════════════════════════════════════════════════════════════
        // SHARED HELPER
        // ══════════════════════════════════════════════════════════════════════
        private void ShowMsg(Label lbl, string msg, bool success)
        {
            lbl.Text = msg;
            lbl.CssClass = "alert " + (success ? "alert-success" : "alert-error");
            lbl.Visible = true;
        }

        // ══════════════════════════════════════════════════════════════════════
        // LOGOUT
        // ══════════════════════════════════════════════════════════════════════
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(CurrentUserID, "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Redirect("~/Login.aspx");
        }
    }
}