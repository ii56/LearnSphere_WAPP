<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editPublish.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.editPublish" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit and Publish</title>
    <link href="CreateCourse.css" rel="stylesheet" />
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

                    <a href="Message.aspx" class="nav-item message-link">
                        Messaging
                    </a>

                    <asp:Button ID="btnLogout" runat="server"
                        Text="Logout"
                        CssClass="logout-btn"
                        OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">

                <h2>Review & Publish</h2>
                <div class="review-card">

                    <div class="review-header">
                        <h3>Course Overview</h3>
                        <span class="review-status">Ready to Publish</span>
                    </div>

                    <div class="review-course-info">
                        <div class="review-title">
                            <asp:Label ID="lblCourseName" runat="server" />
                        </div>
                        <div class="review-description">
                            <asp:Label ID="lblCourseDesc" runat="server" />
                        </div>
                        <div class="review-price">
                            <asp:Label ID="lblCoursePrice" runat="server" />
                        </div>
                    </div>

                    <div class="review-divider"></div>

                    <h3 class="review-section-title">Modules & Lessons</h3>

                    <asp:Repeater ID="rptModules" runat="server">
                        <ItemTemplate>
                            <div class="module-card">

                                <div class="module-title">
                                    <%# Eval("modulename") %>
                                </div>

                                <div class="lesson-list">
                                    <asp:Repeater ID="rptLessons"
                                        runat="server"
                                        DataSource='<%# Eval("Lessons") %>'>
                                        <ItemTemplate>
                                            <div class="lesson-item">
                                                <span class="lesson-name">
                                                    <%# Eval("lessontitle") %>
                                                </span>
                                                <span class="lesson-duration">
                                                    <%# Eval("duration") %> mins
                                                </span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div class="review-actions">
                        <asp:Button ID="btnPublish" runat="server" Text="Publish Course" CssClass="btn-modern" OnClick="btnPublish_Click" />

                        <asp:Button ID="btnBack" runat="server" Text="← Back to Edit" CssClass="btn-modern-secondary" OnClick="btnBack_Click" />
                    </div>

                    <asp:Label ID="lblMessage" runat="server" />
                </div>
            </div>

        </div>
      </form> 
</body>
</html>