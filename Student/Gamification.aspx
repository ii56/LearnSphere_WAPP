<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Gamification.aspx.cs" Inherits="LearnSphere_WAPP.Student.Gamification" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Achievements - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --primary: #2563eb;
            --primary-bg: rgba(37,99,235,0.08);
            --accent-orange: #f59e0b;
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
            --text: #1e293b;
            --text-secondary: #64748b;
            --text-muted: #94a3b8;
            --border: #e2e8f0;
            --border-light: #f1f5f9;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04), 0 1px 2px rgba(0,0,0,0.03);
            --shadow-md: 0 4px 12px rgba(0,0,0,0.06), 0 2px 4px rgba(0,0,0,0.03);
            --radius: 14px;
            --radius-sm: 10px;
        }
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: 'DM Sans', sans-serif;
            background: var(--bg-gradient); background-attachment: fixed;
            color: var(--text); min-height: 100vh;
        }

        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82); backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 12px; }
        .logo img { height: 38px; width: 38px; object-fit: contain; }
        .logo-text { font-size: 1.2rem; font-weight: 700; color: var(--text); }
        .logo-text span { color: var(--primary); }
        .header-right { display: flex; align-items: center; gap: 14px; }
        .user-pill {
            display: flex; align-items: center; gap: 10px;
            background: var(--bg); border: 1px solid var(--border);
            border-radius: 50px; padding: 5px 16px 5px 5px;
        }
        .user-avatar {
            width: 32px; height: 32px;
            background: linear-gradient(135deg, var(--primary), var(--accent-purple));
            border-radius: 50%; display: flex; align-items: center; justify-content: center;
            font-size: 13px; font-weight: 700; color: white;
        }
        .user-name { font-size: 0.85rem; font-weight: 600; }
        .btn-logout {
            background: transparent; border: 1px solid var(--border);
            color: var(--text-secondary); padding: 7px 18px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-logout:hover { background: #fef2f2; color: #ef4444; border-color: #fecaca; }

        .nav {
            background: var(--surface); border-bottom: 1px solid var(--border);
            padding: 0 36px; display: flex; gap: 2px;
        }
        .nav a {
            color: var(--text-muted); text-decoration: none; padding: 14px 18px;
            font-size: 0.85rem; font-weight: 600; display: flex; align-items: center; gap: 8px;
            border-bottom: 2.5px solid transparent; transition: all 0.2s;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active { color: var(--primary); border-bottom-color: var(--primary); }

        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }
        .page-header { margin-bottom: 24px; animation: slideUp 0.4s ease both; }
        .page-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: var(--primary); margin-bottom: 6px;
            font-family: 'Space Mono', monospace;
        }
        .page-title { font-size: 1.6rem; font-weight: 700; }

        /* hero points card */
        .hero-card {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 50%, #60a5fa 100%);
            border-radius: var(--radius); padding: 36px; margin-bottom: 20px;
            text-align: center; position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(37,99,235,0.25);
            animation: slideUp 0.5s ease both;
        }
        .hero-card::before {
            content: ''; position: absolute; top: -40%; right: -10%;
            width: 250px; height: 250px;
            background: radial-gradient(circle, rgba(255,255,255,0.1), transparent 65%);
            border-radius: 50%;
        }
        .badge-emoji { font-size: 4rem; margin-bottom: 12px; display: block; }
        .points-display {
            font-size: 3.5rem; font-weight: 700; font-family: 'Space Mono', monospace;
            color: white; line-height: 1; margin-bottom: 6px;
        }
        .points-label { color: rgba(255,255,255,0.7); font-size: 0.82rem; text-transform: uppercase; letter-spacing: 1.5px; margin-bottom: 14px; }
        .badge-name-pill {
            display: inline-block; padding: 5px 18px; border-radius: 20px;
            font-size: 0.8rem; font-weight: 700; letter-spacing: 1px; text-transform: uppercase;
            font-family: 'Space Mono', monospace;
            background: rgba(255,255,255,0.2); color: white; border: 1px solid rgba(255,255,255,0.3);
        }

        /* milestones */
        .progress-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 24px;
            margin-bottom: 20px; box-shadow: var(--shadow-sm);
            animation: slideUp 0.5s 0.1s ease both;
        }
        .progress-card-title {
            font-size: 0.95rem; font-weight: 700; margin-bottom: 18px;
            display: flex; align-items: center; gap: 10px;
        }
        .title-dot { width: 8px; height: 8px; border-radius: 50%; background: var(--primary); }
        .badge-milestones { display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; margin-bottom: 20px; }
        .milestone {
            background: var(--border-light); border: 1px solid var(--border);
            border-radius: 12px; padding: 16px; text-align: center; transition: all 0.2s;
        }
        .milestone.achieved { border-color: rgba(37,99,235,0.3); background: var(--primary-bg); }
        .milestone-emoji { font-size: 1.8rem; margin-bottom: 6px; }
        .milestone-name { font-size: 0.72rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.8px; margin-bottom: 3px; color: var(--text-secondary); }
        .milestone-pts { font-size: 0.68rem; color: var(--text-muted); font-family: 'Space Mono', monospace; }
        .milestone.achieved .milestone-name { color: var(--primary); }

        .next-badge-label { font-size: 0.8rem; color: var(--text-secondary); margin-bottom: 8px; }
        .next-badge-label span { color: var(--text); font-weight: 600; }
        .progress-bar-bg { width: 100%; height: 8px; background: var(--border-light); border-radius: 10px; overflow: hidden; margin-bottom: 6px; }
        .progress-bar-fill { height: 100%; background: linear-gradient(90deg, var(--primary), var(--accent-purple)); border-radius: 10px; transition: width 1s ease; }
        .progress-pct { font-size: 0.72rem; color: var(--text-muted); text-align: right; font-family: 'Space Mono', monospace; }

        /* leaderboard */
        .leaderboard-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); overflow: hidden; box-shadow: var(--shadow-sm);
            animation: slideUp 0.5s 0.2s ease both;
        }
        .leaderboard-header {
            padding: 18px 24px; border-bottom: 1px solid var(--border);
            display: flex; align-items: center; gap: 10px;
            font-size: 0.95rem; font-weight: 700;
        }
        .leaderboard-row {
            display: grid; grid-template-columns: 55px 1fr 100px 90px;
            align-items: center; padding: 12px 24px;
            border-bottom: 1px solid var(--border-light); transition: background 0.2s;
        }
        .leaderboard-row:last-child { border-bottom: none; }
        .leaderboard-row:hover { background: var(--border-light); }
        .leaderboard-row.header-row {
            background: var(--border-light); font-size: 0.7rem; font-weight: 700;
            letter-spacing: 1px; text-transform: uppercase; color: var(--text-muted);
        }
        .leaderboard-row.is-me { background: var(--primary-bg); border-left: 3px solid var(--primary); }
        .rank { font-family: 'Space Mono', monospace; font-weight: 700; font-size: 0.95rem; }
        .leaderboard-row.top-1 .rank { color: #f59e0b; }
        .leaderboard-row.top-2 .rank { color: #94a3b8; }
        .leaderboard-row.top-3 .rank { color: #cd7f32; }
        .student-name { font-size: 0.85rem; font-weight: 500; }
        .student-points { font-family: 'Space Mono', monospace; font-size: 0.85rem; color: var(--primary); font-weight: 600; }
        .student-badge { font-size: 0.75rem; color: var(--text-secondary); }
        .empty-leaderboard { text-align: center; padding: 40px; color: var(--text-muted); font-size: 0.85rem; }

        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 900px) {
            .container { padding: 20px; } .header, .nav { padding: 0 20px; }
            .badge-milestones { grid-template-columns: repeat(2,1fr); }
            .leaderboard-row { grid-template-columns: 45px 1fr 70px 70px; padding: 10px 16px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <div class="user-pill">
                    <div class="user-avatar"><asp:Label ID="lblAvatarInitial" runat="server" Text="S" /></div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="nav">
            <a href="StudentDashboard.aspx"><span>📊</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>📚</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span>🎓</span> My Courses</a>
            <a href="Gamification.aspx" class="active"><span>🏆</span> Achievements</a>
            <a href="StudentProfile.aspx"><span>👤</span> Profile</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
        </div>

        <div class="container">
            <div class="page-header">
                <div class="page-label">Gamification</div>
                <div class="page-title">My Achievements</div>
            </div>

            <!-- points hero -->
            <div class="hero-card">
                <span class="badge-emoji"><asp:Label ID="lblBadgeEmoji" runat="server" Text="🥉" /></span>
                <div class="points-display"><asp:Label ID="lblPoints" runat="server" Text="0" /></div>
                <div class="points-label">Total Points</div>
                <span class="badge-name-pill"><asp:Label ID="lblBadgeName" runat="server" Text="Bronze" /></span>
            </div>

            <!-- milestones and progress -->
            <div class="progress-card">
                <div class="progress-card-title"><span class="title-dot"></span> Badge Milestones</div>
                <div class="badge-milestones">
                    <div id="milestoneBronze" runat="server" class="milestone achieved">
                        <div class="milestone-emoji">🥉</div>
                        <div class="milestone-name">Bronze</div>
                        <div class="milestone-pts">0 pts</div>
                    </div>
                    <div id="milestoneSilver" runat="server" class="milestone">
                        <div class="milestone-emoji">🥈</div>
                        <div class="milestone-name">Silver</div>
                        <div class="milestone-pts">100 pts</div>
                    </div>
                    <div id="milestoneGold" runat="server" class="milestone">
                        <div class="milestone-emoji">🥇</div>
                        <div class="milestone-name">Gold</div>
                        <div class="milestone-pts">300 pts</div>
                    </div>
                    <div id="milestoneDiamond" runat="server" class="milestone">
                        <div class="milestone-emoji">💎</div>
                        <div class="milestone-name">Diamond</div>
                        <div class="milestone-pts">600 pts</div>
                    </div>
                </div>
                <div class="next-badge-label"><asp:Label ID="lblNextBadge" runat="server" /></div>
                <div class="progress-bar-bg">
                    <div class="progress-bar-fill" id="progressFill" runat="server" style="width:0%"></div>
                </div>
                <div class="progress-pct"><asp:Label ID="lblProgressPct" runat="server" Text="0%" /></div>
            </div>

            <!-- leaderboard -->
            <div class="leaderboard-card">
                <div class="leaderboard-header"><span class="title-dot"></span> Top Learners Leaderboard</div>
                <div class="leaderboard-row header-row">
                    <div>Rank</div>
                    <div>Student</div>
                    <div>Points</div>
                    <div>Badge</div>
                </div>
                <asp:Repeater ID="rptLeaderboard" runat="server">
                    <ItemTemplate>
                        <div class='leaderboard-row <%# Convert.ToInt32(Eval("Rank")) == 1 ? "top-1" : Convert.ToInt32(Eval("Rank")) == 2 ? "top-2" : Convert.ToInt32(Eval("Rank")) == 3 ? "top-3" : "" %> <%# Eval("userid").ToString() == Session["userid"].ToString() ? "is-me" : "" %>'>
                            <div class="rank">
                                <%# Convert.ToInt32(Eval("Rank")) == 1 ? "🥇" : Convert.ToInt32(Eval("Rank")) == 2 ? "🥈" : Convert.ToInt32(Eval("Rank")) == 3 ? "🥉" : "#" + Eval("Rank") %>
                            </div>
                            <div class="student-name">
                                <%# Eval("uname") %>
                                <%# Eval("userid").ToString() == Session["userid"].ToString() ? " (You)" : "" %>
                            </div>
                            <div class="student-points"><%# Eval("totalpoints") %></div>
                            <div class="student-badge"><%# Eval("badge") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlEmptyLeaderboard" runat="server" Visible="false">
                    <div class="empty-leaderboard">No leaderboard data yet. Start earning points!</div>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
