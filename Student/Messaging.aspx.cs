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

        // Keeps track of which lecturer is selected across postbacks
        public int SelectedLecturerId { get; set; }

        // Redirects unauthenticated users, sets up the header, then loads lecturers and chat
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

            // Lecturer ID comes from the query string when a lecturer item is clicked
            if (Request.QueryString["lecturerId"] != null)
                SelectedLecturerId = Convert.ToInt32(Request.QueryString["lecturerId"]);

            if (!IsPostBack)
            {
                LoadLecturers();

                if (SelectedLecturerId > 0)
                    LoadChat(SelectedLecturerId);
            }
        }

        // Fetches the student's first name from session or the DB if not cached yet
        private string GetDisplayName()
        {
            if (Session["fname"] != null && Session["fname"].ToString() != "")
                return Session["fname"].ToString();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT fname FROM [User] WHERE userid = @uid", con))
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

        // Loads the list of lecturers from courses the student is actively enrolled in
        private void LoadLecturers()
        {
            try
            {
                int userId = Convert.ToInt32(Session["userid"]);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query = @"
                        SELECT DISTINCT u.userid, u.fname, u.lname, c.coursename
                        FROM [User] u
                        INNER JOIN Course c     ON c.ownerid  = u.userid
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

        // Loads the chat header info and all messages for the selected lecturer
        private void LoadChat(int lecturerId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["userid"]);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Populate the chat header with the lecturer's name and course
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT u.fname, u.lname, c.coursename
                        FROM [User] u
                        INNER JOIN Course c     ON c.ownerid  = u.userid
                        INNER JOIN Enrollment e ON e.courseid = c.courseid
                        WHERE u.userid = @lid AND e.userid = @uid AND e.isactive = 1", con))
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

                    int convId = GetOrCreateConversation(con, userId, lecturerId);

                    // Read messages using the correct column names: SenderID and content
                    string msgQuery = @"
                        SELECT messageid, SenderID AS senderid, content AS message, creationtime
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

                    // Persist the conversation and lecturer IDs so btnSend can use them
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

        // Returns an existing conversation ID between the two users or creates a new one
        // ConversationType must be 'User' to match the lecturer messaging page
        private int GetOrCreateConversation(SqlConnection con, int userId, int lecturerId)
        {
            // Check both directions since either party could have started the conversation
            string checkQuery = @"
                SELECT conversationid FROM ChatConversation
                WHERE ConversationType = 'User'
                AND ((userid = @uid AND SecondUserID = @lid)
                  OR (userid = @lid AND SecondUserID = @uid))";

            using (SqlCommand cmd = new SqlCommand(checkQuery, con))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@lid", lecturerId);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
            }

            // No conversation found — create one with the correct type
            using (SqlCommand ins = new SqlCommand(@"
                INSERT INTO ChatConversation (userid, SecondUserID, ConversationType, creationtime)
                VALUES (@uid, @lid, 'User', @now);
                SELECT SCOPE_IDENTITY();", con))
            {
                ins.Parameters.AddWithValue("@uid", userId);
                ins.Parameters.AddWithValue("@lid", lecturerId);
                ins.Parameters.AddWithValue("@now", DateTime.Now);
                return Convert.ToInt32(ins.ExecuteScalar());
            }
        }

        // Inserts the message using the correct column names (content, SenderID) then reloads the chat
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

                    // Column names match the lecturer's ChatMessage schema: content and SenderID
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ChatMessage (conversationid, SenderID, content, creationtime, role)
                        VALUES (@cid, @sid, @msg, @now, 'User')", con))
                    {
                        cmd.Parameters.AddWithValue("@cid", convId);
                        cmd.Parameters.AddWithValue("@sid", userId);
                        cmd.Parameters.AddWithValue("@msg", msg);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }

                txtMessage.Text = "";

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

        // Lecturer selection is handled via JavaScript redirect — this handler is unused
        protected void rptLecturers_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
        }

        // Clears the session and sends the student back to the login page
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}