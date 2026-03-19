using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class CreateCourse : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // 🔐 CSRF PROTECTION
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 AUTH CHECK
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
            }
        }

        private void LoadDraftData(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT coursename, description, price, category 
                         FROM Course 
                         WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtCourseName.Text = reader["coursename"].ToString();
                    txtDescription.Text = reader["description"].ToString();
                    txtPrice.Text = reader["price"].ToString();
                    ddlCategory.SelectedValue = reader["category"].ToString();
                }
            }
        }

        private int CurrentUserID
        {
            get
            {
                return Convert.ToInt32(Session["userid"]);
            }
        }

        // 🔐 PROFILE IMAGE SAFE LOAD
        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = CurrentUserID;

                con.Open();
                object result = cmd.ExecuteScalar();

                string imagePath = "~/images/default-user.png";

                if (result != null && result != DBNull.Value)
                {
                    string path = result.ToString();
                    if (path.StartsWith("~/images/"))
                        imagePath = path;
                }

                imgSidebarProfile.Src = ResolveUrl(imagePath);
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                string courseName = txtCourseName.Text.Trim();
                string description = txtDescription.Text.Trim();
                string category = ddlCategory.SelectedValue;
                decimal price;

                decimal.TryParse(txtPrice.Text, out price);

                // 🚫 PREVENT EMPTY DRAFTS
                if (string.IsNullOrWhiteSpace(courseName) &&
                    string.IsNullOrWhiteSpace(description) &&
                    string.IsNullOrWhiteSpace(category) &&
                    price == 0)
                {
                    lblMessage.Text = "Please enter course details before proceeding.";
                    return;
                }

                int courseId;

                // ✅ CREATE OR USE EXISTING DRAFT
                if (Session["CurrentCourseID"] == null)
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();

                        string insertQuery = @"
                INSERT INTO Course
                (ownerid, coursename, description, price, creationtime, deletiontime, category, status)
                OUTPUT INSERTED.courseid
                VALUES
                (@ownerid, @name, @desc, @price, GETDATE(), NULL, @category, 'Unactive')";

                        SqlCommand cmd = new SqlCommand(insertQuery, con);

                        cmd.Parameters.AddWithValue("@ownerid", CurrentUserID);
                        cmd.Parameters.AddWithValue("@name", courseName);
                        cmd.Parameters.AddWithValue("@desc", description);

                        var priceParam = cmd.Parameters.Add("@price", SqlDbType.Decimal);
                        priceParam.Precision = 18;
                        priceParam.Scale = 2;
                        priceParam.Value = price;

                        cmd.Parameters.AddWithValue("@category", category);

                        courseId = (int)cmd.ExecuteScalar();

                        Session["CurrentCourseID"] = courseId;
                    }
                }
                else
                {
                    courseId = Convert.ToInt32(Session["CurrentCourseID"]);

                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();

                        string updateQuery = @"
                UPDATE Course
                SET coursename = @name,
                    description = @desc,
                    price = @price,
                    category = @category
                WHERE courseid = @id";

                        SqlCommand cmd = new SqlCommand(updateQuery, con);

                        cmd.Parameters.AddWithValue("@id", courseId);
                        cmd.Parameters.AddWithValue("@name", courseName);
                        cmd.Parameters.AddWithValue("@desc", description);

                        var priceParam = cmd.Parameters.Add("@price", SqlDbType.Decimal);
                        priceParam.Precision = 18;
                        priceParam.Scale = 2;
                        priceParam.Value = price;

                        cmd.Parameters.AddWithValue("@category", category);

                        cmd.ExecuteNonQuery();
                        LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Created Course (CourseName: " + courseName + ")");
                    }
                }

                Response.Redirect("AddModules.aspx");
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        // 🔐 VERIFY LECTURER EXISTS
        private bool IsValidLecturer(int userId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = "SELECT COUNT(*) FROM [User] WHERE userid=@id AND usertype='Lecturer'";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Redirect("~/Login.aspx");
        }
    }
}