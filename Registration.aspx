<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="LearnSphere_WAPP.Registration" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration - LearnSphere</title>
    <link href="registration.css?v=6" rel="stylesheet" />
    <style>
        .google-btn {
            display: flex; align-items: center; justify-content: center; gap: 10px;
            width: 100%; padding: 10px 20px; margin: 10px 0;
            background: #fff; border: 1.5px solid #dadce0; border-radius: 6px;
            font-family: 'Segoe UI', sans-serif; font-size: 0.9rem; font-weight: 500;
            color: #3c4043; cursor: pointer; transition: background 0.2s, box-shadow 0.2s;
        }
        .google-btn:hover { background: #f8f9fa; box-shadow: 0 1px 4px rgba(0,0,0,0.15); }
        .google-btn img { width: 20px; height: 20px; }
        .divider { display: flex; align-items: center; gap: 10px; margin: 12px 0; }
        .divider hr { flex: 1; border: none; border-top: 1px solid #e0e0e0; }
        .divider span { font-size: 0.78rem; color: #9e9e9e; }
        .google-note {
            background: rgba(37,99,235,0.06); border: 1px solid rgba(37,99,235,0.2);
            color: #2563eb; border-radius: 6px; padding: 8px 14px;
            font-size: 0.78rem; margin-bottom: 10px; display: none;
        }
    </style>
</head>
<body>
    <div class="blob blob1"></div>
    <div class="blob blob2"></div>
    <div class="blob blob3"></div>

<form id="form1" runat="server">

    <%-- Hidden fields carry Google data across postbacks --%>
    <asp:HiddenField ID="hfGoogleEmail"    runat="server" />
    <asp:HiddenField ID="hfGoogleFname"    runat="server" />
    <asp:HiddenField ID="hfGoogleLname"    runat="server" />
    <asp:HiddenField ID="hfIsGoogleSignup" runat="server" Value="0" />

    <%-- Hidden trigger button — JS clicks this after Google callback to fire a postback --%>
    <asp:Button ID="btnGoogleRegisterTrigger" runat="server"
        Text="" Style="display:none" CausesValidation="false"
        OnClick="btnGoogleRegisterTrigger_Click" />

<div class="registration-container">
    <h2>Create Account</h2>

    <div class="step-indicator">
        Step <asp:Label ID="lblStep" runat="server" Text="1" /> of 3
    </div>

    <!-- ═══ STEP 1 — Username ═══ -->
    <asp:Panel ID="pnlStep1" runat="server">

        <%-- Google note shows when arriving via Google signup --%>
        <div id="googleNote" class="google-note">
            ✓ Google account connected — just choose a username to continue.
        </div>

        <div class="form-group">
            Username:
            <asp:TextBox ID="uname" runat="server" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="uname"
                ErrorMessage="Username required" CssClass="error-message"
                ValidationGroup="Step1" />
            <asp:CustomValidator ID="cvUsername" runat="server"
                ControlToValidate="uname"
                ErrorMessage="Username already exists"
                OnServerValidate="cvUsername_ServerValidate"
                CssClass="error-message" ValidationGroup="Step1" />
        </div>

        <asp:Button ID="btnNext1" runat="server" Text="Next"
            CssClass="btn-register" OnClick="btnNext1_Click"
            ValidationGroup="Step1" />

        <div class="divider"><hr /><span>or</span><hr /></div>

        <%-- Google Sign Up button — only shown when NOT already linked --%>
        <asp:Panel ID="pnlGoogleSignupBtn" runat="server">
            <button type="button" class="google-btn" onclick="startGoogleSignup()">
                <img src="https://www.svgrepo.com/show/475656/google-color.svg" alt="Google" />
                Sign up with Google
            </button>
        </asp:Panel>

        <div class="login-redirect">
            <span>Already have an account?</span>
            <asp:HyperLink runat="server" NavigateUrl="~/Login.aspx" CssClass="btn-link-login">Login here</asp:HyperLink>
        </div>
    </asp:Panel>

    <!-- ═══ STEP 2 — Email & Password ═══ -->
    <asp:Panel ID="pnlStep2" runat="server" Visible="false">

        <%-- Shown when email is pre-filled by Google --%>
        <asp:Panel ID="pnlEmailReadonly" runat="server" Visible="false">
            <div class="google-note" style="display:block;">
                ✓ Email filled from your Google account — set a password for your LearnSphere account.
            </div>
            <div class="form-group">
                Email (from Google):
                <asp:Label ID="lblGoogleEmailDisplay" runat="server" CssClass="form-readonly" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlEmailEditable" runat="server" Visible="true">
            <div class="form-group">
                Email:
                <asp:TextBox ID="email" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="email"
                    ErrorMessage="Email is required" CssClass="error-message"
                    ValidationGroup="Step2" Display="Dynamic" />
                <asp:CustomValidator ID="cvEmail" runat="server"
                    ControlToValidate="email"
                    ErrorMessage="Email is already registered"
                    OnServerValidate="cvEmail_ServerValidate"
                    CssClass="error-message" ValidationGroup="Step2" Display="Dynamic" />
            </div>
        </asp:Panel>

        <div class="form-group">
            Password:
            <asp:TextBox ID="pwd" runat="server" TextMode="Password" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="pwd"
                ErrorMessage="Password is required" CssClass="error-message"
                ValidationGroup="Step2" Display="Dynamic" />
        </div>
        <div class="form-group">
            Confirm Password:
            <asp:TextBox ID="pwd2" runat="server" TextMode="Password" />
        </div>

        <asp:Button ID="btnBack1" runat="server" Text="Back"
            CssClass="btn-register" OnClick="btnBack1_Click" CausesValidation="false" />
        <asp:Button ID="btnNext2" runat="server" Text="Next"
            CssClass="btn-register" OnClick="btnNext2_Click" ValidationGroup="Step2" />
    </asp:Panel>

    <!-- ═══ STEP 3 — Personal Info ═══ -->
    <asp:Panel ID="pnlStep3" runat="server" Visible="false">
        <div class="form-group">
            First Name:
            <asp:TextBox ID="fname" runat="server" />
        </div>
        <div class="form-group">
            Last Name:
            <asp:TextBox ID="lname" runat="server" />
        </div>
        <div class="form-group">
            Age:
            <asp:TextBox ID="age" runat="server" />
        </div>
        <div class="form-group">
            Gender:
            <asp:DropDownList ID="gender" runat="server">
                <asp:ListItem Text="Select" Value="" />
                <asp:ListItem Text="Male"   Value="Male" />
                <asp:ListItem Text="Female" Value="Female" />
            </asp:DropDownList>
        </div>
        <asp:Button ID="btnBack2" runat="server" Text="Back"
            CssClass="btn-register" OnClick="btnBack2_Click" CausesValidation="false" />
        <asp:Button ID="btnRegister" runat="server" Text="Register"
            CssClass="btn-register" OnClick="btnRegister_Click" CausesValidation="false" />
    </asp:Panel>

    <asp:Label ID="errMsg" runat="server" CssClass="error-message" />
</div>
</form>

<!-- Google Identity Services -->
<script src="https://accounts.google.com/gsi/client" async defer></script>
<script>
    var GOOGLE_CLIENT_ID = '387653567048-qdimnvn7bg13ubpsqoh6bofr4da4ahoq.apps.googleusercontent.com';

    // Decodes the base64-encoded JWT payload to extract user info without a server round-trip
    function decodeJwtPayload(token) {
        var base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
        var json = decodeURIComponent(atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(json);
    }

    function startGoogleSignup() {
        google.accounts.id.initialize({
            client_id: GOOGLE_CLIENT_ID,
            callback: handleGoogleSignupResponse,
            context: 'signup'
        });
        google.accounts.id.prompt(function (notification) {
            if (notification.isNotDisplayed() || notification.isSkippedMoment()) {
                // Fallback: render a standard popup
                google.accounts.id.renderButton(
                    document.getElementById('googleFallbackSignup'),
                    { theme: 'outline', size: 'large' }
                );
            }
        });
    }

    function handleGoogleSignupResponse(response) {
        var payload = decodeJwtPayload(response.credential);

        // Split Google display name into first and last
        var parts = (payload.name || '').split(' ');
        var fname = parts[0] || '';
        var lname = parts.slice(1).join(' ') || '';

        // Store in hidden fields so the server can read them on postback
        document.getElementById('<%= hfGoogleEmail.ClientID    %>').value = payload.email || '';
        document.getElementById('<%= hfGoogleFname.ClientID    %>').value = fname;
        document.getElementById('<%= hfGoogleLname.ClientID    %>').value = lname;
        document.getElementById('<%= hfIsGoogleSignup.ClientID %>').value = '1';

        // Fire a postback so the server can process the Google data
        document.getElementById('<%= btnGoogleRegisterTrigger.ClientID %>').click();
    }

    // Show the Google note banner if we arrived via Google
    window.onload = function () {
        var isGoogle = document.getElementById('<%= hfIsGoogleSignup.ClientID %>').value;
        if (isGoogle === '1') {
            var note = document.getElementById('googleNote');
            if (note) note.style.display = 'block';
        }
    };
</script>
<div id="googleFallbackSignup"></div>
</body>
</html>
