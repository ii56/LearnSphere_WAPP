<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminMessage.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminMessage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Message</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <span class="verified-badge">Administrator</span>

                <div class="user-pill">
                        <div class="user-avatar">
                            <img id="sidebarImg" runat="server" />
                        </div>
                        <span class="user-name"><%= Session["uname"] %></span>
                    </div>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="nav">
                <a href="AdminDashboard.aspx" >Dashboard</a>
                <a href="UserManagement.aspx" >User Management</a>
                <a href="CourseManagement.aspx" >Course Management</a>
                <a href="Database.aspx" >Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
                <a href="AdminSyslog.aspx" >Syslog</a>
                <a href="AdminMessage.aspx" class="active">
                    <span>✉️</span> Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="nav-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                </a>
                <a href="../Chatbot/AdminChatbotKnowledge.aspx" >Chatbot</a>
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
                            <asp:TextBox ID="txtSearchUser" runat="server" CssClass="search-box" placeholder="Search users..." AutoPostBack="True" OnTextChanged="txtSearchUser_TextChanged" />
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
    </form>

    <script>
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
