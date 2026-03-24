using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class Messaging : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // Keeps track of the selected lecturer ID from the URL
        public int SelectedLecturerId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null || Session["usertype"].ToString() != "General")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (Request.QueryString["lecturerId"] != null)
            {
                SelectedLecturerId = Convert.ToInt32(Request.QueryString["lecturerId"]);
            }

            if (!IsPostBack)
            {
                LoadHeaderProfile();
                LoadLecturers();

                if (SelectedLecturerId > 0)
                {
                    LoadChat(SelectedLecturerId);
                }
            }
        }

        private void LoadHeaderProfile()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string displayName = "User";
                using (SqlCommand cmd = new SqlCommand("SELECT fname, ProfileImage FROM [User] WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            displayName = reader["fname"].ToString();
                            Session["fname"] = displayName;

                            string imgPath = reader["ProfileImage"] != DBNull.Value ? reader["ProfileImage"].ToString() : "";

                            if (!string.IsNullOrEmpty(imgPath))
                            {
                                imgHeaderAvatar.ImageUrl = ResolveUrl(imgPath);
                                imgHeaderAvatar.Visible = true;
                                lblAvatarInitial.Visible = false;
                            }
                            else
                            {
                                imgHeaderAvatar.Visible = false;
                                lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();
                                lblAvatarInitial.Visible = true;
                            }
                        }
                    }
                }
                lblHeaderName.Text = displayName;
            }
        }

        protected string GetProfileImage(object imageObj)
        {
            if (imageObj == null || imageObj == DBNull.Value || string.IsNullOrEmpty(imageObj.ToString().Trim()))
                return ResolveUrl("~/images/default-user.png");
            return ResolveUrl(imageObj.ToString());
        }

        private void LoadLecturers()
        {
            try
            {
                int userId = Convert.ToInt32(Session["userid"]);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Get lecturers of courses the general user is enrolled in (free courses)
                    string query = @"
                        SELECT DISTINCT u.userid, u.fname, u.lname, u.ProfileImage, c.coursename
                        FROM [User] u
                        INNER JOIN Course c ON c.ownerid = u.userid
                        INNER JOIN Enrollment e ON e.courseid = c.courseid
                        WHERE e.userid = @uid AND e.isactive = 1 AND c.status = 'Active'
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

                    // FIXED: simpler validation
                    using (SqlCommand cmd = new SqlCommand(@"
                SELECT fname, lname, ProfileImage 
                FROM [User]
                WHERE userid = @lid", con))
                    {
                        cmd.Parameters.AddWithValue("@lid", lecturerId);

                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                lblChatName.Text = r["fname"] + " " + r["lname"];
                                imgChatAvatar.ImageUrl = GetProfileImage(r["ProfileImage"]);
                                lblChatCourse.Text = "";
                            }
                            else
                            {
                                Response.Redirect("Messaging.aspx");
                                return;
                            }
                        }
                    }

                    int convId = GetOrCreateConversation(con, userId, lecturerId);

                    string msgQuery = @"
                SELECT 
                    m.messageid,
                    m.SenderID,
                    m.content,
                    m.creationtime,
                    u.ProfileImage,
                    CASE WHEN m.SenderID = @uid THEN 1 ELSE 0 END AS IsMine
                FROM ChatMessage m
                INNER JOIN [User] u ON m.SenderID = u.userid
                WHERE m.conversationid = @cid
                ORDER BY m.creationtime ASC";

                    using (SqlDataAdapter da = new SqlDataAdapter(msgQuery, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@cid", convId);
                        da.SelectCommand.Parameters.AddWithValue("@uid", userId);

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        rptMessages.DataSource = dt;
                        rptMessages.DataBind();
                    }

                    ViewState["convId"] = convId;
                    ViewState["lecturerId"] = lecturerId;
                }

                pnlChatPlaceholder.Visible = false;
                pnlChat.Visible = true;
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private int GetOrCreateConversation(SqlConnection con, int userId, int lecturerId)
        {
            // Check if conversation exists
            string checkQuery = @"
                SELECT conversationid FROM ChatConversation
                WHERE ConversationType = 'User'
                AND ((userid = @uid AND SecondUserID = @lid) OR (userid = @lid AND SecondUserID = @uid))";

            using (SqlCommand cmd = new SqlCommand(checkQuery, con))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@lid", lecturerId);
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }

            // Create new conversation
            using (SqlCommand ins = new SqlCommand(@"
                INSERT INTO ChatConversation (userid, SecondUserID, ConversationType, creationtime) 
                VALUES (@uid, @lid, 'User', GETDATE()); 
                SELECT SCOPE_IDENTITY();", con))
            {
                ins.Parameters.AddWithValue("@uid", userId);
                ins.Parameters.AddWithValue("@lid", lecturerId);
                return Convert.ToInt32(ins.ExecuteScalar());
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg) || ViewState["convId"] == null) return;

            // Simple rate limit to prevent spam clicking
            if (Session["lastMsgTime"] != null)
            {
                DateTime last = (DateTime)Session["lastMsgTime"];
                if ((DateTime.UtcNow - last).TotalSeconds < 1) return;
            }
            Session["lastMsgTime"] = DateTime.UtcNow;

            int convId = Convert.ToInt32(ViewState["convId"]);
            int userId = Convert.ToInt32(Session["userid"]);
            int lecturerId = Convert.ToInt32(ViewState["lecturerId"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Note: Insert uses the 'content' and 'role' schema structure.
                    string query = @"
                        INSERT INTO ChatMessage (conversationid, role, content, SenderID, creationtime)
                        VALUES (@cid, 'User', @msg, @sid, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@cid", convId);
                        cmd.Parameters.AddWithValue("@sid", userId);
                        cmd.Parameters.AddWithValue("@msg", msg);
                        cmd.ExecuteNonQuery();
                    }
                }

                txtMessage.Text = "";

                // Reload the chat panel to display the new message
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

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}