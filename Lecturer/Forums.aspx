<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forums.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.Forums" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forums</title>
    <link href="Forums.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>

                    <a href="LecturerDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="CreateCourse.aspx" class="nav-item">Create Course</a>
                    <a href="ViewCourses.aspx" class="nav-item">View Courses</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="../Chatbot/Chatbot.aspx" class="nav-item">Chatbot</a>
                    <a href="Forums.aspx" class="nav-item active">Forums</a>
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
                <h2>Manage Course Forums</h2>
                    <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" CssClass="forum-table" OnRowCommand="gvCourses_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button ID="btnCreate" runat="server" Text="Create Forum" CommandName="CreateForum" CommandArgument='<%# Eval("courseid") %>' CssClass="btn-primary" Visible='<%# !Convert.ToBoolean(Eval("HasForum")) %>' />
                                    <asp:Button ID="btnView" runat="server" Text="View Forum" CommandName="ViewForum" CommandArgument='<%# Eval("courseid") %>' CssClass="btn-view" Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete Forum" CommandName="DeleteForum" CommandArgument='<%# Eval("courseid") %>' CssClass="btn-danger" Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                   </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
