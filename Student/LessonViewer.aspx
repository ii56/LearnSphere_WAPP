<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LessonViewer.aspx.cs" Inherits="LearnSphere_WAPP.Student.LessonViewer" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lesson Viewer - LearnSphere</title>
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

        /* LAYOUT */
        .page-wrap { max-width: 1200px; margin: 0 auto; padding: 36px 40px; display: flex; gap: 24px; position: relative; z-index: 1; }

        /* SIDEBAR */
        .sidebar { width: 280px; flex-shrink: 0; }
        .sidebar-card { background: var(--surface); border: 1px solid var(--border); border-radius: 16px; overflow: hidden; }
        .sidebar-header { padding: 18px 20px; border-bottom: 1px solid var(--border); }
        .sidebar-course-label { font-size: 0.7rem; font-weight: 600; letter-spacing: 1.5px; text-transform: uppercase; color: var(--accent); margin-bottom: 6px; font-family: 'JetBrains Mono', monospace; }
        .sidebar-course-name { font-size: 0.95rem; font-weight: 700; }
        .back-link { display: flex; align-items: center; gap: 6px; color: var(--muted); text-decoration: none; font-size: 0.8rem; margin-top: 10px; transition: color 0.2s; }
        .back-link:hover { color: var(--accent); }

        .module-section { border-bottom: 1px solid var(--border); }
        .module-title { padding: 12px 20px; font-size: 0.78rem; font-weight: 700; letter-spacing: 0.5px; text-transform: uppercase; color: var(--muted); background: var(--surface2); }
        .lesson-item { padding: 12px 20px; font-size: 0.85rem; cursor: pointer; display: flex; align-items: center; gap: 10px; transition: background 0.2s; border-bottom: 1px solid var(--border); text-decoration: none; color: var(--text); }
        .lesson-item:last-child { border-bottom: none; }
        .lesson-item:hover { background: rgba(255,255,255,0.03); }
        .lesson-item.active { background: rgba(233,69,96,0.08); color: var(--accent); }
        .lesson-item.completed { color: #10b981; }
        .lesson-dot { width: 8px; height: 8px; border-radius: 50%; background: var(--border); flex-shrink: 0; }
        .lesson-item.completed .lesson-dot { background: #10b981; }
        .lesson-item.active .lesson-dot { background: var(--accent); }


        /* FILE MATERIALS */
        .files-section { margin-bottom: 24px; }
        .files-title { font-size: 0.85rem; font-weight: 700; margin-bottom: 12px; color: var(--text); }
        .file-item {
            display: flex; align-items: center; gap: 12px;
            background: var(--surface2); border: 1px solid var(--border);
            border-radius: 10px; padding: 14px 18px; margin-bottom: 8px;
            text-decoration: none; color: var(--text); transition: border-color 0.2s;
        }
        .file-item:hover { border-color: rgba(233,69,96,0.4); }
        .file-icon { font-size: 1.3rem; }
        .file-name { flex: 1; font-size: 0.875rem; font-weight: 500; }
        .file-arrow { color: var(--accent); font-weight: 700; }


        /* CONTENT */
        .content { flex: 1; min-width: 0; }
        .content-card { background: var(--surface); border: 1px solid var(--border); border-radius: 16px; padding: 36px; animation: fadeUp 0.4s ease both; }
        .lesson-title { font-size: 1.5rem; font-weight: 700; letter-spacing: -0.5px; margin-bottom: 12px; }
        .lesson-desc { color: var(--muted); font-size: 0.9rem; line-height: 1.7; margin-bottom: 28px; }

        .video-box {
            background: #000; border-radius: 12px; height: 320px;
            display: flex; flex-direction: column; align-items: center; justify-content: center;
            margin-bottom: 28px; border: 1px solid var(--border); position: relative; overflow: hidden;
        }
        .video-box::before { content: ''; position: absolute; inset: 0; background: radial-gradient(ellipse at center, rgba(233,69,96,0.05), transparent 70%); }
        .video-icon { font-size: 3rem; margin-bottom: 12px; opacity: 0.4; }
        .video-text { color: var(--muted); font-size: 0.875rem; }

        .btn-complete {
            background: linear-gradient(135deg, #10b981, #059669);
            color: white; border: none; padding: 14px 32px; border-radius: 10px;
            font-family: 'Sora', sans-serif; font-size: 0.95rem; font-weight: 700;
            cursor: pointer; transition: opacity 0.2s; display: flex; align-items: center; gap: 10px;
        }
        .btn-complete:hover { opacity: 0.85; }
        .btn-complete:disabled { background: var(--surface2); color: var(--muted); cursor: not-allowed; opacity: 1; border: 1px solid var(--border); }

        .alert-success { background: rgba(16,185,129,0.1); border: 1px solid rgba(16,185,129,0.3); color: #10b981; padding: 14px 20px; border-radius: 10px; font-size: 0.875rem; font-weight: 500; margin-top: 16px; }

        /* SELECT LESSON STATE */
        .select-state { text-align: center; padding: 80px 20px; color: var(--muted); }
        .select-state-icon { font-size: 3rem; margin-bottom: 16px; }
        .select-state h3 { font-size: 1.1rem; font-weight: 700; color: var(--text); margin-bottom: 8px; }
        .select-state p { font-size: 0.875rem; }

        /* NO LESSONS STATE */
        .no-lessons { text-align: center; padding: 60px 20px; color: var(--muted); }
        .no-lessons-icon { font-size: 3rem; margin-bottom: 12px; }
        .no-lessons a { display: inline-block; margin-top: 16px; background: var(--accent); color: white; text-decoration: none; padding: 10px 24px; border-radius: 8px; font-size: 0.85rem; font-weight: 600; }

        @keyframes fadeUp { from { opacity: 0; transform: translateY(16px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 900px) { .page-wrap { flex-direction: column; padding: 20px; } .sidebar { width: 100%; } }
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
            <a href="MyCourses.aspx" class="active"><span>▤</span> My Courses</a>
            <a href="Gamification.aspx"><span>◆</span> Achievements</a>
        </div>

        <div class="page-wrap">

            <!-- SIDEBAR -->
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

            <!-- CONTENT -->
            <div class="content">
                <asp:Panel ID="pnlLesson" runat="server" Visible="false">
                    <div class="content-card">
                        <div class="lesson-title">
                            <asp:Label ID="lblLessonTitle" runat="server" /></div>
                        <div class="lesson-desc">
                            <asp:Label ID="lblLessonDesc" runat="server" /></div>

                        <!-- VIDEO MATERIAL -->
                        <asp:Panel ID="pnlVideo" runat="server" Visible="false">
                            <div class="video-box">
                                <iframe id="iframeVideo" runat="server"
                                    style="width: 100%; height: 100%; border: none; border-radius: 12px;"
                                    allowfullscreen="allowfullscreen"></iframe>
                            </div>
                        </asp:Panel>

                        <!-- NO VIDEO — placeholder -->
                        <asp:Panel ID="pnlNoVideo" runat="server" Visible="true">
                            <div class="video-box">
                                <div class="video-icon">▶</div>
                                <div class="video-text">No video uploaded for this lesson yet</div>
                            </div>
                        </asp:Panel>

                        <!-- FILE MATERIALS -->
                        <asp:Panel ID="pnlFiles" runat="server" Visible="false">
                            <div class="files-section">
                                <div class="files-title">📎 Lesson Materials</div>
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
                            Text="✓ Mark as Completed (+10 Points)"
                            CssClass="btn-complete"
                            OnClick="btnComplete_Click" />
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