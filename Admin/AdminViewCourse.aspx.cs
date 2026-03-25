using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminViewCourse : System.Web.UI.Page
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
        int courseid;
        protected void Page_Load(object sender, EventArgs e)
        {
            courseid = Request.QueryString["courseid"] != null ? Convert.ToInt32(Request.QueryString["courseid"]) : 0;
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!IsPostBack)
            {
                loadCourse();
                LoadSidebarProfileImage();
            }
        }
        public void LoadSidebarProfileImage()
        {
            string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", Session["userid"]);

            con.Open();

            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                string imagePath = result.ToString();
                sidebarImg.Src = ResolveUrl(imagePath);
            }
            else
            {
                sidebarImg.Src = ResolveUrl("~/images/default-user.png");
            }
            con.Close();
        }

        private void loadCourse()
        {
            con.Open();

            string query = "Select ownerid, coursename, description, price, creationtime, deletiontime, category, status from Course where courseid = @courseid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@courseid", courseid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            lblCourseId.Text = courseid.ToString();
            lblOwnerId.Text = dt.Rows[0]["ownerid"].ToString();
            lblCname.Text = dt.Rows[0]["coursename"].ToString();
            lblDescription.Text = dt.Rows[0]["description"].ToString();
            lblPrice.Text = dt.Rows[0]["price"].ToString();
            lblCtime.Text = dt.Rows[0]["creationtime"].ToString();
            lblDtime.Text = dt.Rows[0]["deletiontime"].ToString();
            lblCategory.Text = dt.Rows[0]["category"].ToString();
            lblStatus.Text = dt.Rows[0]["status"].ToString();

            if (lblDtime.Text.IsNullOrWhiteSpace())
            {
                lblDtime.Text = "N/A";
            }

            con.Close();

        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("~/Login.aspx");
        }

        protected void btnDeleteCourse_Click(object sender, EventArgs e)
        {
            con.Open();

            string query = "Update [Course] set status = 'Deleted', deletiontime = @deletiontime where courseid = @courseid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@courseid", courseid);
            cmd.Parameters.AddWithValue("@deletiontime", DateTime.Now);
            cmd.ExecuteNonQuery();

            LearnSphere_WAPP.Syslog.action((int)Session["userid"], "Deleted Course (CourseID:" + courseid + ")");

            con.Close();

            Response.Write("<script>alert('Course Deleted'); window.location.href='CourseManagement.aspx';</script>");
        }
    }
}