using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Student
{
    public partial class BrowseCourses : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string displayName = Session["fname"] != null
                ? Session["fname"].ToString()
                : Session["uname"]?.ToString() ?? "Student";

            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

            if (!IsPostBack)
                LoadCourses();
        }

        private void LoadCourses()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"
                    SELECT c.courseid, c.coursename, c.description, c.category, c.price,
                           CASE WHEN e.enrollmentid IS NOT NULL THEN 1 ELSE 0 END AS IsEnrolled
                    FROM Course c
                    LEFT JOIN Enrollment e 
                        ON c.courseid = e.courseid 
                        AND e.userid = @uid 
                        AND e.isactive = 1
                    WHERE c.status = 1
                    ORDER BY c.creationtime DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptCourses.DataSource = dt;
                    rptCourses.DataBind();
                }
            }
        }

        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string[] args = e.CommandArgument.ToString().Split('|');
            int courseId = Convert.ToInt32(args[0]);
            string courseName = args[1];
            string price = args[2];

            if (e.CommandName == "EnrollFree")
            {
                EnrollStudent(courseId);
                lblMessage.Text = "✓ Successfully enrolled in " + courseName + "!";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;
                LoadCourses();
            }
            else if (e.CommandName == "OpenPayment")
            {
                // Pass data to modal via hidden field
                hfCourseId.Value = courseId + "|" + courseName + "|" + price;
                LoadCourses();
            }
        }
        protected void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            string courseData = hfCourseId.Value;
            if (string.IsNullOrEmpty(courseData)) return;

            string[] parts = courseData.Split('|');
            int courseId = Convert.ToInt32(parts[0]);
            string courseName = parts[1];
            decimal amount = Convert.ToDecimal(parts[2]);
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Save invoice record
                using (SqlCommand invCmd = new SqlCommand(@"
            INSERT INTO Invoice (userid, courseid, amount, overdue, duration, creationtime, deadline)
            VALUES (@uid, @cid, @amount, 0, 30, @now, @deadline)", con))
                {
                    invCmd.Parameters.AddWithValue("@uid", userId);
                    invCmd.Parameters.AddWithValue("@cid", courseId);
                    invCmd.Parameters.AddWithValue("@amount", amount);
                    invCmd.Parameters.AddWithValue("@now", DateTime.Now);
                    invCmd.Parameters.AddWithValue("@deadline", DateTime.Now.AddDays(30));
                    invCmd.ExecuteNonQuery();
                }

                // Enroll student
                EnrollStudent(courseId);
            }

            hfCourseId.Value = "";
            lblMessage.Text = "✓ Payment successful! You are now enrolled in " + courseName + ".";
            lblMessage.CssClass = "alert alert-success";
            lblMessage.Visible = true;
            LoadCourses();
        }

        private void EnrollStudent(int courseId)
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                using (SqlCommand checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Enrollment WHERE userid = @uid AND courseid = @cid AND isactive = 1", con))
                {
                    checkCmd.Parameters.AddWithValue("@uid", userId);
                    checkCmd.Parameters.AddWithValue("@cid", courseId);
                    int existing = (int)checkCmd.ExecuteScalar();

                    if (existing == 0)
                    {
                        using (SqlCommand enrollCmd = new SqlCommand(
                            "INSERT INTO Enrollment (userid, courseid, enrolldate, isactive) VALUES (@uid, @cid, @date, 1)", con))
                        {
                            enrollCmd.Parameters.AddWithValue("@uid", userId);
                            enrollCmd.Parameters.AddWithValue("@cid", courseId);
                            enrollCmd.Parameters.AddWithValue("@date", DateTime.Now);
                            enrollCmd.ExecuteNonQuery();
                        }
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