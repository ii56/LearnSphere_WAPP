<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminChangePassword.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Password</title>
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
                <a href="AdminEditProfile.aspx" class="active">Edit Profile</a>
                <a href="AdminSyslog.aspx" >Syslog</a>
                <a href="AdminMessage.aspx">
                    <span>✉️</span> Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="nav-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                </a>
                <a href="../Chatbot/AdminChatbotKnowledge.aspx" >Chatbot</a>
            </div>

            <div class="container">
                <div class="welcome-banner">
                        <h2 class="welcome-label">Admin Portal</h2>
                    <h2 class="welcome-name">Change Password</h2>
                    <h3 class="welcome-sub">You can change your password here if you forget it</h3>
                </div>

            <div class="main-content">
                <div class="dashboard-header">
                    <h2>Edit Password</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>

                <div class="profile-edit">
                    <div class="profile-card">
                        <div class="form-group">
                            <asp:Label ID="Label2" runat="server" Text="New Password:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-input"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label3" runat="server" Text="Confirm New Password:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtNewPassword2" runat="server" CssClass="form-input"></asp:TextBox>
                        </div>

                        <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="save-btn" OnClick="btnSave_Click" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
