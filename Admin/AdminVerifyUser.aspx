<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminVerifyUser.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminVerifyUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verify User</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <span class="verified-badge">Administrator</span>

                <div class="user-pill">
                        <div class="user-avatar">
                            <img id="sidebarImg" runat="server" />
                        </div>
                        <span class="user-name"><%= Session["uname"] %></span>
                    </div>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="nav">
                <a href="AdminDashboard.aspx" >Dashboard</a>
                <a href="UserManagement.aspx" class="active">User Management</a>
                <a href="CourseManagement.aspx" >Course Management</a>
                <a href="Database.aspx" >Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
                <a href="AdminSyslog.aspx" >Syslog</a>
                <a href="AdminMessage.aspx">
                    Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="nav-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                </a>
                <a href="../Chatbot/AdminChatbotKnowledge.aspx" >Chatbot</a>
            </div>

            <div class="container">
                <div class="welcome-banner">
                    <h2 class="welcome-label">Admin Portal</h2>
                    <h2 class="welcome-name">Verify User Request</h2>
                    <h3 class="welcome-sub">Approve or reject user's request</h3>
                </div>

                <div class="card verify-card">
                    <div class="course-header">
                        <h2>User Verification Details</h2>
                    </div>
                    <div class="verify-grid">
                        <div class="field-label">Request ID</div>
                        <div class="field-value"><asp:Label ID="lblRequestId" runat="server" /></div>

                        <div class="field-label">User ID</div>
                        <div class="field-value"><asp:Label ID="lblUserId" runat="server" /></div>

                        <div class="field-label">Current Role</div>
                        <div class="field-value"><asp:Label ID="lblCurrentRole" runat="server" /></div>

                        <div class="field-label">Requested Role</div>
                        <div class="field-value highlight"><asp:Label ID="lblRequestedRole" runat="server" /></div>

                        <div class="field-label">Request Time</div>
                        <div class="field-value"><asp:Label ID="lblRequestTime" runat="server" /></div>

                        <div class="field-label">Status</div>
                        <div class="field-value"><asp:Label ID="lblStatus" runat="server" /></div>

                        <div class="field-label">Reviewed Time</div>
                        <div class="field-value"><asp:Label ID="lblReviewTime" runat="server" /></div>

                        <div class="field-label">Reviewed By User Id</div>
                        <div class="field-value"><asp:Label ID="lblReviewBy" runat="server" /></div>

                        <div class="field-label">Remarks</div>
                        <div class="field-value">
                            <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="5"></asp:TextBox>
                        </div>
                    </div>

                    <div class="action-buttons" style="margin-top: 20px;">
                        <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn-primary" OnClick="btnApprove_Click" />
                        <asp:Button ID="btnDecline" runat="server" Text="Decline" CssClass="btn-danger" OnClick="btnDecline_Click" />
                    </div>
                </div>

                <div class="card pdf-card">
                    <div class="course-header">
                        <h2>Uploaded Document</h2>
                    </div>
                    <iframe id="docFrame" runat="server" class="document-frame"></iframe>
                </div>
            </div>
    </form>
</body>
</html>
