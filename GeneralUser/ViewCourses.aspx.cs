using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class ViewCourses : System.Web.UI.Page
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadUnreadMessagesBadge();
                LoadAllPublishedCourses();
            }
        }

        private void LoadSidebarProfileImage()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        con.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                            imgSidebarProfile.Src = ResolveUrl(result.ToString());
                        else
                            imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading profile image: " + ex.Message);
                imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
            }
        }

        private void LoadUnreadMessagesBadge()
        {
            if (Session["unreadCount"] != null && int.TryParse(Session["unreadCount"].ToString(), out int unreadCount) && unreadCount > 0)
            {
                litUnreadBadge.Text = $"<span class='message-badge'>{unreadCount}</span>";
            }
        }

        private void LoadAllPublishedCourses()
        {
            int currentUserId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT c.courseid, c.coursename, c.category, c.price, 
                               u.fname + ' ' + u.lname AS lecturerName,
                               CAST(CASE WHEN inv.userid IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS IsEnrolled
                        FROM Course c
                        INNER JOIN [User] u ON c.ownerid = u.userid
                        LEFT JOIN Invoice inv ON c.courseid = inv.courseid AND inv.userid = @currentUserId
                        WHERE c.status = 1 
                        AND c.deletiontime IS NULL";

                    List<SqlParameter> parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@currentUserId", currentUserId)
                    };

                    if (!string.IsNullOrEmpty(txtSearch.Text))
                    {
                        query += " AND c.coursename LIKE @search";
                        parameters.Add(new SqlParameter("@search", "%" + txtSearch.Text.Trim() + "%"));
                    }

                    if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                    {
                        query += " AND c.category = @category";
                        parameters.Add(new SqlParameter("@category", ddlCategory.SelectedValue));
                    }

                    if (!string.IsNullOrEmpty(txtMaxPrice.Text))
                    {
                        query += " AND c.price <= @maxPrice";
                        parameters.Add(new SqlParameter("@maxPrice", txtMaxPrice.Text));
                    }

                    query += " ORDER BY c.creationtime DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            gvCourses.DataSource = dt;
                            gvCourses.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Database Error in LoadAllPublishedCourses: " + ex.Message);
                lblMessage.Text = "An error occurred while loading the courses.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        // --- SIMPLIFIED ROW COMMAND ---
        // Clicking ANY button on the grid row will now safely redirect to the Details Page
        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                if (int.TryParse(e.CommandArgument.ToString(), out int courseId))
                {
                    Response.Redirect($"CourseDetails.aspx?courseid={courseId}");
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadAllPublishedCourses();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlCategory.SelectedIndex = 0;
            txtMaxPrice.Text = "";
            LoadAllPublishedCourses();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}