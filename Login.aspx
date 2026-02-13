<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LearnSphere_WAPP.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <link href="registration.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="registration-container">
            <h2>Login</h2>

            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="error-message" DisplayMode="BulletList" />

            <div class="form-group">
                <label>Username</label>
                <asp:TextBox ID="uname" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Isername is required" ControlToValidate="uname" CssClass="error-message"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="pwd" runat="server" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="pwd" ErrorMessage="Password is required." CssClass="error-message"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label>User Type</label>
                <asp:DropDownList ID="usertype" runat="server">
                    <asp:ListItem>Select</asp:ListItem>
                    <asp:ListItem>Admin</asp:ListItem>
                    <asp:ListItem>Lecturer</asp:ListItem>
                    <asp:ListItem>Student</asp:ListItem>
                    <asp:ListItem>Public</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="User type is required" ControlToValidate="usertype" CssClass="error-message"></asp:RequiredFieldValidator>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-register" OnClick="btnLogin_Click" />
            <br /><br />

            <asp:Label ID="errMsg" runat="server" CssClass="error-message" />
        </div>
    </form>
</body>
</html>
