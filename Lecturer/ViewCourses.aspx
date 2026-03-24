<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewCourses.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.ViewCourses" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Courses - LearnSphere</title>
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
            --accent-red: #ef4444;
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
            background: linear-gradient(135deg, #10b981, var(--accent-purple));
            border-radius: 50%; overflow: hidden; position: relative;
        }
        .user-avatar img { width:100%; height:100%; object-fit:cover; border-radius:50%; position:absolute; top:0; left:0; }
        .user-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }
        .verified-badge {
            display: inline-flex; align-items: center; gap: 5px;
            background: rgba(16,185,129,0.1); border: 1px solid rgba(16,185,129,0.25);
            color: #059669; font-size: 0.72rem; font-weight: 700;
            padding: 4px 12px; border-radius: 20px;
        }
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
        .nav-badge {
            background: #ef4444; color: white; font-size: 0.65rem; font-weight: 700;
            padding: 1px 6px; border-radius: 10px; min-width: 18px; text-align: center;
        }

        /* ═══ CONTAINER ═══ */
        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }

        /* ═══ BANNERS ═══ */
        .page-banner {
            border-radius: var(--radius); padding: 28px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            animation: slideDown 0.5s ease both;
        }
        .banner-courses { background: linear-gradient(135deg,#2563eb,#3b82f6 55%,#60a5fa); box-shadow:0 8px 30px rgba(37,99,235,0.25); }
        .banner-edit    { background: linear-gradient(135deg,#059669,#10b981 55%,#34d399); box-shadow:0 8px 30px rgba(16,185,129,0.28); }
        .banner-module  { background: linear-gradient(135deg,#f59e0b,#fbbf24 55%,#fcd34d); box-shadow:0 8px 30px rgba(245,158,11,0.28); }
        .banner-lesson  { background: linear-gradient(135deg,#7c3aed,#8b5cf6 55%,#a78bfa); box-shadow:0 8px 30px rgba(139,92,246,0.25); }
        .banner-students{ background: linear-gradient(135deg,#0891b2,#06b6d4 55%,#67e8f9); box-shadow:0 8px 30px rgba(8,145,178,0.28); }
        .banner-review  { background: linear-gradient(135deg,#dc2626,#ef4444 55%,#f87171); box-shadow:0 8px 30px rgba(220,38,38,0.28); }
        .page-banner::before {
            content:''; position:absolute; top:-40%; right:-10%; width:280px; height:280px;
            background:radial-gradient(circle,rgba(255,255,255,0.12),transparent 65%);
            border-radius:50%; pointer-events:none;
        }
        .banner-label { font-size:0.72rem; font-weight:700; letter-spacing:2px; text-transform:uppercase; color:rgba(255,255,255,0.75); margin-bottom:6px; font-family:'Space Mono',monospace; }
        .banner-title { font-size:1.5rem; font-weight:700; color:white; margin-bottom:4px; }
        .banner-sub   { color:rgba(255,255,255,0.75); font-size:0.85rem; }

        /* ═══ BACK BUTTON ═══ */
        .btn-back {
            display:inline-flex; align-items:center; gap:8px;
            background:var(--surface); border:1px solid var(--border);
            color:var(--text-secondary); padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.83rem; font-weight:600;
            cursor:pointer; transition:all 0.2s; margin-bottom:20px;
        }
        .btn-back:hover { border-color:var(--primary); color:var(--primary); }

        /* ═══ FILTER BAR ═══ */
        .filter-bar {
            display:flex; gap:10px; flex-wrap:wrap;
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:16px 20px;
            margin-bottom:20px; box-shadow:var(--shadow-sm);
        }
        .filter-input {
            flex:1; min-width:130px; background:var(--border-light);
            border:1px solid var(--border); border-radius:var(--radius-sm);
            padding:9px 14px; font-family:'DM Sans',sans-serif; font-size:0.875rem;
            outline:none; transition:border-color 0.2s; color:var(--text);
        }
        .filter-input:focus { border-color:var(--primary); background:white; }

        /* ═══ SECTION CARD ═══ */
        .section {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); box-shadow:var(--shadow-sm);
            overflow:hidden; margin-bottom:20px;
            animation:slideUp 0.5s ease both;
        }
        /* Wide tables (e.g. students list) get horizontal scroll instead of truncating */
        .section-wide {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); box-shadow:var(--shadow-sm);
            margin-bottom:20px; animation:slideUp 0.5s ease both;
        }
        .section-wide .section-header {
            padding:16px 24px; border-bottom:1px solid var(--border);
            display:flex; align-items:center; justify-content:space-between;
        }
        .section-wide .table-scroll {
            overflow-x:auto; width:100%;
        }
        .section-wide table { width:100%; border-collapse:collapse; min-width:900px; }
        .section-wide table th {
            background:var(--border-light); padding:10px 14px; text-align:left;
            font-size:0.68rem; font-weight:700; letter-spacing:0.8px; text-transform:uppercase;
            color:var(--text-muted); border-bottom:1px solid var(--border);
            white-space:nowrap;
        }
        .section-wide table td {
            padding:11px 14px; font-size:0.82rem; color:var(--text);
            border-bottom:1px solid var(--border-light); vertical-align:middle;
            white-space:nowrap;
        }
        .section-wide table tr:last-child td { border-bottom:none; }
        .section-wide table tr:hover td { background:var(--surface-hover); }
        .section-header {
            padding:16px 24px; border-bottom:1px solid var(--border);
            display:flex; align-items:center; justify-content:space-between;
        }
        .section-title { font-size:0.9rem; font-weight:700; color:var(--text); display:flex; align-items:center; gap:10px; }
        .title-dot { width:8px; height:8px; border-radius:50%; display:inline-block; flex-shrink:0; }
        .dot-blue   { background:var(--primary); }
        .dot-green  { background:var(--accent-green); }
        .dot-orange { background:var(--accent-orange); }
        .dot-purple { background:var(--accent-purple); }
        .dot-red    { background:var(--accent-red); }
        .dot-cyan   { background:#0891b2; }

        /* ═══ TABLES ═══ */
        .section table { width:100%; border-collapse:collapse; }
        .section table th {
            background:var(--border-light); padding:10px 20px; text-align:left;
            font-size:0.72rem; font-weight:700; letter-spacing:1px; text-transform:uppercase;
            color:var(--text-muted); border-bottom:1px solid var(--border);
        }
        .section table td {
            padding:13px 20px; font-size:0.875rem; color:var(--text);
            border-bottom:1px solid var(--border-light); vertical-align:middle;
        }
        .section table tr:last-child td { border-bottom:none; }
        .section table tr:hover td { background:var(--surface-hover); }

        /* ═══ BADGES ═══ */
        .badge {
            display:inline-block; padding:3px 12px; border-radius:20px;
            font-size:0.72rem; font-weight:700; letter-spacing:0.3px;
        }
        .badge-published { background:rgba(16,185,129,0.1); color:#059669; border:1px solid rgba(16,185,129,0.25); }
        .badge-draft     { background:rgba(245,158,11,0.1); color:var(--accent-orange); border:1px solid rgba(245,158,11,0.25); }
        .badge-deleted   { background:rgba(239,68,68,0.08); color:var(--accent-red); border:1px solid rgba(239,68,68,0.2); }
        .badge-free      { background:rgba(37,99,235,0.08); color:var(--primary); border:1px solid rgba(37,99,235,0.2); }

        /* ═══ BUTTONS ═══ */
        .btn-primary {
            background:var(--primary); color:white; border:none;
            padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.82rem; font-weight:600;
            cursor:pointer; transition:background 0.2s;
        }
        .btn-primary:hover { background:#1d4ed8; }
        .btn-secondary {
            background:var(--border-light); color:var(--text-secondary);
            border:1px solid var(--border); padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.82rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-secondary:hover { border-color:var(--primary); color:var(--primary); }
        .btn-success {
            background:var(--accent-green); color:white; border:none;
            padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.82rem; font-weight:600;
            cursor:pointer; transition:background 0.2s;
        }
        .btn-success:hover { background:#059669; }
        .btn-warning {
            background:rgba(245,158,11,0.1); color:var(--accent-orange);
            border:1px solid rgba(245,158,11,0.3); padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.82rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-warning:hover { background:var(--accent-orange); color:white; }
        .btn-danger {
            background:rgba(239,68,68,0.08); color:var(--accent-red);
            border:1px solid rgba(239,68,68,0.2); padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.82rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-danger:hover { background:var(--accent-red); color:white; }
        .btn-sm {
            padding:5px 12px; font-size:0.75rem; border-radius:7px;
        }
        .btn-actions { display:flex; gap:8px; flex-wrap:wrap; }
        .btn-create {
            display:inline-block; background:var(--primary); color:white;
            text-decoration:none; padding:9px 20px; border-radius:8px;
            font-size:0.83rem; font-weight:600; transition:background 0.2s;
        }
        .btn-create:hover { background:#1d4ed8; }

        /* ═══ COURSE INFO (edit view) ═══ */
        .course-info-banner {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:22px 28px; margin-bottom:20px;
            box-shadow:var(--shadow-sm);
        }
        .course-info-banner h3 { font-size:1.1rem; font-weight:700; margin-bottom:6px; }
        .course-info-banner p { font-size:0.875rem; color:var(--text-secondary); margin-bottom:4px; }
        .course-action-row { display:flex; gap:10px; flex-wrap:wrap; margin-top:16px; padding-top:16px; border-top:1px solid var(--border-light); }

        /* ═══ MODULE CARDS ═══ */
        .module-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:20px 24px; margin-bottom:16px;
            box-shadow:var(--shadow-sm);
        }
        .module-card-header {
            display:flex; align-items:center; justify-content:space-between;
            margin-bottom:14px; flex-wrap:wrap; gap:10px;
        }
        .module-card-title { font-size:0.95rem; font-weight:700; color:var(--text); display:flex; align-items:center; gap:10px; }
        .module-tag {
            background:var(--primary-bg); color:var(--primary); border:1px solid var(--primary-border);
            font-size:0.65rem; font-weight:700; padding:2px 10px; border-radius:20px;
            font-family:'Space Mono',monospace; text-transform:uppercase;
        }
        .lesson-table { width:100%; border-collapse:collapse; margin-top:8px; }
        .lesson-table th {
            background:var(--border-light); padding:8px 14px; text-align:left;
            font-size:0.7rem; font-weight:700; letter-spacing:1px; text-transform:uppercase;
            color:var(--text-muted); border-bottom:1px solid var(--border);
        }
        .lesson-table td {
            padding:10px 14px; font-size:0.83rem; color:var(--text);
            border-bottom:1px solid var(--border-light); vertical-align:middle;
        }
        .lesson-table tr:last-child td { border-bottom:none; }
        .lesson-table tr:hover td { background:var(--surface-hover); }
        .points-pill {
            display:inline-flex; align-items:center; gap:4px;
            background:rgba(245,158,11,0.1); border:1px solid rgba(245,158,11,0.25);
            color:var(--accent-orange); font-size:0.7rem; font-weight:700;
            padding:2px 10px; border-radius:20px; font-family:'Space Mono',monospace;
        }

        /* ═══ FORM CARD ═══ */
        .form-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:32px;
            box-shadow:var(--shadow-sm); margin-bottom:20px;
            animation:slideUp 0.5s ease both;
        }
        .form-card-title { font-size:1rem; font-weight:700; margin-bottom:6px; display:flex; align-items:center; gap:10px; }
        .form-card-sub { font-size:0.83rem; color:var(--text-secondary); margin-bottom:24px; }
        .form-group { margin-bottom:18px; }
        .form-row { display:grid; grid-template-columns:1fr 1fr; gap:16px; margin-bottom:18px; }
        .form-label {
            display:block; font-size:0.75rem; font-weight:600; color:var(--text-secondary);
            text-transform:uppercase; letter-spacing:0.5px; margin-bottom:7px;
        }
        .form-input {
            width:100%; background:var(--border-light); border:1px solid var(--border);
            border-radius:var(--radius-sm); padding:10px 14px;
            font-family:'DM Sans',sans-serif; font-size:0.875rem;
            color:var(--text); outline:none; transition:border-color 0.2s; box-sizing:border-box;
        }
        .form-input:focus { border-color:var(--primary); background:white; }
        textarea.form-input { resize:vertical; min-height:90px; line-height:1.5; }
        select.form-input {
            appearance:none; cursor:pointer;
            background-image:url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%2394a3b8' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
            background-repeat:no-repeat; background-position:right 14px center; padding-right:36px;
        }
        input[type="file"].form-input { padding:8px 14px; cursor:pointer; }
        .form-hint { font-size:0.72rem; color:var(--text-muted); margin-top:5px; }
        .validation-error { font-size:0.75rem; color:#dc2626; margin-top:4px; display:block; }
        .validation-summary {
            background:rgba(239,68,68,0.06); border:1px solid rgba(239,68,68,0.2);
            color:#dc2626; border-radius:var(--radius-sm);
            padding:14px 18px; font-size:0.83rem; margin-bottom:20px;
        }
        .btn-row { display:flex; gap:10px; margin-top:24px; flex-wrap:wrap; align-items:center; }

        /* ═══ ALERTS ═══ */
        .alert { padding:10px 16px; border-radius:var(--radius-sm); font-size:0.83rem; font-weight:500; margin:10px 0; display:inline-block; }
        .alert-success { background:rgba(16,185,129,0.08); border:1px solid rgba(16,185,129,0.2); color:#059669; }
        .alert-error   { background:rgba(239,68,68,0.07); border:1px solid rgba(239,68,68,0.2); color:#dc2626; }
        .empty-msg { display:block; text-align:center; padding:30px; color:var(--text-muted); font-size:0.88rem; }

        /* ═══ REVIEW PANEL ═══ */
        .review-info-card {
            background:var(--border-light); border:1px solid var(--border);
            border-radius:var(--radius-sm); padding:18px 20px; margin-bottom:20px;
        }
        .review-info-label { font-size:0.72rem; font-weight:600; color:var(--text-muted); text-transform:uppercase; letter-spacing:0.5px; margin-bottom:3px; }
        .review-info-value { font-weight:600; color:var(--text); font-size:0.875rem; margin-bottom:12px; }
        .review-info-value:last-child { margin-bottom:0; }
        .module-review-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius-sm); padding:16px 20px; margin-bottom:12px;
        }
        .module-review-title { font-size:0.9rem; font-weight:700; color:var(--text); margin-bottom:10px; display:flex; align-items:center; gap:8px; }
        .lesson-item-row {
            display:flex; justify-content:space-between; align-items:center;
            padding:7px 12px; background:var(--border-light);
            border-radius:8px; margin-bottom:5px; font-size:0.82rem;
        }
        .lesson-item-row:last-child { margin-bottom:0; }
        .lesson-item-name { color:var(--text); font-weight:500; }
        .lesson-item-meta { display:flex; align-items:center; gap:10px; }

        /* ═══ PUBLISH BANNER ═══ */
        .ready-banner {
            background:linear-gradient(135deg,#059669,#10b981 55%,#34d399);
            border-radius:var(--radius); padding:20px 28px; margin-bottom:20px;
            text-align:center; box-shadow:0 6px 20px rgba(16,185,129,0.25);
        }
        .ready-banner h3 { color:white; font-size:1.1rem; }
        .ready-banner p { color:rgba(255,255,255,0.85); font-size:0.83rem; margin-top:4px; }
        .draft-warning {
            background:rgba(245,158,11,0.08); border:1px solid rgba(245,158,11,0.25);
            border-radius:var(--radius-sm); padding:12px 18px; margin-bottom:16px;
            font-size:0.83rem; color:var(--accent-orange); font-weight:500;
        }

        /* ═══ ANIMATIONS ═══ */
        @keyframes slideDown { from{opacity:0;transform:translateY(-12px);}to{opacity:1;transform:translateY(0);} }
        @keyframes slideUp   { from{opacity:0;transform:translateY(14px);}to{opacity:1;transform:translateY(0);} }

        @media(max-width:900px){
            .container{padding:20px;}
            .header,.nav{padding:0 20px;}
            .filter-bar{flex-direction:column;}
            .form-row{grid-template-columns:1fr;}
        }
    </style>
</head>
<body>
<form id="form1" runat="server" enctype="multipart/form-data">

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
                <span class="user-name"><%= Session["uname"] != null ? Server.HtmlEncode(Session["uname"].ToString()) : "" %></span>
            </div>
            <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
        </div>
    </div>

    <!-- ═══ NAV ═══ -->
    <div class="nav">
        <a href="LecturerDashboard.aspx"><span>📊</span> Dashboard</a>
        <a href="CreateCourse.aspx"><span>➕</span> Create Course</a>
        <a href="ViewCourses.aspx" class="active"><span>📚</span> View Courses</a>
        <a href="EditProfile.aspx"><span>👤</span> Edit Profile</a>
        <a href="Forums.aspx"><span>💬</span> Forums</a>
        <a href="Message.aspx">
            <span>✉️</span> Messaging
            <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                <span class="nav-badge"><%= Session["unreadCount"] %></span>
            <% } %>
        </a>
    </div>

    <div class="container">

        <!-- ═══════════════════════════════════════
             PANEL 1 — VIEW COURSES
             ═══════════════════════════════════════ -->
        <asp:Panel ID="pnlViewCourses" runat="server">
            <div class="page-banner banner-courses">
                <div class="banner-label">Lecturer Portal</div>
                <div class="banner-title">Your Courses</div>
                <div class="banner-sub">Manage, edit and publish your courses.</div>
            </div>

            <div class="filter-bar">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-input" placeholder="Search course name..." />
                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="filter-input">
                    <asp:ListItem Value="">All Categories</asp:ListItem>
                    <asp:ListItem>AI</asp:ListItem>
                    <asp:ListItem>Programming</asp:ListItem>
                    <asp:ListItem>Machine Learning</asp:ListItem>
                    <asp:ListItem>Web Development</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="filter-input">
                    <asp:ListItem Value="">All Status</asp:ListItem>
                    <asp:ListItem Value="Active">Published</asp:ListItem>
                    <asp:ListItem Value="Unactive">Draft</asp:ListItem>
                    <asp:ListItem Value="Deleted">Deleted</asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="txtMinPrice" runat="server" CssClass="filter-input" placeholder="Min Price" />
                <asp:TextBox ID="txtMaxPrice" runat="server" CssClass="filter-input" placeholder="Max Price" />
                <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn-primary" OnClick="btnFilter_Click" />
                <asp:Button ID="btnReset"  runat="server" Text="Reset"        CssClass="btn-secondary" OnClick="btnReset_Click" />
            </div>

            <div class="section">
                <div class="section-header">
                    <div class="section-title"><span class="title-dot dot-blue"></span> All Courses</div>
                    <a href="CreateCourse.aspx" class="btn-create">+ New Course</a>
                </div>
                <asp:GridView ID="gvCourses" runat="server"
                    AutoGenerateColumns="False" Width="100%"
                    BorderStyle="None" GridLines="None"
                    DataKeyNames="courseid"
                    OnRowCommand="gvCourses_RowCommand"
                    EmptyDataText="No courses found.">
                    <Columns>
                        <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                        <asp:BoundField DataField="category"   HeaderText="Category" />
                        <asp:BoundField DataField="price"      HeaderText="Price (RM)" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='badge <%# Eval("statusText").ToString() == "Published" ? "badge-published" : (Eval("statusText").ToString() == "Deleted" ? "badge-deleted" : "badge-draft") %>'>
                                    <%# Eval("statusText") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="btn-actions">
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-primary btn-sm"
                                        CommandName="EditCourse" CommandArgument="<%# Container.DataItemIndex %>" />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-danger btn-sm"
                                        CommandName="DeleteCourse" CommandArgument="<%# Container.DataItemIndex %>"
                                        OnClientClick="return confirm('Delete this course?');" />
                                    <asp:Button ID="btnViewStudents" runat="server" Text="Students" CssClass="btn-secondary btn-sm"
                                        CommandName="ViewStudents" CommandArgument="<%# Container.DataItemIndex %>" />
                                    <asp:Button ID="btnPreview" runat="server" Text="Preview" CssClass="btn-warning btn-sm"
                                        CommandName="PreviewCourse" CommandArgument="<%# Container.DataItemIndex %>"
                                        Visible='<%# Eval("statusText").ToString() == "Published" %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <asp:Label ID="lblCoursesMsg" runat="server" CssClass="alert" Visible="false" />
        </asp:Panel>

        <!-- ═══════════════════════════════════════
             PANEL 2 — EDIT COURSE (modules + lessons)
             ═══════════════════════════════════════ -->
        <asp:Panel ID="pnlEditCourse" runat="server" Visible="false">
            <div class="page-banner banner-edit">
                <div class="banner-label">Course Editor</div>
                <div class="banner-title"><asp:Label ID="lblEditBannerTitle" runat="server" Text="Edit Course" /></div>
                <div class="banner-sub">Manage modules and lessons. Unsaved changes auto-save as draft.</div>
            </div>

            <asp:Button ID="btnBackToCourses" runat="server" Text="← Back to Courses"
                CssClass="btn-back" OnClick="btnBackToCourses_Click" CausesValidation="false" />

            <!-- Course info + action buttons -->
            <div class="course-info-banner">
                <h3><asp:Label ID="lblCourseName" runat="server" /></h3>
                <p><asp:Label ID="lblCourseDescription" runat="server" /></p>
                <p><strong>Price:</strong> RM <asp:Label ID="lblCoursePrice" runat="server" /></p>
                <div class="course-action-row">
                    <asp:Button ID="btnAddModule"   runat="server" Text="+ Add Module"   CssClass="btn-primary"   OnClick="btnAddModule_Click" CausesValidation="false" />
                    <asp:Button ID="btnReview"       runat="server" Text="Review &amp; Publish" CssClass="btn-success" OnClick="btnReview_Click" CausesValidation="false" />
                    <asp:Button ID="btnCreateExam"   runat="server" Text="Create Exam"   CssClass="btn-warning"   OnClick="btnCreateExam_Click" CausesValidation="false" />
                    <asp:Button ID="btnEditExam"     runat="server" Text="Edit Exam"     CssClass="btn-secondary" OnClick="btnEditExam_Click" CausesValidation="false" />
                    <asp:Button ID="btnDeleteExam"   runat="server" Text="Delete Exam"   CssClass="btn-danger"    OnClick="btnDeleteExam_Click" CausesValidation="false"
                        OnClientClick="return confirm('Delete this exam and all its questions?');" />
                </div>
            </div>

            <asp:Label ID="lblEditCourseMsg" runat="server" CssClass="alert" Visible="false" />

            <!-- Modules + nested Lessons -->
            <asp:Repeater ID="rptModules" runat="server"
                OnItemDataBound="rptModules_ItemDataBound"
                OnItemCommand="rptModules_ItemCommand">
                <ItemTemplate>
                    <div class="module-card">
                        <div class="module-card-header">
                            <div class="module-card-title">
                                <span class="module-tag">Module</span>
                                <%# Eval("modulename") != null ? Server.HtmlEncode(Eval("modulename").ToString()) : "" %>
                            </div>
                            <div class="btn-actions">
                                <asp:Button runat="server" Text="Edit Module" CssClass="btn-secondary btn-sm"
                                    CommandName="EditModule" CommandArgument='<%# Eval("moduleid") %>'
                                    OnCommand="Module_Command" />
                                <asp:Button runat="server" Text="Delete Module" CssClass="btn-danger btn-sm"
                                    CommandName="DeleteModule" CommandArgument='<%# Eval("moduleid") %>'
                                    OnCommand="Module_Command"
                                    OnClientClick="this.disabled=true; return confirm('Delete this module and all its lessons?');" />
                                <asp:Button runat="server" Text="+ Add Lesson" CssClass="btn-primary btn-sm"
                                    CommandName="AddLesson" CommandArgument='<%# Eval("moduleid") %>'
                                    OnCommand="Module_Command" />
                            </div>
                        </div>

                        <asp:Repeater ID="rptLessons" runat="server">
                            <HeaderTemplate>
                                <table class="lesson-table">
                                    <tr><th>Lesson Title</th><th>Duration</th><th>Points</th><th>Actions</th></tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("lessontitle") != null ? Server.HtmlEncode(Eval("lessontitle").ToString()) : "" %></td>
                                    <td style="color:var(--text-muted);font-family:'Space Mono',monospace;font-size:0.78rem;">
                                        <%# Eval("duration") %> mins
                                    </td>
                                    <td>
                                        <%# (Eval("lessonpoints") != DBNull.Value && Convert.ToInt32(Eval("lessonpoints")) > 0)
                                            ? "<span class='points-pill'>⚡ " + Eval("lessonpoints") + " pts</span>"
                                            : "<span style='color:var(--text-muted);font-size:0.78rem;'>—</span>" %>
                                    </td>
                                    <td>
                                        <div class="btn-actions">
                                            <asp:Button runat="server" Text="Edit" CssClass="btn-secondary btn-sm"
                                                CommandName="EditLesson" CommandArgument='<%# Eval("lessonid") %>'
                                                OnCommand="Lesson_Command" />
                                            <asp:Button runat="server" Text="Delete" CssClass="btn-danger btn-sm"
                                                CommandName="DeleteLesson" CommandArgument='<%# Eval("lessonid") %>'
                                                OnCommand="Lesson_Command"
                                                OnClientClick="this.disabled=true; return confirm('Delete this lesson?');" />
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate></table></FooterTemplate>
                        </asp:Repeater>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <!-- ═══════════════════════════════════════
             PANEL 3 — EDIT MODULE (add / edit)
             ═══════════════════════════════════════ -->
        <asp:Panel ID="pnlEditModule" runat="server" Visible="false">
            <div class="page-banner banner-module">
                <div class="banner-label">Course Editor</div>
                <div class="banner-title"><asp:Label ID="lblModModeTitle" runat="server" Text="Add Module" /></div>
                <div class="banner-sub"><asp:Label ID="lblModCourseTitle" runat="server" /></div>
            </div>

            <asp:Button ID="btnCancelModule" runat="server" Text="← Cancel"
                CssClass="btn-back" OnClick="btnCancelModule_Click" CausesValidation="false" />

            <div class="form-card">
                <div class="form-card-title"><span class="title-dot dot-orange"></span> Module Details</div>
                <div class="form-card-sub">Enter the module name and an optional description and order.</div>

                <asp:ValidationSummary ID="vsModule" runat="server"
                    CssClass="validation-summary" HeaderText="Please fix:"
                    ValidationGroup="moduleForm" />

                <div class="form-group">
                    <label class="form-label">Module Name *</label>
                    <asp:TextBox ID="txtModName" runat="server" CssClass="form-input" MaxLength="100" />
                    <asp:RequiredFieldValidator ControlToValidate="txtModName" runat="server"
                        ErrorMessage="Module name is required." CssClass="validation-error"
                        ValidationGroup="moduleForm" Display="Dynamic" />
                    <asp:RegularExpressionValidator ControlToValidate="txtModName" runat="server"
                        ValidationExpression="^[a-zA-Z0-9 .\-]+$"
                        ErrorMessage="Only letters, numbers, spaces, dots and dashes allowed."
                        CssClass="validation-error" ValidationGroup="moduleForm" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label class="form-label">Module Description</label>
                    <asp:TextBox ID="txtModDesc" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="3" MaxLength="1000" />
                </div>
                <div class="form-group">
                    <label class="form-label">Order Number <span style="color:var(--text-muted);font-weight:400;text-transform:none;">(1–100)</span></label>
                    <asp:TextBox ID="txtModOrder" runat="server" CssClass="form-input" MaxLength="3" />
                    <asp:RegularExpressionValidator ControlToValidate="txtModOrder" runat="server"
                        ValidationExpression="^\d*$" ErrorMessage="Order must be a number."
                        CssClass="validation-error" ValidationGroup="moduleForm" Display="Dynamic" />
                    <asp:RangeValidator ControlToValidate="txtModOrder" runat="server"
                        MinimumValue="1" MaximumValue="100" Type="Integer"
                        ErrorMessage="Order must be between 1 and 100."
                        CssClass="validation-error" ValidationGroup="moduleForm" Display="Dynamic" />
                </div>

                <div class="btn-row">
                    <asp:Button ID="btnSaveModule" runat="server" Text="Save Module"
                        CssClass="btn-primary" ValidationGroup="moduleForm" OnClick="btnSaveModule_Click" />
                    <asp:Label ID="lblModMsg" runat="server" CssClass="alert" Visible="false" />
                </div>
            </div>
        </asp:Panel>

        <!-- ═══════════════════════════════════════
             PANEL 4 — EDIT LESSON (add / edit) with lessonpoints
             ═══════════════════════════════════════ -->
        <asp:Panel ID="pnlEditLesson" runat="server" Visible="false">
            <div class="page-banner banner-lesson">
                <div class="banner-label">Course Editor</div>
                <div class="banner-title"><asp:Label ID="lblLsnModeTitle" runat="server" Text="Add Lesson" /></div>
                <div class="banner-sub"><asp:Label ID="lblLsnModuleName" runat="server" /></div>
            </div>

            <asp:Button ID="btnCancelLesson" runat="server" Text="← Cancel"
                CssClass="btn-back" OnClick="btnCancelLesson_Click" CausesValidation="false" />

            <div class="form-card">
                <div class="form-card-title"><span class="title-dot dot-purple"></span> Lesson Details</div>
                <div class="form-card-sub">Fill in the lesson content. Points are awarded to students on completion.</div>

                <asp:ValidationSummary ID="vsLesson" runat="server"
                    CssClass="validation-summary" HeaderText="Please fix:"
                    ValidationGroup="lessonForm" />

                <div class="form-group">
                    <label class="form-label">Lesson Title *</label>
                    <asp:TextBox ID="txtLsnTitle" runat="server" CssClass="form-input" MaxLength="100" />
                    <asp:RequiredFieldValidator ControlToValidate="txtLsnTitle" runat="server"
                        ErrorMessage="Lesson title is required." CssClass="validation-error"
                        ValidationGroup="lessonForm" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label class="form-label">Lesson Description</label>
                    <asp:TextBox ID="txtLsnDesc" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="3" MaxLength="1000" />
                </div>
                <div class="form-row">
                    <div class="form-group" style="margin-bottom:0;">
                        <label class="form-label">Duration (minutes) *</label>
                        <asp:TextBox ID="txtLsnDuration" runat="server" CssClass="form-input" MaxLength="4" placeholder="e.g. 30" />
                        <asp:RequiredFieldValidator ControlToValidate="txtLsnDuration" runat="server"
                            ErrorMessage="Duration is required." CssClass="validation-error"
                            ValidationGroup="lessonForm" Display="Dynamic" />
                        <asp:RegularExpressionValidator ControlToValidate="txtLsnDuration" runat="server"
                            ValidationExpression="^\d+$" ErrorMessage="Duration must be a number."
                            CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                        <asp:RangeValidator ControlToValidate="txtLsnDuration" runat="server"
                            MinimumValue="1" MaximumValue="600" Type="Integer"
                            ErrorMessage="Duration must be 1–600 minutes."
                            CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                    </div>
                    <div class="form-group" style="margin-bottom:0;">
                        <label class="form-label">Lesson Points
                            <span style="color:var(--text-muted);font-weight:400;text-transform:none;">(on completion)</span>
                        </label>
                        <asp:TextBox ID="txtLsnPoints" runat="server" CssClass="form-input" MaxLength="5" placeholder="e.g. 10" />
                        <asp:RegularExpressionValidator ControlToValidate="txtLsnPoints" runat="server"
                            ValidationExpression="^\d*$" ErrorMessage="Points must be a whole number."
                            CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                        <asp:RangeValidator ControlToValidate="txtLsnPoints" runat="server"
                            MinimumValue="0" MaximumValue="10000" Type="Integer"
                            ErrorMessage="Points must be 0–10000."
                            CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                        <span class="form-hint">Leave blank for no points.</span>
                    </div>
                </div>
                <div class="form-group" style="margin-top:18px;">
                    <label class="form-label">Video URL</label>
                    <asp:TextBox ID="txtLsnVideoUrl" runat="server" CssClass="form-input" MaxLength="500" placeholder="https://..." />
                    <asp:RegularExpressionValidator ControlToValidate="txtLsnVideoUrl" runat="server"
                        ValidationExpression="^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-\.~:\/?#\[\]@!\$&amp;'\(\)\*\+,;=]*)?$"
                        ErrorMessage="Invalid video URL format."
                        CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label class="form-label">Upload File (PDF, DOC, DOCX, PPT, PPTX — max 5MB)</label>
                    <asp:FileUpload ID="fuLsnFile" runat="server" CssClass="form-input" />
                    <asp:RegularExpressionValidator ControlToValidate="fuLsnFile" runat="server"
                        ValidationExpression="^.*\.(pdf|doc|docx|ppt|pptx)$"
                        ErrorMessage="Only PDF, DOC, DOCX, PPT, PPTX files allowed."
                        CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                </div>

                <div class="btn-row">
                    <asp:Button ID="btnSaveLesson" runat="server"
                        CssClass="btn-primary" ValidationGroup="lessonForm" OnClick="btnSaveLesson_Click" />
                    <asp:Label ID="lblLsnMsg" runat="server" CssClass="alert" Visible="false" />
                </div>
            </div>
        </asp:Panel>

        <!-- ═══════════════════════════════════════
             PANEL 5 — VIEW STUDENTS
             ═══════════════════════════════════════ -->
        <asp:Panel ID="pnlViewStudents" runat="server" Visible="false">
            <div class="page-banner banner-students">
                <div class="banner-label">Course Management</div>
                <div class="banner-title">Enrolled Students</div>
                <div class="banner-sub"><asp:Label ID="lblStudentsCourseTitle" runat="server" /></div>
            </div>

            <div class="btn-row" style="margin-bottom:20px;">
                <asp:Button ID="btnBackFromStudents" runat="server" Text="← Back to Courses"
                    CssClass="btn-back" OnClick="btnBackFromStudents_Click" CausesValidation="false"
                    style="margin-bottom:0;" />
                <asp:Button ID="btnExport" runat="server" Text="Export to Excel"
                    CssClass="btn-success" OnClick="btnExport_Click" CausesValidation="false" />
            </div>

            <div class="section-wide">
                <div class="section-header">
                    <div class="section-title"><span class="title-dot dot-cyan"></span> Student List</div>
                </div>
                <div class="table-scroll">
                <asp:GridView ID="gvStudents" runat="server"
                    AutoGenerateColumns="False" Width="100%"
                    BorderStyle="None" GridLines="None"
                    DataKeyNames="userid"
                    OnRowCommand="gvStudents_RowCommand"
                    EmptyDataText="No enrolled students found.">
                    <Columns>
                        <asp:BoundField DataField="userid"     HeaderText="ID" />
                        <asp:BoundField DataField="uname"      HeaderText="Username" />
                        <asp:BoundField DataField="fname"      HeaderText="First Name" />
                        <asp:BoundField DataField="lname"      HeaderText="Last Name" />
                        <asp:BoundField DataField="email"      HeaderText="Email" />
                        <asp:BoundField DataField="age"        HeaderText="Age" />
                        <asp:BoundField DataField="gender"     HeaderText="Gender" />
                        <asp:BoundField DataField="EnrolledOn" HeaderText="Enrolled On"
                            DataFormatString="{0:dd MMM yyyy}" />
                        <asp:TemplateField HeaderText="Payment">
                            <ItemTemplate>
                                <span class='badge <%# Eval("PaymentStatus").ToString() == "Paid" ? "badge-published" : (Eval("PaymentStatus").ToString() == "Free" ? "badge-free" : "badge-draft") %>'>
                                    <%# Eval("PaymentStatus") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="AmountPaid"     HeaderText="Amount (RM)"
                            DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="Overdue"         HeaderText="Overdue (RM)"
                            DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="InvoiceDate"     HeaderText="Invoice Date"
                            DataFormatString="{0:dd MMM yyyy}" NullDisplayText="—" />
                        <asp:BoundField DataField="PaymentDeadline" HeaderText="Deadline"
                            DataFormatString="{0:dd MMM yyyy}" NullDisplayText="—" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="btn-actions">
                                    <asp:Button ID="btnRemoveStudent" runat="server" Text="Remove"
                                        CssClass="btn-danger btn-sm" CommandName="DeleteStudent"
                                        CommandArgument="<%# Container.DataItemIndex %>"
                                        OnClientClick="return confirm('Remove this student from the course?');" />
                                    <asp:LinkButton ID="btnViewReceipt" runat="server" Text="Receipt"
                                        CssClass="btn-secondary btn-sm" CommandName="ViewReceipt"
                                        CommandArgument="<%# Container.DataItemIndex %>" />
                                    <asp:LinkButton ID="btnViewExamResults" runat="server" Text="📝 Exam Results"
                                        CssClass="btn-secondary btn-sm" CommandName="ViewExamResults"
                                        CommandArgument="<%# Container.DataItemIndex %>" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                </div>
            </div>

            <asp:Label ID="lblStudentsMsg" runat="server" CssClass="alert" Visible="false" />
        </asp:Panel>

        <!-- ═══════════════════════════════════════
             PANEL 6 — REVIEW & PUBLISH
             ═══════════════════════════════════════ -->
        <asp:Panel ID="pnlReviewPublish" runat="server" Visible="false">
            <div class="page-banner banner-review">
                <div class="banner-label">Course Editor</div>
                <div class="banner-title">Review &amp; Publish</div>
                <div class="banner-sub">Review your course content before making it live.</div>
            </div>

            <asp:Button ID="btnBackToEdit" runat="server" Text="← Back to Editor"
                CssClass="btn-back" OnClick="btnBackToEdit_Click" CausesValidation="false" />

            <div class="ready-banner">
                <h3>🚀 Ready to Publish!</h3>
                <p>Everything looks good. Hit Publish to make this course live for students.</p>
            </div>

            <div class="form-card">
                <div class="form-card-title"><span class="title-dot dot-blue"></span> Course Overview</div>
                <div class="review-info-card">
                    <div class="review-info-label">Course Name</div>
                    <div class="review-info-value"><asp:Label ID="lblReviewCourseName" runat="server" /></div>
                    <div class="review-info-label">Description</div>
                    <div class="review-info-value"><asp:Label ID="lblReviewCourseDesc" runat="server" /></div>
                    <div class="review-info-label">Price</div>
                    <div class="review-info-value"><asp:Label ID="lblReviewCoursePrice" runat="server" /></div>
                </div>

                <div class="form-card-title" style="margin-top:20px;">
                    <span class="title-dot dot-green"></span> Modules &amp; Lessons
                </div>

                <asp:Repeater ID="rptReviewModules" runat="server">
                    <ItemTemplate>
                        <div class="module-review-card">
                            <div class="module-review-title">
                                <span class="module-tag">Module</span>
                                <%# Server.HtmlEncode(Eval("modulename").ToString()) %>
                            </div>
                            <asp:Repeater ID="rptReviewLessons" runat="server" DataSource='<%# Eval("Lessons") %>'>
                                <ItemTemplate>
                                    <div class="lesson-item-row">
                                        <span class="lesson-item-name">📖 <%# Server.HtmlEncode(Eval("lessontitle").ToString()) %></span>
                                        <span class="lesson-item-meta">
                                            <span style="color:var(--text-muted);font-size:0.75rem;font-family:'Space Mono',monospace;">
                                                <%# Eval("duration") %> mins
                                            </span>
                                            <%# (Eval("lessonpoints") != DBNull.Value && Convert.ToInt32(Eval("lessonpoints")) > 0)
                                                ? "<span class='points-pill'>⚡ " + Eval("lessonpoints") + " pts</span>" : "" %>
                                        </span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- Course Exam (preserved from editPublish) -->
                <asp:Panel ID="pnlCourseExam" runat="server" Visible="false">
                    <div class="form-card-title" style="margin-top:20px;">
                        <span class="title-dot dot-red"></span> Course Exam
                    </div>
                    <div class="module-review-card">
                        <div class="module-review-title"><asp:Label ID="lblCourseExamTitle" runat="server" /></div>
                        <div class="lesson-item-row">
                            <span>Total Questions:</span>
                            <asp:Label ID="lblCourseExamQuestions" runat="server" />
                        </div>
                    </div>
                </asp:Panel>

                <div class="btn-row" style="margin-top:24px;">
                    <asp:Button ID="btnPublish" runat="server" Text="🚀 Publish Course"
                        CssClass="btn-success" OnClick="btnPublish_Click" CausesValidation="false"
                        OnClientClick="return confirm('Publish this course? It will be visible to students.');" />
                    <asp:Label ID="lblPublishMsg" runat="server" CssClass="alert" Visible="false" />
                </div>
            </div>
        </asp:Panel>

    </div>

    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
    <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

</form>
</body>
</html>
