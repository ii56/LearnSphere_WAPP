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

        // Restricts access to Admins, SuperAdmins and Lecturers only, then loads both grids
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

        // Loads all knowledge entries joined with the creator's username, newest first
        private void LoadKnowledge()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT k.knowledgeID, k.question, k.category, k.isActive,
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

        // Inserts a new knowledge entry or updates an existing one depending on whether hfKnowledgeID is set
        protected void btnSaveKnowledge_Click(object sender, EventArgs e)
        {
            int userID = Convert.ToInt32(Session["userid"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (string.IsNullOrEmpty(hfKnowledgeID.Value))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ChatbotKnowledge (question, answer, category, createdBy)
                        VALUES (@question, @answer, @category, @createdBy)", conn);
                    cmd.Parameters.AddWithValue("@question", txtQuestion.Text.Trim());
                    cmd.Parameters.AddWithValue("@answer", txtAnswer.Text.Trim());
                    cmd.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
                    cmd.Parameters.AddWithValue("@createdBy", userID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE ChatbotKnowledge
                        SET question=@question, answer=@answer, category=@category
                        WHERE knowledgeID=@id", conn);
                    cmd.Parameters.AddWithValue("@question", txtQuestion.Text.Trim());
                    cmd.Parameters.AddWithValue("@answer", txtAnswer.Text.Trim());
                    cmd.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(hfKnowledgeID.Value));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Save Chatbot Knowledge");
            ClearKnowledgeForm();
            LoadKnowledge();
        }

        // Routes Edit, Toggle and Delete commands from the knowledge grid to the correct handler
        protected void gvKnowledge_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int id = Convert.ToInt32(gvKnowledge.Rows[index].Cells[0].Text);

            if (e.CommandName == "DeleteKnowledge") DeleteKnowledge(id);
            if (e.CommandName == "EditKnowledge") LoadEditKnowledge(id);
            if (e.CommandName == "ToggleKnowledge") ToggleKnowledge(id);
        }

        // Permanently deletes a knowledge entry by ID
        private void DeleteKnowledge(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM ChatbotKnowledge WHERE knowledgeID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Deleted Chatbot Knowledge");
            LoadKnowledge();
        }

        // Flips the isActive flag — 1 becomes 0 and 0 becomes 1
        private void ToggleKnowledge(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE ChatbotKnowledge
                    SET isActive = CASE WHEN isActive=1 THEN 0 ELSE 1 END
                    WHERE knowledgeID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Toggle Chatbot Knowledge");
            LoadKnowledge();
        }

        // Fills the form with the selected knowledge entry's values ready for editing
        private void LoadEditKnowledge(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM ChatbotKnowledge WHERE knowledgeID=@id", conn);
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
            }
        }

        // Resets the knowledge form back to Add mode
        private void ClearKnowledgeForm()
        {
            txtQuestion.Text = "";
            txtAnswer.Text = "";
            txtCategory.Text = "";
            hfKnowledgeID.Value = "";
            btnSaveKnowledge.Text = "Add Knowledge";
        }

        // Loads all rules joined with the creator's username, newest first
        private void LoadRules()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT r.ruleID, r.ruleName, r.isActive,
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

        // Inserts a new rule or updates an existing one depending on whether hfRuleID is set
        protected void btnSaveRule_Click(object sender, EventArgs e)
        {
            int userID = Convert.ToInt32(Session["userid"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (string.IsNullOrEmpty(hfRuleID.Value))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ChatbotRules (ruleName, ruleDescription, ruleContent, createdBy)
                        VALUES (@ruleName, @ruleDescription, @ruleContent, @createdBy)", conn);
                    cmd.Parameters.AddWithValue("@ruleName", txtRuleName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ruleDescription", txtRuleDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@ruleContent", txtRuleContent.Text.Trim());
                    cmd.Parameters.AddWithValue("@createdBy", userID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE ChatbotRules
                        SET ruleName=@ruleName, ruleDescription=@ruleDescription, ruleContent=@ruleContent
                        WHERE ruleID=@id", conn);
                    cmd.Parameters.AddWithValue("@ruleName", txtRuleName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ruleDescription", txtRuleDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@ruleContent", txtRuleContent.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(hfRuleID.Value));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Update Chatbot Rule");
            ClearRuleForm();
            LoadRules();
        }

        // Routes Edit, Toggle and Delete commands from the rules grid to the correct handler
        protected void gvRules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int id = Convert.ToInt32(gvRules.Rows[index].Cells[0].Text);

            if (e.CommandName == "DeleteRule") DeleteRule(id);
            if (e.CommandName == "EditRule") LoadEditRule(id);
            if (e.CommandName == "ToggleRule") ToggleRule(id);
        }

        // Permanently deletes a rule by ID
        private void DeleteRule(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM ChatbotRules WHERE ruleID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Deleted Chatbot Rule");
            LoadRules();
        }

        // Flips the isActive flag — 1 becomes 0 and 0 becomes 1
        private void ToggleRule(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE ChatbotRules
                    SET isActive = CASE WHEN isActive=1 THEN 0 ELSE 1 END
                    WHERE ruleID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Toggle Chatbot Rule");
            LoadRules();
        }

        // Fills the form with the selected rule's values ready for editing
        private void LoadEditRule(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM ChatbotRules WHERE ruleID=@id", conn);
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
            }
        }

        // Resets the rules form back to Add mode
        private void ClearRuleForm()
        {
            txtRuleName.Text = "";
            txtRuleDescription.Text = "";
            txtRuleContent.Text = "";
            hfRuleID.Value = "";
            btnSaveRule.Text = "Add Rule";
        }

        protected void btnBack_Click(object sender, EventArgs e) { }
    }
}