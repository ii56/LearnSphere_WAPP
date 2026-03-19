<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Questions.aspx.cs" Inherits="LearnSphere_WAPP.Student.Questions" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Forum Questions - LearnSphere</title>
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
            --accent-green: #10b981;
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
            border-radius: 50%; display: flex; align-items: center;
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

        .breadcrumb {
            display: flex; align-items: center; gap: 8px;
            font-size: 0.82rem; color: var(--text-muted);
            margin-bottom: 20px;
        }
        .breadcrumb a { color: var(--primary); text-decoration: none; }
        .breadcrumb a:hover { text-decoration: underline; }

        .page-header {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 50%, #60a5fa 100%);
            border-radius: var(--radius); padding: 28px 36px;
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
            margin-bottom: 6px; font-family: 'Space Mono', monospace;
        }
        .page-header-title { font-size: 1.5rem; font-weight: 700; color: white; margin-bottom: 4px; }
        .page-header-sub { color: rgba(255,255,255,0.75); font-size: 0.85rem; }

        .toolbar {
            display: flex; justify-content: space-between; align-items: center;
            margin-bottom: 20px; animation: slideUp 0.4s ease both;
        }
        .toolbar-count { font-size: 0.85rem; color: var(--text-secondary); font-weight: 600; }
        .btn-ask {
            background: var(--primary); color: white; border: none;
            padding: 10px 22px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s; text-decoration: none;
            display: inline-flex; align-items: center; gap: 8px;
        }
        .btn-ask:hover { background: #1d4ed8; }

        .questions-list { display: flex; flex-direction: column; gap: 14px; }

        .question-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 22px 26px;
            box-shadow: var(--shadow-sm); transition: transform 0.2s, box-shadow 0.2s;
            animation: slideUp 0.4s ease both;
        }
        .question-card:hover { transform: translateY(-2px); box-shadow: var(--shadow-md); }

        .question-top { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 10px; }
        .question-title {
            font-size: 1rem; font-weight: 700; color: var(--text);
            text-decoration: none; transition: color 0.2s; flex: 1; margin-right: 16px;
        }
        .question-title:hover { color: var(--primary); }

        .answer-count {
            background: var(--primary-bg); color: var(--primary);
            padding: 4px 14px; border-radius: 20px;
            font-size: 0.75rem; font-weight: 700;
            font-family: 'Space Mono', monospace; flex-shrink: 0;
        }

        .question-content {
            font-size: 0.85rem; color: var(--text-secondary);
            margin-bottom: 14px; line-height: 1.6;
            display: -webkit-box; -webkit-line-clamp: 2;
            -webkit-box-orient: vertical; overflow: hidden;
        }

        .question-footer {
            display: flex; align-items: center; justify-content: space-between;
        }
        .question-meta { display: flex; align-items: center; gap: 12px; }
        .meta-author {
            display: flex; align-items: center; gap: 6px;
            font-size: 0.78rem; color: var(--text-secondary); font-weight: 600;
        }
        .author-dot {
            width: 22px; height: 22px;
            background: linear-gradient(135deg, var(--primary), var(--accent-purple));
            border-radius: 50%; display: flex; align-items: center;
            justify-content: center; font-size: 0.65rem; font-weight: 700; color: white;
        }
        .meta-date { font-size: 0.75rem; color: var(--text-muted); font-family: 'Space Mono', monospace; }

        .tags { display: flex; gap: 6px; }
        .tag {
            background: var(--border-light); color: var(--text-secondary);
            padding: 3px 10px; border-radius: 20px; font-size: 0.72rem; font-weight: 600;
        }

        .empty-state {
            text-align: center; padding: 60px 20px;
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); color: var(--text-secondary);
        }
        .empty-icon { font-size: 3rem; margin-bottom: 14px; opacity: 0.5; }
        .empty-state p { font-size: 0.9rem; margin-bottom: 20px; }

        .alert { padding: 12px 18px; border-radius: var(--radius-sm); font-size: 0.85rem; margin-bottom: 16px; }
        .alert-danger { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }

        @keyframes slideDown { from { opacity: 0; transform: translateY(-12px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 768px) {
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
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
            <a href="Forums.aspx" class="active"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
            <a href="StudentProfile.aspx"><span>👤</span> Profile</a>
        </div>

        <div class="container">

            <div class="breadcrumb">
                <a href="Forums.aspx">Forums</a>
                <span>›</span>
                <span><asp:Label ID="lblForumTitle" runat="server" Text="Forum" /></span>
            </div>

            <div class="page-header">
                <div class="page-header-label">Discussion</div>
                <div class="page-header-title"><asp:Label ID="lblPageTitle" runat="server" Text="Forum Questions" /></div>
                <div class="page-header-sub"><asp:Label ID="lblForumDesc" runat="server" /></div>
            </div>

            <div class="toolbar">
                <span class="toolbar-count"><asp:Label ID="lblPostCount" runat="server" Text="0 questions" /></span>
                <a href="#" id="linkAskQuestion" runat="server" class="btn-ask">+ Ask a Question</a>
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger" Visible="false" />

            <div class="questions-list">
                <asp:Repeater ID="rptQuestions" runat="server">
                    <ItemTemplate>
                        <div class="question-card">
                            <div class="question-top">
                                <a href='Answers.aspx?postid=<%# Eval("postid") %>' class="question-title">
                                    <%# Eval("title") %>
                                </a>
                                <span class="answer-count"><%# Eval("replycount") %> answers</span>
                            </div>
                            <div class="question-content"><%# Eval("content") %></div>
                            <div class="question-footer">
                                <div class="question-meta">
                                    <div class="meta-author">
                                        <div class="author-dot"><%# Eval("fname").ToString().Substring(0,1).ToUpper() %></div>
                                        <%# Eval("fname") + " " + Eval("lname") %>
                                    </div>
                                    <span class="meta-date"><%# Convert.ToDateTime(Eval("creationtime")).ToString("MMM dd, yyyy") %></span>
                                </div>
                                <div class="tags">
                                    <%# !string.IsNullOrEmpty(Eval("tags").ToString()) ? "<span class='tag'>" + Eval("tags").ToString().Replace(",", "</span><span class='tag'>") + "</span>" : "" %>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <div class="empty-icon">❓</div>
                    <p>No questions yet. Be the first to ask!</p>
                    <a href="#" id="linkAskFirst" runat="server" class="btn-ask">+ Ask a Question</a>
                </div>
            </asp:Panel>

        </div>
    </form>
</body>
</html>