<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forums.aspx.cs" Inherits="LearnSphere_WAPP.Student.Forums" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Forums - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --surface-hover: #f8fafd;
            --primary: #2563eb;
            --primary-bg: rgba(37,99,235,0.08);
            --primary-border: rgba(37,99,235,0.18);
            --accent-purple: #8b5cf6;
            --text: #1e293b;
            --text-secondary: #64748b;
            --text-muted: #94a3b8;
            --border: #e2e8f0;
            --border-light: #f1f5f9;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04);
            --shadow-md: 0 4px 12px rgba(0,0,0,0.06);
            --radius: 14px;
            --radius-sm: 10px;
        }
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: 'DM Sans', sans-serif;
            background: var(--bg-gradient);
            background-attachment: fixed;
            color: var(--text); min-height: 100vh;
        }
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82);
            backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 12px; text-decoration: none; }
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
            border-radius: 50%;
            display: flex; align-items: center;
            justify-content: center; font-size: 13px; font-weight: 700; color: white;
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
            color: var(--text-muted); text-decoration: none;
            padding: 14px 18px; font-size: 0.85rem; font-weight: 600;
            display: flex; align-items: center; gap: 8px;
            border-bottom: 2.5px solid transparent; transition: all 0.2s;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active { color: var(--primary); border-bottom-color: var(--primary); }
        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }
        .page-header {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 50%, #60a5fa 100%);
            border-radius: var(--radius); padding: 32px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(37,99,235,0.25);
            animation: slideDown 0.5s ease both;
        }
        .page-header::before {
            content: ''; position: absolute; top: -40%; right: -10%;
            width: 300px; height: 300px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%);
            border-radius: 50%;
        }
        .page-header-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: rgba(255,255,255,0.7);
            margin-bottom: 8px; font-family: 'Space Mono', monospace;
        }
        .page-header-title { font-size: 1.75rem; font-weight: 700; color: white; margin-bottom: 6px; }
        .page-header-sub { color: rgba(255,255,255,0.75); font-size: 0.88rem; }
        .search-bar { display: flex; gap: 12px; margin-bottom: 20px; animation: slideUp 0.4s ease both; }
        .search-input {
            flex: 1; padding: 10px 16px; border: 1px solid var(--border);
            border-radius: var(--radius-sm); font-family: 'DM Sans', sans-serif;
            font-size: 0.88rem; background: var(--surface); color: var(--text); outline: none;
        }
        .search-input:focus { border-color: var(--primary); }
        .btn-primary {
            background: var(--primary); color: white; border: none;
            padding: 10px 22px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-primary:hover { background: #1d4ed8; }
        .btn-secondary {
            background: var(--text-secondary); color: white; border: none;
            padding: 10px 22px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer;
        }
        .forums-grid { display: flex; flex-direction: column; gap: 14px; }
        .forum-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 22px 26px;
            box-shadow: var(--shadow-sm);
            display: flex; align-items: center; justify-content: space-between;
            transition: transform 0.2s, box-shadow 0.2s;
            text-decoration: none; color: var(--text);
            animation: slideUp 0.4s ease both;
        }
        .forum-card:hover { transform: translateY(-2px); box-shadow: var(--shadow-md); border-color: var(--primary-border); }
        .forum-left { display: flex; align-items: center; gap: 18px; flex: 1; }
        .forum-icon {
            width: 48px; height: 48px; background: var(--primary-bg);
            border-radius: 12px; display: flex; align-items: center;
            justify-content: center; font-size: 1.4rem; flex-shrink: 0;
        }
        .forum-info { flex: 1; }
        .forum-title { font-size: 0.95rem; font-weight: 700; margin-bottom: 4px; }
        .forum-desc {
            font-size: 0.82rem; color: var(--text-secondary); margin-bottom: 8px;
            white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 500px;
        }
        .forum-meta { display: flex; align-items: center; gap: 12px; }
        .forum-course-tag {
            background: var(--primary-bg); color: var(--primary);
            padding: 3px 12px; border-radius: 20px; font-size: 0.75rem; font-weight: 600;
        }
        .forum-date { font-size: 0.75rem; color: var(--text-muted); font-family: 'Space Mono', monospace; }
        .forum-right { display: flex; align-items: center; gap: 20px; flex-shrink: 0; }
        .forum-stat { text-align: center; }
        .forum-stat-num { font-size: 1.2rem; font-weight: 700; font-family: 'Space Mono', monospace; color: var(--primary); }
        .forum-stat-label { font-size: 0.7rem; color: var(--text-muted); font-weight: 600; text-transform: uppercase; }
        .arrow-icon { color: var(--text-muted); font-size: 1.1rem; transition: transform 0.2s, color 0.2s; }
        .forum-card:hover .arrow-icon { color: var(--primary); transform: translateX(4px); }
        .empty-state {
            text-align: center; padding: 60px 20px;
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); color: var(--text-secondary);
        }
        .empty-icon { font-size: 3rem; margin-bottom: 14px; opacity: 0.5; }
        .empty-state p { font-size: 0.9rem; }
        .alert { padding: 12px 18px; border-radius: var(--radius-sm); font-size: 0.85rem; margin-bottom: 16px; }
        .alert-danger { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
        @keyframes slideDown { from { opacity: 0; transform: translateY(-12px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 768px) {
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
            .forum-right { display: none; }
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
                    <div class="user-avatar">
                        <asp:Label ID="lblAvatarInitial" runat="server" Text="S" />
                    </div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>
<div class="nav">
    <a href="StudentDashboard.aspx"><span>📊</span> Dashboard</a>
    <a href="BrowseCourses.aspx"><span>📚</span> Browse Courses</a>
    <a href="MyCourses.aspx"><span>🎓</span> My Courses</a>
    <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
    <a href="StudentProfile.aspx"><span>👤</span> Profile</a>
    <a href="Forums.aspx" class="active"><span>💬</span> Forums</a>
    <a href="Messaging.aspx"><span>✉️</span> Messages</a>
</div>
        <div class="container">
            <div class="page-header">
                <div class="page-header-label">Community</div>
                <div class="page-header-title">Course Forums</div>
                <div class="page-header-sub">Browse discussions, ask questions and help others.</div>
            </div>
            <div class="search-bar">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search forums..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-primary" OnClick="btnSearch_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn-secondary" OnClick="btnClear_Click" />
            </div>
            <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger" Visible="false" />
            <div class="forums-grid">
                <asp:Repeater ID="rptForums" runat="server">
                    <ItemTemplate>
                        <a href='Questions.aspx?forumid=<%# Eval("forumid") %>' class="forum-card">
                            <div class="forum-left">
                                <div class="forum-icon">💬</div>
                                <div class="forum-info">
                                    <div class="forum-title"><%# Eval("title") %></div>
                                    <div class="forum-desc"><%# Eval("description") %></div>
                                    <div class="forum-meta">
                                        <span class="forum-course-tag"><%# Eval("coursename") %></span>
                                        <span class="forum-date"><%# Convert.ToDateTime(Eval("creationtime")).ToString("MMM dd, yyyy") %></span>
                                    </div>
                                </div>
                            </div>
                            <div class="forum-right">
                                <div class="forum-stat">
                                    <div class="forum-stat-num"><%# Eval("postcount") %></div>
                                    <div class="forum-stat-label">Posts</div>
                                </div>
                                <span class="arrow-icon">→</span>
                            </div>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <div class="empty-icon">💬</div>
                    <p>No forums found.</p>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>