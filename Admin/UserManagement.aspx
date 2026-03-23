<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="LearnSphere_WAPP.Admin.UserManagement" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Management</title>
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
                <a href="UserManagement.aspx" class="active">User Management</a>
                <a href="CourseManagement.aspx" >Course Management</a>
                <a href="Database.aspx" >Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
                <a href="AdminSyslog.aspx" >Syslog</a>
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
                    <h2 class="welcome-name">Users Management</h2>
                    <h3 class="welcome-sub">Manage user account and verify their request</h3>
                </div>

                <div class="filter-bar">
                    
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-input" placeholder="Search user..." AutoPostBack="True" OnTextChanged="txtSearch_TextChanged" />

                    <div class="sort-controls">
                        <asp:CheckBox ID="chkPending" runat="server" CssClass="filter-input" Text="Pending" AutoPostBack="True" OnCheckedChanged="chkPending_CheckedChanged" />
                        <asp:DropDownList ID="Sortby" runat="server" CssClass="filter-input" AutoPostBack="True" OnSelectedIndexChanged="Sortby_SelectedIndexChanged">
                            <asp:ListItem>User ID</asp:ListItem>
                            <asp:ListItem>Username</asp:ListItem>
                            <asp:ListItem>Age</asp:ListItem>
                            <asp:ListItem>Gender</asp:ListItem>
                            <asp:ListItem>Creation Time</asp:ListItem>
                            <asp:ListItem>User Type</asp:ListItem>
                            <asp:ListItem>Status</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="Order" runat="server" CssClass="filter-input" AutoPostBack="True" OnSelectedIndexChanged="Order_SelectedIndexChanged">
                            <asp:ListItem>Ascending</asp:ListItem>
                            <asp:ListItem>Descending</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                </div>
                <div class="section">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="100%" AllowPaging="True" PageSize="8" OnRowCommand="GridView1_RowCommand" OnPageIndexChanging="GridView1_PageIndexChanging1" >
                        <Columns>
                            <asp:BoundField DataField="userid" HeaderText="User ID" ItemStyle-Width="5%"/>
                            <asp:BoundField DataField="uname" HeaderText="Username" ItemStyle-Width="14%"/>
                            <asp:BoundField DataField="email" HeaderText="Email" ItemStyle-Width="20%"/>
                            <asp:BoundField DataField="age" HeaderText="Age" ItemStyle-Width="4%"/>
                            <asp:BoundField DataField="gender" HeaderText="Gender" ItemStyle-Width="5%"/>
                            <asp:BoundField DataField="creationtime" HeaderText="Creation Time" ItemStyle-Width="12%"/>
                            <asp:BoundField DataField="usertype" HeaderText="User Type" ItemStyle-Width="7%"/>
                            <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-Width="7%"/>
                            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="26%">
                                <ItemTemplate>

                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-primary btn-sm" CommandName="EditUser" CommandArgument='<%# Eval("userid") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-danger btn-sm" CommandName="DeleteUser" CommandArgument='<%# Eval("userid") %>' OnClientClick="return confirm('Delete this user account?');" />
                                    <asp:Button ID="btnVerify" runat="server" Text="Verify" CssClass="btn-secondary btn-sm" 
                                        CommandName="VerifyUser" CommandArgument='<%# Eval("userid") %>' 
                                        Visible='<%# Convert.ToInt32(Eval("HasPending")) == 1 %>' />

                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <asp:Label ID="lblResult" runat="server" Text="" Style="display:block; text-align:center; margin-bottom:20px; color:red; font-weight:bold;"></asp:Label>

                <div class="long-container">
                    <asp:Button ID="btnCreate" runat="server" Text="Add new user +" CssClass="long-button" OnClick="btnCreate_Click"/>
                </div>
                
            </div>
    </form>
</body>
</html>
