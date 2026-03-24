using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class AttemptExam : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
                Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
            {
                LoadExam();
                Session["CurrentQ"] = 0;
                Session["Answers"] = new Dictionary<int, string>();
                ShowQuestion();
            }
        }

        private void LoadExam()
        {
            int examId = Convert.ToInt32(Request.QueryString["examid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = "SELECT * FROM ExamQuestion WHERE examid=@id ORDER BY ordernumber";
                SqlDataAdapter da = new SqlDataAdapter(q, con);
                da.SelectCommand.Parameters.AddWithValue("@id", examId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                Session["Questions"] = dt;
                lblTotal.Text = dt.Rows.Count.ToString();
            }
        }

        private void ShowQuestion()
        {
            DataTable dt = (DataTable)Session["Questions"];
            int index = (int)Session["CurrentQ"];

            DataRow row = dt.Rows[index];

            lblQNo.Text = (index + 1).ToString();
            lblQuestion.Text = row["questiontext"].ToString();

            rblOptions.Items.Clear();

            rblOptions.Items.Add(new System.Web.UI.WebControls.ListItem("A. " + row["optionA"], "A"));
            rblOptions.Items.Add(new System.Web.UI.WebControls.ListItem("B. " + row["optionB"], "B"));

            if (row["optionC"] != DBNull.Value)
                rblOptions.Items.Add(new System.Web.UI.WebControls.ListItem("C. " + row["optionC"], "C"));

            if (row["optionD"] != DBNull.Value)
                rblOptions.Items.Add(new System.Web.UI.WebControls.ListItem("D. " + row["optionD"], "D"));

            // restore selected answer
            var answers = (Dictionary<int, string>)Session["Answers"];
            if (answers.ContainsKey(index))
                rblOptions.SelectedValue = answers[index];
        }

        private void SaveAnswer()
        {
            int index = (int)Session["CurrentQ"];
            var answers = (Dictionary<int, string>)Session["Answers"];

            if (rblOptions.SelectedValue != "")
                answers[index] = rblOptions.SelectedValue;
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            SaveAnswer();

            int index = (int)Session["CurrentQ"];
            DataTable dt = (DataTable)Session["Questions"];

            if (index < dt.Rows.Count - 1)
            {
                Session["CurrentQ"] = index + 1;
                ShowQuestion();
            }
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            SaveAnswer();

            int index = (int)Session["CurrentQ"];
            if (index > 0)
            {
                Session["CurrentQ"] = index - 1;
                ShowQuestion();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SaveAnswer();

            DataTable dt = (DataTable)Session["Questions"];
            var answers = (Dictionary<int, string>)Session["Answers"];

            int score = 0;
            List<object> review = new List<object>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string correct = dt.Rows[i]["correctanswer"].ToString();
                string user = answers.ContainsKey(i) ? answers[i] : "-";

                if (user == correct)
                    score += Convert.ToInt32(dt.Rows[i]["marks"]);

                review.Add(new
                {
                    questiontext = dt.Rows[i]["questiontext"],
                    UserAnswer = user,
                    CorrectAnswer = correct,
                    IsCorrect = user == correct
                });
            }

            // save result
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO ExamResult (examid,userid,score) VALUES(@e,@u,@s)", con);

                cmd.Parameters.AddWithValue("@e", Request.QueryString["examid"]);
                cmd.Parameters.AddWithValue("@u", Session["userid"]);
                cmd.Parameters.AddWithValue("@s", score);

                cmd.ExecuteNonQuery();
            }

            Session["Review"] = review;

            pnlExam.Visible = false;
            pnlResult.Visible = true;

            lblScore.Text = score.ToString();
        }

        protected void btnReview_Click(object sender, EventArgs e)
        {
            pnlResult.Visible = false;
            pnlReview.Visible = true;

            rptReview.DataSource = Session["Review"];
            rptReview.DataBind();
        }

        protected void btnBackLesson_Click(object sender, EventArgs e)
        {
            Response.Redirect("LessonViewer.aspx?courseid=" + Request.QueryString["courseid"]);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("LessonViewer.aspx?courseid=" + Request.QueryString["courseid"]);
        }
    }
}