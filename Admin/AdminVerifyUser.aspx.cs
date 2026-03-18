using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminVerifyUser : System.Web.UI.Page
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
        int requestId;

        protected void Page_Load(object sender, EventArgs e)
        {
            requestId = Request.QueryString["requestid"] != null ? Convert.ToInt32(Request.QueryString["requestid"]) : 0;

            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome " + Session["uname"];
                LoadVerificationRequest();
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
                sidebarImg.Src = ResolveUrl(result.ToString());
            }
            else
            {
                sidebarImg.Src = ResolveUrl("~/images/default-user.png");
            }
            con.Close();
        }

        private void LoadVerificationRequest()
        {
            string query = @"SELECT userid, currentrole, requestedrole, documentpath, status, requesttime, remarks 
                             FROM VerificationRequest 
                             WHERE requestid = @requestid";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@requestid", requestId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                lblRequestId.Text = requestId.ToString();
                lblUserId.Text = dr["userid"].ToString();
                lblCurrentRole.Text = dr["currentrole"].ToString();
                lblRequestedRole.Text = dr["requestedrole"].ToString();
                lblRequestTime.Text = Convert.ToDateTime(dr["requesttime"]).ToString("yyyy-MM-dd HH:mm");
                lblStatus.Text = dr["status"].ToString();
                lblRemarks.Text = dr["remarks"].ToString();

                string docPath = dr["documentpath"].ToString();
                if (File.Exists(Server.MapPath(docPath)))
                {
                    docFrame.Src = ResolveUrl(docPath);
                }
            }
            dr.Close();
            con.Close();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            UpdateStatus("Approved");
        }

        protected void btnDecline_Click(object sender, EventArgs e)
        {
            UpdateStatus("Declined");
        }

        private void UpdateStatus(string newStatus)
        {
            string query = @"UPDATE VerificationRequest 
                             SET status=@status, reviewedtime=GETDATE(), reviewedby=@adminId 
                             WHERE requestid=@requestid";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@status", newStatus);
            cmd.Parameters.AddWithValue("@adminId", Session["userid"]);
            cmd.Parameters.AddWithValue("@requestid", requestId);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            LoadVerificationRequest();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("../Login.aspx");
        }
    }
}