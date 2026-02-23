using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace LearnSphere_WAPP.Chatbot
{
    public partial class Chatbot : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        // Static HttpClient (correct pattern)
        private static readonly HttpClient client;

        // Static constructor runs ONCE
        static Chatbot()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;

            client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // DELETE
            if (!string.IsNullOrEmpty(Request.QueryString["delete"]))
            {
                int deleteId;
                if (int.TryParse(Request.QueryString["delete"], out deleteId))
                {
                    if (ConversationBelongsToUser(deleteId))
                    {
                        DeleteConversation(deleteId);
                    }
                }

                Session["ConversationID"] = null;
                Response.Redirect("Chatbot.aspx");
                return;
            }

            // RENAME
            if (!string.IsNullOrEmpty(Request.QueryString["rename"]) &&
                !string.IsNullOrEmpty(Request.QueryString["title"]))
            {
                int renameId;
                if (int.TryParse(Request.QueryString["rename"], out renameId))
                {
                    if (ConversationBelongsToUser(renameId))
                    {
                        RenameConversation(renameId, Request.QueryString["title"]);
                    }
                }

                Response.Redirect("Chatbot.aspx?cid=" + renameId);
                return;
            }

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["cid"]))
                {
                    int requestedId;
                    if (int.TryParse(Request.QueryString["cid"], out requestedId)
                        && ConversationBelongsToUser(requestedId))
                    {
                        Session["ConversationID"] = requestedId;
                    }
                }

                LoadConversationList();
                LoadMessagesFromDatabase();
            }
        }

        private int CreateNewConversation()
        {
            int userId = Convert.ToInt32(Session["userid"]); // assumes login session

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO ChatConversation (userid, title)
                         OUTPUT INSERTED.conversationid
                         VALUES (@userid, @title)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@title", "New Chat");

                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        private void SaveMessage(int conversationId, string role, string content)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO ChatMessage (conversationid, role, content)
                         VALUES (@conversationid, @role, @content)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);
                cmd.Parameters.AddWithValue("@role", role);
                cmd.Parameters.AddWithValue("@content", content);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private List<object> GetConversationHistory(int conversationId)
        {
            List<object> messages = new List<object>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT role, content
                         FROM ChatMessage
                         WHERE conversationid = @conversationid
                         ORDER BY creationtime";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    messages.Add(new
                    {
                        role = reader["role"].ToString(),
                        content = reader["content"].ToString()
                    });
                }
            }

            return messages;
        }

        private async Task<string> GetDeepSeekResponse(int conversationId)
        {
            string apiKey = ConfigurationManager.AppSettings["OpenRouterApiKey"];

            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Remove("HTTP-Referer");
            client.DefaultRequestHeaders.Remove("X-Title");
            client.DefaultRequestHeaders.Remove("Accept");

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
            client.DefaultRequestHeaders.Add("HTTP-Referer", "https://localhost");
            client.DefaultRequestHeaders.Add("X-Title", "LearnSphereChatbot");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var history = GetConversationHistory(conversationId);

            var requestBody = new
            {
                model = "deepseek/deepseek-r1",
                messages = history
            };

            string json = new JavaScriptSerializer().Serialize(requestBody);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                content
            );

            string responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return "API Error: " + responseText;

            dynamic parsed = new JavaScriptSerializer()
                .DeserializeObject(responseText);

            return parsed["choices"][0]["message"]["content"];
        }

        protected async void btnSend_Click1(object sender, EventArgs e)
        {
            string userInput = txtQuestion.Text.Trim();

            if (string.IsNullOrEmpty(userInput))
                return;

            int conversationId;

            // 🔥 CREATE CONVERSATION ONLY IF NONE EXISTS
            if (Session["ConversationID"] == null ||
                !ConversationExists(Convert.ToInt32(Session["ConversationID"])))
            {
                conversationId = CreateNewConversation();
                Session["ConversationID"] = conversationId;
                LoadConversationList(); // refresh sidebar
            }
            else
            {
                conversationId = Convert.ToInt32(Session["ConversationID"]);
            }

            bool isFirstMessage = IsFirstMessage(conversationId);

            SaveMessage(conversationId, "user", userInput);

            if (isFirstMessage)
            {
                UpdateConversationTitle(conversationId, userInput);
                LoadConversationList();
            }

            chatOutput.InnerHtml +=
                $"<div class='userMsg'><span>{HttpUtility.HtmlEncode(userInput)}</span></div>";

            string botReply = await GetDeepSeekResponse(conversationId);

            SaveMessage(conversationId, "assistant", botReply);

            chatOutput.InnerHtml +=
                $"<div class='botMsg'><span class='markdown-content'>{HttpUtility.HtmlEncode(botReply)}</span></div>";

            txtQuestion.Text = "";
        }

        private bool IsFirstMessage(int conversationId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) FROM ChatMessage
                         WHERE conversationid = @conversationid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);

                con.Open();
                int count = (int)cmd.ExecuteScalar();

                return count == 0;
            }
        }

        protected void btnNewChat_Click(object sender, EventArgs e)
        {
            if (Session["ConversationID"] != null)
            {
                int oldId = Convert.ToInt32(Session["ConversationID"]);

                if (IsConversationEmpty(oldId))
                {
                    DeleteConversation(oldId);
                }
            }

            Session["ConversationID"] = CreateNewConversation();

            Response.Redirect("Chatbot.aspx");
        }

        private bool ConversationExists(int conversationId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) 
                         FROM ChatConversation
                         WHERE conversationid = @conversationid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);

                con.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }

        private bool IsConversationEmpty(int conversationId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) 
                         FROM ChatMessage
                         WHERE conversationid = @conversationid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);

                con.Open();
                int count = (int)cmd.ExecuteScalar();

                return count == 0;
            }
        }

        private void DeleteConversation(int conversationId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd1 = new SqlCommand(
                    "DELETE FROM ChatMessage WHERE conversationid = @id", con);
                cmd1.Parameters.AddWithValue("@id", conversationId);
                cmd1.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand(
                    "DELETE FROM ChatConversation WHERE conversationid = @id", con);
                cmd2.Parameters.AddWithValue("@id", conversationId);
                cmd2.ExecuteNonQuery();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Session["usertype"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string userType = Session["usertype"].ToString();

            switch (userType)
            {
                case "Lecturer":
                    Response.Redirect("~/Lecturer/LecturerDashboard.aspx");
                    break;

                case "Student":
                    Response.Redirect("~/Student/StudentDashboard.aspx");
                    break;

                case "Admin":
                    Response.Redirect("~/Admin/AdminDashboard.aspx");
                    break;

                default:
                    Response.Redirect("~/Login.aspx");
                    break;
            }
        }

        private void UpdateConversationTitle(int conversationId, string firstMessage)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string shortTitle = firstMessage.Length > 50
                    ? firstMessage.Substring(0, 50) + "..."
                    : firstMessage;

                string query = @"UPDATE ChatConversation
                         SET title = @title
                         WHERE conversationid = @conversationid
                         AND title = 'New Chat'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@title", shortTitle);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void LoadConversationList()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT conversationid, title
                         FROM ChatConversation
                         WHERE userid = @userid
                         ORDER BY creationtime DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userid", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                StringBuilder sb = new StringBuilder();

                while (reader.Read())
                {
                    int convId = Convert.ToInt32(reader["conversationid"]);
                    string title = reader["title"].ToString();

                    bool isActive = Session["ConversationID"] != null &&
                                    Convert.ToInt32(Session["ConversationID"]) == convId;

                    string activeClass = isActive ? "active-chat" : "";

                    sb.Append($@"
            <div class='conversation-item {activeClass}'>
                <div class='conversation-title'
                     onclick='window.location.href=""Chatbot.aspx?cid={convId}""'>
                    {HttpUtility.HtmlEncode(title)}
                </div>

                <div class='conversation-actions'>
                    <span class='rename-btn'
                          onclick='event.stopPropagation();
                          let newTitle = prompt(""Rename conversation:"");
                          if(newTitle){{
                              window.location.href=""Chatbot.aspx?rename={convId}&title="" + encodeURIComponent(newTitle);
                          }}'>
                        +
                    </span>

                    <span class='delete-btn'
                          onclick='event.stopPropagation();
                          if(confirm(""Delete this conversation?"")){{
                              window.location.href=""Chatbot.aspx?delete={convId}"";
                          }}'>
                        -
                    </span>
                </div>
            </div>");
                }

                sidebarConversations.InnerHtml = sb.ToString();
            }
        }

        private void LoadMessagesFromDatabase()
        {
            if (Session["ConversationID"] == null)
                return;

            int conversationId = Convert.ToInt32(Session["ConversationID"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT role, content
                         FROM ChatMessage
                         WHERE conversationid = @conversationid
                         ORDER BY creationtime";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                StringBuilder sb = new StringBuilder();

                while (reader.Read())
                {
                    string role = reader["role"].ToString();
                    string content = reader["content"].ToString();

                    if (role == "user")
                    {
                        sb.Append($"<div class='userMsg'><span>{HttpUtility.HtmlEncode(content)}</span></div>");
                    }
                    else
                    {
                        sb.Append($"<div class='botMsg'><span>{HttpUtility.HtmlEncode(content)}</span></div>");
                    }
                }

                chatOutput.InnerHtml = sb.ToString();
            }
        }

        private bool ConversationBelongsToUser(int conversationId)
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) 
                         FROM ChatConversation
                         WHERE conversationid = @conversationid
                         AND userid = @userid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);
                cmd.Parameters.AddWithValue("@userid", userId);

                con.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }

        private void RenameConversation(int conversationId, string newTitle)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE ChatConversation
                         SET title = @title
                         WHERE conversationid = @conversationid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@title", newTitle);
                cmd.Parameters.AddWithValue("@conversationid", conversationId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}