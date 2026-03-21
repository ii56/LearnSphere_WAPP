<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="EditProfile.aspx.cs"
    Inherits="LearnSphere_WAPP.Lecturer.EditProfile" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Profile - LearnSphere</title>
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
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04), 0 1px 2px rgba(0,0,0,0.03);
            --shadow-md: 0 4px 12px rgba(0,0,0,0.06), 0 2px 4px rgba(0,0,0,0.03);
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

        /* ═══ HEADER ═══ */
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82);
            backdrop-filter: blur(20px); -webkit-backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 12px; text-decoration: none; }
        .logo img { height: 38px; width: 38px; object-fit: contain; }
        .logo-text { font-size: 1.2rem; font-weight: 700; color: var(--text); letter-spacing: -0.3px; }
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
            border-radius: 50%; display: flex; align-items: center;
            justify-content: center; font-size: 13px; font-weight: 700; color: white;
            overflow: hidden; position: relative;
        }
        .user-avatar img {
            width: 100%; height: 100%; object-fit: cover; border-radius: 50%;
            position: absolute; top: 0; left: 0;
        }
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
            border-bottom: 2.5px solid transparent; transition: all 0.2s; position: relative;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active { color: var(--primary); border-bottom-color: var(--primary); }
        .nav-badge {
            background: #ef4444; color: white; font-size: 0.65rem; font-weight: 700;
            padding: 1px 6px; border-radius: 10px; font-family: 'Space Mono', monospace;
            min-width: 18px; text-align: center; display: inline-block;
        }

        /* ═══ CONTAINER ═══ */
        .container { max-width: 900px; margin: 0 auto; padding: 28px 36px; }

        /* ═══ PAGE HEADER ═══ */
        .page-header { margin-bottom: 24px; animation: slideUp 0.4s ease both; }
        .page-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: var(--accent-green); margin-bottom: 6px;
            font-family: 'Space Mono', monospace;
        }
        .page-title { font-size: 1.6rem; font-weight: 700; }

        /* ═══ PROFILE HERO BANNER ═══ */
        .profile-hero {
            background: linear-gradient(135deg, #059669 0%, #10b981 55%, #34d399 100%);
            border-radius: var(--radius); padding: 32px;
            margin-bottom: 20px; display: flex; align-items: center; gap: 24px;
            position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(16,185,129,0.28);
            animation: slideUp 0.5s ease both;
        }
        .profile-hero::before {
            content: ''; position: absolute; top: -40%; right: -10%;
            width: 250px; height: 250px;
            background: radial-gradient(circle, rgba(255,255,255,0.1), transparent 65%);
            border-radius: 50%; pointer-events: none;
        }
        .hero-avatar-wrap { position: relative; flex-shrink: 0; }
        .hero-avatar {
            width: 90px; height: 90px; border-radius: 50%;
            border: 3px solid rgba(255,255,255,0.4); overflow: hidden;
            background: rgba(255,255,255,0.2);
            display: flex; align-items: center; justify-content: center;
        }
        .hero-avatar img { width: 100%; height: 100%; object-fit: cover; }
        .hero-avatar-initial {
            font-size: 2.2rem; font-weight: 700; color: white;
        }
        .hero-verify {
            position: absolute; bottom: 2px; right: 2px;
            width: 24px; height: 24px; border-radius: 50%;
            background: var(--accent-green); color: white; border: 2px solid white;
            display: flex; align-items: center; justify-content: center;
            font-size: 0.65rem; font-weight: 700;
        }
        .hero-info h2 { font-size: 1.4rem; font-weight: 700; color: white; margin-bottom: 4px; }
        .hero-info p { font-size: 0.85rem; color: rgba(255,255,255,0.75); }
        .role-tag {
            display: inline-block; margin-top: 8px;
            background: rgba(255,255,255,0.2); border: 1px solid rgba(255,255,255,0.3);
            color: white; font-size: 0.7rem; font-weight: 700;
            letter-spacing: 0.8px; text-transform: uppercase;
            padding: 3px 12px; border-radius: 20px;
            font-family: 'Space Mono', monospace;
        }

        /* ═══ CARDS ═══ */
        .card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 28px;
            box-shadow: var(--shadow-sm); margin-bottom: 20px;
            animation: slideUp 0.5s ease both;
        }
        .card-title {
            font-size: 0.95rem; font-weight: 700; margin-bottom: 20px;
            display: flex; align-items: center; gap: 10px; color: var(--text);
        }
        .title-dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
        .dot-green  { background: var(--accent-green); }
        .dot-blue   { background: var(--primary); }
        .dot-purple { background: var(--accent-purple); }
        .dot-orange { background: var(--accent-orange); }

        /* ═══ FORM ELEMENTS ═══ */
        .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
        .form-group { display: flex; flex-direction: column; gap: 6px; }
        .form-group.full { grid-column: 1 / -1; }
        .form-label {
            font-size: 0.78rem; font-weight: 600; color: var(--text-secondary);
            text-transform: uppercase; letter-spacing: 0.5px;
        }
        .form-input, .modern-input {
            width: 100%; background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 11px 14px;
            color: var(--text); font-family: 'DM Sans', sans-serif;
            font-size: 0.875rem; outline: none; transition: border-color 0.2s;
            box-sizing: border-box;
        }
        .form-input:focus, .modern-input:focus { border-color: var(--primary); background: white; }
        .form-input:disabled, .modern-input:disabled {
            background: var(--border-light); color: var(--text-muted); cursor: not-allowed;
        }
        textarea.form-input, textarea.modern-input {
            resize: vertical; min-height: 90px; line-height: 1.5;
        }
        select.form-input, select.modern-input {
            appearance: none; cursor: pointer;
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%2394a3b8' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
            background-repeat: no-repeat; background-position: right 14px center;
            padding-right: 36px;
        }

        /* validation */
        .validation-error {
            font-size: 0.75rem; color: #dc2626; margin-top: 2px; display: block;
        }
        .validation-summary {
            background: rgba(239,68,68,0.06); border: 1px solid rgba(239,68,68,0.2);
            color: #dc2626; border-radius: var(--radius-sm);
            padding: 14px 18px; font-size: 0.83rem; margin-bottom: 20px;
        }

        /* feedback labels */
        .msg-success {
            display: inline-block; margin-top: 10px;
            background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2);
            color: #059669; padding: 8px 16px; border-radius: var(--radius-sm);
            font-size: 0.83rem; font-weight: 500;
        }
        .msg-error {
            display: inline-block; margin-top: 10px;
            background: rgba(239,68,68,0.07); border: 1px solid rgba(239,68,68,0.2);
            color: #dc2626; padding: 8px 16px; border-radius: var(--radius-sm);
            font-size: 0.83rem; font-weight: 500;
        }

        /* buttons */
        .btn-primary {
            background: var(--primary); color: white; border: none;
            padding: 11px 28px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-primary:hover { background: #1d4ed8; }
        .btn-row { display: flex; gap: 10px; margin-top: 20px; flex-wrap: wrap; }

        /* upload */
        .upload-section { display: flex; align-items: center; gap: 20px; }
        .upload-preview {
            width: 70px; height: 70px; border-radius: 50%;
            border: 3px solid var(--border); overflow: hidden;
            background: var(--border-light); flex-shrink: 0;
            display: flex; align-items: center; justify-content: center;
        }
        .upload-preview img { width: 100%; height: 100%; object-fit: cover; }
        .upload-preview-initial { font-size: 1.6rem; font-weight: 700; color: var(--text-muted); }

        /* verification history */
        .verification-item {
            background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 14px 18px;
            font-size: 0.83rem; margin-bottom: 10px; line-height: 1.7;
        }
        .verification-item .pending  { color: var(--accent-orange); font-weight: 700; }
        .verification-item .approved { color: var(--accent-green);  font-weight: 700; }
        .verification-item .rejected { color: #ef4444;              font-weight: 700; }
        .verification-item hr { border: none; border-top: 1px solid var(--border); margin: 8px 0; }

        /* ═══ ANIMATIONS ═══ */
        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }

        /* ═══ RESPONSIVE ═══ */
        @media (max-width: 700px) {
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
            .form-grid { grid-template-columns: 1fr; }
            .profile-hero { flex-direction: column; text-align: center; }
            .upload-section { flex-direction: column; }
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
            <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
        </div>
    </div>

    <!-- ═══ NAV ═══ -->
    <div class="nav">
        <a href="LecturerDashboard.aspx"><span>📊</span> Dashboard</a>
        <a href="CreateCourse.aspx"><span>➕</span> Create Course</a>
        <a href="ViewCourses.aspx"><span>📚</span> View Courses</a>
        <a href="EditProfile.aspx" class="active"><span>👤</span> Edit Profile</a>
        <a href="Forums.aspx"><span>💬</span> Forums</a>
        <a href="Message.aspx">
            <span>✉️</span> Messaging
            <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                <span class="nav-badge"><%= Session["unreadCount"] %></span>
            <% } %>
        </a>
    </div>

    <!-- ═══ CONTENT ═══ -->
    <div class="container">

        <div class="page-header">
            <div class="page-label">Account</div>
            <div class="page-title">Profile Settings</div>
        </div>

        <!-- Validation Summary -->
        <asp:ValidationSummary ID="ValidationSummary1" runat="server"
            CssClass="validation-summary"
            HeaderText="Please fix the following errors:"
            ValidationGroup="profileGroup" />

        <!-- ── PROFILE HERO BANNER ── -->
        <div class="profile-hero">
            <div class="hero-avatar-wrap">
                <div class="hero-avatar">
                    <img id="imgHeroProfile" runat="server" style="width:100%;height:100%;object-fit:cover;" />
                </div>
                <% if (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") { %>
                    <div class="hero-verify">✔</div>
                <% } %>
            </div>
            <div class="hero-info">
                <h2><%= Session["uname"] != null ? Server.HtmlEncode(Session["uname"].ToString()) : "Lecturer" %></h2>
                <p><asp:Label ID="lblHeroEmail" runat="server" /></p>
                <span class="role-tag">
                    <%= (Session["usertype"] != null) ? Session["usertype"].ToString() : "Lecturer" %>
                </span>
            </div>
        </div>

        <!-- ── PERSONAL INFORMATION ── -->
        <div class="card">
            <div class="card-title">
                <span class="title-dot dot-green"></span>
                Personal Information
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input" Enabled="false" />
                </div>
                <div class="form-group">
                    <label class="form-label">Email *</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input" MaxLength="100" />
                    <asp:RequiredFieldValidator ControlToValidate="txtEmail" ErrorMessage="Email is required."
                        CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                    <asp:RegularExpressionValidator ControlToValidate="txtEmail"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                        ErrorMessage="Invalid email format." CssClass="validation-error"
                        ValidationGroup="profileGroup" runat="server" />
                </div>
                <div class="form-group">
                    <label class="form-label">First Name *</label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input" MaxLength="50" />
                    <asp:RequiredFieldValidator ControlToValidate="txtFirstName" ErrorMessage="First name required."
                        CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                </div>
                <div class="form-group">
                    <label class="form-label">Last Name *</label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input" MaxLength="50" />
                    <asp:RequiredFieldValidator ControlToValidate="txtLastName" ErrorMessage="Last name required."
                        CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                </div>
                <div class="form-group">
                    <label class="form-label">Age *</label>
                    <asp:TextBox ID="txtAge" runat="server" CssClass="form-input" MaxLength="3" />
                    <asp:RequiredFieldValidator ControlToValidate="txtAge" ErrorMessage="Age required."
                        CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                    <asp:RangeValidator ControlToValidate="txtAge" MinimumValue="13" MaximumValue="120"
                        Type="Integer" ErrorMessage="Age must be between 13 and 120."
                        CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                </div>
                <div class="form-group">
                    <label class="form-label">Gender</label>
                    <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-input">
                        <asp:ListItem Text="Male" />
                        <asp:ListItem Text="Female" />
                        <asp:ListItem Text="Other" />
                    </asp:DropDownList>
                </div>
                <div class="form-group full">
                    <label class="form-label">Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="4" MaxLength="500" />
                </div>
            </div>

            <div class="btn-row">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-primary"
                    ValidationGroup="profileGroup" OnClick="btnSave_Click" />
                <asp:Label ID="lblMessage" runat="server" />
            </div>
        </div>

        <!-- ── ACCOUNT SECURITY ── -->
        <div class="card">
            <div class="card-title">
                <span class="title-dot dot-blue"></span>
                Account Security
            </div>
            <div class="form-grid">
                <div class="form-group full">
                    <label class="form-label">New Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                        CssClass="form-input" MaxLength="50" />
                </div>
            </div>
            <div class="btn-row">
                <asp:Button ID="btnSavePassword" runat="server" Text="Update Password" CssClass="btn-primary"
                    OnClick="btnSave_Click" ValidationGroup="profileGroup" />
            </div>
        </div>

        <!-- ── PROFILE PICTURE ── -->
        <div class="card">
            <div class="card-title">
                <span class="title-dot dot-purple"></span>
                Profile Picture
            </div>
            <div class="upload-section">
                <div class="upload-preview">
                    <img id="imgPreview"
                         src='<%= ResolveUrl(Session["profileImage"] != null
                                ? Session["profileImage"].ToString()
                                : "~/images/default-user.png") %>'
                         style="width:100%;height:100%;object-fit:cover;border-radius:50%;" />
                </div>
                <div>
                    <label class="form-label" style="margin-bottom:8px;">Upload new photo</label>
                    <asp:FileUpload ID="fuProfileImage" runat="server" CssClass="form-input"
                        onchange="previewImage(this)" style="padding:8px;" />
                    <asp:Label ID="lblUploadMessage" runat="server" />
                </div>
            </div>
        </div>

        <!-- ── ROLE VERIFICATION REQUEST ── -->
        <div class="card">
            <div class="card-title">
                <span class="title-dot dot-orange"></span>
                Request Role Upgrade
            </div>
            <p style="font-size:0.85rem; color:var(--text-secondary); margin-bottom:16px;">
                Submit a request to upgrade your account role. Approval is required from an administrator.
            </p>
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Current Role</label>
                    <asp:TextBox ID="txtCurrentRole" runat="server" CssClass="form-input" Enabled="false" />
                </div>
                <div class="form-group">
                    <label class="form-label">Request New Role *</label>
                    <asp:DropDownList ID="ddlRequestedRole" runat="server" CssClass="form-input" />
                    <asp:RequiredFieldValidator ControlToValidate="ddlRequestedRole" InitialValue=""
                        ErrorMessage="Please select a role." CssClass="validation-error"
                        ValidationGroup="verifyGroup" runat="server" />
                </div>
                <div class="form-group full">
                    <label class="form-label">Supporting Document (PDF only) *</label>
                    <asp:FileUpload ID="fuVerificationDoc" runat="server" CssClass="form-input" style="padding:8px;" />
                </div>
            </div>
            <div class="btn-row">
                <asp:Button ID="btnSendVerification" runat="server" Text="Send Verification Request"
                    CssClass="btn-primary" ValidationGroup="verifyGroup" OnClick="btnSendVerification_Click" />
                <asp:Label ID="lblVerificationMsg" runat="server" />
            </div>

            <!-- Request History -->
            <asp:Repeater ID="rptVerificationHistory" runat="server">
                <ItemTemplate>
                    <div class="verification-item">
                        <strong>Requested Role:</strong> <%# Eval("requestedrole") %><br />
                        <strong>Status:</strong>
                        <span class='<%# Eval("status").ToString().ToLower() %>'><%# Eval("status") %></span><br />
                        <strong>Date:</strong> <%# Eval("requesttime", "{0:dd MMM yyyy}") %>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </div>

    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
    <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

</form>

<script>
    function previewImage(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById("imgPreview").src = e.target.result;
            };
            reader.readAsDataURL(input.files[0]);
        }
    }
</script>
</body>
</html>
