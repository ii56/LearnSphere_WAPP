using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Chatbot
{
    public partial class ChatbotAPI : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();

            // Allow external requests (Botpress)
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            Response.ContentType = "application/json";

            try
            {
                string question = Request.QueryString["q"];

                if (string.IsNullOrEmpty(question))
                {
                    WriteResponse("No question provided.");
                    return;
                }

                string answer = GetAnswer(question);

                WriteResponse(answer);
            }
            catch (Exception ex)
            {
                WriteResponse("Error: " + ex.Message);
            }
        }

        private string GetAnswer(string question)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT TOP 1 answer
                FROM ChatbotKnowledge
                WHERE question LIKE '%' + @question + '%'
                AND isActive = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@question", question);

                conn.Open();

                object result = cmd.ExecuteScalar();

                if (result != null)
                    return result.ToString();
                else
                    return "Sorry, I could not find an answer to that question.";
            }
        }

        private void WriteResponse(string answer)
        {
            var result = new
            {
                answer = answer
            };

            JavaScriptSerializer js = new JavaScriptSerializer();

            Response.Write(js.Serialize(result));
            Response.Flush();

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}