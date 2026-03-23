<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AddUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add User</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" />
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
                    <h2 class="welcome-name">Add User</h2>
                    <h3 class="welcome-sub">Add a new general user</h3>
                </div>

                <asp:Panel runat="server" DefaultButton="btnSave">
                <div class="card">
                    <div class="card-title">
                        <span class="title-dot dot-green"></span>
                        Enter User Information
                    </div>

                    <div class="form-grid">
                        <div class="form-group">
                            <asp:Label ID="Label2" runat="server" Text="Username:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtUname" runat="server" CssClass="form-input"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="profileGroup" runat="server"
                                ControlToValidate="txtUname"
                                ErrorMessage="Username required"
                                CssClass="validation-error"/>
                            <asp:CustomValidator ID="cvUsername" ValidationGroup="profileGroup" runat="server"
                                ControlToValidate="txtUname"
                                ErrorMessage="Username already exists"
                                CssClass="validation-error" 
                                OnServerValidate="cvUsername_ServerValidate"/>
                        </div>
                        
                        <div class="form-group">
                            <asp:Label ID="Label5" runat="server" Text="Email:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtEmail" ErrorMessage="Email is required."
                                CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                            <asp:RegularExpressionValidator ControlToValidate="txtEmail"
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                ErrorMessage="Invalid email format." CssClass="validation-error"
                                ValidationGroup="profileGroup" runat="server" />
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label3" runat="server" Text="First Name:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtFname" runat="server" CssClass="form-input"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtFname" ErrorMessage="First name required."
                                CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label4" runat="server" Text="Last Name:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtLname" runat="server" CssClass="form-input"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtLname" ErrorMessage="Last name required."
                                CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label6" runat="server" Text="Age:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtAge" runat="server" CssClass="form-input"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtAge" ErrorMessage="Age required."
                                CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                            <asp:RangeValidator ControlToValidate="txtAge" MinimumValue="13" MaximumValue="120"
                                Type="Integer" ErrorMessage="Age must be between 13 and 120."
                                CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label7" runat="server" Text="Gender:" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="dropdownGender" runat="server" CssClass="form-input">
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label8" runat="server" Text="Password:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtPwd" runat="server" Text="" CssClass="form-input" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtPwd" ErrorMessage="Password is required."
                                CssClass="validation-error" ValidationGroup="profileGroup" runat="server" />
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label9" runat="server" Text="Confirmation Password:" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="txtVer" runat="server" Text="" CssClass="form-input" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator 
                                ID="rfvVer" 
                                runat="server"
                                ControlToValidate="txtVer"
                                ErrorMessage="Confirmation password is required."
                                ValidationGroup="profileGroup"
                                CssClass="validation-error" />
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPwd" ValidationGroup="profileGroup" ControlToValidate="txtVer" ErrorMessage="Password not match." CssClass="validation-error"></asp:CompareValidator>
                        </div>

                        <div class="btn-row">
                            <asp:Button ID="btnSave" runat="server" Text="Add User" CssClass="btn-primary"
                                ValidationGroup="profileGroup" OnClick="btnSave_Click" />
                            <asp:Label ID="lblMessage" runat="server" />
                        </div>

                        <asp:ValidationSummary 
                            runat="server" 
                            ValidationGroup="profileGroup"
                            CssClass="validation-error" />
                    </div>
                </div>
                </asp:Panel>
            </div>
    </form>
</body>
</html>
