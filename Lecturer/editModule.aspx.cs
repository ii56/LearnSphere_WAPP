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
    public partial class editModule : System.Web.UI.Page
    {
        string connStr = ConfigurationManager
            .ConnectionStrings["LearnSphereDB"].ConnectionString;

        int moduleId;
        int courseId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
            }

            int.TryParse(Request.QueryString["moduleid"], out moduleId);

            int.TryParse(Request.QueryString["moduleid"], out moduleId);

            if (!IsPostBack)
            {
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

        private void LoadCourseTitle()
        {
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

        private void LoadModule()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT modulename, moduledescription, ordernumber
                    FROM Module
                    WHERE moduleid = @id
                    AND deletiontime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", moduleId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtModuleName.Text = reader["modulename"].ToString();
                    txtModuleDesc.Text = reader["moduledescription"].ToString();
                    txtOrderNumber.Text = reader["ordernumber"].ToString();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                if (moduleId > 0)
                {
                    string query = @"
                        UPDATE Module
                        SET modulename = @name,
                            moduledescription = @desc,
                            ordernumber = @order
                        WHERE moduleid = @id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtModuleName.Text);
                    cmd.Parameters.AddWithValue("@desc", txtModuleDesc.Text);
                    cmd.Parameters.AddWithValue("@order",
                        string.IsNullOrEmpty(txtOrderNumber.Text)
                        ? 1
                        : Convert.ToInt32(txtOrderNumber.Text));
                    cmd.Parameters.AddWithValue("@id", moduleId);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string query = @"
                        INSERT INTO Module
                        (courseid, modulename, moduledescription, ordernumber, creationtime)
                        VALUES
                        (@courseid, @name, @desc, @order, GETDATE())";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@courseid", courseId);
                    cmd.Parameters.AddWithValue("@name", txtModuleName.Text);
                    cmd.Parameters.AddWithValue("@desc", txtModuleDesc.Text);
                    cmd.Parameters.AddWithValue("@order",
                        string.IsNullOrEmpty(txtOrderNumber.Text)
                        ? 1
                        : Convert.ToInt32(txtOrderNumber.Text));
                    cmd.ExecuteNonQuery();
                }
            }

            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("editCourse.aspx?courseid=" + courseId);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}