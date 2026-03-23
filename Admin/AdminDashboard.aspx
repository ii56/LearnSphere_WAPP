<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Dashboard</title>
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
                <a href="AdminDashboard.aspx" class="active">Dashboard</a>
                <a href="UserManagement.aspx" >User Management</a>
                <a href="CourseManagement.aspx" >Course Management</a>
                <a href="Database.aspx" >Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
                <a href="AdminSyslog.aspx" >Syslog</a>
                <a href="AdminMessage.aspx">
                    Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="nav-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                </a>
                <a href="../Chatbot/AdminChatbotKnowledge.aspx" >Chatbot</a>
            </div>

            <div class="container">
                <div class="welcome-banner">
                    <h2 class="welcome-label">Dashboard Overview</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-name" />
                    <h3 class="welcome-sub">Here's an overview of the admin system</h3>
                </div>
                
                <h2>System Overview</h2>
                <div class="stats-grid">

                    <div class="stat-card">
                        <h3 class="stat-label">Total Users</h3>
                        <asp:Label ID="lblTotalUsers" runat="server" CssClass="stat-value" />
                    </div>

                    <div class="stat-card">
                        <h3 class="stat-label">Total Students</h3>
                        <asp:Label ID="lblTotalStudents" runat="server" CssClass="stat-value" />
                    </div>

                    <div class="stat-card">
                        <h3 class="stat-label">Total Lecturers</h3>
                        <asp:Label ID="lblTotalLecturers" runat="server" CssClass="stat-value" />
                    </div>

                    <div class="stat-card">
                        <h3 class="stat-label">Total Courses</h3>
                        <asp:Label ID="lblTotalCourses" runat="server" CssClass="stat-value" />
                    </div>

                    <div class="stat-card">
                        <h3 class="stat-label">Total Forums</h3>
                        <asp:Label ID="lblTotalForums" runat="server" CssClass="stat-value" />
                    </div>

                </div>

                <h2>Pending Request</h2>
                <div class="stats-grid">
                    <div class="stat-card">
                        <h3 class="stat-label">Lecturers Validation</h3>
                        <asp:Label ID="lecturersVal" runat="server" CssClass="stat-value" />
                    </div>
                    <div class="stat-card">
                        <h3 class="stat-label">Students Validation</h3>
                        <asp:Label ID="studentsVal" runat="server" CssClass="stat-value" />
                    </div>
                    <div class="stat-card">
                        <h3 class="stat-label">Courses Validation</h3>
                        <asp:Label ID="coursesVal" runat="server" CssClass="stat-value" />
                    </div>
                </div>

                <div class="section section-actions">
                    <div class="section-header section-title">
                        <div class="section-title">
                            <span class="section-title-dot dot-purple"></span>
                            Quick Actions
                        </div>
                    </div>
                    <div class="quick-grid">
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
