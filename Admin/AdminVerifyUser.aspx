<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminVerifyUser.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminVerifyUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verify User</title>
    <link href="~/Admin/AdminCSS.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="AdminDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="UserManagement.aspx" class="nav-item active">Verify Users</a>
                    <a href="CourseManagement.aspx" class="nav-item">Course Management</a>
                    <a href="Database.aspx" class="nav-item">Database</a>
                    <a href="AdminForums.aspx" class="nav-item">Forums</a>
                    <a href="AdminEditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="AdminSyslog.aspx" class="nav-item">Syslog</a>
                </div>
                <div class="sidebar-profile">
                    <div class="profile-box admin">
                        <div class="profile-img-wrapper">
                            <img id="sidebarImg" runat="server" class="profile-img" />
                            <div class="verification-badge">✔</div>
                        </div>
                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] %></div>
                            <div class="profile-status">administrator</div>
                        </div>
                    </div>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">
                <div class="dashboard-header">
                    <h2>Verify User Request</h2>
                    <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-text" />
                </div>

                <div class="verify-wrapper">
                    <div class="verify-card">
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

                            <div class="field-label">Remarks</div>
                            <div class="field-value description"><asp:Label ID="lblRemarks" runat="server" /></div>
                        </div>

                        <div class="action-buttons">
                            <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn-primary" OnClick="btnApprove_Click" />
                            <asp:Button ID="btnDecline" runat="server" Text="Decline" CssClass="btn-danger" OnClick="btnDecline_Click" />
                        </div>
                    </div>

                    <iframe id="docFrame" runat="server" class="document-frame"></iframe>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
