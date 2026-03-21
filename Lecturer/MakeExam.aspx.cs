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
    public partial class MakeExam : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int courseId;
        int moduleId;
        int examId;
        bool isEdit = false;
        int userId;

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
            if (Session["userid"] == null || Session["usertype"]?.ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);

            int.TryParse(Request.QueryString["courseid"], out courseId);
            int.TryParse(Request.QueryString["moduleid"], out moduleId);
            isEdit = Request.QueryString["edit"] == "1";

            if (!IsOwner(courseId))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            LoadSidebarProfileImage();

            if (!IsPostBack)
            {
                // Banner title changes for edit mode
                lblBannerTitle.Text = isEdit ? "Edit Exam" : "Create Exam";

                CreateQuestionTable();
                LoadExamType();

                if (isEdit)
                    LoadExamForEdit();
                else
                    LoadCourses();
            }

            RefreshQCount();
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
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                con.Open();
                object result = cmd.ExecuteScalar();
                imgSidebarProfile.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // OWNERSHIP CHECK — matches original exactly
        // ══════════════════════════════════════════════════════════════════════
        private bool IsOwner(int cid)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Course WHERE courseid=@cid AND ownerid=@uid", con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = cid;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // QUESTION TABLE SETUP — matches original exactly
        // ══════════════════════════════════════════════════════════════════════
        private void CreateQuestionTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Question");
            dt.Columns.Add("A");
            dt.Columns.Add("B");
            dt.Columns.Add("C");
            dt.Columns.Add("D");
            dt.Columns.Add("Correct");
            dt.Columns.Add("Marks");
            ViewState["questions"] = dt;
        }

        // Refresh the question count badge and empty state panel
        private void RefreshQCount()
        {
            DataTable dt = ViewState["questions"] as DataTable;
            int count = dt != null ? dt.Rows.Count : 0;
            lblQCount.Text = count.ToString();
            pnlEmpty.Visible = (count == 0);
        }

        // ══════════════════════════════════════════════════════════════════════
        // LOAD EXAM TYPE — matches original exactly
        // ══════════════════════════════════════════════════════════════════════
        private void LoadExamType()
        {
            ddlExamType.Items.Clear();
            ddlExamType.Items.Add(new ListItem("Select Exam Type", ""));
            ddlExamType.Items.Add(new ListItem("Course Exam", "course"));
            ddlExamType.Items.Add(new ListItem("Module Exam", "module"));
        }

        // ══════════════════════════════════════════════════════════════════════
        // LOAD COURSES — matches original exactly
        // ══════════════════════════════════════════════════════════════════════
        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT courseid, coursename FROM Course WHERE ownerid=@uid AND deletiontime IS NULL", con);
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                con.Open();
                ddlTarget.DataSource = cmd.ExecuteReader();
                ddlTarget.DataTextField = "coursename";
                ddlTarget.DataValueField = "courseid";
                ddlTarget.DataBind();
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // LOAD FOR EDIT MODE — matches original LoadExamForEdit exactly
        // ══════════════════════════════════════════════════════════════════════
        private void LoadExamForEdit()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = moduleId > 0
                    ? "SELECT examid, examtitle FROM Exam WHERE moduleid=@id"
                    : "SELECT examid, examtitle FROM Exam WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value =
                    moduleId > 0 ? moduleId : courseId;

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    examId = Convert.ToInt32(reader["examid"]);
                    txtExamTitle.Text = Server.HtmlEncode(reader["examtitle"].ToString());
                }
                reader.Close();

                // Load existing questions into ViewState table
                SqlCommand qCmd = new SqlCommand(@"
                    SELECT questiontext, optionA, optionB, optionC, optionD,
                           correctanswer, marks
                    FROM ExamQuestion
                    WHERE examid=@examid", con);
                qCmd.Parameters.Add("@examid", SqlDbType.Int).Value = examId;
                SqlDataReader qReader = qCmd.ExecuteReader();

                DataTable dt = (DataTable)ViewState["questions"];
                while (qReader.Read())
                {
                    dt.Rows.Add(
                        qReader["questiontext"].ToString(),
                        qReader["optionA"].ToString(),
                        qReader["optionB"].ToString(),
                        qReader["optionC"].ToString(),
                        qReader["optionD"].ToString(),
                        qReader["correctanswer"].ToString(),
                        qReader["marks"].ToString()
                    );
                }
                ViewState["questions"] = dt;

                gvQuestions.DataSource = dt;
                gvQuestions.DataBind();
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // EXAM TYPE CHANGED (AutoPostBack)
        // ══════════════════════════════════════════════════════════════════════
        protected void ddlExamType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlTarget.Items.Clear();
            ddlTarget.Items.Add(new ListItem("Select Target", ""));

            if (ddlExamType.SelectedValue == "course")
                LoadCourses();
            else if (ddlExamType.SelectedValue == "module")
                LoadModulesForCourse();
        }

        // ══════════════════════════════════════════════════════════════════════
        // LOAD MODULES — matches original LoadModulesForCourse exactly
        // ══════════════════════════════════════════════════════════════════════
        private void LoadModulesForCourse()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT moduleid, modulename FROM Module
                    WHERE courseid=@cid AND deletiontime IS NULL", con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;
                con.Open();
                ddlTarget.DataSource = cmd.ExecuteReader();
                ddlTarget.DataTextField = "modulename";
                ddlTarget.DataValueField = "moduleid";
                ddlTarget.DataBind();
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // ADD QUESTION — matches original btnAddQuestion_Click exactly
        // ══════════════════════════════════════════════════════════════════════
        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            if (!ValidateQuestionInputs()) return;

            DataTable dt = (DataTable)ViewState["questions"];

            dt.Rows.Add(
                Server.HtmlEncode(txtQuestion.Text.Trim()),
                Server.HtmlEncode(txtA.Text.Trim()),
                Server.HtmlEncode(txtB.Text.Trim()),
                Server.HtmlEncode(txtC.Text.Trim()),
                Server.HtmlEncode(txtD.Text.Trim()),
                ddlCorrect.SelectedValue,
                int.Parse(txtMarks.Text)
            );

            ViewState["questions"] = dt;

            gvQuestions.DataSource = dt;
            gvQuestions.DataBind();

            ClearInputs();
            ShowMsg("Question added successfully.", true);
            RefreshQCount();
        }

        // Matches original ValidateQuestionInputs exactly
        private bool ValidateQuestionInputs()
        {
            if (string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                ShowMsg("Question text is required.", false);
                return false;
            }
            if (txtA.Text.Trim() == txtB.Text.Trim() ||
                txtA.Text.Trim() == txtC.Text.Trim() ||
                txtA.Text.Trim() == txtD.Text.Trim())
            {
                ShowMsg("All answer options must be unique.", false);
                return false;
            }
            int marks;
            if (!int.TryParse(txtMarks.Text, out marks) || marks < 1 || marks > 100)
            {
                ShowMsg("Marks must be between 1 and 100.", false);
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            txtQuestion.Text = "";
            txtA.Text = "";
            txtB.Text = "";
            txtC.Text = "";
            txtD.Text = "";
            txtMarks.Text = "1";
            ddlCorrect.SelectedIndex = 0;
        }

        // ══════════════════════════════════════════════════════════════════════
        // PUBLISH — matches original btnPublish_Click exactly
        // ══════════════════════════════════════════════════════════════════════
        protected void btnPublish_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTarget.SelectedValue == "" || ddlExamType.SelectedValue == "")
                {
                    ShowMsg("Select exam type and target.", false);
                    return;
                }

                DataTable questions = (DataTable)ViewState["questions"];
                if (questions == null || questions.Rows.Count == 0)
                {
                    ShowMsg("Add at least one question before publishing.", false);
                    return;
                }

                SaveExam(questions, true);
            }
            catch
            {
                ShowMsg("Unexpected error occurred.", false);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // SAVE DRAFT — matches original btnDraft_Click exactly
        // ══════════════════════════════════════════════════════════════════════
        protected void btnDraft_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTarget.SelectedValue == "" || ddlExamType.SelectedValue == "")
                {
                    ShowMsg("Select exam type and target.", false);
                    return;
                }

                DataTable questions = (DataTable)ViewState["questions"];
                if (questions == null || questions.Rows.Count == 0)
                {
                    ShowMsg("Add at least one question before saving draft.", false);
                    return;
                }

                SaveExam(questions, false);
            }
            catch
            {
                ShowMsg("Unexpected error occurred.", false);
            }
        }

        // Shared save logic for publish and draft — transaction matches original exactly
        private void SaveExam(DataTable questions, bool publish)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    // INSERT Exam — matches original exactly
                    string insertExam = ddlExamType.SelectedValue == "module"
                        ? @"INSERT INTO Exam(moduleid, examtitle, totalmarks)
                            OUTPUT INSERTED.examid
                            VALUES(@target, @title, 0)"
                        : @"INSERT INTO Exam(courseid, examtitle, totalmarks)
                            OUTPUT INSERTED.examid
                            VALUES(@target, @title, 0)";

                    SqlCommand cmd = new SqlCommand(insertExam, con, trans);
                    cmd.Parameters.Add("@target", SqlDbType.Int).Value = Convert.ToInt32(ddlTarget.SelectedValue);
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = Server.HtmlEncode(txtExamTitle.Text.Trim());
                    examId = (int)cmd.ExecuteScalar();

                    // INSERT questions — matches original exactly
                    foreach (DataRow r in questions.Rows)
                    {
                        SqlCommand qCmd = new SqlCommand(@"
                            INSERT INTO ExamQuestion
                            (examid, questiontext, optionA, optionB, optionC, optionD, correctanswer, marks)
                            VALUES
                            (@eid, @q, @A, @B, @C, @D, @correct, @marks)", con, trans);

                        qCmd.Parameters.Add("@eid", SqlDbType.Int).Value = examId;
                        qCmd.Parameters.Add("@q", SqlDbType.NVarChar).Value = r["Question"];
                        qCmd.Parameters.Add("@A", SqlDbType.NVarChar).Value = r["A"];
                        qCmd.Parameters.Add("@B", SqlDbType.NVarChar).Value = r["B"];
                        qCmd.Parameters.Add("@C", SqlDbType.NVarChar).Value = r["C"];
                        qCmd.Parameters.Add("@D", SqlDbType.NVarChar).Value = r["D"];
                        qCmd.Parameters.Add("@correct", SqlDbType.Char).Value = r["Correct"];
                        qCmd.Parameters.Add("@marks", SqlDbType.Int).Value = r["Marks"];

                        qCmd.ExecuteNonQuery();
                    }

                    trans.Commit();

                    if (publish)
                    {
                        LearnSphere_WAPP.Syslog.action(userId, "Publish Exam (ExamID: " + examId + ")");
                        Response.Redirect("ViewCourses.aspx");
                    }
                    else
                    {
                        LearnSphere_WAPP.Syslog.action(userId, "Saved Draft Exam (ExamID: " + examId + ")");
                        ShowMsg("Draft saved successfully!", true);
                    }
                }
                catch
                {
                    trans.Rollback();
                    ShowMsg(publish ? "Failed to publish exam." : "Failed to save draft.", false);
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // CANCEL
        // ══════════════════════════════════════════════════════════════════════
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewCourses.aspx");
        }

        // ══════════════════════════════════════════════════════════════════════
        // LOGOUT
        // ══════════════════════════════════════════════════════════════════════
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(userId, "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        // ══════════════════════════════════════════════════════════════════════
        // HELPER
        // ══════════════════════════════════════════════════════════════════════
        private void ShowMsg(string msg, bool success)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = "alert " + (success ? "alert-success" : "alert-error");
            lblMessage.Visible = true;
        }
    }
}