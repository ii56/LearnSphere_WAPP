using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class Messaging : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // keeps track of which lecturer is selected
        public int SelectedLecturerId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string displayName = GetDisplayName();
            lblHeaderName.Text = displayName;
            lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

            // check if a lecturer was selected from the url
            if (Request.QueryString["lecturerId"] != null)
                SelectedLecturerId = Convert.ToInt32(Request.QueryString["lecturerId"]);

            if (!IsPostBack)
            {
                LoadLecturers();

                if (SelectedLecturerId > 0)
                    LoadChat(SelectedLecturerId);
            }
        }

        private string GetDisplayName()
        {
            if (Session["fname"] != null && Session["fname"].ToString() != "")
                return Session["fname"].ToString();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT fname FROM [User] WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", Convert.ToInt32(Session["userid"]));
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        Session["fname"] = result.ToString();
                        return result.ToString();
                    }
                }
            }
            return "Student";
        }

        private void LoadLecturers()
        {
            try
            {
                int userId = Convert.ToInt32(Session["userid"]);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // get lecturers of courses the student is enrolled in
                    string query = @"SELECT DISTINCT u.userid, u.fname, u.lname, c.coursename
                                     FROM [User] u
                                     INNER JOIN Course c ON c.ownerid = u.userid
                                     INNER JOIN Enrollment e ON e.courseid = c.courseid
                                     WHERE e.userid = @uid AND e.isactive = 1
                                     ORDER BY u.fname";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptLecturers.DataSource = dt;
                            rptLecturers.DataBind();
                            pnlNoLecturers.Visible = false;
                        }
                        else
                        {
                            rptLecturers.DataSource = null;
                            rptLecturers.DataBind();
                            pnlNoLecturers.Visible = true;
                        }
                    }
                }
            }
            catch
            {
                lblError.Text = "Could not load lecturers.";
                lblError.Visible = true;
            }
        }

        private void LoadChat(int lecturerId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["userid"]);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // get lecturer info to show in chat header
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT u.fname, u.lname, c.coursename FROM [User] u INNER JOIN Course c ON c.ownerid = u.userid INNER JOIN Enrollment e ON e.courseid = c.courseid WHERE u.userid = @lid AND e.userid = @uid AND e.isactive = 1", con))
                    {
                        cmd.Parameters.AddWithValue("@lid", lecturerId);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                string fname = r["fname"].ToString();
                                string lname = r["lname"].ToString();
                                lblChatName.Text = fname + " " + lname;
                                lblChatInitial.Text = fname.Substring(0, 1).ToUpper();
                                lblChatCourse.Text = r["coursename"].ToString();
                            }
                        }
                    }

                    // find or create conversation between student and lecturer
                    int convId = GetOrCreateConversation(con, userId, lecturerId);

                    // load all messages in this conversation
                    string msgQuery = @"SELECT messageid, senderid, message, creationtime
                                        FROM ChatMessage
                                        WHERE conversationid = @cid
                                        ORDER BY creationtime ASC";

                    using (SqlDataAdapter da = new SqlDataAdapter(msgQuery, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@cid", convId);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        rptMessages.DataSource = dt;
                        rptMessages.DataBind();
                    }

                    // save convId to viewstate so send button can use it
                    ViewState["convId"] = convId;
                    ViewState["lecturerId"] = lecturerId;
                }

                pnlChatPlaceholder.Visible = false;
                pnlChat.Visible = true;
            }
            catch
            {
                lblError.Text = "Could not load chat.";
                lblError.Visible = true;
            }
        }

        private int GetOrCreateConversation(SqlConnection con, int userId, int lecturerId)
        {
            // check if a conversation already exists between these two users
            string checkQuery = @"SELECT conversationid FROM ChatConversation
                                   WHERE (userid = @uid AND SecondUserID = @lid)
                                   OR (userid = @lid AND SecondUserID = @uid)";

            using (SqlCommand cmd = new SqlCommand(checkQuery, con))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@lid", lecturerId);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
            }

            // no conversation yet, create one
            using (SqlCommand ins = new SqlCommand(
                "INSERT INTO ChatConversation (userid, SecondUserID, ConversationType, creationtime) VALUES (@uid, @lid, 'Direct', @now); SELECT SCOPE_IDENTITY();", con))
            {
                ins.Parameters.AddWithValue("@uid", userId);
                ins.Parameters.AddWithValue("@lid", lecturerId);
                ins.Parameters.AddWithValue("@now", DateTime.Now);
                return Convert.ToInt32(ins.ExecuteScalar());
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            if (ViewState["convId"] == null) return;

            int convId = Convert.ToInt32(ViewState["convId"]);
            int userId = Convert.ToInt32(Session["userid"]);
            int lecturerId = Convert.ToInt32(ViewState["lecturerId"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query = @"INSERT INTO ChatMessage (conversationid, senderid, message, creationtime, isread)
                                     VALUES (@cid, @sid, @msg, @now, 0)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@cid", convId);
                        cmd.Parameters.AddWithValue("@sid", userId);
                        cmd.Parameters.AddWithValue("@msg", msg);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }

                txtMessage.Text = "";

                // reload chat to show the new message
                SelectedLecturerId = lecturerId;
                LoadLecturers();
                LoadChat(lecturerId);
            }
            catch
            {
                lblError.Text = "Could not send message. Please try again.";
                lblError.Visible = true;
            }
        }

        protected void rptLecturers_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            // handled via javascript redirect instead
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