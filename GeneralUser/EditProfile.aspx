<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditProfile.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.EditProfile" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Profile - LearnSphere</title>
    <link href="Forums.css" rel="stylesheet" />
    <link href="EditProfile.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item">Browse Courses</a>
                    <a href="MyCourse.aspx" class="nav-item">My Learning</a>
                    <a href="Forums.aspx" class="nav-item">Course Forums</a>
                    <a href="EditProfile.aspx" class="nav-item active">Edit Profile</a>
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
                <h2 style="margin-bottom: 25px; color: #0f172a;">Profile Settings</h2>

                <div class="profile-card">
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
                            <asp:TextBox ID="txtAge" runat="server" CssClass="modern-input" TextMode="Number"></asp:TextBox>
                        </div>
                        <div class="col">
                            <label>Gender</label>
                            <asp:DropDownList ID="ddlGender" runat="server" CssClass="modern-input">
                                <asp:ListItem Text="Male" Value="Male" />
                                <asp:ListItem Text="Female" Value="Female" />
                                <asp:ListItem Text="Other" Value="Other" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col">
                            <label>Bio / Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="modern-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="profile-card">
                    <h3>Account Security</h3>
                    <label>New Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="modern-input" TextMode="Password" placeholder="Leave blank to keep current"></asp:TextBox>
                </div>

                <div class="profile-card">
                    <h3>Profile Picture</h3>
                    <div class="profile-upload-section">
                        <img id="imgLargePreview" runat="server" class="profile-preview-img" src="~/images/default-user.png" />
                        <div class="upload-controls">
                            <asp:FileUpload ID="fuProfileImage" runat="server" CssClass="modern-input" onchange="previewImage(this)" />
                            <asp:Label ID="lblUploadMessage" runat="server" ForeColor="Red" Font-Size="Small"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="profile-card">
                    <h3>Upgrade to Lecturer</h3>
                    <p class="sub-text">Upload your teaching certifications (PDF only) to request an upgrade.</p>
                    <div class="row" style="align-items: center;">
                        <div class="col">
                            <asp:FileUpload ID="fuVerificationDoc" runat="server" CssClass="modern-input" />
                        </div>
                        <div class="col">
                            <asp:Button ID="btnUploadVerification" runat="server" Text="Submit Request" CssClass="btn-modern" OnClick="btnUploadVerification_Click" />
                        </div>
                    </div>
                    <asp:Label ID="lblVerificationMsg" runat="server"></asp:Label>
                </div>

                <div style="margin-top: 30px; display: flex; align-items: center; gap: 20px;">
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-save-main" OnClick="btnSave_Click" />
                    <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
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
                    document.getElementById('<%= imgLargePreview.ClientID %>').src = e.target.result;
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</body>
</html>