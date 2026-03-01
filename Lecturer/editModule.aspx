<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editModule.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.editModule" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Module</title>
    <link href="courses.css" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">
<div class="layout">
    <div class="sidebar">
        <div>
            <div class="sidebar-title">LearnSphere</div>
            <a href="LecturerDashboard.aspx" class="nav-item">Dashboard</a>
            <a href="CreateCourse.aspx" class="nav-item">Create Course</a>
            <a href="ViewCourses.aspx" class="nav-item active">View Courses</a>
            <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
            <a href="Forums.aspx" class="nav-item">Forums</a>
        </div>

                <div class="sidebar-profile">
                    <div class="profile-box <%= (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") ? "verified" : "not-verified" %>">
                        <div class="profile-img-wrapper">
                            <img id="imgSidebarProfile" runat="server" class="profile-img" />

                            <% if (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") { %>
                                <div class="verification-badge">✔</div>
                            <% } %>
                        </div>

                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] %></div>
                            <div class="profile-status">
                                <%= (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") 
                                    ? "Verified Lecturer" 
                                    : "General User" %>
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

        <h2>
            <asp:Label ID="lblModeTitle" runat="server" />
            -
            <asp:Label ID="lblCourseTitle" runat="server" />
        </h2>

        <div class="form-card-modern">

            <label>Module Name *</label>
            <asp:TextBox ID="txtModuleName" runat="server" CssClass="modern-input"></asp:TextBox>

            <label>Module Description</label>
            <asp:TextBox ID="txtModuleDesc" runat="server" TextMode="MultiLine" Rows="3" CssClass="modern-input"></asp:TextBox>

            <label>Order Number</label>
            <asp:TextBox ID="txtOrderNumber" runat="server" CssClass="modern-input"></asp:TextBox>

            <br /><br />

            <asp:Button ID="btnSave" runat="server" CssClass="btn-modern" OnClick="btnSave_Click" />

            &nbsp;&nbsp;

            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-modern-secondary" OnClick="btnCancel_Click" />

            <br /><br />

            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />

        </div>

    </div>

</div>
</form>
</body>
</html>