<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateForum.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.CreateForum" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Forum</title>
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
                        <%= Session["uname"] %>
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

        <h2>Create Forum</h2>

        <div class="form-card">

            <!-- VALIDATION SUMMARY -->
            <asp:ValidationSummary
                ID="ValidationSummary1"
                runat="server"
                CssClass="validation-summary"
                HeaderText="Please fix the following errors:"
                ValidationGroup="forumForm" />



            <!-- FORUM TITLE -->
            <label>Forum Title *</label>

            <asp:TextBox
                ID="txtTitle"
                runat="server"
                CssClass="input-box"
                MaxLength="100">
            </asp:TextBox>

            <asp:RequiredFieldValidator
                ID="reqTitle"
                runat="server"
                ControlToValidate="txtTitle"
                ErrorMessage="Forum title is required."
                Display="Dynamic"
                CssClass="validation-error"
                ValidationGroup="forumForm" />

<asp:RegularExpressionValidator
    ID="regexTitle"
    runat="server"
    ControlToValidate="txtTitle"
    ValidationExpression="^[a-zA-Z0-9 _-]+$"
    ErrorMessage="Only letters, numbers, spaces, dash and underscore allowed."
    CssClass="validation-error"
    ValidationGroup="forumForm" />



            <!-- DESCRIPTION -->
            <label>Description *</label>

            <asp:TextBox
                ID="txtDescription"
                runat="server"
                TextMode="MultiLine"
                Rows="4"
                CssClass="input-box"
                MaxLength="1000">
            </asp:TextBox>

            <asp:RequiredFieldValidator
                ID="reqDescription"
                runat="server"
                ControlToValidate="txtDescription"
                ErrorMessage="Description is required."
                Display="Dynamic"
                CssClass="validation-error"
                ValidationGroup="forumForm" />



            <!-- TAGS -->
            <label>Allowed Tags (comma separated)</label>

            <asp:TextBox
                ID="txtTags"
                runat="server"
                CssClass="input-box"
                MaxLength="200">
            </asp:TextBox>

            <asp:RegularExpressionValidator
                ID="regexTags"
                runat="server"
                ControlToValidate="txtTags"
                ValidationExpression="^[a-zA-Z0-9\s,]*$"
                ErrorMessage="Tags can only contain letters, numbers and commas."
                Display="Dynamic"
                CssClass="validation-error"
                ValidationGroup="forumForm" />



            <!-- SUBMIT BUTTON -->
            <asp:Button
                ID="btnCreate"
                runat="server"
                Text="Create Forum"
                CssClass="btn-primary"
                ValidationGroup="forumForm"
                OnClick="btnCreate_Click" />

        </div>

    </div>

</div>

</form>

</body>
</html>