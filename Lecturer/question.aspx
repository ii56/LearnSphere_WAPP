<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="question.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.question" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ask Question</title>
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
            <a href="../Chatbot/Chatbot.aspx" class="nav-item">Chatbot</a>
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

                    <!-- SAFE USERNAME -->
                    <div class="profile-name">
                        <%= Session["uname"] != null ? Server.HtmlEncode(Session["uname"].ToString()) : "" %>
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
                    <span class="message-badge"><%= Session["unreadCount"] %></span>
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

        <div class="form-container">

            <h2>Ask a Question</h2>

            <!-- VALIDATION SUMMARY -->
            <asp:ValidationSummary
                ID="vsSummary"
                runat="server"
                ForeColor="Red"
                CssClass="validation-summary" />

            <!-- TITLE -->
            <div class="form-group">
                <label>Title *</label>

                <asp:TextBox ID="txtTitle"
                    runat="server"
                    CssClass="form-input"
                    MaxLength="150" />

                <asp:RequiredFieldValidator
                    ControlToValidate="txtTitle"
                    ErrorMessage="Title is required"
                    ForeColor="Red"
                    runat="server" />

                <asp:RegularExpressionValidator
                    ControlToValidate="txtTitle"
                    ValidationExpression="^[a-zA-Z0-9\s\-\?\!\.,]{3,150}$"
                    ErrorMessage="Invalid title format"
                    ForeColor="Red"
                    runat="server" />
            </div>

            <!-- CONTENT -->
            <div class="form-group">
                <label>Content *</label>

                <asp:TextBox ID="txtContent"
                    runat="server"
                    TextMode="MultiLine"
                    Rows="6"
                    CssClass="form-input"
                    MaxLength="2000" />

                <asp:RequiredFieldValidator
                    ControlToValidate="txtContent"
                    ErrorMessage="Content is required"
                    ForeColor="Red"
                    runat="server" />
            </div>

            <!-- TAGS -->
            <div class="form-group">
                <label>Tags (comma separated)</label>

                <asp:TextBox ID="txtTags"
                    runat="server"
                    CssClass="form-input"
                    MaxLength="200" />

                <asp:RegularExpressionValidator
                    ControlToValidate="txtTags"
                    ValidationExpression="^[a-zA-Z0-9,\s\-]*$"
                    ErrorMessage="Invalid tags format"
                    ForeColor="Red"
                    runat="server" />
            </div>

            <!-- DOCUMENT UPLOAD -->
            <div class="form-group">
                <label>Upload Document (PDF, DOCX, ZIP)</label>

                <asp:FileUpload ID="fileUploadFile"
                    runat="server"
                    accept=".pdf,.docx,.zip" />
            </div>

            <!-- IMAGE UPLOAD -->
            <div class="form-group">
                <label>Upload Image (JPG, PNG)</label>

                <asp:FileUpload ID="fileUploadImage"
                    runat="server"
                    accept=".jpg,.jpeg,.png" />
            </div>

            <!-- VIDEO URL -->
            <div class="form-group">
                <label>Video URL</label>

                <asp:TextBox ID="txtVideoUrl"
                    runat="server"
                    CssClass="form-input"
                    MaxLength="300" />

                <asp:RegularExpressionValidator
                    ControlToValidate="txtVideoUrl"
                    ValidationExpression="^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-\.~:\/?#[\]@!\$&'\(\)\*\+,;=]*)?$"
                    ErrorMessage="Invalid URL format"
                    ForeColor="Red"
                    runat="server" />
            </div>

            <!-- ACTIONS -->
            <div class="form-actions">

                <asp:Button ID="btnPost"
                    runat="server"
                    Text="Post Question"
                    CssClass="btn-primary"
                    OnClick="btnPost_Click" />

                <asp:Button ID="btnCancel"
                    runat="server"
                    Text="Cancel"
                    CssClass="btn-secondary"
                    OnClick="btnCancel_Click" />

            </div>

            <br />

            <asp:Label ID="lblMessage"
                runat="server"
                CssClass="error-message"
                ForeColor="Red" />

        </div>

    </div>

</div>

</form>

</body>
</html>