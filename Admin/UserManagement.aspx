<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="LearnSphere_WAPP.Admin.UserManagement" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Management</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="AdminDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="UserManagement.aspx" class="nav-item active">User Management</a>
                    <a href="CourseManagement.aspx" class="nav-item">Course Management</a>
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

                    <a href="AdminMessage.aspx" class="nav-item message-link">
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
                    <h2>User Management</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>

                <div class="sorting">
                    
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" placeholder="Search user..." AutoPostBack="True" OnTextChanged="txtSearch_TextChanged" />

                    <div class="sort-controls">
                        <asp:CheckBox ID="chkPending" runat="server" Text="Pending" AutoPostBack="True" OnCheckedChanged="chkPending_CheckedChanged" />
                        <asp:DropDownList ID="Sortby" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Sortby_SelectedIndexChanged">
                            <asp:ListItem>User ID</asp:ListItem>
                            <asp:ListItem>Username</asp:ListItem>
                            <asp:ListItem>Age</asp:ListItem>
                            <asp:ListItem>Gender</asp:ListItem>
                            <asp:ListItem>Creation Time</asp:ListItem>
                            <asp:ListItem>User Type</asp:ListItem>
                            <asp:ListItem>Status</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="Order" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Order_SelectedIndexChanged">
                            <asp:ListItem>Ascending</asp:ListItem>
                            <asp:ListItem>Descending</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                </div>

                <asp:GridView ID="GridView1" runat="server" CssClass="admin-table" AutoGenerateColumns="False" Width="100%" AllowPaging="True" PageSize="8" OnRowCommand="GridView1_RowCommand" OnPageIndexChanging="GridView1_PageIndexChanging1">
                    <Columns>
                        <asp:BoundField DataField="userid" HeaderText="User ID" ItemStyle-Width="4%"/>
                        <asp:BoundField DataField="uname" HeaderText="Username" ItemStyle-Width="15%"/>
                        <asp:BoundField DataField="email" HeaderText="Email" ItemStyle-Width="13%"/>
                        <asp:BoundField DataField="fname" HeaderText="First Name" ItemStyle-Width="11%"/>
                        <asp:BoundField DataField="lname" HeaderText="Last Name" ItemStyle-Width="11%"/>
                        <asp:BoundField DataField="age" HeaderText="Age" ItemStyle-Width="3%"/>
                        <asp:BoundField DataField="gender" HeaderText="Gender" ItemStyle-Width="3%"/>
                        <asp:BoundField DataField="creationtime" HeaderText="Creation Time" ItemStyle-Width="13%"/>
                        <asp:BoundField DataField="usertype" HeaderText="User Type" ItemStyle-Width="6%"/>
                        <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-Width="8%"/>
                        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="12%">
                            <ItemTemplate>

                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-edit" CommandName="EditUser" CommandArgument='<%# Eval("userid") %>' />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-delete" CommandName="DeleteUser" CommandArgument='<%# Eval("userid") %>' OnClientClick="return confirm('Delete this user account?');" />
                                <asp:Button ID="btnVerify" runat="server" Text="Verify" CssClass="btn-verify" 
                                    CommandName="VerifyUser" CommandArgument='<%# Eval("userid") %>' 
                                    Visible='<%# HasPendingRequest(Eval("userid")) %>' />

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
