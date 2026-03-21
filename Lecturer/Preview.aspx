<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Preview.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.Preview" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Preview - LearnSphere</title>
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
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
            --accent-orange: #f59e0b;
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
            background: var(--bg-gradient); background-attachment: fixed;
            color: var(--text); min-height: 100vh;
        }

        /* ═══ HEADER ═══ */
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82); backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 24px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 12px; }
        .logo img { height: 36px; width: 36px; object-fit: contain; }
        .logo-text { font-size: 1.1rem; font-weight: 700; color: var(--text); }
        .logo-text span { color: var(--primary); }
        .header-right { display: flex; align-items: center; gap: 12px; }
        .preview-pill {
            background: rgba(245,158,11,0.1); border: 1px solid rgba(245,158,11,0.3);
            color: var(--accent-orange); font-size: 0.7rem; font-weight: 700;
            padding: 4px 12px; border-radius: 20px; letter-spacing: 0.8px;
            text-transform: uppercase; font-family: 'Space Mono', monospace;
        }
        .btn-back-header {
            background: var(--surface); border: 1px solid var(--border);
            color: var(--text-secondary); padding: 7px 16px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s; text-decoration: none;
            display: inline-flex; align-items: center; gap: 6px;
        }
        .btn-back-header:hover { border-color: var(--primary); color: var(--primary); }

        /* ═══ PREVIEW LAYOUT ═══ */
        .preview-layout {
            display: grid;
            grid-template-columns: 280px 1fr;
            min-height: calc(100vh - 64px);
        }

        /* ═══ SIDEBAR ═══ */
        .preview-sidebar {
            background: var(--surface);
            border-right: 1px solid var(--border);
            overflow-y: auto;
            position: sticky; top: 64px;
            height: calc(100vh - 64px);
        }
        .sidebar-header {
            padding: 20px 20px 14px;
            border-bottom: 1px solid var(--border);
        }
        .sidebar-label {
            font-size: 0.65rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: var(--text-muted);
            font-family: 'Space Mono', monospace;
        }
        .sidebar-course-title {
            font-size: 0.88rem; font-weight: 700; color: var(--text);
            margin-top: 4px; line-height: 1.4;
        }

        /* Progress bar in sidebar */
        .sidebar-progress { padding: 12px 20px; border-bottom: 1px solid var(--border); }
        .progress-label { font-size: 0.7rem; color: var(--text-muted); margin-bottom: 6px; font-family: 'Space Mono', monospace; }
        .progress-track {
            height: 6px; background: var(--border); border-radius: 10px; overflow: hidden;
        }
        .progress-fill {
            height: 100%; background: linear-gradient(90deg, var(--primary), var(--accent-green));
            border-radius: 10px; transition: width 0.4s ease;
        }
        .progress-text {
            font-size: 0.68rem; color: var(--text-muted); margin-top: 4px;
            font-family: 'Space Mono', monospace; text-align: right;
        }

        /* Module + lesson list */
        .sidebar-modules { padding: 10px 0; }
        .module-block { margin-bottom: 4px; }
        .module-title {
            padding: 10px 20px 6px;
            font-size: 0.72rem; font-weight: 700; color: var(--text-muted);
            text-transform: uppercase; letter-spacing: 0.8px;
            display: flex; align-items: center; gap: 8px;
        }
        .module-title::before {
            content: ''; width: 6px; height: 6px; border-radius: 50%;
            background: var(--primary); flex-shrink: 0;
        }
        .lesson-item a {
            display: flex; align-items: center; gap: 10px;
            padding: 9px 20px 9px 34px;
            font-size: 0.83rem; color: var(--text-secondary);
            text-decoration: none; transition: all 0.15s;
            border-left: 3px solid transparent;
        }
        .lesson-item a:hover {
            background: var(--border-light); color: var(--primary);
            border-left-color: var(--primary-border);
        }
        .lesson-item a.active {
            background: var(--primary-bg); color: var(--primary);
            border-left-color: var(--primary); font-weight: 600;
        }
        .lesson-dot {
            width: 6px; height: 6px; border-radius: 50%;
            background: var(--border); flex-shrink: 0; margin-top: 1px;
        }
        .lesson-item a.active .lesson-dot { background: var(--primary); }

        /* ═══ MAIN CONTENT ═══ */
        .preview-content { padding: 32px 40px; max-width: 860px; }

        /* overview card */
        .overview-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 40px;
            box-shadow: var(--shadow-sm);
            animation: fadeIn 0.4s ease both;
        }
        .overview-banner {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 55%, #60a5fa 100%);
            border-radius: var(--radius); padding: 32px 36px;
            margin-bottom: 28px; position: relative; overflow: hidden;
            box-shadow: 0 8px 24px rgba(37,99,235,0.22);
        }
        .overview-banner::before {
            content: ''; position: absolute; top: -40%; right: -10%;
            width: 250px; height: 250px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%);
            border-radius: 50%;
        }
        .overview-label { font-size: 0.7rem; font-weight: 700; letter-spacing: 2px; text-transform: uppercase; color: rgba(255,255,255,0.7); margin-bottom: 6px; font-family: 'Space Mono', monospace; }
        .overview-title { font-size: 1.6rem; font-weight: 700; color: white; margin-bottom: 6px; }
        .overview-sub { color: rgba(255,255,255,0.75); font-size: 0.85rem; }

        .overview-card p { font-size: 0.9rem; color: var(--text-secondary); line-height: 1.7; margin-bottom: 24px; }

        .continue-btn {
            display: inline-flex; align-items: center; gap: 8px;
            background: var(--primary); color: white; text-decoration: none;
            padding: 12px 28px; border-radius: 8px;
            font-size: 0.88rem; font-weight: 600; transition: background 0.2s;
        }
        .continue-btn:hover { background: #1d4ed8; }

        /* lesson view */
        .lesson-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 36px;
            box-shadow: var(--shadow-sm);
            animation: fadeIn 0.4s ease both;
        }
        .lesson-card h2 {
            font-size: 1.4rem; font-weight: 700; margin-bottom: 16px; color: var(--text);
        }
        .lesson-card p {
            font-size: 0.9rem; color: var(--text-secondary); line-height: 1.75;
            margin-bottom: 24px;
        }
        .video-wrapper {
            border-radius: var(--radius-sm); overflow: hidden;
            margin-bottom: 24px; background: #000;
            box-shadow: var(--shadow-md);
        }
        .video-wrapper iframe { display: block; width: 100%; height: 460px; }
        .doc-link {
            display: inline-flex; align-items: center; gap: 8px;
            background: var(--border-light); border: 1px solid var(--border);
            color: var(--primary); text-decoration: none;
            padding: 9px 18px; border-radius: 8px;
            font-size: 0.83rem; font-weight: 600; margin-bottom: 8px;
            transition: all 0.2s;
        }
        .doc-link:hover { background: var(--primary-bg); border-color: var(--primary-border); }

        /* navigation row */
        .lesson-nav {
            display: flex; justify-content: space-between; align-items: center;
            margin-top: 32px; padding-top: 20px; border-top: 1px solid var(--border);
            flex-wrap: wrap; gap: 10px;
        }
        .nav-btn {
            display: inline-flex; align-items: center; gap: 8px;
            background: var(--surface); border: 1px solid var(--border);
            color: var(--text-secondary); text-decoration: none;
            padding: 10px 22px; border-radius: 8px;
            font-size: 0.85rem; font-weight: 600; transition: all 0.2s;
        }
        .nav-btn:hover { border-color: var(--primary); color: var(--primary); }
        .nav-btn.next {
            background: var(--primary); color: white; border-color: var(--primary);
        }
        .nav-btn.next:hover { background: #1d4ed8; }

        /* completion card */
        .completion-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 60px 40px;
            box-shadow: var(--shadow-sm); text-align: center;
            animation: fadeIn 0.4s ease both;
        }
        .completion-icon { font-size: 4rem; margin-bottom: 20px; }
        .completion-card h2 {
            font-size: 1.8rem; font-weight: 700; margin-bottom: 10px; color: var(--text);
        }
        .completion-card p {
            font-size: 0.9rem; color: var(--text-secondary); margin-bottom: 28px;
        }
        .back-courses-btn {
            display: inline-flex; align-items: center; gap: 8px;
            background: var(--accent-green); color: white; text-decoration: none;
            padding: 12px 28px; border-radius: 8px;
            font-size: 0.88rem; font-weight: 600; transition: background 0.2s;
        }
        .back-courses-btn:hover { background: #059669; }

        /* ═══ ANIMATIONS ═══ */
        @keyframes fadeIn { from{opacity:0;transform:translateY(10px);}to{opacity:1;transform:translateY(0);} }

        @media(max-width:900px){
            .preview-layout { grid-template-columns: 1fr; }
            .preview-sidebar { position: relative; height: auto; }
            .preview-content { padding: 20px; }
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
            <span class="preview-pill">👁 Preview Mode</span>
            <asp:Button ID="btnBack" runat="server" Text="← Back to Courses"
                CssClass="btn-back-header" OnClick="btnBack_Click" CausesValidation="false" />
        </div>
    </div>

    <!-- ═══ PREVIEW LAYOUT ═══ -->
    <div class="preview-layout">

        <!-- LEFT: Module/Lesson sidebar -->
        <div class="preview-sidebar">

            <div class="sidebar-header">
                <div class="sidebar-label">Course Preview</div>
                <div class="sidebar-course-title">
                    <asp:Label ID="lblCourseName" runat="server" />
                </div>
            </div>

            <!-- Progress bar (populated by code-behind) -->
            <div class="sidebar-progress">
                <div class="progress-label">Progress</div>
                <div class="progress-track">
                    <asp:Literal ID="litProgressFill" runat="server" />
                </div>
                <div class="progress-text">
                    <asp:Literal ID="litProgressText" runat="server" />
                </div>
            </div>

            <!-- Modules + Lessons -->
            <div class="sidebar-modules">
                <asp:Repeater ID="rptModules" runat="server">
                    <ItemTemplate>
                        <div class="module-block">
                            <div class="module-title"><%# Eval("modulename") %></div>
                            <asp:Repeater ID="rptLessons" runat="server"
                                DataSource='<%# Eval("Lessons") %>'>
                                <ItemTemplate>
                                    <div class="lesson-item">
                                        <a href='Preview.aspx?courseid=<%# Request.QueryString["courseid"] %>&lessonid=<%# Eval("lessonid") %>'>
                                            <span class="lesson-dot"></span>
                                            <%# Eval("lessontitle") %>
                                        </a>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- RIGHT: Content area -->
        <div class="preview-content">
            <asp:PlaceHolder ID="phOverview"    runat="server" />
            <asp:PlaceHolder ID="phLesson"      runat="server" />
            <asp:PlaceHolder ID="phCompletion"  runat="server" />
        </div>

    </div>

</form>
</body>
</html>
