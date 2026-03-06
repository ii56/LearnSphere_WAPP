using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class StudentDashboard : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            string displayName = Session["fname"] != null
                ? Session["fname"].ToString()
                : Session["uname"]?.ToString() ?? "Student";

            lblWelcome.Text = displayName;
            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Enrollment WHERE userid = @uid AND isactive = 1", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    lblEnrolled.Text = cmd.ExecuteScalar().ToString();
                }

                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM LessonProgress WHERE userid = @uid AND iscompleted = 1", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    lblCompleted.Text = cmd.ExecuteScalar().ToString();
                }

                using (SqlCommand cmd = new SqlCommand(
                    "SELECT totalpoints, badge FROM StudentPoints WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblPoints.Text = reader["totalpoints"].ToString();
                            lblBadge.Text = reader["badge"]?.ToString() ?? "Bronze";
                        }
                        else
                        {
                            lblPoints.Text = "0";
                            lblBadge.Text = "Bronze";
                        }
                    }
                }

                string query = @"
                    SELECT c.courseid, c.coursename, c.category, e.enrolldate
                    FROM Course c
                    INNER JOIN Enrollment e ON c.courseid = e.courseid
                    WHERE e.userid = @uid
                      AND e.isactive = 1
                      AND c.status = 1
                    ORDER BY e.enrolldate DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        gvCourses.DataSource = dt;
                        gvCourses.DataBind();
                        lblCourseCount.Text = $"{dt.Rows.Count} course{(dt.Rows.Count != 1 ? "s" : "")}";
                        pnlEmpty.Visible = false;
                        pnlCourses.Visible = true;
                    }
                    else
                    {
                        pnlCourses.Visible = false;
                        pnlEmpty.Visible = true;
                        lblCourseCount.Text = "0 courses";
                    }
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}