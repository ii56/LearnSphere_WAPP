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
            // 🔐 CSRF Protection
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 AUTHENTICATION
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
            get
            {
                return Session["userid"] != null ? Convert.ToInt32(Session["userid"]) : 0;
            }
        }

        protected int CurrentConversationID
        {
            get
            {
                return ViewState["ConversationID"] != null ? Convert.ToInt32(ViewState["ConversationID"]) : 0;
            }
            set
            {
                ViewState["ConversationID"] = value;
            }
        }

        // 🔐 PROFILE IMAGE
        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = CurrentUserID;

                con.Open();
                object result = cmd.ExecuteScalar();

                string imagePath = "~/images/default-user.png";

                if (result != null && result != DBNull.Value)
                {
                    string path = result.ToString();

                    if (path.StartsWith("~/images/"))
                        imagePath = path;
                }

                imgSidebarProfile.Src = ResolveUrl(imagePath);
            }
        }

        // 🔐 CONVERSATION ACCESS CONTROL
        private bool CanAccessConversation(int convId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string q = @"
                    SELECT COUNT(*)
                    FROM ChatConversation
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

        // 🔐 VIEW PROFILE
        protected void ViewProfile_Command(object sender, CommandEventArgs e)
        {
            int userID;
            if (!int.TryParse(e.CommandArgument.ToString(), out userID))
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT fname + ' ' + lname, email, usertype, ProfileImage, Description, status
                                 FROM [User] WHERE userid=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userID;

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblProfileName.Text = Server.HtmlEncode(dr[0].ToString());
                    lblProfileEmail.Text = Server.HtmlEncode(dr[1].ToString());
                    lblProfileRole.Text = Server.HtmlEncode(dr[2].ToString());
                    lblProfileDesc.Text = Server.HtmlEncode(dr[4].ToString());

                    lblProfileStatus.Text = Convert.ToInt32(dr[5]) == 1 ? "Active" : "Inactive";

                    string img = "~/images/default-user.png";
                    if (dr[3] != DBNull.Value && dr[3].ToString().StartsWith("~/images/"))
                        img = dr[3].ToString();

                    imgProfileCard.ImageUrl = ResolveUrl(img);

                    lblVerifyBadge.Visible = dr[2].ToString().ToLower() == "lecturer";
                    profilePopup.Visible = true;
                }
            }
        }

        // 🔐 OPEN CHAT
        protected void rptConversations_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
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
            if (CurrentConversationID == 0 || !CanAccessConversation(CurrentConversationID))
                return;

            string msg = txtMessage.Text.Trim();

            if (msg.Length == 0 || msg.Length > 1000)
            {
                return;
            }

            // 🔐 Rate limiting (2 sec)
            if (Session["lastMsgTime"] != null)
            {
                DateTime last = (DateTime)Session["lastMsgTime"];
                if ((DateTime.Now - last).TotalSeconds < 2)
                    return;
            }

            Session["lastMsgTime"] = DateTime.Now;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    INSERT INTO ChatMessage (conversationid, role, content, SenderID)
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

                string check = @"
                    SELECT conversationid
                    FROM ChatConversation
                    WHERE ConversationType='User'
                    AND ((userid=@u1 AND SecondUserID=@u2)
                    OR (userid=@u2 AND SecondUserID=@u1))";

                SqlCommand cmd = new SqlCommand(check, conn);
                cmd.Parameters.Add("@u1", SqlDbType.Int).Value = user1;
                cmd.Parameters.Add("@u2", SqlDbType.Int).Value = user2;

                object res = cmd.ExecuteScalar();

                if (res != null)
                    return Convert.ToInt32(res);

                string insert = @"
                    INSERT INTO ChatConversation(userid,SecondUserID,ConversationType)
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