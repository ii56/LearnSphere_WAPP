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

                    <div class="row">
                        <div class="col">
                            <label>Description</label>
                            <asp:TextBox ID="txtDescription" runat="server"
                                CssClass="modern-input"
                                TextMode="MultiLine"
                                Rows="4"
                                placeholder="Write a short description about yourself...">
                            </asp:TextBox>
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

                    <% if (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") { %>

                        <p style="color:#16a34a;font-weight:600;">
                            ✔ You are a verified lecturer.
                        </p>

                    <% } else { %>

                        <p class="sub-text">
                            Upload your teaching certifications to request lecturer status.
                        </p>

                    <% } %>

                    <hr />

                    <!-- Upload Section -->
                    <div class="verification-upload-section">

                        <asp:FileUpload 
                            ID="fuVerificationDoc" 
                            runat="server" 
                            CssClass="modern-input" />

                        <asp:Button 
                            ID="btnUploadVerification"
                            runat="server"
                            Text="Upload Document"
                            CssClass="btn-modern"
                            OnClick="btnUploadVerification_Click" />

                        <asp:Label 
                            ID="lblVerificationMsg" 
                            runat="server" 
                            ForeColor="Red" />

                    </div>

                    <hr />

                    <!-- Uploaded Documents -->
                    <h4>Your Uploaded Verification Documents</h4>

                    <asp:Repeater ID="rptVerificationDocs" runat="server">
                        <ItemTemplate>
                            <div class="verification-doc-item">
                                <a href='<%# ResolveUrl(Eval("fileurl").ToString()) %>'
                                   target="_blank"
                                   class="verification-link">
                                    📄 View Document
                                </a>

                                <span class="verification-date">
                                    <%# Eval("uploadtime", "{0:dd MMM yyyy HH:mm}") %>
                                </span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>

                <div class="form-card-modern" style="margin-top:30px;">
                    <h3>Profile Picture</h3>

                    <div class="profile-upload-section">
                        <div class="current-profile-preview">
                            <div class="profile-img-wrapper-large">
                                <img src='<%= ResolveUrl(Session["profileImage"] != null 
                                        ? Session["profileImage"].ToString() 
                                        : "~/images/default-user.png") %>'
                                     class='<%= Session["usertype"] != null && 
                                              Session["usertype"].ToString() == "Lecturer" 
                                              ? "profile-preview-img verified-glow" 
                                              : "profile-preview-img" %>' />

                            </div>
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
                        document.querySelector(".profile-preview-img").src = e.target.result;
                    };
                    reader.readAsDataURL(input.files[0]);
                }
            }
        </script>
</body>
</html>
