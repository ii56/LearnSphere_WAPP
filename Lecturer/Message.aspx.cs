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

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

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

        protected int CurrentUserID
        {
            get { return Session["userid"] != null ? Convert.ToInt32(Session["userid"]) : 0; }
        }

        protected int CurrentConversationID
        {
            get { return ViewState["ConversationID"] != null ? Convert.ToInt32(ViewState["ConversationID"]) : 0; }
            set { ViewState["ConversationID"] = value; }
        }

        // 🔐 PROFILE IMAGE
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

        // 🔐 CONVERSATION ACCESS
        private bool CanAccessConversation(int convId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = @"SELECT COUNT(*) FROM ChatConversation
                             WHERE conversationid=@cid
                             AND (userid=@uid OR SecondUserID=@uid)";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = convId;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;

                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // 🔐 LOAD CONVERSATIONS
        private void LoadConversations()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT 
                        c.conversationid,
                        CASE WHEN c.userid=@uid THEN u2.userid ELSE u1.userid END AS userid,
                        CASE WHEN c.userid=@uid THEN (u2.fname + ' ' + u2.lname)
                             ELSE (u1.fname + ' ' + u1.lname) END AS DisplayName,
                        CASE WHEN c.userid=@uid THEN u2.ProfileImage ELSE u1.ProfileImage END AS ProfileImage
                    FROM ChatConversation c
                    LEFT JOIN [User] u1 ON c.userid = u1.userid
                    LEFT JOIN [User] u2 ON c.SecondUserID = u2.userid
                    WHERE (c.userid=@uid OR c.SecondUserID=@uid)
                    AND c.ConversationType='User'
                    ORDER BY c.creationtime DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;

                conn.Open();
                rptConversations.DataSource = cmd.ExecuteReader();
                rptConversations.DataBind();
            }
        }

        // 🔐 VIEW PROFILE (WITH AUTHORIZATION)
        private void ShowProfile(int userID)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
        SELECT fname + ' ' + lname AS FullName,
               email, usertype, ProfileImage, Description, status
        FROM [User]
        WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userID;

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblProfileName.Text = Server.HtmlEncode(dr["FullName"].ToString());
                    lblProfileEmail.Text = Server.HtmlEncode(dr["email"].ToString());
                    lblProfileRole.Text = Server.HtmlEncode(dr["usertype"].ToString());
                    lblProfileDesc.Text = Server.HtmlEncode(dr["Description"].ToString());

                    int status = dr["status"] != DBNull.Value ? Convert.ToInt32(dr["status"]) : 0;
                    lblProfileStatus.Text = status == 1 ? "Active" : "Inactive";

                    string finalImageUrl = ResolveUrl("~/images/default-user.png");

                    if (dr["ProfileImage"] != DBNull.Value)
                    {
                        string path = dr["ProfileImage"].ToString().Trim();

                        if (!string.IsNullOrEmpty(path))
                        {
                            if (path.StartsWith("~/"))
                                finalImageUrl = ResolveUrl(path);
                            else if (path.StartsWith("images/"))
                                finalImageUrl = ResolveUrl("~/" + path);
                            else if (path.StartsWith("http"))
                                finalImageUrl = path;
                            else
                                finalImageUrl = ResolveUrl("~/images/" + path);
                        }
                    }

                    imgProfileCard.ImageUrl = finalImageUrl;

                    string role = dr["usertype"].ToString().ToLower();
                    imgProfileCard.CssClass = "popup-avatar";

                    if (role == "lecturer") imgProfileCard.CssClass += " avatar-lecturer";
                    else if (role == "admin") imgProfileCard.CssClass += " avatar-admin";
                    else if (role == "student") imgProfileCard.CssClass += " avatar-student";
                    else imgProfileCard.CssClass += " avatar-public";

                    lblVerifyBadge.Visible = role == "lecturer";

                    // 🔥 IMPORTANT FIX
                    profilePopup.Style["display"] = "flex";
                }
            }
        }

        // 🔐 OPEN CHAT
        protected void rptConversations_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // ✅ ONLY handle ViewProfile when explicitly clicked
            if (e.CommandName == "ViewProfile")
            {
                int userID;
                if (!int.TryParse(e.CommandArgument.ToString(), out userID))
                    return;

                ShowProfile(userID);
                return; // 🔥 STOP further execution
            }

            // ✅ ONLY handle OpenChat here
            if (e.CommandName == "OpenChat")
            {
                int convId;
                if (!int.TryParse(e.CommandArgument.ToString(), out convId))
                    return;

                if (!CanAccessConversation(convId))
                    return;

                CurrentConversationID = convId;
                LoadMessages();
            }
        }

        // 🔐 SEND MESSAGE
        protected void btnSend_Click(object sender, EventArgs e)
        {
            profilePopup.Style["display"] = "none";
            if (CurrentConversationID == 0 || !CanAccessConversation(CurrentConversationID))
                return;

            string msg = Server.HtmlEncode(txtMessage.Text.Trim());

            if (msg.Length == 0 || msg.Length > 1000)
                return;

            if (Session["lastMsgTime"] != null)
            {
                DateTime last = (DateTime)Session["lastMsgTime"];
                if ((DateTime.UtcNow - last).TotalSeconds < 2)
                    return;
            }

            Session["lastMsgTime"] = DateTime.UtcNow;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO ChatMessage 
                                (conversationid, role, content, SenderID)
                                VALUES (@cid, 'User', @content, @sid)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = CurrentConversationID;
                cmd.Parameters.Add("@content", SqlDbType.NVarChar, 1000).Value = msg;
                cmd.Parameters.Add("@sid", SqlDbType.Int).Value = CurrentUserID;

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            txtMessage.Text = "";
            LoadMessages();
        }

        // 🔐 LOAD MESSAGES
        private void LoadMessages()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT m.content, m.creationtime, m.SenderID, u.ProfileImage,
                    CASE WHEN m.SenderID=@uid THEN 1 ELSE 0 END AS IsMine
                    FROM ChatMessage m
                    INNER JOIN [User] u ON m.SenderID=u.userid
                    WHERE m.conversationid=@cid
                    ORDER BY m.creationtime ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = CurrentConversationID;
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;

                conn.Open();
                rptMessages.DataSource = cmd.ExecuteReader();
                rptMessages.DataBind();
            }

            // 🔥 AUTO SCROLL
            ScriptManager.RegisterStartupScript(this, GetType(), "scroll",
                "setTimeout(function(){var chat=document.querySelector('.chat-messages'); if(chat){chat.scrollTop=chat.scrollHeight;}},100);",
                true);
        }

        // 🔐 SEARCH USERS
        protected void btnSearchUser_Click(object sender, EventArgs e)
        {
            string input = txtSearchUser.Text.Trim();

            if (input.Length < 2)
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT TOP 20 userid, fname + ' ' + lname AS FullName, email, ProfileImage
                    FROM [User]
                    WHERE (fname + ' ' + lname) LIKE @search
                    AND userid != @uid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@search", SqlDbType.NVarChar, 100).Value = "%" + input + "%";
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = CurrentUserID;

                conn.Open();
                rptSearchResults.DataSource = cmd.ExecuteReader();
                rptSearchResults.DataBind();
            }
        }

        // 🔐 START CHAT
        protected void rptSearchResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // 🔥 STRICT separation

            if (e.CommandName == "ViewProfile")
            {
                int userID;
                if (!int.TryParse(e.CommandArgument.ToString(), out userID))
                    return;

                ShowProfile(userID);
                return; // 🔥 IMPORTANT
            }

            if (e.CommandName == "StartChat")
            {
                int targetId;
                if (!int.TryParse(e.CommandArgument.ToString(), out targetId))
                    return;

                int convId = GetOrCreateConversation(CurrentUserID, targetId);

                CurrentConversationID = convId;
                LoadMessages();
                LoadConversations();

                rptSearchResults.DataSource = null;
                rptSearchResults.DataBind();
                txtSearchUser.Text = "";
            }
        }

        private int GetOrCreateConversation(int user1, int user2)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string check = @"SELECT conversationid FROM ChatConversation
                                 WHERE ConversationType='User'
                                 AND ((userid=@u1 AND SecondUserID=@u2)
                                 OR (userid=@u2 AND SecondUserID=@u1))";

                SqlCommand cmd = new SqlCommand(check, conn);
                cmd.Parameters.Add("@u1", SqlDbType.Int).Value = user1;
                cmd.Parameters.Add("@u2", SqlDbType.Int).Value = user2;

                object res = cmd.ExecuteScalar();

                if (res != null)
                    return Convert.ToInt32(res);

                string insert = @"INSERT INTO ChatConversation(userid,SecondUserID,ConversationType)
                                  VALUES(@u1,@u2,'User');
                                  SELECT SCOPE_IDENTITY();";

                SqlCommand ins = new SqlCommand(insert, conn);
                ins.Parameters.Add("@u1", SqlDbType.Int).Value = user1;
                ins.Parameters.Add("@u2", SqlDbType.Int).Value = user2;

                return Convert.ToInt32(ins.ExecuteScalar());
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}