using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Org.BouncyCastle.Bcpg;

namespace LearnSphere_WAPP.Admin
{
    public partial class EditUsers : System.Web.UI.Page
    {
        int userid;
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            userid = Request.QueryString["userid"] != null ? Convert.ToInt32(Request.QueryString["userid"]) : 0;
            if (!IsPostBack)
            {
                if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
                {
                    Response.Redirect("../Login.aspx");
                }
                if (userid != 0)
                {
                    loadUserDetails();
                    LoadSidebarProfileImage();
                }
                else
                {
                    Response.Redirect("UserManagement.aspx");
                }
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

        private void loadUserDetails()
        {
            con.Open();

            string query = "Select uname, fname, lname, email, age, gender, usertype from [User] where userid = @userid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@userid", userid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            txtUserid.Text = userid.ToString();
            txtUname.Text = dt.Rows[0]["uname"].ToString();
            txtFname.Text = dt.Rows[0]["fname"].ToString();
            txtLname.Text = dt.Rows[0]["lname"].ToString();
            txtEmail.Text = dt.Rows[0]["email"].ToString();
            txtAge.Text = dt.Rows[0]["age"].ToString();
            dropdownGender.Text = dt.Rows[0]["gender"].ToString();
            txtUsertype.Text = dt.Rows[0]["usertype"].ToString();

            con.Close();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            con.Open();

            string query = "Update [User] Set uname = @uname, fname = @fname, lname = @lname, email = @email, age = @age, gender = @gender where userid = @userid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@uname", txtUname.Text);
            cmd.Parameters.AddWithValue("@fname", txtFname.Text);
            cmd.Parameters.AddWithValue("@lname", txtLname.Text);
            cmd.Parameters.AddWithValue("@email", txtEmail.Text);
            cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtAge.Text));
            cmd.Parameters.AddWithValue("@gender", dropdownGender.Text);
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.ExecuteNonQuery();

            Response.Write("<script>alert('User updated successfully'); window.location='UserManagement.aspx';</script>");

            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Edit User Profile (UserID: " + userid + ")");
            con.Close();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }
    }
}