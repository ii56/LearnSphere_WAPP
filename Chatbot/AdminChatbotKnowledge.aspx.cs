using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminChatbotKnowledge : System.Web.UI.Page
    {

        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string role = Session["usertype"].ToString();

            if (role != "Admin" && role != "Lecturer" && role != "SuperAdmin")
            {
                Response.Redirect("~/Unauthorized.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadKnowledge();
                LoadRules();
            }
        }

        // ===============================
        // KNOWLEDGE FUNCTIONS
        // ===============================

        private void LoadKnowledge()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT k.knowledgeID,
                       k.question,
                       k.category,
                       k.isActive,
                       u.uname AS CreatedBy
                FROM ChatbotKnowledge k
                JOIN [User] u ON k.createdBy = u.userid
                ORDER BY k.knowledgeID DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                DataTable dt = new DataTable();

                da.Fill(dt);

                gvKnowledge.DataSource = dt;
                gvKnowledge.DataBind();
            }
        }

        protected void btnSaveKnowledge_Click(object sender, EventArgs e)
        {
            int userID = Convert.ToInt32(Session["userid"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {

                if (string.IsNullOrEmpty(hfKnowledgeID.Value))
                {
                    string query = @"INSERT INTO ChatbotKnowledge
                                    (question,answer,category,createdBy)
                                    VALUES
                                    (@question,@answer,@category,@createdBy)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@question", txtQuestion.Text.Trim());
                    cmd.Parameters.AddWithValue("@answer", txtAnswer.Text.Trim());
                    cmd.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
                    cmd.Parameters.AddWithValue("@createdBy", userID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                else
                {
                    string query = @"UPDATE ChatbotKnowledge
                                     SET question=@question,
                                         answer=@answer,
                                         category=@category
                                     WHERE knowledgeID=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@question", txtQuestion.Text.Trim());
                    cmd.Parameters.AddWithValue("@answer", txtAnswer.Text.Trim());
                    cmd.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(hfKnowledgeID.Value));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }

            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Save Chatbot Knowledge");
            ClearKnowledgeForm();
            LoadKnowledge();
        }

        protected void gvKnowledge_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);

            int id = Convert.ToInt32(gvKnowledge.Rows[index].Cells[0].Text);

            if (e.CommandName == "DeleteKnowledge")
            {
                DeleteKnowledge(id);
            }

            if (e.CommandName == "EditKnowledge")
            {
                LoadEditKnowledge(id);
            }

            if (e.CommandName == "ToggleKnowledge")
            {
                ToggleKnowledge(id);
            }
        }

        private void DeleteKnowledge(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM ChatbotKnowledge WHERE knowledgeID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Deleted Chatbot Knowledge");
            }

            LoadKnowledge();
        }

        private void ToggleKnowledge(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"UPDATE ChatbotKnowledge
                                 SET isActive = CASE WHEN isActive=1 THEN 0 ELSE 1 END
                                 WHERE knowledgeID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Toggle Chatbot Knowledge");
            }

            LoadKnowledge();
        }

        private void LoadEditKnowledge(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM ChatbotKnowledge WHERE knowledgeID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    hfKnowledgeID.Value = reader["knowledgeID"].ToString();
                    txtQuestion.Text = reader["question"].ToString();
                    txtAnswer.Text = reader["answer"].ToString();
                    txtCategory.Text = reader["category"].ToString();

                    btnSaveKnowledge.Text = "Update Knowledge";
                }

                conn.Close();
            }
        }

        private void ClearKnowledgeForm()
        {
            txtQuestion.Text = "";
            txtAnswer.Text = "";
            txtCategory.Text = "";
            hfKnowledgeID.Value = "";
            btnSaveKnowledge.Text = "Add Knowledge";
        }

        // ===============================
        // RULE FUNCTIONS
        // ===============================

        private void LoadRules()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT r.ruleID,
                       r.ruleName,
                       r.isActive,
                       u.uname AS CreatedBy
                FROM ChatbotRules r
                JOIN [User] u ON r.createdBy = u.userid
                ORDER BY r.ruleID DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                DataTable dt = new DataTable();

                da.Fill(dt);

                gvRules.DataSource = dt;
                gvRules.DataBind();
            }
        }

        protected void btnSaveRule_Click(object sender, EventArgs e)
        {
            int userID = Convert.ToInt32(Session["userid"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {

                if (string.IsNullOrEmpty(hfRuleID.Value))
                {
                    string query = @"INSERT INTO ChatbotRules
                                    (ruleName,ruleDescription,ruleContent,createdBy)
                                    VALUES
                                    (@ruleName,@ruleDescription,@ruleContent,@createdBy)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@ruleName", txtRuleName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ruleDescription", txtRuleDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@ruleContent", txtRuleContent.Text.Trim());
                    cmd.Parameters.AddWithValue("@createdBy", userID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                else
                {
                    string query = @"UPDATE ChatbotRules
                                     SET ruleName=@ruleName,
                                         ruleDescription=@ruleDescription,
                                         ruleContent=@ruleContent
                                     WHERE ruleID=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@ruleName", txtRuleName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ruleDescription", txtRuleDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@ruleContent", txtRuleContent.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(hfRuleID.Value));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Update Chatbot Rule");

            }

            ClearRuleForm();
            LoadRules();
        }

        protected void gvRules_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);

            int id = Convert.ToInt32(gvRules.Rows[index].Cells[0].Text);

            if (e.CommandName == "DeleteRule")
            {
                DeleteRule(id);
            }

            if (e.CommandName == "EditRule")
            {
                LoadEditRule(id);
            }

            if (e.CommandName == "ToggleRule")
            {
                ToggleRule(id);
            }
        }

        private void DeleteRule(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM ChatbotRules WHERE ruleID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Deleted Chatbot Rule");
            LoadRules();
        }

        private void ToggleRule(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"UPDATE ChatbotRules
                                 SET isActive = CASE WHEN isActive=1 THEN 0 ELSE 1 END
                                 WHERE ruleID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Toggle Chatbot Rule");
            }

            LoadRules();
        }

        private void LoadEditRule(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM ChatbotRules WHERE ruleID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    hfRuleID.Value = reader["ruleID"].ToString();
                    txtRuleName.Text = reader["ruleName"].ToString();
                    txtRuleDescription.Text = reader["ruleDescription"].ToString();
                    txtRuleContent.Text = reader["ruleContent"].ToString();

                    btnSaveRule.Text = "Update Rule";
                }

                conn.Close();
            }
        }

        private void ClearRuleForm()
        {
            txtRuleName.Text = "";
            txtRuleDescription.Text = "";
            txtRuleContent.Text = "";
            hfRuleID.Value = "";
            btnSaveRule.Text = "Add Rule";
        }

    }
}