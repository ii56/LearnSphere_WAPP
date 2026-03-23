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

        // Tracks which step of the wizard the lecturer is on (1–4)
        protected int CurrentStep
        {
            get { return ViewState["CurrentStep"] != null ? (int)ViewState["CurrentStep"] : 1; }
            set { ViewState["CurrentStep"] = value; }
        }

        // Used by the step indicator in the .aspx to apply active/done CSS classes
        protected string StepClass(int step)
        {
            if (step < CurrentStep) return "step done";
            if (step == CurrentStep) return "step active";
            return "step";
        }

        // Returns a checkmark for completed steps, or the step number otherwise
        protected string StepIcon(int step)
        {
            return step < CurrentStep ? "✓" : step.ToString();
        }

        // Shortcut to the logged-in lecturer's user ID
        private int CurrentUserID
        {
            get { return Convert.ToInt32(Session["userid"]); }
        }

        // The course being built — persisted in session so it survives postbacks
        private int CurrentCourseID
        {
            get { return Session["CurrentCourseID"] != null ? Convert.ToInt32(Session["CurrentCourseID"]) : 0; }
            set { Session["CurrentCourseID"] = value; }
        }

        // The module currently being added to — also in session for the same reason
        private int CurrentModuleID
        {
            get { return Session["CurrentModuleID"] != null ? Convert.ToInt32(Session["CurrentModuleID"]) : 0; }
            set { Session["CurrentModuleID"] = value; }
        }

        // Ties the ViewState to the user session to prevent CSRF attacks
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        // Redirects non-lecturers away, then loads the correct wizard step on first visit
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

                // If the lecturer is returning mid-flow, restore what they already filled in
                if (CurrentCourseID > 0)
                    LoadDraftData(CurrentCourseID);
            }
            else
            {
                ShowStep(CurrentStep);
            }
        }

        // Shows the right panel and updates the banner text for the given step
        private void ShowStep(int step)
        {
            CurrentStep = step;

            pnlCourseDetails.Visible = (step == 1);
            pnlAddModules.Visible = (step == 2);
            pnlAddLessons.Visible = (step == 3);
            pnlReviewPublish.Visible = (step == 4);

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

        // Pulls the lecturer's profile picture and sets it in the header avatar
        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ProfileImage FROM [User] WHERE userid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = CurrentUserID;
                con.Open();
                object result = cmd.ExecuteScalar();

                imgSidebarProfile.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        // Refills the Step 1 form fields from an existing draft so the lecturer can continue where they left off
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

        // Creates a new draft course or updates an existing one, then moves to Step 2
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
                    // Brand new course — insert as a draft so changes are never lost
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
                    // Returning to an existing draft — just update its details
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

        // Goes back to Step 1 and restores whatever the lecturer previously saved
        protected void btnBackToCourse_Click(object sender, EventArgs e)
        {
            if (CurrentCourseID > 0) LoadDraftData(CurrentCourseID);
            ShowStep(1);
        }

        // Fetches the course name and shows it as a subtitle on the modules step
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

        // Validates input, checks for duplicates, then inserts the module with an auto-calculated order number
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

                moduleName = Server.HtmlEncode(moduleName);
                moduleDesc = Server.HtmlEncode(moduleDesc);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Prevent duplicate module names within the same course
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

                    // Order number is computed from the DB so there are no gaps or conflicts
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

        // Refreshes the modules grid on Step 2
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

        // Handles the "Add Lessons" button on each module row — verifies ownership then opens Step 3
        protected void gvModules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "AddLessons") return;

            int moduleId = Convert.ToInt32(e.CommandArgument);

            // Make sure this module actually belongs to the current course before trusting it
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

        // Moves to Step 3 using the first available module if none was explicitly selected
        protected void btnContinue_Click(object sender, EventArgs e)
        {
            if (CurrentModuleID == 0)
            {
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

        // Goes back to the modules step
        protected void btnBackToModules_Click(object sender, EventArgs e)
        {
            LoadCourseTitle();
            LoadModules();
            ShowStep(2);
        }

        // Shows the module name as a subtitle on the lessons step
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

        // Validates all lesson fields, inserts the lesson, saves any uploaded file and video URL as Material rows
        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
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

                int duration;
                if (!int.TryParse(txtDuration.Text, out duration) || duration <= 0 || duration > 600)
                {
                    ShowMsg(lblLessonMsg, "Duration must be between 1 and 600 minutes.", false);
                    return;
                }

                // Points are optional — leave blank for no gamification on this lesson
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

                    // Order number is computed from existing lessons so nothing clashes
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

                    // Store the video link as a separate Material row if one was provided
                    if (!string.IsNullOrWhiteSpace(videoUrl))
                    {
                        SqlCommand videoCmd = new SqlCommand(@"
                            INSERT INTO Material (clickcount, filetype, lessonid, fileurl, videourl)
                            VALUES (0, 'URL', @lessonid, NULL, @videourl)", con);
                        videoCmd.Parameters.AddWithValue("@lessonid", newLessonId);
                        videoCmd.Parameters.AddWithValue("@videourl", videoUrl);
                        videoCmd.ExecuteNonQuery();
                    }

                    // File upload — allowed types: PDF, DOC, DOCX, PPT, PPTX, max 5 MB
                    // Saved with a random GUID name to avoid conflicts
                    if (fuLessonFile.HasFile)
                    {
                        string extension = Path.GetExtension(fuLessonFile.FileName).ToLower();
                        string[] allowed = { ".pdf", ".doc", ".docx", ".ppt", ".pptx" };

                        if (!Array.Exists(allowed, x => x == extension))
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

                        string fileName = Guid.NewGuid().ToString() + extension;
                        fuLessonFile.SaveAs(Path.Combine(folder, fileName));

                        string fileUrl = "~/Uploads/LessonMaterials/" + fileName;

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

        // Refreshes the lessons grid on Step 3
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

        // Loads the review data and moves to Step 4
        protected void btnGoToReview_Click(object sender, EventArgs e)
        {
            LoadCourseForReview();
            LoadModulesAndLessons();
            ShowStep(4);
        }

        // Goes back to the lessons step
        protected void btnBackToLessons_Click(object sender, EventArgs e)
        {
            LoadModuleTitle();
            LoadLessons();
            ShowStep(3);
        }

        // Fetches the course name, description and price for the review summary card
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
                    ShowStep(1);
                }
            }
        }

        // Builds the nested module/lesson structure needed by the review repeater
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

                // Fetch lessons for each module individually to keep things readable
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

        // Sets the course to Active, clears the session draft, and redirects to View Courses
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

                // Ownership check — if nothing updated, the lecturer doesn't own this course
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

        // Sets a label's text, style and visibility in one call
        private void ShowMsg(Label lbl, string msg, bool success)
        {
            lbl.Text = msg;
            lbl.CssClass = "alert " + (success ? "alert-success" : "alert-error");
            lbl.Visible = true;
        }

        // Clears the session and sends the lecturer back to the login page
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