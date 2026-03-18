<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Dashboard</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="AdminDashboard.aspx" class="nav-item active">Dashboard</a>
                    <a href="UserManagement.aspx" class="nav-item">User Management</a>
                    <a href="CourseManagement.aspx" class="nav-item">Course Management</a>
                    <a href="Database.aspx" class="nav-item">Database</a>
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
                    <h2>Dashboard Overview</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>
                
                <h2>System Overview</h2>
                <div class="stats-grid">

                    <div class="stat-card blue">
                        <div class="stat-title">Total Users</div>
                        <asp:Label ID="lblTotalUsers" runat="server" CssClass="stat-number" />
                    </div>

                    <div class="stat-card purple">
                        <div class="stat-title">Total Students</div>
                        <asp:Label ID="lblTotalStudents" runat="server" CssClass="stat-number" />
                    </div>

                    <div class="stat-card pink">
                        <div class="stat-title">Total Lecturers</div>
                        <asp:Label ID="lblTotalLecturers" runat="server" CssClass="stat-number" />
                    </div>

                    <div class="stat-card orange">
                        <div class="stat-title">Total Courses</div>
                        <asp:Label ID="lblTotalCourses" runat="server" CssClass="stat-number" />
                    </div>

                    <div class="stat-card green">
                        <div class="stat-title">Total Forums</div>
                        <asp:Label ID="lblTotalForums" runat="server" CssClass="stat-number" />
                    </div>

                </div>

                <h2>Pending Request</h2>
                <div class="stats-grid">
                    <div class="stat-card blue">
                        <div class="stat-title">Lecturers Validation</div>
                        <asp:Label ID="lecturersVal" runat="server" CssClass="stat-number" />
                    </div>
                    <div class="stat-card purple">
                        <div class="stat-title">Students Validation</div>
                        <asp:Label ID="studentsVal" runat="server" CssClass="stat-number" />
                    </div>
                    <div class="stat-card pink">
                        <div class="stat-title">Courses Validation</div>
                        <asp:Label ID="coursesVal" runat="server" CssClass="stat-number" />
                    </div>
                </div>

                <div class="quick-actions-section">
                    <h3>Quick Actions</h3>
                    <div class="quick-card-grid">
                        <a href="UserManagement.aspx" class="quick-card">
                            <div class="quick-card-title">Manage User</div>
                            <div class="quick-card-desc">create/delete/modify users account.</div>
                        </a>

                        <a href="CourseManagement.aspx" class="quick-card">
                            <div class="quick-card-title">Manage Course</div>
                            <div class="quick-card-desc">create/delete/modify courses.</div>
                        </a>

                        <a href="Database.aspx" class="quick-card">
                            <div class="quick-card-title">Database</div>
                            <div class="quick-card-desc">manage database</div>
                        </a>

                        <a href="AdminEditProfile.aspx" class="quick-card">
                            <div class="quick-card-title">Edit Profile</div>
                            <div class="quick-card-desc">edit the information of account</div>
                        </a>

                        <a href="AdminSyslog.aspx" class="quick-card">
                            <div class="quick-card-title">System Log</div>
                            <div class="quick-card-desc">view system log of user action</div>
                        </a>

                        <a href="../Chatbot/AdminChatbotKnowledge.aspx" class="quick-card">
                            <div class="quick-card-title">Manage Chatbot</div>
                            <div class="quick-card-desc">manage chatbot rule and knowledge base</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
