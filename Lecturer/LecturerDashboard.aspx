<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LecturerDashboard.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.LecturerDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lecturer Dashboard - LearnSphere</title>
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
            background: linear-gradient(135deg, #10b981, var(--accent-purple));
            border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            font-size: 13px; font-weight: 700; color: white;
            overflow: hidden; position: relative;
        }
        .user-avatar img {
            width: 100%; height: 100%; object-fit: cover; border-radius: 50%;
            position: absolute; top: 0; left: 0;
        }
        .user-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }

        .verified-badge {
            display: inline-flex; align-items: center; gap: 5px;
            background: rgba(16,185,129,0.1);
            border: 1px solid rgba(16,185,129,0.25);
            color: #059669;
            font-size: 0.72rem; font-weight: 700;
            padding: 4px 12px; border-radius: 20px;
            letter-spacing: 0.3px;
        }

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
            transition: all 0.2s; position: relative;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active { color: var(--primary); border-bottom-color: var(--primary); }

        .nav-badge {
            background: #ef4444; color: white;
            font-size: 0.65rem; font-weight: 700;
            padding: 1px 6px; border-radius: 10px;
            font-family: 'Space Mono', monospace;
            min-width: 18px; text-align: center;
            display: inline-block;
        }

        /* ═══ MAIN CONTAINER ═══ */
        .container {
            max-width: 1140px; margin: 0 auto;
            padding: 28px 36px;
        }

        /* ═══ WELCOME BANNER ═══ */
        .welcome-banner {
            background: linear-gradient(135deg, #059669 0%, #10b981 55%, #34d399 100%);
            border-radius: var(--radius); padding: 32px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(16,185,129,0.28);
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
            color: rgba(255,255,255,0.75); margin-bottom: 8px;
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
        .stat-card:nth-child(2) { animation-delay: 0.10s; }
        .stat-card:nth-child(3) { animation-delay: 0.15s; }
        .stat-card:nth-child(4) { animation-delay: 0.20s; }
        .stat-card:hover { transform: translateY(-2px); box-shadow: var(--shadow-md); }

        .stat-card::after {
            content: '';
            position: absolute; top: 0; left: 0;
            width: 100%; height: 3px;
        }
        .stat-card:nth-child(1)::after { background: var(--primary); }
        .stat-card:nth-child(2)::after { background: var(--accent-green); }
        .stat-card:nth-child(3)::after { background: var(--accent-purple); }
        .stat-card:nth-child(4)::after { background: var(--accent-orange); }

        .stat-icon {
            width: 42px; height: 42px; border-radius: 10px;
            display: flex; align-items: center; justify-content: center;
            font-size: 1.2rem; margin-bottom: 14px;
        }
        .stat-card:nth-child(1) .stat-icon { background: rgba(37,99,235,0.1); }
        .stat-card:nth-child(2) .stat-icon { background: rgba(16,185,129,0.1); }
        .stat-card:nth-child(3) .stat-icon { background: rgba(139,92,246,0.1); }
        .stat-card:nth-child(4) .stat-icon { background: rgba(245,158,11,0.1); }

        .stat-value {
            font-size: 2rem; font-weight: 700;
            font-family: 'Space Mono', monospace;
            letter-spacing: -1px; line-height: 1; margin-bottom: 6px;
        }
        .stat-card:nth-child(1) .stat-value { color: var(--primary); }
        .stat-card:nth-child(2) .stat-value { color: var(--accent-green); }
        .stat-card:nth-child(3) .stat-value { color: var(--accent-purple); }
        .stat-card:nth-child(4) .stat-value { color: var(--accent-orange); }

        .stat-label {
            font-size: 0.78rem; color: var(--text-secondary);
            font-weight: 600; letter-spacing: 0.3px;
        }

        /* ═══ SECTION ═══ */
        .section {
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: var(--radius);
            box-shadow: var(--shadow-sm);
            overflow: hidden;
            margin-bottom: 20px;
        }
        .section-courses { animation: slideUp 0.5s 0.25s ease both; }
        .section-actions { animation: slideUp 0.5s 0.35s ease both; }

        .section-header {
            padding: 18px 24px;
            border-bottom: 1px solid var(--border);
            display: flex; align-items: flex-start; justify-content: space-between;
        }
        .section-title {
            font-size: 0.95rem; font-weight: 700; color: var(--text);
            display: flex; align-items: center; gap: 10px;
        }
        .section-title-dot {
            width: 8px; height: 8px; border-radius: 50%;
            flex-shrink: 0;
        }
        .dot-green  { background: var(--accent-green); }
        .dot-purple { background: var(--accent-purple); }

        .section-sub {
            font-size: 0.78rem; color: var(--text-muted);
            margin-top: 4px; padding-left: 18px;
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
        .price-tag {
            display: inline-block;
            background: rgba(16,185,129,0.08);
            border: 1px solid rgba(16,185,129,0.2);
            color: #059669;
            padding: 3px 12px; border-radius: 20px;
            font-size: 0.78rem; font-weight: 700;
            font-family: 'Space Mono', monospace;
        }
        .price-tag.free {
            background: rgba(245,158,11,0.08);
            border-color: rgba(245,158,11,0.2);
            color: var(--accent-orange);
        }

        /* ═══ QUICK ACTIONS ═══ */
        .quick-grid {
            display: grid;
            grid-template-columns: repeat(5, 1fr);
            gap: 14px;
            padding: 20px 24px;
        }
        .quick-card {
            background: var(--bg);
            border: 1px solid var(--border);
            border-radius: var(--radius-sm);
            padding: 18px 16px;
            text-decoration: none;
            transition: all 0.2s;
            display: block;
        }
        .quick-card:hover {
            background: var(--surface);
            border-color: var(--primary-border);
            transform: translateY(-2px);
            box-shadow: var(--shadow-md);
        }
        .quick-card-icon { font-size: 1.4rem; margin-bottom: 10px; display: block; }
        .quick-card-title {
            font-size: 0.82rem; font-weight: 700; color: var(--text);
            margin-bottom: 4px;
        }
        .quick-card-desc { font-size: 0.72rem; color: var(--text-muted); line-height: 1.4; }

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
        @media (max-width: 1000px) {
            .quick-grid { grid-template-columns: repeat(3, 1fr); }
        }
        @media (max-width: 900px) {
            .stats-grid { grid-template-columns: repeat(2, 1fr); }
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
            .welcome-name { font-size: 1.4rem; }
            .quick-grid { grid-template-columns: repeat(2, 1fr); }
        }
        @media (max-width: 500px) {
            .stats-grid { grid-template-columns: 1fr; }
            .quick-grid { grid-template-columns: 1fr; }
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
                <% if (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") { %>
                    <span class="verified-badge">✔ Verified Lecturer</span>
                <% } %>
                <div class="user-pill">
                    <div class="user-avatar">
                        <img id="imgSidebarProfile" runat="server" />
                    </div>
                    <span class="user-name"><%= Session["uname"] %></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <!-- ═══ NAV ═══ -->
        <div class="nav">
            <a href="LecturerDashboard.aspx" class="active"><span>📊</span> Dashboard</a>
            <a href="CreateCourse.aspx"><span>➕</span> Create Course</a>
            <a href="ViewCourses.aspx"><span>📚</span> View Courses</a>
            <a href="EditProfile.aspx"><span>👤</span> Edit Profile</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Message.aspx">
                <span>✉️</span> Messaging
                <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                    <span class="nav-badge"><%= Session["unreadCount"] %></span>
                <% } %>
            </a>
        </div>

        <!-- ═══ CONTENT ═══ -->
        <div class="container">

            <!-- Welcome Banner -->
            <div class="welcome-banner">
                <div class="welcome-label">Lecturer Portal</div>
                <div class="welcome-name">
                    <asp:Label ID="lblWelcome" runat="server" />
                </div>
                <div class="welcome-sub">Here's an overview of your courses and student activity.</div>
            </div>

            <!-- Stats Grid -->
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-icon">📚</div>
                    <div class="stat-value"><asp:Label ID="lblTotalCourses" runat="server" Text="0" /></div>
                    <div class="stat-label">Total Courses</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">🎓</div>
                    <div class="stat-value"><asp:Label ID="lblTotalStudents" runat="server" Text="0" /></div>
                    <div class="stat-label">Total Students</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">💳</div>
                    <div class="stat-value"><asp:Label ID="lblPaidCourses" runat="server" Text="0" /></div>
                    <div class="stat-label">Paid Courses</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">🆓</div>
                    <div class="stat-value"><asp:Label ID="lblFreeCourses" runat="server" Text="0" /></div>
                    <div class="stat-label">Free Courses</div>
                </div>
            </div>

            <!-- Top 5 Recent Courses Table -->
            <div class="section section-courses">
                <div class="section-header">
                    <div>
                        <div class="section-title">
                            <span class="section-title-dot dot-green"></span>
                            Top 5 Recent Courses
                        </div>
                        <div class="section-sub">Latest courses you created</div>
                    </div>
                </div>

                <asp:GridView ID="gvTopCourses" runat="server"
                    AutoGenerateColumns="False"
                    Width="100%"
                    BorderStyle="None"
                    GridLines="None"
                    EmptyDataText="No courses found.">
                    <Columns>
                        <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                        <asp:TemplateField HeaderText="Category">
                            <ItemTemplate>
                                <span class="category-tag"><%# Eval("category") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Price">
                            <ItemTemplate>
                                <%# Convert.ToDecimal(Eval("price")) == 0
                                    ? "<span class=\"price-tag free\">FREE</span>"
                                    : "<span class=\"price-tag\">RM " + string.Format("{0:N2}", Eval("price")) + "</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="creationtime" HeaderText="Created Date" DataFormatString="{0:dd MMM yyyy}" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Quick Actions -->
            <div class="section section-actions">
                <div class="section-header">
                    <div class="section-title">
                        <span class="section-title-dot dot-purple"></span>
                        Quick Actions
                    </div>
                </div>
                <div class="quick-grid">
                    <a href="CreateCourse.aspx" class="quick-card">
                        <span class="quick-card-icon">✏️</span>
                        <div class="quick-card-title">Create / Continue Course</div>
                        <div class="quick-card-desc">Start a new course or continue your draft</div>
                    </a>
                    <a href="ViewCourses.aspx" class="quick-card">
                        <span class="quick-card-icon">🗂️</span>
                        <div class="quick-card-title">Manage Courses</div>
                        <div class="quick-card-desc">Edit, publish, or delete your courses</div>
                    </a>
                    <a href="Forums.aspx" class="quick-card">
                        <span class="quick-card-icon">💬</span>
                        <div class="quick-card-title">Manage Forums</div>
                        <div class="quick-card-desc">Create and manage course discussions</div>
                    </a>
                    <a href="EditProfile.aspx" class="quick-card">
                        <span class="quick-card-icon">👤</span>
                        <div class="quick-card-title">Edit Profile</div>
                        <div class="quick-card-desc">Update your personal details</div>
                    </a>
                    <a href="Message.aspx" class="quick-card">
                        <span class="quick-card-icon">✉️</span>
                        <div class="quick-card-title">Messages</div>
                        <div class="quick-card-desc">Check and respond to messages</div>
                    </a>
                </div>
            </div>

        </div>

        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

    </form>
</body>
</html>
