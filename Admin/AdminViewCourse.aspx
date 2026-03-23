<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminViewCourse.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminViewCourse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Course</title>
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
                <a href="CourseManagement.aspx" class="active">Course Management</a>
                <a href="Database.aspx" >Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
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
                    <h2 class="welcome-name">View Course</h2>
                    <h3 class="welcome-sub">View course details</h3>
                </div>

                <div class="card course-card">
                    <div class="course-header">
                        <h2>Course Details</h2>
                    </div>

                    <div class="course-grid">
                        <div class="field-label">Course ID</div>
                        <div class="field-value"><asp:Label ID="lblCourseId" runat="server" /></div>

                        <div class="field-label">Owner ID</div>
                        <div class="field-value"><asp:Label ID="lblOwnerId" runat="server" /></div>

                        <div class="field-label">Course Name</div>
                        <div class="field-value highlight"><asp:Label ID="lblCname" runat="server" /></div>

                        <div class="field-label">Description</div>
                        <div class="field-value description"><asp:Label ID="lblDescription" runat="server" /></div>

                        <div class="field-label">Price</div>
                        <div class="field-value price">RM <asp:Label ID="lblPrice" runat="server" /></div>

                        <div class="field-label">Creation Time</div>
                        <div class="field-value"><asp:Label ID="lblCtime" runat="server" /></div>

                        <div class="field-label">Deletion Time</div>
                        <div class="field-value"><asp:Label ID="lblDtime" runat="server" /></div>

                        <div class="field-label">Category</div>
                        <div class="field-value"><asp:Label ID="lblCategory" runat="server" /></div>

                        <div class="field-label">Status</div>
                        <div class="field-value">
                            <span class="status-badge"><asp:Label ID="lblStatus" runat="server" /></span>
                        </div>
                    </div>

                    <div class="action-buttons" style="margin-top:20px;">
                        <asp:Button ID="btnDeleteCourse" runat="server" Text="Delete Course" CssClass="btn-danger" OnClick="btnDeleteCourse_Click"  />
                    </div>
                </div>
            </div>
    </form>
</body>
</html>
