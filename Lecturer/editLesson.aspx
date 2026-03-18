<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editLesson.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.editLesson" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Lessons</title>
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
                        <span class="message-badge">
                            <%= Session["unreadCount"] %>
                        </span>
                    <% } %>
                </a>

                <asp:Button
                    ID="btnLogout"
                    runat="server"
                    Text="Logout"
                    CssClass="logout-btn"
                    OnClick="btnLogout_Click" />
            </div>

        </div>


        <!-- MAIN CONTENT -->
        <div class="main-content">

            <div class="course-header">
                <h2>
                    Edit Lesson -
                    <asp:Label ID="lblModuleName" runat="server"></asp:Label>
                </h2>
            </div>

            <div class="form-card-modern">

                <!-- VALIDATION SUMMARY -->
                <asp:ValidationSummary
                    ID="ValidationSummary1"
                    runat="server"
                    CssClass="validation-summary"
                    HeaderText="Please fix the following errors:"
                    ValidationGroup="lessonForm" />


                <!-- LESSON TITLE -->
                <label>Lesson Title *</label>

                <asp:TextBox
                    ID="txtLessonTitle"
                    runat="server"
                    CssClass="modern-input"
                    MaxLength="100" />

                <asp:RequiredFieldValidator
                    ID="reqLessonTitle"
                    runat="server"
                    ControlToValidate="txtLessonTitle"
                    ErrorMessage="Lesson title is required."
                    CssClass="validation-error"
                    ValidationGroup="lessonForm" />


                <!-- DESCRIPTION -->
                <label>Lesson Description</label>

                <asp:TextBox
                    ID="txtLessonDesc"
                    runat="server"
                    TextMode="MultiLine"
                    Rows="3"
                    CssClass="modern-input"
                    MaxLength="1000" />


                <!-- VIDEO URL -->
                <label>Video URL</label>

                <asp:TextBox
                    ID="txtVideoUrl"
                    runat="server"
                    CssClass="modern-input"
                    MaxLength="500" />

                <asp:RegularExpressionValidator
                    ID="regexVideoUrl"
                    runat="server"
                    ControlToValidate="txtVideoUrl"
                    ValidationExpression="^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-\.~:\/?#[\]@!$&amp;'()*+,;=]*)?$"
                    ErrorMessage="Invalid video URL format."
                    CssClass="validation-error"
                    ValidationGroup="lessonForm" />


                <!-- FILE UPLOAD -->
                <label>Upload File (PDF, DOC, etc.)</label>

                <asp:FileUpload
                    ID="fuLessonFile"
                    runat="server"
                    CssClass="modern-input" />

                <asp:RegularExpressionValidator
                    ID="regexFile"
                    runat="server"
                    ControlToValidate="fuLessonFile"
                    ValidationExpression="^.*\.(pdf|doc|docx|ppt|pptx)$"
                    ErrorMessage="Only PDF, DOC, DOCX, PPT, PPTX files allowed."
                    CssClass="validation-error"
                    ValidationGroup="lessonForm" />


                <!-- DURATION -->
                <label>Duration (minutes) *</label>

                <asp:TextBox
                    ID="txtDuration"
                    runat="server"
                    CssClass="modern-input"
                    MaxLength="4" />

                <asp:RequiredFieldValidator
                    ID="reqDuration"
                    runat="server"
                    ControlToValidate="txtDuration"
                    ErrorMessage="Duration is required."
                    CssClass="validation-error"
                    ValidationGroup="lessonForm" />

                <asp:RegularExpressionValidator
                    ID="regexDuration"
                    runat="server"
                    ControlToValidate="txtDuration"
                    ValidationExpression="^\d+$"
                    ErrorMessage="Duration must be a number."
                    CssClass="validation-error"
                    ValidationGroup="lessonForm" />

                <asp:RangeValidator
                    ID="rangeDuration"
                    runat="server"
                    ControlToValidate="txtDuration"
                    MinimumValue="1"
                    MaximumValue="600"
                    Type="Integer"
                    ErrorMessage="Duration must be between 1 and 600 minutes."
                    CssClass="validation-error"
                    ValidationGroup="lessonForm" />


                <!-- BUTTONS -->
                <br /><br />

                <asp:Button
                    ID="btnUpdateModule"
                    runat="server"
                    Text="Update and Continue"
                    CssClass="btn-modern"
                    ValidationGroup="lessonForm"
                    OnClick="btnUpdateModule_Click" />

                &nbsp;&nbsp;

                <asp:Button
                    ID="btnCancel"
                    runat="server"
                    Text="Cancel and Continue"
                    CssClass="btn-modern-secondary"
                    OnClick="btnCancel_Click" />

                <br /><br />

                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="validation-error" />

            </div>

        </div>

    </div>

</form>

</body>
</html>