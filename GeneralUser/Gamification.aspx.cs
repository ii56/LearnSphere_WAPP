using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class Gamification : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null || Session["usertype"].ToString() != "General")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadHeaderProfile();
                LoadGamificationData();
                LoadLeaderboard();
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
                            string imgPath = reader["ProfileImage"] != DBNull.Value ? reader["ProfileImage"].ToString() : "";

                            if (!string.IsNullOrEmpty(imgPath))
                            {
                                imgAvatar.ImageUrl = ResolveUrl(imgPath);
                                imgAvatar.Visible = true;
                                lblAvatarInitial.Visible = false;
                            }
                            else
                            {
                                imgAvatar.Visible = false;
                                lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();
                                lblAvatarInitial.Visible = true;
                            }
                        }
                    }
                }
                lblHeaderName.Text = displayName;
                Session["fname"] = displayName;
            }
        }

        private void LoadGamificationData()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

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
                            badge = reader["badge"] != DBNull.Value ? reader["badge"].ToString() : "Bronze";
                        }
                    }
                }

                // If record doesn't exist, create default
                if (points == 0 && badge == "Bronze")
                {
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM StudentPoints WHERE userid = @uid", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        if ((int)chk.ExecuteScalar() == 0)
                        {
                            using (SqlCommand ins = new SqlCommand("INSERT INTO StudentPoints (userid, totalpoints, badge, lastupdated) VALUES (@uid, 0, 'Bronze', GETDATE())", con))
                            {
                                ins.Parameters.AddWithValue("@uid", userId);
                                ins.ExecuteNonQuery();
                            }
                        }
                    }
                }

                lblPoints.Text = points.ToString();
                lblBadgeName.Text = badge;

                string emoji = "🥉";
                switch (badge)
                {
                    case "Silver": emoji = "🥈"; break;
                    case "Gold": emoji = "🥇"; break;
                    case "Diamond": emoji = "💎"; break;
                }
                lblBadgeEmoji.Text = emoji;

                // Highlight achieved milestones
                if (points >= 100) milestoneSilver.Attributes["class"] = "milestone achieved";
                if (points >= 300) milestoneGold.Attributes["class"] = "milestone achieved";
                if (points >= 600) milestoneDiamond.Attributes["class"] = "milestone achieved";

                // Calculate progress to next badge
                int nextThreshold = 0;
                string nextBadge = "";
                int prevThreshold = 0;

                if (points < 100) { nextThreshold = 100; nextBadge = "Silver"; prevThreshold = 0; }
                else if (points < 300) { nextThreshold = 300; nextBadge = "Gold"; prevThreshold = 100; }
                else if (points < 600) { nextThreshold = 600; nextBadge = "Diamond"; prevThreshold = 300; }

                if (nextThreshold > 0)
                {
                    int range = nextThreshold - prevThreshold;
                    int currentProgress = points - prevThreshold;
                    int progressPercent = (currentProgress * 100) / range;

                    progressFill.Style["width"] = progressPercent + "%";
                    lblProgressPct.Text = progressPercent + "%";
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
                // Leaderboard includes Students and General Users so everyone can compete
                string query = @"
                    SELECT TOP 10
                        ROW_NUMBER() OVER (ORDER BY sp.totalpoints DESC) AS Rank,
                        u.userid, u.uname, u.ProfileImage, sp.totalpoints, sp.badge
                    FROM StudentPoints sp
                    INNER JOIN [User] u ON sp.userid = u.userid
                    WHERE u.usertype IN ('Student', 'General', 'General User')
                    AND u.status = 'Active' AND u.deletiontime IS NULL
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
            LearnSphere_WAPP.Syslog.action(Convert.ToInt32(Session["userid"]), "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}