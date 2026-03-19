<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminMessage.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminMessage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Message</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="AdminDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="UserManagement.aspx" class="nav-item">User Management</a>
                    <a href="CourseManagement.aspx" class="nav-item">Course Management</a>
                    <a href="Database.aspx" class="nav-item">Database</a>
                    <a href="AdminForums.aspx" class="nav-item">Forums</a>
                    <a href="AdminEditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="AdminSyslog.aspx" class="nav-item active">Syslog</a>
                </div>

                <div class="sidebar-profile">
                    <div class="profile-box admin">
                        <div class="profile-img-wrapper">
                            <img id="sidebarImg" runat="server" class="profile-img" />
                            <div class="verification-badge">✔</div>
                        </div>

                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] %></div>
                            <div class="profile-status">
                                administrator
                            </div>
                        </div>
                    </div>

                    <a href="AdminMessage.aspx" class="nav-item message-link">
                        Messaging
                        <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                            <span class="message-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

                <!-- MAIN -->
            <div class="main-content">

                <div class="chat-container">

                    <!-- LEFT -->
                    <div class="chat-sidebar">

                        <!-- SEARCH -->
                        <asp:Panel runat="server" DefaultButton="btnSearchUser" CssClass="search-section">

                            <asp:TextBox ID="txtSearchUser"
                                runat="server"
                                CssClass="search-box"
                                placeholder="Search users..." />

                            <asp:Button ID="btnSearchUser"
                                runat="server"
                                Text="Search"
                                CssClass="search-btn"
                                OnClick="btnSearchUser_Click" />

                        </asp:Panel>

                        <!-- SEARCH RESULTS -->
                        <asp:Repeater ID="rptSearchResults" runat="server"
                            OnItemCommand="rptSearchResults_ItemCommand">
                            <ItemTemplate>

                                <div class="user-card">

                                    <div class="user-left">

                                        <asp:LinkButton runat="server"
                                            CommandName="ViewProfile"
                                            CommandArgument='<%# Eval("userid") %>'
                                            CssClass="avatar-link"
                                            CausesValidation="false">

                                            <img src='<%# Eval("ProfileImage") != DBNull.Value 
                                                ? ResolveUrl(Eval("ProfileImage").ToString()) 
                                                : ResolveUrl("~/images/default-user.png") %>' 
                                                class="user-avatar" />

                                        </asp:LinkButton>

                                        <div class="user-text">
                                            <div class="user-name">
                                                <%# Server.HtmlEncode(Eval("FullName").ToString()) %>
                                            </div>
                                            <div class="user-email">
                                                <%# Server.HtmlEncode(Eval("email").ToString()) %>
                                            </div>
                                        </div>

                                    </div>

                                    <asp:Button ID="btnStartChat"
                                        runat="server"
                                        Text="Start Chat"
                                        CssClass="start-chat-btn"
                                        CommandName="StartChat"
                                        CommandArgument='<%# Eval("userid") %>' />

                                </div>

                            </ItemTemplate>
                        </asp:Repeater>

                        <!-- CONVERSATIONS -->
                        <asp:Repeater ID="rptConversations" runat="server"
                            OnItemCommand="rptConversations_ItemCommand">
                            <ItemTemplate>

                                <div class="conversation-item">

                                    <asp:LinkButton runat="server"
                                        CommandName="ViewProfile"
                                        CommandArgument='<%# Eval("userid") %>'
                                        CssClass="avatar-link"
                                        CausesValidation="false">

                                        <img src='<%# Eval("ProfileImage") != DBNull.Value 
                                            ? ResolveUrl(Eval("ProfileImage").ToString()) 
                                            : ResolveUrl("~/images/default-user.png") %>' 
                                            class="conversation-avatar" />

                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnOpenChat"
                                        runat="server"
                                        CommandName="OpenChat"
                                        CommandArgument='<%# Eval("conversationid") %>'
                                        CssClass="conversation-link-full">

                                        <%# Server.HtmlEncode(Eval("DisplayName").ToString()) %>

                                    </asp:LinkButton>

                                </div>

                            </ItemTemplate>
                        </asp:Repeater>

                    </div>

                    <!-- CHAT -->
                    <div class="chat-main">

                        <div class="chat-header">
                            <asp:Label ID="lblChatTitle"
                                runat="server"
                                Text="Select a conversation" />
                        </div>

                        <!-- MESSAGES -->
                        <div class="chat-messages">
                            <asp:Repeater ID="rptMessages" runat="server">
                                <ItemTemplate>

                                    <div class="message-row <%# Convert.ToInt32(Eval("IsMine")) == 1 ? "mine" : "other" %>">

                                        <asp:LinkButton runat="server"
                                            CommandName="ViewProfile"
                                            CommandArgument='<%# Eval("SenderID") %>'
                                            CssClass="avatar-link"
                                            CausesValidation="false"
                                            Visible='<%# Convert.ToInt32(Eval("IsMine")) == 0 %>'>

                                            <img src='<%# Eval("ProfileImage") != DBNull.Value 
                                                ? ResolveUrl(Eval("ProfileImage").ToString()) 
                                                : ResolveUrl("~/images/default-user.png") %>' 
                                                class="message-avatar" />

                                        </asp:LinkButton>

                                        <div class='<%# Convert.ToInt32(Eval("IsMine")) == 1 ? "message sent" : "message received" %>'>

                                            <%# Server.HtmlEncode(Eval("content").ToString()) %>

                                            <div class="message-time">
                                                <%# Eval("creationtime", "{0:hh:mm tt}") %>
                                            </div>

                                        </div>

                                    </div>

                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- INPUT -->
                        <div class="chat-input">

                            <asp:TextBox ID="txtMessage"
                                runat="server"
                                CssClass="message-box"
                                TextMode="MultiLine"
                                Rows="2"
                                MaxLength="1000" />

                            <asp:Button ID="btnSend"
                                runat="server"
                                Text="Send"
                                CssClass="send-btn"
                                OnClick="btnSend_Click" />

                        </div>

                    </div>

                    <!-- PROFILE POPUP -->
                    <div class="profile-popup" id="profilePopup" runat="server" style="display:none;">

                        <div class="profile-card-inner">

                            <div class="profile-img-wrapper">
                                <asp:Image ID="imgProfileCard" runat="server" CssClass="popup-avatar"/>
                                <asp:Label ID="lblVerifyBadge" runat="server" CssClass="verify-badge">✔</asp:Label>
                            </div>

                            <asp:Label ID="lblProfileName" runat="server" CssClass="popup-name"/>
                            <asp:Label ID="lblProfileRole" runat="server" CssClass="popup-role"/>
                            <asp:Label ID="lblProfileEmail" runat="server" CssClass="popup-email"/>
                            <asp:Label ID="lblProfileStatus" runat="server" CssClass="popup-status"/>
                            <asp:Label ID="lblProfileDesc" runat="server" CssClass="popup-desc"/>

                        </div>

                    </div>

                </div>

            </div>

        </div>
    </form>

    <script>
        // CLOSE POPUP WHEN CLICK OUTSIDE
        document.addEventListener("click", function (e) {
            var popup = document.getElementById("<%= profilePopup.ClientID %>");
            var card = document.querySelector(".profile-card-inner");

            if (!popup) return;

            if (card && !card.contains(e.target) && !e.target.closest(".avatar-link")) {
                popup.style.display = "none";
            }
        });

        // ESC TO CLOSE
        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape") {
                var popup = document.getElementById("<%= profilePopup.ClientID %>");
                if (popup) popup.style.display = "none";
            }
        });
    </script>
</body>
</html>
