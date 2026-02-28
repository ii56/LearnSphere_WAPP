<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewStudents.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.ViewStudents" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Enrolled Students</title>
    <link href="ViewStudents.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="layout">
        <div class="sidebar">

                <div>
                    <div class="sidebar-title">LearnSphere</div>

                    <a href="LecturerDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="CreateCourse.aspx" class="nav-item">Create Course</a>
                    <a href="ViewCourses.aspx" class="nav-item active">View Courses</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="Forums.aspx" class="nav-item">Forums</a>
                </div>

                <div class="sidebar-profile">

                    <div class="profile-box <%= (Session["verified"] != null && (bool)Session["verified"]) ? "verified" : "not-verified" %>">

                        <div class="profile-img-wrapper">
                            <img id="imgSidebarProfile" runat="server" class="profile-img" />
                            <% if (Session["verified"] != null && (bool)Session["verified"]) { %>
                                <div class="verification-badge">✔</div>
                            <% } %>
                        </div>

                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] %></div>
                            <div class="profile-status">
                                <%= (Session["verified"] != null && (bool)Session["verified"]) ? "Verified Lecturer" : "Not Verified" %>
                            </div>
                        </div>
                    </div>

                    <a href="Message.aspx" class="nav-item message-link">
                        Messaging
                        <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                            <span class="message-badge"><%= Session["unreadCount"] %></span>
                        <% } %>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>

            </div>
            <div class="main-content">
    <div class="container">
        <div class="students-header">

    <h2>Enrolled Students</h2>

    <div class="students-actions">

        <asp:Button ID="btnBackTop"
            runat="server"
            Text="← Back"
            CssClass="btn-back"
            OnClick="btnBack_Click" />

        <asp:Button ID="btnExport"
            runat="server"
            Text="Export to Excel"
            CssClass="btn-export"
            OnClick="btnExport_Click" />

    </div>

</div>

<asp:GridView ID="gvStudents" runat="server"
    AutoGenerateColumns="False"
    CssClass="students-table"
    OnRowCommand="gvStudents_RowCommand"
    DataKeyNames="userid">

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

        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>

                <asp:Button ID="btnDelete" runat="server"
                    Text="Delete"
                    CssClass="btn-delete"
                    CommandName="DeleteStudent"
                    CommandArgument="<%# Container.DataItemIndex %>"
                    OnClientClick="return confirm('Remove this student from course?');" />

                <asp:Button ID="btnReceipt" runat="server"
                    Text="View Receipt"
                    CssClass="btn-receipt"
                    CommandName="ViewReceipt"
                    CommandArgument="<%# Container.DataItemIndex %>" />

            </ItemTemplate>
        </asp:TemplateField>

    </Columns>

</asp:GridView>
        <asp:Label ID="lblMessage" runat="server" CssClass="message-label" ForeColor="Red" />
    </div>
    </div>
        </div>
    </form>
</body>
</html>
