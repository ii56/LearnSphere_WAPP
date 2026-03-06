<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentDashboard.aspx.cs" Inherits="LearnSphere_WAPP.Student.StudentDashboard" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Student Dashboard - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=Sora:wght@300;400;600;700&family=JetBrains+Mono:wght@400;600&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #0b0f1a;
            --surface: #111827;
            --surface2: #1a2235;
            --accent: #e94560;
            --accent2: #f97316;
            --gold: #f59e0b;
            --text: #e8eaf0;
            --muted: #6b7280;
            --border: rgba(255,255,255,0.07);
            --glow: rgba(233,69,96,0.25);
        }

        * { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'Sora', sans-serif;
            background: var(--bg);
            color: var(--text);
            min-height: 100vh;
        }

        body::before {
            content: '';
            position: fixed;
            top: -50%; left: -50%;
            width: 200%; height: 200%;
            background:
                radial-gradient(ellipse 600px 400px at 20% 20%, rgba(233,69,96,0.06) 0%, transparent 60%),
                radial-gradient(ellipse 500px 300px at 80% 80%, rgba(249,115,22,0.05) 0%, transparent 60%);
            z-index: 0;
            pointer-events: none;
        }

        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(11,15,26,0.85);
            backdrop-filter: blur(16px);
            border-bottom: 1px solid var(--border);
            padding: 0 40px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }

        .logo { display: flex; align-items: center; gap: 10px; }

        .logo-icon {
            width: 32px; height: 32px;
            background: linear-gradient(135deg, var(--accent), var(--accent2));
            border-radius: 8px;
            display: flex; align-items: center; justify-content: center;
            font-size: 16px; font-weight: 700; color: white;
        }

        .logo-text { font-size: 1.2rem; font-weight: 700; letter-spacing: -0.5px; }
        .logo-text span { color: var(--accent); }

        .header-right { display: flex; align-items: center; gap: 16px; }

        .user-pill {
            display: flex; align-items: center; gap: 10px;
            background: var(--surface2);
            border: 1px solid var(--border);
            border-radius: 50px;
            padding: 6px 16px 6px 6px;
        }

        .user-avatar {
            width: 30px; height: 30px;
            background: linear-gradient(135deg, var(--accent), var(--accent2));
            border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            font-size: 13px; font-weight: 700; color: white;
        }

        .user-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }

        .btn-logout {
            background: transparent;
            border: 1px solid rgba(233,69,96,0.4);
            color: var(--accent);
            padding: 7px 18px; border-radius: 8px;
            font-family: 'Sora', sans-serif;
            font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s; letter-spacing: 0.3px;
        }
        .btn-logout:hover { background: var(--accent); color: white; border-color: var(--accent); }

        .nav {
            background: var(--surface);
            border-bottom: 1px solid var(--border);
            padding: 0 40px;
            display: flex; gap: 4px;
            position: relative; z-index: 1;
        }

        .nav a {
            color: var(--muted); text-decoration: none;
            padding: 14px 18px;
            font-size: 0.875rem; font-weight: 500;
            display: flex; align-items: center; gap: 8px;
            border-bottom: 2px solid transparent;
            transition: all 0.2s;
        }
        .nav a:hover { color: var(--text); }
        .nav a.active { color: var(--accent); border-bottom-color: var(--accent); }

        .container {
            max-width: 1200px; margin: 0 auto;
            padding: 36px 40px;
            position: relative; z-index: 1;
        }

        .welcome-banner {
            background: linear-gradient(135deg, #1a0a10 0%, #1a1a2e 50%, #0f1a2e 100%);
            border: 1px solid var(--border);
            border-radius: 16px; padding: 32px 36px;
            margin-bottom: 32px;
            position: relative; overflow: hidden;
            animation: fadeUp 0.5s ease both;
        }

        .welcome-banner::before {
            content: '';
            position: absolute; top: 0; right: 0;
            width: 300px; height: 100%;
            background: radial-gradient(ellipse at right center, rgba(233,69,96,0.12), transparent 70%);
            pointer-events: none;
        }

        .welcome-banner::after {
            content: '◈';
            position: absolute; right: 36px; top: 50%;
            transform: translateY(-50%);
            font-size: 5rem; color: rgba(233,69,96,0.08);
            pointer-events: none;
        }

        .welcome-label {
            font-size: 0.75rem; font-weight: 600;
            letter-spacing: 2px; text-transform: uppercase;
            color: var(--accent); margin-bottom: 8px;
            font-family: 'JetBrains Mono', monospace;
        }

        .welcome-name {
            font-size: 2rem; font-weight: 700;
            letter-spacing: -0.5px; margin-bottom: 6px;
        }

        .welcome-sub { color: var(--muted); font-size: 0.9rem; }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 16px; margin-bottom: 32px;
        }

        .stat-card {
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: 14px; padding: 24px;
            position: relative; overflow: hidden;
            transition: transform 0.2s, border-color 0.2s;
            animation: fadeUp 0.5s ease both;
        }

        .stat-card:nth-child(1) { animation-delay: 0.1s; }
        .stat-card:nth-child(2) { animation-delay: 0.15s; }
        .stat-card:nth-child(3) { animation-delay: 0.2s; }
        .stat-card:nth-child(4) { animation-delay: 0.25s; }

        .stat-card:hover { transform: translateY(-2px); border-color: rgba(233,69,96,0.3); }

        .stat-card::before {
            content: '';
            position: absolute; top: 0; left: 0;
            width: 3px; height: 100%;
            background: linear-gradient(to bottom, var(--accent), transparent);
            border-radius: 14px 0 0 14px;
        }

        .stat-card:nth-child(2)::before { background: linear-gradient(to bottom, #3b82f6, transparent); }
        .stat-card:nth-child(3)::before { background: linear-gradient(to bottom, #10b981, transparent); }
        .stat-card:nth-child(4)::before { background: linear-gradient(to bottom, var(--gold), transparent); }

        .stat-icon { font-size: 1.5rem; margin-bottom: 12px; }

        .stat-value {
            font-size: 2.2rem; font-weight: 700;
            font-family: 'JetBrains Mono', monospace;
            letter-spacing: -1px; line-height: 1; margin-bottom: 6px;
        }

        .stat-card:nth-child(1) .stat-value { color: var(--accent); }
        .stat-card:nth-child(2) .stat-value { color: #3b82f6; }
        .stat-card:nth-child(3) .stat-value { color: #10b981; }
        .stat-card:nth-child(4) .stat-value { color: var(--gold); }

        .stat-label {
            font-size: 0.8rem; color: var(--muted);
            font-weight: 500; text-transform: uppercase; letter-spacing: 0.8px;
        }

        .badge-pill {
            display: inline-block;
            background: linear-gradient(135deg, #78350f, #92400e);
            border: 1px solid var(--gold);
            color: var(--gold);
            font-size: 0.7rem; font-weight: 700;
            letter-spacing: 1px; text-transform: uppercase;
            padding: 3px 10px; border-radius: 20px; margin-bottom: 6px;
            font-family: 'JetBrains Mono', monospace;
        }

        .section {
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: 16px; overflow: hidden;
            animation: fadeUp 0.5s 0.3s ease both;
        }

        .section-header {
            padding: 20px 28px;
            border-bottom: 1px solid var(--border);
            display: flex; align-items: center; justify-content: space-between;
        }

        .section-title {
            font-size: 1rem; font-weight: 700;
            display: flex; align-items: center; gap: 10px;
            letter-spacing: -0.3px;
        }

        .section-title::before {
            content: '';
            width: 4px; height: 18px;
            background: linear-gradient(to bottom, var(--accent), var(--accent2));
            border-radius: 2px;
        }

        .section-count {
            font-size: 0.75rem; color: var(--muted);
            background: var(--surface2);
            padding: 3px 10px; border-radius: 20px;
            font-family: 'JetBrains Mono', monospace;
        }

        .section table { width: 100%; border-collapse: collapse; }

        .section table th {
            background: var(--surface2);
            padding: 12px 28px; text-align: left;
            font-size: 0.72rem; font-weight: 600;
            letter-spacing: 1.5px; text-transform: uppercase;
            color: var(--muted); border-bottom: 1px solid var(--border);
        }

        .section table td {
            padding: 16px 28px; font-size: 0.875rem;
            color: var(--text); border-bottom: 1px solid var(--border);
        }

        .section table tr:last-child td { border-bottom: none; }
        .section table tr:hover td { background: rgba(255,255,255,0.02); }

        .empty-state {
            text-align: center; padding: 60px 20px; color: var(--muted);
        }
        .empty-state-icon { font-size: 3rem; margin-bottom: 12px; }
        .empty-state p { font-size: 0.9rem; }
        .empty-state a {
            display: inline-block; margin-top: 16px;
            background: var(--accent); color: white;
            text-decoration: none; padding: 10px 24px;
            border-radius: 8px; font-size: 0.85rem; font-weight: 600;
            transition: opacity 0.2s;
        }
        .empty-state a:hover { opacity: 0.85; }

        @keyframes fadeUp {
            from { opacity: 0; transform: translateY(16px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        @media (max-width: 900px) {
            .stats-grid { grid-template-columns: repeat(2, 1fr); }
            .container { padding: 24px 20px; }
            .header, .nav { padding: 0 20px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="header">
            <div class="logo">
<img src="~/LEARNSPHERE.png" runat="server" style="height:40px;width:40px;object-fit:contain;" />                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <div class="user-pill">
                    <div class="user-avatar">
                        <asp:Label ID="lblAvatarInitial" runat="server" Text="S" />
                    </div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="nav">
            <a href="StudentDashboard.aspx" class="active"><span>⊞</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>◎</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span>▤</span> My Courses</a>
            <a href="Gamification.aspx"><span>◆</span> Achievements</a>
        </div>

        <div class="container">

            <div class="welcome-banner">
                <div class="welcome-label">Student Portal</div>
                <div class="welcome-name">Welcome back, <asp:Label ID="lblWelcome" runat="server" />!</div>
                <div class="welcome-sub">Here's an overview of your learning progress today.</div>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-icon">📚</div>
                    <div class="stat-value"><asp:Label ID="lblEnrolled" runat="server" Text="0" /></div>
                    <div class="stat-label">Enrolled Courses</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">✅</div>
                    <div class="stat-value"><asp:Label ID="lblCompleted" runat="server" Text="0" /></div>
                    <div class="stat-label">Completed Lessons</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">⚡</div>
                    <div class="stat-value"><asp:Label ID="lblPoints" runat="server" Text="0" /></div>
                    <div class="stat-label">Total Points</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">🏅</div>
                    <div class="stat-value" style="font-size:1.2rem; padding-top:6px;">
                        <span class="badge-pill"><asp:Label ID="lblBadge" runat="server" Text="Bronze" /></span>
                    </div>
                    <div class="stat-label">Current Badge</div>
                </div>
            </div>

            <div class="section">
                <div class="section-header">
                    <div class="section-title">My Enrolled Courses</div>
                    <asp:Label ID="lblCourseCount" runat="server" CssClass="section-count" Text="0 courses" />
                </div>

                <asp:Panel ID="pnlCourses" runat="server">
                    <asp:GridView ID="gvCourses" runat="server"
                        AutoGenerateColumns="false"
                        Width="100%"
                        BorderStyle="None"
                        GridLines="None"
                        ShowHeaderWhenEmpty="false">
                        <Columns>
                            <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                            <asp:TemplateField HeaderText="Category">
                                <ItemTemplate>
                                    <span style="background:rgba(233,69,96,0.15);color:#e94560;padding:3px 10px;border-radius:20px;font-size:0.75rem;font-weight:600;">
                                        <%# Eval("category") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="enrolldate" HeaderText="Enrolled On" DataFormatString="{0:MMM dd, yyyy}" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <a href='LessonViewer.aspx?courseid=<%# Eval("courseid") %>'
                                       style="color:#e94560;font-size:0.82rem;font-weight:600;text-decoration:none;">
                                        Continue →
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>

                <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                    <div class="empty-state">
                        <div class="empty-state-icon">📭</div>
                        <p>You haven't enrolled in any courses yet.</p>
                        <a href="BrowseCourses.aspx">Browse Courses</a>
                    </div>
                </asp:Panel>
            </div>

        </div>
    </form>
</body>
</html>