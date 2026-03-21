<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MakeExam.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.MakeExam" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Make Exam - LearnSphere</title>
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
        .container { max-width: 900px; margin: 0 auto; padding: 28px 36px; }

        /* ═══ PAGE BANNER ═══ */
        .page-banner {
            background: linear-gradient(135deg, #7c3aed 0%, #8b5cf6 55%, #a78bfa 100%);
            border-radius: var(--radius); padding: 28px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(139,92,246,0.28);
            animation: slideDown 0.5s ease both;
        }
        .page-banner::before {
            content: ''; position: absolute; top: -40%; right: -10%; width: 280px; height: 280px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%);
            border-radius: 50%; pointer-events: none;
        }
        .banner-label { font-size: 0.72rem; font-weight: 700; letter-spacing: 2px; text-transform: uppercase; color: rgba(255,255,255,0.75); margin-bottom: 6px; font-family: 'Space Mono', monospace; }
        .banner-title { font-size: 1.5rem; font-weight: 700; color: white; margin-bottom: 4px; }
        .banner-sub   { color: rgba(255,255,255,0.75); font-size: 0.85rem; }

        /* ═══ BACK BUTTON ═══ */
        .btn-back {
            display: inline-flex; align-items: center; gap: 8px;
            background: var(--surface); border: 1px solid var(--border);
            color: var(--text-secondary); padding: 8px 18px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.83rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s; margin-bottom: 20px; text-decoration: none;
        }
        .btn-back:hover { border-color: var(--primary); color: var(--primary); }

        /* ═══ FORM CARD ═══ */
        .form-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 32px;
            box-shadow: var(--shadow-sm); margin-bottom: 20px;
            animation: slideUp 0.5s ease both;
        }
        .form-card-title {
            font-size: 0.95rem; font-weight: 700; margin-bottom: 6px;
            display: flex; align-items: center; gap: 10px; color: var(--text);
        }
        .form-card-sub { font-size: 0.83rem; color: var(--text-secondary); margin-bottom: 24px; }
        .title-dot { width: 8px; height: 8px; border-radius: 50%; display: inline-block; flex-shrink: 0; }
        .dot-purple { background: var(--accent-purple); }
        .dot-blue   { background: var(--primary); }
        .dot-orange { background: var(--accent-orange); }
        .dot-green  { background: var(--accent-green); }

        /* ═══ FORM ELEMENTS ═══ */
        .form-group { margin-bottom: 18px; }
        .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 18px; }
        .form-row-4 { display: grid; grid-template-columns: 1fr 1fr 1fr 1fr; gap: 12px; margin-bottom: 18px; }
        .form-label {
            display: block; font-size: 0.75rem; font-weight: 600; color: var(--text-secondary);
            text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 7px;
        }
        .form-input {
            width: 100%; background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 10px 14px;
            font-family: 'DM Sans', sans-serif; font-size: 0.875rem;
            color: var(--text); outline: none; transition: border-color 0.2s; box-sizing: border-box;
        }
        .form-input:focus { border-color: var(--primary); background: white; }
        textarea.form-input { resize: vertical; min-height: 80px; line-height: 1.5; }
        select.form-input {
            appearance: none; cursor: pointer;
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%2394a3b8' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
            background-repeat: no-repeat; background-position: right 14px center; padding-right: 36px;
        }

        /* validators */
        .validation-error { font-size: 0.75rem; color: #dc2626; margin-top: 4px; display: block; }
        .validation-summary {
            background: rgba(239,68,68,0.06); border: 1px solid rgba(239,68,68,0.2);
            color: #dc2626; border-radius: var(--radius-sm);
            padding: 14px 18px; font-size: 0.83rem; margin-bottom: 20px;
        }

        /* ═══ DIVIDER ═══ */
        .divider {
            border: none; border-top: 1px solid var(--border);
            margin: 24px 0;
        }

        /* ═══ BUTTONS ═══ */
        .btn-primary {
            background: var(--primary); color: white; border: none;
            padding: 10px 24px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-primary:hover { background: #1d4ed8; }
        .btn-secondary {
            background: var(--border-light); color: var(--text-secondary);
            border: 1px solid var(--border); padding: 10px 24px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-secondary:hover { border-color: var(--primary); color: var(--primary); }
        .btn-success {
            background: var(--accent-green); color: white; border: none;
            padding: 10px 24px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-success:hover { background: #059669; }
        .btn-danger {
            background: rgba(239,68,68,0.08); color: var(--accent-red);
            border: 1px solid rgba(239,68,68,0.2); padding: 10px 24px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-danger:hover { background: var(--accent-red); color: white; }
        .btn-row { display: flex; gap: 10px; flex-wrap: wrap; margin-top: 20px; align-items: center; }

        /* ═══ QUESTIONS TABLE ═══ */
        .questions-section {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); box-shadow: var(--shadow-sm);
            overflow: hidden; margin-bottom: 20px;
        }
        .questions-header {
            padding: 16px 24px; border-bottom: 1px solid var(--border);
            display: flex; align-items: center; justify-content: space-between;
        }
        .questions-title { font-size: 0.9rem; font-weight: 700; display: flex; align-items: center; gap: 10px; }
        .q-count {
            background: var(--bg); border: 1px solid var(--border);
            font-size: 0.72rem; font-family: 'Space Mono', monospace;
            padding: 3px 10px; border-radius: 20px; color: var(--text-muted);
        }

        .questions-section table { width: 100%; border-collapse: collapse; }
        .questions-section table th {
            background: var(--border-light); padding: 10px 16px; text-align: left;
            font-size: 0.7rem; font-weight: 700; letter-spacing: 1px; text-transform: uppercase;
            color: var(--text-muted); border-bottom: 1px solid var(--border);
        }
        .questions-section table td {
            padding: 11px 16px; font-size: 0.83rem; color: var(--text);
            border-bottom: 1px solid var(--border-light); vertical-align: middle;
        }
        .questions-section table tr:last-child td { border-bottom: none; }
        .questions-section table tr:hover td { background: var(--surface-hover); }

        .correct-badge {
            display: inline-block; background: rgba(16,185,129,0.1);
            border: 1px solid rgba(16,185,129,0.25); color: #059669;
            font-size: 0.72rem; font-weight: 700; padding: 2px 10px; border-radius: 20px;
            font-family: 'Space Mono', monospace;
        }
        .marks-badge {
            display: inline-block; background: var(--primary-bg);
            border: 1px solid var(--primary-border); color: var(--primary);
            font-size: 0.72rem; font-weight: 700; padding: 2px 10px; border-radius: 20px;
            font-family: 'Space Mono', monospace;
        }
        .empty-state { text-align: center; padding: 36px; color: var(--text-muted); font-size: 0.88rem; }

        /* ═══ ALERTS ═══ */
        .alert { padding: 10px 16px; border-radius: var(--radius-sm); font-size: 0.83rem; font-weight: 500; display: inline-block; margin-top: 8px; }
        .alert-success { background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2); color: #059669; }
        .alert-error   { background: rgba(239,68,68,0.07); border: 1px solid rgba(239,68,68,0.2); color: #dc2626; }

        /* ═══ ANIMATIONS ═══ */
        @keyframes slideDown { from{opacity:0;transform:translateY(-12px);}to{opacity:1;transform:translateY(0);} }
        @keyframes slideUp   { from{opacity:0;transform:translateY(14px);}to{opacity:1;transform:translateY(0);} }

        @media(max-width:900px){
            .container{padding:20px;}
            .header,.nav{padding:0 20px;}
            .form-row,.form-row-4{grid-template-columns:1fr;}
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
                <span class="user-name"><%= Session["uname"] != null ? Server.HtmlEncode(Session["uname"].ToString()) : "" %></span>
            </div>
            <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout"
                OnClick="btnLogout_Click" CausesValidation="false" />
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

        <!-- Page Banner -->
        <div class="page-banner">
            <div class="banner-label">Course Management</div>
            <div class="banner-title"><asp:Label ID="lblBannerTitle" runat="server" Text="Create Exam" /></div>
            <div class="banner-sub">Build a multiple-choice exam with questions and answer options.</div>
        </div>

        <a href="ViewCourses.aspx" class="btn-back">← Back to Courses</a>

        <!-- ── SECTION 1: EXAM SETUP ── -->
        <div class="form-card">
            <div class="form-card-title">
                <span class="title-dot dot-purple"></span> Exam Setup
            </div>
            <div class="form-card-sub">Choose the exam type, target, and give your exam a title.</div>

            <asp:ValidationSummary ID="vsSummary" runat="server"
                CssClass="validation-summary"
                HeaderText="Please fix the following errors:"
                ValidationGroup="ExamGroup" />

            <div class="form-row">
                <div class="form-group" style="margin-bottom:0;">
                    <label class="form-label">Exam Type *</label>
                    <asp:DropDownList ID="ddlExamType" runat="server" CssClass="form-input"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlExamType_SelectedIndexChanged">
                        <asp:ListItem Text="Select Exam Type" Value="" />
                        <asp:ListItem Text="Course Exam"      Value="course" />
                        <asp:ListItem Text="Module Exam"      Value="module" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ControlToValidate="ddlExamType" InitialValue=""
                        ErrorMessage="Select exam type." CssClass="validation-error"
                        ValidationGroup="ExamGroup" runat="server" Display="Dynamic" />
                </div>
                <div class="form-group" style="margin-bottom:0;">
                    <label class="form-label">Select Module / Course *</label>
                    <asp:DropDownList ID="ddlTarget" runat="server" CssClass="form-input"
                        AppendDataBoundItems="true">
                        <asp:ListItem Text="Select Target" Value="" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ControlToValidate="ddlTarget" InitialValue=""
                        ErrorMessage="Select a target." CssClass="validation-error"
                        ValidationGroup="ExamGroup" runat="server" Display="Dynamic" />
                </div>
            </div>

            <div class="form-group">
                <label class="form-label">Exam Title *</label>
                <asp:TextBox ID="txtExamTitle" runat="server" CssClass="form-input"
                    MaxLength="100" placeholder="e.g. End of Module Quiz" />
                <asp:RequiredFieldValidator ControlToValidate="txtExamTitle"
                    ErrorMessage="Enter exam title." CssClass="validation-error"
                    ValidationGroup="ExamGroup" runat="server" Display="Dynamic" />
            </div>
        </div>

        <!-- ── SECTION 2: ADD QUESTION ── -->
        <div class="form-card">
            <div class="form-card-title">
                <span class="title-dot dot-blue"></span> Add Question
            </div>
            <div class="form-card-sub">Fill in the question, four answer options, the correct answer, and marks.</div>

            <div class="form-group">
                <label class="form-label">Question *</label>
                <asp:TextBox ID="txtQuestion" runat="server" CssClass="form-input"
                    TextMode="MultiLine" MaxLength="500"
                    placeholder="Type your question here..." />
                <asp:RequiredFieldValidator ControlToValidate="txtQuestion"
                    ErrorMessage="Enter question text." CssClass="validation-error"
                    ValidationGroup="QuestionGroup" runat="server" Display="Dynamic" />
            </div>

            <!-- 4 options in a row -->
            <div class="form-row-4">
                <div class="form-group" style="margin-bottom:0;">
                    <label class="form-label">Option A *</label>
                    <asp:TextBox ID="txtA" runat="server" CssClass="form-input" MaxLength="200" />
                    <asp:RequiredFieldValidator ControlToValidate="txtA"
                        ErrorMessage="Enter option A." CssClass="validation-error"
                        ValidationGroup="QuestionGroup" runat="server" Display="Dynamic" />
                </div>
                <div class="form-group" style="margin-bottom:0;">
                    <label class="form-label">Option B *</label>
                    <asp:TextBox ID="txtB" runat="server" CssClass="form-input" MaxLength="200" />
                    <asp:RequiredFieldValidator ControlToValidate="txtB"
                        ErrorMessage="Enter option B." CssClass="validation-error"
                        ValidationGroup="QuestionGroup" runat="server" Display="Dynamic" />
                </div>
                <div class="form-group" style="margin-bottom:0;">
                    <label class="form-label">Option C *</label>
                    <asp:TextBox ID="txtC" runat="server" CssClass="form-input" MaxLength="200" />
                    <asp:RequiredFieldValidator ControlToValidate="txtC"
                        ErrorMessage="Enter option C." CssClass="validation-error"
                        ValidationGroup="QuestionGroup" runat="server" Display="Dynamic" />
                </div>
                <div class="form-group" style="margin-bottom:0;">
                    <label class="form-label">Option D *</label>
                    <asp:TextBox ID="txtD" runat="server" CssClass="form-input" MaxLength="200" />
                    <asp:RequiredFieldValidator ControlToValidate="txtD"
                        ErrorMessage="Enter option D." CssClass="validation-error"
                        ValidationGroup="QuestionGroup" runat="server" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-group" style="margin-bottom:0;">
                    <label class="form-label">Correct Answer *</label>
                    <asp:DropDownList ID="ddlCorrect" runat="server" CssClass="form-input">
                        <asp:ListItem Text="Select Correct Answer" Value="" />
                        <asp:ListItem Text="A" Value="A" />
                        <asp:ListItem Text="B" Value="B" />
                        <asp:ListItem Text="C" Value="C" />
                        <asp:ListItem Text="D" Value="D" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ControlToValidate="ddlCorrect" InitialValue=""
                        ErrorMessage="Select the correct answer." CssClass="validation-error"
                        ValidationGroup="QuestionGroup" runat="server" Display="Dynamic" />
                </div>
                <div class="form-group" style="margin-bottom:0;">
                    <label class="form-label">Marks *</label>
                    <asp:TextBox ID="txtMarks" runat="server" CssClass="form-input"
                        Text="1" TextMode="Number" />
                    <asp:RequiredFieldValidator ControlToValidate="txtMarks"
                        ErrorMessage="Enter marks." CssClass="validation-error"
                        ValidationGroup="QuestionGroup" runat="server" Display="Dynamic" />
                    <asp:RangeValidator ControlToValidate="txtMarks"
                        MinimumValue="1" MaximumValue="100" Type="Integer"
                        ErrorMessage="Marks must be between 1 and 100."
                        CssClass="validation-error" ValidationGroup="QuestionGroup"
                        runat="server" Display="Dynamic" />
                </div>
            </div>

            <div class="btn-row">
                <asp:Button ID="btnAddQuestion" runat="server" Text="+ Add Question"
                    CssClass="btn-primary" ValidationGroup="QuestionGroup"
                    OnClick="btnAddQuestion_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false" />
            </div>
        </div>

        <!-- ── SECTION 3: QUESTIONS LIST ── -->
        <div class="questions-section">
            <div class="questions-header">
                <div class="questions-title">
                    <span class="title-dot dot-orange"></span> Questions Added
                    <span class="q-count"><asp:Label ID="lblQCount" runat="server" Text="0" /> questions</span>
                </div>
                <div>
                    <asp:DropDownList ID="ddlQuestionFilter" runat="server" CssClass="form-input"
                        AutoPostBack="true" style="width:160px;padding:6px 12px;font-size:0.8rem;">
                        <asp:ListItem Text="All Questions" Value="all" />
                        <asp:ListItem Text="Module Questions" Value="module" />
                        <asp:ListItem Text="Course Questions" Value="course" />
                    </asp:DropDownList>
                </div>
            </div>

            <asp:GridView ID="gvQuestions" runat="server"
                AutoGenerateColumns="false"
                Width="100%" BorderStyle="None" GridLines="None"
                EmptyDataText="">
                <Columns>
                    <asp:BoundField DataField="Question" HeaderText="Question" />
                    <asp:BoundField DataField="A"        HeaderText="A" />
                    <asp:BoundField DataField="B"        HeaderText="B" />
                    <asp:BoundField DataField="C"        HeaderText="C" />
                    <asp:BoundField DataField="D"        HeaderText="D" />
                    <asp:TemplateField HeaderText="Answer">
                        <ItemTemplate>
                            <span class="correct-badge"><%# Eval("Correct") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Marks">
                        <ItemTemplate>
                            <span class="marks-badge"><%# Eval("Marks") %> pts</span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowSelectButton="true" SelectText="Select" />
                </Columns>
            </asp:GridView>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="true">
                <div class="empty-state">📝 No questions added yet. Use the form above to add questions.</div>
            </asp:Panel>
        </div>

        <!-- ── SECTION 4: PUBLISH / DRAFT ── -->
        <div class="form-card">
            <div class="form-card-title">
                <span class="title-dot dot-green"></span> Finalise Exam
            </div>
            <div class="form-card-sub">Publish the exam to make it available, or save as a draft to continue later.</div>

            <div class="btn-row">
                <asp:Button ID="btnPublish" runat="server" Text="🚀 Publish Exam"
                    CssClass="btn-success" ValidationGroup="ExamGroup" OnClick="btnPublish_Click" />
                <asp:Button ID="btnDraft" runat="server" Text="Save as Draft"
                    CssClass="btn-secondary" CausesValidation="false" OnClick="btnDraft_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel"
                    CssClass="btn-danger" CausesValidation="false" OnClick="btnCancel_Click" />
            </div>
        </div>

    </div>

    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
    <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

</form>
</body>
</html>
