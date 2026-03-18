<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="EditProfile.aspx.cs"
    Inherits="LearnSphere_WAPP.Lecturer.EditProfile" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Profile</title>
    <link href="lecturer.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

<div class="layout">

    <!-- SIDEBAR -->
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
                    <%= Session["uname"] != null ? Server.HtmlEncode(Session["uname"].ToString()) : "" %>
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

            <asp:Button ID="btnLogout"
                runat="server"
                Text="Logout"
                CssClass="logout-btn"
                OnClick="btnLogout_Click" />

        </div>
    </div>


    <!-- MAIN CONTENT -->
    <div class="main-content">

        <div class="profile-header">
            <h2>Profile Settings</h2>
            <p class="sub-text">Manage your personal and account information</p>
        </div>


        <!-- VALIDATION SUMMARY -->
        <asp:ValidationSummary
            ID="ValidationSummary1"
            runat="server"
            CssClass="validation-summary"
            HeaderText="Please fix the following errors:"
            ValidationGroup="profileGroup" />


        <!-- PERSONAL INFO -->
        <div class="form-card-modern">
            <h3>Personal Information</h3>

            <div class="row">

                <div class="col">
                    <label>Username</label>
                    <asp:TextBox ID="txtUsername"
                        runat="server"
                        CssClass="modern-input"
                        Enabled="false" />
                </div>

                <div class="col">
                    <label>Email *</label>
                    <asp:TextBox ID="txtEmail"
                        runat="server"
                        CssClass="modern-input"
                        MaxLength="100" />

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

            </div>


            <div class="row">

                <div class="col">
                    <label>First Name *</label>
                    <asp:TextBox ID="txtFirstName"
                        runat="server"
                        CssClass="modern-input"
                        MaxLength="50" />

                    <asp:RequiredFieldValidator
                        ControlToValidate="txtFirstName"
                        ErrorMessage="First name required."
                        CssClass="validation-error"
                        ValidationGroup="profileGroup"
                        runat="server" />
                </div>

                <div class="col">
                    <label>Last Name *</label>
                    <asp:TextBox ID="txtLastName"
                        runat="server"
                        CssClass="modern-input"
                        MaxLength="50" />

                    <asp:RequiredFieldValidator
                        ControlToValidate="txtLastName"
                        ErrorMessage="Last name required."
                        CssClass="validation-error"
                        ValidationGroup="profileGroup"
                        runat="server" />
                </div>

            </div>


            <div class="row">

                <div class="col">
                    <label>Age *</label>
                    <asp:TextBox ID="txtAge"
                        runat="server"
                        CssClass="modern-input"
                        MaxLength="3" />

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

                <div class="col">
                    <label>Gender</label>
                    <asp:DropDownList ID="ddlGender"
                        runat="server"
                        CssClass="modern-input">
                        <asp:ListItem Text="Male" />
                        <asp:ListItem Text="Female" />
                        <asp:ListItem Text="Other" />
                    </asp:DropDownList>
                </div>

            </div>


            <div class="row">
                <div class="col">
                    <label>Description</label>
                    <asp:TextBox ID="txtDescription"
                        runat="server"
                        CssClass="modern-input"
                        TextMode="MultiLine"
                        Rows="4"
                        MaxLength="500" />
                </div>
            </div>

        </div>


        <!-- PASSWORD -->
        <div class="form-card-modern" style="margin-top:30px;">
            <h3>Account Security</h3>

            <label>New Password</label>
            <asp:TextBox ID="txtPassword"
                runat="server"
                TextMode="Password"
                CssClass="modern-input"
                MaxLength="50" />

        </div>


        <!-- VERIFICATION -->
<!-- ROLE VERIFICATION REQUEST -->
<div class="form-card-modern" style="margin-top:30px;">
    <h3>Request Role Upgrade</h3>

    <p class="sub-text">
        Submit a request to upgrade your account role. Approval is required.
    </p>

    <!-- CURRENT ROLE DISPLAY -->
    <label>Current Role</label>
    <asp:TextBox ID="txtCurrentRole"
        runat="server"
        CssClass="modern-input"
        Enabled="false" />

    <br />

    <!-- ROLE SELECTION (DYNAMICALLY FILLED IN .CS) -->
    <label>Request New Role *</label>
    <asp:DropDownList ID="ddlRequestedRole"
        runat="server"
        CssClass="modern-input">
    </asp:DropDownList>

    <asp:RequiredFieldValidator
        ControlToValidate="ddlRequestedRole"
        InitialValue=""
        ErrorMessage="Please select a role."
        CssClass="validation-error"
        ValidationGroup="verifyGroup"
        runat="server" />

    <br /><br />

    <!-- DOCUMENT UPLOAD -->
    <label>Upload Supporting Document (PDF only) *</label>

    <asp:FileUpload ID="fuVerificationDoc"
        runat="server"
        CssClass="modern-input" />

    <br /><br />

    <!-- SUBMIT BUTTON -->
    <asp:Button ID="btnSendVerification"
        runat="server"
        Text="Send Verification Request"
        CssClass="btn-modern"
        ValidationGroup="verifyGroup"
        OnClick="btnSendVerification_Click" />

    <br /><br />

    <!-- STATUS MESSAGE -->
    <asp:Label ID="lblVerificationMsg" runat="server" />

    <br />

    <!-- REQUEST HISTORY (VERY GOOD FEATURE) -->
    <asp:Repeater ID="rptVerificationHistory" runat="server">
        <ItemTemplate>
            <div class="verification-item">

                <b>Requested:</b> <%# Eval("requestedrole") %> <br />
                <b>Status:</b> 
                <span class='<%# Eval("status").ToString().ToLower() %>'>
                    <%# Eval("status") %>
                </span>
                <br />

                <b>Date:</b> <%# Eval("requesttime", "{0:dd MMM yyyy}") %>

                <hr />
            </div>
        </ItemTemplate>
    </asp:Repeater>

</div>


<!-- PROFILE IMAGE -->
<div class="form-card-modern" style="margin-top:30px;">
    <h3>Profile Picture</h3>

    <div class="profile-upload-section">

        <!-- CURRENT IMAGE PREVIEW -->
        <div class="current-profile-preview">
            <div class="profile-img-wrapper-large">

                <img id="imgPreview"
                     src='<%= ResolveUrl(Session["profileImage"] != null 
                            ? Session["profileImage"].ToString() 
                            : "~/images/default-user.png") %>'
                     class='<%= Session["usertype"] != null && 
                              Session["usertype"].ToString() == "Lecturer" 
                              ? "profile-preview-img verified-glow" 
                              : "profile-preview-img" %>' />

            </div>
        </div>

        <!-- UPLOAD -->
        <div class="upload-controls">
            <asp:FileUpload ID="fuProfileImage"
                runat="server"
                CssClass="modern-input"
                onchange="previewImage(this)" />

            <asp:Label ID="lblUploadMessage" runat="server" />
        </div>

    </div>
</div>


        <!-- SAVE -->
        <div style="margin-top:30px;">
            <asp:Button ID="btnSave"
                runat="server"
                Text="Save Changes"
                CssClass="btn-modern"
                ValidationGroup="profileGroup"
                OnClick="btnSave_Click" />

            <asp:Label ID="lblMessage" runat="server" />
        </div>

    </div>

</div>

</form>
    <script>
        function previewImage(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    document.getElementById("imgPreview").src = e.target.result;
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</body>
</html>
