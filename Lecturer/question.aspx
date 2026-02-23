<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="question.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.question" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ask Question</title>
    <link href="question.css" rel="stylesheet" />
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

            <div class="form-container">
                <h2>Ask a Question</h2>

                <div class="form-group">
                    <label>Title *</label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-input" />
                </div>

                <div class="form-group">
                    <label>Content *</label>
                    <asp:TextBox ID="txtContent" runat="server"
                        TextMode="MultiLine"
                        Rows="6"
                        CssClass="form-input" />
                </div>

                <div class="form-group">
                    <label>Tags (comma separated)</label>
                    <asp:TextBox ID="txtTags" runat="server" CssClass="form-input" />
                </div>

                <div class="form-group">
                    <label>Upload Document (PDF, DOCX, ZIP)</label>
                    <asp:FileUpload ID="fileUploadFile" runat="server" accept=".pdf,.docx,.zip" />
                </div>

                <div class="form-group">
                    <label>Upload Image (JPG, PNG)</label>
                    <asp:FileUpload ID="fileUploadImage" runat="server" accept=".jpg,.jpeg,.png" />
                </div>

                <div class="form-group">
                    <label>Video URL</label>
                    <asp:TextBox ID="txtVideoUrl" runat="server" CssClass="form-input" />
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnPost" runat="server" Text="Post Question"
                        CssClass="btn-primary"
                        OnClick="btnPost_Click" />

                    <asp:Button ID="btnCancel" runat="server" Text="Cancel"
                        CssClass="btn-secondary"
                        OnClick="btnCancel_Click" />
                </div>

                <asp:Label ID="lblMessage" runat="server" CssClass="error-message" />

            </div>

        </div>
    </div>
    </form>
</body>
</html>
