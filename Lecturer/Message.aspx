<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Message.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.Message" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Messaging</title>
    <link href="Message.css" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">
    <div class="layout">
        <div class="sidebar">
            <div>
                <div class="sidebar-title">LearnSphere</div>
                <a href="LecturerDashboard.aspx" class="nav-item">Dashboard</a>
                <a href="CreateCourse.aspx" class="nav-item">Create Course</a>
                <a href="ViewCourses.aspx" class="nav-item">View Courses</a>
                <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                <a href="../Chatbot/Chatbot.aspx" class="nav-item">Chatbot</a>
                <a href="Forums.aspx" class="nav-item">Forums</a>
            </div>

            <div class="sidebar-profile">
                <div class="profile-box <%= (Session["verified"] != null && (bool)Session["verified"]) ? "verified" : "not-verified" %>">
                    <div class="profile-img-wrapper">
                       <img id="imgSidebarProfile" runat="server" class="profile-img" />
                        <% if (Session["verified"] != null && (bool)Session["verified"]) { %>
                            <div class="verification-badge">✔</div>
                        <% } %>
                    </div>

                    <div class="profile-info">
                        <div class="profile-name"><%= Session["uname"] %></div>
                        <div class="profile-status">
                            <%= (Session["verified"] != null && (bool)Session["verified"]) ? "Verified Lecturer" : "Not Verified" %>
                        </div>
                    </div>
                </div>

                <a href="Message.aspx" class="nav-item active message-link">
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
                        <asp:TextBox ID="txtSearchUser" runat="server" CssClass="search-box" placeholder="Search users by name..."></asp:TextBox>

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