<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LessonViewer.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.LessonViewer" %>
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
            --accent-orange: #f59e0b;
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

        /* ═══ HEADER ═══ */
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82); backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 12px; text-decoration: none;}
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
            font-size: 13px; font-weight: 700; color: white; overflow: hidden; position: relative;
        }
        .user-avatar img { width: 100%; height: 100%; object-fit: cover; position: absolute; top: 0; left: 0; }
        .user-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }
        .btn-logout {
            background: transparent; border: 1px solid var(--border);
            color: var(--text-secondary); padding: 7px 18px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-logout:hover { background: #fef2f2; color: #ef4444; border-color: #fecaca; }

        /* ═══ NAV ═══ */
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

        /* ═══ LAYOUT ═══ */
        .page-wrap {
            max-width: 1140px; margin: 0 auto; padding: 28px 36px;
            display: flex; gap: 22px; align-items: flex-start;
        }

        /* ═══ SIDEBAR ═══ */
        .sidebar { width: 290px; flex-shrink: 0; position: sticky; top: 90px; max-height: calc(100vh - 120px); overflow-y: auto; }
        .sidebar::-webkit-scrollbar { width: 6px; }
        .sidebar::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 10px; }
        
        .sidebar-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); overflow: hidden; box-shadow: var(--shadow-sm);
        }
        .sidebar-header { padding: 18px 20px; border-bottom: 1px solid var(--border); background: var(--surface-hover); }
        .sidebar-course-label {
            font-size: 0.68rem; font-weight: 700; letter-spacing: 1.5px;
            text-transform: uppercase; color: var(--primary); margin-bottom: 6px;
            font-family: 'Space Mono', monospace;
        }
        .sidebar-course-name { font-size: 0.95rem; font-weight: 700; line-height: 1.3; color: var(--text); }
        .back-link {
            display: inline-flex; align-items: center; gap: 6px;
            color: var(--text-muted); text-decoration: none; font-size: 0.78rem;
            margin-top: 12px; font-weight: 600; transition: color 0.2s;
        }
        .back-link:hover { color: var(--primary); }

        .module-section { border-bottom: 1px solid var(--border); }
        .module-section:last-child { border-bottom: none; }
        .module-title {
            padding: 12px 20px; font-size: 0.75rem; font-weight: 700;
            letter-spacing: 0.5px; text-transform: uppercase;
            color: var(--text-muted); background: var(--border-light);
        }
        
        .lesson-item {
            padding: 12px 20px; font-size: 0.85rem; cursor: pointer;
            display: flex; align-items: center; gap: 10px; transition: background 0.2s;
            border-bottom: 1px solid var(--border-light); text-decoration: none; color: var(--text);
        }
        .lesson-item:last-child { border-bottom: none; }
        .lesson-item:hover { background: var(--surface-hover); }
        .lesson-item.active { background: var(--primary-bg); color: var(--primary); font-weight: 600; border-left: 3px solid var(--primary); padding-left: 17px;}
        .lesson-item.completed { color: var(--accent-green); }
        
        .lesson-dot { width: 8px; height: 8px; border-radius: 50%; background: var(--border); flex-shrink: 0; }
        .lesson-item.completed .lesson-dot { background: var(--accent-green); }
        .lesson-item.active .lesson-dot { background: var(--primary); }
        .exam-dot { background: var(--accent-orange) !important; }

        /* ═══ MAIN CONTENT ═══ */
        .content { flex: 1; min-width: 0; }
        .content-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 36px; box-shadow: var(--shadow-sm);
            animation: slideUp 0.4s ease both;
        }
        .lesson-title { font-size: 1.5rem; font-weight: 700; margin-bottom: 12px; color: var(--text); }
        .lesson-desc { color: var(--text-secondary); font-size: 0.9rem; line-height: 1.7; margin-bottom: 28px; }

        /* Video */
        .video-box {
            background: #0f172a; border-radius: 12px; aspect-ratio: 16 / 9;
            display: flex; flex-direction: column; align-items: center; justify-content: center;
            margin-bottom: 28px; overflow: hidden; box-shadow: var(--shadow-md);
        }
        .no-video-box {
            background: var(--border-light); border-radius: 12px; padding: 40px;
            display: flex; flex-direction: column; align-items: center; justify-content: center;
            margin-bottom: 28px; border: 1px dashed var(--border); color: var(--text-muted);
        }
        .video-icon { font-size: 2.5rem; margin-bottom: 10px; opacity: 0.5; }
        .video-text { font-size: 0.85rem; font-weight: 500; }

        /* Materials */
        .files-section { margin-bottom: 30px; }
        .files-title { font-size: 0.9rem; font-weight: 700; margin-bottom: 12px; color: var(--text); }
        .file-item {
            display: flex; align-items: center; gap: 12px;
            background: var(--surface-hover); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 14px 18px; margin-bottom: 10px;
            text-decoration: none; color: var(--text); transition: border-color 0.2s;
        }
        .file-item:hover { border-color: var(--primary); background: var(--surface); box-shadow: var(--shadow-sm); }
        .file-icon { font-size: 1.2rem; }
        .file-name { flex: 1; font-size: 0.85rem; font-weight: 600; }
        .file-arrow { color: var(--primary); font-weight: 700; }

        /* Buttons & Alerts */
        .btn-complete {
            background: var(--accent-green); color: white; border: none;
            padding: 14px 28px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.95rem; font-weight: 700;
            cursor: pointer; transition: background 0.2s;
            display: inline-flex; align-items: center; gap: 8px;
        }
        .btn-complete:hover { background: #059669; }
        .btn-complete:disabled {
            background: var(--border-light); color: var(--text-muted);
            cursor: not-allowed; border: 1px solid var(--border);
        }
        
        .btn-primary {
            background: var(--primary); color: white; border: none;
            padding: 14px 28px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.95rem; font-weight: 700;
            cursor: pointer; transition: background 0.2s; text-decoration: none; display: inline-block;
        }
        .btn-primary:hover { background: #1d4ed8; }

        .alert-success {
            background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2);
            color: #059669; padding: 14px 20px; border-radius: var(--radius-sm);
            font-size: 0.85rem; font-weight: 600; margin-top: 18px; display: inline-block;
        }

        /* Empty / Select State */
        .select-state { text-align: center; padding: 80px 20px; color: var(--text-muted); }
        .select-state-icon { font-size: 3rem; margin-bottom: 16px; opacity: 0.4; }
        .select-state h3 { font-size: 1.2rem; font-weight: 700; color: var(--text); margin-bottom: 8px; }
        .select-state p { font-size: 0.9rem; }

        .no-lessons { text-align: center; padding: 60px 20px; color: var(--text-muted); }
        .no-lessons-icon { font-size: 3rem; margin-bottom: 14px; opacity: 0.4; }
        .no-lessons p { font-size: 0.9rem; margin-bottom: 20px; }
        .no-lessons a {
            display: inline-block; background: var(--primary); color: white;
            text-decoration: none; padding: 10px 24px; border-radius: 8px; font-size: 0.85rem; font-weight: 600;
        }

        /* Exam Meta Box */
        .exam-meta-box {
            background: var(--border-light); border: 1px dashed var(--border);
            padding: 20px; border-radius: var(--radius-sm); margin-bottom: 24px;
            display: flex; gap: 30px; font-size: 0.9rem; color: var(--text-secondary); font-weight: 500;
        }
        .exam-meta-box strong { color: var(--text); font-family: 'Space Mono', monospace; font-size: 1.1rem; display: block; margin-top: 4px;}

        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 900px) { .page-wrap { flex-direction: column; padding: 20px; } .sidebar { width: 100%; position: static; max-height: none; } }
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
                        <asp:Image ID="imgAvatar" runat="server" Visible="false" />
                        <asp:Label ID="lblAvatarInitial" runat="server" Text="G" />
                    </div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="nav">
            <a href="GeneralDashboard.aspx"><span>📊</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>📚</span> Browse Courses</a>
            <a href="MyCourses.aspx" class="active"><span>🎓</span> My Learning</a>
            <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
            <a href="EditProfile.aspx"><span>👤</span> Profile</a>
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

                                    <asp:Repeater ID="rptModuleExams" runat="server" DataSource='<%# Eval("Exams") %>'>
                                        <ItemTemplate>
                                            <a href='LessonViewer.aspx?courseid=<%# Request.QueryString["courseid"] %>&examId=<%# Eval("ExamId") %>'
                                               class='lesson-item <%# Convert.ToBoolean(Eval("IsCompleted")) ? "completed" : "" %> <%# Request.QueryString["examId"] == Eval("ExamId").ToString() ? "active" : "" %>'>
                                                <div class="lesson-dot exam-dot"></div>
                                                📝 <%# Eval("ExamTitle") %>
                                            </a>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:Repeater ID="rptCourseExams" runat="server">
                            <HeaderTemplate><div class="module-section"><div class="module-title">Final Exams</div></HeaderTemplate>
                            <ItemTemplate>
                                <a href='LessonViewer.aspx?courseid=<%# Request.QueryString["courseid"] %>&examId=<%# Eval("ExamId") %>'
                                    class='lesson-item <%# Convert.ToBoolean(Eval("IsCompleted")) ? "completed" : "" %> <%# Request.QueryString["examId"] == Eval("ExamId").ToString() ? "active" : "" %>'>
                                    <div class="lesson-dot exam-dot"></div>
                                    📝 <%# Eval("ExamTitle") %>
                                </a>
                            </ItemTemplate>
                            <FooterTemplate></div></FooterTemplate>
                        </asp:Repeater>
                    </asp:Panel>

                    <asp:Panel ID="pnlNoModules" runat="server" Visible="false">
                        <div class="no-lessons">
                            <div class="no-lessons-icon">📭</div>
                            <p>No content added yet for this course.</p>
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
                            <div class="no-video-box">
                                <div class="video-icon">▶</div>
                                <div class="video-text">No video available for this lesson</div>
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

                        <asp:Button ID="btnComplete" runat="server" Text="Mark as Completed (+10 Points)" CssClass="btn-complete" OnClick="btnComplete_Click" />
                        <br />
                        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="alert-success" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlExamIntro" runat="server" Visible="false">
                    <div class="content-card">
                        <div class="lesson-title">📝 <asp:Label ID="lblExamTitle" runat="server" /></div>
                        <div class="lesson-desc">Ready to test your knowledge? Ensure you have a stable connection before beginning the exam.</div>
                        
                        <div class="exam-meta-box">
                            <div>
                                Time Limit:
                                <strong><asp:Label ID="lblExamDuration" runat="server" /> mins</strong>
                            </div>
                            <div>
                                Total Possible Points:
                                <strong><asp:Label ID="lblExamMarks" runat="server" /> pts</strong>
                            </div>
                        </div>

                        <asp:Button ID="btnStartExam" runat="server" Text="Start Exam Now →" CssClass="btn-primary" OnClick="btnStartExam_Click" />
                        <br /><br />
                        <asp:Label ID="lblExamMessage" runat="server" Visible="false" CssClass="alert-success" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlSelectLesson" runat="server" Visible="true">
                    <div class="content-card">
                        <div class="select-state">
                            <div class="select-state-icon">👈</div>
                            <h3>Select a module to start</h3>
                            <p>Choose a lesson or exam from the sidebar to begin learning.</p>
                        </div>
                    </div>
                </asp:Panel>

            </div>
        </div>
        
        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>
</body>
</html>