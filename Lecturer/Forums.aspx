<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forums.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.Forums" ValidateRequest="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forums</title>
    <link href="lecturer.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <div class="layout">

        <!-- SIDEBAR -->
        <div class="sidebar">
            <div>
                <div class="sidebar-title">LearnSphere</div>

                <a href="LecturerDashboard.aspx" class="nav-item">Dashboard</a>
                <a href="CreateCourse.aspx" class="nav-item">Create Course</a>
                <a href="ViewCourses.aspx" class="nav-item">View Courses</a>
                <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                <a href="Forums.aspx" class="nav-item active">Forums</a>
            </div>

            <div class="sidebar-profile">
                <div class="profile-box <%= (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") ? "verified" : "not-verified" %>">

                    <div class="profile-img-wrapper">
                        <img id="imgSidebarProfile" runat="server" class="profile-img" />

                        <% if (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") { %>
                            <div class="verification-badge">✔</div>
                        <% } %>
                    </div>

                    <div class="profile-info">
                        <div class="profile-name">
                            <%= Server.HtmlEncode(Session["uname"] != null ? Session["uname"].ToString() : "") %>
                        </div>

                        <div class="profile-status">
                            <%= (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer")
                                ? "Verified Lecturer"
                                : "General User" %>
                        </div>
                    </div>
                </div>

                <a href="Message.aspx" class="nav-item message-link">
                    Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="message-badge">
                            <%= Session["unreadCount"] %>
                        </span>
                    <% } %>
                </a>

                <asp:Button ID="btnLogout"
                            runat="server"
                            Text="Logout"
                            CssClass="logout-btn"
                            OnClick="btnLogout_Click" />
            </div>
        </div>

        <!-- MAIN CONTENT -->
        <div class="main-content">

            <h2>Manage Course Forums</h2>

            <!-- GRID -->
            <asp:GridView ID="gvCourses"
                          runat="server"
                          AutoGenerateColumns="False"
                          CssClass="forum-table"
                          OnRowCommand="gvCourses_RowCommand"
                          EmptyDataText="No courses available."
                          DataKeyNames="courseid">

                <Columns>

                    <asp:BoundField DataField="coursename"
                                    HeaderText="Course Name" />

                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>

                            <!-- CREATE -->
                            <asp:Button ID="btnCreate"
                                        runat="server"
                                        Text="Create Forum"
                                        CommandName="CreateForum"
                                        CommandArgument='<%# Eval("courseid") %>'
                                        CssClass="btn-primary"
                                        Visible='<%# !Convert.ToBoolean(Eval("HasForum")) %>' />

                            <!-- VIEW -->
                            <asp:Button ID="btnView"
                                        runat="server"
                                        Text="View Forum"
                                        CommandName="ViewForum"
                                        CommandArgument='<%# Eval("courseid") %>'
                                        CssClass="btn-view"
                                        Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>' />

                            <!-- DELETE -->
                            <asp:Button ID="btnDelete"
                                        runat="server"
                                        Text="Delete Forum"
                                        CommandName="DeleteForum"
                                        CommandArgument='<%# Eval("courseid") %>'
                                        CssClass="btn-danger"
                                        Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>'
                                        OnClientClick="return confirm('Are you sure you want to delete this forum?');" />

                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

            <!-- EMPTY STATE MESSAGE -->
            <asp:Label ID="lblMessage"
                       runat="server"
                       CssClass="empty-message"
                       ForeColor="Red" />

        </div>

    </div>

    <!-- CHATBOT -->
    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
    <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

</form>
</body>
</html>