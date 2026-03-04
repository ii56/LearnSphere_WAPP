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
            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (Session["CurrentCourseID"] == null)
            {
                Response.Redirect("CreateCourse.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadCourseTitle();
                LoadSidebarProfileImage();
                LoadModules();
            }
            if (!IsPostBack)
            {
                ViewState["Step"] = "2";
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
                    lblCourseTitle.Text = result.ToString();
                }
            }
        }


        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            try
            {
                int courseId = Convert.ToInt32(Session["CurrentCourseID"]);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

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
                    cmd.Parameters.AddWithValue("@name", txtModuleName.Text);
                    cmd.Parameters.AddWithValue("@desc", txtModuleDesc.Text);

                    cmd.ExecuteNonQuery();
                }

                txtModuleName.Text = "";
                txtModuleDesc.Text = "";

                LoadModules();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
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