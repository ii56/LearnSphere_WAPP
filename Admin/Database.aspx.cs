using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Admin
{
    public partial class Database : System.Web.UI.Page
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
            }
        }
        public void LoadSidebarProfileImage()
        {
            con.Open();
            string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", Session["userid"]);

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

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlDetail.Style["display"] = "none";
        }

        private void Reset()
        {
            gvVerification.Visible = false;
            gvUser.Visible = false;
            gvInvoice.Visible = false;
            gvReceipt.Visible = false;
            gvModule.Visible = false;
            gvLesson.Visible = false;
            gvMaterial.Visible = false;
        }

        protected void btnVerification_Click(object sender, EventArgs e)
        {
            Reset();
            dbTitle.Text = "Verification Request Database";
            ViewState["CurrentTable"] = "Verification";
            gvVerification.Visible = true;
            BindData();
        }

        protected void btnUser_Click(object sender, EventArgs e)
        {
            Reset();
            dbTitle.Text = "User Database";
            ViewState["CurrentTable"] = "User";
            gvUser.Visible = true;
            BindData();
        }

        protected void btnInvoice_Click(object sender, EventArgs e)
        {
            Reset();
            dbTitle.Text = "Invoice Database";
            ViewState["CurrentTable"] = "Invoice";
            gvInvoice.Visible = true;
            BindData();
        }

        protected void btnReceipt_Click(object sender, EventArgs e)
        {
            Reset();
            dbTitle.Text = "Receipt Database";
            ViewState["CurrentTable"] = "Receipt";
            gvReceipt.Visible = true;
            BindData();
        }

        protected void btnModule_Click(object sender, EventArgs e)
        {
            Reset();
            dbTitle.Text = "Module Database";
            ViewState["CurrentTable"] = "Module";
            gvModule.Visible = true;
            BindData();
        }

        protected void btnLesson_Click(object sender, EventArgs e)
        {
            Reset();
            dbTitle.Text = "Lesson Database";
            ViewState["CurrentTable"] = "Lesson";
            gvLesson.Visible = true;
            BindData();
        }

        protected void btnMaterial_Click(object sender, EventArgs e)
        {
            Reset();
            dbTitle.Text = "Material Database";
            ViewState["CurrentTable"] = "Material";
            gvMaterial.Visible = true;
            BindData();
        }

        protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            pnlDetail.Style["display"] = "none";
            ((GridView)sender).PageIndex = e.NewPageIndex;
            BindData();
        }

        private void BindData()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query;
                switch (ViewState["CurrentTable"])
                {
                    case "Verification":
                        query = "SELECT requestid, userid, requestedrole, requesttime, status FROM VerificationRequest";
                        gvVerification.DataSource = new SqlDataAdapter(query, con).FillDataTable();
                        gvVerification.DataBind();
                        break;
                    case "User":
                        query = "SELECT userid, uname, fname, lname, age, gender, usertype, status FROM [User]";
                        gvUser.DataSource = new SqlDataAdapter(query, con).FillDataTable();
                        gvUser.DataBind();
                        break;
                    case "Invoice":
                        query = "SELECT invid, userid, courseid, amount, overdue, duration, creationtime, deadline, settlementtime FROM Invoice";
                        gvInvoice.DataSource = new SqlDataAdapter(query, con).FillDataTable();
                        gvInvoice.DataBind();
                        break;
                    case "Receipt":
                        query = "SELECT recid, invid, amount, creationtime FROM Receipt";
                        gvReceipt.DataSource = new SqlDataAdapter(query, con).FillDataTable();
                        gvReceipt.DataBind();
                        break;
                    case "Module":
                        query = "SELECT moduleid, courseid, modulename, creationtime, ordernumber, deletiontime FROM Module";
                        gvModule.DataSource = new SqlDataAdapter(query, con).FillDataTable();
                        gvModule.DataBind();
                        break;
                    case "Lesson":
                        query = "SELECT lessonid, moduleid, lessontitle, description, creationtime, ordernumber, deletiontime FROM Lesson";
                        gvLesson.DataSource = new SqlDataAdapter(query, con).FillDataTable();
                        gvLesson.DataBind();
                        break;
                    case "Material":
                        query = "SELECT materialid, lessonid, clickcount, filetype, uploadtime FROM Material";
                        gvMaterial.DataSource = new SqlDataAdapter(query, con).FillDataTable();
                        gvMaterial.DataBind();
                        break;
                }
            }
        }

        private void ShowPanel(Panel pnl)
        {
            pnlVerificationDetail.Visible = false;
            pnlUserDetail.Visible = false;
            pnlInvoiceDetail.Visible = false;
            pnlReceiptDetail.Visible = false;
            pnlModuleDetail.Visible = false;
            pnlLessonDetail.Visible = false;
            pnlMaterialDetail.Visible = false;

            pnl.Visible = true;
            pnlDetail.Style["display"] = "flex";
        }

        protected void gvVerification_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewVerification")
            {
                int requestId = Convert.ToInt32(e.CommandArgument);

                hfRequestId.Value = requestId.ToString();

                ShowPanel(pnlVerificationDetail);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "SELECT * FROM VerificationRequest WHERE requestid=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", requestId);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        txtVUserId.Text = dr["userid"].ToString();
                        txtRequestedRole.Text = dr["requestedrole"].ToString();
                        txtVStatus.Text = dr["status"].ToString();
                        txtRemarks.Text = dr["remarks"].ToString();
                    }
                }
            }
        }

        protected void gvUser_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewUser")
            {
                int userId = Convert.ToInt32(e.CommandArgument);

                hfUserId.Value = userId.ToString();

                ShowPanel(pnlUserDetail);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "SELECT * FROM [User] WHERE userid=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", userId);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        txtUname.Text = dr["uname"].ToString();
                        txtFname.Text = dr["fname"].ToString();
                        txtLname.Text = dr["lname"].ToString();
                        txtAge.Text = dr["age"].ToString();
                        txtGender.Text = dr["gender"].ToString();
                        txtUserType.Text = dr["usertype"].ToString();
                        txtStatus.Text = dr["status"].ToString();
                    }
                }
            }
        }

        // --------- Invoice ----------
        protected void gvInvoice_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewInvoice")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                hfInvoiceId.Value = id.ToString();
                ShowPanel(pnlInvoiceDetail);
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Invoice WHERE invid=@id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtIUserId.Text = dr["userid"].ToString();
                        txtICourseId.Text = dr["courseid"].ToString();
                        txtIAmount.Text = dr["amount"].ToString();
                        txtIOverdue.Text = dr["overdue"].ToString();
                        txtIDuration.Text = dr["duration"].ToString();
                        txtICreationTime.Text = dr["creationtime"].ToString();
                        txtIDeadline.Text = dr["deadline"].ToString();
                        txtISettlementTime.Text = dr["settlementtime"].ToString();
                    }
                }
            }
        }

        // --------- Receipt ----------
        protected void gvReceipt_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewReceipt")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                hfReceiptId.Value = id.ToString();
                ShowPanel(pnlReceiptDetail);
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Receipt WHERE recid=@id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtRInvoiceId.Text = dr["invid"].ToString();
                        txtRAmount.Text = dr["amount"].ToString();
                        txtRCreationTime.Text = dr["creationtime"].ToString();
                    }
                }
            }
        }

        // --------- Module ----------
        protected void gvModule_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewModule")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                hfModuleId.Value = id.ToString();
                ShowPanel(pnlModuleDetail);
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Module WHERE moduleid=@id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtMCourseId.Text = dr["courseid"].ToString();
                        txtMName.Text = dr["modulename"].ToString();
                        txtMCreationTime.Text = dr["creationtime"].ToString();
                        txtMOrderNumber.Text = dr["ordernumber"].ToString();
                        txtMDeletionTime.Text = dr["deletiontime"].ToString();
                    }
                }
            }
        }

        // --------- Lesson ----------
        protected void gvLesson_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewLesson")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                hfLessonId.Value = id.ToString();
                ShowPanel(pnlLessonDetail);
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Lesson WHERE lessonid=@id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtLModuleId.Text = dr["moduleid"].ToString();
                        txtLTitle.Text = dr["lessontitle"].ToString();
                        txtLDescription.Text = dr["description"].ToString();
                        txtLCreationTime.Text = dr["creationtime"].ToString();
                        txtLOrderNumber.Text = dr["ordernumber"].ToString();
                        txtLDeletionTime.Text = dr["deletiontime"].ToString();
                    }
                }
            }
        }

        // --------- Material ----------
        protected void gvMaterial_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewMaterial")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                hfMaterialId.Value = id.ToString();
                ShowPanel(pnlMaterialDetail);
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Material WHERE materialid=@id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtMLessonId.Text = dr["lessonid"].ToString();
                        txtMClickCount.Text = dr["clickcount"].ToString();
                        txtMFileType.Text = dr["filetype"].ToString();
                        txtMFileUrl.Text = dr["fileurl"].ToString();
                        txtMVideoUrl.Text = dr["videourl"].ToString();
                        txtMUploadTime.Text = dr["uploadtime"].ToString();
                    }
                }
            }
        }

        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE [User] 
                         SET uname=@uname, fname=@fname, lname=@lname, age=@age, 
                             gender=@gender, usertype=@usertype, status=@status
                         WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfUserId.Value);
                cmd.Parameters.AddWithValue("@uname", txtUname.Text);
                cmd.Parameters.AddWithValue("@fname", txtFname.Text);
                cmd.Parameters.AddWithValue("@lname", txtLname.Text);
                cmd.Parameters.AddWithValue("@age", txtAge.Text);
                cmd.Parameters.AddWithValue("@gender", txtGender.Text);
                cmd.Parameters.AddWithValue("@usertype", txtUserType.Text);
                cmd.Parameters.AddWithValue("@status", txtStatus.Text);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindData();
        }

        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {
            int rowsAffected = 0;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "UPDATE [User] SET status='Deleted' where not status='Deleted' and userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfUserId.Value);

                con.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            pnlDetail.Style["display"] = "none";
            BindData();

            if (rowsAffected > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Deleted User Successfully');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to Deleted or Already Deleted');", true);
            }
        }

        protected void btnUpdateVerification_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE VerificationRequest 
                         SET requestedrole=@role, status=@status, remarks=@remarks
                         WHERE requestid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfRequestId.Value);
                cmd.Parameters.AddWithValue("@role", txtRequestedRole.Text);
                cmd.Parameters.AddWithValue("@status", txtVStatus.Text);
                cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindData();
        }

        protected void btnDeleteVerification_Click(object sender, EventArgs e)
        {
            int rowsAffected = 0;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "DELETE FROM VerificationRequest WHERE requestid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfRequestId.Value);

                con.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            pnlDetail.Style["display"] = "none";
            BindData();

            if (rowsAffected > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Deleted Request Successfully');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to Delete or Already Deleted');", true);
            }
        }

        // -------- Invoice --------
        protected void btnUpdateInvoice_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE Invoice 
                         SET userid=@uid, courseid=@cid, amount=@amt, overdue=@ovd, duration=@dur, creationtime=@ct, deadline=@dl, settlementtime=@st 
                         WHERE invid=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfInvoiceId.Value);
                cmd.Parameters.AddWithValue("@uid", txtIUserId.Text);
                cmd.Parameters.AddWithValue("@cid", txtICourseId.Text);
                cmd.Parameters.AddWithValue("@amt", txtIAmount.Text);
                cmd.Parameters.AddWithValue("@ovd", txtIOverdue.Text);
                cmd.Parameters.AddWithValue("@dur", txtIDuration.Text);
                cmd.Parameters.AddWithValue("@ct", txtICreationTime.Text);
                cmd.Parameters.AddWithValue("@dl", txtIDeadline.Text);
                cmd.Parameters.AddWithValue("@st", txtISettlementTime.Text);
                con.Open(); cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();
        }

        protected void btnDeleteInvoice_Click(object sender, EventArgs e)
        {
            int rowsAffected = 0;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Invoice WHERE invid=@id", con);
                cmd.Parameters.AddWithValue("@id", hfInvoiceId.Value);
                con.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();

            if (rowsAffected > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Deleted Successfully');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to Deleted or Already Deleted');", true);
            }
        }

        // -------- Receipt --------
        protected void btnUpdateReceipt_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE Receipt 
                         SET invid=@invid, amount=@amt, creationtime=@ct 
                         WHERE recid=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfReceiptId.Value);
                cmd.Parameters.AddWithValue("@invid", txtRInvoiceId.Text);
                cmd.Parameters.AddWithValue("@amt", txtRAmount.Text);
                cmd.Parameters.AddWithValue("@ct", txtRCreationTime.Text);
                con.Open(); cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();
        }

        protected void btnDeleteReceipt_Click(object sender, EventArgs e)
        {
            int rowsAffected = 0;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Receipt WHERE recid=@id", con);
                cmd.Parameters.AddWithValue("@id", hfReceiptId.Value);
                con.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();

            if (rowsAffected > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Deleted Successfully');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to Deleted or Already Deleted');", true);
            }
        }

        // -------- Module --------
        protected void btnUpdateModule_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE Module 
                         SET courseid=@cid, modulename=@name, creationtime=@ct, ordernumber=@ord, deletiontime=@del 
                         WHERE moduleid=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfModuleId.Value);
                cmd.Parameters.AddWithValue("@cid", txtMCourseId.Text);
                cmd.Parameters.AddWithValue("@name", txtMName.Text);
                cmd.Parameters.AddWithValue("@ct", txtMCreationTime.Text);
                cmd.Parameters.AddWithValue("@ord", txtMOrderNumber.Text);
                cmd.Parameters.AddWithValue("@del", txtMDeletionTime.Text);
                con.Open(); cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();
        }

        protected void btnDeleteModule_Click(object sender, EventArgs e)
        {
            int rowsAffected = 0;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Module WHERE moduleid=@id", con);
                cmd.Parameters.AddWithValue("@id", hfModuleId.Value);
                con.Open(); 
                rowsAffected = cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();

            if (rowsAffected > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Deleted Successfully');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to Deleted or Already Deleted');", true);
            }
        }

        // -------- Lesson --------
        protected void btnUpdateLesson_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE Lesson 
                         SET moduleid=@mid, lessontitle=@title, description=@desc, creationtime=@ct, ordernumber=@ord, deletiontime=@del 
                         WHERE lessonid=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfLessonId.Value);
                cmd.Parameters.AddWithValue("@mid", txtLModuleId.Text);
                cmd.Parameters.AddWithValue("@title", txtLTitle.Text);
                cmd.Parameters.AddWithValue("@desc", txtLDescription.Text);
                cmd.Parameters.AddWithValue("@ct", txtLCreationTime.Text);
                cmd.Parameters.AddWithValue("@ord", txtLOrderNumber.Text);
                cmd.Parameters.AddWithValue("@del", txtLDeletionTime.Text);
                con.Open(); cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();
        }

        protected void btnDeleteLesson_Click(object sender, EventArgs e)
        {
            int rowsAffected = 0;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Lesson WHERE lessonid=@id", con);
                cmd.Parameters.AddWithValue("@id", hfLessonId.Value);
                con.Open(); 
                rowsAffected = cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();

            if (rowsAffected > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Deleted Successfully');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to Deleted or Already Deleted');", true);
            }
        }

        // -------- Material --------
        protected void btnUpdateMaterial_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE Material 
                         SET lessonid=@lid, clickcount=@click, filetype=@ftype, fileurl=@furl, videourl=@vurl, uploadtime=@ut 
                         WHERE materialid=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", hfMaterialId.Value);
                cmd.Parameters.AddWithValue("@lid", txtMLessonId.Text);
                cmd.Parameters.AddWithValue("@click", txtMClickCount.Text);
                cmd.Parameters.AddWithValue("@ftype", txtMFileType.Text);
                cmd.Parameters.AddWithValue("@furl", txtMFileUrl.Text);
                cmd.Parameters.AddWithValue("@vurl", txtMVideoUrl.Text);
                cmd.Parameters.AddWithValue("@ut", txtMUploadTime.Text);
                con.Open(); cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();
        }

        protected void btnDeleteMaterial_Click(object sender, EventArgs e)
        {
            int rowsAffected = 0;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Material WHERE materialid=@id", con);
                cmd.Parameters.AddWithValue("@id", hfMaterialId.Value);
                con.Open(); 
                rowsAffected = cmd.ExecuteNonQuery();
            }
            pnlDetail.Style["display"] = "none"; BindData();
            if (rowsAffected > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Deleted Successfully');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to Deleted or Already Deleted');", true);
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Session.Abandon();
            Request.Cookies.Clear();
            Response.Redirect("~/Login.aspx");
        }

    }

    public static class SqlHelper
    {
        public static DataTable FillDataTable(this SqlDataAdapter da)
        {
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}