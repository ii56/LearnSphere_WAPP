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
    public partial class ViewStudents : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["YourConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["courseId"] != null)
                {
                    int courseId = Convert.ToInt32(Request.QueryString["courseId"]);
                    LoadStudents(courseId);
                }
                else
                {
                    lblMessage.Text = "Invalid course selected.";
                }
            }
        }

        private void LoadStudents(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
            SELECT DISTINCT 
                u.userid,
                u.uname,
                u.fname,
                u.lname,
                u.email,
                u.age,
                u.gender,
                i.creationtime AS EnrolledOn
            FROM Invoice i
            INNER JOIN Receipt r ON i.invid = r.invid
            INNER JOIN [User] u ON i.userid = u.userid
            WHERE i.courseid = @courseId
              AND u.usertype = 'Student'
              AND u.deletiontime IS NULL
              AND u.status = 1
        ";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@courseId", courseId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvStudents.DataSource = dt;
                    gvStudents.DataBind();
                }
                else
                {
                    lblMessage.Text = "No enrolled students found.";
                }
            }
        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewCourses.aspx");
        }
    }
}