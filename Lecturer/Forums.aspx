<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forums.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.Forums" ValidateRequest="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
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
            --accent-orange: #f59e0b;
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
            --accent-red: #ef4444;
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

        /* HEADER */
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
            display: flex; align-items: center; justify-content: center;
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

        /* NAV */
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

        /* CONTAINER */
        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }

        /* BANNERS */
        .page-banner {
            border-radius: var(--radius); padding: 28px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            animation: slideDown 0.5s ease both;
        }
        .banner-forums { background: linear-gradient(135deg,#059669,#10b981 55%,#34d399); box-shadow:0 8px 30px rgba(16,185,129,0.28); }
        .banner-create  { background: linear-gradient(135deg,#f59e0b,#fbbf24 55%,#fcd34d); box-shadow:0 8px 30px rgba(245,158,11,0.28); }
        .banner-edit-f  { background: linear-gradient(135deg,#d97706,#f59e0b 55%,#fbbf24); box-shadow:0 8px 30px rgba(217,119,6,0.28); }
        .banner-view    { background: linear-gradient(135deg,#2563eb,#3b82f6 55%,#60a5fa); box-shadow:0 8px 30px rgba(37,99,235,0.25); }
        .banner-detail  { background: linear-gradient(135deg,#7c3aed,#8b5cf6 55%,#a78bfa); box-shadow:0 8px 30px rgba(139,92,246,0.25); }
        .banner-edit-q  { background: linear-gradient(135deg,#0891b2,#06b6d4 55%,#67e8f9); box-shadow:0 8px 30px rgba(8,145,178,0.28); }
        .page-banner::before {
            content:''; position:absolute; top:-40%; right:-10%; width:280px; height:280px;
            background:radial-gradient(circle,rgba(255,255,255,0.12),transparent 65%);
            border-radius:50%; pointer-events:none;
        }
        .banner-label { font-size:0.72rem; font-weight:700; letter-spacing:2px; text-transform:uppercase; color:rgba(255,255,255,0.75); margin-bottom:6px; font-family:'Space Mono',monospace; }
        .banner-title { font-size:1.5rem; font-weight:700; color:white; margin-bottom:4px; }
        .banner-sub   { color:rgba(255,255,255,0.75); font-size:0.85rem; }

        /* BACK BUTTON */
        .btn-back {
            display:inline-flex; align-items:center; gap:8px;
            background:var(--surface); border:1px solid var(--border);
            color:var(--text-secondary); padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.83rem; font-weight:600;
            cursor:pointer; transition:all 0.2s; margin-bottom:20px;
        }
        .btn-back:hover { border-color:var(--primary); color:var(--primary); }

        /* FILTER BAR */
        .filter-bar {
            display:flex; gap:10px; flex-wrap:wrap;
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:16px 20px;
            margin-bottom:20px; box-shadow:var(--shadow-sm);
        }
        .filter-input {
            flex:1; min-width:160px; background:var(--border-light);
            border:1px solid var(--border); border-radius:var(--radius-sm);
            padding:9px 14px; font-family:'DM Sans',sans-serif; font-size:0.875rem;
            outline:none; transition:border-color 0.2s; color:var(--text);
        }
        .filter-input:focus { border-color:var(--primary); background:white; }

        /* SECTION CARD */
        .section {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); box-shadow:var(--shadow-sm);
            overflow:hidden; margin-bottom:20px;
            animation:slideUp 0.5s ease both;
        }
        .section-header {
            padding:18px 24px; border-bottom:1px solid var(--border);
            display:flex; align-items:center; justify-content:space-between;
        }
        .section-title { font-size:0.95rem; font-weight:700; color:var(--text); display:flex; align-items:center; gap:10px; }
        .title-dot { width:8px; height:8px; border-radius:50%; display:inline-block; flex-shrink:0; }
        .dot-green  { background:var(--accent-green); }
        .dot-blue   { background:var(--primary); }
        .dot-purple { background:var(--accent-purple); }
        .dot-orange { background:var(--accent-orange); }
        .dot-cyan   { background:#0891b2; }

        /* TABLE */
        .section table { width:100%; border-collapse:collapse; }
        .section table th {
            background:var(--border-light); padding:11px 24px; text-align:left;
            font-size:0.72rem; font-weight:700; letter-spacing:1px; text-transform:uppercase;
            color:var(--text-muted); border-bottom:1px solid var(--border);
        }
        .section table td {
            padding:14px 24px; font-size:0.875rem; color:var(--text);
            border-bottom:1px solid var(--border-light); vertical-align:middle;
        }
        .section table tr:last-child td { border-bottom:none; }
        .section table tr:hover td { background:var(--surface-hover); }

        /* BUTTONS */
        .btn-primary {
            background:var(--primary); color:white; border:none;
            padding:9px 20px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.83rem; font-weight:600;
            cursor:pointer; transition:background 0.2s;
        }
        .btn-primary:hover { background:#1d4ed8; }
        .btn-secondary {
            background:var(--border-light); color:var(--text-secondary);
            border:1px solid var(--border); padding:9px 20px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.83rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-secondary:hover { border-color:var(--primary); color:var(--primary); }
        .btn-sm { padding:5px 13px; font-size:0.77rem; border-radius:7px; }
        .btn-view {
            background:var(--primary-bg); color:var(--primary);
            border:1px solid var(--primary-border); padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.82rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-view:hover { background:var(--primary); color:white; }
        .btn-danger {
            background:rgba(239,68,68,0.08); color:var(--accent-red);
            border:1px solid rgba(239,68,68,0.2); padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.82rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-danger:hover { background:var(--accent-red); color:white; }
        .btn-delete {
            background:rgba(239,68,68,0.08); color:var(--accent-red);
            border:1px solid rgba(239,68,68,0.2); padding:5px 14px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.78rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-delete:hover { background:var(--accent-red); color:white; }
        .btn-edit-sm {
            background:rgba(37,99,235,0.08); color:var(--primary);
            border:1px solid var(--primary-border); padding:5px 14px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.78rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-edit-sm:hover { background:var(--primary); color:white; }
        .btn-actions { display:flex; gap:8px; flex-wrap:wrap; }

        /* FORM CARD */
        .form-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:32px;
            box-shadow:var(--shadow-sm); margin-bottom:20px;
            animation:slideUp 0.5s ease both;
        }
        .form-card-title {
            font-size:0.95rem; font-weight:700; margin-bottom:22px;
            display:flex; align-items:center; gap:10px; color:var(--text);
        }
        .form-group { margin-bottom:18px; }
        .form-label {
            display:block; font-size:0.75rem; font-weight:600;
            color:var(--text-secondary); text-transform:uppercase;
            letter-spacing:0.5px; margin-bottom:7px;
        }
        .form-input {
            width:100%; background:var(--border-light); border:1px solid var(--border);
            border-radius:var(--radius-sm); padding:10px 14px;
            font-family:'DM Sans',sans-serif; font-size:0.875rem;
            color:var(--text); outline:none; transition:border-color 0.2s; box-sizing:border-box;
        }
        .form-input:focus { border-color:var(--primary); background:white; }
        textarea.form-input { resize:vertical; min-height:100px; line-height:1.5; }
        .form-hint { font-size:0.72rem; color:var(--text-muted); margin-top:5px; }
        .validation-error { font-size:0.75rem; color:#dc2626; margin-top:4px; display:block; }
        .validation-summary {
            background:rgba(239,68,68,0.06); border:1px solid rgba(239,68,68,0.2);
            color:#dc2626; border-radius:var(--radius-sm);
            padding:14px 18px; font-size:0.83rem; margin-bottom:20px;
        }
        .form-btn-row { display:flex; gap:10px; margin-top:24px; flex-wrap:wrap; }

        /* FORUM HEADER INFO */
        .forum-info-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:22px 28px;
            margin-bottom:16px; box-shadow:var(--shadow-sm);
        }
        .forum-info-card h3 { font-size:1rem; font-weight:700; margin-bottom:8px; }
        .forum-info-row { display:flex; align-items:center; justify-content:space-between; flex-wrap:wrap; gap:12px; }
        .tags-row { display:flex; gap:8px; flex-wrap:wrap; align-items:center; }
        .tag-label { font-size:0.72rem; color:var(--text-muted); font-weight:600; }
        .tag-pill {
            background:var(--primary-bg); color:var(--primary);
            border:1px solid var(--primary-border);
            padding:3px 12px; border-radius:20px; font-size:0.72rem; font-weight:600;
        }
        .forum-actions-row { display:flex; justify-content:flex-end; gap:10px; margin-bottom:20px; flex-wrap:wrap; }

        /* INLINE FORM PANEL */
        .inline-form {
            background:var(--border-light); border:1px solid var(--border);
            border-radius:var(--radius); padding:22px 24px; margin-bottom:20px;
        }
        .inline-form-title {
            font-size:0.9rem; font-weight:700; margin-bottom:16px;
            color:var(--text); display:flex; align-items:center; gap:8px;
        }

        /* QUESTION CARDS */
        .question-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:22px 24px;
            margin-bottom:14px; box-shadow:var(--shadow-sm);
            transition:transform 0.2s,box-shadow 0.2s;
        }
        .question-card:hover { transform:translateY(-1px); box-shadow:var(--shadow-md); }
        .card-meta { display:flex; align-items:center; gap:12px; margin-bottom:12px; }
        .card-user-avatar { width:36px; height:36px; border-radius:50%; object-fit:cover; border:2px solid var(--border); flex-shrink:0; }
        .card-user-name { font-size:0.85rem; font-weight:600; color:var(--text); }
        .card-date { font-size:0.73rem; color:var(--text-muted); font-family:'Space Mono',monospace; }
        .card-question-title { font-size:1rem; font-weight:700; margin-bottom:8px; color:var(--text); }
        .card-preview { font-size:0.875rem; color:var(--text-secondary); line-height:1.6; margin-bottom:12px; }
        .card-footer-row {
            display:flex; align-items:center; justify-content:space-between;
            flex-wrap:wrap; gap:10px; padding-top:12px;
            border-top:1px solid var(--border-light);
        }
        .vote-group { display:flex; gap:8px; }
        .vote-btn {
            display:inline-flex; align-items:center; gap:6px;
            background:var(--border-light); border:1px solid var(--border);
            color:var(--text-secondary); padding:5px 14px; border-radius:20px;
            font-family:'DM Sans',sans-serif; font-size:0.8rem; font-weight:600;
            cursor:pointer; transition:all 0.2s; text-decoration:none;
        }
        .vote-btn.like-btn:hover    { background:rgba(16,185,129,0.1); color:var(--accent-green); border-color:rgba(16,185,129,0.3); }
        .vote-btn.dislike-btn:hover { background:rgba(239,68,68,0.08); color:var(--accent-red);   border-color:rgba(239,68,68,0.25); }
        .card-action-group { display:flex; gap:8px; flex-wrap:wrap; align-items:center; }
        .btn-comment {
            background:var(--primary-bg); color:var(--primary);
            border:1px solid var(--primary-border); padding:6px 16px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.8rem; font-weight:600;
            cursor:pointer; transition:all 0.2s; text-decoration:none;
        }
        .btn-comment:hover { background:var(--primary); color:white; }

        /* DETAIL CARD */
        .detail-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:28px;
            margin-bottom:24px; box-shadow:var(--shadow-sm);
        }
        .detail-title  { font-size:1.3rem; font-weight:700; margin:14px 0 12px; }
        .detail-content { font-size:0.9rem; line-height:1.7; color:var(--text-secondary); }
        .question-preview-box {
            background:var(--border-light); border:1px solid var(--border);
            border-radius:var(--radius-sm); padding:16px 20px; margin-bottom:20px;
        }
        .question-title-preview { display:block; font-weight:700; font-size:0.95rem; margin-bottom:6px; }
        .question-content-preview { display:block; font-size:0.85rem; color:var(--text-secondary); line-height:1.5; }

        /* ANSWER CARDS */
        .answer-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:20px 24px;
            margin-bottom:12px; box-shadow:var(--shadow-sm);
        }
        .answers-heading {
            font-size:0.95rem; font-weight:700; margin-bottom:14px;
            display:flex; align-items:center; gap:8px; color:var(--text);
        }

        /* ALERTS */
        .alert { padding:12px 18px; border-radius:var(--radius-sm); font-size:0.83rem; font-weight:500; margin:10px 0; display:block; }
        .alert-success { background:rgba(16,185,129,0.08); border:1px solid rgba(16,185,129,0.2); color:#059669; }
        .alert-error   { background:rgba(239,68,68,0.07); border:1px solid rgba(239,68,68,0.2); color:#dc2626; }
        .empty-message { display:block; text-align:center; padding:40px; color:var(--text-muted); font-size:0.88rem; }

        input[type="file"].form-input { padding:8px 14px; cursor:pointer; }

        /* ANIMATIONS */
        @keyframes slideDown { from{opacity:0;transform:translateY(-12px);}to{opacity:1;transform:translateY(0);} }
        @keyframes slideUp   { from{opacity:0;transform:translateY(14px);}to{opacity:1;transform:translateY(0);} }

        @media(max-width:900px){
            .container{padding:20px;}
            .header,.nav{padding:0 20px;}
            .filter-bar{flex-direction:column;}
        }
    </style>
</head>
<body>
<form id="form1" runat="server" enctype="multipart/form-data">

    <!-- HEADER -->
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

    <!-- NAV -->
    <div class="nav">
        <a href="LecturerDashboard.aspx"><span>📊</span> Dashboard</a>
        <a href="CreateCourse.aspx"><span>➕</span> Create Course</a>
        <a href="ViewCourses.aspx"><span>📚</span> View Courses</a>
        <a href="EditProfile.aspx"><span>👤</span> Edit Profile</a>
        <a href="Forums.aspx" class="active"><span>💬</span> Forums</a>
        <a href="Message.aspx">
            <span>✉️</span> Messaging
            <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                <span class="nav-badge"><%= Session["unreadCount"] %></span>
            <% } %>
        </a>
    </div>

    <div class="container">

        <!-- list forums -->
        <asp:Panel ID="pnlForumsList" runat="server">
            <div class="page-banner banner-forums">
                <div class="banner-label">Lecturer Portal</div>
                <div class="banner-title">Manage Course Forums</div>
                <div class="banner-sub">Create, view and delete discussion forums for your courses.</div>
            </div>

            <div class="filter-bar">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-input" placeholder="Search course name..." />
                <asp:DropDownList ID="ddlForumStatus" runat="server" CssClass="filter-input">
                    <asp:ListItem Value="">All Forums</asp:ListItem>
                    <asp:ListItem Value="1">Has Forum</asp:ListItem>
                    <asp:ListItem Value="0">No Forum</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn-primary" OnClick="btnFilter_Click" />
                <asp:Button ID="btnReset"  runat="server" Text="Reset"        CssClass="btn-secondary" OnClick="btnReset_Click" />
            </div>

            <div class="section">
                <div class="section-header">
                    <div class="section-title">
                        <span class="title-dot dot-green"></span> Your Courses
                    </div>
                </div>
                <asp:GridView ID="gvCourses" runat="server"
                    AutoGenerateColumns="False" Width="100%"
                    BorderStyle="None" GridLines="None"
                    OnRowCommand="gvCourses_RowCommand"
                    EmptyDataText="No courses available."
                    DataKeyNames="courseid">
                    <Columns>
                        <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="btn-actions">
                                    <asp:Button ID="btnCreate" runat="server" Text="Create Forum"
                                        CommandName="CreateForum" CommandArgument='<%# Eval("courseid") %>'
                                        CssClass="btn-primary"
                                        Visible='<%# !Convert.ToBoolean(Eval("HasForum")) %>' />
                                    <asp:Button ID="btnView" runat="server" Text="View Forum"
                                        CommandName="ViewForum" CommandArgument='<%# Eval("courseid") %>'
                                        CssClass="btn-view"
                                        Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete Forum"
                                        CommandName="DeleteForum" CommandArgument='<%# Eval("courseid") %>'
                                        CssClass="btn-danger"
                                        Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>'
                                        OnClientClick="return confirm('Delete this forum and all its posts?');" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <asp:Label ID="lblListMessage" runat="server" CssClass="alert" Visible="false" />
        </asp:Panel>

        <!-- create and edit forum panel -->
        <asp:Panel ID="pnlCreateForum" runat="server" Visible="false">
            <div class="page-banner banner-create">
                <div class="banner-label">Forums</div>
                <div class="banner-title">
                    <asp:Label ID="lblCreateForumTitle" runat="server" Text="Create a New Forum" />
                </div>
                <div class="banner-sub">Fill in the details below to set up the forum for your course.</div>
            </div>

            <asp:Button ID="btnBackFromCreate" runat="server" Text="← Back to Forums"
                CssClass="btn-back" OnClick="btnBackFromCreate_Click" CausesValidation="false" />

            <div class="form-card">
                <div class="form-card-title">
                    <span class="title-dot dot-orange"></span> Forum Details
                </div>

                <asp:ValidationSummary ID="vsCreate" runat="server"
                    CssClass="validation-summary"
                    HeaderText="Please fix the following errors:"
                    ValidationGroup="forumForm" />

                <div class="form-group">
                    <label class="form-label">Forum Title *</label>
                    <asp:TextBox ID="txtForumTitle" runat="server" CssClass="form-input" MaxLength="100" />
                    <asp:RequiredFieldValidator ControlToValidate="txtForumTitle" runat="server"
                        ErrorMessage="Forum title is required." CssClass="validation-error"
                        ValidationGroup="forumForm" Display="Dynamic" />
                    <asp:RegularExpressionValidator ControlToValidate="txtForumTitle" runat="server"
                        ValidationExpression="^[a-zA-Z0-9 _\-]+$"
                        ErrorMessage="Only letters, numbers, spaces, dash and underscore allowed."
                        CssClass="validation-error" ValidationGroup="forumForm" Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label class="form-label">Description *</label>
                    <asp:TextBox ID="txtForumDescription" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="4" MaxLength="1000" />
                    <asp:RequiredFieldValidator ControlToValidate="txtForumDescription" runat="server"
                        ErrorMessage="Description is required." CssClass="validation-error"
                        ValidationGroup="forumForm" Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label class="form-label">Tags (comma-separated)</label>
                    <asp:TextBox ID="txtForumTags" runat="server" CssClass="form-input" MaxLength="200"
                        placeholder="e.g. python, AI, loops" />
                    <asp:RegularExpressionValidator ControlToValidate="txtForumTags" runat="server"
                        ValidationExpression="^[a-zA-Z0-9\s,]*$"
                        ErrorMessage="Tags can only contain letters, numbers and commas."
                        CssClass="validation-error" ValidationGroup="forumForm" Display="Dynamic" />
                    <span class="form-hint">Leave blank to allow all tags.</span>
                </div>

                <div class="form-btn-row">
                    <asp:Button ID="btnCreateForum" runat="server" Text="Create Forum"
                        CssClass="btn-primary" ValidationGroup="forumForm" OnClick="btnCreateForum_Click" />
                    <asp:Button ID="btnCancelCreate" runat="server" Text="Cancel"
                        CssClass="btn-secondary" OnClick="btnBackFromCreate_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblCreateMsg" runat="server" CssClass="alert" Visible="false" />
            </div>
        </asp:Panel>

        <!-- view forums panel -->
        <asp:Panel ID="pnlViewForum" runat="server" Visible="false">
            <div class="page-banner banner-view">
                <div class="banner-label">Course Forum</div>
                <div class="banner-title"><asp:Label ID="lblForumTitle" runat="server" /></div>
                <div class="banner-sub"><asp:Label ID="lblDescription" runat="server" /></div>
            </div>

            <asp:Button ID="btnBackToList" runat="server" Text="← Back to Forums"
                CssClass="btn-back" OnClick="btnBackToList_Click" CausesValidation="false" />

            <!-- Forum meta: tags + Edit Forum button -->
            <div class="forum-info-card">
                <div class="forum-info-row">
                    <div class="tags-row">
                        <span class="tag-label">Tags:</span>
                        <asp:Label ID="lblTags" runat="server" />
                    </div>
                    <asp:Button ID="btnEditForum" runat="server" Text="✏️ Edit Forum"
                        CssClass="btn-secondary btn-sm" OnClick="btnEditForum_Click"
                        CausesValidation="false" Visible="false" />
                </div>
                <asp:Label ID="lblForumMsg" runat="server" CssClass="alert" Visible="false" />
            </div>

            <div class="forum-actions-row">
                <asp:Button ID="btnAskQuestion" runat="server" Text="+ Ask a Question"
                    CssClass="btn-primary" OnClick="btnAskQuestion_Click" CausesValidation="false" />
            </div>

            <!-- Inline: Ask Question form -->
            <asp:Panel ID="pnlAskQuestion" runat="server" Visible="false">
                <div class="inline-form">
                    <div class="inline-form-title">✏️ Post a New Question</div>

                    <asp:ValidationSummary ID="vsQuestion" runat="server"
                        CssClass="validation-summary" HeaderText="Please fix:"
                        ValidationGroup="questionForm" />

                    <div class="form-group">
                        <label class="form-label">Title *</label>
                        <asp:TextBox ID="txtQuestionTitle" runat="server" CssClass="form-input" MaxLength="150" />
                        <asp:RequiredFieldValidator ControlToValidate="txtQuestionTitle" runat="server"
                            ErrorMessage="Title is required." CssClass="validation-error"
                            ValidationGroup="questionForm" Display="Dynamic" />
                        <asp:RegularExpressionValidator ControlToValidate="txtQuestionTitle" runat="server"
                            ValidationExpression="^[a-zA-Z0-9\s\-\?\!\.,]{3,150}$"
                            ErrorMessage="Invalid title format (3–150 chars)."
                            CssClass="validation-error" ValidationGroup="questionForm" Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Content *</label>
                        <asp:TextBox ID="txtQuestionContent" runat="server" CssClass="form-input"
                            TextMode="MultiLine" Rows="5" MaxLength="2000" />
                        <asp:RequiredFieldValidator ControlToValidate="txtQuestionContent" runat="server"
                            ErrorMessage="Content is required." CssClass="validation-error"
                            ValidationGroup="questionForm" Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Tags (comma-separated)</label>
                        <asp:TextBox ID="txtQuestionTags" runat="server" CssClass="form-input" MaxLength="200"
                            placeholder="e.g. python, loops, debugging" />
                        <asp:RegularExpressionValidator ControlToValidate="txtQuestionTags" runat="server"
                            ValidationExpression="^[a-zA-Z0-9,\s\-]*$"
                            ErrorMessage="Invalid tags format." CssClass="validation-error"
                            ValidationGroup="questionForm" Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Upload Document (PDF, DOCX, ZIP)</label>
                        <asp:FileUpload ID="fileUploadQFile" runat="server" CssClass="form-input" />
                        <asp:RegularExpressionValidator ControlToValidate="fileUploadQFile" runat="server"
                            ValidationExpression="^.*\.(pdf|docx|zip)$"
                            ErrorMessage="Only PDF, DOCX or ZIP files allowed."
                            CssClass="validation-error" ValidationGroup="questionForm" Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Upload Image (JPG, PNG)</label>
                        <asp:FileUpload ID="fileUploadQImage" runat="server" CssClass="form-input" />
                        <asp:RegularExpressionValidator ControlToValidate="fileUploadQImage" runat="server"
                            ValidationExpression="^.*\.(jpg|jpeg|png)$"
                            ErrorMessage="Only JPG or PNG images allowed."
                            CssClass="validation-error" ValidationGroup="questionForm" Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Video URL</label>
                        <asp:TextBox ID="txtQuestionVideoUrl" runat="server" CssClass="form-input" MaxLength="300"
                            placeholder="https://..." />
                        <asp:RegularExpressionValidator ControlToValidate="txtQuestionVideoUrl" runat="server"
                            ValidationExpression="^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-\.~:\/?#\[\]@!\$&amp;'\(\)\*\+,;=]*)?$"
                            ErrorMessage="Invalid video URL." CssClass="validation-error"
                            ValidationGroup="questionForm" Display="Dynamic" />
                    </div>
                    <div class="form-btn-row">
                        <asp:Button ID="btnSubmitQuestion" runat="server" Text="Post Question"
                            CssClass="btn-primary" ValidationGroup="questionForm" OnClick="btnSubmitQuestion_Click" />
                        <asp:Button ID="btnCancelQuestion" runat="server" Text="Cancel"
                            CssClass="btn-secondary" OnClick="btnCancelQuestion_Click" CausesValidation="false" />
                    </div>
                    <asp:Label ID="lblQuestionFormMsg" runat="server" CssClass="alert" Visible="false" />
                </div>
            </asp:Panel>

            <!-- Questions repeater -->
            <asp:Repeater ID="rptQuestions" runat="server" OnItemCommand="rptQuestions_ItemCommand">
                <ItemTemplate>
                    <div class="question-card">
                        <div class="card-meta">
                            <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' class="card-user-avatar" alt="" />
                            <div>
                                <div class="card-user-name"><%# Server.HtmlEncode(Eval("uname").ToString()) %></div>
                                <div class="card-date"><%# Convert.ToDateTime(Eval("creationtime")).ToString("dd MMM yyyy") %></div>
                            </div>
                        </div>
                        <div class="card-question-title"><%# Server.HtmlEncode(Eval("title").ToString()) %></div>
                        <div class="card-preview">
                            <%# Server.HtmlEncode(Eval("content").ToString().Length > 180
                                ? Eval("content").ToString().Substring(0,180) + "…"
                                : Eval("content").ToString()) %>
                        </div>
                        <div class="tags-row" style="margin-bottom:12px;">
                            <%# FormatTags(Eval("tags")) %>
                        </div>
                        <div class="card-footer-row">
                            <div class="vote-group">
                                <asp:LinkButton runat="server" CommandName="Like"
                                    CommandArgument='<%# Eval("postid") %>'
                                    CssClass="vote-btn like-btn" CausesValidation="false">
                                    👍 <%# Eval("upvotes") %>
                                </asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="Dislike"
                                    CommandArgument='<%# Eval("postid") %>'
                                    CssClass="vote-btn dislike-btn" CausesValidation="false">
                                    👎 <%# Eval("downvotes") %>
                                </asp:LinkButton>
                            </div>
                            <div class="card-action-group">
                                <%-- Edit button — only visible to the question's author --%>
                                <asp:LinkButton runat="server" CommandName="EditQuestion"
                                    CommandArgument='<%# Eval("postid") %>'
                                    CssClass="btn-edit-sm" CausesValidation="false"
                                    Visible='<%# IsOwner(Eval("userid")) %>'>
                                    ✏️ Edit
                                </asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="ViewDetail"
                                    CommandArgument='<%# Eval("postid") %>'
                                    CssClass="btn-comment" CausesValidation="false">
                                    View &amp; Answer →
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblNoQuestions" runat="server" CssClass="empty-message" Visible="false"
                Text="No questions yet. Be the first to ask!" />
            <asp:Label ID="lblViewForumMsg" runat="server" CssClass="alert alert-error" Visible="false" />
        </asp:Panel>

        <!-- Edit questions panel -->
        <asp:Panel ID="pnlEditQuestion" runat="server" Visible="false">
            <div class="page-banner banner-edit-q">
                <div class="banner-label">Forums</div>
                <div class="banner-title">Edit Question</div>
                <div class="banner-sub">Update your question's title, content, tags or video link.</div>
            </div>

            <asp:Button ID="btnCancelEditQuestion" runat="server" Text="← Cancel"
                CssClass="btn-back" OnClick="btnCancelEditQuestion_Click" CausesValidation="false" />

            <div class="form-card">
                <div class="form-card-title">
                    <span class="title-dot dot-cyan"></span> Edit Question
                </div>

                <asp:Label ID="lblEditQuestionMsg" runat="server" CssClass="alert" Visible="false" />

                <div class="form-group">
                    <label class="form-label">Title *</label>
                    <asp:TextBox ID="txtEditQuestionTitle" runat="server" CssClass="form-input" MaxLength="150" />
                </div>
                <div class="form-group">
                    <label class="form-label">Content *</label>
                    <asp:TextBox ID="txtEditQuestionContent" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="7" MaxLength="2000" />
                </div>
                <div class="form-group">
                    <label class="form-label">Tags <span style="font-weight:400;text-transform:none;">(comma-separated)</span></label>
                    <asp:TextBox ID="txtEditQuestionTags" runat="server" CssClass="form-input" MaxLength="200" />
                </div>
                <div class="form-group">
                    <label class="form-label">Video URL</label>
                    <asp:TextBox ID="txtEditQuestionVideoUrl" runat="server" CssClass="form-input"
                        MaxLength="300" placeholder="https://..." />
                </div>

                <div class="form-btn-row">
                    <asp:Button ID="btnSaveEditQuestion" runat="server" Text="Save Changes"
                        CssClass="btn-primary" OnClick="btnSaveEditQuestion_Click" CausesValidation="false" />
                    <asp:Button runat="server" Text="Cancel"
                        CssClass="btn-secondary" OnClick="btnCancelEditQuestion_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>

        <!-- Forum details panel -->
        <asp:Panel ID="pnlForumDetail" runat="server" Visible="false">
            <div class="page-banner banner-detail">
                <div class="banner-label">Forum Discussion</div>
                <div class="banner-title">Question Detail</div>
                <div class="banner-sub">View the full question and all answers below.</div>
            </div>

            <asp:Button ID="btnBackToForum" runat="server" Text="← Back to Forum"
                CssClass="btn-back" OnClick="btnBackToForum_Click" CausesValidation="false" />

            <!-- Question detail card -->
            <div class="detail-card">
                <div class="card-meta">
                    <asp:Image ID="imgQuestionUser" runat="server" CssClass="card-user-avatar" />
                    <div>
                        <div class="card-user-name"><asp:Label ID="lblQuestionUser" runat="server" /></div>
                        <div class="card-date"><asp:Label ID="lblQuestionDate" runat="server" /></div>
                    </div>
                </div>
                <div class="detail-title"><asp:Label ID="lblQuestionTitle" runat="server" /></div>
                <div class="detail-content"><asp:Label ID="lblQuestionContent" runat="server" /></div>
                <div class="tags-row" style="margin-top:14px;">
                    <span class="tag-label">Tags:</span>
                    <asp:Literal ID="litTags" runat="server" />
                </div>
                <div class="card-footer-row" style="margin-top:16px;">
                    <div class="vote-group">
                        <asp:LinkButton ID="btnLikeQuestion" runat="server"
                            CssClass="vote-btn like-btn" OnClick="btnLikeQuestion_Click" CausesValidation="false">
                            👍 <span id="likeCount" runat="server"></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnDislikeQuestion" runat="server"
                            CssClass="vote-btn dislike-btn" OnClick="btnDislikeQuestion_Click" CausesValidation="false">
                            👎 <span id="dislikeCount" runat="server"></span>
                        </asp:LinkButton>
                    </div>
                    <div class="card-action-group">
                        <%-- Edit Question button — only visible to the question's author --%>
                        <asp:Button ID="btnEditDetailQuestion" runat="server" Text="✏️ Edit Question"
                            CssClass="btn-edit-sm" OnClick="btnEditDetailQuestion_Click"
                            CausesValidation="false" Visible="false" />
                        <asp:Button ID="btnAnswer" runat="server" Text="+ Add Answer"
                            CssClass="btn-primary" OnClick="btnAnswer_Click" CausesValidation="false" />
                    </div>
                </div>
            </div>

            <!-- Inline: Add Answer form -->
            <asp:Panel ID="pnlAddAnswer" runat="server" Visible="false">
                <div class="inline-form">
                    <div class="inline-form-title">💬 Write Your Answer</div>

                    <div class="question-preview-box">
                        <asp:Label ID="lblAnswerPreviewTitle"   runat="server" CssClass="question-title-preview" />
                        <asp:Label ID="lblAnswerPreviewContent" runat="server" CssClass="question-content-preview" />
                    </div>

                    <asp:ValidationSummary ID="vsAnswer" runat="server"
                        CssClass="validation-summary" HeaderText="Please fix:"
                        ValidationGroup="answerForm" />

                    <div class="form-group">
                        <label class="form-label">Your Answer *</label>
                        <asp:TextBox ID="txtAnswerContent" runat="server" CssClass="form-input"
                            TextMode="MultiLine" Rows="6" MaxLength="2000" />
                        <asp:RequiredFieldValidator ControlToValidate="txtAnswerContent" runat="server"
                            ErrorMessage="Answer cannot be empty." CssClass="validation-error"
                            ValidationGroup="answerForm" Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Upload Document (PDF, DOCX, ZIP)</label>
                        <asp:FileUpload ID="fileUploadAFile" runat="server" CssClass="form-input" />
                        <asp:RegularExpressionValidator ControlToValidate="fileUploadAFile" runat="server"
                            ValidationExpression="^.*\.(pdf|docx|zip)$"
                            ErrorMessage="Only PDF, DOCX or ZIP files allowed."
                            CssClass="validation-error" ValidationGroup="answerForm" Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Upload Image (JPG, PNG)</label>
                        <asp:FileUpload ID="fileUploadAImage" runat="server" CssClass="form-input" />
                        <asp:RegularExpressionValidator ControlToValidate="fileUploadAImage" runat="server"
                            ValidationExpression="^.*\.(jpg|jpeg|png)$"
                            ErrorMessage="Only JPG or PNG images allowed."
                            CssClass="validation-error" ValidationGroup="answerForm" Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Video URL</label>
                        <asp:TextBox ID="txtAnswerVideoUrl" runat="server" CssClass="form-input" MaxLength="500"
                            placeholder="https://..." />
                        <asp:RegularExpressionValidator ControlToValidate="txtAnswerVideoUrl" runat="server"
                            ValidationExpression="^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-\.~:\/?#\[\]@!\$&amp;'\(\)\*\+,;=]*)?$"
                            ErrorMessage="Invalid video URL." CssClass="validation-error"
                            ValidationGroup="answerForm" Display="Dynamic" />
                    </div>
                    <div class="form-btn-row">
                        <asp:Button ID="btnSubmitAnswer" runat="server" Text="Submit Answer"
                            CssClass="btn-primary" ValidationGroup="answerForm" OnClick="btnSubmitAnswer_Click" />
                        <asp:Button ID="btnCancelAnswer" runat="server" Text="Cancel"
                            CssClass="btn-secondary" OnClick="btnCancelAnswer_Click" CausesValidation="false" />
                    </div>
                    <asp:Label ID="lblAnswerFormMsg" runat="server" CssClass="alert" Visible="false" />
                </div>
            </asp:Panel>

            <!-- Inline: Edit Answer form (shown below the answer being edited) -->
            <asp:Panel ID="pnlEditAnswer" runat="server" Visible="false">
                <div class="inline-form" style="border-color:rgba(37,99,235,0.25);background:rgba(37,99,235,0.03);">
                    <div class="inline-form-title">✏️ Edit Answer</div>

                    <asp:Label ID="lblEditAnswerMsg" runat="server" CssClass="alert" Visible="false" />

                    <div class="form-group">
                        <label class="form-label">Answer *</label>
                        <asp:TextBox ID="txtEditAnswerContent" runat="server" CssClass="form-input"
                            TextMode="MultiLine" Rows="5" MaxLength="2000" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Video URL</label>
                        <asp:TextBox ID="txtEditAnswerVideoUrl" runat="server" CssClass="form-input"
                            MaxLength="300" placeholder="https://..." />
                    </div>
                    <div class="form-btn-row">
                        <asp:Button ID="btnSaveEditAnswer" runat="server" Text="Save Changes"
                            CssClass="btn-primary" OnClick="btnSaveEditAnswer_Click" CausesValidation="false" />
                        <asp:Button ID="btnCancelEditAnswer" runat="server" Text="Cancel"
                            CssClass="btn-secondary" OnClick="btnCancelEditAnswer_Click" CausesValidation="false" />
                    </div>
                </div>
            </asp:Panel>

            <!-- Answers list -->
            <div class="answers-heading">
                <span class="title-dot dot-blue"></span> Answers
            </div>

            <asp:Repeater ID="rptAnswers" runat="server" OnItemCommand="rptAnswers_ItemCommand">
                <ItemTemplate>
                    <div class="answer-card">
                        <div class="card-meta">
                            <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' class="card-user-avatar" alt="" />
                            <div>
                                <div class="card-user-name"><%# Server.HtmlEncode(Eval("uname").ToString()) %></div>
                                <div class="card-date"><%# Convert.ToDateTime(Eval("creationtime")).ToString("dd MMM yyyy") %></div>
                            </div>
                        </div>
                        <div class="card-preview" style="margin-bottom:14px;">
                            <%# Server.HtmlEncode(Eval("content").ToString()) %>
                        </div>
                        <div class="card-footer-row">
                            <div class="vote-group">
                                <asp:LinkButton runat="server" CommandName="LikeAnswer"
                                    CommandArgument='<%# Eval("postid") %>'
                                    CssClass="vote-btn like-btn" CausesValidation="false">
                                    👍 <%# Eval("upvotes") %>
                                </asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="DislikeAnswer"
                                    CommandArgument='<%# Eval("postid") %>'
                                    CssClass="vote-btn dislike-btn" CausesValidation="false">
                                    👎 <%# Eval("downvotes") %>
                                </asp:LinkButton>
                            </div>
                            <div class="card-action-group">
                                <%-- Edit button — only visible to the answer's author --%>
                                <asp:LinkButton runat="server" CommandName="EditAnswer"
                                    CommandArgument='<%# Eval("postid") %>'
                                    CssClass="btn-edit-sm" CausesValidation="false"
                                    Visible='<%# IsOwner(Eval("userid")) %>'>
                                    ✏️ Edit
                                </asp:LinkButton>
                                <asp:Button ID="btnDeleteAnswer" runat="server"
                                    Text="Delete" CssClass="btn-delete"
                                    CommandName="DeletePost"
                                    CommandArgument='<%# Eval("postid") %>'
                                    Visible='<%# IsOwner(Eval("userid")) %>'
                                    OnClientClick="return confirm('Delete this answer?');" />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblNoAnswers" runat="server" CssClass="empty-message"
                Text="No answers yet. Be the first to answer!" Visible="false" />
            <asp:Label ID="lblDetailMessage" runat="server" CssClass="alert" Visible="false" />
        </asp:Panel>

    </div>

    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
    <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

</form>
</body>
</html>
