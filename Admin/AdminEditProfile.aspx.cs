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
using iTextSharp.xmp.impl.xpath;

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
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
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

            txtUserid.Text = Session["userid"].ToString();
            txtUsername.Text = dt.Rows[0]["uname"].ToString();
            txtFname.Text = dt.Rows[0]["fname"].ToString();
            txtLname.Text = dt.Rows[0]["lname"].ToString();
            txtEmail.Text = dt.Rows[0]["email"].ToString();
            txtAge.Text = dt.Rows[0]["age"].ToString();
            dropdownGender.SelectedValue = dt.Rows[0]["gender"].ToString().Trim();
            txtDescription.Text = dt.Rows[0]["description"].ToString();

            if (dt.Rows[0]["ProfileImage"] != DBNull.Value)
            {
                profilePic.Src = ResolveUrl(dt.Rows[0]["ProfileImage"].ToString());
                lblUploadMessage.Text = dt.Rows[0]["ProfileImage"].ToString();
            }
            else
            {
                profilePic.Src = ResolveUrl("~/images/default-user.png");
                lblUploadMessage.Text = "~/images/default-user.png";
            }

            con.Close();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            con.Open();
            string query = "update [User] set uname = @uname, email = @email, fname = @fname, lname = @lname, age = @age, gender = @gender, description = @description where userid = @userid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@uname", txtUsername.Text);
            cmd.Parameters.AddWithValue("@email", txtEmail.Text);
            cmd.Parameters.AddWithValue("@fname", txtFname.Text);
            cmd.Parameters.AddWithValue("@lname", txtLname.Text);
            cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtAge.Text));
            cmd.Parameters.AddWithValue("@gender", dropdownGender.Text);
            cmd.Parameters.AddWithValue("@description", txtDescription.Text);
            cmd.Parameters.AddWithValue("@userid", Session["userid"]);
            cmd.ExecuteNonQuery();

            con.Close();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Update Profile");
            Response.Write("<script>alert('Profile Updated'); window.history.back();</script>");
        }

        protected void btnSave_Click1(object sender, EventArgs e)
        {
            string folderPath = Server.MapPath("~/ProfilePic/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string ImgPath = "";
            if (this.fuProfileImage.HasFile)
            {
                ImgPath = "~/ProfilePic/" + this.fuProfileImage.FileName.ToString();
                fuProfileImage.SaveAs(folderPath + Path.GetFileName(fuProfileImage.FileName));
            }
            else
            {
                ImgPath = lblUploadMessage.Text;
            }

            con.Open();
            string query = "Update [User] set ProfileImage = @profileimage where userid=@userid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@profileimage", ImgPath);
            cmd.Parameters.AddWithValue("@userid", Session["userid"]);
            cmd.ExecuteNonQuery();

            con.Close();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Update Profile Image");
            Response.Write("<script>alert('Profile Image Updated'); window.history.back();</script>");
        }

        protected void btnSave_Click2(object sender, EventArgs e)
        {
            con.Open();
            string query = "Update [User] set pwd=@pwd where userid=@userid";
            SqlCommand cmd = new SqlCommand(query, con); 
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text.Trim());
            cmd.Parameters.AddWithValue("@pwd", hashedPassword);
            cmd.Parameters.AddWithValue("@userid", Session["userid"]);
            cmd.ExecuteNonQuery();

            con.Close();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Update Password");
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Write("<script>alert('Password Updated'); window.location='~/Login.aspx';</script>");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("~/Login.aspx");
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminChangePassword.aspx");
        }
    }
}