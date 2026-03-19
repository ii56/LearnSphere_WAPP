<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AskQuestion.aspx.cs" Inherits="LearnSphere_WAPP.Student.AskQuestion" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Ask a Question - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --primary: #2563eb;
            --primary-bg: rgba(37,99,235,0.08);
            --accent-purple: #8b5cf6;
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
        .container { max-width: 800px; margin: 0 auto; padding: 28px 36px; }

        .breadcrumb {
            display: flex; align-items: center; gap: 8px;
            font-size: 0.82rem; color: var(--text-muted); margin-bottom: 20px;
        }
        .breadcrumb a { color: var(--primary); text-decoration: none; }
        .breadcrumb a:hover { text-decoration: underline; }

        .form-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 32px;
            box-shadow: var(--shadow-sm); animation: slideUp 0.4s ease both;
        }
        .form-card-title {
            font-size: 1.2rem; font-weight: 700; margin-bottom: 6px;
        }
        .form-card-sub {
            font-size: 0.85rem; color: var(--text-secondary); margin-bottom: 28px;
        }

        .form-group { margin-bottom: 20px; }
        .form-label {
            display: block; font-size: 0.85rem; font-weight: 600;
            margin-bottom: 8px; color: var(--text);
        }
        .form-label span { color: #ef4444; }

        .form-input, .form-textarea {
            width: 100%; padding: 10px 14px;
            border: 1px solid var(--border); border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.88rem;
            color: var(--text); background: var(--surface); outline: none;
            transition: border-color 0.2s;
        }
        .form-input:focus, .form-textarea:focus { border-color: var(--primary); }
        .form-textarea { resize: vertical; min-height: 140px; line-height: 1.6; }

        .form-hint { font-size: 0.75rem; color: var(--text-muted); margin-top: 5px; }

        .error-msg { color: #dc2626; font-size: 0.78rem; margin-top: 4px; display: block; }

        .form-actions { display: flex; gap: 12px; margin-top: 24px; }
        .btn-submit {
            background: var(--primary); color: white; border: none;
            padding: 11px 28px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.88rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-submit:hover { background: #1d4ed8; }
        .btn-cancel {
            background: transparent; color: var(--text-secondary);
            border: 1px solid var(--border); padding: 11px 22px;
            border-radius: var(--radius-sm); font-family: 'DM Sans', sans-serif;
            font-size: 0.88rem; font-weight: 600; cursor: pointer;
            text-decoration: none; display: inline-flex; align-items: center;
        }
        .btn-cancel:hover { background: var(--border-light); }

        .alert { padding: 12px 18px; border-radius: var(--radius-sm); font-size: 0.85rem; margin-bottom: 20px; }
        .alert-danger { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
        .alert-success { background: #f0fdf4; color: #16a34a; border: 1px solid #bbf7d0; }

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
                <a href="#" id="linkBack" runat="server">Questions</a>
                <span>›</span>
                <span>Ask a Question</span>
            </div>

            <div class="form-card">
                <div class="form-card-title">Ask a Question</div>
                <div class="form-card-sub">Post your question to the forum community.</div>

                <asp:Label ID="lblMessage" runat="server" Visible="false" />

                <div class="form-group">
                    <label class="form-label">Question Title <span>*</span></label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-input" placeholder="Write a clear and specific title..." MaxLength="300" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle"
                        ErrorMessage="Title is required." CssClass="error-msg" Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label class="form-label">Details <span>*</span></label>
                    <asp:TextBox ID="txtContent" runat="server" CssClass="form-textarea" TextMode="MultiLine"
                        placeholder="Describe your question in detail..." />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtContent"
                        ErrorMessage="Details are required." CssClass="error-msg" Display="Dynamic" />
                    <div class="form-hint">Be specific — the more detail you give, the better answers you'll get.</div>
                </div>

                <div class="form-group">
                    <label class="form-label">Tags (optional)</label>
                    <asp:TextBox ID="txtTags" runat="server" CssClass="form-input" placeholder="e.g. html, css, database" MaxLength="300" />
                    <div class="form-hint">Separate tags with commas.</div>
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnSubmit" runat="server" Text="Post Question" CssClass="btn-submit" OnClick="btnSubmit_Click" />
                    <a href="#" id="linkCancel" runat="server" class="btn-cancel">Cancel</a>
                </div>
            </div>

        </div>
    </form>
</body>
</html>