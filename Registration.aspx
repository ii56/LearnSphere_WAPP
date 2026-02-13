<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="LearnSphere_WAPP.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Registration</title>
    <link href="registration.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="registration-container">
            <h2>Create Account</h2>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Please fix the following issues:" DisplayMode="BulletList" ForeColor="Red"/>
            <br />
            
            <div class="form-group">
                Username:
                <asp:TextBox ID="uname" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Username is required" ControlToValidate="uname" ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvUsername" runat="server" ErrorMessage="Username already exists! Try again please" ControlToValidate="uname" OnServerValidate="cvUsername_ServerValidate" ForeColor="Red"></asp:CustomValidator>
            </div>

            <div class="form-group">
                Email:
                <asp:TextBox ID="email" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="email is required" ControlToValidate="email" ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Invalid email format" ControlToValidate="email" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ForeColor="Red"></asp:RegularExpressionValidator>
            <?div>
            
            <div class="form-group">
                Password:
                <asp:TextBox ID="pwd" runat="server" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Password is required" ControlToValidate="pwd" ForeColor="Red"></asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
                Confirm password:
                <asp:TextBox ID="pwd2" runat="server" TextMode="Password"></asp:TextBox>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Passwords don`t match. Try again" ControlToValidate="pwd2" ControlToCompare="pwd" ForeColor="Red"></asp:CompareValidator>
            </div>

            <div class="form-group">
                First Name:
                <asp:TextBox ID="fname" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="fname" ErrorMessage="First name is required." ForeColor="Red"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                Last Name:
                <asp:TextBox ID="lname" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="lname" ErrorMessage="Last name is required." ForeColor="Red"></asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
                Age:
                <asp:TextBox ID="age" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="age" ErrorMessage="Age is required." ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RangeValidator runat="server" ControlToValidate="age" MinimumValue="1" MaximumValue="120" Type="Integer" ErrorMessage="Age must be between 1 and 120." ForeColor="Red"></asp:RangeValidator>
            <?div>

            <div class="form-group">
                Gender:
                <asp:DropDownList ID="gender" runat="server">
                    <asp:ListItem Text="Select"></asp:ListItem>
                    <asp:ListItem Text="Male"></asp:ListItem>  
                    <asp:ListItem Text="Female"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server"  ControlToValidate="gender" InitialValue="" ErrorMessage="Gender is required." ForeColor="Red"></asp:RequiredFieldValidator>
            </div>

            <div>
                User Type:
                <asp:DropDownList ID="usertype" runat="server">
                    <asp:ListItem>Select</asp:ListItem>
                    <asp:ListItem>Admin</asp:ListItem>
                    <asp:ListItem>Lecturer</asp:ListItem>
                    <asp:ListItem>Student</asp:ListItem>
                    <asp:ListItem>Public</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="User type is required plese choose one." ControlToValidate="usertype" InitialValue="" ForeColor="Red"></asp:RequiredFieldValidator>
            </div>

            <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" />
            <br /><br />
            <asp:Label ID="errMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
        </div>
    </form>
</body>
</html>
