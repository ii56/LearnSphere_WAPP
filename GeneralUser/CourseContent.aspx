<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseContent.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.CourseContent" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Content - LearnSphere</title>
    <link href="CourseContent.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item">Browse Courses</a>
                    <a href="MyCourse.aspx" class="nav-item active">My Learning</a> <a href="Forums.aspx" class="nav-item">Course Forums</a>
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
                <div class="content-wrapper">
                    
                    <div class="course-nav-panel">
                        <div class="nav-header">
                            <span class="nav-label">Now Learning</span>
                            <h3 class="nav-course-name"><asp:Label ID="lblCourseName" runat="server" Text="..." /></h3>
                            <a href="MyCourses.aspx" class="back-link">← Back to My Courses</a>
                        </div>

                        <div class="nav-modules">
                            <asp:Panel ID="pnlModules" runat="server">
                                <asp:Repeater ID="rptModules" runat="server">
                                    <ItemTemplate>
                                        <div class="module-group">
                                            <div class="module-title"><%# Eval("ModuleName") %></div>
                                            
                                            <asp:Repeater ID="rptLessons" runat="server" DataSource='<%# Eval("Lessons") %>'>
                                                <ItemTemplate>
                                                    <a href='CourseContent.aspx?id=<%# Request.QueryString["id"] %>&lessonId=<%# Eval("LessonId") %>'
                                                       class='lesson-item <%# Convert.ToBoolean(Eval("IsCompleted")) ? "completed" : "" %> <%# Request.QueryString["lessonId"] == Eval("LessonId").ToString() ? "active" : "" %>'>
                                                        <div class="lesson-dot"></div>
                                                        <%# Eval("LessonTitle") %>
                                                    </a>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </asp:Panel>

                            <asp:Panel ID="pnlNoModules" runat="server" Visible="false">
                                <div class="empty-state">
                                    <p>No lessons available for this course yet.</p>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>

                    <div class="viewer-panel">
                        
                        <asp:Panel ID="pnlLesson" runat="server" Visible="false">
                            <div class="viewer-card">
                                <h1 class="lesson-title"><asp:Label ID="lblLessonTitle" runat="server" /></h1>
                                <p class="lesson-desc"><asp:Label ID="lblLessonDesc" runat="server" /></p>

                                <asp:Panel ID="pnlVideo" runat="server" Visible="false" CssClass="video-container">
                                    <iframe id="iframeVideo" runat="server" allowfullscreen="true"></iframe>
                                </asp:Panel>

                                <asp:Panel ID="pnlNoVideo" runat="server" Visible="true" CssClass="no-video-container">
                                    <span class="icon">▶</span>
                                    <span>No video available for this lesson.</span>
                                </asp:Panel>

                                <asp:Panel ID="pnlFiles" runat="server" Visible="false">
                                    <h4 class="materials-title">Lesson Materials</h4>
                                    <div class="materials-grid">
                                        <asp:Repeater ID="rptMaterials" runat="server">
                                            <ItemTemplate>
                                                <a href='<%# Eval("fileurl") %>' target="_blank" class="material-link">
                                                    <span class="file-icon">📄</span>
                                                    <span class="file-name"><%# Eval("filetype") %> Document</span>
                                                    <span class="file-arrow">→</span>
                                                </a>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </asp:Panel>

                                <div class="completion-area">
                                    <asp:Button ID="btnComplete" runat="server" Text="Mark as Completed (+10 Points)" CssClass="btn-complete" OnClick="btnComplete_Click" />
                                    <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="status-msg" />
                                </div>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlSelectLesson" runat="server" Visible="true">
                            <div class="viewer-card empty-viewer">
                                <span class="icon">👈</span>
                                <h2>Select a lesson to start</h2>
                                <p>Choose a lesson from the menu on the left to begin learning.</p>
                            </div>
                        </asp:Panel>

                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>