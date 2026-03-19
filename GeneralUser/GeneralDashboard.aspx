<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneralDashboard.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.GeneralDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>User Dashboard - LearnSphere</title>
    <link href="GeneralDashboard.css?v=1" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item active">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item">Browse Courses</a>
                    <a href="MyCourse.aspx" class="nav-item">My Learning</a>
                    <a href="Forums.aspx" class="nav-item">Course Forums</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                </div>

                <div class="sidebar-profile">
                    <div class="profile-box not-verified">
                        <div class="profile-img-wrapper">
                            <img id="imgSidebarProfile" runat="server" class="profile-img" src="../Assets/default-avatar.png" alt="Profile Image" />
                        </div>

                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] != null ? Session["uname"].ToString() : "Guest" %></div>
                            <div class="profile-status">General User</div>
                        </div>
                    </div>

                    <a href="Message.aspx" class="nav-item message-link">
                        Messages
                        <asp:Literal ID="litUnreadBadge" runat="server"></asp:Literal>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">
                <div class="dashboard-header">
                    <h2>Welcome back, <asp:Label ID="lblUserName" runat="server" />!</h2>
                    <span class="welcome-text">Ready to continue your learning journey?</span>
                </div>

                <div class="courses-section">
                    <h3>Continue Studying</h3>
                    <span class="section-sub">Jump right back into your active courses</span>

                    <div class="course-card-grid">
                        <asp:Repeater ID="rptStudyingCourses" runat="server">
                            <ItemTemplate>
                                <div class="course-card border-blue">
                                    <div class="course-card-content">
                                        <div class="course-category"><%# Eval("category") %></div>
                                        <h4 class="course-title"><%# Eval("coursename") %></h4>
                                        <p class="course-desc"><%# Eval("description").ToString().Length > 60 ? Eval("description").ToString().Substring(0, 60) + "..." : Eval("description") %></p>
                                    </div>
                                    <div class="course-card-footer">
                                        <a href='CourseContent.aspx?courseid=<%# Eval("courseid") %>' class="action-btn primary-btn">Resume Course</a>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblNoStudying" runat="server" Visible="false" CssClass="empty-state-msg">You haven't enrolled in any courses yet.</asp:Label>
                    </div>
                </div>

                <div class="courses-section">
                    <h3>Recommended For You</h3>
                    <span class="section-sub">Discover new topics based on platform popularity</span>

                    <div class="course-card-grid">
                        <asp:Repeater ID="rptRecommendedCourses" runat="server">
                            <ItemTemplate>
                                <div class="course-card border-green">
                                    <div class="course-card-content">
                                        <div class="course-category"><%# Eval("category") %></div>
                                        <h4 class="course-title"><%# Eval("coursename") %></h4>
                                        <div class="course-price">RM <%# Convert.ToDecimal(Eval("price")).ToString("0.00") %></div>
                                    </div>
                                    <div class="course-card-footer">
                                        <a href='CourseDetails.aspx?courseid=<%# Eval("courseid") %>' class="action-btn outline-btn">View Details</a>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblNoRecommendations" runat="server" Visible="false" CssClass="empty-state-msg">No recommendations available at the moment.</asp:Label>
                    </div>
                </div>

                <div class="quick-actions-section">
                    <h3>Quick Links</h3>
                    <div class="quick-card-grid">
                        <a href="ViewCourses.aspx" class="quick-card">
                            <div class="quick-card-title">Browse Catalog</div>
                            <div class="quick-card-desc">Explore all available courses</div>
                        </a>
                        <a href="Forums.aspx" class="quick-card">
                            <div class="quick-card-title">Community Forums</div>
                            <div class="quick-card-desc">Engage with other learners</div>
                        </a>
                        <a href="EditProfile.aspx" class="quick-card">
                            <div class="quick-card-title">Update Profile</div>
                            <div class="quick-card-desc">Manage your account settings</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>

        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>
</body>
</html>