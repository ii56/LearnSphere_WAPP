<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewStudents.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.ViewStudents" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Enrolled Students</title>
    <link href="ViewStudents.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="container">
        <h2>Enrolled Students</h2>

        <asp:GridView ID="gvStudents" runat="server"
            AutoGenerateColumns="False"
            CssClass="students-table">

            <Columns>
                <asp:BoundField DataField="userid" HeaderText="User ID" />
                <asp:BoundField DataField="uname" HeaderText="Username" />
                <asp:BoundField DataField="fname" HeaderText="First Name" />
                <asp:BoundField DataField="lname" HeaderText="Last Name" />
                <asp:BoundField DataField="email" HeaderText="Email" />
                <asp:BoundField DataField="age" HeaderText="Age" />
                <asp:BoundField DataField="gender" HeaderText="Gender" />
                <asp:BoundField DataField="EnrolledOn" HeaderText="Enrolled On" 
                    DataFormatString="{0:dd MMM yyyy}" />
            </Columns>

        </asp:GridView>

        <asp:Label ID="lblMessage" runat="server" CssClass="message-label" />

        <br />
        <asp:Button ID="btnBack" runat="server" Text="Back to Courses"
            PostBackUrl="ViewCourses.aspx" CssClass="btn-back" OnClick="btnBack_Click" />

    </div>
    </form>
</body>
</html>
