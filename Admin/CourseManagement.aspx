<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseManagement.aspx.cs" Inherits="LearnSphere_WAPP.Admin.CourseManagement" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Management</title>
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
                <a href="CourseManagement.aspx" class="active">Course Management</a>
                <a href="Database.aspx" >Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
                <a href="AdminSyslog.aspx" >Syslog</a>
                <a href="AdminMessage.aspx">
                    <span>✉️</span> Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="nav-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                </a>
                <a href="../Chatbot/AdminChatbotKnowledge.aspx" >Chatbot</a>
            </div>

            <div class="container">
                <div class="welcome-banner">
                    <h2 class="welcome-label">Admin Portal</h2>
                    <h2 class="welcome-name">Courses Management</h2>
                    <h3 class="welcome-sub">Manage courses details</h3>
                </div>

                <div class="filter-bar">
        
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-input" placeholder="Search course name..." AutoPostBack="True" OnTextChanged="txtSearch_TextChanged" />

                    <div class="sort-controls">
                        <asp:DropDownList ID="Sortby" runat="server" CssClass="filter-input" AutoPostBack="True" OnSelectedIndexChanged="Sortby_SelectedIndexChanged">
                            <asp:ListItem>Course ID</asp:ListItem>
                            <asp:ListItem>Owner ID</asp:ListItem>
                            <asp:ListItem>Course Name</asp:ListItem>
                            <asp:ListItem>Price</asp:ListItem>
                            <asp:ListItem>Creation Time</asp:ListItem>
                            <asp:ListItem>Deletion Time</asp:ListItem>
                            <asp:ListItem>Category</asp:ListItem>
                            <asp:ListItem>Status</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="Order" runat="server" CssClass="filter-input" AutoPostBack="True" OnSelectedIndexChanged="Order_SelectedIndexChanged">
                            <asp:ListItem>Ascending</asp:ListItem>
                            <asp:ListItem>Descending</asp:ListItem>
                        </asp:DropDownList>
                    </div>
        
                </div>

                <div class="section">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="100%" AllowPaging="True" PageSize="8" OnRowCommand="GridView1_RowCommand" OnPageIndexChanging="GridView1_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="courseid" HeaderText="Course ID" ItemStyle-Width="4"/>
                            <asp:BoundField DataField="ownerid" HeaderText="Owner ID" ItemStyle-Width="4%"/>
                            <asp:BoundField DataField="coursename" HeaderText="Course Name" ItemStyle-Width="21%"/>
                            <asp:BoundField DataField="price" HeaderText="Price" ItemStyle-Width="8%"/>
                            <asp:BoundField DataField="creationtime" HeaderText="Creation Time" ItemStyle-Width="15%"/>
                            <asp:BoundField DataField="category" HeaderText="Category" ItemStyle-Width="8%"/>
                            <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-Width="10%"/>
                            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="15%">
                                <ItemTemplate>

                                    <asp:Button ID="btnView" runat="server" class="btn-primary" Text="View" CommandName="ViewCourse" CommandArgument='<%# Eval("courseid") %>' />
                                    <asp:Button ID="btnDelete" runat="server" class="btn-danger" Text="Delete" CommandName="DeleteCourse" CommandArgument='<%# Eval("courseid") %>' OnClientClick="return confirm('Delete this course?');" />

                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
    
                    <asp:Label ID="lblResult" runat="server" Text="" Style="display:block; text-align:center; margin-bottom:20px; color:red; font-weight:bold;"></asp:Label>

            </div>

    </form>
</body>
</html>
