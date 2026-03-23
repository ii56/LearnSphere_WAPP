using iTextSharp.text;
using iTextSharp.text.pdf;
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
    // Typed classes used by the review repeater — strongly typed so the nested
    // repeater in the .aspx can data-bind without casting to dynamic
    public class ModuleView
    {
        public int moduleid { get; set; }
        public string modulename { get; set; }
        public List<LessonView> Lessons { get; set; }
    }
    public class LessonView
    {
        public string lessontitle { get; set; }
        public int duration { get; set; }
        public object lessonpoints { get; set; }   // stored as object so DBNull doesn't throw
    }

    public partial class ViewCourses : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // Tracks which panel is currently visible across postbacks
        protected string CurrentView
        {
            get { return ViewState["CV"] as string ?? "courses"; }
            set { ViewState["CV"] = value; }
        }

        // The course being edited — 0 means none selected
        protected int EditCourseId
        {
            get { return ViewState["ECId"] != null ? (int)ViewState["ECId"] : 0; }
            set { ViewState["ECId"] = value; }
        }

        // The module being edited — 0 means a new module is being added
        protected int EditModuleId
        {
            get { return ViewState["EMId"] != null ? (int)ViewState["EMId"] : 0; }
            set { ViewState["EMId"] = value; }
        }

        // The lesson being edited — 0 means a new lesson is being added
        protected int EditLessonId
        {
            get { return ViewState["ELId"] != null ? (int)ViewState["ELId"] : 0; }
            set { ViewState["ELId"] = value; }
        }

        // The module a new lesson will be added to
        protected int LessonModuleId
        {
            get { return ViewState["LMId"] != null ? (int)ViewState["LMId"] : 0; }
            set { ViewState["LMId"] = value; }
        }

        // The course whose students are being viewed
        protected int StudentsForCourseId
        {
            get { return ViewState["SFCId"] != null ? (int)ViewState["SFCId"] : 0; }
            set { ViewState["SFCId"] = value; }
        }

        // Shortcut to the logged-in lecturer's user ID
        private int CurrentUserId
        {
            get { return Session["userid"] != null ? Convert.ToInt32(Session["userid"]) : 0; }
        }

        // Redirects non-lecturers away, then loads the course list on first visit
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            LoadSidebarProfileImage();

            if (!IsPostBack)
            {
                LoadCourses();
                ShowView("courses");
            }
            else
            {
                ShowView(CurrentView);
            }
        }

        // Shows the correct panel and hides all others
        private void ShowView(string view)
        {
            pnlViewCourses.Visible = (view == "courses");
            pnlEditCourse.Visible = (view == "edit");
            pnlEditModule.Visible = (view == "module");
            pnlEditLesson.Visible = (view == "lesson");
            pnlViewStudents.Visible = (view == "students");
            pnlReviewPublish.Visible = (view == "review");
            CurrentView = view;
        }

        // Pulls the lecturer's profile picture and sets it in the header avatar
        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null) return;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ProfileImage FROM [User] WHERE userid=@id", con);
                cmd.Parameters.AddWithValue("@id", CurrentUserId);
                con.Open();
                object result = cmd.ExecuteScalar();
                imgSidebarProfile.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        // Loads the lecturer's courses with optional filtering by name, category, status and price
        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT courseid, coursename, category, price,
                        CASE
                            WHEN status = 'Active'   THEN 'Published'
                            WHEN status = 'Unactive' THEN 'Draft'
                            WHEN status = 'Deleted'  THEN 'Deleted'
                            ELSE 'Unknown'
                        END AS statusText
                    FROM Course
                    WHERE ownerid=@id AND deletiontime IS NULL";

                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@id", CurrentUserId));

                // Each filter is only applied if the user actually entered a value
                if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    query += " AND coursename LIKE @search";
                    parameters.Add(new SqlParameter("@search", "%" + txtSearch.Text.Trim() + "%"));
                }
                if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                {
                    query += " AND category=@cat";
                    parameters.Add(new SqlParameter("@cat", ddlCategory.SelectedValue));
                }
                if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                {
                    query += " AND status=@status";
                    parameters.Add(new SqlParameter("@status", ddlStatus.SelectedValue));
                }
                if (!string.IsNullOrEmpty(txtMinPrice.Text.Trim()))
                {
                    query += " AND price>=@minPrice";
                    parameters.Add(new SqlParameter("@minPrice", txtMinPrice.Text.Trim()));
                }
                if (!string.IsNullOrEmpty(txtMaxPrice.Text.Trim()))
                {
                    query += " AND price<=@maxPrice";
                    parameters.Add(new SqlParameter("@maxPrice", txtMaxPrice.Text.Trim()));
                }

                query += " ORDER BY creationtime DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                foreach (var p in parameters) cmd.Parameters.Add(p);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvCourses.DataSource = dt;
                gvCourses.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadCourses();
            ShowView("courses");
        }

        // Clears all filter fields and reloads the full course list
        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlCategory.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            txtMinPrice.Text = "";
            txtMaxPrice.Text = "";
            LoadCourses();
            ShowView("courses");
        }

        // Routes each action button in the course grid to the correct handler
        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument.ToString(), out int index)) return;
            int courseId = Convert.ToInt32(gvCourses.DataKeys[index].Value);

            switch (e.CommandName)
            {
                case "EditCourse":
                    EditCourseId = courseId;
                    LoadCourseInfo(courseId);
                    LoadModules(courseId);
                    ShowView("edit");
                    break;

                case "DeleteCourse":
                    SoftDeleteCourse(courseId);
                    LoadCourses();
                    ShowView("courses");
                    break;

                case "ViewStudents":
                    StudentsForCourseId = courseId;
                    LoadStudentsCourseTitle(courseId);
                    LoadStudents(courseId);
                    ShowView("students");
                    break;

                // Preview redirects to a separate page since it involves full lesson navigation
                case "PreviewCourse":
                    Response.Redirect("Preview.aspx?courseid=" + courseId);
                    break;
            }
        }

        // Marks the course as deleted without removing it from the database
        private void SoftDeleteCourse(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Course SET deletiontime=GETDATE() WHERE courseid=@id AND ownerid=@uid", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserId;
                con.Open();
                cmd.ExecuteNonQuery();
                LearnSphere_WAPP.Syslog.action(CurrentUserId, "Deleted Course (CourseID: " + courseId + ")");
            }
            ShowMsg(lblCoursesMsg, "Course deleted successfully.", true);
        }

        // Goes back to the course list
        protected void btnBackToCourses_Click(object sender, EventArgs e)
        {
            LoadCourses();
            ShowView("courses");
        }

        // Fetches the course name, description and price for the edit panel header
        private void LoadCourseInfo(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT coursename, description, price
                    FROM Course WHERE courseid=@id AND deletiontime IS NULL", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblCourseName.Text = Server.HtmlEncode(reader["coursename"].ToString());
                    lblEditBannerTitle.Text = Server.HtmlEncode(reader["coursename"].ToString());
                    lblCourseDescription.Text = Server.HtmlEncode(reader["description"].ToString());
                    lblCoursePrice.Text = reader["price"].ToString();
                }
                else
                {
                    LoadCourses(); ShowView("courses");
                }
            }
        }

        // Loads the module list for the edit panel — the nested lessons are bound in ItemDataBound
        private void LoadModules(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT moduleid, modulename FROM Module
                    WHERE courseid=@courseid AND deletiontime IS NULL
                    ORDER BY ordernumber", con);
                cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;
                con.Open();
                rptModules.DataSource = cmd.ExecuteReader();
                rptModules.DataBind();
            }
        }

        // Binds the nested lesson repeater for each module row as the modules repeater renders
        protected void rptModules_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem) return;

            int moduleId;
            if (!int.TryParse(DataBinder.Eval(e.Item.DataItem, "moduleid").ToString(), out moduleId))
                return;

            Repeater rptLessons = (Repeater)e.Item.FindControl("rptLessons");
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT lessonid, lessontitle, duration, lessonpoints
                    FROM Lesson
                    WHERE moduleid=@moduleid AND deletiontime IS NULL
                    ORDER BY ordernumber", con);
                cmd.Parameters.Add("@moduleid", SqlDbType.Int).Value = moduleId;
                con.Open();
                rptLessons.DataSource = cmd.ExecuteReader();
                rptLessons.DataBind();
            }
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Module_Command(source, e);
        }

        // Handles Edit Module, Delete Module and Add Lesson commands from the modules repeater
        protected void Module_Command(object sender, CommandEventArgs e)
        {
            int moduleId;
            if (!int.TryParse(e.CommandArgument.ToString(), out moduleId)) return;

            // Security check — the module must actually belong to the course being edited
            if (!IsModuleValid(moduleId, EditCourseId)) return;

            if (e.CommandName == "EditModule")
            {
                EditModuleId = moduleId;
                LoadModuleForEdit(moduleId);
                ShowView("module");
            }
            else if (e.CommandName == "DeleteModule")
            {
                DeleteModule(moduleId);
                LoadModules(EditCourseId);
                ShowView("edit");
            }
            else if (e.CommandName == "AddLesson")
            {
                EditLessonId = 0;
                LessonModuleId = moduleId;
                PrepareAddLesson(moduleId);
                ShowView("lesson");
            }
        }

        // Handles Edit Lesson and Delete Lesson commands from the lessons repeater
        protected void Lesson_Command(object sender, CommandEventArgs e)
        {
            int lessonId;
            if (!int.TryParse(e.CommandArgument.ToString(), out lessonId)) return;

            // Security check — the lesson must belong to this course via its module
            if (!IsLessonValid(lessonId, EditCourseId)) return;

            if (e.CommandName == "EditLesson")
            {
                EditLessonId = lessonId;
                LoadLessonForEdit(lessonId);
                ShowView("lesson");
            }
            else if (e.CommandName == "DeleteLesson")
            {
                DeleteLesson(lessonId);
                SetCourseToDraft(EditCourseId);
                LoadModules(EditCourseId);
                ShowView("edit");
            }
        }

        // Opens the module form in Add mode with all fields cleared
        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            EditModuleId = 0;
            txtModName.Text = "";
            txtModDesc.Text = "";
            txtModOrder.Text = "";
            lblModMsg.Visible = false;
            lblModModeTitle.Text = "Add Module";
            btnSaveModule.Text = "Add Module";
            LoadModCourseTitle(EditCourseId);
            ShowView("module");
        }

        // Validates the course has content, then loads the review panel
        protected void btnReview_Click(object sender, EventArgs e)
        {
            if (!IsCourseValidForPublish())
            {
                ShowMsg(lblEditCourseMsg, "Course must have at least one module with a lesson before publishing.", false);
                ShowView("edit");
                return;
            }
            LoadCourseForReview();
            LoadModulesAndLessonsForReview();
            ShowView("review");
        }

        // Exam buttons redirect to the dedicated exam page since exam creation is a separate flow
        protected void btnCreateExam_Click(object sender, EventArgs e)
        {
            Response.Redirect("MakeExam.aspx?courseid=" + EditCourseId);
        }
        protected void btnEditExam_Click(object sender, EventArgs e)
        {
            Response.Redirect("MakeExam.aspx?courseid=" + EditCourseId + "&edit=1");
        }

        // Deletes the exam for this course — questions cascade via the DB foreign key
        protected void btnDeleteExam_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Exam WHERE courseid=@courseid", con);
                cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = EditCourseId;
                con.Open();
                cmd.ExecuteNonQuery();
                LearnSphere_WAPP.Syslog.action(CurrentUserId, "Deleted Exam (CourseID: " + EditCourseId + ")");
            }
            ShowMsg(lblEditCourseMsg, "Exam deleted.", true);
            ShowView("edit");
        }

        // Deletes materials, lessons and then the module itself in a single transaction to keep the DB consistent
        private void DeleteModule(int moduleId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    new SqlCommand("DELETE FROM Material WHERE lessonid IN (SELECT lessonid FROM Lesson WHERE moduleid=@id)", con, trans)
                    { Parameters = { new SqlParameter("@id", moduleId) } }.ExecuteNonQuery();
                    new SqlCommand("DELETE FROM Lesson WHERE moduleid=@id", con, trans)
                    { Parameters = { new SqlParameter("@id", moduleId) } }.ExecuteNonQuery();
                    new SqlCommand("DELETE FROM Module WHERE moduleid=@id", con, trans)
                    { Parameters = { new SqlParameter("@id", moduleId) } }.ExecuteNonQuery();
                    trans.Commit();
                    SetCourseToDraft(EditCourseId);
                    LearnSphere_WAPP.Syslog.action(CurrentUserId, "Deleted Module (ModuleID: " + moduleId + ")");
                }
                catch { trans.Rollback(); }
            }
        }

        // Deletes materials then the lesson itself in a single transaction
        private void DeleteLesson(int lessonId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    new SqlCommand("DELETE FROM Material WHERE lessonid=@id", con, trans)
                    { Parameters = { new SqlParameter("@id", lessonId) } }.ExecuteNonQuery();
                    new SqlCommand("DELETE FROM Lesson WHERE lessonid=@id", con, trans)
                    { Parameters = { new SqlParameter("@id", lessonId) } }.ExecuteNonQuery();
                    trans.Commit();
                    LearnSphere_WAPP.Syslog.action(CurrentUserId, "Deleted Lesson (LessonID: " + lessonId + ")");
                }
                catch { trans.Rollback(); }
            }
        }

        // Goes back to the edit panel without saving
        protected void btnCancelModule_Click(object sender, EventArgs e)
        {
            LoadModules(EditCourseId);
            ShowView("edit");
        }

        // Shows the course name as a subtitle on the module form
        private void LoadModCourseTitle(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT coursename FROM Course WHERE courseid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;
                con.Open();
                object r = cmd.ExecuteScalar();
                lblModCourseTitle.Text = r != null ? Server.HtmlEncode(r.ToString()) : "";
            }
        }

        // Fills the module form fields with the existing module's data ready for editing
        private void LoadModuleForEdit(int moduleId)
        {
            lblModModeTitle.Text = "Edit Module";
            btnSaveModule.Text = "Update Module";
            LoadModCourseTitle(EditCourseId);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT modulename, moduledescription, ordernumber
                    FROM Module WHERE moduleid=@id AND deletiontime IS NULL", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtModName.Text = Server.HtmlEncode(reader["modulename"].ToString());
                    txtModDesc.Text = Server.HtmlEncode(reader["moduledescription"].ToString());
                    txtModOrder.Text = reader["ordernumber"].ToString();
                }
            }
        }

        // Validates and saves a module — inserts a new one or updates the existing one
        protected void btnSaveModule_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string name = txtModName.Text.Trim();
            string desc = txtModDesc.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowMsg(lblModMsg, "Module name is required.", false);
                return;
            }
            if (name.Length > 100) { ShowMsg(lblModMsg, "Module name too long.", false); return; }
            if (desc.Length > 1000) { ShowMsg(lblModMsg, "Description too long.", false); return; }

            int order = 1;
            if (!string.IsNullOrWhiteSpace(txtModOrder.Text))
            {
                if (!int.TryParse(txtModOrder.Text, out order) || order < 1 || order > 100)
                {
                    ShowMsg(lblModMsg, "Invalid order number.", false);
                    return;
                }
            }

            name = Server.HtmlEncode(name);
            desc = Server.HtmlEncode(desc);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    if (EditModuleId > 0)
                    {
                        SqlCommand cmd = new SqlCommand(@"
                            UPDATE Module SET modulename=@name, moduledescription=@desc, ordernumber=@order
                            WHERE moduleid=@id AND courseid=@cid", con);
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.Parameters.Add("@order", SqlDbType.Int).Value = order;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = EditModuleId;
                        cmd.Parameters.Add("@cid", SqlDbType.Int).Value = EditCourseId;
                        cmd.ExecuteNonQuery();
                        LearnSphere_WAPP.Syslog.action(CurrentUserId, "Updated Module (ModuleID: " + EditModuleId + ")");
                    }
                    else
                    {
                        // Order number is computed from the DB so there are no gaps or conflicts
                        SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO Module (courseid, modulename, moduledescription, ordernumber, creationtime)
                            VALUES (@courseid, @name, @desc,
                                (SELECT ISNULL(MAX(ordernumber),0)+1 FROM Module WHERE courseid=@courseid AND deletiontime IS NULL),
                                GETDATE())", con);
                        cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = EditCourseId;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.ExecuteNonQuery();
                    }
                    SetCourseToDraft(EditCourseId);
                }

                LoadModules(EditCourseId);
                ShowView("edit");
            }
            catch
            {
                ShowMsg(lblModMsg, "An error occurred. Please try again.", false);
            }
        }

        // Goes back to the edit panel without saving
        protected void btnCancelLesson_Click(object sender, EventArgs e)
        {
            LoadModules(EditCourseId);
            ShowView("edit");
        }

        // Opens the lesson form in Add mode with all fields cleared
        private void PrepareAddLesson(int moduleId)
        {
            lblLsnModeTitle.Text = "Add Lesson";
            btnSaveLesson.Text = "Add Lesson";
            txtLsnTitle.Text = "";
            txtLsnDesc.Text = "";
            txtLsnVideoUrl.Text = "";
            txtLsnDuration.Text = "";
            txtLsnPoints.Text = "";
            lblLsnMsg.Visible = false;
            LoadLsnModuleName(moduleId);
        }

        // Shows the module name as a subtitle on the lesson form
        private void LoadLsnModuleName(int moduleId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT modulename FROM Module WHERE moduleid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;
                con.Open();
                object r = cmd.ExecuteScalar();
                lblLsnModuleName.Text = r != null ? "Module: " + Server.HtmlEncode(r.ToString()) : "";
            }
        }

        // Fills the lesson form with the existing lesson's data including points
        private void LoadLessonForEdit(int lessonId)
        {
            lblLsnModeTitle.Text = "Edit Lesson";
            btnSaveLesson.Text = "Update Lesson";
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT l.lessontitle, l.description, l.duration, l.lessonpoints,
                           m.modulename, l.moduleid
                    FROM Lesson l
                    INNER JOIN Module m ON l.moduleid = m.moduleid
                    WHERE l.lessonid=@id AND l.deletiontime IS NULL", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = lessonId;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtLsnTitle.Text = Server.HtmlEncode(reader["lessontitle"].ToString());
                    txtLsnDesc.Text = Server.HtmlEncode(reader["description"].ToString());
                    txtLsnDuration.Text = reader["duration"].ToString();
                    txtLsnPoints.Text = reader["lessonpoints"] != DBNull.Value
                                            ? reader["lessonpoints"].ToString() : "";
                    lblLsnModuleName.Text = "Module: " + Server.HtmlEncode(reader["modulename"].ToString());
                    LessonModuleId = Convert.ToInt32(reader["moduleid"]);
                }
            }
        }

        // Validates and saves a lesson — inserts a new one or updates the existing one, then handles file upload
        protected void btnSaveLesson_Click(object sender, EventArgs e)
        {
            try
            {
                string title = txtLsnTitle.Text.Trim();
                string desc = txtLsnDesc.Text.Trim();
                string videoUrl = txtLsnVideoUrl.Text.Trim();

                if (string.IsNullOrWhiteSpace(title))
                {
                    ShowMsg(lblLsnMsg, "Lesson title is required.", false);
                    return;
                }

                int duration;
                if (!int.TryParse(txtLsnDuration.Text, out duration) || duration < 1 || duration > 600)
                {
                    ShowMsg(lblLsnMsg, "Invalid duration (1–600 minutes).", false);
                    return;
                }

                // Points are optional — leave blank for no gamification on this lesson
                int? lessonPoints = null;
                if (!string.IsNullOrWhiteSpace(txtLsnPoints.Text))
                {
                    int pts;
                    if (!int.TryParse(txtLsnPoints.Text.Trim(), out pts) || pts < 0 || pts > 10000)
                    {
                        ShowMsg(lblLsnMsg, "Points must be a whole number 0–10000.", false);
                        return;
                    }
                    lessonPoints = pts;
                }

                title = Server.HtmlEncode(title);
                desc = Server.HtmlEncode(desc);
                videoUrl = Server.HtmlEncode(videoUrl);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    int currentLessonId = EditLessonId;

                    if (EditLessonId > 0)
                    {
                        // The WHERE clause includes a subquery to confirm the lesson belongs to this course
                        SqlCommand cmd = new SqlCommand(@"
                            UPDATE Lesson
                            SET lessontitle=@title, description=@desc, duration=@duration, lessonpoints=@pts
                            WHERE lessonid=@id AND moduleid IN
                                (SELECT moduleid FROM Module WHERE courseid=@cid)", con);
                        cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = title;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
                        cmd.Parameters.Add("@pts", SqlDbType.Int).Value =
                            lessonPoints.HasValue ? (object)lessonPoints.Value : DBNull.Value;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = EditLessonId;
                        cmd.Parameters.Add("@cid", SqlDbType.Int).Value = EditCourseId;
                        cmd.ExecuteNonQuery();
                        LearnSphere_WAPP.Syslog.action(CurrentUserId, "Updated Lesson (LessonID: " + EditLessonId + ")");
                    }
                    else
                    {
                        // Order number computed from the DB so there are no gaps or conflicts
                        SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO Lesson
                            (moduleid, lessontitle, description, duration, lessonpoints, ordernumber, creationtime)
                            OUTPUT INSERTED.lessonid
                            VALUES
                            (@moduleid, @title, @desc, @duration, @pts,
                             (SELECT ISNULL(MAX(ordernumber),0)+1 FROM Lesson WHERE moduleid=@moduleid AND deletiontime IS NULL),
                             GETDATE())", con);
                        cmd.Parameters.Add("@moduleid", SqlDbType.Int).Value = LessonModuleId;
                        cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = title;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
                        cmd.Parameters.Add("@pts", SqlDbType.Int).Value =
                            lessonPoints.HasValue ? (object)lessonPoints.Value : DBNull.Value;
                        currentLessonId = (int)cmd.ExecuteScalar();
                    }

                    // File upload — allowed types: PDF, DOC, DOCX, PPT, PPTX, max 5 MB
                    // Saved with a random GUID name to avoid conflicts
                    if (fuLsnFile.HasFile)
                    {
                        if (fuLsnFile.PostedFile.ContentLength > 5 * 1024 * 1024)
                        {
                            ShowMsg(lblLsnMsg, "File too large (max 5MB).", false);
                            return;
                        }
                        string ext = Path.GetExtension(fuLsnFile.FileName).ToLower();
                        string[] allow = { ".pdf", ".doc", ".docx", ".ppt", ".pptx" };
                        if (Array.IndexOf(allow, ext) < 0)
                        {
                            ShowMsg(lblLsnMsg, "Invalid file type.", false);
                            return;
                        }
                        string folder = Server.MapPath("~/Uploads/LessonMaterials/");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                        string fileName = Guid.NewGuid().ToString() + ext;
                        fuLsnFile.SaveAs(Path.Combine(folder, fileName));
                        LearnSphere_WAPP.Syslog.action(CurrentUserId, "Uploaded file for Lesson (LessonID: " + currentLessonId + ")");
                    }

                    SetCourseToDraft(EditCourseId);
                }

                LoadModules(EditCourseId);
                ShowView("edit");
            }
            catch
            {
                ShowMsg(lblLsnMsg, "An error occurred. Please try again.", false);
            }
        }

        // Goes back to the courses list from the students panel
        protected void btnBackFromStudents_Click(object sender, EventArgs e)
        {
            LoadCourses();
            ShowView("courses");
        }

        // Shows the course name as a subtitle on the students panel
        private void LoadStudentsCourseTitle(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT coursename FROM Course WHERE courseid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;
                con.Open();
                object r = cmd.ExecuteScalar();
                lblStudentsCourseTitle.Text = r != null ? Server.HtmlEncode(r.ToString()) : "";
            }
        }

        // Loads only active, verified students who are enrolled in the course
        private void LoadStudents(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT
                        u.userid, u.uname, u.fname, u.lname, u.email, u.age, u.gender,
                        e.enrolldate AS EnrolledOn
                    FROM Enrollment e
                    INNER JOIN Course c  ON e.courseid = c.courseid
                    INNER JOIN [User] u  ON e.userid   = u.userid
                    WHERE e.courseid=@courseId
                      AND e.isactive=1
                      AND u.usertype='Student'
                      AND u.deletiontime IS NULL
                      AND u.status='Active'", con);
                cmd.Parameters.AddWithValue("@courseId", courseId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvStudents.DataSource = dt;
                gvStudents.DataBind();

                if (dt.Rows.Count == 0)
                    ShowMsg(lblStudentsMsg, "No enrolled students found.", false);
            }
        }

        // Handles Remove and View Receipt commands from the students grid
        protected void gvStudents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteStudent" && e.CommandName != "ViewReceipt") return;
            int index = Convert.ToInt32(e.CommandArgument);
            int userId = Convert.ToInt32(gvStudents.DataKeys[index].Value);
            int courseId = StudentsForCourseId;

            if (e.CommandName == "DeleteStudent")
            {
                RemoveEnrollment(userId, courseId);
                LoadStudents(courseId);
                ShowView("students");
            }
            else if (e.CommandName == "ViewReceipt")
            {
                GenerateReceipt(userId, courseId);
            }
        }

        // Removes the enrollment and cleans up any associated invoice and receipt records
        private void RemoveEnrollment(int userId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                new SqlCommand("DELETE FROM Enrollment WHERE userid=@uid AND courseid=@cid", con)
                { Parameters = { new SqlParameter("@uid", userId), new SqlParameter("@cid", courseId) } }
                    .ExecuteNonQuery();

                // If there's an invoice, clean up the receipt first then the invoice
                SqlCommand getInvoice = new SqlCommand(
                    "SELECT invid FROM Invoice WHERE userid=@uid AND courseid=@cid", con);
                getInvoice.Parameters.AddWithValue("@uid", userId);
                getInvoice.Parameters.AddWithValue("@cid", courseId);
                object invoiceIdObj = getInvoice.ExecuteScalar();

                if (invoiceIdObj != null)
                {
                    int invId = Convert.ToInt32(invoiceIdObj);
                    new SqlCommand("DELETE FROM Receipt WHERE invid=@invid", con)
                    { Parameters = { new SqlParameter("@invid", invId) } }.ExecuteNonQuery();
                    new SqlCommand("DELETE FROM Invoice WHERE invid=@invid", con)
                    { Parameters = { new SqlParameter("@invid", invId) } }.ExecuteNonQuery();
                }
            }
            LearnSphere_WAPP.Syslog.action(CurrentUserId,
                "Student removed from course (CourseID: " + courseId + ", UserID: " + userId + ")");
            ShowMsg(lblStudentsMsg, "Student removed from course.", true);
        }

        // Fetches the data needed for the receipt and hands it to the PDF generator
        private void GenerateReceipt(int userId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT u.fname + ' ' + u.lname AS StudentName,
                           c.coursename,
                           ISNULL(i.amount, 0)                 AS amount,
                           ISNULL(i.creationtime, e.enrolldate) AS creationtime
                    FROM Enrollment e
                    INNER JOIN [User] u ON e.userid   = u.userid
                    INNER JOIN Course c ON e.courseid  = c.courseid
                    LEFT  JOIN Invoice i ON e.userid   = i.userid AND e.courseid = i.courseid
                    WHERE e.userid=@uid AND e.courseid=@cid", con);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@cid", courseId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    GeneratePdf(
                        reader["StudentName"].ToString(),
                        reader["coursename"].ToString(),
                        Convert.ToDecimal(reader["amount"]),
                        Convert.ToDateTime(reader["creationtime"]));
                }
                else
                {
                    ShowMsg(lblStudentsMsg, "No receipt data found.", false);
                }
            }
        }

        // Builds a PDF receipt using iTextSharp and streams it directly to the browser
        private void GeneratePdf(string studentName, string courseName, decimal amount, DateTime date)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=Receipt.pdf");

            Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter.GetInstance(doc, Response.OutputStream);
            doc.Open();

            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            Paragraph title = new Paragraph("LearnSphere Receipt", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);
            doc.Add(new Paragraph(" "));

            Font subFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Paragraph subtitle = new Paragraph("Official Payment Receipt", subFont);
            subtitle.Alignment = Element.ALIGN_CENTER;
            doc.Add(subtitle);
            doc.Add(new Paragraph("\n"));

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 30f, 70f });

            Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            void AddRow(string label, string value)
            {
                PdfPCell c1 = new PdfPCell(new Phrase(label, labelFont)) { Border = Rectangle.NO_BORDER, Padding = 8 };
                PdfPCell c2 = new PdfPCell(new Phrase(value, valueFont)) { Border = Rectangle.NO_BORDER, Padding = 8 };
                table.AddCell(c1); table.AddCell(c2);
            }

            AddRow("Student Name:", studentName);
            AddRow("Course Name:", courseName);
            AddRow("Amount Paid:", "RM " + amount.ToString("N2"));
            AddRow("Payment Date:", date.ToString("dd MMM yyyy"));
            doc.Add(table);
            doc.Add(new Paragraph("\n"));

            PdfPTable line = new PdfPTable(1);
            line.WidthPercentage = 100;
            PdfPCell lineCell = new PdfPCell(new Phrase(""))
            { BorderWidthBottom = 1f, Border = Rectangle.BOTTOM_BORDER };
            line.AddCell(lineCell);
            doc.Add(line);
            doc.Add(new Paragraph("\n"));

            Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10);
            Paragraph footer = new Paragraph(
                "Thank you for your payment.\nThis is a system-generated receipt.", footerFont);
            footer.Alignment = Element.ALIGN_CENTER;
            doc.Add(footer);

            doc.Close();
            Response.Flush();
            Response.End();
        }

        // Exports the student list to an Excel file — only includes paid students for paid courses
        protected void btnExport_Click(object sender, EventArgs e)
        {
            int courseId = StudentsForCourseId;
            if (courseId == 0) return;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string courseName = new SqlCommand(
                    "SELECT coursename FROM Course WHERE courseid=@cid", con)
                { Parameters = { new SqlParameter("@cid", courseId) } }
                    .ExecuteScalar()?.ToString() ?? "Course";

                // Sanitise the course name so it's safe to use as a filename
                foreach (char c in Path.GetInvalidFileNameChars())
                    courseName = courseName.Replace(c, '_');
                courseName = courseName.Replace(" ", "_");

                SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT
                        u.userid AS [User ID], u.uname AS [Username],
                        u.fname AS [First Name], u.lname AS [Last Name],
                        u.email AS [Email], u.age AS [Age], u.gender AS [Gender],
                        e.enrolldate AS [Enrolled On]
                    FROM Enrollment e
                    INNER JOIN Course c ON e.courseid = c.courseid
                    INNER JOIN [User] u ON e.userid   = u.userid
                    LEFT  JOIN Invoice i ON e.userid   = i.userid AND e.courseid = i.courseid
                    LEFT  JOIN Receipt r ON i.invid    = r.invid
                    WHERE e.courseid=@courseId AND e.isactive=1
                      AND u.usertype='Student' AND u.deletiontime IS NULL AND u.status='Active'
                      AND (c.price=0 OR (c.price>0 AND r.invid IS NOT NULL))", con);
                cmd.Parameters.AddWithValue("@courseId", courseId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                string fileName = $"{courseName}_EnrolledStudents_{DateTime.Now:ddMMMyyyy}.xls";
                ExportToExcel(dt, fileName);
                LearnSphere_WAPP.Syslog.action(CurrentUserId, "Export student information csv file");
            }
        }

        // Renders a DataTable as an HTML table and streams it as an XLS file
        private void ExportToExcel(DataTable dt, string fileName)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                GridView gv = new GridView();
                gv.DataSource = dt;
                gv.DataBind();
                gv.RenderControl(hw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }

        // Required override so ASP.NET doesn't throw when rendering a GridView outside of a form for the export
        public override void VerifyRenderingInServerForm(Control control) { }

        // Goes back to the edit panel from the review screen
        protected void btnBackToEdit_Click(object sender, EventArgs e)
        {
            LoadCourseInfo(EditCourseId);
            LoadModules(EditCourseId);
            ShowView("edit");
        }

        // Fetches course details for the review summary, with an ownership check
        private void LoadCourseForReview()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT coursename, description, price
                    FROM Course WHERE courseid=@id AND ownerid=@uid", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = EditCourseId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserId;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblReviewCourseName.Text = Server.HtmlEncode(reader["coursename"].ToString());
                    lblReviewCourseDesc.Text = Server.HtmlEncode(reader["description"].ToString());
                    lblReviewCoursePrice.Text = "RM " + reader["price"].ToString();
                }
                else
                {
                    LoadCourses(); ShowView("courses");
                }
            }
        }

        // Builds the nested module/lesson structure for the review repeater, and loads the exam summary if one exists
        private void LoadModulesAndLessonsForReview()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                var modules = new List<ModuleView>();

                SqlCommand mCmd = new SqlCommand(@"
                    SELECT moduleid, modulename FROM Module
                    WHERE courseid=@courseid AND deletiontime IS NULL
                    ORDER BY ordernumber", con);
                mCmd.Parameters.Add("@courseid", SqlDbType.Int).Value = EditCourseId;
                SqlDataReader mr = mCmd.ExecuteReader();
                while (mr.Read())
                    modules.Add(new ModuleView
                    {
                        moduleid = Convert.ToInt32(mr["moduleid"]),
                        modulename = mr["modulename"].ToString(),
                        Lessons = new List<LessonView>()
                    });
                mr.Close();

                foreach (var module in modules)
                {
                    SqlCommand lCmd = new SqlCommand(@"
                        SELECT lessontitle, duration, lessonpoints FROM Lesson
                        WHERE moduleid=@mid AND deletiontime IS NULL
                        ORDER BY ordernumber", con);
                    lCmd.Parameters.Add("@mid", SqlDbType.Int).Value = module.moduleid;
                    SqlDataReader lr = lCmd.ExecuteReader();
                    while (lr.Read())
                        module.Lessons.Add(new LessonView
                        {
                            lessontitle = lr["lessontitle"].ToString(),
                            duration = Convert.ToInt32(lr["duration"]),
                            lessonpoints = lr["lessonpoints"]
                        });
                    lr.Close();
                }

                rptReviewModules.DataSource = modules;
                rptReviewModules.DataBind();

                // Only show the exam panel if an exam has been created for this course
                SqlCommand eCmd = new SqlCommand(@"
                    SELECT examtitle, exampoints,
                        (SELECT COUNT(*) FROM ExamQuestion WHERE examid=e.examid) AS qcount
                    FROM Exam e WHERE e.courseid=@cid", con);
                eCmd.Parameters.Add("@cid", SqlDbType.Int).Value = EditCourseId;
                SqlDataReader er = eCmd.ExecuteReader();
                if (er.Read())
                {
                    pnlCourseExam.Visible = true;
                    lblCourseExamTitle.Text = Server.HtmlEncode(er["examtitle"].ToString());
                    lblCourseExamQuestions.Text = er["qcount"].ToString()
                        + " questions · ⚡ " + er["exampoints"].ToString() + " pts";
                }
                else
                {
                    pnlCourseExam.Visible = false;
                }
                er.Close();
            }
        }

        // Returns false if the course has no modules, or has modules but no lessons
        private bool IsCourseValidForPublish()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand c1 = new SqlCommand(
                    "SELECT COUNT(*) FROM Module WHERE courseid=@id AND deletiontime IS NULL", con);
                c1.Parameters.Add("@id", SqlDbType.Int).Value = EditCourseId;
                if ((int)c1.ExecuteScalar() == 0) return false;

                SqlCommand c2 = new SqlCommand(@"
                    SELECT COUNT(*) FROM Lesson
                    WHERE moduleid IN (SELECT moduleid FROM Module WHERE courseid=@id)", con);
                c2.Parameters.Add("@id", SqlDbType.Int).Value = EditCourseId;
                if ((int)c2.ExecuteScalar() == 0) return false;

                return true;
            }
        }

        // Sets the course to Active — the ownership check in the WHERE means only the real owner can publish
        protected void btnPublish_Click(object sender, EventArgs e)
        {
            if (!IsCourseValidForPublish())
            {
                ShowMsg(lblPublishMsg, "Course must have at least one module and lesson.", false);
                return;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Course SET status='Active' WHERE courseid=@id AND ownerid=@uid", con);
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = EditCourseId;
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserId;
                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        ShowMsg(lblPublishMsg, "Unauthorized action.", false);
                        return;
                    }
                    LearnSphere_WAPP.Syslog.action(CurrentUserId, "Publish Course (CourseID: " + EditCourseId + ")");
                }
                LoadCourses();
                ShowView("courses");
            }
            catch
            {
                ShowMsg(lblPublishMsg, "Error publishing course. Please try again.", false);
            }
        }

        // Sets the course back to Draft whenever content is edited so it can't be live with stale content
        private void SetCourseToDraft(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Course SET status='Unactive' WHERE courseid=@id AND ownerid=@uid", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserId;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Confirms the given course belongs to the currently logged-in lecturer
        private bool IsCourseOwner(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Course WHERE courseid=@cid AND ownerid=@uid", con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserId;
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Confirms the module belongs to the given course — used before acting on any module command
        private bool IsModuleValid(int moduleId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Module WHERE moduleid=@mid AND courseid=@cid", con);
                cmd.Parameters.Add("@mid", SqlDbType.Int).Value = moduleId;
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Confirms the lesson belongs to the given course via its module — used before acting on any lesson command
        private bool IsLessonValid(int lessonId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Lesson l
                    JOIN Module m ON l.moduleid = m.moduleid
                    WHERE l.lessonid=@lid AND m.courseid=@cid", con);
                cmd.Parameters.Add("@lid", SqlDbType.Int).Value = lessonId;
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
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
            LearnSphere_WAPP.Syslog.action(CurrentUserId, "Logout system");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}