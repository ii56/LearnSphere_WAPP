using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class Message : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // Ties the ViewState to the user session to prevent CSRF attacks
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        // Redirects non-lecturers away, then loads the profile image and conversation list on first visit
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null ||
                Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfileImage();
                LoadConversations();
            }
        }

        // Shortcut to the logged-in user's ID
        protected int CurrentUserID
        {
            get { return Session["userid"] != null ? Convert.ToInt32(Session["userid"]) : 0; }
        }

        // The conversation currently open in the chat panel — persisted across postbacks via ViewState
        protected int CurrentConversationID
        {
            get { return ViewState["ConversationID"] != null ? Convert.ToInt32(ViewState["ConversationID"]) : 0; }
            set { ViewState["ConversationID"] = value; }
        }

        // Pulls the lecturer's profile picture and sets it in the header avatar
        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null) return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ProfileImage FROM [User] WHERE userid = @id", con);
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

        // Confirms the current user is a participant in the given conversation before allowing access
        private bool CanAccessConversation(int convId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM ChatConversation
                    WHERE conversationid=@cid
                    AND (userid=@uid OR SecondUserID=@uid)", con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = convId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Loads all conversations the current user is part of, showing the other participant's name and avatar
        private void LoadConversations()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT
                        c.conversationid,
                        CASE WHEN c.userid=@uid THEN u2.userid      ELSE u1.userid      END AS userid,
                        CASE WHEN c.userid=@uid THEN (u2.fname + ' ' + u2.lname)
                                                ELSE (u1.fname + ' ' + u1.lname) END AS DisplayName,
                        CASE WHEN c.userid=@uid THEN u2.ProfileImage ELSE u1.ProfileImage END AS ProfileImage
                    FROM ChatConversation c
                    LEFT JOIN [User] u1 ON c.userid       = u1.userid
                    LEFT JOIN [User] u2 ON c.SecondUserID = u2.userid
                    WHERE (c.userid=@uid OR c.SecondUserID=@uid)
                    AND c.ConversationType='User'
                    ORDER BY c.creationtime DESC", conn);
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;
                conn.Open();
                rptConversations.DataSource = cmd.ExecuteReader();
                rptConversations.DataBind();
            }
        }

        // Returns the display name of the other participant so the chat header stays accurate when opening a conversation
        private string GetConversationPartnerName(int convId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT
                        CASE WHEN c.userid=@uid THEN (u2.fname + ' ' + u2.lname)
                                                ELSE (u1.fname + ' ' + u1.lname) END AS DisplayName
                    FROM ChatConversation c
                    LEFT JOIN [User] u1 ON c.userid       = u1.userid
                    LEFT JOIN [User] u2 ON c.SecondUserID = u2.userid
                    WHERE c.conversationid = @cid", conn);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = convId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "Conversation";
            }
        }

        // Fetches a user's profile details and displays them in the popup card
        private void ShowProfile(int userID)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT fname + ' ' + lname AS FullName,
                           email, usertype, ProfileImage, Description, status
                    FROM [User]
                    WHERE userid=@id", conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userID;
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblProfileName.Text = Server.HtmlEncode(dr["FullName"].ToString());
                    lblProfileEmail.Text = Server.HtmlEncode(dr["email"].ToString());
                    lblProfileRole.Text = Server.HtmlEncode(dr["usertype"].ToString());
                    lblProfileDesc.Text = Server.HtmlEncode(dr["Description"].ToString());

                    // Status can be stored as either an int (0/1) or a plain string depending on the record
                    if (dr["status"] != DBNull.Value)
                    {
                        string rawStatus = dr["status"].ToString().Trim();
                        int statusInt;
                        if (int.TryParse(rawStatus, out statusInt))
                            lblProfileStatus.Text = statusInt == 1 ? "Active" : "Inactive";
                        else
                            lblProfileStatus.Text = rawStatus;
                    }
                    else
                    {
                        lblProfileStatus.Text = "Unknown";
                    }

                    // Resolve the profile image path from whatever format it was stored in
                    string finalImageUrl = ResolveUrl("~/images/default-user.png");
                    if (dr["ProfileImage"] != DBNull.Value)
                    {
                        string path = dr["ProfileImage"].ToString().Trim();
                        if (!string.IsNullOrEmpty(path))
                        {
                            if (path.StartsWith("~/")) finalImageUrl = ResolveUrl(path);
                            else if (path.StartsWith("images/")) finalImageUrl = ResolveUrl("~/" + path);
                            else if (path.StartsWith("http")) finalImageUrl = path;
                            else finalImageUrl = ResolveUrl("~/images/" + path);
                        }
                    }

                    imgProfileCard.ImageUrl = finalImageUrl;

                    // Add a role-specific CSS class so the avatar border colour matches the user type
                    string role = dr["usertype"].ToString().ToLower();
                    imgProfileCard.CssClass = "popup-avatar";
                    if (role == "lecturer") imgProfileCard.CssClass += " avatar-lecturer";
                    else if (role == "admin") imgProfileCard.CssClass += " avatar-admin";
                    else if (role == "student") imgProfileCard.CssClass += " avatar-student";
                    else imgProfileCard.CssClass += " avatar-public";

                    // Only show the verified badge for lecturers
                    lblVerifyBadge.Visible = (role == "lecturer");
                    profilePopup.Style["display"] = "flex";
                }
            }
        }

        // Handles opening a conversation or viewing a profile from the conversations list
        protected void rptConversations_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewProfile")
            {
                int userID;
                if (!int.TryParse(e.CommandArgument.ToString(), out userID)) return;
                ShowProfile(userID);
                return;
            }

            if (e.CommandName == "OpenChat")
            {
                int convId;
                if (!int.TryParse(e.CommandArgument.ToString(), out convId)) return;
                if (!CanAccessConversation(convId)) return;

                CurrentConversationID = convId;
                lblChatTitle.Text = "💬 " + Server.HtmlEncode(GetConversationPartnerName(convId));
                LoadMessages();
            }
        }

        // Validates the message, applies a 2-second rate limit, then inserts it and refreshes the chat
        protected void btnSend_Click(object sender, EventArgs e)
        {
            profilePopup.Style["display"] = "none";

            if (CurrentConversationID == 0 || !CanAccessConversation(CurrentConversationID))
                return;

            string msg = Server.HtmlEncode(txtMessage.Text.Trim());
            if (msg.Length == 0 || msg.Length > 1000) return;

            // Prevent accidental or deliberate rapid-fire messages
            if (Session["lastMsgTime"] != null)
            {
                DateTime last = (DateTime)Session["lastMsgTime"];
                if ((DateTime.UtcNow - last).TotalSeconds < 2) return;
            }
            Session["lastMsgTime"] = DateTime.UtcNow;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO ChatMessage
                    (conversationid, role, content, SenderID)
                    VALUES (@cid, 'User', @content, @sid)", conn);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = CurrentConversationID;
                cmd.Parameters.Add("@content", SqlDbType.NVarChar, 1000).Value = msg;
                cmd.Parameters.Add("@sid", SqlDbType.Int).Value = CurrentUserID;
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            txtMessage.Text = "";
            LoadMessages();
        }

        // Fetches all messages for the current conversation and auto-scrolls the chat to the bottom
        private void LoadMessages()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT m.content, m.creationtime, m.SenderID, u.ProfileImage,
                           CASE WHEN m.SenderID=@uid THEN 1 ELSE 0 END AS IsMine
                    FROM ChatMessage m
                    INNER JOIN [User] u ON m.SenderID = u.userid
                    WHERE m.conversationid = @cid
                    ORDER BY m.creationtime ASC", conn);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = CurrentConversationID;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;
                conn.Open();
                rptMessages.DataSource = cmd.ExecuteReader();
                rptMessages.DataBind();
            }

            // ClientScript is used here instead of ScriptManager since this page has no UpdatePanel
            ClientScript.RegisterStartupScript(
                this.GetType(),
                "scrollChat",
                "setTimeout(function(){ var c = document.querySelector('.chat-messages'); if(c) c.scrollTop = c.scrollHeight; }, 100);",
                true);
        }

        // Searches for users by name and shows the top 20 matches in the search results panel
        protected void btnSearchUser_Click(object sender, EventArgs e)
        {
            string input = txtSearchUser.Text.Trim();
            if (input.Length < 2) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 20 userid, fname + ' ' + lname AS FullName, email, ProfileImage
                    FROM [User]
                    WHERE (fname + ' ' + lname) LIKE @search
                    AND userid != @uid", conn);
                cmd.Parameters.Add("@search", SqlDbType.NVarChar, 100).Value = "%" + input + "%";
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;
                conn.Open();
                rptSearchResults.DataSource = cmd.ExecuteReader();
                rptSearchResults.DataBind();
            }
        }

        // Handles starting a chat or viewing a profile from the search results
        protected void rptSearchResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewProfile")
            {
                int userID;
                if (!int.TryParse(e.CommandArgument.ToString(), out userID)) return;
                ShowProfile(userID);
                return;
            }

            if (e.CommandName == "StartChat")
            {
                int targetId;
                if (!int.TryParse(e.CommandArgument.ToString(), out targetId)) return;

                int convId = GetOrCreateConversation(CurrentUserID, targetId);
                CurrentConversationID = convId;
                lblChatTitle.Text = "💬 " + Server.HtmlEncode(GetConversationPartnerName(convId));

                LoadMessages();
                LoadConversations();

                // Clear the search panel once the chat is open
                rptSearchResults.DataSource = null;
                rptSearchResults.DataBind();
                txtSearchUser.Text = "";
            }
        }

        // Returns an existing conversation ID between two users, or creates a new one if none exists
        private int GetOrCreateConversation(int user1, int user2)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Check both directions since either user could have started the conversation
                SqlCommand cmd = new SqlCommand(@"
                    SELECT conversationid FROM ChatConversation
                    WHERE ConversationType='User'
                    AND ((userid=@u1 AND SecondUserID=@u2)
                      OR (userid=@u2 AND SecondUserID=@u1))", conn);
                cmd.Parameters.Add("@u1", SqlDbType.Int).Value = user1;
                cmd.Parameters.Add("@u2", SqlDbType.Int).Value = user2;
                object res = cmd.ExecuteScalar();
                if (res != null) return Convert.ToInt32(res);

                SqlCommand ins = new SqlCommand(@"
                    INSERT INTO ChatConversation(userid, SecondUserID, ConversationType)
                    VALUES(@u1, @u2, 'User');
                    SELECT SCOPE_IDENTITY();", conn);
                ins.Parameters.Add("@u1", SqlDbType.Int).Value = user1;
                ins.Parameters.Add("@u2", SqlDbType.Int).Value = user2;
                return Convert.ToInt32(ins.ExecuteScalar());
            }
        }

        // Clears the session and sends the lecturer back to the login page
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}