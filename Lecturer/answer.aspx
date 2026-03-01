<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="answer.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.answer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Post Answer</title>
    <link href="question.css" rel="stylesheet" />
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

            <div class="form-container">
                <h2>Answer Question</h2>

                <div class="question-preview-box">
                    <asp:Label ID="lblQuestionTitle" runat="server" CssClass="question-title-preview" />
                    <asp:Label ID="lblQuestionContent" runat="server" CssClass="question-content-preview" />
                </div>

                <hr style="margin:20px 0;" />

                <div class="form-group">
                    <label>Your Answer *</label>
                    <asp:TextBox ID="txtAnswer" runat="server"
                        TextMode="MultiLine"
                        Rows="8"
                        CssClass="form-input" />
                </div>

                <div class="form-group">
                    <label>Upload Document (PDF, DOCX, ZIP)</label>
                    <asp:FileUpload ID="fileUploadFile" runat="server"
                        accept=".pdf,.docx,.zip" />
                </div>

                <div class="form-group">
                    <label>Upload Image (JPG, PNG)</label>
                    <asp:FileUpload ID="fileUploadImage" runat="server"
                        accept=".jpg,.jpeg,.png" />
                </div>

                <div class="form-group">
                    <label>Video URL</label>
                    <asp:TextBox ID="txtVideoUrl" runat="server"
                        CssClass="form-input" />
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnPostAnswer" runat="server"
                        Text="Post Answer"
                        CssClass="btn-primary"
                        OnClick="btnPostAnswer_Click" />

                    <asp:Button ID="btnCancel" runat="server"
                        Text="Cancel"
                        CssClass="btn-secondary"
                        OnClick="btnCancel_Click" />
                </div>

                <asp:Label ID="lblMessage" runat="server"
                    CssClass="error-message" />

            </div>

        </div>
        </div>
    </form>
</body>
</html>
