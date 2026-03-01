<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="editCourse.aspx.cs"
    Inherits="LearnSphere_WAPP.Lecturer.editCourse" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Course Content</title>
    <link href="courses.css" rel="stylesheet" />
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
                    <div class="profile-box <%= (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") ? "verified" : "not-verified" %>">
                        <div class="profile-img-wrapper">
                            <img id="imgSidebarProfile" runat="server" class="profile-img" />

                            <% if (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") { %>
                                <div class="verification-badge">✔</div>
                            <% } %>
                        </div>

                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] %></div>
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
                            <span class="message-badge"><%= Session["unreadCount"] %></span>
                        <% } %>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
    </div>

    <div class="main-content">
        <div class="course-info">
            <h2><asp:Label ID="lblCourseName" runat="server" /></h2>
            <p><asp:Label ID="lblCourseDescription" runat="server" /></p>
            <p><strong>Price:</strong> $<asp:Label ID="lblCoursePrice" runat="server" /></p>

            <asp:Button ID="btnAddModule" runat="server" Text="+ Add Module" CssClass="btn btn-primary" OnClick="btnAddModule_Click" />
        </div>

        <hr />

        <asp:Repeater ID="rptModules"
            runat="server"
            OnItemDataBound="rptModules_ItemDataBound" OnItemCommand="rptModules_ItemCommand">

            <ItemTemplate>

                <div class="module-card">

                    <h3><%# Eval("modulename") %></h3>

                    <div class="module-actions">
                        <asp:Button runat="server" Text="Edit Module" CssClass="btn btn-edit" CommandName="EditModule" CommandArgument='<%# Eval("moduleid") %>' OnCommand="Module_Command" />

                        <asp:Button runat="server" Text="Delete Module" CssClass="btn btn-delete" CommandName="DeleteModule" CommandArgument='<%# Eval("moduleid") %>' OnCommand="Module_Command" OnClientClick="return confirm('Delete this module and all its lessons?');" />

                        <asp:Button runat="server" Text="+ Add Lesson" CssClass="btn btn-secondary" CommandName="AddLesson" CommandArgument='<%# Eval("moduleid") %>' OnCommand="Module_Command" />
                    </div>

                    <asp:Repeater ID="rptLessons" runat="server">

                        <HeaderTemplate>
                            <table class="lesson-table">
                                <tr>
                                    <th>Lesson Title</th>
                                    <th>Actions</th>
                                </tr>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("lessontitle") %></td>
                                <td>
                                    <asp:Button runat="server" Text="Edit" CssClass="btn btn-edit" CommandName="EditLesson" CommandArgument='<%# Eval("lessonid") %>' OnCommand="Lesson_Command" />
                                    <asp:Button runat="server" Text="Delete" CssClass="btn btn-delete" CommandName="DeleteLesson" CommandArgument='<%# Eval("lessonid") %>' OnCommand="Lesson_Command" OnClientClick="return confirm('Delete this lesson?');" />
                                </td>
                            </tr>
                        </ItemTemplate>

                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>

                <hr />
            </ItemTemplate>
        </asp:Repeater>

        <div class="bottom-actions">
            <asp:Button ID="btnReview" runat="server" Text="Review & Update" CssClass="btn btn-success" OnClick="btnReview_Click" />
        </div>
    </div>
</div>
</form>
</body>
</html>
