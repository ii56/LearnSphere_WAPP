using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class MyCourses : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null) { Response.Redirect("~/Login.aspx"); return; }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string displayName = "Student";
                if (Session["fname"] != null && Session["fname"].ToString() != "")
                {
                    displayName = Session["fname"].ToString();
                }
                else
                {
                    using (SqlCommand nameCmd = new SqlCommand("SELECT fname FROM [User] WHERE userid = @uid", con))
                    {
                        nameCmd.Parameters.AddWithValue("@uid", Convert.ToInt32(Session["userid"]));
                        object result = nameCmd.ExecuteScalar();
                        if (result != null) { displayName = result.ToString(); Session["fname"] = displayName; }
                    }
                }
                lblHeaderName.Text = displayName;
                lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();
            }

            if (!IsPostBack) LoadCourses();
        }

        private void LoadCourses()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = @"
                    SELECT c.courseid, c.coursename, c.category, e.enrolldate
                    FROM Enrollment e
                    INNER JOIN Course c ON e.courseid = c.courseid
                    WHERE e.userid = @uid AND e.isactive = 1
                    ORDER BY e.enrolldate DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // calculate real progress for each course
                    dt.Columns.Add("Progress", typeof(int));
                    foreach (DataRow row in dt.Rows)
                    {
                        int courseId = Convert.ToInt32(row["courseid"]);
                        int progress = 0;

                        try
                        {
                            // count total lessons in this course
                            using (SqlCommand totalCmd = new SqlCommand(@"
                                SELECT COUNT(*) FROM Lesson l
                                INNER JOIN Module m ON l.moduleid = m.moduleid
                                WHERE m.courseid = @cid", con))
                            {
                                totalCmd.Parameters.AddWithValue("@cid", courseId);
                                int totalLessons = (int)totalCmd.ExecuteScalar();

                                if (totalLessons > 0)
                                {
                                    // count completed lessons by this student
                                    using (SqlCommand doneCmd = new SqlCommand(@"
                                        SELECT COUNT(*) FROM LessonProgress lp
                                        INNER JOIN Lesson l ON lp.lessonid = l.lessonid
                                        INNER JOIN Module m ON l.moduleid = m.moduleid
                                        WHERE m.courseid = @cid AND lp.userid = @uid AND lp.iscompleted = 1", con))
                                    {
                                        doneCmd.Parameters.AddWithValue("@cid", courseId);
                                        doneCmd.Parameters.AddWithValue("@uid", userId);
                                        int completedLessons = (int)doneCmd.ExecuteScalar();
                                        progress = (completedLessons * 100) / totalLessons;
                                    }
                                }
                            }
                        }
                        catch { progress = 0; }

                        row["Progress"] = progress;
                    }

                    if (dt.Rows.Count > 0)
                    {
                        rptCourses.DataSource = dt;
                        rptCourses.DataBind();
                        pnlEmpty.Visible = false;
                        pnlCourses.Visible = true;
                    }
                    else
                    {
                        pnlCourses.Visible = false;
                        pnlEmpty.Visible = true;
                    }
                }
            }
        }

        protected void btnUnenrollConfirm_Click(object sender, EventArgs e)
        {
            string val = hfUnenrollId.Value;
            if (string.IsNullOrEmpty(val)) return;

            int courseId = Convert.ToInt32(val);
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Enrollment SET isactive = 0 WHERE userid = @uid AND courseid = @cid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@cid", courseId);
                    cmd.ExecuteNonQuery();
                }
            }

            hfUnenrollId.Value = "";
            lblMessage.Text = "Successfully unenrolled from the course.";
            lblMessage.CssClass = "alert alert-success";
            lblMessage.Visible = true;
            LoadCourses();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}