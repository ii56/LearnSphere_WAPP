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
                    query = "SELECT u.userid, u.uname, u.email, u.fname, u.lname, u.age, u.gender, u.creationtime, u.usertype, u.status, " +
                            "CASE WHEN EXISTS (SELECT 1 FROM VerificationRequest v WHERE v.userid = u.userid AND v.status = 'Pending') THEN 1 ELSE 0 END AS HasPending " +
                            "FROM [User] u WHERE NOT u.usertype='Admin' AND NOT u.usertype='SuperAdmin' AND NOT u.status='Deleted'";
                else
                    query = "SELECT u.userid, u.uname, u.email, u.fname, u.lname, u.age, u.gender, u.creationtime, u.usertype, u.status " +
                            "CASE WHEN EXISTS (SELECT 1 FROM VerificationRequest v WHERE v.userid = u.userid AND v.status = 'Pending') THEN 1 ELSE 0 END AS HasPending " +
                            "FROM [User] u WHERE NOT u.usertype='SuperAdmin' AND NOT u.status='Deleted'";

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    query += " AND u.uname LIKE @search";
                }

                if (chkPending.Checked)
                {
                    query += " AND EXISTS (SELECT 1 FROM VerificationRequest v WHERE v.userid=u.userid AND v.status='Pending')";
                }

                // 排序
                switch (Sortby.Text)
                {
                    case "User ID": query += " ORDER BY u.userid"; break;
                    case "Username": query += " ORDER BY u.uname"; break;
                    case "Age": query += " ORDER BY u.age"; break;
                    case "Gender": query += " ORDER BY u.gender"; break;
                    case "Creation Time": query += " ORDER BY u.creationtime"; break;
                    case "User Type": query += " ORDER BY u.usertype"; break;
                    case "Status": query += " ORDER BY u.status"; break;
                    default: query += " ORDER BY u.userid"; break;
                }
                query += (Order.Text == "Descending") ? " DESC" : " ASC";

                SqlCommand cmd = new SqlCommand(query, con);
                if (!string.IsNullOrEmpty(txtSearch.Text))
                    cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridView1.DataSource = dt;
                GridView1.DataBind();

                lblResult.Text = dt.Rows.Count == 0 ? "No user found" : "";
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

                LearnSphere_WAPP.Syslog.action((int)Session["userid"], "Deleted User (UserID:" + userId + ")");
                Response.Write("<script>alert('User Deleted'); window.history.back();</script>");

                con.Close();

                loadUsers();
            }

            if (e.CommandName == "VerifyUser")
            {
                string userId = e.CommandArgument.ToString();
                Response.Redirect("AdminVerifyUser.aspx?userid=" + userId);
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            loadUsers();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }

        protected void GridView1_PageIndexChanging1(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            loadUsers();
        }

        protected void chkPending_CheckedChanged(object sender, EventArgs e)
        {
            loadUsers();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddUser.aspx");
        }
    }
}