<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestConnection.aspx.cs" Inherits="LearnSphere_WAPP.TestConnection" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin:50px">
            <h2>Database Connection Test</h2>

            <asp:Button ID="btnTest" runat="server" 
                Text="Test Connection" 
                OnClick="btnTest_Click" />

            <br /><br />

            <asp:Label ID="lblResult" runat="server" 
                Font-Bold="true" />
        </div>
    </form>
</body>
</html>
