<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="editPublish.aspx.cs"
    Inherits="LearnSphere_WAPP.Lecturer.editPublish" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit and Publish</title>
    <link href="CreateCourse.css" rel="stylesheet" />
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

        <h2>Review & Publish</h2>

        <!-- VALIDATION SUMMARY -->
        <asp:ValidationSummary
            ID="ValidationSummary1"
            runat="server"
            CssClass="validation-summary"
            HeaderText="Please fix the following issues:" />

        <div class="review-card">

            <!-- COURSE INFO -->
            <div class="review-header">
                <h3>Course Overview</h3>
                <span class="review-status">Ready to Publish</span>
            </div>

            <div class="review-course-info">
                <div class="review-title">
                    <asp:Label ID="lblCourseName" runat="server" />
                </div>

                <div class="review-description">
                    <asp:Label ID="lblCourseDesc" runat="server" />
                </div>

                <div class="review-price">
                    <asp:Label ID="lblCoursePrice" runat="server" />
                </div>
            </div>


            <div class="review-divider"></div>

            <h3 class="review-section-title">Modules & Lessons</h3>

            <!-- MODULES -->
            <asp:Repeater ID="rptModules" runat="server">
                <ItemTemplate>

                    <div class="module-card">

                        <div class="module-title">
                            <%# Server.HtmlEncode(Eval("modulename").ToString()) %>
                        </div>

                        <div class="lesson-list">

                            <asp:Repeater ID="rptLessons"
                                runat="server"
                                DataSource='<%# Eval("Lessons") %>'>

                                <ItemTemplate>
                                    <div class="lesson-item">

                                        <span class="lesson-name">
                                            <%# Server.HtmlEncode(Eval("lessontitle").ToString()) %>
                                        </span>

                                        <span class="lesson-duration">
                                            <%# Eval("duration") %> mins
                                        </span>

                                    </div>
                                </ItemTemplate>

                            </asp:Repeater>

                        </div>

                    </div>

                </ItemTemplate>
            </asp:Repeater>


            <!-- COURSE EXAM -->
            <asp:Panel ID="pnlCourseExam" runat="server" Visible="false">

                <div class="review-divider"></div>

                <h3 class="review-section-title">Course Exam</h3>

                <div class="module-card">

                    <div class="module-title">
                        <asp:Label ID="lblCourseExamTitle" runat="server" />
                    </div>

                    <div class="lesson-item">
                        Total Questions:
                        <asp:Label ID="lblCourseExamQuestions" runat="server" />
                    </div>

                </div>

            </asp:Panel>


            <!-- ACTIONS -->
            <div class="review-actions">

                <asp:Button ID="btnPublish"
                    runat="server"
                    Text="Publish Course"
                    CssClass="btn-modern"
                    OnClick="btnPublish_Click"
                    OnClientClick="return confirm('Are you sure you want to publish this course?');" />

                <asp:Button ID="btnBack"
                    runat="server"
                    Text="← Back to Edit"
                    CssClass="btn-modern-secondary"
                    OnClick="btnBack_Click" />

            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="validation-error" />

        </div>
    </div>

</div>

</form>
</body>
</html>