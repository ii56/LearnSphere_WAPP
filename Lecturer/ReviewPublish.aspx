<<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReviewPublish.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.ReviewPublish" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Review & Publish</title>
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

            <div class='profile-box <%= (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") ? "verified" : "not-verified" %>'>

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

            <asp:Button ID="btnLogout" runat="server"
                Text="Logout"
                CssClass="logout-btn"
                OnClick="btnLogout_Click" />

        </div>
    </div>


    <!-- MAIN CONTENT -->
    <div class="main-content">

        <h2>Review & Publish</h2>

        <!-- STEP INDICATOR -->
        <div class="step-indicator">

            <div class='step <%= (ViewState["Step"] != null && ViewState["Step"].ToString() == "1") ? "active" : "" %>'>
                <div class="circle">1</div>
                <span>Course Details</span>
            </div>

            <div class='step <%= (ViewState["Step"] != null && ViewState["Step"].ToString() == "2") ? "active" : "" %>'>
                <div class="circle">2</div>
                <span>Modules</span>
            </div>

            <div class='step <%= (ViewState["Step"] != null && ViewState["Step"].ToString() == "3") ? "active" : "" %>'>
                <div class="circle">3</div>
                <span>Lessons</span>
            </div>

            <div class='step <%= (ViewState["Step"] != null && ViewState["Step"].ToString() == "4") ? "active" : "" %>'>
                <div class="circle">4</div>
                <span>Publish</span>
            </div>

        </div>

        <div class="progress-bar-container">
    <div class="progress-bar-fill" style="width:100%"></div>
</div>


        <!-- REVIEW CARD -->
        <div class="review-card">

            <div class="review-header">
                <h3>Course Overview</h3>
                <span class="review-status">Ready to Publish</span>
            </div>

            <!-- COURSE INFO -->
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

                            <asp:Repeater ID="rptLessons" runat="server" DataSource='<%# Eval("Lessons") %>'>
                                <ItemTemplate>

                                    <div class="lesson-item">

                                        <span class="lesson-name">
                                            <%# Server.HtmlEncode(Eval("lessontitle").ToString()) %>
                                        </span>

                                        <span class="lesson-duration">
                                            <%# Server.HtmlEncode(Eval("duration").ToString()) %> mins
                                        </span>

                                    </div>

                                </ItemTemplate>
                            </asp:Repeater>

                        </div>

                        <!-- MODULE EXAM -->
                        <asp:Panel runat="server" Visible='<%# Eval("HasExam") %>'>

                            <div class="review-divider"></div>

                            <div class="module-exam">
                                <strong>Module Exam:</strong>
                                <%# Server.HtmlEncode(Eval("ExamTitle").ToString()) %>

                                <br />
                                Questions: <%# Server.HtmlEncode(Eval("QuestionCount").ToString()) %>
                            </div>

                        </asp:Panel>

                    </div>

                </ItemTemplate>
            </asp:Repeater>


            <div class="review-divider"></div>

            <!-- COURSE EXAM -->
            <asp:Panel ID="pnlCourseExam" runat="server" Visible="false">

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


            <!-- ACTION BUTTONS -->
            <div class="review-actions">

<asp:Button ID="btnPublish"
    runat="server"
    Text="Publish Course"
    CssClass="btn-modern"
    OnClick="btnPublish_Click"
OnClientClick="if(confirmPublish()){ this.value='Publishing...'; this.disabled=true; return true;} else {return false;}" />

                <asp:Button ID="btnBackToLessons"
                    runat="server"
                    Text="← Back to Lessons"
                    CssClass="btn-modern-secondary"
                    OnClick="btnBackToLessons_Click" />

            </div>

            <asp:Label ID="lblMessage" runat="server" />

        </div>

    </div>

</div>

</form>
    <script>
function confirmPublish() {
    return window.confirm("🚀 Publish Course?\n\nThis will make your course live.");
}
    </script>
</body>
</html>