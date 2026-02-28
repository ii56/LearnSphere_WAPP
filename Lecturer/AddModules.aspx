<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddModules.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.AddModules" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Modules</title>
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
                <h2>
                    Add Modules - 
                    <asp:Label ID="lblCourseTitle" runat="server" />
                </h2>

                <div class="step-indicator">
                    <div class="step <%= ViewState["Step"]?.ToString() == "1" ? "active" : "" %>">
                        <div class="circle">1</div>
                        <span>Course Details</span>
                    </div>

                    <div class="step <%= ViewState["Step"]?.ToString() == "2" ? "active" : "" %>">
                        <div class="circle">2</div>
                        <span>Modules</span>
                    </div>

                    <div class="step <%= ViewState["Step"]?.ToString() == "3" ? "active" : "" %>">
                        <div class="circle">3</div>
                        <span>Lessons</span>
                    </div>

                    <div class="step <%= ViewState["Step"]?.ToString() == "4" ? "active" : "" %>">
                        <div class="circle">4</div>
                        <span>Publish</span>
                    </div>
                </div>

                <div class="form-card-modern">
                    <label>Module Name *</label>
                    <asp:TextBox ID="txtModuleName" runat="server" CssClass="modern-input"></asp:TextBox>

                    <label>Module Description</label>
                    <asp:TextBox ID="txtModuleDesc" runat="server" TextMode="MultiLine" Rows="3" CssClass="modern-input"></asp:TextBox>
                    <asp:Button ID="btnAddModule" runat="server" Text="Add Module" CssClass="btn-modern" OnClick="btnAddModule_Click" />
                    <asp:Label ID="lblMessage" runat="server" />
                </div>

                <br />

                <div class="courses-section">
                    <h3>Modules Added</h3>
                        <asp:GridView ID="gvModules" runat="server"  AutoGenerateColumns="False"  CssClass="modern-table" OnRowCommand="gvModules_RowCommand" DataKeyNames="moduleid">
                            <Columns>
                                <asp:BoundField DataField="modulename" HeaderText="Module Name" />
                                <asp:BoundField DataField="moduledescription" HeaderText="Description" />

                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:Button ID="btnAddLessons"  runat="server" Text="Add Lessons" CssClass="btn-modern-small" CommandName="AddLessons" CommandArgument='<%# Eval("moduleid") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    
                    <br />

                    <asp:Button ID="btnContinue" runat="server" Text="Continue to Lessons" CssClass="btn-modern" OnClick="btnContinue_Click" />
                    <br />
                    <asp:Button ID="btnBackToCourse" runat="server" Text="← Back to Course Details" CssClass="btn-modern-secondary" OnClick="btnBackToCourse_Click" Width="264px" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
