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
using System.IO;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminEditProfile : System.Web.UI.Page
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
        string imgText;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome " + Session["uname"];
                loadProfile();
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

        private void loadProfile()
        {
            con.Open();

            string query = "Select uname, fname, lname, email, age, gender, ProfileImage, description from [User] where userid = @userid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@userid", Session["userid"]);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            lblUname.Text = dt.Rows[0]["uname"].ToString();
            lblFname.Text = dt.Rows[0]["fname"].ToString();
            lblLname.Text = dt.Rows[0]["lname"].ToString();
            lblEmail.Text = dt.Rows[0]["email"].ToString();
            lblAge.Text = dt.Rows[0]["age"].ToString();
            dropdownGender.Text = dt.Rows[0]["gender"].ToString();
            Image1.ImageUrl = dt.Rows[0]["ProfileImage"].ToString();
            txtDescription.Text = dt.Rows[0]["description"].ToString();
            imgText = dt.Rows[0]["ProfileImage"].ToString();

            con.Close();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string folderPath = Server.MapPath("../ProfilePic/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string ImgPath = "";
            if (this.FileUpload1.HasFile)
            {
                ImgPath = "../ProfilePic/" + this.FileUpload1.FileName.ToString();
                FileUpload1.SaveAs(folderPath + Path.GetFileName(FileUpload1.FileName));
            }
            else
            {
                ImgPath = imgText;
            }

            con.Open();
            string query = "update [User] set uname = @uname, email = @email, fname = @fname, lname = @lname, age = @age, gender = @gender, ProfileImage = @profileimage, description = @description where userid = @userid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@uname", lblUname.Text);
            cmd.Parameters.AddWithValue("@email", lblEmail.Text);
            cmd.Parameters.AddWithValue("@fname", lblFname.Text);
            cmd.Parameters.AddWithValue("@lname", lblLname.Text);
            cmd.Parameters.AddWithValue("@age", Convert.ToInt32(lblAge.Text));
            cmd.Parameters.AddWithValue("@gender", dropdownGender.Text);
            cmd.Parameters.AddWithValue("@profileimage", ImgPath);
            cmd.Parameters.AddWithValue("@description", txtDescription.Text);
            cmd.Parameters.AddWithValue("@userid", Session["userid"]);
            cmd.ExecuteNonQuery();

            con.Close();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Update Profile");
            Response.Write("<script>alert('Profile Updated'); window.history.back();</script>");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminChangePassword.aspx");
        }
    }
}