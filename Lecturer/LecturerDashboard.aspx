<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LecturerDashboard.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.LecturerDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lecturer Dashboard</title>
    <link href="LecturerDashboard.css?v=2" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="LecturerDashboard.aspx" class="nav-item active">Dashboard</a>
                    <a href="CreateCourse.aspx" class="nav-item">Create Course</a>
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
                <div class="dashboard-header">
                    <h2>Dashboard Overview</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>

                <div class="stats-grid">

                    <div class="stat-card blue">
                        <div class="stat-title">Total Courses</div>
                        <asp:Label ID="lblTotalCourses" runat="server" CssClass="stat-number" />
                    </div>

                    <div class="stat-card green">
                        <div class="stat-title">Total Students</div>
                        <asp:Label ID="lblTotalStudents" runat="server" CssClass="stat-number" />
                    </div>

                    <div class="stat-card purple">
                        <div class="stat-title">Paid Courses</div>
                        <asp:Label ID="lblPaidCourses" runat="server" CssClass="stat-number" />
                    </div>

                    <div class="stat-card orange">
                        <div class="stat-title">Free Courses</div>
                        <asp:Label ID="lblFreeCourses" runat="server" CssClass="stat-number" />
                    </div>

                </div>

                <div class="courses-section">
                    <h3>Top 5 Recent Courses</h3>
                    <p class="section-sub">Latest courses you created</p>

                    <asp:GridView ID="gvTopCourses" runat="server" AutoGenerateColumns="False" CssClass="modern-table" EmptyDataText="No courses found.">
                        <Columns>
                            <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                            <asp:BoundField DataField="category" HeaderText="Category" />
                            <asp:BoundField DataField="price" HeaderText="Price" DataFormatString="RM {0:N2}" />
                            <asp:BoundField DataField="creationtime" HeaderText="Created Date" DataFormatString="{0:dd MMM yyyy}" />
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="quick-actions-section">
                    <h3>Quick Actions</h3>
                    <div class="quick-card-grid">
                        <a href="CreateCourse.aspx" class="quick-card">
                            <div class="quick-card-title">Create New Course</div>
                            <div class="quick-card-desc">Add a new course to the platform</div>
                        </a>

                        <a href="ViewCourses.aspx" class="quick-card">
                            <div class="quick-card-title">View All Courses</div>
                            <div class="quick-card-desc">Manage and edit your courses</div>
                        </a>

                        <a href="EditProfile.aspx" class="quick-card">
                            <div class="quick-card-title">Edit Profile</div>
                            <div class="quick-card-desc">Update your personal details</div>
                        </a>

                        <a href="../Chatbot/Chatbot.aspx" class="quick-card">
                            <div class="quick-card-title">Go to Chatbot</div>
                            <div class="quick-card-desc">Ask questions and get assistance</div>
                        </a>

                        <a href="MessageBox.aspx" class="quick-card">
                            <div class="quick-card-title">Message Box</div>
                            <div class="quick-card-desc">View your messages</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
        
    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
<script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    
    </form>
</body>
</html>
