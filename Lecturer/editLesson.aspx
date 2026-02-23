<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editLesson.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.editLesson" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Lessons</title>
    <link href="CreateCourse.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="LecturerDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="CreateCourse.aspx" class="nav-item active">Create Course</a>
                    <a href="ViewCourses.aspx" class="nav-item">View Courses</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="../Chatbot/Chatbot.aspx" class="nav-item">Chatbot</a>
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

                <div class="course-header">
                    <h2>
                        Edit Lesson - 
                        <asp:Label ID="lblModuleName" runat="server"></asp:Label>
                    </h2>
                </div>

                <div class="form-card-modern">

                    <label>Lesson Title *</label>
                    <asp:TextBox ID="txtLessonTitle" runat="server"  CssClass="modern-input"></asp:TextBox>

                    <label>Lesson Description</label>
                    <asp:TextBox ID="txtLessonDesc"  runat="server"  TextMode="MultiLine"  Rows="3"  CssClass="modern-input"></asp:TextBox>

                    <label>Video URL</label>
                    <asp:TextBox ID="txtVideoUrl"  runat="server"  CssClass="modern-input"></asp:TextBox>

                    <label>Upload File (PDF, DOC, etc.)</label>
                    <asp:FileUpload ID="fuLessonFile"  runat="server"  CssClass="modern-input" />

                    <label>Duration (minutes)</label>
                    <asp:TextBox ID="txtDuration"  runat="server"  CssClass="modern-input"></asp:TextBox>

                    <br /><br />

                    <asp:Button ID="btnUpdateModule"  runat="server"  Text="Update and Continue" CssClass="btn-modern"  OnClick="btnUpdateModule_Click" />

                    &nbsp;&nbsp;

                    <asp:Button ID="btnCancel"  runat="server"  Text="Cancel and Continue"  CssClass="btn-modern-secondary"  OnClick="btnCancel_Click" />

                    <br /><br />

                    <asp:Label ID="lblMessage" runat="server"  ForeColor="Red" />
                </div>

            </div>
        </div>
    </form>
</body>
</html>