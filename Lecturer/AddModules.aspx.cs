using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class AddModules : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure user is a lecturer
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Ensure course creation flow is valid
            if (Session["CurrentCourseID"] == null)
            {
                Response.Redirect("CreateCourse.aspx");
                return;
            }

            if (Session["CurrentCourseID"] == null)
            {
                Response.Redirect("ViewCourses.aspx"); // better UX
                return;
            }

            if (!IsPostBack)
            {
                LoadCourseTitle();
                LoadSidebarProfileImage();
                LoadModules();
                ViewState["Step"] = "2";
            }
        }



        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
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



        private void LoadCourseTitle()
        {
            int courseId = Convert.ToInt32(Session["CurrentCourseID"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT coursename FROM Course WHERE courseid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", courseId);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    lblCourseTitle.Text = Server.HtmlEncode(result.ToString());
                }
            }
        }



        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                int courseId = Convert.ToInt32(Session["CurrentCourseID"]);

                string moduleName = txtModuleName.Text.Trim();
                string moduleDesc = txtModuleDesc.Text.Trim();

                // -------- INPUT VALIDATION --------

                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    lblMessage.Text = "Module name is required.";
                    return;
                }

                if (moduleName.Length > 100)
                {
                    lblMessage.Text = "Module name cannot exceed 100 characters.";
                    return;
                }

                if (moduleDesc.Length > 1000)
                {
                    lblMessage.Text = "Description cannot exceed 1000 characters.";
                    return;
                }

                // Prevent XSS
                moduleName = Server.HtmlEncode(moduleName);
                moduleDesc = Server.HtmlEncode(moduleDesc);


                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // -------- DUPLICATE MODULE CHECK --------

                    string checkQuery = @"SELECT COUNT(*) 
                                          FROM Module 
                                          WHERE modulename = @name 
                                          AND courseid = @courseid
                                          AND deletiontime IS NULL";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);

                    checkCmd.Parameters.AddWithValue("@name", moduleName);
                    checkCmd.Parameters.AddWithValue("@courseid", courseId);

                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        lblMessage.Text = "A module with this name already exists.";
                        return;
                    }


                    // -------- INSERT MODULE --------

                    string query = @"
                        INSERT INTO Module
                        (courseid, modulename, moduledescription, ordernumber, creationtime, deletiontime)
                        VALUES
                        (
                            @courseid,
                            @name,
                            @desc,
                            (
                                SELECT ISNULL(MAX(ordernumber),0) + 1
                                FROM Module
                                WHERE courseid = @courseid
                                AND deletiontime IS NULL
                            ),
                            GETDATE(),
                            NULL
                        )";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@courseid", courseId);
                    cmd.Parameters.AddWithValue("@name", moduleName);
                    cmd.Parameters.AddWithValue("@desc", moduleDesc);

                    cmd.ExecuteNonQuery();
                }

                txtModuleName.Text = "";
                txtModuleDesc.Text = "";

                lblMessage.Text = "Module added successfully.";

                LoadModules();
            }
            catch
            {
                lblMessage.Text = "An error occurred while adding the module.";
            }
        }



        private void LoadModules()
        {
            int courseId = Convert.ToInt32(Session["CurrentCourseID"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT moduleid, modulename, moduledescription
                                 FROM Module
                                 WHERE courseid = @courseid
                                 AND deletiontime IS NULL
                                 ORDER BY ordernumber";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@courseid", courseId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvModules.DataSource = dt;
                gvModules.DataBind();
            }
        }



        protected void gvModules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddLessons")
            {
                int moduleId = Convert.ToInt32(e.CommandArgument);

                // Verify module belongs to this course (security check)
                int courseId = Convert.ToInt32(Session["CurrentCourseID"]);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"SELECT COUNT(*) 
                                     FROM Module
                                     WHERE moduleid = @moduleid
                                     AND courseid = @courseid";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@moduleid", moduleId);
                    cmd.Parameters.AddWithValue("@courseid", courseId);

                    con.Open();

                    int exists = (int)cmd.ExecuteScalar();

                    if (exists == 0)
                    {
                        lblMessage.Text = "Invalid module selection.";
                        return;
                    }
                }

                Session["CurrentModuleID"] = moduleId;

                Response.Redirect("AddLessons.aspx");
            }
        }



        protected void btnContinue_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddLessons.aspx");
        }



        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }



        protected void btnBackToCourse_Click(object sender, EventArgs e)
        {
            Response.Redirect("CreateCourse.aspx");
        }
    }
}