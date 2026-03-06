using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Admin
{
    public partial class Syslog : System.Web.UI.Page
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
                loadTable();
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

        private void loadTable()
        {
            try
            {
                con.Open();
                string query = "Select s.userid, u.usertype, s.action, s.dateTime from Syslog s Inner Join [User] u ON s.userid = u.userid";
                SqlCommand cmd = new SqlCommand(query, con);
                bool search = false;
                if (!string.IsNullOrEmpty(txtSearch1.Text))
                {
                    search = true;
                    cmd.CommandText += " where s.userid Like @search1";
                    cmd.Parameters.AddWithValue("@search1", "%" + txtSearch1.Text + "%");
                    if (!string.IsNullOrEmpty(txtSearch1.Text))
                    {
                        cmd.CommandText += " and s.action Like @search2";
                        cmd.Parameters.AddWithValue("@search2", "%" + txtSearch2.Text + "%");
                    }
                }
                else if (!string.IsNullOrEmpty(txtSearch2.Text))
                {
                    search = true;
                    cmd.CommandText += " where s.action Like @search2";
                    cmd.Parameters.AddWithValue("@search2", "%" + txtSearch2.Text + "%");
                }
                switch (Filter.Text)
                {
                    case "All":
                        break;

                    case "General":
                        if (search) cmd.CommandText += " and u.usertype = 'General'";
                        else cmd.CommandText += " where u.usertype = 'General'";
                        break;

                    case "Student":
                        if (search) cmd.CommandText += " and u.usertype = 'Student'";
                        else cmd.CommandText += " where u.usertype = 'Student'";
                        break;

                    case "Lecturer":
                        if (search) cmd.CommandText += " and u.usertype = 'Lecturer'";
                        else cmd.CommandText += " where u.usertype = 'Lecturer'";
                        break;

                    case "Admin":
                        if (search) cmd.CommandText += " and u.usertype = 'Admin'";
                        else cmd.CommandText += " where u.usertype = 'Admin'";
                        break;

                    default:
                        break;
                }
                switch (Order.Text)
                {
                    case "Datetime Ascending":
                        cmd.CommandText += " order by dateTime asc";
                        break;

                    case "Datetime Descending":
                        cmd.CommandText += " order by dateTime desc";
                        break;

                    default:
                        cmd.CommandText += " order by dateTime desc";
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

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            loadTable();
        }

        protected void Order_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadTable();
        }

        protected void Sortby_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadTable();
        }

        protected void txtSearch1_TextChanged(object sender, EventArgs e)
        {
            loadTable();
        }

        protected void txtSearch2_TextChanged(object sender, EventArgs e)
        {
            loadTable();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }
    }
}