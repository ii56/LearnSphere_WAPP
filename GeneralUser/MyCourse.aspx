<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyCourses.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.MyCourses" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Learning - LearnSphere</title>
    <link href="MyCourse.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item">Browse Courses</a>
                    <a href="MyCourses.aspx" class="nav-item active">My Learning</a>
                    <a href="Forums.aspx" class="nav-item">Course Forums</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                </div>

                <div class="sidebar-profile">
                    <div class="profile-box not-verified">
                        <div class="profile-img-wrapper">
                            <img id="imgSidebarProfile" runat="server" class="profile-img" src="~/images/default-user.png" />
                        </div>
                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] %></div>
                            <div class="profile-status">General User</div>
                        </div>
                    </div>
                    <a href="Message.aspx" class="nav-item message-link">
                        Messages <asp:Literal ID="litUnreadBadge" runat="server" />
                    </a>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">
                <div class="dashboard-header">
                    <h2>My Learning Journey</h2>
                    <span class="welcome-text">Pick up right where you left off.</span>
                </div>

                <div class="courses-section">
                    <div class="course-card-grid">
                        <asp:Repeater ID="rptMyCourses" runat="server">
                            <ItemTemplate>
                                <div class="course-card border-blue">
                                    <div class="course-card-content">
                                        <div class="course-category"><%# Eval("category") %></div>
                                        <h3 class="course-title"><%# Eval("coursename") %></h3>
                                        <p class="course-desc"><%# Eval("description").ToString().Length > 80 ? Eval("description").ToString().Substring(0, 80) + "..." : Eval("description") %></p>
                                    </div>
                                    <div class="course-card-footer">
                                        <a href='CourseContent.aspx?id=<%# Eval("courseid") %>' class="action-btn primary-btn">Continue Learning</a>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <asp:Label ID="lblNoCourses" runat="server" CssClass="empty-state-msg" Text="You haven't enrolled in any courses yet." Visible="false" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>