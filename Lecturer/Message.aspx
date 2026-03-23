<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Message.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.Message" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Messaging - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --surface-hover: #f8fafd;
            --primary: #2563eb;
            --primary-light: #3b82f6;
            --primary-bg: rgba(37,99,235,0.08);
            --primary-border: rgba(37,99,235,0.18);
            --accent-orange: #f59e0b;
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
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
            color: var(--text);
            min-height: 100vh;
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
            padding: 4px 12px; border-radius: 20px; letter-spacing: 0.3px;
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
            color: var(--text-muted); text-decoration: none;
            padding: 14px 18px; font-size: 0.85rem; font-weight: 600;
            display: flex; align-items: center; gap: 8px;
            border-bottom: 2.5px solid transparent; transition: all 0.2s; position: relative;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active { color: var(--primary); border-bottom-color: var(--primary); }
        .nav-badge {
            background: #ef4444; color: white;
            font-size: 0.65rem; font-weight: 700;
            padding: 1px 6px; border-radius: 10px;
            font-family: 'Space Mono', monospace;
            min-width: 18px; text-align: center; display: inline-block;
        }

        /* ═══ CONTAINER ═══ */
        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }

        /* ═══ PAGE BANNER ═══ */
        .page-header {
            background: linear-gradient(135deg, #059669 0%, #10b981 55%, #34d399 100%);
            border-radius: var(--radius); padding: 28px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(16,185,129,0.28);
            animation: slideDown 0.5s ease both;
        }
        .page-header::before {
            content: ''; position: absolute; top: -40%; right: -10%;
            width: 300px; height: 300px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%);
            border-radius: 50%; pointer-events: none;
        }
        .page-header-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: rgba(255,255,255,0.75);
            margin-bottom: 6px; font-family: 'Space Mono', monospace;
        }
        .page-header-title { font-size: 1.5rem; font-weight: 700; color: white; margin-bottom: 4px; }
        .page-header-sub { color: rgba(255,255,255,0.75); font-size: 0.85rem; }

        /* ═══ CHAT LAYOUT ═══ */
        .chat-layout {
            display: grid; grid-template-columns: 300px 1fr; gap: 20px;
            height: 620px; animation: slideUp 0.5s 0.15s ease both;
        }

        /* ═══ LEFT PANEL ═══ */
        .lecturers-panel {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); overflow: hidden;
            display: flex; flex-direction: column; box-shadow: var(--shadow-sm);
        }
        .panel-header {
            padding: 16px 18px; border-bottom: 1px solid var(--border);
            font-size: 0.85rem; font-weight: 700; color: var(--text);
            display: flex; align-items: center; gap: 8px;
        }
        .panel-header-dot {
            width: 7px; height: 7px; border-radius: 50%; background: var(--accent-green);
        }

        /* search */
        .search-section {
            padding: 12px 14px; border-bottom: 1px solid var(--border);
            display: flex; gap: 8px;
        }
        .search-box {
            flex: 1; padding: 8px 12px; border: 1px solid var(--border);
            border-radius: var(--radius-sm); font-family: 'DM Sans', sans-serif;
            font-size: 0.82rem; outline: none; transition: border-color 0.2s;
        }
        .search-box:focus { border-color: var(--primary); }
        .search-btn {
            background: var(--primary); color: white; border: none;
            padding: 8px 14px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.8rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s; white-space: nowrap;
        }
        .search-btn:hover { background: #1d4ed8; }

        .panel-list { flex: 1; overflow-y: auto; }

        /* user card (search results) */
        .user-card {
            display: flex; align-items: center; justify-content: space-between;
            padding: 10px 14px; border-bottom: 1px solid var(--border-light);
            transition: background 0.15s;
        }
        .user-card:hover { background: var(--surface-hover); }
        .user-left { display: flex; align-items: center; gap: 10px; }
        .user-avatar-img {
            width: 36px; height: 36px; border-radius: 50%; object-fit: cover;
            border: 2px solid var(--border);
        }
        .user-text .user-name-text { font-size: 0.83rem; font-weight: 600; color: var(--text); }
        .user-text .user-email-text { font-size: 0.72rem; color: var(--text-muted); }
        .start-chat-btn {
            background: var(--primary-bg); color: var(--primary);
            border: 1px solid var(--primary-border); padding: 5px 12px;
            border-radius: 8px; font-family: 'DM Sans', sans-serif;
            font-size: 0.75rem; font-weight: 600; cursor: pointer; transition: all 0.2s;
            white-space: nowrap;
        }
        .start-chat-btn:hover { background: var(--primary); color: white; }

        /* conversation items */
        .conversation-item {
            display: flex; align-items: center; gap: 12px;
            padding: 12px 14px; border-bottom: 1px solid var(--border-light);
            transition: background 0.15s; cursor: pointer;
        }
        .conversation-item:hover { background: var(--surface-hover); }
        .conversation-avatar {
            width: 38px; height: 38px; border-radius: 50%; object-fit: cover;
            border: 2px solid var(--border); flex-shrink: 0;
        }
        .conversation-link {
            font-size: 0.85rem; font-weight: 600; color: var(--text);
            text-decoration: none; flex: 1;
        }
        .conversation-link:hover { color: var(--primary); }
        .avatar-link { display: flex; text-decoration: none; }

        /* ═══ RIGHT: CHAT PANEL ═══ */
        .chat-panel {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); overflow: hidden;
            display: flex; flex-direction: column; box-shadow: var(--shadow-sm);
        }
        .chat-header {
            padding: 16px 20px; border-bottom: 1px solid var(--border);
            font-size: 0.9rem; font-weight: 700; color: var(--text);
            display: flex; align-items: center; gap: 10px; min-height: 56px;
        }
        .chat-messages {
            flex: 1; overflow-y: auto; padding: 20px;
            display: flex; flex-direction: column; gap: 12px;
            background: var(--border-light);
        }

        /* message bubbles */
        .message-row { display: flex; align-items: flex-end; gap: 8px; }
        .message-row.mine { flex-direction: row-reverse; }
        .message-row.other { flex-direction: row; }

        .message-avatar { width: 28px; height: 28px; border-radius: 50%; object-fit: cover; flex-shrink: 0; }

        .message {
            max-width: 65%; padding: 10px 14px; border-radius: 14px;
            font-size: 0.87rem; line-height: 1.5; position: relative;
        }
        .message.sent {
            background: var(--primary); color: white; border-bottom-right-radius: 4px;
        }
        .message.received {
            background: var(--surface); color: var(--text);
            border: 1px solid var(--border); border-bottom-left-radius: 4px;
        }
        .message-time {
            font-size: 0.68rem; margin-top: 5px; font-family: 'Space Mono', monospace;
            opacity: 0.7;
        }
        .message.sent .message-time { text-align: right; }
        .message.received .message-time { text-align: left; }

        /* placeholder */
        .chat-placeholder {
            flex: 1; display: flex; align-items: center; justify-content: center;
            flex-direction: column; gap: 10px; color: var(--text-muted);
            background: var(--border-light);
        }
        .chat-placeholder-icon { font-size: 2.5rem; opacity: 0.4; }
        .chat-placeholder p { font-size: 0.88rem; }

        /* input row */
        .chat-input {
            padding: 14px 16px; border-top: 1px solid var(--border);
            display: flex; gap: 10px; align-items: flex-end;
        }
        .message-box {
            flex: 1; padding: 10px 14px; border: 1px solid var(--border);
            border-radius: var(--radius-sm); font-family: 'DM Sans', sans-serif;
            font-size: 0.88rem; outline: none; resize: none; max-height: 100px; line-height: 1.5;
        }
        .message-box:focus { border-color: var(--primary); }
        .send-btn {
            background: var(--primary); color: white; border: none;
            padding: 10px 22px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s; white-space: nowrap;
        }
        .send-btn:hover { background: #1d4ed8; }

        /* ═══ PROFILE POPUP ═══ */
        .profile-popup {
            position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%);
            z-index: 200; width: 300px;
        }
        .profile-card-inner {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 28px 24px; text-align: center;
            box-shadow: var(--shadow-md);
            display: flex; flex-direction: column; align-items: center; gap: 8px;
        }
        .profile-img-wrapper { position: relative; display: inline-block; margin-bottom: 6px; }
        .popup-avatar { width: 72px; height: 72px; border-radius: 50%; object-fit: cover; border: 3px solid var(--border); }
        .verify-badge {
            position: absolute; bottom: 0; right: 0;
            background: var(--accent-green); color: white;
            width: 22px; height: 22px; border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            font-size: 0.65rem; border: 2px solid white;
        }
        .popup-name { font-size: 1rem; font-weight: 700; color: var(--text); }
        .popup-role { font-size: 0.75rem; color: var(--primary); font-weight: 600; }
        .popup-email { font-size: 0.78rem; color: var(--text-muted); }
        .popup-status { font-size: 0.75rem; color: var(--accent-green); font-weight: 600; }
        .popup-desc { font-size: 0.8rem; color: var(--text-secondary); line-height: 1.5; }

        /* ═══ ANIMATIONS ═══ */
        @keyframes slideDown { from { opacity: 0; transform: translateY(-12px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }

        /* ═══ RESPONSIVE ═══ */
        @media (max-width: 768px) {
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
            .chat-layout { grid-template-columns: 1fr; height: auto; }
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
        <a href="EditProfile.aspx"><span>👤</span> Edit Profile</a>
        <a href="Forums.aspx"><span>💬</span> Forums</a>
        <a href="Message.aspx" class="active">
            <span>✉️</span> Messaging
            <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                <span class="nav-badge"><%= Session["unreadCount"] %></span>
            <% } %>
        </a>
    </div>

    <!-- ═══ CONTENT ═══ -->
    <div class="container">

        <!-- Page Banner -->
        <div class="page-header">
            <div class="page-header-label">Communication</div>
            <div class="page-header-title">Messaging</div>
            <div class="page-header-sub">Search for users or continue a conversation.</div>
        </div>

        <!-- Chat Layout -->
        <div class="chat-layout">

            <!-- LEFT: Conversations + Search -->
            <div class="lecturers-panel">

                <div class="panel-header">
                    <span class="panel-header-dot"></span>
                    Conversations
                </div>

                <!-- Search -->
                <div class="search-section">
                    <asp:TextBox ID="txtSearchUser" runat="server" CssClass="search-box" placeholder="Search users..." />
                    <asp:Button ID="btnSearchUser" runat="server" Text="Search" CssClass="search-btn" OnClick="btnSearchUser_Click" />
                </div>

                <div class="panel-list">

                    <!-- Search Results -->
                    <asp:Repeater ID="rptSearchResults" runat="server" OnItemCommand="rptSearchResults_ItemCommand">
                        <ItemTemplate>
                            <div class="user-card">
                                <div class="user-left">
                                    <asp:LinkButton runat="server" CommandName="ViewProfile"
                                        CommandArgument='<%# Eval("userid") %>'
                                        CssClass="avatar-link" CausesValidation="false">
                                        <img src='<%# Eval("ProfileImage") != DBNull.Value
                                            ? ResolveUrl(Eval("ProfileImage").ToString())
                                            : ResolveUrl("~/images/default-user.png") %>'
                                            class="user-avatar-img" />
                                    </asp:LinkButton>
                                    <div class="user-text">
                                        <div class="user-name-text"><%# Server.HtmlEncode(Eval("FullName").ToString()) %></div>
                                        <div class="user-email-text"><%# Server.HtmlEncode(Eval("email").ToString()) %></div>
                                    </div>
                                </div>
                                <asp:Button ID="btnStartChat" runat="server" Text="Chat"
                                    CssClass="start-chat-btn" CommandName="StartChat"
                                    CommandArgument='<%# Eval("userid") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <!-- Conversations -->
                    <asp:Repeater ID="rptConversations" runat="server" OnItemCommand="rptConversations_ItemCommand">
                        <ItemTemplate>
                            <div class="conversation-item">
                                <asp:LinkButton runat="server" CommandName="ViewProfile"
                                    CommandArgument='<%# Eval("userid") %>'
                                    CssClass="avatar-link" CausesValidation="false">
                                    <img src='<%# Eval("ProfileImage") != DBNull.Value
                                        ? ResolveUrl(Eval("ProfileImage").ToString())
                                        : ResolveUrl("~/images/default-user.png") %>'
                                        class="conversation-avatar" />
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnOpenChat" runat="server"
                                    CommandName="OpenChat"
                                    CommandArgument='<%# Eval("conversationid") %>'
                                    CssClass="conversation-link">
                                    <%# Server.HtmlEncode(Eval("DisplayName").ToString()) %>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>

            <!-- RIGHT: Chat Area -->
            <div class="chat-panel">

                <div class="chat-header">
                    <asp:Label ID="lblChatTitle" runat="server" Text="Select a conversation" />
                </div>

                <div class="chat-messages" id="chatMessages">
                    <asp:Repeater ID="rptMessages" runat="server">
                        <ItemTemplate>
                            <div class="message-row <%# Convert.ToInt32(Eval("IsMine")) == 1 ? "mine" : "other" %>">
                                <asp:LinkButton runat="server"
                                    CommandName="ViewProfile"
                                    CommandArgument='<%# Eval("SenderID") %>'
                                    CssClass="avatar-link" CausesValidation="false"
                                    Visible='<%# Convert.ToInt32(Eval("IsMine")) == 0 %>'>
                                    <img src='<%# Eval("ProfileImage") != DBNull.Value
                                        ? ResolveUrl(Eval("ProfileImage").ToString())
                                        : ResolveUrl("~/images/default-user.png") %>'
                                        class="message-avatar" />
                                </asp:LinkButton>
                                <div class='<%# Convert.ToInt32(Eval("IsMine")) == 1 ? "message sent" : "message received" %>'>
                                    <%# Server.HtmlEncode(Eval("content").ToString()) %>
                                    <div class="message-time"><%# Eval("creationtime", "{0:hh:mm tt}") %></div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="chat-input">
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="message-box"
                        TextMode="MultiLine" Rows="2" MaxLength="1000" />
                    <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="send-btn" OnClick="btnSend_Click" />
                </div>

            </div>

        </div>
    </div>

    <!-- Profile Popup (unchanged functionality) -->
    <div class="profile-popup" id="profilePopup" runat="server" style="display:none;">
        <div class="profile-card-inner">
            <div class="profile-img-wrapper">
                <asp:Image ID="imgProfileCard" runat="server" CssClass="popup-avatar" />
                <asp:Label ID="lblVerifyBadge" runat="server" CssClass="verify-badge">✔</asp:Label>
            </div>
            <asp:Label ID="lblProfileName" runat="server" CssClass="popup-name" />
            <asp:Label ID="lblProfileRole" runat="server" CssClass="popup-role" />
            <asp:Label ID="lblProfileEmail" runat="server" CssClass="popup-email" />
            <asp:Label ID="lblProfileStatus" runat="server" CssClass="popup-status" />
            <asp:Label ID="lblProfileDesc" runat="server" CssClass="popup-desc" />
        </div>
    </div>

    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
    <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

</form>

<script>
    // Scroll to bottom on load
    window.onload = function () {
        var chat = document.getElementById('chatMessages');
        if (chat) chat.scrollTop = chat.scrollHeight;
    };

    // Close popup on outside click
    document.addEventListener("click", function (e) {
        var popup = document.getElementById("<%= profilePopup.ClientID %>");
        var card = document.querySelector(".profile-card-inner");
        if (!popup) return;
        if (card && !card.contains(e.target) && !e.target.closest(".avatar-link")) {
            popup.style.display = "none";
        }
    });

    // ESC to close popup
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape") {
            var popup = document.getElementById("<%= profilePopup.ClientID %>");
            if (popup) popup.style.display = "none";
        }
    });
</script>
</body>
</html>
