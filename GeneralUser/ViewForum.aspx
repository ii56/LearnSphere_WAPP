<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewForum.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.ViewForum" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Forum - LearnSphere</title>
    <link href="Forums.css?v=5" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">

            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item">Browse Courses</a>
                    <a href="MyCourse.aspx" class="nav-item">My Learning</a>
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
                <div class="forum-header">
                    <h2><asp:Label ID="lblForumTitle" runat="server" /></h2>
                    <p class="forum-description">
                        <asp:Label ID="lblDescription" runat="server" />
                    </p>
                    <div class="forum-tags">
                        Allowed Tags: <asp:Label ID="lblTags" runat="server" />
                    </div>
                </div>

                <div class="forum-actions">
                    <asp:Button ID="btnAskQuestion" runat="server" Text="Ask Question" CssClass="btn-primary" OnClick="btnAskQuestion_Click" />
                </div>

                <asp:Repeater ID="rptQuestions" runat="server" OnItemCommand="rptQuestions_ItemCommand">
                    <ItemTemplate>
                        <div class="modern-card">
                            <div class="card-header">
                                <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' class="user-avatar" />
                                <div class="user-info">
                                    <div class="username"><%# Eval("uname") %></div>
                                    <div class="post-time">
                                        <%# Convert.ToDateTime(Eval("creationtime")).ToString("dd MMM yyyy") %>
                                    </div>
                                </div>
                            </div>

                            <div class="card-title">
                                <%# Eval("title") %>
                            </div>

                            <div class="card-preview">
                                <%# Eval("content").ToString().Length > 180 
                                    ? Eval("content").ToString().Substring(0,180) + "..." 
                                    : Eval("content") %>
                            </div>

                            <div class="card-tags">
                                <%# FormatTags(Eval("tags")) %>
                            </div>

                            <div class="card-footer">
                                <div class="stats">
                                    <span>▲ <%# Eval("upvotes") %></span>
                                    <span>▼ <%# Eval("downvotes") %></span>
                                </div>

                                <div class="card-actions">
                                    <a class="btn-view" href='ForumDetail.aspx?postid=<%# Eval("postid") %>&courseid=<%# Request.QueryString["courseid"] %>'>
                                        View
                                    </a>
                                    <a class="btn-comment" href='answer.aspx?postid=<%# Eval("postid") %>'>
                                        Comment
                                    </a>
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