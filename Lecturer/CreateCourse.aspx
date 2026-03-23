<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateCourse.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.CreateCourse" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Course - LearnSphere</title>
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
        .container { max-width: 860px; margin: 0 auto; padding: 28px 36px; }

        /* ═══ PAGE BANNER ═══ */
        .page-banner {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 55%, #60a5fa 100%);
            border-radius: var(--radius); padding: 28px 36px;
            margin-bottom: 28px; position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(37,99,235,0.25);
            animation: slideDown 0.5s ease both;
        }
        .page-banner::before {
            content: ''; position: absolute; top: -40%; right: -10%;
            width: 280px; height: 280px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%);
            border-radius: 50%; pointer-events: none;
        }
        .banner-label { font-size: 0.72rem; font-weight: 700; letter-spacing: 2px; text-transform: uppercase; color: rgba(255,255,255,0.75); margin-bottom: 6px; font-family: 'Space Mono', monospace; }
        .banner-title { font-size: 1.5rem; font-weight: 700; color: white; margin-bottom: 4px; }
        .banner-sub   { color: rgba(255,255,255,0.75); font-size: 0.85rem; }
        .draft-pill {
            display: inline-block; margin-top: 10px;
            background: rgba(255,255,255,0.2); border: 1px solid rgba(255,255,255,0.3);
            color: white; font-size: 0.7rem; font-weight: 700; letter-spacing: 0.8px;
            text-transform: uppercase; padding: 3px 12px; border-radius: 20px;
            font-family: 'Space Mono', monospace;
        }

        /* ═══ STEP INDICATOR ═══ */
        .step-indicator {
            display: flex; align-items: center; justify-content: center;
            gap: 0; margin-bottom: 28px;
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 20px 24px;
            box-shadow: var(--shadow-sm);
        }
        .step {
            display: flex; flex-direction: column; align-items: center; gap: 8px;
            flex: 1; position: relative;
        }
        .step:not(:last-child)::after {
            content: ''; position: absolute;
            top: 18px; left: calc(50% + 20px);
            width: calc(100% - 40px); height: 2px;
            background: var(--border);
        }
        .step.done:not(:last-child)::after,
        .step.active:not(:last-child)::after { background: var(--primary); }

        .step-circle {
            width: 36px; height: 36px; border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            font-size: 0.85rem; font-weight: 700;
            border: 2px solid var(--border);
            background: var(--border-light);
            color: var(--text-muted);
            font-family: 'Space Mono', monospace;
            transition: all 0.3s; z-index: 1;
        }
        .step.active .step-circle {
            background: var(--primary); border-color: var(--primary);
            color: white; box-shadow: 0 0 0 4px rgba(37,99,235,0.15);
        }
        .step.done .step-circle {
            background: var(--accent-green); border-color: var(--accent-green);
            color: white;
        }
        .step-label {
            font-size: 0.72rem; font-weight: 600; color: var(--text-muted);
            text-align: center; white-space: nowrap;
        }
        .step.active .step-label { color: var(--primary); }
        .step.done  .step-label  { color: var(--accent-green); }

        /* ═══ FORM CARD ═══ */
        .form-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 32px;
            box-shadow: var(--shadow-sm); margin-bottom: 20px;
            animation: slideUp 0.5s ease both;
        }
        .form-card-title {
            font-size: 1rem; font-weight: 700; margin-bottom: 6px;
            display: flex; align-items: center; gap: 10px; color: var(--text);
        }
        .form-card-sub { font-size: 0.83rem; color: var(--text-secondary); margin-bottom: 24px; }
        .title-dot { width: 8px; height: 8px; border-radius: 50%; display: inline-block; flex-shrink: 0; }
        .dot-blue   { background: var(--primary); }
        .dot-green  { background: var(--accent-green); }
        .dot-orange { background: var(--accent-orange); }
        .dot-purple { background: var(--accent-purple); }

        /* ═══ FORM ELEMENTS ═══ */
        .form-group { margin-bottom: 18px; }
        .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 18px; }
        .form-label {
            display: block; font-size: 0.75rem; font-weight: 600;
            color: var(--text-secondary); text-transform: uppercase;
            letter-spacing: 0.5px; margin-bottom: 7px;
        }
        .form-input {
            width: 100%; background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 10px 14px;
            font-family: 'DM Sans', sans-serif; font-size: 0.875rem;
            color: var(--text); outline: none; transition: border-color 0.2s; box-sizing: border-box;
        }
        .form-input:focus { border-color: var(--primary); background: white; }
        textarea.form-input { resize: vertical; min-height: 100px; line-height: 1.5; }
        select.form-input {
            appearance: none; cursor: pointer;
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%2394a3b8' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
            background-repeat: no-repeat; background-position: right 14px center; padding-right: 36px;
        }
        input[type="file"].form-input { padding: 8px 14px; cursor: pointer; }
        .form-hint { font-size: 0.72rem; color: var(--text-muted); margin-top: 5px; }

        /* validators */
        .validation-error { font-size: 0.75rem; color: #dc2626; margin-top: 4px; display: block; }
        .validation-summary {
            background: rgba(239,68,68,0.06); border: 1px solid rgba(239,68,68,0.2);
            color: #dc2626; border-radius: var(--radius-sm);
            padding: 14px 18px; font-size: 0.83rem; margin-bottom: 20px;
        }

        /* ═══ BUTTONS ═══ */
        .btn-primary {
            background: var(--primary); color: white; border: none;
            padding: 11px 28px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-primary:hover { background: #1d4ed8; }
        .btn-secondary {
            background: var(--border-light); color: var(--text-secondary);
            border: 1px solid var(--border); padding: 11px 28px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-secondary:hover { border-color: var(--primary); color: var(--primary); }
        .btn-row { display: flex; gap: 10px; margin-top: 24px; flex-wrap: wrap; }

        /* ═══ SECTION CARD (modules/lessons list) ═══ */
        .section-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); box-shadow: var(--shadow-sm);
            overflow: hidden; margin-bottom: 20px;
        }
        .section-header {
            padding: 16px 24px; border-bottom: 1px solid var(--border);
            display: flex; align-items: center; gap: 10px;
            font-size: 0.9rem; font-weight: 700; color: var(--text);
        }
        .section-card table { width: 100%; border-collapse: collapse; }
        .section-card table th {
            background: var(--border-light); padding: 10px 20px; text-align: left;
            font-size: 0.72rem; font-weight: 700; letter-spacing: 1px; text-transform: uppercase;
            color: var(--text-muted); border-bottom: 1px solid var(--border);
        }
        .section-card table td {
            padding: 12px 20px; font-size: 0.875rem; color: var(--text);
            border-bottom: 1px solid var(--border-light); vertical-align: middle;
        }
        .section-card table tr:last-child td { border-bottom: none; }
        .section-card table tr:hover td { background: var(--surface-hover); }

        .btn-small {
            background: var(--primary-bg); color: var(--primary);
            border: 1px solid var(--primary-border); padding: 5px 14px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.78rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-small:hover { background: var(--primary); color: white; }

        /* ═══ ALERTS ═══ */
        .alert { padding: 10px 16px; border-radius: var(--radius-sm); font-size: 0.83rem; font-weight: 500; margin: 12px 0; display: inline-block; }
        .alert-success { background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2); color: #059669; }
        .alert-error   { background: rgba(239,68,68,0.07); border: 1px solid rgba(239,68,68,0.2); color: #dc2626; }

        /* ═══ REVIEW CARD ═══ */
        .review-info-card {
            background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 18px 20px; margin-bottom: 20px;
        }
        .review-info-row { font-size: 0.875rem; margin-bottom: 8px; }
        .review-info-row:last-child { margin-bottom: 0; }
        .review-info-label { color: var(--text-muted); font-size: 0.75rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 3px; }
        .review-info-value { font-weight: 600; color: var(--text); }

        .module-review-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 18px 20px; margin-bottom: 12px;
        }
        .module-review-title {
            font-size: 0.9rem; font-weight: 700; color: var(--text);
            margin-bottom: 12px; display: flex; align-items: center; gap: 8px;
        }
        .module-tag {
            background: var(--primary-bg); color: var(--primary);
            border: 1px solid var(--primary-border);
            font-size: 0.65rem; font-weight: 700; padding: 2px 10px; border-radius: 20px;
            font-family: 'Space Mono', monospace;
        }
        .lesson-item-row {
            display: flex; justify-content: space-between; align-items: center;
            padding: 8px 12px; background: var(--border-light);
            border-radius: 8px; margin-bottom: 6px; font-size: 0.83rem;
        }
        .lesson-item-row:last-child { margin-bottom: 0; }
        .lesson-name { color: var(--text); font-weight: 500; }
        .lesson-duration { color: var(--text-muted); font-size: 0.75rem; font-family: 'Space Mono', monospace; }

        .publish-banner {
            background: linear-gradient(135deg, #059669, #10b981 55%, #34d399);
            border-radius: var(--radius); padding: 22px 28px;
            margin-bottom: 20px; text-align: center;
            box-shadow: 0 6px 20px rgba(16,185,129,0.25);
        }
        .publish-banner p { color: rgba(255,255,255,0.85); font-size: 0.85rem; margin-top: 4px; }
        .publish-banner h3 { color: white; font-size: 1.1rem; }

        /* ═══ ANIMATIONS ═══ */
        @keyframes slideDown { from{opacity:0;transform:translateY(-12px);}to{opacity:1;transform:translateY(0);} }
        @keyframes slideUp   { from{opacity:0;transform:translateY(14px);}to{opacity:1;transform:translateY(0);} }

        @media(max-width:700px){
            .container{padding:20px;}
            .header,.nav{padding:0 20px;}
            .form-row{grid-template-columns:1fr;}
            .step-indicator{gap:0; padding:16px;}
            .step-label{display:none;}
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
        <a href="CreateCourse.aspx" class="active"><span>➕</span> Create Course</a>
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

    <div class="container">

        <!-- ── PAGE BANNER ── -->
        <div class="page-banner">
            <div class="banner-label">Course Creation Wizard</div>
            <div class="banner-title"><asp:Label ID="lblBannerTitle" runat="server" Text="Create New Course" /></div>
            <div class="banner-sub"><asp:Label ID="lblBannerSub" runat="server" Text="Fill in the details, add modules and lessons, then publish." /></div>
            <span class="draft-pill"><asp:Label ID="lblDraftPill" runat="server" Text="Draft" /></span>
        </div>

        <!-- ── STEP INDICATOR (shared, updated per panel) ── -->
        <div class="step-indicator">
            <div class="step <%= StepClass(1) %>">
                <div class="step-circle"><%=StepIcon(1)%></div>
                <span class="step-label">Course Details</span>
            </div>
            <div class="step <%= StepClass(2) %>">
                <div class="step-circle"><%=StepIcon(2)%></div>
                <span class="step-label">Modules</span>
            </div>
            <div class="step <%= StepClass(3) %>">
                <div class="step-circle"><%=StepIcon(3)%></div>
                <span class="step-label">Lessons</span>
            </div>
            <div class="step <%= StepClass(4) %>">
                <div class="step-circle"><%=StepIcon(4)%></div>
                <span class="step-label">Publish</span>
            </div>
        </div>

        <!-- Course details -->
        <asp:Panel ID="pnlCourseDetails" runat="server">
            <div class="form-card">
                <div class="form-card-title">
                    <span class="title-dot dot-blue"></span> Course Basics
                </div>
                <div class="form-card-sub">Set up the fundamental information about your course.</div>

                <asp:ValidationSummary ID="vsCourse" runat="server"
                    CssClass="validation-summary"
                    HeaderText="Please fix the following errors:"
                    ValidationGroup="courseForm" />

                <div class="form-group">
                    <label class="form-label">Course Title *</label>
                    <asp:TextBox ID="txtCourseName" runat="server" CssClass="form-input" MaxLength="100" />
                    <asp:RequiredFieldValidator ControlToValidate="txtCourseName" runat="server"
                        ErrorMessage="Course title is required." CssClass="validation-error"
                        ValidationGroup="courseForm" Display="Dynamic" />
                    <asp:RegularExpressionValidator ControlToValidate="txtCourseName" runat="server"
                        ValidationExpression="^[a-zA-Z0-9\s\-\+\#\(\)\.]{3,100}$"
                        ErrorMessage="Invalid course title (3–100 chars, letters/numbers/basic symbols)."
                        CssClass="validation-error" ValidationGroup="courseForm" Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label class="form-label">Course Description *</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="5" MaxLength="1000" />
                    <asp:RequiredFieldValidator ControlToValidate="txtDescription" runat="server"
                        ErrorMessage="Description is required." CssClass="validation-error"
                        ValidationGroup="courseForm" Display="Dynamic" />
                </div>

                <div class="form-row">
                    <div class="form-group" style="margin-bottom:0;">
                        <label class="form-label">Category *</label>
                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-input">
                            <asp:ListItem Text="Select Category" Value="" />
                            <asp:ListItem Text="AI" />
                            <asp:ListItem Text="Machine Learning" />
                            <asp:ListItem Text="Web Development" />
                            <asp:ListItem Text="Programming" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ControlToValidate="ddlCategory" InitialValue="" runat="server"
                            ErrorMessage="Select a category." CssClass="validation-error"
                            ValidationGroup="courseForm" Display="Dynamic" />
                    </div>
                    <div class="form-group" style="margin-bottom:0;">
                        <label class="form-label">Level *</label>
                        <asp:DropDownList ID="ddlLevel" runat="server" CssClass="form-input">
                            <asp:ListItem Text="Beginner" />
                            <asp:ListItem Text="Intermediate" />
                            <asp:ListItem Text="Advanced" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="form-group" style="margin-top:18px;">
                    <label class="form-label">Price (RM) <span style="color:var(--text-muted);font-weight:400;">(leave blank or 0 for free)</span></label>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-input" MaxLength="10" placeholder="0.00" />
                    <asp:RangeValidator ControlToValidate="txtPrice" runat="server"
                        MinimumValue="0" MaximumValue="10000" Type="Double"
                        ErrorMessage="Price must be between 0 and 10000."
                        CssClass="validation-error" ValidationGroup="courseForm" Display="Dynamic" />
                </div>

                <div class="btn-row">
                    <asp:Button ID="btnCreate" runat="server" Text="Save &amp; Add Modules →"
                        CssClass="btn-primary" ValidationGroup="courseForm" OnClick="btnCreate_Click" />
                    <asp:Label ID="lblCourseMsg" runat="server" CssClass="alert" Visible="false" />
                </div>
            </div>
        </asp:Panel>

        <!-- Add modules-->
        <asp:Panel ID="pnlAddModules" runat="server" Visible="false">
            <div class="form-card">
                <div class="form-card-title">
                    <span class="title-dot dot-green"></span>
                    Add Modules —
                    <asp:Label ID="lblCourseTitle" runat="server" style="font-weight:500; color:var(--text-secondary);" />
                </div>
                <div class="form-card-sub">Organise your course into modules. Add at least one module before proceeding.</div>

                <asp:ValidationSummary ID="vsModule" runat="server"
                    CssClass="validation-summary" HeaderText="Please fix:"
                    ValidationGroup="moduleForm" />

                <div class="form-group">
                    <label class="form-label">Module Name *</label>
                    <asp:TextBox ID="txtModuleName" runat="server" CssClass="form-input" MaxLength="100" />
                    <asp:RequiredFieldValidator ControlToValidate="txtModuleName" runat="server"
                        ErrorMessage="Module name is required." CssClass="validation-error"
                        ValidationGroup="moduleForm" Display="Dynamic" />
                    <asp:RegularExpressionValidator ControlToValidate="txtModuleName" runat="server"
                        ValidationExpression="^[a-zA-Z0-9 .\-]+$"
                        ErrorMessage="Only letters, numbers, spaces, dots and dashes allowed."
                        CssClass="validation-error" ValidationGroup="moduleForm" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label class="form-label">Module Description</label>
                    <asp:TextBox ID="txtModuleDesc" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="3" MaxLength="1000" />
                </div>

                <div class="btn-row">
                    <asp:Button ID="btnAddModule" runat="server" Text="Add Module"
                        CssClass="btn-primary" ValidationGroup="moduleForm" OnClick="btnAddModule_Click" />
                    <asp:Button ID="btnBackToCourse" runat="server" Text="← Course Details"
                        CssClass="btn-secondary" OnClick="btnBackToCourse_Click" CausesValidation="false" />
                    <asp:Label ID="lblModuleMsg" runat="server" CssClass="alert" Visible="false" />
                </div>
            </div>

            <!-- Modules list -->
            <div class="section-card">
                <div class="section-header">
                    <span class="title-dot dot-green"></span> Modules Added
                </div>
                <asp:GridView ID="gvModules" runat="server"
                    AutoGenerateColumns="False" Width="100%"
                    BorderStyle="None" GridLines="None"
                    OnRowCommand="gvModules_RowCommand"
                    DataKeyNames="moduleid"
                    EmptyDataText="No modules added yet.">
                    <Columns>
                        <asp:BoundField DataField="modulename" HeaderText="Module Name" />
                        <asp:BoundField DataField="moduledescription" HeaderText="Description" />
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:Button ID="btnAddLessons" runat="server" Text="Add Lessons"
                                    CssClass="btn-small" CommandName="AddLessons"
                                    CommandArgument='<%# Eval("moduleid") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div class="btn-row">
                <asp:Button ID="btnContinue" runat="server" Text="Continue to Lessons →"
                    CssClass="btn-primary" OnClick="btnContinue_Click" CausesValidation="false" />
            </div>
        </asp:Panel>

        <!-- Add lessons-->
        <asp:Panel ID="pnlAddLessons" runat="server" Visible="false">
            <div class="form-card">
                <div class="form-card-title">
                    <span class="title-dot dot-orange"></span>
                    Add Lessons —
                    <asp:Label ID="lblModuleName" runat="server" style="font-weight:500; color:var(--text-secondary);" />
                </div>
                <div class="form-card-sub">Add lessons to this module. Each lesson can include a video, files and a duration.</div>

                <asp:ValidationSummary ID="vsLesson" runat="server"
                    CssClass="validation-summary" HeaderText="Please fix:"
                    ValidationGroup="lessonForm" />

                <div class="form-group">
                    <label class="form-label">Lesson Title *</label>
                    <asp:TextBox ID="txtLessonTitle" runat="server" CssClass="form-input" MaxLength="100" />
                    <asp:RequiredFieldValidator ControlToValidate="txtLessonTitle" runat="server"
                        ErrorMessage="Lesson title is required." CssClass="validation-error"
                        ValidationGroup="lessonForm" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label class="form-label">Lesson Description</label>
                    <asp:TextBox ID="txtLessonDesc" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="3" MaxLength="1000" />
                </div>
                <div class="form-row">
                    <div class="form-group" style="margin-bottom:0;">
                        <label class="form-label">Duration (minutes) *</label>
                        <asp:TextBox ID="txtDuration" runat="server" CssClass="form-input" MaxLength="4" placeholder="e.g. 30" />
                        <asp:RequiredFieldValidator ControlToValidate="txtDuration" runat="server"
                            ErrorMessage="Duration is required." CssClass="validation-error"
                            ValidationGroup="lessonForm" Display="Dynamic" />
                        <asp:RegularExpressionValidator ControlToValidate="txtDuration" runat="server"
                            ValidationExpression="^\d+$" ErrorMessage="Duration must be a number."
                            CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                        <asp:RangeValidator ControlToValidate="txtDuration" runat="server"
                            MinimumValue="1" MaximumValue="600" Type="Integer"
                            ErrorMessage="Duration must be 1–600 minutes."
                            CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                    </div>
                    <div class="form-group" style="margin-bottom:0;">
                        <label class="form-label">Lesson Points
                            <span style="color:var(--text-muted);font-weight:400;text-transform:none;letter-spacing:0;">(awarded on completion)</span>
                        </label>
                        <asp:TextBox ID="txtLessonPoints" runat="server" CssClass="form-input" MaxLength="5" placeholder="e.g. 10" />
                        <asp:RegularExpressionValidator ControlToValidate="txtLessonPoints" runat="server"
                            ValidationExpression="^\d*$" ErrorMessage="Points must be a whole number."
                            CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                        <asp:RangeValidator ControlToValidate="txtLessonPoints" runat="server"
                            MinimumValue="0" MaximumValue="10000" Type="Integer"
                            ErrorMessage="Points must be between 0 and 10000."
                            CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                        <span class="form-hint">Leave blank for no points on this lesson.</span>
                    </div>
                </div>
                <div class="form-group" style="margin-top:18px;">
                    <label class="form-label">Video URL</label>
                    <asp:TextBox ID="txtVideoUrl" runat="server" CssClass="form-input" MaxLength="500" placeholder="https://..." />
                    <asp:RegularExpressionValidator ControlToValidate="txtVideoUrl" runat="server"
                        ValidationExpression="^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-\.~:\/?#\[\]@!\$&amp;'\(\)\*\+,;=]*)?$"
                        ErrorMessage="Invalid video URL format."
                        CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                </div>
                <div class="form-group" style="margin-top:18px;">
                    <label class="form-label">Upload File (PDF, DOC, DOCX, PPT, PPTX — max 5MB)</label>
                    <asp:FileUpload ID="fuLessonFile" runat="server" CssClass="form-input" />
                    <asp:RegularExpressionValidator ControlToValidate="fuLessonFile" runat="server"
                        ValidationExpression="^.*\.(pdf|doc|docx|ppt|pptx)$"
                        ErrorMessage="Only PDF, DOC, DOCX, PPT, PPTX files allowed."
                        CssClass="validation-error" ValidationGroup="lessonForm" Display="Dynamic" />
                </div>

                <div class="btn-row">
                    <asp:Button ID="btnAddLesson" runat="server" Text="Add Lesson"
                        CssClass="btn-primary" ValidationGroup="lessonForm" OnClick="btnAddLesson_Click" />
                    <asp:Button ID="btnBackToModules" runat="server" Text="← Back to Modules"
                        CssClass="btn-secondary" OnClick="btnBackToModules_Click" CausesValidation="false" />
                    <asp:Label ID="lblLessonMsg" runat="server" CssClass="alert" Visible="false" />
                </div>
            </div>

            <!-- Lessons list -->
            <div class="section-card">
                <div class="section-header">
                    <span class="title-dot dot-orange"></span> Lessons Added
                </div>
                <asp:GridView ID="gvLessons" runat="server"
                    AutoGenerateColumns="False" Width="100%"
                    BorderStyle="None" GridLines="None"
                    EmptyDataText="No lessons added yet.">
                    <Columns>
                        <asp:BoundField DataField="lessontitle" HeaderText="Lesson Title" />
                        <asp:BoundField DataField="duration"    HeaderText="Duration (mins)" />
                        <asp:BoundField DataField="lessonpoints" HeaderText="Points" />
                    </Columns>
                </asp:GridView>
            </div>

            <div class="btn-row">
                <asp:Button ID="btnGoToReview" runat="server" Text="Review &amp; Publish →"
                    CssClass="btn-primary" OnClick="btnGoToReview_Click" CausesValidation="false" />
            </div>
        </asp:Panel>

        <!-- Review and Publish-->
        <asp:Panel ID="pnlReviewPublish" runat="server" Visible="false">

            <div class="publish-banner">
                <h3>🚀 Ready to Publish!</h3>
                <p>Review your course below and hit Publish when you're happy with it.</p>
            </div>

            <!-- Course info -->
            <div class="form-card">
                <div class="form-card-title">
                    <span class="title-dot dot-blue"></span> Course Overview
                </div>
                <div class="review-info-card">
                    <div class="review-info-label">Course Name</div>
                    <div class="review-info-value"><asp:Label ID="lblCourseName" runat="server" /></div>
                    <div style="margin-top:12px;" class="review-info-label">Description</div>
                    <div class="review-info-value"><asp:Label ID="lblCourseDesc" runat="server" /></div>
                    <div style="margin-top:12px;" class="review-info-label">Price</div>
                    <div class="review-info-value"><asp:Label ID="lblCoursePrice" runat="server" /></div>
                </div>

                <!-- Modules + Lessons nested repeaters -->
                <div class="form-card-title" style="margin-top:20px;">
                    <span class="title-dot dot-green"></span> Modules &amp; Lessons
                </div>

                <asp:Repeater ID="rptModules" runat="server">
                    <ItemTemplate>
                        <div class="module-review-card">
                            <div class="module-review-title">
                                <%# Server.HtmlEncode(Eval("modulename").ToString()) %>
                                <span class="module-tag">MODULE</span>
                            </div>
                            <asp:Repeater ID="rptLessons" runat="server" DataSource='<%# Eval("Lessons") %>'>
                                <ItemTemplate>
                                    <div class="lesson-item-row">
                                        <span class="lesson-name">📖 <%# Server.HtmlEncode(Eval("lessontitle").ToString()) %></span>
                                        <span class="lesson-duration">
                                            <%# Server.HtmlEncode(Eval("duration").ToString()) %> mins
                                            <%# (Eval("lessonpoints") != DBNull.Value && Convert.ToInt32(Eval("lessonpoints")) > 0)
                                                ? " · <span style='color:var(--accent-orange);font-weight:700;'>⚡ " + Eval("lessonpoints") + " pts</span>"
                                                : "" %>
                                        </span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <div class="btn-row">
                    <asp:Button ID="btnPublish" runat="server" Text="🚀 Publish Course"
                        CssClass="btn-primary" OnClick="btnPublish_Click" CausesValidation="false" />
                    <asp:Button ID="btnBackToLessons" runat="server" Text="← Back to Lessons"
                        CssClass="btn-secondary" OnClick="btnBackToLessons_Click" CausesValidation="false" />
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
