<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditProfile.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.EditProfile" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>My Profile - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --primary: #2563eb;
            --primary-bg: rgba(37,99,235,0.08);
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
            --accent-orange: #f59e0b;
            --text: #1e293b;
            --text-secondary: #64748b;
            --text-muted: #94a3b8;
            --border: #e2e8f0;
            --border-light: #f1f5f9;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04), 0 1px 2px rgba(0,0,0,0.03);
            --shadow-md: 0 4px 12px rgba(0,0,0,0.06), 0 2px 4px rgba(0,0,0,0.03);
            --radius: 14px;
            --radius-sm: 10px;
        }
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: 'DM Sans', sans-serif;
            background: var(--bg-gradient); background-attachment: fixed;
            color: var(--text); min-height: 100vh;
        }

        /* ═══ HEADER ═══ */
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82); backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 12px; text-decoration: none; }
        .logo img { height: 38px; width: 38px; object-fit: contain; }
        .logo-text { font-size: 1.2rem; font-weight: 700; color: var(--text); }
        .logo-text span { color: var(--primary); }
        
        .header-right { display: flex; align-items: center; gap: 14px; }
        .user-pill {
            display: flex; align-items: center; gap: 10px;
            background: var(--bg); border: 1px solid var(--border);
            border-radius: 50px; padding: 5px 16px 5px 5px;
        }
        .user-avatar {
            width: 32px; height: 32px;
            background: linear-gradient(135deg, var(--primary), var(--accent-purple));
            border-radius: 50%; display: flex; align-items: center; justify-content: center;
            font-size: 13px; font-weight: 700; color: white; overflow: hidden; position: relative;
        }
        .user-avatar img { width: 100%; height: 100%; object-fit: cover; position: absolute; top: 0; left: 0; }
        .user-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }
        
        .btn-logout {
            background: transparent; border: 1px solid var(--border);
            color: var(--text-secondary); padding: 7px 18px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-logout:hover { background: #fef2f2; color: #ef4444; border-color: #fecaca; }

        /* ═══ NAV ═══ */
        .nav {
            background: var(--surface); border-bottom: 1px solid var(--border);
            padding: 0 36px; display: flex; gap: 2px;
        }
        .nav a {
            color: var(--text-muted); text-decoration: none; padding: 14px 18px;
            font-size: 0.85rem; font-weight: 600; display: flex; align-items: center; gap: 8px;
            border-bottom: 2.5px solid transparent; transition: all 0.2s;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active { color: var(--primary); border-bottom-color: var(--primary); }

        /* ═══ CONTAINER ═══ */
        .container { max-width: 900px; margin: 0 auto; padding: 28px 36px; }
        .page-header { margin-bottom: 24px; animation: slideUp 0.4s ease both; }
        .page-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: var(--primary); margin-bottom: 6px;
            font-family: 'Space Mono', monospace;
        }
        .page-title { font-size: 1.6rem; font-weight: 700; }

        .alert {
            padding: 14px 20px; border-radius: var(--radius-sm);
            font-size: 0.85rem; font-weight: 500; margin-bottom: 20px;
            display: flex; align-items: center; gap: 10px;
        }
        .alert-success { background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2); color: #059669; }
        .alert-error { background: rgba(239,68,68,0.08); border: 1px solid rgba(239,68,68,0.2); color: #dc2626; }

        /* ═══ PROFILE HERO ═══ */
        .profile-hero {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 50%, #60a5fa 100%);
            border-radius: var(--radius); padding: 32px;
            margin-bottom: 20px; display: flex; align-items: center; gap: 24px;
            position: relative; overflow: hidden;
            box-shadow: 0 8px 30px rgba(37,99,235,0.25);
            animation: slideUp 0.5s ease both;
        }
        .profile-hero::before {
            content: ''; position: absolute; top: -40%; right: -10%;
            width: 250px; height: 250px;
            background: radial-gradient(circle, rgba(255,255,255,0.1), transparent 65%);
            border-radius: 50%;
        }
        .profile-pic-wrapper { position: relative; flex-shrink: 0; }
        .profile-pic {
            width: 90px; height: 90px; border-radius: 50%;
            border: 3px solid rgba(255,255,255,0.4);
            object-fit: cover; background: rgba(255,255,255,0.2);
            display: flex; align-items: center; justify-content: center; overflow: hidden;
        }
        .profile-pic img { width: 100%; height: 100%; object-fit: cover; }
        .profile-pic-initial {
            width: 90px; height: 90px; border-radius: 50%;
            border: 3px solid rgba(255,255,255,0.4); background: rgba(255,255,255,0.2);
            display: flex; align-items: center; justify-content: center;
            font-size: 2.2rem; font-weight: 700; color: white;
        }
        .profile-hero-info h2 { font-size: 1.4rem; font-weight: 700; color: white; margin-bottom: 4px; }
        .profile-hero-info p { font-size: 0.85rem; color: rgba(255,255,255,0.7); }
        .role-tag {
            display: inline-block; margin-top: 8px;
            background: rgba(255,255,255,0.2); border: 1px solid rgba(255,255,255,0.3);
            color: white; font-size: 0.7rem; font-weight: 700;
            letter-spacing: 0.8px; text-transform: uppercase;
            padding: 3px 12px; border-radius: 20px; font-family: 'Space Mono', monospace;
        }

        /* ═══ CARDS ═══ */
        .card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 28px;
            box-shadow: var(--shadow-sm); margin-bottom: 20px;
            animation: slideUp 0.5s ease both;
        }
        .card-title {
            font-size: 0.95rem; font-weight: 700; margin-bottom: 20px;
            display: flex; align-items: center; gap: 10px;
        }
        .title-dot { width: 8px; height: 8px; border-radius: 50%; background: var(--primary); }
        .dot-green { background: var(--accent-green); }
        .dot-orange { background: var(--accent-orange); }

        .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
        .form-group { margin-bottom: 0; }
        .form-group.full { grid-column: 1 / -1; }
        .form-label {
            display: block; font-size: 0.78rem; font-weight: 600;
            color: var(--text-secondary); margin-bottom: 6px;
            text-transform: uppercase; letter-spacing: 0.5px;
        }
        .form-input {
            width: 100%; background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 11px 14px;
            color: var(--text); font-family: 'DM Sans', sans-serif;
            font-size: 0.875rem; outline: none; transition: border-color 0.2s; box-sizing: border-box;
        }
        .form-input:focus { border-color: var(--primary); background: white; }
        .form-input:disabled { background: var(--border-light); color: var(--text-muted); cursor: not-allowed; }
        .form-select {
            width: 100%; background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 11px 14px;
            color: var(--text); font-family: 'DM Sans', sans-serif;
            font-size: 0.875rem; outline: none; box-sizing: border-box;
        }
        textarea.form-input { resize: vertical; min-height: 80px; }

        .btn-primary {
            background: var(--primary); color: white; border: none;
            padding: 11px 28px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s; margin-top: 8px;
        }
        .btn-primary:hover { background: #1d4ed8; }
        .btn-secondary {
            background: var(--border-light); color: var(--text); border: 1px solid var(--border);
            padding: 11px 28px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s; margin-top: 8px;
        }
        .btn-secondary:hover { border-color: var(--primary); color: var(--primary); }
        .btn-row { display: flex; gap: 10px; margin-top: 16px; }

        /* upload section */
        .upload-section { display: flex; align-items: center; gap: 16px; margin-top: 4px; }
        .upload-preview {
            width: 60px; height: 60px; border-radius: 50%;
            border: 2px solid var(--border); overflow: hidden;
            background: var(--border-light); display: flex; align-items: center; justify-content: center;
        }
        .upload-preview img { width: 100%; height: 100%; object-fit: cover; }
        .upload-initial { font-size: 1.4rem; font-weight: 700; color: var(--text-muted); }

        /* info row for view mode */
        .info-row { display: flex; justify-content: space-between; padding: 12px 0; border-bottom: 1px solid var(--border-light); }
        .info-row:last-child { border-bottom: none; }
        .info-label { font-size: 0.82rem; color: var(--text-muted); font-weight: 500; }
        .info-value { font-size: 0.85rem; font-weight: 600; color: var(--text); }

        /* Verification History */
        .verification-item {
            background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 14px 18px;
            font-size: 0.85rem; margin-bottom: 10px; line-height: 1.7;
        }
        .verification-item .Pending { color: var(--accent-orange); font-weight: 700; }
        .verification-item .Approved { color: var(--accent-green); font-weight: 700; }
        .verification-item .Rejected, .verification-item .Denied { color: #ef4444; font-weight: 700; }
        .sub-text { font-size: 0.85rem; color: var(--text-secondary); margin-bottom: 16px; line-height: 1.5;}

        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 700px) {
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
            .form-grid { grid-template-columns: 1fr; }
            .profile-hero { flex-direction: column; text-align: center; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <div class="user-pill">
                    <div class="user-avatar">
                        <asp:Image ID="imgHeaderAvatar" runat="server" Visible="false" />
                        <asp:Label ID="lblAvatarInitial" runat="server" Text="U" />
                    </div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" CausesValidation="false" />
            </div>
        </div>

        <div class="nav">
            <a href="GeneralDashboard.aspx"><span>📊</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>📚</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span>🎓</span> My Learning</a>
            <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
            <a href="EditProfile.aspx" class="active"><span>👤</span> Profile</a>
        </div>

        <div class="container">
            <div class="page-header">
                <div class="page-label">Account</div>
                <div class="page-title">Profile Settings</div>
            </div>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />

            <div class="profile-hero">
                <div class="profile-pic-wrapper">
                    <asp:Panel ID="pnlProfilePic" runat="server" Visible="false">
                        <div class="profile-pic"><asp:Image ID="imgProfile" runat="server" /></div>
                    </asp:Panel>
                    <asp:Panel ID="pnlProfileInitial" runat="server" Visible="true">
                        <div class="profile-pic-initial"><asp:Label ID="lblHeroInitial" runat="server" Text="U" /></div>
                    </asp:Panel>
                </div>
                <div class="profile-hero-info">
                    <h2><asp:Label ID="lblFullName" runat="server" /></h2>
                    <p><asp:Label ID="lblEmail" runat="server" /></p>
                    <span class="role-tag">General User</span>
                </div>
            </div>

            <div class="card">
                <div class="card-title"><span class="title-dot"></span> Personal Information</div>

                <asp:Panel ID="pnlView" runat="server">
                    <div class="info-row"><span class="info-label">First Name</span><span class="info-value"><asp:Label ID="lblFname" runat="server" /></span></div>
                    <div class="info-row"><span class="info-label">Last Name</span><span class="info-value"><asp:Label ID="lblLname" runat="server" /></span></div>
                    <div class="info-row"><span class="info-label">Email</span><span class="info-value"><asp:Label ID="lblEmailView" runat="server" /></span></div>
                    <div class="info-row"><span class="info-label">Username</span><span class="info-value"><asp:Label ID="lblUsername" runat="server" /></span></div>
                    <div class="info-row"><span class="info-label">Age</span><span class="info-value"><asp:Label ID="lblAge" runat="server" /></span></div>
                    <div class="info-row"><span class="info-label">Gender</span><span class="info-value"><asp:Label ID="lblGender" runat="server" /></span></div>
                    <div class="info-row"><span class="info-label">Bio</span><span class="info-value"><asp:Label ID="lblBio" runat="server" /></span></div>
                    <div class="info-row"><span class="info-label">Member Since</span><span class="info-value"><asp:Label ID="lblJoined" runat="server" /></span></div>
                    <div class="btn-row"><asp:Button ID="btnEdit" runat="server" Text="Edit Profile" CssClass="btn-primary" OnClick="btnEdit_Click" /></div>
                </asp:Panel>

                <asp:Panel ID="pnlEdit" runat="server" Visible="false">
                    <div class="form-grid">
                        <div class="form-group">
                            <label class="form-label">First Name</label>
                            <asp:TextBox ID="txtFname" runat="server" CssClass="form-input" MaxLength="50" />
                        </div>
                        <div class="form-group">
                            <label class="form-label">Last Name</label>
                            <asp:TextBox ID="txtLname" runat="server" CssClass="form-input" MaxLength="50" />
                        </div>
                        <div class="form-group">
                            <label class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input" MaxLength="100" />
                        </div>
                        <div class="form-group">
                            <label class="form-label">Age</label>
                            <asp:TextBox ID="txtAge" runat="server" CssClass="form-input" TextMode="Number" MaxLength="3" />
                        </div>
                        <div class="form-group">
                            <label class="form-label">Gender</label>
                            <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Male" Value="Male" />
                                <asp:ListItem Text="Female" Value="Female" />
                                <asp:ListItem Text="Other" Value="Other" />
                            </asp:DropDownList>
                        </div>
                        <div class="form-group full">
                            <label class="form-label">Bio / Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-input" TextMode="MultiLine" MaxLength="500" />
                        </div>
                    </div>
                    <div class="btn-row">
                        <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-primary" OnClick="btnSave_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" OnClick="btnCancel_Click" />
                    </div>
                </asp:Panel>
            </div>

            <div class="card">
                <div class="card-title"><span class="title-dot dot-green"></span> Profile Picture</div>
                <div class="upload-section">
                    <div class="upload-preview">
                        <asp:Panel ID="pnlUploadPreview" runat="server" Visible="false">
                            <asp:Image ID="imgUploadPreview" runat="server" />
                        </asp:Panel>
                        <asp:Panel ID="pnlUploadInitial" runat="server" Visible="true">
                            <span class="upload-initial"><asp:Label ID="lblUploadInitial" runat="server" Text="U" /></span>
                        </asp:Panel>
                    </div>
                    <div>
                        <asp:FileUpload ID="fuProfilePic" runat="server" CssClass="form-input" style="padding:8px;" />
                        <asp:Button ID="btnUpload" runat="server" Text="Upload Photo" CssClass="btn-primary" OnClick="btnUpload_Click" />
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-title"><span class="title-dot dot-purple"></span> Change Password</div>
                <div class="form-grid">
                    <div class="form-group full">
                        <label class="form-label">Current Password</label>
                        <asp:TextBox ID="txtCurrentPwd" runat="server" CssClass="form-input" TextMode="Password" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">New Password</label>
                        <asp:TextBox ID="txtNewPwd" runat="server" CssClass="form-input" TextMode="Password" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Confirm New Password</label>
                        <asp:TextBox ID="txtConfirmPwd" runat="server" CssClass="form-input" TextMode="Password" />
                    </div>
                </div>
                <div class="btn-row">
                    <asp:Button ID="btnChangePwd" runat="server" Text="Update Password" CssClass="btn-primary" OnClick="btnChangePwd_Click" />
                </div>
            </div>

            <div class="card">
                <div class="card-title"><span class="title-dot dot-orange"></span> Request Lecturer Upgrade</div>
                <p class="sub-text">Submit your teaching certifications (PDF only) to request a role upgrade. This will be reviewed by an administrator.</p>
                
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">Target Role</label>
                        <asp:TextBox ID="txtRequestedRole" runat="server" CssClass="form-input" Text="Lecturer" Enabled="false" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Verification Document *</label>
                        <asp:FileUpload ID="fuVerificationDoc" runat="server" CssClass="form-input" style="padding:8px;" />
                    </div>
                </div>
                
                <div class="btn-row">
                    <asp:Button ID="btnSendVerification" runat="server" Text="Submit Request" CssClass="btn-primary" OnClick="btnSendVerification_Click" />
                </div>
                
                <asp:Label ID="lblVerificationMsg" runat="server" style="margin-top:14px; display:block;" />

                <div style="margin-top: 24px;">
                    <div class="form-label">Request History</div>
                    <asp:Repeater ID="rptVerificationHistory" runat="server">
                        <ItemTemplate>
                            <div class="verification-item">
                                <strong>Status:</strong> <span class='<%# Eval("status") %>'><%# Eval("status") %></span><br />
                                <strong>Requested Role:</strong> <%# Eval("requestedrole") %><br />
                                <strong>Date:</strong> <%# Convert.ToDateTime(Eval("requesttime")).ToString("dd MMM yyyy - hh:mm tt") %>
                                <%# Eval("remarks") != DBNull.Value && !string.IsNullOrEmpty(Eval("remarks").ToString()) ? "<hr/><strong>Remarks:</strong> " + Eval("remarks") : "" %>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblNoHistory" runat="server" Text="No requests submitted yet." CssClass="sub-text" Visible="false" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>