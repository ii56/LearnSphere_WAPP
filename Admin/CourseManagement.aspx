<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseManagement.aspx.cs" Inherits="LearnSphere_WAPP.Admin.CourseManagement" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Management</title>
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
                    <a href="CourseManagement.aspx" class="nav-item active">Course Management</a>
                    <a href="Database.aspx" class="nav-item">Database</a>
                    <a href="AdminForums.aspx" class="nav-item">Forums</a>
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
                    <h2>Course Management</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>

                    <div class="sorting">
        
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" placeholder="Search course name..." AutoPostBack="True" OnTextChanged="txtSearch_TextChanged" />

                        <div class="sort-controls">
                            <asp:DropDownList ID="Sortby" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Sortby_SelectedIndexChanged">
                                <asp:ListItem>Course ID</asp:ListItem>
                                <asp:ListItem>Owner ID</asp:ListItem>
                                <asp:ListItem>Course Name</asp:ListItem>
                                <asp:ListItem>Price</asp:ListItem>
                                <asp:ListItem>Creation Time</asp:ListItem>
                                <asp:ListItem>Deletion Time</asp:ListItem>
                                <asp:ListItem>Category</asp:ListItem>
                                <asp:ListItem>Status</asp:ListItem>
                            </asp:DropDownList>
                            <asp:DropDownList ID="Order" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Order_SelectedIndexChanged">
                                <asp:ListItem>Ascending</asp:ListItem>
                                <asp:ListItem>Descending</asp:ListItem>
                            </asp:DropDownList>
                        </div>
        
                    </div>

                    <asp:GridView ID="GridView1" runat="server" CssClass="admin-table" AutoGenerateColumns="False" Width="100%" AllowPaging="True" PageSize="8" OnRowCommand="GridView1_RowCommand" OnPageIndexChanging="GridView1_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="courseid" HeaderText="Course ID" ItemStyle-Width="4"/>
                            <asp:BoundField DataField="ownerid" HeaderText="Owner ID" ItemStyle-Width="4%"/>
                            <asp:BoundField DataField="coursename" HeaderText="Course Name" ItemStyle-Width="21%"/>
                            <asp:BoundField DataField="price" HeaderText="Price" ItemStyle-Width="8%"/>
                            <asp:BoundField DataField="creationtime" HeaderText="Creation Time" ItemStyle-Width="15%"/>
                            <asp:BoundField DataField="deletiontime" HeaderText="Deletion Time" ItemStyle-Width="15%"/>
                            <asp:BoundField DataField="category" HeaderText="Category" ItemStyle-Width="8%"/>
                            <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-Width="10%"/>
                            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="15%">
                                <ItemTemplate>

                                    <asp:Button ID="btnView" runat="server" Text="View" CommandName="ViewCourse" CommandArgument='<%# Eval("courseid") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteCourse" CommandArgument='<%# Eval("courseid") %>' OnClientClick="return confirm('Delete this course?');" />

                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
    
                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>

                </div>

            </div>
    </form>
</body>
</html>
