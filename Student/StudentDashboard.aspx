<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentDashboard.aspx.cs" Inherits="LearnSphere_WAPP.Student.StudentDashboard" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Student Dashboard - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --surface-hover: #f8fafd;
            --primary: #2563eb;
            --primary-light: #3b82f6;
            --primary-bg: rgba(37,99,235,0.08);
            --primary-border: rgba(37,99,235,0.18);
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
            --shadow-lg: 0 10px 30px rgba(0,0,0,0.08);
            --radius: 14px;
            --radius-sm: 10px;
        }

        * { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'DM Sans', sans-serif;
            background: var(--bg-gradient);
            background-attachment: fixed;
            color: var(--text);
            min-height: 100vh;
        }

        /* ═══ HEADER ═══ */
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82);
            backdrop-filter: blur(20px);
            -webkit-backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }

        .logo { display: flex; align-items: center; gap: 12px; text-decoration: none; }
        .logo img { height: 38px; width: 38px; object-fit: contain; }
        .logo-text { font-size: 1.2rem; font-weight: 700; color: var(--text); letter-spacing: -0.3px; }
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
            border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            font-size: 13px; font-weight: 700; color: white;
        }
        .user-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }

        .btn-logout {
            background: transparent;
            border: 1px solid var(--border);
            color: var(--text-secondary);
            padding: 7px 18px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif;
            font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-logout:hover { background: #fef2f2; color: #ef4444; border-color: #fecaca; }

        /* ═══ NAV ═══ */
        .nav {
            background: var(--surface);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; display: flex; gap: 2px;
        }
        .nav a {
            color: var(--text-muted); text-decoration: none;
            padding: 14px 18px; font-size: 0.85rem; font-weight: 600;
            display: flex; align-items: center; gap: 8px;
            border-bottom: 2.5px solid transparent;
            transition: all 0.2s;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active {
            color: var(--primary);
            border-bottom-color: var(--primary);
        }
        .nav-icon { font-size: 1rem; }

        /* ═══ MAIN CONTAINER ═══ */
        .container {
            max-width: 1140px; margin: 0 auto;
            padding: 28px 36px;
        }

        /* ═══ WELCOME BANNER ═══ */
        .welcome-banner {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 50%, #60a5fa 100%);
            border-radius: var(--radius); padding: 32px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(37,99,235,0.25);
            animation: slideDown 0.5s ease both;
        }
        .welcome-banner::before {
            content: '';
            position: absolute; top: -40%; right: -10%;
            width: 300px; height: 300px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%);
            border-radius: 50%; pointer-events: none;
        }
        .welcome-banner::after {
            content: '';
            position: absolute; bottom: -30%; left: 50%;
            width: 200px; height: 200px;
            background: radial-gradient(circle, rgba(255,255,255,0.08), transparent 65%);
            border-radius: 50%; pointer-events: none;
        }
        .welcome-label {
            font-size: 0.72rem; font-weight: 700;
            letter-spacing: 2px; text-transform: uppercase;
            color: rgba(255,255,255,0.7); margin-bottom: 8px;
            font-family: 'Space Mono', monospace;
        }
        .welcome-name {
            font-size: 1.75rem; font-weight: 700; color: white;
            letter-spacing: -0.5px; margin-bottom: 6px;
        }
        .welcome-sub { color: rgba(255,255,255,0.75); font-size: 0.88rem; }

        /* ═══ STATS ═══ */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 16px; margin-bottom: 24px;
        }

        .stat-card {
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: var(--radius); padding: 22px 24px;
            box-shadow: var(--shadow-sm);
            transition: transform 0.2s, box-shadow 0.2s;
            animation: slideUp 0.4s ease both;
            position: relative; overflow: hidden;
        }
        .stat-card:nth-child(1) { animation-delay: 0.05s; }
        .stat-card:nth-child(2) { animation-delay: 0.1s; }
        .stat-card:nth-child(3) { animation-delay: 0.15s; }
        .stat-card:nth-child(4) { animation-delay: 0.2s; }
        .stat-card:hover { transform: translateY(-2px); box-shadow: var(--shadow-md); }

        .stat-card::after {
            content: '';
            position: absolute; top: 0; left: 0;
            width: 100%; height: 3px;
        }
        .stat-card:nth-child(1)::after { background: var(--primary); }
        .stat-card:nth-child(2)::after { background: var(--accent-green); }
        .stat-card:nth-child(3)::after { background: var(--accent-orange); }
        .stat-card:nth-child(4)::after { background: var(--accent-purple); }

        .stat-icon {
            width: 42px; height: 42px; border-radius: 10px;
            display: flex; align-items: center; justify-content: center;
            font-size: 1.2rem; margin-bottom: 14px;
        }
        .stat-card:nth-child(1) .stat-icon { background: rgba(37,99,235,0.1); }
        .stat-card:nth-child(2) .stat-icon { background: rgba(16,185,129,0.1); }
        .stat-card:nth-child(3) .stat-icon { background: rgba(245,158,11,0.1); }
        .stat-card:nth-child(4) .stat-icon { background: rgba(139,92,246,0.1); }

        .stat-value {
            font-size: 2rem; font-weight: 700;
            font-family: 'Space Mono', monospace;
            letter-spacing: -1px; line-height: 1; margin-bottom: 6px;
        }
        .stat-card:nth-child(1) .stat-value { color: var(--primary); }
        .stat-card:nth-child(2) .stat-value { color: var(--accent-green); }
        .stat-card:nth-child(3) .stat-value { color: var(--accent-orange); }
        .stat-card:nth-child(4) .stat-value { color: var(--accent-purple); }

        .stat-label {
            font-size: 0.78rem; color: var(--text-secondary);
            font-weight: 600; letter-spacing: 0.3px;
        }

        .badge-pill {
            display: inline-block;
            background: linear-gradient(135deg, rgba(139,92,246,0.1), rgba(139,92,246,0.15));
            border: 1px solid rgba(139,92,246,0.25);
            color: var(--accent-purple);
            font-size: 0.72rem; font-weight: 700;
            letter-spacing: 0.8px; text-transform: uppercase;
            padding: 4px 12px; border-radius: 20px;
            font-family: 'Space Mono', monospace;
        }

        /* ═══ SECTION ═══ */
        .section {
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: var(--radius);
            box-shadow: var(--shadow-sm);
            overflow: hidden;
            animation: slideUp 0.5s 0.25s ease both;
        }
        .section-header {
            padding: 18px 24px;
            border-bottom: 1px solid var(--border);
            display: flex; align-items: center; justify-content: space-between;
        }
        .section-title {
            font-size: 0.95rem; font-weight: 700; color: var(--text);
            display: flex; align-items: center; gap: 10px;
        }
        .section-title-dot {
            width: 8px; height: 8px; border-radius: 50%;
            background: var(--primary);
        }
        .section-count {
            font-size: 0.75rem; color: var(--text-muted);
            background: var(--bg); border: 1px solid var(--border);
            padding: 3px 12px; border-radius: 20px;
            font-family: 'Space Mono', monospace;
        }

        /* TABLE */
        .section table { width: 100%; border-collapse: collapse; }
        .section table th {
            background: var(--border-light);
            padding: 11px 24px; text-align: left;
            font-size: 0.72rem; font-weight: 700;
            letter-spacing: 1px; text-transform: uppercase;
            color: var(--text-muted);
            border-bottom: 1px solid var(--border);
        }
        .section table td {
            padding: 14px 24px; font-size: 0.875rem;
            color: var(--text); border-bottom: 1px solid var(--border-light);
        }
        .section table tr:last-child td { border-bottom: none; }
        .section table tr:hover td { background: var(--surface-hover); }

        .category-tag {
            display: inline-block;
            background: var(--primary-bg);
            color: var(--primary);
            padding: 3px 12px; border-radius: 20px;
            font-size: 0.75rem; font-weight: 600;
        }
        .link-action {
            color: var(--primary); font-size: 0.82rem;
            font-weight: 600; text-decoration: none;
            transition: color 0.2s;
        }
        .link-action:hover { color: #1d4ed8; }

        /* EMPTY */
        .empty-state {
            text-align: center; padding: 60px 20px; color: var(--text-secondary);
        }
        .empty-state-icon { font-size: 3rem; margin-bottom: 14px; opacity: 0.5; }
        .empty-state p { font-size: 0.9rem; margin-bottom: 20px; }
        .empty-state a {
            display: inline-block;
            background: var(--primary); color: white;
            text-decoration: none; padding: 10px 24px;
            border-radius: 8px; font-size: 0.85rem; font-weight: 600;
            transition: background 0.2s;
        }
        .empty-state a:hover { background: #1d4ed8; }

        /* ═══ ANIMATIONS ═══ */
        @keyframes slideDown {
            from { opacity: 0; transform: translateY(-12px); }
            to   { opacity: 1; transform: translateY(0); }
        }
        @keyframes slideUp {
            from { opacity: 0; transform: translateY(14px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        /* ═══ RESPONSIVE ═══ */
        @media (max-width: 900px) {
            .stats-grid { grid-template-columns: repeat(2, 1fr); }
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
            .welcome-name { font-size: 1.4rem; }
        }
        @media (max-width: 500px) {
            .stats-grid { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- ═══ HEADER ═══ -->
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" />
                <div class="logo-text">Learn<span>Sphere</span></div>
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

        <!-- ═══ NAV ═══ -->
        <div class="nav">
            <a href="StudentDashboard.aspx" class="active"><span class="nav-icon">📊</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span class="nav-icon">📚</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span class="nav-icon">🎓</span> My Courses</a>
            <a href="Gamification.aspx"><span class="nav-icon">🏆</span> Achievements</a>
            <a href="StudentProfile.aspx"><span>👤</span> Profile</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
        </div>

        <!-- ═══ CONTENT ═══ -->
        <div class="container">

            <!-- Welcome Banner -->
            <div class="welcome-banner">
                <div class="welcome-label">Student Portal</div>
                <div class="welcome-name">Welcome back, <asp:Label ID="lblWelcome" runat="server" />!</div>
                <div class="welcome-sub">Here's an overview of your learning progress.</div>
            </div>

            <!-- Stats -->
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
                    <div class="stat-value" style="font-size:1rem; padding-top:4px;">
                        <span class="badge-pill"><asp:Label ID="lblBadge" runat="server" Text="Bronze" /></span>
                    </div>
                    <div class="stat-label">Current Badge</div>
                </div>
            </div>

            <!-- Courses Table -->
            <div class="section">
                <div class="section-header">
                    <div class="section-title">
                        <span class="section-title-dot"></span>
                        My Enrolled Courses
                    </div>
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
                                    <span class="category-tag">
                                        <%# Eval("category") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="enrolldate" HeaderText="Enrolled On" DataFormatString="{0:MMM dd, yyyy}" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <a href='LessonViewer.aspx?courseid=<%# Eval("courseid") %>' class="link-action">
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
