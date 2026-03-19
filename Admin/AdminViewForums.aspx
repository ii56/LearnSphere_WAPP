<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminViewForums.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminForums" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forums</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="AdminDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="UserManagement.aspx" class="nav-item">User Management</a>
                    <a href="CourseManagement.aspx" class="nav-item">Course Management</a>
                    <a href="Database.aspx" class="nav-item">Database</a>
                    <a href="AdminForums.aspx" class="nav-item active">Forums</a>
                    <a href="AdminEditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="AdminSyslog.aspx" class="nav-item">Syslog</a>
                </div>

                <div class="sidebar-profile">
                    <div class="profile-box admin">
                        <div class="profile-img-wrapper">
                            <img id="sidebarImg" runat="server" class="profile-img" />
                            <div class="verification-badge">✔</div>
                        </div>

                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] %></div>
                            <div class="profile-status">
                                administrator
                            </div>
                        </div>
                    </div>

                    <a href="AdminMessage.aspx" class="nav-item message-link">
                        Messaging
                        <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                            <span class="message-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click1" />
                </div>
            </div>

            <div class="main-content">
                <div class="forum-header">
                    <h2><asp:Label ID="lblForumTitle" runat="server" /></h2
                        >
                    <p class="forum-description">
                        <asp:Label ID="lblDescription" runat="server" />
                    </p>
                    <div class="forum-tags">
                        Allowed Tags: <asp:Label ID="lblTags" runat="server" />
                    </div>
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

                            <asp:LinkButton 
                                ID="btnDelete" 
                                runat="server" 
                                CssClass="btn-danger"
                                CommandName="Delete"
                                CommandArgument='<%# Eval("postid") %>'
                                OnClientClick="return confirm('Are you sure you want to delete this post?');">
                                Delete
                            </asp:LinkButton>

                            <a class="btn-view"
                               href='AdminForumDetails.aspx?postid=<%# Eval("postid") %>&courseid=<%# Request.QueryString["courseid"] %>'>
                                Details
                            </a>

                            <a class="btn-comment"
                               href='AdminReply.aspx?postid=<%# Eval("postid") %>&courseid=<%# Request.QueryString["courseid"] %>'>
                                Comment
                            </a>

                        </div>

                    </div>

                </div>

                </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </form>
</body>
</html>
