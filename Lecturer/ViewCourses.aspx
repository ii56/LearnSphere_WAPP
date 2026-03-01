<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewCourses.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.ViewCourses" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Courses</title>
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

                <div class="page-header">
                    <h2>Your Courses</h2>
                    <a href="CreateCourse.aspx" class="btn-create">+ Create New Course</a>
                </div>

                <div class="courses-card">

                    <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" CssClass="courses-table" DataKeyNames="courseid" OnRowCommand="gvCourses_RowCommand">

                        <Columns>

                            <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                            <asp:BoundField DataField="category" HeaderText="Category" />
                            <asp:BoundField DataField="price" HeaderText="Price" />

                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span class='<%# Eval("statusText").ToString() == "Published" ? "badge-published" : "badge-draft" %>'>
                                        <%# Eval("statusText") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>

                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-edit" CommandName="EditCourse" CommandArgument="<%# Container.DataItemIndex %>" />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-delete" CommandName="DeleteCourse" CommandArgument="<%# Container.DataItemIndex %>" OnClientClick="return confirm('Delete this course?');" />
                                    <asp:Button ID="btnViewStudents" runat="server" Text="Students" CssClass="btn-students"  CommandName="ViewStudents" CommandArgument="<%# Container.DataItemIndex %>" />              
                                    <asp:Button ID="btnPreview" runat="server" Text="Preview" CssClass="btn-preview" CommandName="PreviewCourse" CommandArgument="<%# Container.DataItemIndex %>" Visible='<%# Eval("statusText").ToString() == "Published" %>' />

                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
            </div>
        </div>
        
    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
<script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    
    </form>
</body>
</html>
