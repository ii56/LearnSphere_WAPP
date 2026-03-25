<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LearnSphere_WAPP.Login" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - LearnSphere</title>
    <link href="registration.css?v=5" rel="stylesheet" />
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
            font-size: 0.78rem; margin-bottom: 10px;
        }
    </style>
</head>
<body>
    <div class="blob blob1"></div>
    <div class="blob blob2"></div>
    <div class="blob blob3"></div>

<form id="form1" runat="server">
    <%-- Hidden field carries the Google-resolved username back to the form --%>
    <asp:HiddenField ID="hfGoogleUsername" runat="server" />
    <asp:Button ID="btnGoogleLoginTrigger" runat="server"
        Text="" Style="display:none" CausesValidation="false"
        OnClick="btnGoogleLoginTrigger_Click" />

    <div class="registration-container">
        <h2>Login</h2>
        <div class="logo-container">
            <img src='<%= ResolveUrl("~/LEARNSPHERE_sign.png") %>' alt="LearnSphere Logo" class="login-logo" />
        </div>

        <asp:Panel ID="pnlGoogleNote" runat="server" Visible="false">
            <div class="google-note">
                ✓ Google account matched — enter your password to continue.
            </div>
        </asp:Panel>

        <asp:ValidationSummary runat="server" CssClass="error-message" DisplayMode="BulletList" />

        <div class="form-group">
            <label>Username</label>
            <asp:TextBox ID="uname" runat="server" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="uname"
                ErrorMessage="Username is required" CssClass="error-message" />
        </div>
        <div class="form-group">
            <label>Password</label>
            <asp:TextBox ID="pwd" runat="server" TextMode="Password" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="pwd"
                ErrorMessage="Password is required." CssClass="error-message" />
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Login"
            CssClass="btn-register" OnClick="btnLogin_Click" />

        <div class="divider"><hr /><span>or</span><hr /></div>

        <button type="button" class="google-btn" onclick="startGoogleLogin()">
            <img src="https://www.svgrepo.com/show/475656/google-color.svg" alt="Google" />
            Sign in with Google
        </button>

        <br />
        <div class="register-redirect">
            <span>Don't have an account?</span>
            <asp:HyperLink runat="server" NavigateUrl="~/Registration.aspx" CssClass="btn-link-register">
                Register here
            </asp:HyperLink>
        </div>

        <asp:Label ID="errMsg" runat="server" CssClass="error-message" />
    </div>
</form>

<script src="https://accounts.google.com/gsi/client" async defer></script>
<script>
    // Replace with your actual Google Client ID from console.cloud.google.com
    var GOOGLE_CLIENT_ID = '387653567048-qdimnvn7bg13ubpsqoh6bofr4da4ahoq.apps.googleusercontent.com';

    function decodeJwtPayload(token) {
        var base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
        var json = decodeURIComponent(atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(json);
    }

    function startGoogleLogin() {
        google.accounts.id.initialize({
            client_id: GOOGLE_CLIENT_ID,
            callback: handleGoogleLoginResponse,
            context: 'signin'
        });
        google.accounts.id.prompt(function (notification) {
            if (notification.isNotDisplayed() || notification.isSkippedMoment()) {
                google.accounts.id.renderButton(
                    document.getElementById('googleFallbackLogin'),
                    { theme: 'outline', size: 'large' }
                );
            }
        });
    }

    function handleGoogleLoginResponse(response) {
        var payload = decodeJwtPayload(response.credential);
        // Store the Google email in the hidden field and trigger a server-side lookup
        document.getElementById('<%= hfGoogleUsername.ClientID %>').value = payload.email || '';
        document.getElementById('<%= btnGoogleLoginTrigger.ClientID %>').click();
    }
</script>
<div id="googleFallbackLogin"></div>
</body>
</html>
