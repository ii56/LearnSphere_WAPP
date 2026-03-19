<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Database.aspx.cs" Inherits="LearnSphere_WAPP.Admin.Database" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Database</title>
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
                    <a href="Database.aspx" class="nav-item active">Database</a>
                    <a href="AdminForums.aspx" class="nav-item">Forums</a>
                    <a href="AdminEditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="AdminSyslog.aspx" class="nav-item">Syslog</a>
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

            <div class="main-content">
                <div class="dashboard-header">
                    <h2>Database</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>


            </div>

        </div>
    </form>
</body>
</html>
