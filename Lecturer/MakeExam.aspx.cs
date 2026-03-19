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

        protected void Page_Init(object sender, EventArgs e)
        {
            // 🔐 CSRF Protection
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 AUTHENTICATION
            if (Session["userid"] == null || Session["usertype"]?.ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);

            // 🔐 SAFE QUERY PARSING
            int.TryParse(Request.QueryString["courseid"], out courseId);
            int.TryParse(Request.QueryString["moduleid"], out moduleId);
            isEdit = Request.QueryString["edit"] == "1";

            // 🔐 AUTHORIZATION (CRITICAL)
            if (!IsOwner(courseId))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CreateQuestionTable();
                LoadExamType();

                if (isEdit)
                    LoadExamForEdit();
                else
                    LoadCourses();
            }
        }

        void LoadExamForEdit()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = moduleId > 0
                    ? "SELECT examid, examtitle FROM Exam WHERE moduleid=@id"
                    : "SELECT examid, examtitle FROM Exam WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);

                if (moduleId > 0)
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;
                else
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    examId = Convert.ToInt32(reader["examid"]);
                    txtExamTitle.Text = Server.HtmlEncode(reader["examtitle"].ToString());
                }

                reader.Close();

                // Load questions
                string q = @"SELECT questiontext, optionA, optionB, optionC, optionD,
                            correctanswer, marks
                     FROM ExamQuestion
                     WHERE examid=@examid";

                SqlCommand qCmd = new SqlCommand(q, con);
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

        // 🔐 CHECK COURSE OWNERSHIP
        private bool IsOwner(int cid)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = "SELECT COUNT(*) FROM Course WHERE courseid=@cid AND ownerid=@uid";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = cid;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        void CreateQuestionTable()
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

        void LoadExamType()
        {
            ddlExamType.Items.Clear();
            ddlExamType.Items.Add(new ListItem("Select Exam Type", ""));
            ddlExamType.Items.Add(new ListItem("Course Exam", "course"));
            ddlExamType.Items.Add(new ListItem("Module Exam", "module"));
        }

        void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = "SELECT courseid, coursename FROM Course WHERE ownerid=@uid AND deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;

                con.Open();
                ddlTarget.DataSource = cmd.ExecuteReader();
                ddlTarget.DataTextField = "coursename";
                ddlTarget.DataValueField = "courseid";
                ddlTarget.DataBind();
            }
        }

        protected void ddlExamType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlExamType.SelectedValue == "course")
                LoadCourses();
            else if (ddlExamType.SelectedValue == "module")
                LoadModulesForCourse();
        }

        void LoadModulesForCourse()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = @"SELECT moduleid, modulename
                             FROM Module
                             WHERE courseid=@cid AND deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;

                con.Open();
                ddlTarget.DataSource = cmd.ExecuteReader();
                ddlTarget.DataTextField = "modulename";
                ddlTarget.DataValueField = "moduleid";
                ddlTarget.DataBind();
            }
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            if (!ValidateQuestionInputs())
                return;

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
        }

        private bool ValidateQuestionInputs()
        {
            if (string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                lblMessage.Text = "Question required.";
                return false;
            }

            if (txtA.Text == txtB.Text || txtA.Text == txtC.Text || txtA.Text == txtD.Text)
            {
                lblMessage.Text = "Options must be unique.";
                return false;
            }

            if (!int.TryParse(txtMarks.Text, out int marks) || marks < 1 || marks > 100)
            {
                lblMessage.Text = "Marks must be 1–100.";
                return false;
            }

            return true;
        }

        void ClearInputs()
        {
            txtQuestion.Text = "";
            txtA.Text = "";
            txtB.Text = "";
            txtC.Text = "";
            txtD.Text = "";
            txtMarks.Text = "1";
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTarget.SelectedValue == "" || ddlExamType.SelectedValue == "")
                {
                    lblMessage.Text = "Select exam type and target.";
                    return;
                }

                DataTable questions = (DataTable)ViewState["questions"];

                if (questions.Rows.Count == 0)
                {
                    lblMessage.Text = "Add at least one question.";
                    return;
                }

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlTransaction trans = con.BeginTransaction();

                    try
                    {
                        string insertExam = ddlExamType.SelectedValue == "module"
                            ? @"INSERT INTO Exam(moduleid, examtitle, totalmarks)
                                OUTPUT INSERTED.examid
                                VALUES(@target, @title, 0)"
                            : @"INSERT INTO Exam(courseid, examtitle, totalmarks)
                                OUTPUT INSERTED.examid
                                VALUES(@target, @title, 0)";

                        SqlCommand cmd = new SqlCommand(insertExam, con, trans);
                        cmd.Parameters.Add("@target", SqlDbType.Int).Value = Convert.ToInt32(ddlTarget.SelectedValue);
                        cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = Server.HtmlEncode(txtExamTitle.Text);

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
                        Response.Redirect("ViewCourses.aspx");
                    }
                    catch
                    {
                        trans.Rollback();
                        lblMessage.Text = "Failed to publish exam.";
                    }
                }
            }
            catch
            {
                lblMessage.Text = "Unexpected error occurred.";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewCourses.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        protected void btnDraft_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate basic inputs
                if (ddlTarget.SelectedValue == "" || ddlExamType.SelectedValue == "")
                {
                    lblMessage.Text = "Select exam type and target.";
                    return;
                }

                DataTable questions = (DataTable)ViewState["questions"];

                if (questions == null || questions.Rows.Count == 0)
                {
                    lblMessage.Text = "Add at least one question before saving draft.";
                    return;
                }

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlTransaction trans = con.BeginTransaction();

                    try
                    {
                        // Insert exam (draft)
                        string insertExam = ddlExamType.SelectedValue == "module"
                            ? @"INSERT INTO Exam(moduleid, examtitle, totalmarks)
                       OUTPUT INSERTED.examid
                       VALUES(@target, @title, 0)"
                            : @"INSERT INTO Exam(courseid, examtitle, totalmarks)
                       OUTPUT INSERTED.examid
                       VALUES(@target, @title, 0)";

                        SqlCommand cmd = new SqlCommand(insertExam, con, trans);
                        cmd.Parameters.Add("@target", SqlDbType.Int).Value = Convert.ToInt32(ddlTarget.SelectedValue);
                        cmd.Parameters.Add("@title", SqlDbType.NVarChar, 100).Value = Server.HtmlEncode(txtExamTitle.Text);

                        examId = (int)cmd.ExecuteScalar();

                        // Insert questions
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

                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        lblMessage.Text = "Draft saved successfully!";
                    }
                    catch
                    {
                        trans.Rollback();
                        lblMessage.Text = "Failed to save draft.";
                    }
                }
            }
            catch
            {
                lblMessage.Text = "Unexpected error occurred.";
            }
        }
    }
}