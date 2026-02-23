<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateCourse.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.CreateCourse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Course</title>
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
                    <h2>Create New Course</h2>
                    <span class="draft-label">Draft</span>
                </div>

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


                <div class="form-card-modern">
                    <h3>Course Basics</h3>
                    <p class="sub-text">Set up the fundamental information about your course</p>

                    <label>Course Title *</label>
                    <asp:TextBox ID="txtCourseName" runat="server" CssClass="modern-input"></asp:TextBox>

                    <label>Course Description *</label>
                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" CssClass="modern-input"></asp:TextBox>

                    <label>Course Thumbnail</label>
                    <div class="upload-box">
                        <p>Drag and drop an image here, or click to select</p>
                        <asp:FileUpload ID="fileThumbnail" runat="server" />
                    </div>

                    <div class="row">
                        <div class="col">
                            <label>Category *</label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="modern-input">
                                <asp:ListItem Text="AI" />
                                <asp:ListItem Text="Machine Learning" />
                                <asp:ListItem Text="Web Development" />
                                <asp:ListItem Text="Programming" />
                            </asp:DropDownList>
                        </div>

                        <div class="col">
                            <label>Level *</label>
                            <asp:DropDownList ID="ddlLevel" runat="server" CssClass="modern-input">
                                <asp:ListItem Text="Beginner" />
                                <asp:ListItem Text="Intermediate" />
                                <asp:ListItem Text="Advanced" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <label>Price (USD)</label>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="modern-input"></asp:TextBox>
                    <asp:Button ID="btnCreate" runat="server" Text="Add modules" CssClass="btn-modern" OnClick="btnCreate_Click" />
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
