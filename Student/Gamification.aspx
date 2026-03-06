<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Gamification.aspx.cs" Inherits="LearnSphere_WAPP.Student.Gamification" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Achievements - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=Sora:wght@300;400;600;700&family=JetBrains+Mono:wght@400;600&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #0b0f1a; --surface: #111827; --surface2: #1a2235;
            --accent: #e94560; --accent2: #f97316; --gold: #f59e0b;
            --text: #e8eaf0; --muted: #6b7280; --border: rgba(255,255,255,0.07);
        }
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body { font-family: 'Sora', sans-serif; background: var(--bg); color: var(--text); min-height: 100vh; }
        body::before {
            content: ''; position: fixed; top: -50%; left: -50%; width: 200%; height: 200%;
            background: radial-gradient(ellipse 600px 400px at 20% 20%, rgba(233,69,96,0.06) 0%, transparent 60%),
                        radial-gradient(ellipse 500px 300px at 80% 80%, rgba(249,115,22,0.05) 0%, transparent 60%);
            z-index: 0; pointer-events: none;
        }
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(11,15,26,0.85); backdrop-filter: blur(16px);
            border-bottom: 1px solid var(--border);
            padding: 0 40px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 10px; }
        .logo-icon { width: 32px; height: 32px; background: linear-gradient(135deg, var(--accent), var(--accent2)); border-radius: 8px; display: flex; align-items: center; justify-content: center; font-size: 16px; font-weight: 700; color: white; }
        .logo-text { font-size: 1.2rem; font-weight: 700; letter-spacing: -0.5px; }
        .logo-text span { color: var(--accent); }
        .header-right { display: flex; align-items: center; gap: 16px; }
        .user-pill { display: flex; align-items: center; gap: 10px; background: var(--surface2); border: 1px solid var(--border); border-radius: 50px; padding: 6px 16px 6px 6px; }
        .user-avatar { width: 30px; height: 30px; background: linear-gradient(135deg, var(--accent), var(--accent2)); border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 13px; font-weight: 700; color: white; }
        .user-name { font-size: 0.85rem; font-weight: 600; }
        .btn-logout { background: transparent; border: 1px solid rgba(233,69,96,0.4); color: var(--accent); padding: 7px 18px; border-radius: 8px; font-family: 'Sora', sans-serif; font-size: 0.82rem; font-weight: 600; cursor: pointer; transition: all 0.2s; }
        .btn-logout:hover { background: var(--accent); color: white; }
        .nav { background: var(--surface); border-bottom: 1px solid var(--border); padding: 0 40px; display: flex; gap: 4px; position: relative; z-index: 1; }
        .nav a { color: var(--muted); text-decoration: none; padding: 14px 18px; font-size: 0.875rem; font-weight: 500; display: flex; align-items: center; gap: 8px; border-bottom: 2px solid transparent; transition: all 0.2s; }
        .nav a:hover { color: var(--text); }
        .nav a.active { color: var(--accent); border-bottom-color: var(--accent); }
        .container { max-width: 1200px; margin: 0 auto; padding: 36px 40px; position: relative; z-index: 1; }
        .page-header { margin-bottom: 32px; animation: fadeUp 0.4s ease both; }
        .page-label { font-size: 0.75rem; font-weight: 600; letter-spacing: 2px; text-transform: uppercase; color: var(--accent); margin-bottom: 8px; font-family: 'JetBrains Mono', monospace; }
        .page-title { font-size: 1.8rem; font-weight: 700; letter-spacing: -0.5px; }

        /* HERO CARD */
        .hero-card {
            background: linear-gradient(135deg, #1a0a10 0%, #1a1a2e 50%, #0f1a2e 100%);
            border: 1px solid var(--border); border-radius: 20px; padding: 40px;
            margin-bottom: 24px; text-align: center; position: relative; overflow: hidden;
            animation: fadeUp 0.5s ease both;
        }
        .hero-card::before {
            content: ''; position: absolute; inset: 0;
            background: radial-gradient(ellipse at center, rgba(233,69,96,0.08), transparent 70%);
            pointer-events: none;
        }
        .badge-emoji { font-size: 5rem; margin-bottom: 16px; display: block; animation: pulse 2s ease infinite; }
        @keyframes pulse { 0%,100% { transform: scale(1); } 50% { transform: scale(1.08); } }
        .points-display { font-size: 4rem; font-weight: 700; font-family: 'JetBrains Mono', monospace; color: var(--accent); line-height: 1; margin-bottom: 8px; }
        .points-label { color: var(--muted); font-size: 0.875rem; text-transform: uppercase; letter-spacing: 1.5px; margin-bottom: 16px; }
        .badge-name-pill {
            display: inline-block; padding: 6px 20px; border-radius: 20px;
            font-size: 0.85rem; font-weight: 700; letter-spacing: 1px; text-transform: uppercase;
            font-family: 'JetBrains Mono', monospace;
        }
        .badge-bronze { background: rgba(205,127,50,0.15); border: 1px solid #cd7f32; color: #cd7f32; }
        .badge-silver { background: rgba(192,192,192,0.15); border: 1px solid #c0c0c0; color: #c0c0c0; }
        .badge-gold   { background: rgba(255,215,0,0.15);   border: 1px solid #ffd700; color: #ffd700; }
        .badge-diamond{ background: rgba(185,242,255,0.15); border: 1px solid #b9f2ff; color: #b9f2ff; }

        /* PROGRESS */
        .progress-card {
            background: var(--surface); border: 1px solid var(--border); border-radius: 16px; padding: 28px;
            margin-bottom: 24px; animation: fadeUp 0.5s 0.1s ease both;
        }
        .progress-card-title { font-size: 1rem; font-weight: 700; margin-bottom: 20px; display: flex; align-items: center; gap: 10px; }
        .progress-card-title::before { content: ''; width: 4px; height: 18px; background: linear-gradient(to bottom, var(--accent), var(--accent2)); border-radius: 2px; }
        .badge-milestones { display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; margin-bottom: 24px; }
        .milestone {
            background: var(--surface2); border: 1px solid var(--border); border-radius: 12px;
            padding: 16px; text-align: center; transition: border-color 0.2s;
        }
        .milestone.achieved { border-color: rgba(233,69,96,0.4); background: rgba(233,69,96,0.05); }
        .milestone-emoji { font-size: 2rem; margin-bottom: 8px; }
        .milestone-name { font-size: 0.75rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 4px; }
        .milestone-pts { font-size: 0.7rem; color: var(--muted); font-family: 'JetBrains Mono', monospace; }
        .milestone.achieved .milestone-name { color: var(--accent); }

        .next-badge-label { font-size: 0.8rem; color: var(--muted); margin-bottom: 10px; }
        .next-badge-label span { color: var(--text); font-weight: 600; }
        .progress-bar-bg { width: 100%; height: 10px; background: var(--surface2); border-radius: 10px; overflow: hidden; margin-bottom: 8px; }
        .progress-bar-fill { height: 100%; background: linear-gradient(90deg, var(--accent), var(--accent2)); border-radius: 10px; transition: width 1s ease; }
        .progress-pct { font-size: 0.75rem; color: var(--muted); text-align: right; font-family: 'JetBrains Mono', monospace; }

        /* LEADERBOARD */
        .leaderboard-card {
            background: var(--surface); border: 1px solid var(--border); border-radius: 16px;
            overflow: hidden; animation: fadeUp 0.5s 0.2s ease both;
        }
        .leaderboard-header { padding: 20px 28px; border-bottom: 1px solid var(--border); display: flex; align-items: center; gap: 10px; font-size: 1rem; font-weight: 700; }
        .leaderboard-header::before { content: ''; width: 4px; height: 18px; background: linear-gradient(to bottom, var(--gold), var(--accent2)); border-radius: 2px; }
        .leaderboard-row { display: grid; grid-template-columns: 60px 1fr 120px 100px; align-items: center; padding: 14px 28px; border-bottom: 1px solid var(--border); transition: background 0.2s; }
        .leaderboard-row:last-child { border-bottom: none; }
        .leaderboard-row:hover { background: rgba(255,255,255,0.02); }
        .leaderboard-row.header-row { background: var(--surface2); font-size: 0.72rem; font-weight: 600; letter-spacing: 1.5px; text-transform: uppercase; color: var(--muted); }
        .leaderboard-row.top-1 .rank { color: #ffd700; }
        .leaderboard-row.top-2 .rank { color: #c0c0c0; }
        .leaderboard-row.top-3 .rank { color: #cd7f32; }
        .leaderboard-row.is-me { background: rgba(233,69,96,0.06); border-left: 3px solid var(--accent); }
        .rank { font-family: 'JetBrains Mono', monospace; font-weight: 700; font-size: 1rem; }
        .student-name { font-size: 0.875rem; font-weight: 500; }
        .student-points { font-family: 'JetBrains Mono', monospace; font-size: 0.875rem; color: var(--accent); font-weight: 600; }
        .student-badge { font-size: 0.75rem; }

        .empty-leaderboard { text-align: center; padding: 40px; color: var(--muted); font-size: 0.875rem; }

        @keyframes fadeUp { from { opacity: 0; transform: translateY(16px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 900px) { .container { padding: 24px 20px; } .header, .nav { padding: 0 20px; } .badge-milestones { grid-template-columns: repeat(2,1fr); } .leaderboard-row { grid-template-columns: 50px 1fr 80px 80px; padding: 12px 16px; } }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" style="height:40px;width:40px;object-fit:contain;" />
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
            <a href="StudentDashboard.aspx"><span>⊞</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>◎</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span>▤</span> My Courses</a>
            <a href="Gamification.aspx" class="active"><span>◆</span> Achievements</a>
        </div>

        <div class="container">
            <div class="page-header">
                <div class="page-label">Gamification</div>
                <div class="page-title">My Achievements</div>
            </div>

            <!-- HERO -->
            <div class="hero-card">
                <span class="badge-emoji"><asp:Label ID="lblBadgeEmoji" runat="server" Text="🥉" /></span>
                <div class="points-display"><asp:Label ID="lblPoints" runat="server" Text="0" /></div>
                <div class="points-label">Total Points</div>
                <span class="badge-name-pill badge-bronze"><asp:Label ID="lblBadgeName" runat="server" Text="Bronze" /></span>
            </div>

            <!-- PROGRESS -->
            <div class="progress-card">
                <div class="progress-card-title">Badge Milestones</div>
                <div class="badge-milestones">
                    <div class="milestone <asp:Literal ID='litBronzeClass' runat='server' />">
                        <div class="milestone-emoji">🥉</div>
                        <div class="milestone-name" style="color:#cd7f32;">Bronze</div>
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

            <!-- LEADERBOARD -->
            <div class="leaderboard-card">
                <div class="leaderboard-header">🏆 Top Learners Leaderboard</div>
                <div class="leaderboard-row header-row">
                    <div class="rank">Rank</div>
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
                                <%# Eval("userid").ToString() == Session["userid"].ToString() ? " <span style='color:var(--accent);font-size:0.72rem;'>(You)</span>" : "" %>
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