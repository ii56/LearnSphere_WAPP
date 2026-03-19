<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateCourse.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.CreateCourse" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Course</title>
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

        <!-- HEADER (RESTORED) -->
        <div class="course-header">
            <h2>Create New Course</h2>
            <span class="draft-label">Draft</span>
        </div>

        <!-- STEP INDICATOR (RESTORED) -->
        <div class="step-indicator">

            <div class="step active">
                <div class="circle">1</div>
                <span>Course Details</span>
            </div>

            <div class="step">
                <div class="circle">2</div>
                <span>Modules</span>
            </div>

            <div class="step">
                <div class="circle">3</div>
                <span>Lessons</span>
            </div>

            <div class="step">
                <div class="circle">4</div>
                <span>Publish</span>
            </div>

        </div>


        <!-- FORM -->
        <div class="form-card-modern">

            <h3>Course Basics</h3>
            <p class="sub-text">Set up the fundamental information about your course</p>

            <!-- VALIDATION SUMMARY -->
            <asp:ValidationSummary
                ID="vsSummary"
                runat="server"
                CssClass="validation-summary"
                HeaderText="Please fix the following errors:" />

            <!-- COURSE NAME -->
            <label>Course Title *</label>

            <asp:TextBox ID="txtCourseName"
                runat="server"
                CssClass="modern-input"
                MaxLength="100" />

            <asp:RequiredFieldValidator
                ControlToValidate="txtCourseName"
                ErrorMessage="Course title is required"
                CssClass="validation-error"
                runat="server" />

            <asp:RegularExpressionValidator
                ControlToValidate="txtCourseName"
ValidationExpression="^[a-zA-Z0-9\s\-\+\#\(\)\.]{3,100}$"
                ErrorMessage="Invalid course title"
                CssClass="validation-error"
                runat="server" />


            <!-- DESCRIPTION -->
            <label>Course Description *</label>

            <asp:TextBox ID="txtDescription"
                runat="server"
                TextMode="MultiLine"
                Rows="5"
                CssClass="modern-input"
                MaxLength="1000" />

            <asp:RequiredFieldValidator
                ControlToValidate="txtDescription"
                ErrorMessage="Description is required"
                CssClass="validation-error"
                runat="server" />


            <!-- CATEGORY + LEVEL -->
            <div class="row">

                <div class="col">
                    <label>Category *</label>

                    <asp:DropDownList ID="ddlCategory"
                        runat="server"
                        CssClass="modern-input">

                        <asp:ListItem Text="Select Category" Value=""></asp:ListItem>
                        <asp:ListItem Text="AI" />
                        <asp:ListItem Text="Machine Learning" />
                        <asp:ListItem Text="Web Development" />
                        <asp:ListItem Text="Programming" />

                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ControlToValidate="ddlCategory"
                        InitialValue=""
                        ErrorMessage="Select a category"
                        CssClass="validation-error"
                        runat="server" />
                </div>


                <div class="col">
                    <label>Level *</label>

                    <asp:DropDownList ID="ddlLevel"
                        runat="server"
                        CssClass="modern-input">

                        <asp:ListItem Text="Beginner" />
                        <asp:ListItem Text="Intermediate" />
                        <asp:ListItem Text="Advanced" />

                    </asp:DropDownList>
                </div>

            </div>


            <!-- PRICE -->
            <label>Price (USD)</label>

            <asp:TextBox ID="txtPrice"
                runat="server"
                CssClass="modern-input"
                MaxLength="10" />

            <asp:RangeValidator
                ControlToValidate="txtPrice"
                MinimumValue="0"
                MaximumValue="10000"
                Type="Double"
                ErrorMessage="Price must be between 0 and 10000"
                CssClass="validation-error"
                runat="server" />


            <!-- BUTTON -->
            <asp:Button ID="btnCreate"
                runat="server"
                Text="Add Modules"
                CssClass="btn-modern"
                OnClick="btnCreate_Click" />

            <asp:Label ID="lblMessage" runat="server" />

        </div>

    </div>

</div>

</form>

<!-- BOTPRESS -->
<script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
<script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

</body>
</html>