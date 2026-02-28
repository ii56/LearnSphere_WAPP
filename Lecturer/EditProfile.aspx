<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditProfile.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.EditProfile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Profile</title>
    <link href="EditProfile.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="LecturerDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="CreateCourse.aspx" class="nav-item">Create Course</a>
                    <a href="ViewCourses.aspx" class="nav-item">View Courses</a>
                    <a href="EditProfile.aspx" class="nav-item active">Edit Profile</a>
                    <a href="Forums.aspx" class="nav-item">Forums</a>
                </div>

                <div class="sidebar-profile">
                    <div class="profile-box <%= (Session["verified"] != null && (bool)Session["verified"]) ? "verified" : "not-verified" %>">
                        <div class="profile-img-wrapper">
                            <img src='<%= ResolveUrl(Session["profileImage"] != null ? Session["profileImage"].ToString() : "~/images/default-user.png") %>' class="profile-img" />
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
                <div class="profile-header">
                    <h2>Profile Settings</h2>
                    <p class="sub-text">Manage your personal and account information</p>
                </div>

                <div class="form-card-modern">
                    <h3>Personal Information</h3>
                    <div class="row">
                        <div class="col">
                            <label>Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="modern-input" Enabled="false"></asp:TextBox>
                        </div>

                        <div class="col">
                            <label>Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="modern-input"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col">
                            <label>First Name</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="modern-input"></asp:TextBox>
                        </div>

                        <div class="col">
                            <label>Last Name</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="modern-input"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col">
                            <label>Age</label>
                            <asp:TextBox ID="txtAge" runat="server" CssClass="modern-input"></asp:TextBox>
                        </div>

                        <div class="col">
                            <label>Gender</label>
                            <asp:DropDownList ID="ddlGender" runat="server" CssClass="modern-input">
                                <asp:ListItem Text="Male" />
                                <asp:ListItem Text="Female" />
                                <asp:ListItem Text="Other" />
                            </asp:DropDownList>
                        </div>
                    </div>

                </div>

                <div class="form-card-modern" style="margin-top:30px;">
                    <h3>Account Security</h3>
                    <label>New Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="modern-input" placeholder="Leave blank to keep current password"></asp:TextBox>
                </div>

                <div class="form-card-modern" style="margin-top:30px;">
                    <h3>Verification</h3>
                    <asp:Panel ID="pnlNotVerified" runat="server" Visible='<%# !(Session["verified"] != null && (bool)Session["verified"]) %>'>
                        <p class="sub-text">Your account is not verified. Verified lecturers receive higher trust visibility.</p>
                        <asp:Button ID="btnRequestVerification" runat="server" Text="Request Verification" CssClass="btn-modern-secondary" OnClick="btnRequestVerification_Click" />
                    </asp:Panel>

                    <asp:Panel ID="pnlVerified" runat="server" Visible='<%# (Session["verified"] != null && (bool)Session["verified"]) %>'>
                        <p style="color:#16a34a;font-weight:600;">✔ Your account is verified.</p>
                    </asp:Panel>
                </div>

                <div class="form-card-modern" style="margin-top:30px;">
                    <h3>Profile Picture</h3>

                    <div class="profile-upload-section">
                        <div class="current-profile-preview">
                            <img id="imgPreview" runat="server"
                                 class="profile-preview-img" />
                        </div>

                        <div class="upload-controls">
                            <asp:FileUpload ID="fuProfileImage" runat="server" CssClass="modern-input" onchange="previewImage(this)"  />
                            <asp:Label ID="lblUploadMessage" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                </div>

                <div style="margin-top:30px;">
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-modern" OnClick="btnSave_Click" />
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
            </div>
        </div>
        
    <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
<script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    
    </form>
    <script>
        function previewImage(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    document.getElementById('<%= imgPreview.ClientID %>').src = e.target.result;
                };

                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</body>
</html>
