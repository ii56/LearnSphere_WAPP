<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminEditProfile.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminEditProfile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Profile</title>
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
                    <a href="AdminEditProfile.aspx" class="nav-item active">Edit Profile</a>
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
                    <h2>Edit Profile</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>
                <div class="profile-edit">
                    <div class="profile-card">
                        <div class="form-group">
                            <asp:Label ID="Label2" runat="server" Text="Username:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="lblUname" runat="server" CssClass="form-input"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label3" runat="server" Text="First Name:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="lblFname" runat="server" CssClass="form-input"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label4" runat="server" Text="Last Name:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="lblLname" runat="server" CssClass="form-input"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label5" runat="server" Text="Email:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="lblEmail" runat="server" CssClass="form-input"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label6" runat="server" Text="Age:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="lblAge" runat="server" CssClass="form-input"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label7" runat="server" Text="Gender:" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="dropdownGender" runat="server" CssClass="form-dropdown">
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="profile-card">
                    
                        <asp:Label ID="Label8" runat="server" Text="Profile Image:"></asp:Label>
                        <asp:Image ID="Image1" runat="server" Height="120px" Width="120px" />
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                    
                        <br />

                        <asp:Label ID="Label10" runat="server" Text="Description:"></asp:Label>
                        <asp:TextBox ID="txtDescription" runat="server" Rows="10" TextMode="MultiLine" Columns="50" MaxLength="999"></asp:TextBox>

                        <br />
                        <asp:Label ID="Label9" runat="server" Text="Forgot password? "></asp:Label>
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">Change Password</asp:LinkButton>
                    
                    </div>
                </div>
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="save-btn" OnClick="btnSave_Click" />
            </div>
        </div>
    </form>
</body>
</html>
