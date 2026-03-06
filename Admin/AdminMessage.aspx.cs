using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminMessage : System.Web.UI.Page
    {
        static string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        static SqlConnection con = new SqlConnection(connStr);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
            }
            if (!IsPostBack)
            {
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
                string imagePath = result.ToString();
                sidebarImg.Src = ResolveUrl(imagePath);
            }
            else
            {
                sidebarImg.Src = ResolveUrl("~/images/default-user.png");
            }
            con.Close();
        }

        protected int CurrentUserID
        {
            get
            {
                return Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;
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

        private void LoadConversations()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                                SELECT 
                                    c.conversationid,
                                    CASE 
                                        WHEN c.userid = @UserID 
                                            THEN (u2.fname + ' ' + u2.lname)
                                        ELSE (u1.fname + ' ' + u1.lname)
                                    END AS DisplayName,
                                    CASE 
                                        WHEN c.userid = @UserID 
                                            THEN u2.ProfileImage
                                        ELSE u1.ProfileImage
                                    END AS ProfileImage
                                FROM ChatConversation c
                                LEFT JOIN [User] u1 ON c.userid = u1.userid
                                LEFT JOIN [User] u2 ON c.SecondUserID = u2.userid
                                WHERE 
                                    (c.userid = @UserID OR c.SecondUserID = @UserID)
                                    AND c.ConversationType = 'User'
                                ORDER BY c.creationtime DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID);

                conn.Open();
                rptConversations.DataSource = cmd.ExecuteReader();
                rptConversations.DataBind();
            }
        }


        protected void rptMessages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void rptConversations_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "OpenChat")
            {
                CurrentConversationID = Convert.ToInt32(e.CommandArgument);
                LoadMessages();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text) || CurrentConversationID == 0)
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                INSERT INTO ChatMessage (conversationid, role, content, SenderID)
                VALUES (@ConversationID, 'User', @Content, @SenderID)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ConversationID", CurrentConversationID);
                cmd.Parameters.AddWithValue("@Content", txtMessage.Text.Trim());
                cmd.Parameters.AddWithValue("@SenderID", CurrentUserID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            txtMessage.Text = "";
            LoadMessages();
        }

        private void LoadMessages()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                                SELECT 
                                    m.messageid,
                                    m.content,
                                    m.creationtime,
                                    m.SenderID,
                                    u.ProfileImage,
                                    CASE 
                                        WHEN m.SenderID = @UserID THEN 1 
                                        ELSE 0 
                                    END AS IsMine
                                FROM ChatMessage m
                                INNER JOIN [User] u ON m.SenderID = u.userid
                                WHERE m.conversationid = @ConversationID
                                ORDER BY m.creationtime ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ConversationID", CurrentConversationID);
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID);

                conn.Open();
                rptMessages.DataSource = cmd.ExecuteReader();
                rptMessages.DataBind();
            }
        }

        protected void txtMessage_TextChanged(object sender, EventArgs e)
        {

        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("../Login.aspx");
        }

        protected void btnSearchUser_Click(object sender, EventArgs e)
        {
            SearchUser();
        }

        private void SearchUser()
        {
            if (string.IsNullOrWhiteSpace(txtSearchUser.Text))
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                              SELECT 
                                    userid,
                                    fname + ' ' + lname AS FullName,
                                    email,
                                    ProfileImage
                                FROM [User]
                                WHERE 
                                    (fname + ' ' + lname) LIKE @Search
                                    AND userid != @CurrentUser";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Search", "%" + txtSearchUser.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@CurrentUser", CurrentUserID);

                conn.Open();
                rptSearchResults.DataSource = cmd.ExecuteReader();
                rptSearchResults.DataBind();
            }
        }


        protected void rptSearchResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "StartChat")
            {
                int targetUserID = Convert.ToInt32(e.CommandArgument);
                int conversationID = GetOrCreateConversation(CurrentUserID, targetUserID);

                CurrentConversationID = conversationID;
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
                string checkQuery = @"
                                    SELECT conversationid
                                    FROM ChatConversation
                                    WHERE ConversationType = 'User'
                                    AND (
                                        (userid = @User1 AND SecondUserID = @User2)
                                        OR
                                        (userid = @User2 AND SecondUserID = @User1)
                                    )";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@User1", user1);
                checkCmd.Parameters.AddWithValue("@User2", user2);

                object result = checkCmd.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);

                string insertQuery = @"
                                    INSERT INTO ChatConversation (userid, SecondUserID, ConversationType)
                                    VALUES (@User1, @User2, 'User');
                                    SELECT SCOPE_IDENTITY();";

                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@User1", user1);
                insertCmd.Parameters.AddWithValue("@User2", user2);

                return Convert.ToInt32(insertCmd.ExecuteScalar());
            }
        }

        protected void txtSearchUser_TextChanged(object sender, EventArgs e)
        {
            SearchUser();
        }
    }
}