using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace LearnSphere_WAPP.Admin
{
    public partial class CourseManagement : System.Web.UI.Page
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
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

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            loadCourse();
        }

        private void loadCourse()
        {
            try
            {
                con.Open();
                string query = "Select courseid, ownerid, coursename, price, creationtime, deletiontime, category, status from Course where not status = 'Deleted'";
                SqlCommand cmd = new SqlCommand(query, con);
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    cmd.CommandText += " and coursename Like @coursename";
                    cmd.Parameters.AddWithValue("@coursename", "%" + txtSearch.Text + "%");
                }
                switch (Sortby.Text)
                {
                    case "Course ID":
                        cmd.CommandText += " order by courseid";
                        break;

                    case "Owner ID":
                        cmd.CommandText += " order by ownerid";
                        break;

                    case "Course Name":
                        cmd.CommandText += " order by coursename";
                        break;

                    case "Price":
                        cmd.CommandText += " order by price";
                        break;

                    case "Creation Time":
                        cmd.CommandText += " order by creationtime";
                        break;

                    case "Deletion Time":
                        cmd.CommandText += " order by deletiontime";
                        break;

                    case "Category":
                        cmd.CommandText += " order by category";
                        break;

                    case "Status":
                        cmd.CommandText += " order by status";
                        break;

                    default:
                        cmd.CommandText += " order by userid";
                        break;
                }
                switch (Order.Text)
                {
                    case "Ascending":
                        cmd.CommandText += " asc";
                        break;

                    case "Descending":
                        cmd.CommandText += " desc";
                        break;

                    default:
                        cmd.CommandText += " asc";
                        break;
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridView1.DataSource = dt;
                GridView1.DataBind();

                if (dt.Rows.Count == 0)
                {
                    lblResult.Text = "No user found";
                }
                else
                {
                    lblResult.Text = "";
                }
            }
            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewCourse")
            {
                string courseId = e.CommandArgument.ToString();
                LearnSphere_WAPP.Syslog.action((int)Session["userid"], "View course (courseid:" + courseId + ")");
                Response.Redirect("AdminViewCourse.aspx?courseid=" + courseId);
            }

            if (e.CommandName == "DeleteCourse")
            {
                string courseId = e.CommandArgument.ToString();

                con.Open();

                string query = "Update [Course] set status = 'Deleted', deletiontime = @deletiontime where courseid = @courseid";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@courseid", courseId);
                cmd.Parameters.AddWithValue("@deletiontime", DateTime.Now);
                cmd.ExecuteNonQuery();

                LearnSphere_WAPP.Syslog.action((int)Session["userid"], "Deleted Course (CourseID:" + courseId + ")");
                
                con.Close();
                loadCourse();
                
                Response.Write("<script>alert('Course Deleted'); window.history.back();</script>");

                
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            loadCourse();
        }

        protected void Sortby_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadCourse();
        }

        protected void Order_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadCourse();
        }
    }
}