<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Messaging.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.Messaging" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Messages - LearnSphere</title>
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
            --accent-purple: #8b5cf6;
            --accent-green: #10b981;
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
            backdrop-filter: blur(20px);
            -webkit-backdrop-filter: blur(20px);
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
            overflow: hidden; position: relative;
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
            color: var(--text-muted); text-decoration: none;
            padding: 14px 18px; font-size: 0.85rem; font-weight: 600;
            display: flex; align-items: center; gap: 8px;
            border-bottom: 2.5px solid transparent; transition: all 0.2s;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active { color: var(--primary); border-bottom-color: var(--primary); }

        /* ═══ CONTAINER ═══ */
        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }

        .page-header {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 50%, #60a5fa 100%);
            border-radius: var(--radius); padding: 28px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(37,99,235,0.25);
            animation: slideDown 0.5s ease both;
        }
        .page-header::before {
            content: ''; position: absolute; top: -40%; right: -10%;
            width: 300px; height: 300px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%);
            border-radius: 50%;
        }
        .page-header-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: rgba(255,255,255,0.7);
            margin-bottom: 6px; font-family: 'Space Mono', monospace;
        }
        .page-header-title { font-size: 1.5rem; font-weight: 700; color: white; margin-bottom: 4px; }
        .page-header-sub { color: rgba(255,255,255,0.75); font-size: 0.85rem; }

        /* ═══ TWO COLUMN CHAT LAYOUT ═══ */
        .chat-layout { display: grid; grid-template-columns: 300px 1fr; gap: 20px; height: 600px; }

        /* LEFT PANEL: LECTURERS */
        .lecturers-panel {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); overflow: hidden;
            display: flex; flex-direction: column;
            box-shadow: var(--shadow-sm);
        }
        .panel-header {
            padding: 16px 18px; border-bottom: 1px solid var(--border);
            font-size: 0.85rem; font-weight: 700; color: var(--text);
        }
        .lecturer-list { flex: 1; overflow-y: auto; }
        .lecturer-item {
            display: flex; align-items: center; gap: 12px;
            padding: 14px 18px; cursor: pointer; text-decoration: none;
            border-bottom: 1px solid var(--border-light);
            transition: background 0.15s;
        }
        .lecturer-item:hover { background: var(--surface-hover); }
        .lecturer-item.selected { background: var(--primary-bg); border-left: 3px solid var(--primary); }
        
        .lecturer-avatar {
            width: 38px; height: 38px; flex-shrink: 0;
            background: linear-gradient(135deg, var(--primary), var(--accent-purple));
            border-radius: 50%; display: flex; align-items: center;
            justify-content: center; font-size: 0.85rem; font-weight: 700; color: white;
            overflow: hidden; position: relative;
        }
        .lecturer-avatar img { width: 100%; height: 100%; object-fit: cover; position: absolute; top: 0; left: 0; }
        
        .lecturer-info { flex: 1; min-width: 0; }
        .lecturer-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }
        .lecturer-course { font-size: 0.75rem; color: var(--text-muted); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }

        /* RIGHT PANEL: CHAT */
        .chat-panel {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); overflow: hidden;
            display: flex; flex-direction: column;
            box-shadow: var(--shadow-sm);
        }
        .chat-header {
            padding: 16px 20px; border-bottom: 1px solid var(--border);
            display: flex; align-items: center; gap: 12px;
        }
        .chat-header-avatar {
            width: 36px; height: 36px;
            background: linear-gradient(135deg, var(--primary), var(--accent-purple));
            border-radius: 50%; display: flex; align-items: center;
            justify-content: center; font-size: 0.82rem; font-weight: 700; color: white;
            overflow: hidden; position: relative;
        }
        .chat-header-avatar img { width: 100%; height: 100%; object-fit: cover; position: absolute; top: 0; left: 0; }
        
        .chat-header-name { font-size: 0.9rem; font-weight: 700; }
        .chat-header-sub { font-size: 0.75rem; color: var(--text-muted); }

        .chat-messages {
            flex: 1; overflow-y: auto; padding: 20px;
            display: flex; flex-direction: column; gap: 12px;
            background: var(--border-light);
        }

        .msg-bubble { max-width: 70%; }
        .msg-bubble.mine { align-self: flex-end; }
        .msg-bubble.theirs { align-self: flex-start; }

        .msg-text {
            padding: 10px 14px; border-radius: 14px;
            font-size: 0.87rem; line-height: 1.5;
        }
        .msg-bubble.mine .msg-text {
            background: var(--primary); color: white;
            border-bottom-right-radius: 4px;
        }
        .msg-bubble.theirs .msg-text {
            background: var(--surface); color: var(--text);
            border: 1px solid var(--border); border-bottom-left-radius: 4px;
        }
        .msg-time {
            font-size: 0.7rem; color: var(--text-muted);
            margin-top: 4px; font-family: 'Space Mono', monospace;
            text-align: right;
        }
        .msg-bubble.theirs .msg-time { text-align: left; }

        .chat-input-row {
            padding: 14px 16px; border-top: 1px solid var(--border);
            display: flex; gap: 10px; align-items: flex-end;
        }
        .chat-input {
            flex: 1; padding: 10px 14px;
            border: 1px solid var(--border); border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.88rem;
            outline: none; resize: none; max-height: 100px; line-height: 1.5;
        }
        .chat-input:focus { border-color: var(--primary); }
        .btn-send {
            background: var(--primary); color: white; border: none;
            padding: 10px 20px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s; white-space: nowrap;
        }
        .btn-send:hover { background: #1d4ed8; }

        .chat-placeholder {
            flex: 1; display: flex; align-items: center; justify-content: center;
            flex-direction: column; gap: 10px; color: var(--text-muted);
            background: var(--border-light);
        }
        .chat-placeholder-icon { font-size: 2.5rem; opacity: 0.4; }
        .chat-placeholder p { font-size: 0.88rem; }

        .empty-state {
            text-align: center; padding: 40px 20px; color: var(--text-muted);
            font-size: 0.88rem;
        }

        .alert { padding: 12px 18px; border-radius: var(--radius-sm); font-size: 0.85rem; margin-bottom: 16px; }
        .alert-danger { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }

        @keyframes slideDown { from { opacity: 0; transform: translateY(-12px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 768px) {
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
            .chat-layout { grid-template-columns: 1fr; height: auto; }
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
                        <asp:Image ID="imgHeaderAvatar" runat="server" Visible="false" />
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
            <a href="MyCourses.aspx"><span>🎓</span> My Learning</a>
            <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx" class="active"><span>✉️</span> Messages</a>
            <a href="EditProfile.aspx"><span>👤</span> Profile</a>
        </div>

        <div class="container">

            <div class="page-header">
                <div class="page-header-label">Communication</div>
                <div class="page-header-title">Messages</div>
                <div class="page-header-sub">Chat with lecturers from your enrolled free courses.</div>
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger" Visible="false" />

            <div class="chat-layout">

                <div class="lecturers-panel">
                    <div class="panel-header">Your Lecturers</div>
                    <div class="lecturer-list">
                        <asp:Repeater ID="rptLecturers" runat="server">
                            <ItemTemplate>
                                <a href='Messaging.aspx?lecturerId=<%# Eval("userid") %>' 
                                   class='lecturer-item <%# Convert.ToInt32(Eval("userid")) == SelectedLecturerId ? "selected" : "" %>'>
                                    <div class="lecturer-avatar">
                                        <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' />
                                    </div>
                                    <div class="lecturer-info">
                                        <div class="lecturer-name"><%# Eval("fname") %> <%# Eval("lname") %></div>
                                        <div class="lecturer-course"><%# Eval("coursename") %></div>
                                    </div>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:Panel ID="pnlNoLecturers" runat="server" Visible="false">
                            <div class="empty-state">Enroll in free courses to message lecturers.</div>
                        </asp:Panel>
                    </div>
                </div>

                <div class="chat-panel">

                    <asp:Panel ID="pnlChatPlaceholder" runat="server">
                        <div class="chat-placeholder">
                            <div class="chat-placeholder-icon">✉️</div>
                            <p>Select a lecturer to start chatting.</p>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlChat" runat="server" Visible="false" style="display:flex; flex-direction:column; height:100%;">

                        <div class="chat-header">
                            <div class="chat-header-avatar">
                                <asp:Image ID="imgChatAvatar" runat="server" />
                            </div>
                            <div>
                                <div class="chat-header-name"><asp:Label ID="lblChatName" runat="server" /></div>
                                <div class="chat-header-sub"><asp:Label ID="lblChatCourse" runat="server" /></div>
                            </div>
                        </div>

                        <div class="chat-messages" id="chatMessages">
                            <asp:Repeater ID="rptMessages" runat="server">
                                <ItemTemplate>
                                    <div class="msg-bubble <%# Convert.ToInt32(Eval("senderid")) == Convert.ToInt32(Session["userid"]) ? "mine" : "theirs" %>">
                                        <div class="msg-text"><%# Server.HtmlEncode(Eval("content").ToString()).Replace("\n", "<br/>") %></div>
                                        <div class="msg-time"><%# Convert.ToDateTime(Eval("creationtime")).ToString("MMM dd, hh:mm tt") %></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <div class="chat-input-row">
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="chat-input" TextMode="MultiLine"
                                placeholder="Type a message..." Rows="1" MaxLength="1000" />
                            <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="btn-send" OnClick="btnSend_Click" />
                        </div>

                    </asp:Panel>

                </div>
            </div>

        </div>
        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>

    <script>
        // Scroll to the bottom of the chat smoothly when the page loads
        window.onload = function () {
            var chat = document.getElementById('chatMessages');
            if (chat) {
                chat.scrollTop = chat.scrollHeight;
            }
        };
    </script>
</body>
</html>