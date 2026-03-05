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

        DataTable questionTable;
        int moduleId;
        int examId;
        bool isEdit = false;
        int courseId;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["courseid"] != null)
                courseId = Convert.ToInt32(Request.QueryString["courseid"]);

            if (Request.QueryString["moduleid"] != null)
                moduleId = Convert.ToInt32(Request.QueryString["moduleid"]);

            if (Request.QueryString["edit"] == "1")
                isEdit = true;

            if (!IsPostBack)
            {
                LoadExamType();

                CreateQuestionTable();

                if (isEdit)
                {
                    LoadExamForEdit();
                }
                else
                {
                    LoadCourses();
                }
            }

        }

        void LoadExamType()
        {
            ddlExamType.Items.Clear();

            ddlExamType.Items.Add(new ListItem("Select Exam Type", ""));
            ddlExamType.Items.Add(new ListItem("Course Exam", "course"));
            ddlExamType.Items.Add(new ListItem("Module Exam", "module"));
        }

        protected void ddlExamType_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ddlExamType.SelectedValue == "course")
            {
                LoadCourseOnly();
            }

            if (ddlExamType.SelectedValue == "module")
            {
                LoadModulesForCourse();
            }

        }

        void LoadCourseOnly()
        {

            ddlTarget.Items.Clear();

            using (SqlConnection con = new SqlConnection(connStr))
            {

                string query = "SELECT courseid, coursename FROM Course WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlTarget.Items.Add(new ListItem(
                        reader["coursename"].ToString(),
                        reader["courseid"].ToString()
                    ));
                }

            }

        }

        void LoadModulesForCourse()
        {

            ddlTarget.Items.Clear();

            using (SqlConnection con = new SqlConnection(connStr))
            {

                string query =
                @"SELECT moduleid, modulename
          FROM Module
          WHERE courseid=@courseid
          AND deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ddlTarget.Items.Add(new ListItem(
                        reader["modulename"].ToString(),
                        reader["moduleid"].ToString()
                    ));
                }

            }

        }

        void LoadExamForEdit()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string examQuery = "";

                // Determine whether editing module exam or course exam
                if (moduleId != 0)
                    examQuery = "SELECT examid, examtitle FROM Exam WHERE moduleid=@id";
                else
                    examQuery = "SELECT examid, examtitle FROM Exam WHERE courseid=@id";

                SqlCommand examCmd = new SqlCommand(examQuery, con);

                if (moduleId != 0)
                    examCmd.Parameters.AddWithValue("@id", moduleId);
                else
                    examCmd.Parameters.AddWithValue("@id", courseId);

                SqlDataReader reader = examCmd.ExecuteReader();

                if (reader.Read())
                {
                    examId = Convert.ToInt32(reader["examid"]);
                    txtExamTitle.Text = reader["examtitle"].ToString();
                }

                reader.Close();

                // Set dropdown selections correctly
                if (moduleId != 0)
                {
                    ddlExamType.SelectedValue = "module";
                    LoadModulesForCourse();
                    ddlTarget.SelectedValue = moduleId.ToString();
                }
                else
                {
                    ddlExamType.SelectedValue = "course";
                    LoadCourseOnly();
                    ddlTarget.SelectedValue = courseId.ToString();
                }

                // Load questions
                string q = @"SELECT questiontext, optionA, optionB, optionC, optionD,
                     correctanswer, marks
                     FROM ExamQuestion
                     WHERE examid=@examid";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@examid", examId);

                SqlDataReader qReader = cmd.ExecuteReader();

                questionTable = (DataTable)ViewState["questions"];

                while (qReader.Read())
                {
                    questionTable.Rows.Add(
                        qReader["questiontext"].ToString(),
                        qReader["optionA"].ToString(),
                        qReader["optionB"].ToString(),
                        qReader["optionC"].ToString(),
                        qReader["optionD"].ToString(),
                        qReader["correctanswer"].ToString(),
                        qReader["marks"].ToString()
                    );
                }

                ViewState["questions"] = questionTable;

                gvQuestions.DataSource = questionTable;
                gvQuestions.DataBind();
            }
        }

        void CreateQuestionTable()
        {

            questionTable = new DataTable();

            questionTable.Columns.Add("Question");
            questionTable.Columns.Add("A");
            questionTable.Columns.Add("B");
            questionTable.Columns.Add("C");
            questionTable.Columns.Add("D");
            questionTable.Columns.Add("Correct");
            questionTable.Columns.Add("Marks");

            ViewState["questions"] = questionTable;

        }

        void LoadCourses()
        {

            using (SqlConnection con = new SqlConnection(connStr))
            {

                string q = "SELECT courseid,coursename FROM Course WHERE deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(q, con);

                con.Open();

                ddlTarget.DataSource = cmd.ExecuteReader();
                ddlTarget.DataTextField = "coursename";
                ddlTarget.DataValueField = "courseid";
                ddlTarget.DataBind();

            }

        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            questionTable = (DataTable)ViewState["questions"];

            if (ViewState["editRowIndex"] != null)
            {
                int index = Convert.ToInt32(ViewState["editRowIndex"]);

                questionTable.Rows[index]["Question"] = txtQuestion.Text;
                questionTable.Rows[index]["A"] = txtA.Text;
                questionTable.Rows[index]["B"] = txtB.Text;
                questionTable.Rows[index]["C"] = txtC.Text;
                questionTable.Rows[index]["D"] = txtD.Text;
                questionTable.Rows[index]["Correct"] = ddlCorrect.SelectedValue;
                questionTable.Rows[index]["Marks"] = txtMarks.Text;

                ViewState["editRowIndex"] = null;
            }
            else
            {
                questionTable.Rows.Add(
                    txtQuestion.Text,
                    txtA.Text,
                    txtB.Text,
                    txtC.Text,
                    txtD.Text,
                    ddlCorrect.SelectedValue,
                    txtMarks.Text
                );
            }

            ViewState["questions"] = questionTable;

            gvQuestions.DataSource = questionTable;
            gvQuestions.DataBind();

            // Clear fields
            txtQuestion.Text = "";
            txtA.Text = "";
            txtB.Text = "";
            txtC.Text = "";
            txtD.Text = "";
            txtMarks.Text = "1";
        }

        protected void btnReview_Click(object sender, EventArgs e)
        {

            gvQuestions.DataSource = (DataTable)ViewState["questions"];
            gvQuestions.DataBind();

        }

        protected void gvQuestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvQuestions.SelectedIndex;

            DataTable dt = (DataTable)ViewState["questions"];

            txtQuestion.Text = dt.Rows[index]["Question"].ToString();
            txtA.Text = dt.Rows[index]["A"].ToString();
            txtB.Text = dt.Rows[index]["B"].ToString();
            txtC.Text = dt.Rows[index]["C"].ToString();
            txtD.Text = dt.Rows[index]["D"].ToString();

            ddlCorrect.SelectedValue = dt.Rows[index]["Correct"].ToString();

            txtMarks.Text = dt.Rows[index]["Marks"].ToString();

            // Store index of editing row
            ViewState["editRowIndex"] = index;
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {

            using (SqlConnection con = new SqlConnection(connStr))
            {

                con.Open();

                string examInsert = "";

                // IF EDIT MODE
                if (isEdit)
                {

                    // get exam id from module
                    string getExam = "SELECT examid FROM Exam WHERE moduleid=@moduleid";

                    SqlCommand getCmd = new SqlCommand(getExam, con);
                    getCmd.Parameters.AddWithValue("@moduleid", moduleId);

                    object result = getCmd.ExecuteScalar();

                    if (result != null)
                        examId = Convert.ToInt32(result);

                    // delete old questions
                    string del = "DELETE FROM ExamQuestion WHERE examid=@examid";

                    SqlCommand delCmd = new SqlCommand(del, con);
                    delCmd.Parameters.AddWithValue("@examid", examId);
                    delCmd.ExecuteNonQuery();

                }
                else
                {

                    // CREATE NEW EXAM
                    if (ddlExamType.SelectedValue == "module")
                    {
                        examInsert = @"INSERT INTO Exam(moduleid,examtitle,totalmarks)
                               OUTPUT INSERTED.examid
                               VALUES(@target,@title,0)";
                    }
                    else
                    {
                        examInsert = @"INSERT INTO Exam(courseid,examtitle,totalmarks)
                               OUTPUT INSERTED.examid
                               VALUES(@target,@title,0)";
                    }

                    SqlCommand cmd = new SqlCommand(examInsert, con);

                    cmd.Parameters.AddWithValue("@target", ddlTarget.SelectedValue);
                    cmd.Parameters.AddWithValue("@title", txtExamTitle.Text);

                    examId = (int)cmd.ExecuteScalar();

                }

                // INSERT QUESTIONS
                DataTable questions = (DataTable)ViewState["questions"];

                foreach (DataRow r in questions.Rows)
                {

                    SqlCommand qCmd = new SqlCommand(
                    @"INSERT INTO ExamQuestion
            (examid,questiontext,optionA,optionB,optionC,optionD,correctanswer,marks)
            VALUES
            (@exam,@q,@A,@B,@C,@D,@correct,@marks)", con);

                    qCmd.Parameters.AddWithValue("@exam", examId);
                    qCmd.Parameters.AddWithValue("@q", r["Question"]);
                    qCmd.Parameters.AddWithValue("@A", r["A"]);
                    qCmd.Parameters.AddWithValue("@B", r["B"]);
                    qCmd.Parameters.AddWithValue("@C", r["C"]);
                    qCmd.Parameters.AddWithValue("@D", r["D"]);
                    qCmd.Parameters.AddWithValue("@correct", r["Correct"]);
                    qCmd.Parameters.AddWithValue("@marks", r["Marks"]);

                    qCmd.ExecuteNonQuery();

                }

            }

            Response.Redirect("ViewCourses.aspx");

        }

        protected void btnDraft_Click(object sender, EventArgs e)
        {

            using (SqlConnection con = new SqlConnection(connStr))
            {

                string q = "";

                if (ddlExamType.SelectedValue == "module")
                {
                    q = "INSERT INTO Exam(moduleid,examtitle,totalmarks) VALUES(@target,@title,0)";
                }
                else
                {
                    q = "INSERT INTO Exam(courseid,examtitle,totalmarks) VALUES(@target,@title,0)";
                }

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@target", ddlTarget.SelectedValue);
                cmd.Parameters.AddWithValue("@title", txtExamTitle.Text);

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
    }
}