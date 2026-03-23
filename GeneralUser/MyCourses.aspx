<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyCourses.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.MyCourses" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>My Learning - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --surface-hover: #f8fafd;
            --primary: #2563eb;
            --primary-bg: rgba(37,99,235,0.08);
            --accent-orange: #f59e0b;
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
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

        /* ═══ HEADER & NAV ═══ */
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

        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }
        .page-header { margin-bottom: 24px; animation: slideUp 0.4s ease both; }
        .page-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: var(--primary); margin-bottom: 6px;
            font-family: 'Space Mono', monospace;
        }
        .page-title { font-size: 1.6rem; font-weight: 700; }

        .alert {
            padding: 14px 20px; border-radius: var(--radius-sm);
            font-size: 0.875rem; font-weight: 500; margin-bottom: 20px;
            display: flex; align-items: center; gap: 10px;
        }
        .alert-success { background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2); color: #059669; }

        /* ═══ COURSE GRID ═══ */
        .courses-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(340px, 1fr)); gap: 18px; }
        .course-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 24px;
            display: flex; flex-direction: column; gap: 14px;
            box-shadow: var(--shadow-sm); transition: transform 0.2s, box-shadow 0.2s;
            animation: slideUp 0.5s ease both;
        }
        .course-card:hover { transform: translateY(-3px); box-shadow: var(--shadow-md); }
        .course-top { display: flex; justify-content: space-between; align-items: flex-start; }
        .course-category {
            display: inline-block; background: var(--primary-bg); color: var(--primary);
            font-size: 0.7rem; font-weight: 700; letter-spacing: 0.8px; text-transform: uppercase;
            padding: 4px 12px; border-radius: 20px;
        }
        .course-enrolled { font-size: 0.72rem; color: var(--text-muted); font-family: 'Space Mono', monospace; }
        .course-name { font-size: 1.1rem; font-weight: 700; line-height: 1.4; color: var(--text); }

        .progress-section { display: flex; flex-direction: column; gap: 6px; }
        .progress-top { display: flex; justify-content: space-between; align-items: center; }
        .progress-label { font-size: 0.75rem; color: var(--text-muted); font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }
        .progress-pct { font-size: 0.75rem; font-weight: 700; color: var(--primary); font-family: 'Space Mono', monospace; }
        .progress-bar-bg { width: 100%; height: 6px; background: var(--border-light); border-radius: 10px; overflow: hidden; }
        .progress-bar-fill { height: 100%; background: linear-gradient(90deg, var(--primary), var(--accent-purple)); border-radius: 10px; transition: width 0.8s ease; }

        .course-actions { display: flex; gap: 10px; margin-top: auto; padding-top: 10px; border-top: 1px solid var(--border-light); }
        .btn-continue {
            flex: 1; background: var(--primary); color: white; border: none;
            padding: 10px 16px; border-radius: 8px; font-family: 'DM Sans', sans-serif;
            font-size: 0.85rem; font-weight: 600; cursor: pointer; text-align: center;
            text-decoration: none; display: flex; align-items: center; justify-content: center;
            transition: background 0.2s;
        }
        .btn-continue:hover { background: #1d4ed8; }
        .btn-unenroll {
            background: transparent; border: 1px solid var(--border);
            color: var(--text-muted); padding: 10px 16px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-unenroll:hover { background: #fef2f2; color: #ef4444; border-color: #fecaca; }

        /* ═══ EMPTY STATE ═══ */
        .empty-state { text-align: center; padding: 80px 20px; color: var(--text-secondary); background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); }
        .empty-state-icon { font-size: 3rem; margin-bottom: 14px; opacity: 0.5; }
        .empty-state h3 { font-size: 1.1rem; font-weight: 700; color: var(--text); margin-bottom: 8px; }
        .empty-state p { font-size: 0.875rem; margin-bottom: 20px; }
        .empty-state a {
            display: inline-block; background: var(--primary); color: white;
            text-decoration: none; padding: 10px 24px; border-radius: 8px;
            font-size: 0.85rem; font-weight: 600; transition: background 0.2s;
        }
        .empty-state a:hover { background: #1d4ed8; }

        /* ═══ UNENROLL MODAL ═══ */
        .modal-overlay {
            display: none; position: fixed; inset: 0;
            background: rgba(0,0,0,0.4); backdrop-filter: blur(4px);
            z-index: 999; align-items: center; justify-content: center;
        }
        .modal-overlay.open { display: flex; }
        .modal {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: 20px; width: 100%; max-width: 400px; padding: 32px;
            text-align: center; box-shadow: 0 20px 60px rgba(0,0,0,0.15);
            animation: modalIn 0.3s ease both;
        }
        @keyframes modalIn { from { opacity: 0; transform: scale(0.95); } to { opacity: 1; transform: scale(1); } }
        .modal-icon { font-size: 2.5rem; margin-bottom: 14px; }
        .modal-title { font-size: 1.1rem; font-weight: 700; margin-bottom: 8px; color: var(--text); }
        .modal-text { color: var(--text-secondary); font-size: 0.85rem; margin-bottom: 24px; line-height: 1.6; }
        .modal-actions { display: flex; gap: 12px; }
        .btn-cancel {
            flex: 1; background: var(--border-light); border: 1px solid var(--border);
            color: var(--text); padding: 11px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-cancel:hover { background: #e2e8f0; }
        .btn-confirm-unenroll {
            flex: 1; background: #ef4444; border: none; color: white;
            padding: 11px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-confirm-unenroll:hover { background: #dc2626; }

        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 900px) { .container { padding: 20px; } .header, .nav { padding: 0 20px; } }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <div class="user-pill">
                    <div class="user-avatar">
                        <asp:Image ID="imgAvatar" runat="server" Visible="false" />
                        <asp:Label ID="lblAvatarInitial" runat="server" Text="S" />
                    </div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="nav">
            <a href="GeneralDashboard.aspx"><span>📊</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>📚</span> Browse Courses</a>
            <a href="MyCourses.aspx" class="active"><span>🎓</span> My Learning</a>
            <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
            <a href="EditProfile.aspx"><span>👤</span> Profile</a>
        </div>

        <div class="container">
            <div class="page-header">
                <div class="page-label">Learning</div>
                <div class="page-title">My Courses</div>
            </div>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />
            <asp:HiddenField ID="hfUnenrollId" runat="server" />

            <asp:Panel ID="pnlCourses" runat="server">
                <div class="courses-grid">
                    <asp:Repeater ID="rptCourses" runat="server">
                        <ItemTemplate>
                            <div class="course-card">
                                <div class="course-top">
                                    <span class="course-category"><%# Eval("category") %></span>
                                    <span class="course-enrolled">Enrolled <%# Convert.ToDateTime(Eval("enrolldate")).ToString("MMM dd, yyyy") %></span>
                                </div>
                                <div class="course-name"><%# Eval("coursename") %></div>
                                
                                <div class="progress-section">
                                    <div class="progress-top">
                                        <span class="progress-label">Progress</span>
                                        <span class="progress-pct"><%# Eval("Progress") %>%</span>
                                    </div>
                                    <div class="progress-bar-bg">
                                        <div class="progress-bar-fill" style="width: <%# Eval("Progress") %>%"></div>
                                    </div>
                                </div>

                                <div class="course-actions">
                                    <a href='LessonViewer.aspx?courseid=<%# Eval("courseid") %>' class="btn-continue">Continue Learning →</a>
                                    <button type="button" class="btn-unenroll"
                                        onclick="confirmUnenroll(<%# Eval("courseid") %>, '<%# Eval("coursename").ToString().Replace("'", "\\'") %>')">
                                        Unenroll
                                    </button>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <div class="empty-state-icon">📭</div>
                    <h3>No courses yet</h3>
                    <p>You haven't enrolled in any free courses yet. Start exploring!</p>
                    <a href="BrowseCourses.aspx">Browse Free Courses</a>
                </div>
            </asp:Panel>

            <asp:Button ID="btnUnenrollConfirm" runat="server" Text="Confirm"
                style="display:none;" OnClick="btnUnenrollConfirm_Click" />
        </div>

        <div class="modal-overlay" id="unenrollModal">
            <div class="modal">
                <div class="modal-icon">⚠️</div>
                <div class="modal-title">Unenroll from course?</div>
                <div class="modal-text">
                    You are about to unenroll from <strong id="modalCourseName"></strong>.
                    You will lose access to the materials and your progress will be paused.
                </div>
                <div class="modal-actions">
                    <button type="button" class="btn-cancel" onclick="closeModal()">Cancel</button>
                    <button type="button" class="btn-confirm-unenroll" onclick="submitUnenroll()">Yes, Unenroll</button>
                </div>
            </div>
        </div>

    </form>

    <script>
        function confirmUnenroll(courseId, courseName) {
            document.getElementById('modalCourseName').innerText = courseName;
            document.getElementById('<%= hfUnenrollId.ClientID %>').value = courseId;
            document.getElementById('unenrollModal').classList.add('open');
        }
        function closeModal() {
            document.getElementById('unenrollModal').classList.remove('open');
            document.getElementById('<%= hfUnenrollId.ClientID %>').value = '';
        }
        function submitUnenroll() {
            document.getElementById('<%= btnUnenrollConfirm.ClientID %>').click();
        }
        document.getElementById('unenrollModal').addEventListener('click', function (e) {
            if (e.target === this) closeModal();
        });
    </script>
</body>
</html>