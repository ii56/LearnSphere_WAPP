<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LessonViewer.aspx.cs" Inherits="LearnSphere_WAPP.Student.LessonViewer" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lesson Viewer - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --primary: #2563eb;
            --primary-bg: rgba(37,99,235,0.08);
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

        /* layout */
        .page-wrap {
            max-width: 1140px; margin: 0 auto; padding: 28px 36px;
            display: flex; gap: 22px;
        }

        /* sidebar */
        .sidebar { width: 270px; flex-shrink: 0; }
        .sidebar-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); overflow: hidden; box-shadow: var(--shadow-sm);
        }
        .sidebar-header { padding: 18px 20px; border-bottom: 1px solid var(--border); }
        .sidebar-course-label {
            font-size: 0.68rem; font-weight: 700; letter-spacing: 1.5px;
            text-transform: uppercase; color: var(--primary); margin-bottom: 6px;
            font-family: 'Space Mono', monospace;
        }
        .sidebar-course-name { font-size: 0.92rem; font-weight: 700; }
        .back-link {
            display: flex; align-items: center; gap: 6px;
            color: var(--text-muted); text-decoration: none; font-size: 0.78rem;
            margin-top: 10px; transition: color 0.2s;
        }
        .back-link:hover { color: var(--primary); }

        .module-section { border-bottom: 1px solid var(--border); }
        .module-title {
            padding: 12px 20px; font-size: 0.75rem; font-weight: 700;
            letter-spacing: 0.5px; text-transform: uppercase;
            color: var(--text-muted); background: var(--border-light);
        }
        .lesson-item {
            padding: 11px 20px; font-size: 0.83rem; cursor: pointer;
            display: flex; align-items: center; gap: 10px; transition: background 0.2s;
            border-bottom: 1px solid var(--border-light); text-decoration: none; color: var(--text);
        }
        .lesson-item:last-child { border-bottom: none; }
        .lesson-item:hover { background: var(--border-light); }
        .lesson-item.active { background: var(--primary-bg); color: var(--primary); }
        .lesson-item.completed { color: var(--accent-green); }
        .lesson-dot { width: 8px; height: 8px; border-radius: 50%; background: var(--border); flex-shrink: 0; }
        .lesson-item.completed .lesson-dot { background: var(--accent-green); }
        .lesson-item.active .lesson-dot { background: var(--primary); }

        /* content */
        .content { flex: 1; min-width: 0; }
        .content-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 32px; box-shadow: var(--shadow-sm);
            animation: slideUp 0.4s ease both;
        }
        .lesson-title { font-size: 1.4rem; font-weight: 700; margin-bottom: 10px; }
        .lesson-desc { color: var(--text-secondary); font-size: 0.88rem; line-height: 1.7; margin-bottom: 24px; }

        .video-box {
            background: var(--border-light); border-radius: 12px; height: 320px;
            display: flex; flex-direction: column; align-items: center; justify-content: center;
            margin-bottom: 24px; border: 1px solid var(--border); position: relative; overflow: hidden;
        }
        .video-icon { font-size: 2.5rem; margin-bottom: 10px; opacity: 0.3; }
        .video-text { color: var(--text-muted); font-size: 0.85rem; }

        .files-section { margin-bottom: 24px; }
        .files-title { font-size: 0.85rem; font-weight: 700; margin-bottom: 10px; }
        .file-item {
            display: flex; align-items: center; gap: 12px;
            background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 12px 16px; margin-bottom: 8px;
            text-decoration: none; color: var(--text); transition: border-color 0.2s;
        }
        .file-item:hover { border-color: var(--primary); }
        .file-icon { font-size: 1.2rem; }
        .file-name { flex: 1; font-size: 0.85rem; font-weight: 500; }
        .file-arrow { color: var(--primary); font-weight: 700; }

        .btn-complete {
            background: var(--accent-green); color: white; border: none;
            padding: 12px 28px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.9rem; font-weight: 700;
            cursor: pointer; transition: background 0.2s;
            display: flex; align-items: center; gap: 8px;
        }
        .btn-complete:hover { background: #059669; }
        .btn-complete:disabled {
            background: var(--border-light); color: var(--text-muted);
            cursor: not-allowed; border: 1px solid var(--border);
        }

        .alert-success {
            background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2);
            color: #059669; padding: 12px 18px; border-radius: var(--radius-sm);
            font-size: 0.85rem; font-weight: 500; margin-top: 14px;
        }

        .select-state { text-align: center; padding: 70px 20px; color: var(--text-muted); }
        .select-state-icon { font-size: 2.5rem; margin-bottom: 14px; opacity: 0.4; }
        .select-state h3 { font-size: 1.05rem; font-weight: 700; color: var(--text); margin-bottom: 6px; }
        .select-state p { font-size: 0.85rem; }

        .no-lessons { text-align: center; padding: 50px 20px; color: var(--text-muted); }
        .no-lessons-icon { font-size: 2.5rem; margin-bottom: 10px; opacity: 0.4; }
        .no-lessons a {
            display: inline-block; margin-top: 14px; background: var(--primary); color: white;
            text-decoration: none; padding: 9px 22px; border-radius: 8px; font-size: 0.82rem; font-weight: 600;
        }

        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 900px) { .page-wrap { flex-direction: column; padding: 20px; } .sidebar { width: 100%; } }
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
            <a href="MyCourses.aspx" class="active"><span>🎓</span> My Courses</a>
            <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
            <a href="StudentProfile.aspx"><span>👤</span> Profile</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
        </div>

        <div class="page-wrap">
            <div class="sidebar">
                <div class="sidebar-card">
                    <div class="sidebar-header">
                        <div class="sidebar-course-label">Now Learning</div>
                        <div class="sidebar-course-name"><asp:Label ID="lblCourseName" runat="server" Text="..." /></div>
                        <a href="MyCourses.aspx" class="back-link">← Back to My Courses</a>
                    </div>

                    <asp:Panel ID="pnlModules" runat="server">
                        <asp:Repeater ID="rptModules" runat="server">
                            <ItemTemplate>
                                <div class="module-section">
                                    <div class="module-title"><%# Eval("ModuleName") %></div>
                                    <asp:Repeater ID="rptLessons" runat="server" DataSource='<%# Eval("Lessons") %>'>
                                        <ItemTemplate>
                                            <a href='LessonViewer.aspx?courseid=<%# Request.QueryString["courseid"] %>&lessonId=<%# Eval("LessonId") %>'
                                               class='lesson-item <%# Convert.ToBoolean(Eval("IsCompleted")) ? "completed" : "" %> <%# Request.QueryString["lessonId"] == Eval("LessonId").ToString() ? "active" : "" %>'>
                                                <div class="lesson-dot"></div>
                                                <%# Eval("LessonTitle") %>
                                            </a>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>

                    <asp:Panel ID="pnlNoModules" runat="server" Visible="false">
                        <div class="no-lessons">
                            <div class="no-lessons-icon">📭</div>
                            <p>No lessons added yet for this course.</p>
                            <a href="MyCourses.aspx">Back to My Courses</a>
                        </div>
                    </asp:Panel>
                </div>
            </div>

            <div class="content">
                <asp:Panel ID="pnlLesson" runat="server" Visible="false">
                    <div class="content-card">
                        <div class="lesson-title"><asp:Label ID="lblLessonTitle" runat="server" /></div>
                        <div class="lesson-desc"><asp:Label ID="lblLessonDesc" runat="server" /></div>

                        <asp:Panel ID="pnlVideo" runat="server" Visible="false">
                            <div class="video-box">
                                <iframe id="iframeVideo" runat="server"
                                    style="width:100%;height:100%;border:none;border-radius:12px;"
                                    allowfullscreen="allowfullscreen"></iframe>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlNoVideo" runat="server" Visible="true">
                            <div class="video-box">
                                <div class="video-icon">▶</div>
                                <div class="video-text">No video uploaded for this lesson yet</div>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlFiles" runat="server" Visible="false">
                            <div class="files-section">
                                <div class="files-title">Lesson Materials</div>
                                <asp:Repeater ID="rptMaterials" runat="server">
                                    <ItemTemplate>
                                        <a href='<%# Eval("fileurl") %>' target="_blank" class="file-item">
                                            <span class="file-icon">
                                                <%# Eval("filetype").ToString().ToLower() == "pdf" ? "📄" : 
                                                    Eval("filetype").ToString().ToLower() == "ppt" ? "📊" : "📁" %>
                                            </span>
                                            <span class="file-name"><%# Eval("filetype") %> Document</span>
                                            <span class="file-arrow">→</span>
                                        </a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:Panel>

                        <asp:Button ID="btnComplete" runat="server"
                            Text="Mark as Completed (+10 Points)"
                            CssClass="btn-complete" OnClick="btnComplete_Click" />
                        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="alert-success" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlSelectLesson" runat="server" Visible="true">
                    <div class="content-card">
                        <div class="select-state">
                            <div class="select-state-icon">👈</div>
                            <h3>Select a lesson to start</h3>
                            <p>Choose a lesson from the sidebar to begin learning.</p>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
