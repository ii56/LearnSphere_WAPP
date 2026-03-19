using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Admin
{
    public partial class Database : System.Web.UI.Page
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome " + Session["uname"];
                LoadSidebarProfileImage();
                ViewState["CurrentTable"] = "User";
                BindData();
            }
        }
        public void LoadSidebarProfileImage()
        {
            con.Open();
            string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", Session["userid"]);

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

        protected void btnUser_Click(object sender, EventArgs e)
        {
            ViewState["CurrentTable"] = "User";
            GridView1.PageIndex = 0;
            BindData();
        }

        protected void btnCourse_Click(object sender, EventArgs e)
        {
            ViewState["CurrentTable"] = "Course";
            GridView1.PageIndex = 0;
            BindData();
        }

        protected void btnModule_Click(object sender, EventArgs e)
        {
            ViewState["CurrentTable"] = "Module";
            GridView1.PageIndex = 0;
            BindData();
        }

        private void BindData()
        {
            string tableName = ViewState["CurrentTable"].ToString();
            string query = "";

            switch (tableName)
            {
                case "User":
                    query = "SELECT userid, uname, email, usertype, status, creationtime FROM [User]";
                    break;
                case "Course":
                    query = "SELECT courseid, coursename, price, category, status, creationtime FROM Course";
                    break;
                case "Module":
                    query = "SELECT moduleid, courseid, modulename, ordernumber, creationtime FROM Module";
                    break;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void GridView1_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindData();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Request.Cookies.Clear();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("../Login.aspx");
        }
    }
}