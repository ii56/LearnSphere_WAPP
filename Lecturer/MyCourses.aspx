<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateCourse.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.CreateCourse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Course</title>
    <link href="LecturerDashboard.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="LecturerDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="CreateCourse.aspx" class="nav-item active">Create Course</a>
                    <a href="ViewCourses.aspx" class="nav-item">View Courses</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
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
                <h2>Create New Course</h2>
                <div class="form-card">
                    <label>Course Name</label>
                    <asp:TextBox ID="txtCourseName" runat="server" CssClass="form-input" />
                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtCourseName" ErrorMessage="Course name required" ForeColor="Red"></asp:RequiredFieldValidator>

                    <label>Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-input"></asp:TextBox>

                    <label>Category</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-input">
                        <asp:ListItem Text="AI" />
                        <asp:ListItem Text="Machine Learning" />
                        <asp:ListItem Text="Web Development" />
                        <asp:ListItem Text="Programming" />
                    </asp:DropDownList>

                    <label>Price (0 for Free)</label>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-input" />

                    <asp:Button ID="btnCreate" runat="server" Text="Create Course" CssClass="btn-primary" OnClick="btnCreate_Click" />
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
            </div>
        </div>
        
    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
<script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    
    </form>
</body>
</html>
