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
    public partial class ViewCourses : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadCourses();
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", userId);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string imagePath = result.ToString();
                    Session["profileImage"] = imagePath;
                    imgSidebarProfile.Src = ResolveUrl(imagePath);
                }
                else
                {
                    imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                }
            }
        }

        private void LoadCourses()
        {
            int lecturerId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
SELECT courseid, coursename, category, price,
    CASE 
        WHEN status = 'Active' THEN 'Published'
        WHEN status = 'Unactive' THEN 'Draft'
        WHEN status = 'Deleted' THEN 'Deleted'
        ELSE 'Unknown'
    END AS statusText
FROM Course
WHERE ownerid = @id
AND deletiontime IS NULL
";

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@id", lecturerId));

                // SEARCH
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    query += " AND coursename LIKE @search";
                    parameters.Add(new SqlParameter("@search", "%" + txtSearch.Text.Trim() + "%"));
                }

                // CATEGORY
                if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                {
                    query += " AND category = @category";
                    parameters.Add(new SqlParameter("@category", ddlCategory.SelectedValue));
                }

                // STATUS
                if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                {
                    query += " AND status = @status";
                    parameters.Add(new SqlParameter("@status", ddlStatus.SelectedValue));
                }

                // MIN PRICE
                if (!string.IsNullOrEmpty(txtMinPrice.Text))
                {
                    query += " AND price >= @minPrice";
                    parameters.Add(new SqlParameter("@minPrice", txtMinPrice.Text));
                }

                // MAX PRICE
                if (!string.IsNullOrEmpty(txtMaxPrice.Text))
                {
                    query += " AND price <= @maxPrice";
                    parameters.Add(new SqlParameter("@maxPrice", txtMaxPrice.Text));
                }

                query += " ORDER BY creationtime DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvCourses.DataSource = dt;
                gvCourses.DataBind();
            }
        }

        protected void gvCourses_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument.ToString(), out int index))
                return;

            int courseId = Convert.ToInt32(gvCourses.DataKeys[index].Value);

            if (e.CommandName == "EditCourse")
            {
                string status = gvCourses.Rows[index].Cells[3].Text;

                if (status == "Published")
                {
                    lblMessage.Text = "Published courses cannot be edited.";
                    return;
                }

                Response.Redirect("EditCourse.aspx?courseid=" + courseId);
            }
            else if (e.CommandName == "DeleteCourse")
            {
                SoftDeleteCourse(courseId);
                LoadCourses();
            }
            else if (e.CommandName == "ViewStudents")
            {
                Response.Redirect("ViewStudents.aspx?courseId=" + courseId);
            }
            else if (e.CommandName == "PreviewCourse")
            {
                Response.Redirect("Preview.aspx?courseid=" + courseId);
            }
        }

        private void SoftDeleteCourse(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = "UPDATE Course SET deletiontime = GETDATE() WHERE courseid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);
                cmd.ExecuteNonQuery();
                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Deleted Course (CourseID: " + courseId + ")");
            }

            lblMessage.Text = "Course deleted successfully.";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("~/Login.aspx");
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadCourses();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlCategory.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            txtMinPrice.Text = "";
            txtMaxPrice.Text = "";

            LoadCourses();
        }
    }
}