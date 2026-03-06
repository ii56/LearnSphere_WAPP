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

                    <a href="Message.aspx" class="nav-item message-link">
                        Messaging
                        <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                            <span class="message-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">
                 <div class="chat-container">

                     <div class="chat-sidebar">

                         <div class="search-section">
                             <asp:TextBox ID="txtSearchUser" runat="server" CssClass="message-search-box" placeholder="Search users by name..." AutoPostBack="True" OnTextChanged="txtSearchUser_TextChanged"></asp:TextBox>

                             <asp:Button ID="btnSearchUser" runat="server" Text="Search" CssClass="search-btn" OnClick="btnSearchUser_Click" />
                         </div>

                         <asp:Repeater ID="rptSearchResults" runat="server"
                             OnItemCommand="rptSearchResults_ItemCommand">
                             <ItemTemplate>
                                 <div class="user-card">

                                     <div class="user-left">
                                         <img src='<%# Eval("ProfileImage") != DBNull.Value 
                                             ? ResolveUrl(Eval("ProfileImage").ToString()) 
                                             : ResolveUrl("~/images/default-user.png") %>' 
                                             class="user-avatar" />

                                         <div class="user-text">
                                             <div class="user-name">
                                                 <%# Eval("FullName") %>
                                             </div>
                                             <div class="user-email">
                                                 <%# Eval("email") %>
                                             </div>
                                         </div>
                                     </div>

                                     <asp:Button ID="btnStartChat" runat="server" Text="Start Chat" CssClass="start-chat-btn" CommandName="StartChat" CommandArgument='<%# Eval("userid") %>' />
                                 </div>
                             </ItemTemplate>
                         </asp:Repeater>

                         <asp:Repeater ID="rptConversations" runat="server"
                             OnItemCommand="rptConversations_ItemCommand">
                             <ItemTemplate>
                                 <div class="conversation-item">

                                     <img src='<%# Eval("ProfileImage") != DBNull.Value 
                                         ? ResolveUrl(Eval("ProfileImage").ToString()) 
                                         : ResolveUrl("~/images/default-user.png") %>' 
                                         class="conversation-avatar" />

                                     <asp:LinkButton
                                         ID="btnOpenChat"
                                         runat="server"
                                         CommandName="OpenChat"
                                         CommandArgument='<%# Eval("conversationid") %>'
                                         CssClass="conversation-link">
                                         <%# Eval("DisplayName") %>
                                     </asp:LinkButton>
                                 </div>
                             </ItemTemplate>
                         </asp:Repeater>

                     </div>

                     <div class="chat-main">

                         <div class="chat-header">
                             <asp:Label ID="lblChatTitle" runat="server"
                                 Text="Select a conversation"></asp:Label>
                         </div>

                         <div class="chat-messages">
                             <asp:Repeater ID="rptMessages" runat="server">
                                 <ItemTemplate>
                                     <div class="message-row <%# Convert.ToInt32(Eval("IsMine")) == 1 ? "mine" : "other" %>">

                                         <%# Convert.ToInt32(Eval("IsMine")) == 0 ? 
                                         "<img src='" + 
                                             (Eval("ProfileImage") != DBNull.Value 
                                                 ? ResolveUrl(Eval("ProfileImage").ToString()) 
                                                 : ResolveUrl("~/images/default-user.png")) 
                                         + "' class=\"message-avatar\" />" : "" %>

                                         <div class='<%# Convert.ToInt32(Eval("IsMine")) == 1 ? "message sent" : "message received" %>'>
                                             <%# Eval("content") %>
                                             <div class="message-time">
                                                 <%# Eval("creationtime", "{0:hh:mm tt}") %>
                                             </div>
                                         </div>

                                     </div>
                                 </ItemTemplate>
                             </asp:Repeater>
                         </div>

                         <div class="chat-input">
                             <asp:TextBox ID="txtMessage" runat="server" CssClass="message-box" TextMode="MultiLine" Rows="2"></asp:TextBox>

                             <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="send-btn" OnClick="btnSend_Click" />
                         </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
