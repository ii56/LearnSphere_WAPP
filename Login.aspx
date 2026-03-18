<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LearnSphere_WAPP.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <link href="registration.css?v=5" rel="stylesheet" />
</head>
<body>
    <div class="blob blob1"></div>
<div class="blob blob2"></div>
<div class="blob blob3"></div>
    <form id="form1" runat="server">
        <div class="registration-container">
            <h2>Login</h2>

            <div class="logo-container">
                <img src='<%= ResolveUrl("~/LEARNSPHERE_sign.png") %>'  alt="LearnSphere Logo" class="login-logo" />
            </div>

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

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-register" OnClick="btnLogin_Click" />
            <br /><br />

            <br />

<div class="register-redirect">
    <span>Don't have an account?</span>

    <asp:HyperLink ID="lnkRegister"
        runat="server"
        NavigateUrl="~/Registration.aspx"
        CssClass="btn-link-register">
        Register here
    </asp:HyperLink>
</div>

            <asp:Label ID="errMsg" runat="server" CssClass="error-message" />
        </div>
    </form>
</body>
</html>
