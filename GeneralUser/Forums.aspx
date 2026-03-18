<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forums.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.Forums" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Forums - LearnSphere</title>
    <link href="Forums.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item">Browse Courses</a>
                    <a href="MyCourses.aspx" class="nav-item">My Learning</a>
                    <a href="Forums.aspx" class="nav-item active">Course Forums</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                </div>

                <div class="sidebar-profile">
                    <div class="profile-box not-verified">
                        <div class="profile-img-wrapper">
                            <img id="imgSidebarProfile" runat="server" class="profile-img" src="~/images/default-user.png" />
                        </div>

                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] != null ? Session["uname"].ToString() : "Guest" %></div>
                            <div class="profile-status">General User</div>
                        </div>
                    </div>

                    <a href="Message.aspx" class="nav-item message-link">
                        Messages
                        <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                            <span class="message-badge"><%= Session["unreadCount"] %></span>
                        <% } %>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">
                <h2 style="margin-bottom: 25px; color: #0f172a;">My Enrolled Course Forums</h2>
                
                <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" CssClass="forum-table" OnRowCommand="gvCourses_RowCommand" EmptyDataText="You are not enrolled in any courses yet.">
                    <Columns>
                        <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                        
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:Button ID="btnView" runat="server" Text="Enter Forum" 
                                    CommandName="ViewForum" 
                                    CommandArgument='<%# Eval("courseid") %>' 
                                    CssClass="btn-view" 
                                    Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>' />
                                
                                <asp:Label ID="lblNoForum" runat="server" Text="No Forum Available" 
                                    ForeColor="#64748b" Font-Italic="true" 
                                    Visible='<%# !Convert.ToBoolean(Eval("HasForum")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
               </asp:GridView>
            </div>
        </div>
        
        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>
</body>
</html>