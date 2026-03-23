<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminSyslog.aspx.cs" Inherits="LearnSphere_WAPP.Admin.Syslog" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Syslog</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <span class="verified-badge">Administrator</span>

                <div class="user-pill">
                        <div class="user-avatar">
                            <img id="sidebarImg" runat="server" />
                        </div>
                        <span class="user-name"><%= Session["uname"] %></span>
                    </div>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="nav">
                <a href="AdminDashboard.aspx" >Dashboard</a>
                <a href="UserManagement.aspx" >User Management</a>
                <a href="CourseManagement.aspx" >Course Management</a>
                <a href="Database.aspx" >Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
                <a href="AdminSyslog.aspx" class="active">Syslog</a>
                <a href="AdminMessage.aspx">
                    Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="nav-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                </a>
                <a href="../Chatbot/AdminChatbotKnowledge.aspx" >Chatbot</a>
            </div>

            <div class="container">
                <div class="welcome-banner">
                    <h2 class="welcome-label">Admin Portal</h2>
                    <h2 class="welcome-name">Syslog</h2>
                    <h3 class="welcome-sub">System log that record users activities</h3>
                </div>

                <div class="filter-bar">
                    
                    <div>
                        <asp:TextBox ID="txtSearch1" runat="server" CssClass="filter-input" placeholder="Search userid..." AutoPostBack="True" OnTextChanged="txtSearch1_TextChanged" />
                        <asp:TextBox ID="txtSearch2" runat="server" CssClass="filter-input" placeholder="Search action..." AutoPostBack="True" OnTextChanged="txtSearch2_TextChanged" />
                    </div>

                    <div class="sort-controls">
                        <asp:DropDownList ID="Filter" runat="server" CssClass="filter-input" AutoPostBack="True" OnSelectedIndexChanged="Sortby_SelectedIndexChanged">
                            <asp:ListItem>All</asp:ListItem>
                            <asp:ListItem>General</asp:ListItem>
                            <asp:ListItem>Student</asp:ListItem>
                            <asp:ListItem>Lecturer</asp:ListItem>
                            <asp:ListItem>Admin</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="Order" runat="server" CssClass="filter-input" AutoPostBack="True" OnSelectedIndexChanged="Order_SelectedIndexChanged">
                            <asp:ListItem>Datetime Descending</asp:ListItem>
                            <asp:ListItem>Datetime Ascending</asp:ListItem>
                        </asp:DropDownList>
                    </div>
    
                </div>

                <div class="section">
                    <asp:GridView ID="GridView1" runat="server" CssClass="admin-table" AutoGenerateColumns="False" Width="100%" AllowPaging="true" PageSize="14" OnPageIndexChanging="GridView1_PageIndexChanging" >
                        <Columns>
                            <asp:BoundField DataField="userid" HeaderText="User ID" ItemStyle-Width="6%"/>
                            <asp:BoundField DataField="usertype" HeaderText="UserType" ItemStyle-Width="8%"/>
                            <asp:BoundField DataField="action" HeaderText="Action" ItemStyle-Width="71%"/>
                            <asp:BoundField DataField="dateTime" HeaderText="DateTime" ItemStyle-Width="15%"/>
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:Label ID="lblResult" runat="server" Text="" Style="display:block; text-align:center; margin-bottom:20px; color:red; font-weight:bold;"></asp:Label>
                <div class="long-container">
                    <asp:Button ID="btnExport" runat="server" Text="Export to csv" CssClass="long-button" OnClick="btnExport_Click"/>
                </div>
            </div>

    </form>
</body>
</html>
