using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class CreateCourse : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                ViewState["Step"] = "1";
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", userId);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string imagePath = result.ToString();
                    Session["profileImage"] = imagePath;
                    imgSidebarProfile.Src = ResolveUrl(imagePath);
                }
                else
                {
                    imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                }
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                int ownerId = Convert.ToInt32(Session["userid"]);

                decimal price = 0;
                if (!string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    decimal.TryParse(txtPrice.Text, out price);
                }

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query = @"INSERT INTO Course
                                    (ownerid, coursename, description, price, creationtime, deletiontime, category, status)
                                    OUTPUT INSERTED.courseid
                                    VALUES
                                    (@ownerid, @name, @desc, @price, GETDATE(), NULL, @category, 0)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@ownerid", ownerId);
                    cmd.Parameters.AddWithValue("@name", txtCourseName.Text);
                    cmd.Parameters.AddWithValue("@desc", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@category", ddlCategory.SelectedValue);

                    int newCourseId = (int)cmd.ExecuteScalar();
                    Session["CurrentCourseID"] = newCourseId;

                    Response.Redirect("AddModules.aspx");
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = ex.Message;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}