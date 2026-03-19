using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class editModule : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        int moduleId;
        int courseId;
        int userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // AUTHENTICATION
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);

            // VALIDATE QUERY STRING
            if (!int.TryParse(Request.QueryString["courseid"], out courseId) || courseId <= 0)
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            int.TryParse(Request.QueryString["moduleid"], out moduleId);

            // AUTHORIZATION (CRITICAL)
            if (!IsCourseOwner(courseId, userId))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (moduleId > 0 && !IsModuleValid(moduleId))
            {
                Response.Redirect("ViewCourses.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadCourseTitle();

                if (moduleId > 0)
                {
                    lblModeTitle.Text = "Edit Module";
                    btnSave.Text = "Update and Continue";
                    LoadModule();
                }
                else
                {
                    lblModeTitle.Text = "Add Module";
                    btnSave.Text = "Confirm Addition";
                }
            }
        }

        private bool IsCourseOwner(int courseId, int userId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Course WHERE courseid=@cid AND ownerid=@uid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool IsModuleValid(int moduleId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Module WHERE moduleid=@mid AND courseid=@cid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@mid", SqlDbType.Int).Value = moduleId;
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                con.Open();
                object result = cmd.ExecuteScalar();

                imgSidebarProfile.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        private void LoadCourseTitle()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT coursename FROM Course WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = courseId;

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    lblCourseTitle.Text = Server.HtmlEncode(result.ToString());
                }
            }
        }

        private void LoadModule()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT modulename, moduledescription, ordernumber
                                 FROM Module
                                 WHERE moduleid=@id AND deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtModuleName.Text = Server.HtmlEncode(reader["modulename"].ToString());
                    txtModuleDesc.Text = Server.HtmlEncode(reader["moduledescription"].ToString());
                    txtOrderNumber.Text = reader["ordernumber"].ToString();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtModuleName.Text.Trim();
                string desc = txtModuleDesc.Text.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    lblMessage.Text = "Module name is required.";
                    return;
                }

                int order = 1;
                if (!string.IsNullOrWhiteSpace(txtOrderNumber.Text))
                {
                    if (!int.TryParse(txtOrderNumber.Text, out order) || order < 1 || order > 100)
                    {
                        lblMessage.Text = "Invalid order number.";
                        return;
                    }
                }

                name = Server.HtmlEncode(name);
                desc = Server.HtmlEncode(desc);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    if (moduleId > 0)
                    {
                        string updateQuery = @"UPDATE Module
                                               SET modulename=@name,
                                                   moduledescription=@desc,
                                                   ordernumber=@order
                                               WHERE moduleid=@id";

                        SqlCommand cmd = new SqlCommand(updateQuery, con);

                        cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;
                        cmd.Parameters.Add("@order", SqlDbType.Int).Value = order;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = moduleId;

                        cmd.ExecuteNonQuery();
                        LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Updated Module (ModuleID: " + moduleId + ")");
                    }
                    else
                    {
                        string insertQuery = @"INSERT INTO Module
                            (courseid, modulename, moduledescription, ordernumber, creationtime)
                            VALUES
                            (@courseid, @name, @desc,
                            (SELECT ISNULL(MAX(ordernumber),0)+1 FROM Module WHERE courseid=@courseid AND deletiontime IS NULL),
                            GETDATE())";

                        SqlCommand cmd = new SqlCommand(insertQuery, con);

                        cmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;
                        cmd.Parameters.Add("@desc", SqlDbType.NVarChar).Value = desc;

                        cmd.ExecuteNonQuery();
                    }
                }

                Response.Redirect("editCourse.aspx?courseid=" + courseId);
            }
            catch
            {
                lblMessage.Text = "An error occurred. Please try again.";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}