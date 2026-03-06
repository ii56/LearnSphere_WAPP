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
    public partial class UserManagement : System.Web.UI.Page
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
                lblWelcome.Text = "Welcome " + Session["uname"];
                loadUsers();
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

        private void loadUsers()
        {
            try
            {
                con.Open();
                string query;
                if (Session["usertype"].ToString() == "Admin")
                    query = "Select userid, uname, email, fname, lname, age, gender, creationtime, usertype, status from [User] where not usertype = 'Admin' and not usertype = 'SuperAdmin' and not status = 'Deleted'";
                else
                    query = "Select userid, uname, email, fname, lname, age, gender, creationtime, usertype, status from [User] where not usertype = 'SuperAdmin' and not status = 'Deleted'";
                SqlCommand cmd = new SqlCommand(query, con);
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    cmd.CommandText += " and uname Like @search";
                    cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                }
                switch (Sortby.Text)
                {
                    case "User ID":
                        cmd.CommandText += " order by userid";
                        break;

                    case "Username":
                        cmd.CommandText += " order by uname";
                        break;

                    case "Age":
                        cmd.CommandText += " order by age";
                        break;

                    case "Gender":
                        cmd.CommandText += " order by gender";
                        break;

                    case "Creation Time":
                        cmd.CommandText += " order by creationtime";
                        break;

                    case "User Type":
                        cmd.CommandText += " order by usertype";
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

        protected void Sortby_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadUsers();
        }

        protected void Order_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadUsers();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            loadUsers();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUser")
            {
                string userId = e.CommandArgument.ToString();
                LearnSphere_WAPP.Syslog.action((int)Session["userid"], "Edit user (userid:" + userId + ")");
                Response.Redirect("EditUser.aspx?userid=" + userId);
            }

            if (e.CommandName == "DeleteUser")
            {
                string userId = e.CommandArgument.ToString();

                con.Open();

                string query = "Update [User] set status = 'Deleted', deletiontime = @deletiontime where userid = @userid";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@deletiontime", DateTime.Now);
                cmd.ExecuteNonQuery();

                LearnSphere_WAPP.Syslog.action((int)Session["userid"], "Deleted user (userid:" + userId + ")");
                Response.Write("<script>alert('User Deleted'); window.history.back();</script>");

                con.Close();

                loadUsers();
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            loadUsers();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }

        protected void GridView1_PageIndexChanging1(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            loadUsers();
        }
    }
}