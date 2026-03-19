using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class Gamification : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null) { Response.Redirect("~/Login.aspx"); return; }
            if (!IsPostBack)
            {
                LoadGamificationData();
                LoadLeaderboard();
            }
        }

        private void LoadGamificationData()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // get first name for header
                string displayName = "Student";
                if (Session["fname"] != null && Session["fname"].ToString() != "")
                {
                    displayName = Session["fname"].ToString();
                }
                else
                {
                    using (SqlCommand nameCmd = new SqlCommand("SELECT fname FROM [User] WHERE userid = @uid", con))
                    {
                        nameCmd.Parameters.AddWithValue("@uid", userId);
                        object result = nameCmd.ExecuteScalar();
                        if (result != null) { displayName = result.ToString(); Session["fname"] = displayName; }
                    }
                }
                lblHeaderName.Text = displayName;
                lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

                // get points and badge
                int points = 0;
                string badge = "Bronze";

                using (SqlCommand cmd = new SqlCommand("SELECT totalpoints, badge FROM StudentPoints WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            points = Convert.ToInt32(reader["totalpoints"]);
                            badge = reader["badge"] != null ? reader["badge"].ToString() : "Bronze";
                        }
                    }
                }

                lblPoints.Text = points.ToString();
                lblBadgeName.Text = badge;

                // set badge emoji
                string emoji = "🥉";
                switch (badge)
                {
                    case "Silver": emoji = "🥈"; break;
                    case "Gold": emoji = "🥇"; break;
                    case "Diamond": emoji = "💎"; break;
                }
                lblBadgeEmoji.Text = emoji;

                // highlight achieved milestones
                if (points >= 100) milestoneSilver.Attributes["class"] = "milestone achieved";
                if (points >= 300) milestoneGold.Attributes["class"] = "milestone achieved";
                if (points >= 600) milestoneDiamond.Attributes["class"] = "milestone achieved";

                // calculate progress to next badge
                int nextThreshold = 0;
                string nextBadge = "";
                int prevThreshold = 0;

                if (points < 100) { nextThreshold = 100; nextBadge = "Silver"; prevThreshold = 0; }
                else if (points < 300) { nextThreshold = 300; nextBadge = "Gold"; prevThreshold = 100; }
                else if (points < 600) { nextThreshold = 600; nextBadge = "Diamond"; prevThreshold = 300; }

                if (nextThreshold > 0)
                {
                    int range = nextThreshold - prevThreshold;
                    int current = points - prevThreshold;
                    int progress = (current * 100) / range;
                    progressFill.Style["width"] = progress + "%";
                    lblProgressPct.Text = progress + "%";
                    lblNextBadge.Text = "Next: <span>" + nextBadge + " Badge</span> — " + points + " / " + nextThreshold + " points";
                }
                else
                {
                    progressFill.Style["width"] = "100%";
                    lblProgressPct.Text = "100%";
                    lblNextBadge.Text = "You've reached the highest badge — <span>Diamond!</span>";
                }
            }
        }

        private void LoadLeaderboard()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"
                    SELECT TOP 10
                        ROW_NUMBER() OVER (ORDER BY sp.totalpoints DESC) AS Rank,
                        u.userid, u.uname, sp.totalpoints, sp.badge
                    FROM StudentPoints sp
                    INNER JOIN [User] u ON sp.userid = u.userid
                    WHERE u.usertype = 'Student'
                    ORDER BY sp.totalpoints DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        rptLeaderboard.DataSource = dt;
                        rptLeaderboard.DataBind();
                        pnlEmptyLeaderboard.Visible = false;
                    }
                    else
                    {
                        pnlEmptyLeaderboard.Visible = true;
                    }
                }
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