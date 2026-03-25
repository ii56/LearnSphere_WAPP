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
        int userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Request.QueryString["userid"] != null ? Convert.ToInt32(Request.QueryString["userid"]) : 0;

            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
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
            string query = @"SELECT requestid, userid, currentrole, requestedrole, documentpath, status, requesttime, reviewedtime, reviewedby, remarks 
                             FROM VerificationRequest 
                             WHERE userid = @userid";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@userid", userId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                lblRequestId.Text = dr["requestid"].ToString();
                lblUserId.Text = userId.ToString();
                lblCurrentRole.Text = dr["currentrole"].ToString();
                lblRequestedRole.Text = dr["requestedrole"].ToString();
                lblRequestTime.Text = Convert.ToDateTime(dr["requesttime"]).ToString("yyyy-MM-dd HH:mm");
                lblStatus.Text = dr["status"].ToString();
                if (lblStatus.Text != "Pending")
                {
                    lblReviewTime.Text = Convert.ToDateTime(dr["reviewedtime"]).ToString("yyyy-MM-dd HH:mm");
                    lblReviewBy.Text = dr["reviewedby"].ToString();
                }
                else
                {
                    lblReviewTime.Text = "None";
                    lblReviewBy.Text = "None";
                }

                txtRemarks.Text = dr["remarks"].ToString();

                string docPath = dr["documentpath"].ToString();
                string fullPath = Server.MapPath(docPath);
                if (File.Exists(fullPath))
                {
                    docFrame.Src = ResolveUrl(docPath);
                }
                else
                {
                    docFrame.Src = "";
                }
            }
            dr.Close();
            con.Close();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            UpdateStatus("Approved");
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Approved request for user (UserID: " + userId + ")");
        }

        protected void btnDecline_Click(object sender, EventArgs e)
        {
            UpdateStatus("Rejected");
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Declined request for user (UserID: " + userId + ")");
        }

        private void UpdateStatus(string newStatus)
        {
            string query = @"UPDATE VerificationRequest 
                             SET status=@status, reviewedtime=GETDATE(), reviewedby=@adminId, remarks=@remarks 
                             WHERE userid=@userid";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@status", newStatus);
            cmd.Parameters.AddWithValue("@adminId", Session["userid"]);
            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);
            cmd.Parameters.AddWithValue("@userid", userId);

            con.Open();
            if (newStatus == "Approved")
            {
                string query2 = @"UPDATE [User] SET usertype = 'Lecturer' WHERE userid = @userid";
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.Parameters.AddWithValue("@userid", userId);
                cmd2.ExecuteNonQuery();

            }
            
            cmd.ExecuteNonQuery();
            con.Close();

            LoadVerificationRequest();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("~/Login.aspx");
        }
    }
}