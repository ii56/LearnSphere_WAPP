<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="LearnSphere_WAPP.Registration" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration</title>
    <link href="registration.css?v=6" rel="stylesheet" />
</head>
<body>
        <div class="blob blob1"></div>
<div class="blob blob2"></div>
<div class="blob blob3"></div>
<form id="form1" runat="server">

<div class="registration-container">

    <h2>Create Account</h2>

    <!-- STEP INDICATOR -->
    <div class="step-indicator">
        Step <asp:Label ID="lblStep" runat="server" Text="1" /> of 3
    </div>

    <!-- ================= STEP 1 ================= -->
    <asp:Panel ID="pnlStep1" runat="server">

        <div class="form-group">
            Username:
            <asp:TextBox ID="uname" runat="server"></asp:TextBox>

            <asp:RequiredFieldValidator runat="server"
                ControlToValidate="uname"
                ErrorMessage="Username required"
                CssClass="error-message" />

            <asp:CustomValidator ID="cvUsername" runat="server"
                ControlToValidate="uname"
                ErrorMessage="Username already exists"
                OnServerValidate="cvUsername_ServerValidate"
                CssClass="error-message" />
        </div>

        <asp:Button ID="btnNext1" runat="server"
            Text="Next"
            CssClass="btn-register"
            OnClick="btnNext1_Click" />

    </asp:Panel>

    <!-- ================= STEP 2 ================= -->
    <asp:Panel ID="pnlStep2" runat="server" Visible="false">

        <div class="form-group">
            Email:
            <asp:TextBox ID="email" runat="server"></asp:TextBox>
        </div>

        <div class="form-group">
            Password:
            <asp:TextBox ID="pwd" runat="server" TextMode="Password"></asp:TextBox>
        </div>

        <div class="form-group">
            Confirm Password:
            <asp:TextBox ID="pwd2" runat="server" TextMode="Password"></asp:TextBox>
        </div>

        <asp:Button ID="btnBack1" runat="server"
            Text="Back"
            CssClass="btn-register"
            OnClick="btnBack1_Click" />

        <asp:Button ID="btnNext2" runat="server"
            Text="Next"
            CssClass="btn-register"
            OnClick="btnNext2_Click" />

    </asp:Panel>

    <!-- ================= STEP 3 ================= -->
    <asp:Panel ID="pnlStep3" runat="server" Visible="false">

        <div class="form-group">
            First Name:
            <asp:TextBox ID="fname" runat="server"></asp:TextBox>
        </div>

        <div class="form-group">
            Last Name:
            <asp:TextBox ID="lname" runat="server"></asp:TextBox>
        </div>

        <div class="form-group">
            Age:
            <asp:TextBox ID="age" runat="server"></asp:TextBox>
        </div>

        <div class="form-group">
            Gender:
            <asp:DropDownList ID="gender" runat="server">
                <asp:ListItem Text="Select" Value="" />
                <asp:ListItem Text="Male" Value="Male" />
                <asp:ListItem Text="Female" Value="Female" />
            </asp:DropDownList>
        </div>

        <asp:Button ID="btnBack2" runat="server"
            Text="Back"
            CssClass="btn-register"
            OnClick="btnBack2_Click" />

        <asp:Button ID="btnRegister" runat="server"
            Text="Register"
            CssClass="btn-register"
            OnClick="btnRegister_Click" />

    </asp:Panel>

    <asp:Label ID="errMsg" runat="server" CssClass="error-message" />
    <div class="login-redirect">
    <span>Already have an account?</span>

    <asp:HyperLink ID="lnkLogin"
        runat="server"
        NavigateUrl="~/Login.aspx"
        CssClass="btn-link-login">
        Login here
    </asp:HyperLink>
</div>
</div>

</form>
</body>
</html>