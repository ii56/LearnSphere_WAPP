<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyCourses.aspx.cs" Inherits="LearnSphere_WAPP.Student.MyCourses" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>My Courses - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=Sora:wght@300;400;600;700&family=JetBrains+Mono:wght@400;600&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #0b0f1a; --surface: #111827; --surface2: #1a2235;
            --accent: #e94560; --accent2: #f97316; --gold: #f59e0b;
            --text: #e8eaf0; --muted: #6b7280; --border: rgba(255,255,255,0.07);
        }
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body { font-family: 'Sora', sans-serif; background: var(--bg); color: var(--text); min-height: 100vh; }
        body::before {
            content: ''; position: fixed; top: -50%; left: -50%; width: 200%; height: 200%;
            background: radial-gradient(ellipse 600px 400px at 20% 20%, rgba(233,69,96,0.06) 0%, transparent 60%),
                        radial-gradient(ellipse 500px 300px at 80% 80%, rgba(249,115,22,0.05) 0%, transparent 60%);
            z-index: 0; pointer-events: none;
        }
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(11,15,26,0.85); backdrop-filter: blur(16px);
            border-bottom: 1px solid var(--border);
            padding: 0 40px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 10px; }
        .logo-icon {
            width: 32px; height: 32px;
            background: linear-gradient(135deg, var(--accent), var(--accent2));
            border-radius: 8px; display: flex; align-items: center; justify-content: center;
            font-size: 16px; font-weight: 700; color: white;
        }
        .logo-text { font-size: 1.2rem; font-weight: 700; letter-spacing: -0.5px; }
        .logo-text span { color: var(--accent); }
        .header-right { display: flex; align-items: center; gap: 16px; }
        .user-pill {
            display: flex; align-items: center; gap: 10px;
            background: var(--surface2); border: 1px solid var(--border);
            border-radius: 50px; padding: 6px 16px 6px 6px;
        }
        .user-avatar {
            width: 30px; height: 30px;
            background: linear-gradient(135deg, var(--accent), var(--accent2));
            border-radius: 50%; display: flex; align-items: center; justify-content: center;
            font-size: 13px; font-weight: 700; color: white;
        }
        .user-name { font-size: 0.85rem; font-weight: 600; }
        .btn-logout {
            background: transparent; border: 1px solid rgba(233,69,96,0.4);
            color: var(--accent); padding: 7px 18px; border-radius: 8px;
            font-family: 'Sora', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-logout:hover { background: var(--accent); color: white; }
        .nav {
            background: var(--surface); border-bottom: 1px solid var(--border);
            padding: 0 40px; display: flex; gap: 4px; position: relative; z-index: 1;
        }
        .nav a {
            color: var(--muted); text-decoration: none; padding: 14px 18px;
            font-size: 0.875rem; font-weight: 500; display: flex; align-items: center; gap: 8px;
            border-bottom: 2px solid transparent; transition: all 0.2s;
        }
        .nav a:hover { color: var(--text); }
        .nav a.active { color: var(--accent); border-bottom-color: var(--accent); }
        .container { max-width: 1200px; margin: 0 auto; padding: 36px 40px; position: relative; z-index: 1; }
        .page-header { margin-bottom: 32px; animation: fadeUp 0.4s ease both; }
        .page-label { font-size: 0.75rem; font-weight: 600; letter-spacing: 2px; text-transform: uppercase; color: var(--accent); margin-bottom: 8px; font-family: 'JetBrains Mono', monospace; }
        .page-title { font-size: 1.8rem; font-weight: 700; letter-spacing: -0.5px; }
        .alert { padding: 14px 20px; border-radius: 10px; font-size: 0.875rem; font-weight: 500; margin-bottom: 24px; display: flex; align-items: center; gap: 10px; }
        .alert-success { background: rgba(16,185,129,0.1); border: 1px solid rgba(16,185,129,0.3); color: #10b981; }
        .alert-error { background: rgba(233,69,96,0.1); border: 1px solid rgba(233,69,96,0.3); color: var(--accent); }

        /* COURSE CARDS */
        .courses-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(340px, 1fr)); gap: 20px; }
        .course-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: 16px; padding: 24px;
            display: flex; flex-direction: column; gap: 16px;
            transition: transform 0.2s, border-color 0.2s;
            animation: fadeUp 0.5s ease both; position: relative; overflow: hidden;
        }
        .course-card::before {
            content: ''; position: absolute; top: 0; left: 0; right: 0; height: 3px;
            background: linear-gradient(90deg, var(--accent), var(--accent2));
            opacity: 0; transition: opacity 0.2s;
        }
        .course-card:hover { transform: translateY(-3px); border-color: rgba(233,69,96,0.3); }
        .course-card:hover::before { opacity: 1; }

        .course-top { display: flex; justify-content: space-between; align-items: flex-start; }
        .course-category {
            display: inline-block; background: rgba(233,69,96,0.15); color: var(--accent);
            font-size: 0.7rem; font-weight: 700; letter-spacing: 1px; text-transform: uppercase;
            padding: 4px 12px; border-radius: 20px;
        }
        .course-enrolled { font-size: 0.75rem; color: var(--muted); font-family: 'JetBrains Mono', monospace; }
        .course-name { font-size: 1rem; font-weight: 700; letter-spacing: -0.3px; line-height: 1.4; }

        /* PROGRESS BAR */
        .progress-section { display: flex; flex-direction: column; gap: 8px; }
        .progress-top { display: flex; justify-content: space-between; align-items: center; }
        .progress-label { font-size: 0.75rem; color: var(--muted); font-weight: 500; }
        .progress-pct { font-size: 0.75rem; font-weight: 700; color: var(--accent); font-family: 'JetBrains Mono', monospace; }
        .progress-bar-bg { width: 100%; height: 6px; background: var(--surface2); border-radius: 10px; overflow: hidden; }
        .progress-bar-fill { height: 100%; background: linear-gradient(90deg, var(--accent), var(--accent2)); border-radius: 10px; transition: width 0.8s ease; }

        /* BUTTONS */
        .course-actions { display: flex; gap: 10px; margin-top: auto; }
        .btn-continue {
            flex: 1; background: linear-gradient(135deg, var(--accent), var(--accent2));
            color: white; border: none; padding: 10px 16px; border-radius: 8px;
            font-family: 'Sora', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: opacity 0.2s; text-align: center; text-decoration: none;
            display: flex; align-items: center; justify-content: center;
        }
        .btn-continue:hover { opacity: 0.85; }
        .btn-unenroll {
            background: transparent; border: 1px solid rgba(233,69,96,0.3);
            color: var(--muted); padding: 10px 16px; border-radius: 8px;
            font-family: 'Sora', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-unenroll:hover { background: rgba(233,69,96,0.1); color: var(--accent); border-color: var(--accent); }

        /* EMPTY STATE */
        .empty-state { text-align: center; padding: 80px 20px; color: var(--muted); animation: fadeUp 0.5s ease both; }
        .empty-state-icon { font-size: 3.5rem; margin-bottom: 16px; }
        .empty-state h3 { font-size: 1.1rem; font-weight: 700; color: var(--text); margin-bottom: 8px; }
        .empty-state p { font-size: 0.875rem; margin-bottom: 24px; }
        .empty-state a {
            display: inline-block; background: var(--accent); color: white;
            text-decoration: none; padding: 12px 28px; border-radius: 8px;
            font-size: 0.875rem; font-weight: 600; transition: opacity 0.2s;
        }
        .empty-state a:hover { opacity: 0.85; }

        /* CONFIRM MODAL */
        .modal-overlay {
            display: none; position: fixed; inset: 0;
            background: rgba(0,0,0,0.7); backdrop-filter: blur(6px);
            z-index: 999; align-items: center; justify-content: center;
        }
        .modal-overlay.open { display: flex; }
        .modal {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: 20px; width: 100%; max-width: 400px; padding: 36px;
            text-align: center; animation: modalIn 0.3s cubic-bezier(0.34,1.56,0.64,1) both;
        }
        @keyframes modalIn { from { opacity: 0; transform: scale(0.9); } to { opacity: 1; transform: scale(1); } }
        .modal-icon { font-size: 3rem; margin-bottom: 16px; }
        .modal-title { font-size: 1.2rem; font-weight: 700; margin-bottom: 8px; }
        .modal-text { color: var(--muted); font-size: 0.875rem; margin-bottom: 28px; line-height: 1.6; }
        .modal-actions { display: flex; gap: 12px; }
        .btn-cancel {
            flex: 1; background: var(--surface2); border: 1px solid var(--border);
            color: var(--text); padding: 12px; border-radius: 10px;
            font-family: 'Sora', sans-serif; font-size: 0.875rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-cancel:hover { border-color: var(--muted); }
        .btn-confirm-unenroll {
            flex: 1; background: var(--accent); border: none;
            color: white; padding: 12px; border-radius: 10px;
            font-family: 'Sora', sans-serif; font-size: 0.875rem; font-weight: 600;
            cursor: pointer; transition: opacity 0.2s;
        }
        .btn-confirm-unenroll:hover { opacity: 0.85; }

        @keyframes fadeUp { from { opacity: 0; transform: translateY(16px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 900px) { .container { padding: 24px 20px; } .header, .nav { padding: 0 20px; } }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- HEADER -->
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" style="height:40px;width:40px;object-fit:contain;" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <div class="user-pill">
                    <div class="user-avatar"><asp:Label ID="lblAvatarInitial" runat="server" Text="S" /></div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <!-- NAV -->
        <div class="nav">
            <a href="StudentDashboard.aspx"><span>⊞</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>◎</span> Browse Courses</a>
            <a href="MyCourses.aspx" class="active"><span>▤</span> My Courses</a>
            <a href="Gamification.aspx"><span>◆</span> Achievements</a>
        </div>

        <div class="container">
            <div class="page-header">
                <div class="page-label">Learning</div>
                <div class="page-title">My Courses</div>
            </div>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />

            <!-- Hidden field for unenroll courseid -->
            <asp:HiddenField ID="hfUnenrollId" runat="server" />

            <!-- COURSE CARDS via Repeater -->
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
                                    <a href='LessonViewer.aspx?courseid=<%# Eval("courseid") %>' class="btn-continue">Continue →</a>
                                    <button type="button" class="btn-unenroll"
                                        onclick="confirmUnenroll(<%# Eval("courseid") %>, '<%# Eval("coursename") %>')">
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
                    <p>You haven't enrolled in any courses yet.</p>
                    <a href="BrowseCourses.aspx">Browse Courses</a>
                </div>
            </asp:Panel>

            <!-- Hidden button triggered by JS to submit unenroll -->
            <asp:Button ID="btnUnenrollConfirm" runat="server" Text="Confirm"
                CssClass="hidden-btn" OnClick="btnUnenrollConfirm_Click" />
        </div>

        <!-- UNENROLL CONFIRM MODAL -->
        <div class="modal-overlay" id="unenrollModal">
            <div class="modal">
                <div class="modal-icon">⚠️</div>
                <div class="modal-title">Unenroll from course?</div>
                <div class="modal-text">
                    You are about to unenroll from <strong id="modalCourseName"></strong>.<br/>
                    Your progress will be lost. This cannot be undone.
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

        document.getElementById('unenrollModal').addEventListener('click', function(e) {
            if (e.target === this) closeModal();
        });
    </script>
</body>
</html>