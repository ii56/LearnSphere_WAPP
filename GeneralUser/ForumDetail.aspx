<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForumDetail.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.ForumDetail" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forum Detail - LearnSphere</title>
    <link href="Forums.css?v=1" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item">Browse Courses</a>
                    <a href="MyCourses.aspx" class="nav-item">My Learning</a>
                    <a href="Forums.aspx" class="nav-item active">Course Forums</a>
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
                        Messages
                        <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                            <span class="message-badge"><%= Session["unreadCount"] %></span>
                        <% } %>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">
                <div class="back-container">
                    <a href='ViewForum.aspx?courseid=<%= Request.QueryString["courseid"] %>' class="btn-back">
                        ← Back to Forum
                    </a>
                </div>

                <div class="modern-card question-detail">
                    <div class="card-header">
                        <asp:Image ID="imgQuestionUser" runat="server" CssClass="user-avatar" />
                        <div class="user-info">
                            <asp:Label ID="lblQuestionUser" runat="server" CssClass="username" />
                            <asp:Label ID="lblQuestionDate" runat="server" CssClass="post-time" />
                        </div>
                    </div>

                    <div class="card-title">
                        <asp:Label ID="lblQuestionTitle" runat="server" />
                    </div>

                    <div class="card-preview">
                        <asp:Label ID="lblQuestionContent" runat="server" />
                    </div>

                    <div class="card-tags">
                        <asp:Literal ID="litTags" runat="server" />
                    </div>

                    <div class="card-footer">
                        <div class="stats">
                            <asp:Label ID="lblUpvotes" runat="server" />
                            <asp:Label ID="lblDownvotes" runat="server" />
                        </div>

                        <asp:Button ID="btnAnswer" runat="server" Text="Add Answer" CssClass="btn-comment" OnClick="btnAnswer_Click" />
                    </div>
                </div>

                <h3 style="margin:30px 0 15px 0;">Answers</h3>

                <asp:Repeater ID="rptAnswers" runat="server">
                    <ItemTemplate>
                        <div class="modern-card answer-card">
                            <div class="card-header">
                                <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' class="user-avatar" />
                                <div class="user-info">
                                    <div class="username"><%# Eval("uname") %></div>
                                    <div class="post-time">
                                        <%# Convert.ToDateTime(Eval("creationtime")).ToString("dd MMM yyyy") %>
                                    </div>
                                </div>
                            </div>

                            <div class="card-preview">
                                <%# Eval("content") %>
                            </div>

                            <div class="card-footer">
                                <div class="stats">
                                    <span>▲ <%# Eval("upvotes") %></span>
                                    <span>▼ <%# Eval("downvotes") %></span>
                                </div>

                                <div class="card-actions">
                                    <%# ShowDeleteButton(Eval("userid"), Eval("postid")) %>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        
        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>
</body>
</html>