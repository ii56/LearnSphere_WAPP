<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminSyslog.aspx.cs" Inherits="LearnSphere_WAPP.Admin.Syslog" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Syslog</title>
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
                    <a href="AdminForums.aspx" class="nav-item">Forums</a>
                    <a href="AdminEditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="AdminSyslog.aspx" class="nav-item active">Syslog</a>
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
                <div class="dashboard-header">
                    <h2>Syslog</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>

                <div class="sorting">
                    
                    <div>
                        <asp:TextBox ID="txtSearch1" runat="server" CssClass="search-box" placeholder="Search userid..." AutoPostBack="True" OnTextChanged="txtSearch1_TextChanged" />
                        <asp:TextBox ID="txtSearch2" runat="server" CssClass="search-box" placeholder="Search action..." AutoPostBack="True" OnTextChanged="txtSearch2_TextChanged" />
                    </div>

                    <div class="sort-controls">
                        <asp:DropDownList ID="Filter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Sortby_SelectedIndexChanged">
                            <asp:ListItem>All</asp:ListItem>
                            <asp:ListItem>General</asp:ListItem>
                            <asp:ListItem>Student</asp:ListItem>
                            <asp:ListItem>Lecturer</asp:ListItem>
                            <asp:ListItem>Admin</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="Order" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Order_SelectedIndexChanged">
                            <asp:ListItem>Datetime Descending</asp:ListItem>
                            <asp:ListItem>Datetime Ascending</asp:ListItem>
                        </asp:DropDownList>
                    </div>
    
                </div>

                <asp:GridView ID="GridView1" runat="server" CssClass="admin-table" AutoGenerateColumns="False" Width="100%" AllowPaging="true" PageSize="14" OnPageIndexChanging="GridView1_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="userid" HeaderText="User ID" ItemStyle-Width="6%"/>
                        <asp:BoundField DataField="usertype" HeaderText="UserType" ItemStyle-Width="8%"/>
                        <asp:BoundField DataField="action" HeaderText="Action" ItemStyle-Width="71%"/>
                        <asp:BoundField DataField="dateTime" HeaderText="DateTime" ItemStyle-Width="15%"/>
                    </Columns>
                </asp:GridView>
                
                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>

            </div>

        </div>
    </form>
</body>
</html>
