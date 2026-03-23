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

        // Ties the ViewState to the user session to prevent CSRF attacks
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        // Redirects non-lecturers away, reads the query string params, then loads the right initial state
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
                lblBannerTitle.Text = isEdit ? "Edit Exam" : "Create Exam";
                CreateQuestionTable();
                LoadExamType();

                if (isEdit)
                    LoadExamForEdit();
                else
                    LoadCourses();
            }

            // Runs on every postback so the question count and points badges stay current
            RefreshQCount();
        }

        // Pulls the lecturer's profile picture and sets it in the header avatar
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

        // Confirms the given course belongs to the currently logged-in lecturer
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

        // Creates an empty in-memory DataTable to hold questions as the lecturer builds the exam
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

        // Updates the question count, total points badges, and shows/hides the empty state message
        private void RefreshQCount()
        {
            DataTable dt = ViewState["questions"] as DataTable;
            int count = dt != null ? dt.Rows.Count : 0;

            int total = 0;
            if (dt != null)
                foreach (DataRow r in dt.Rows)
                {
                    int m;
                    if (int.TryParse(r["Marks"].ToString(), out m))
                        total += m;
                }

            lblQCount.Text = count.ToString();
            lblTotalPoints.Text = total.ToString();
            lblFinalTotalPoints.Text = total.ToString();
            pnlEmpty.Visible = (count == 0);
        }

        // Populates the exam type dropdown with Course Exam and Module Exam options
        private void LoadExamType()
        {
            ddlExamType.Items.Clear();
            ddlExamType.Items.Add(new ListItem("Select Exam Type", ""));
            ddlExamType.Items.Add(new ListItem("Course Exam", "course"));
            ddlExamType.Items.Add(new ListItem("Module Exam", "module"));
        }

        // Loads all active courses owned by this lecturer into the target dropdown
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

        // Fetches the existing exam title and all its questions so the lecturer can edit them
        private void LoadExamForEdit()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Look up by module or course depending on which was passed in the query string
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

                // Load existing questions into the ViewState DataTable so the grid can render them
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

        // Swaps the target dropdown between courses and modules when the exam type changes
        protected void ddlExamType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlTarget.Items.Clear();
            ddlTarget.Items.Add(new ListItem("Select Target", ""));

            if (ddlExamType.SelectedValue == "course")
                LoadCourses();
            else if (ddlExamType.SelectedValue == "module")
                LoadModulesForCourse();
        }

        // Loads the modules belonging to the current course into the target dropdown
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

        // Pulls the selected question's data into the form fields and removes it from the list so it can be re-added after editing
        protected void gvQuestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvQuestions.SelectedIndex;
            if (index < 0) return;

            DataTable dt = ViewState["questions"] as DataTable;
            if (dt == null || index >= dt.Rows.Count) return;

            DataRow row = dt.Rows[index];

            txtQuestion.Text = row["Question"].ToString();
            txtA.Text = row["A"].ToString();
            txtB.Text = row["B"].ToString();
            txtC.Text = row["C"].ToString();
            txtD.Text = row["D"].ToString();
            txtMarks.Text = row["Marks"].ToString();
            ddlCorrect.SelectedValue = row["Correct"].ToString();

            dt.Rows.RemoveAt(index);
            ViewState["questions"] = dt;
            gvQuestions.SelectedIndex = -1;
            gvQuestions.DataSource = dt;
            gvQuestions.DataBind();

            RefreshQCount();
            ShowMsg("Question loaded into the form. Edit the fields above and click \"+ Add Question\" to save.", true);
        }

        // Validates the inputs, XSS-encodes them, then appends the question to the in-memory table
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

        // Checks for empty question text, duplicate options, and marks within the allowed range
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

        // Resets the question form back to its default state ready for the next entry
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

        // Validates that a target and questions exist, then saves and redirects to View Courses
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

        // Same validation as Publish but saves without redirecting, leaving the page open to continue editing
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

        // Sums all per-question marks to get the total, inserts the exam and all questions in a single transaction
        private void SaveExam(DataTable questions, bool publish)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    // Total is stored in both totalmarks and exampoints so students and the review panel both see it
                    int totalPoints = 0;
                    foreach (DataRow r in questions.Rows)
                    {
                        int m;
                        if (int.TryParse(r["Marks"].ToString(), out m))
                            totalPoints += m;
                    }

                    string insertExam = ddlExamType.SelectedValue == "module"
                        ? @"INSERT INTO Exam (moduleid, examtitle, totalmarks, exampoints)
                            OUTPUT INSERTED.examid
                            VALUES (@target, @title, @total, @total)"
                        : @"INSERT INTO Exam (courseid, examtitle, totalmarks, exampoints)
                            OUTPUT INSERTED.examid
                            VALUES (@target, @title, @total, @total)";

                    SqlCommand cmd = new SqlCommand(insertExam, con, trans);
                    cmd.Parameters.Add("@target", SqlDbType.Int).Value = Convert.ToInt32(ddlTarget.SelectedValue);
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = Server.HtmlEncode(txtExamTitle.Text.Trim());
                    cmd.Parameters.Add("@total", SqlDbType.Int).Value = totalPoints;
                    examId = (int)cmd.ExecuteScalar();

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

        // Discards the current exam and returns to the courses list
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewCourses.aspx");
        }

        // Clears the session and sends the lecturer back to the login page
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(userId, "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        // Sets the message label's text, style and visibility in one call
        private void ShowMsg(string msg, bool success)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = "alert " + (success ? "alert-success" : "alert-error");
            lblMessage.Visible = true;
        }
    }
}