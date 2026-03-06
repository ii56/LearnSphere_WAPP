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

            string displayName = Session["fname"] != null
                ? Session["fname"].ToString()
                : Session["uname"]?.ToString() ?? "Student";
            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

            if (!IsPostBack) LoadCourses();
        }

        private void LoadCourses()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Simple query — no Lesson/Module tables needed yet
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

                    // Add Progress column with 0 for now
                    dt.Columns.Add("Progress", typeof(int));
                    foreach (DataRow row in dt.Rows)
                        row["Progress"] = 0;

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
            lblMessage.Text = "✓ Successfully unenrolled from the course.";
            lblMessage.CssClass = "alert alert-success";
            lblMessage.Visible = true;
            LoadCourses();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear(); Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}