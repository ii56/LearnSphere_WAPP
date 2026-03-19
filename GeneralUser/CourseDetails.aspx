<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseDetails.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.CourseDetails" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Details - LearnSphere</title>
    <link href="CourseDetails.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item active">Browse Courses</a>
                    <a href="MyCourses.aspx" class="nav-item">My Learning</a>
                    <a href="Forums.aspx" class="nav-item">Course Forums</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                </div>

                <div class="sidebar-profile">
                    <div class="profile-box not-verified">
                        <div class="profile-img-wrapper">
                            <img id="imgSidebarProfile" runat="server" class="profile-img" src="~/images/default-user.png" />
                        </div>
                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] != null ? Session["uname"].ToString() : "Guest" %></div>
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
                
                <div class="back-container">
                    <a href="ViewCourses.aspx" class="btn-back">← Back to Courses</a>
                </div>

                <div class="detail-card">
                    <div class="detail-header-layout">
                        <div class="header-text-area">
                            <asp:Label ID="lblCategory" runat="server" CssClass="detail-category"></asp:Label>
                            <h2 class="detail-title"><asp:Label ID="lblCourseName" runat="server"></asp:Label></h2>
                            
                            <div class="instructor-block">
                                <img src="../images/default-user.png" class="instructor-avatar" />
                                <div class="instructor-info">
                                    <span class="instructor-label">Instructor</span>
                                    <asp:Label ID="lblInstructorName" runat="server" CssClass="instructor-name"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="header-action-area">
                            <div class="price-box">
                                <asp:Label ID="lblPrice" runat="server" CssClass="detail-price"></asp:Label>
                            </div>
                            
                            <asp:Button ID="btnCourseAction" runat="server" CssClass="btn-enroll-large" OnClick="btnCourseAction_Click" />
                            
                            <div style="margin-top: 15px;">
                                <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <hr class="divider" />

                    <div class="detail-body">
                        <h3>About This Course</h3>
                        <div class="description-text">
                            <asp:Literal ID="litDescription" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>
</body>
</html>