<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminEditProfile.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminEditProfile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Profile</title>
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
                <a href="UserManagement.aspx" >User Management</a>
                <a href="CourseManagement.aspx" >Course Management</a>
                <a href="Database.aspx" >Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" class="active">Edit Profile</a>
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
                        <h2 class="welcome-name">Edit Profile</h2>
                    <h3 class="welcome-sub">You can edit your profile and password here</h3>
                </div>

                <asp:Panel runat="server" DefaultButton="btnSave">
                <div class="card">
                    <div class="card-title">
                        <span class="title-dot dot-green"></span>
                        Edit Profile
                    </div>

                    <div class="form-grid">
                        <div class="form-group">
                            <label class="form-label">User Id</label>
                            <asp:TextBox ID="txtUserid" runat="server" CssClass="form-input" Enabled="false" />
                        </div>

                        <div class="form-group">
                            <label class="form-label">Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input" Enabled="false" />
                        </div>

                        <!-- First Name -->
                        <div class="form-group">
                            <asp:Label runat="server" Text="First Name:" CssClass="form-label" />
                            <asp:TextBox ID="txtFname" runat="server" CssClass="form-input" />

                            <asp:RequiredFieldValidator 
                                ControlToValidate="txtFname"
                                ErrorMessage="First name required."
                                CssClass="validation-error"
                                ValidationGroup="profileGroup"
                                runat="server" />
                        </div>

                        <!-- Last Name -->
                        <div class="form-group">
                            <asp:Label runat="server" Text="Last Name:" CssClass="form-label" />
                            <asp:TextBox ID="txtLname" runat="server" CssClass="form-input" />

                            <asp:RequiredFieldValidator 
                                ControlToValidate="txtLname"
                                ErrorMessage="Last name required."
                                CssClass="validation-error"
                                ValidationGroup="profileGroup"
                                runat="server" />
                        </div>

                        <!-- Email -->
                        <div class="form-group">
                            <asp:Label runat="server" Text="Email:" CssClass="form-label" />
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input" />

                            <asp:RequiredFieldValidator 
                                ControlToValidate="txtEmail"
                                ErrorMessage="Email is required."
                                CssClass="validation-error"
                                ValidationGroup="profileGroup"
                                runat="server" />

                            <asp:RegularExpressionValidator 
                                ControlToValidate="txtEmail"
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                ErrorMessage="Invalid email format."
                                CssClass="validation-error"
                                ValidationGroup="profileGroup"
                                runat="server" />
                        </div>

                        <!-- Age -->
                        <div class="form-group">
                            <asp:Label runat="server" Text="Age:" CssClass="form-label" />
                            <asp:TextBox ID="txtAge" runat="server" CssClass="form-input" />

                            <asp:RequiredFieldValidator 
                                ControlToValidate="txtAge"
                                ErrorMessage="Age required."
                                CssClass="validation-error"
                                ValidationGroup="profileGroup"
                                runat="server" />

                            <asp:RangeValidator 
                                ControlToValidate="txtAge"
                                MinimumValue="13"
                                MaximumValue="120"
                                Type="Integer"
                                ErrorMessage="Age must be between 13 and 120."
                                CssClass="validation-error"
                                ValidationGroup="profileGroup"
                                runat="server" />
                        </div>

                        <!-- Gender -->
                        <div class="form-group">
                            <asp:Label runat="server" Text="Gender:" CssClass="form-label" />
                            <asp:DropDownList ID="dropdownGender" runat="server" CssClass="form-input">
                                <asp:ListItem Value="">-- Select Gender --</asp:ListItem>
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>

                            <asp:RequiredFieldValidator 
                                ControlToValidate="dropdownGender"
                                InitialValue=""
                                ErrorMessage="Please select gender."
                                CssClass="validation-error"
                                ValidationGroup="profileGroup"
                                runat="server" />
                        </div>

                        <div class="form-group">
                            <label class="form-label">User Type</label>
                            <asp:TextBox runat="server" CssClass="form-input" Text="Admin" Enabled="false"/>
                        </div>

                        <div class="form-group full">
                            <label class="form-label">Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-input"
                                TextMode="MultiLine" Rows="4" MaxLength="500" />
                        </div>

                        <!-- Buttons -->
                        <div class="btn-row">
                            <asp:Button ID="btnSave" runat="server" Text="Save Changes"
                                CssClass="btn-primary"
                                ValidationGroup="profileGroup"
                                OnClick="btnSave_Click" />

                            <asp:Label ID="lblMessage" runat="server" CssClass="text-muted" />
                        </div>

                    </div>
                </div>
                </asp:Panel>

                <!-- ── PROFILE PICTURE ── -->
                <div class="card">
                    <div class="card-title">
                        <span class="title-dot dot-purple"></span>
                        Profile Picture
                    </div>
                    <div class="upload-section">
                        <div class="upload-preview">
                            <img id="profilePic" runat="server" style="width:100%;height:100%;object-fit:cover;border-radius:50%;" />
                        </div>
                        <div>
                            <label class="form-label" style="margin-bottom:8px;">Upload new photo</label>
                            <asp:FileUpload ID="fuProfileImage" runat="server" CssClass="form-input"
                                onchange="previewImage(this)" style="padding:8px;" />
                            <asp:Label ID="lblUploadMessage" runat="server" />
                        </div>
                    </div>
                    <br />
                    <div class="btn-row">
                        <asp:Button ID="Button1" runat="server" Text="Save Changes"
                            CssClass="btn-primary"
                            ValidationGroup="profileGroup"
                            OnClick="btnSave_Click1" />

                    </div>
                </div>

                <!-- ── ACCOUNT SECURITY ── -->
                <div class="card">
                    <div class="card-title">
                        <span class="title-dot dot-blue"></span>
                        Account Security
                    </div>
                    <div class="form-grid">
                        <div class="form-group full">
                            <label class="form-label">New Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                                CssClass="form-input" MaxLength="50" />
                            <asp:RequiredFieldValidator ControlToValidate="txtPassword" ErrorMessage="Password is required."
                                CssClass="validation-error" ValidationGroup="pwdGroup" runat="server" />
                        </div>

                        <div class="form-group full">
                            <label class="form-label">Confirm Password</label>
                            <asp:TextBox ID="txtValid" runat="server" TextMode="Password"
                                CssClass="form-input" MaxLength="50" />
                            <asp:RequiredFieldValidator 
                                ID="rfvVer" 
                                runat="server"
                                ControlToValidate="txtValid"
                                ErrorMessage="Confirmation password is required."
                                ValidationGroup="pwdGroup"
                                CssClass="validation-error" />
                            <br />
                            <asp:CompareValidator ID="CompareValidator1" runat="server" 
                                ControlToCompare="txtPassword" 
                                ValidationGroup="pwdGroup" 
                                ControlToValidate="txtValid" 
                                ErrorMessage="Password not match." 
                                CssClass="validation-error"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="btn-row">
                        <asp:Button ID="btnSavePassword" runat="server" Text="Update Password" CssClass="btn-primary"
                            OnClick="btnSave_Click2" ValidationGroup="pwdGroup" />
                    </div>
                </div>
            </div>
    </form>
<script>
    function previewImage(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById("profilePic").src = e.target.result;
            };
            reader.readAsDataURL(input.files[0]);
        }
    }
</script>
</body>
</html>
