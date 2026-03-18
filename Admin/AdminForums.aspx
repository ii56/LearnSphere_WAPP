<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminForums.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminForums1" %>

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
                <h2>Course Forums</h2>
                <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" CssClass="forum-table" OnRowCommand="gvCourses_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button ID="btnView" runat="server" Text="View Forum" CommandName="ViewForum" CommandArgument='<%# Eval("courseid") %>' CssClass="btn-view" Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>' />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete Forum" CommandName="DeleteForum" CommandArgument='<%# Eval("courseid") %>' CssClass="btn-danger" Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>' />
                                <asp:Label ID="lblNoForum" runat="server"
                                    Text="No forum yet"
                                    CssClass="no-forum-text"
                                    Visible='<%# !Convert.ToBoolean(Eval("HasForum")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
               </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
