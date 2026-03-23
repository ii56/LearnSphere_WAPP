<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneralDashboard.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.GeneralDashboard" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>General Dashboard - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --surface-hover: #f8fafd;
            --primary: #2563eb;
            --primary-light: #3b82f6;
            --primary-bg: rgba(37,99,235,0.08);
            --primary-border: rgba(37,99,235,0.18);
            --accent-orange: #f59e0b;
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
            --accent-red: #ef4444;
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
            background: var(--bg-gradient);
            background-attachment: fixed;
            color: var(--text);
            min-height: 100vh;
        }

        /* ═══ HEADER ═══ */
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82);
            backdrop-filter: blur(20px);
            -webkit-backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }

        .logo { display: flex; align-items: center; gap: 12px; text-decoration: none; }
        .logo img { height: 38px; width: 38px; object-fit: contain; }
        .logo-text { font-size: 1.2rem; font-weight: 700; color: var(--text); letter-spacing: -0.3px; }
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
        .user-avatar img { width: 100%; height: 100%; object-fit: cover; border-radius: 50%; position: absolute; top: 0; left: 0; }
        .user-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }

        .btn-logout {
            background: transparent; border: 1px solid var(--border); color: var(--text-secondary);
            padding: 7px 18px; border-radius: 8px; font-family: 'DM Sans', sans-serif;
            font-size: 0.82rem; font-weight: 600; cursor: pointer; transition: all 0.2s;
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

        /* ═══ MAIN CONTAINER ═══ */
        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }

        /* ═══ NOTIFICATION ALERT ═══ */
        .alert-notification {
            padding: 16px 24px; border-radius: var(--radius); margin-bottom: 24px;
            display: flex; align-items: center; gap: 14px; font-size: 0.9rem; font-weight: 600;
            animation: slideDown 0.5s ease both; box-shadow: var(--shadow-sm);
        }
        .alert-notification.pending { background: rgba(245,158,11,0.08); border: 1px solid rgba(245,158,11,0.25); color: var(--accent-orange); }
        .alert-notification.approved { background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.25); color: #059669; }
        .alert-notification.denied { background: rgba(239,68,68,0.08); border: 1px solid rgba(239,68,68,0.25); color: var(--accent-red); }
        .alert-icon { font-size: 1.4rem; }

        /* ═══ WELCOME BANNER ═══ */
        .welcome-banner {
            background: linear-gradient(135deg, #2563eb 0%, #3b82f6 50%, #60a5fa 100%);
            border-radius: var(--radius); padding: 32px 36px; margin-bottom: 24px;
            position: relative; overflow: hidden; box-shadow: 0 8px 30px rgba(37,99,235,0.25);
            animation: slideDown 0.5s 0.1s ease both;
        }
        .welcome-banner::before {
            content: ''; position: absolute; top: -40%; right: -10%; width: 300px; height: 300px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%);
            border-radius: 50%; pointer-events: none;
        }
        .welcome-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px; text-transform: uppercase;
            color: rgba(255,255,255,0.7); margin-bottom: 8px; font-family: 'Space Mono', monospace;
        }
        .welcome-name { font-size: 1.75rem; font-weight: 700; color: white; letter-spacing: -0.5px; margin-bottom: 6px; }
        .welcome-sub { color: rgba(255,255,255,0.75); font-size: 0.88rem; }

        /* ═══ STATS ═══ */
        .stats-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 24px; }
        .stat-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 22px 24px; box-shadow: var(--shadow-sm);
            transition: transform 0.2s, box-shadow 0.2s; animation: slideUp 0.4s ease both;
            position: relative; overflow: hidden;
        }
        .stat-card:hover { transform: translateY(-2px); box-shadow: var(--shadow-md); }
        .stat-card::after { content: ''; position: absolute; top: 0; left: 0; width: 100%; height: 3px; }
        .stat-card:nth-child(1)::after { background: var(--primary); }
        .stat-card:nth-child(2)::after { background: var(--accent-green); }
        .stat-card:nth-child(3)::after { background: var(--accent-orange); }
        .stat-card:nth-child(4)::after { background: var(--accent-purple); }

        .stat-icon {
            width: 42px; height: 42px; border-radius: 10px; display: flex; align-items: center; justify-content: center;
            font-size: 1.2rem; margin-bottom: 14px;
        }
        .stat-card:nth-child(1) .stat-icon { background: rgba(37,99,235,0.1); }
        .stat-card:nth-child(2) .stat-icon { background: rgba(16,185,129,0.1); }
        .stat-card:nth-child(3) .stat-icon { background: rgba(245,158,11,0.1); }
        .stat-card:nth-child(4) .stat-icon { background: rgba(139,92,246,0.1); }

        .stat-value {
            font-size: 2rem; font-weight: 700; font-family: 'Space Mono', monospace;
            letter-spacing: -1px; line-height: 1; margin-bottom: 6px;
        }
        .stat-card:nth-child(1) .stat-value { color: var(--primary); }
        .stat-card:nth-child(2) .stat-value { color: var(--accent-green); }
        .stat-card:nth-child(3) .stat-value { color: var(--accent-orange); }
        .stat-card:nth-child(4) .stat-value { color: var(--accent-purple); }
        .stat-label { font-size: 0.78rem; color: var(--text-secondary); font-weight: 600; letter-spacing: 0.3px; }

        .badge-pill {
            display: inline-block; background: linear-gradient(135deg, rgba(139,92,246,0.1), rgba(139,92,246,0.15));
            border: 1px solid rgba(139,92,246,0.25); color: var(--accent-purple);
            font-size: 0.72rem; font-weight: 700; letter-spacing: 0.8px; text-transform: uppercase;
            padding: 4px 12px; border-radius: 20px; font-family: 'Space Mono', monospace;
        }

        /* ═══ SECTION CARDS ═══ */
        .section-header { padding: 18px 0; display: flex; align-items: center; justify-content: space-between; margin-top: 10px;}
        .section-title { font-size: 1.1rem; font-weight: 700; color: var(--text); display: flex; align-items: center; gap: 10px; }
        .section-title-dot { width: 8px; height: 8px; border-radius: 50%; background: var(--primary); }
        .dot-green { background: var(--accent-green); }

        .course-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 18px; margin-bottom: 30px;}
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
        .course-name { font-size: 1.05rem; font-weight: 700; line-height: 1.4; color: var(--text); margin-bottom: 5px;}

        /* Progress Bar */
        .progress-section { display: flex; flex-direction: column; gap: 6px; }
        .progress-top { display: flex; justify-content: space-between; align-items: center; }
        .progress-label { font-size: 0.75rem; color: var(--text-muted); font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }
        .progress-pct { font-size: 0.75rem; font-weight: 700; color: var(--primary); font-family: 'Space Mono', monospace; }
        .progress-bar-bg { width: 100%; height: 6px; background: var(--border-light); border-radius: 10px; overflow: hidden; }
        .progress-bar-fill { height: 100%; background: linear-gradient(90deg, var(--primary), var(--accent-purple)); border-radius: 10px; transition: width 0.8s ease; }

        .price-tag {
            display: inline-block; background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2);
            color: #059669; padding: 4px 12px; border-radius: 20px; font-size: 0.8rem; font-weight: 700; font-family: 'Space Mono', monospace;
        }

        .course-actions { display: flex; gap: 10px; margin-top: auto; padding-top: 10px; border-top: 1px solid var(--border-light); }
        .btn-continue, .btn-details {
            flex: 1; color: white; border: none;
            padding: 10px 16px; border-radius: 8px; font-family: 'DM Sans', sans-serif;
            font-size: 0.85rem; font-weight: 600; cursor: pointer; text-align: center;
            text-decoration: none; display: flex; align-items: center; justify-content: center;
            transition: background 0.2s;
        }
        .btn-continue { background: var(--primary); }
        .btn-continue:hover { background: #1d4ed8; }
        
        .btn-details { background: var(--border-light); color: var(--text); border: 1px solid var(--border); }
        .btn-details:hover { background: var(--surface-hover); border-color: var(--primary-border); color: var(--primary); }

        .empty-state { text-align: center; padding: 60px 20px; background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); color: var(--text-secondary); }
        .empty-state-icon { font-size: 3rem; margin-bottom: 14px; opacity: 0.5; }
        .empty-state h3 { font-size: 1.1rem; font-weight: 700; color: var(--text); margin-bottom: 8px; }
        .empty-state p { font-size: 0.875rem; margin-bottom: 20px; }
        .empty-state a {
            display: inline-block; background: var(--primary); color: white; text-decoration: none;
            padding: 10px 24px; border-radius: 8px; font-size: 0.85rem; font-weight: 600; transition: background 0.2s;
        }
        .empty-state a:hover { background: #1d4ed8; }

        @keyframes slideDown { from { opacity: 0; transform: translateY(-12px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 900px) {
            .stats-grid { grid-template-columns: repeat(2, 1fr); }
            .container { padding: 20px; } .header, .nav { padding: 0 20px; }
            .welcome-name { font-size: 1.4rem; }
        }
        @media (max-width: 500px) { .stats-grid { grid-template-columns: 1fr; } }
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
                        <asp:Label ID="lblAvatarInitial" runat="server" Text="G" />
                    </div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="nav">
            <a href="GeneralDashboard.aspx" class="active"><span>📊</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>📚</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span>🎓</span> My Learning</a>
            <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
            <a href="EditProfile.aspx"><span>👤</span> Profile</a>
        </div>

        <div class="container">

            <asp:Panel ID="pnlNotification" runat="server" Visible="false">
                <div id="notificationWrapper" runat="server" class="alert-notification">
                    <span class="alert-icon"><asp:Literal ID="litNotificationIcon" runat="server" /></span>
                    <div>
                        <div style="margin-bottom: 2px;"><strong>Lecturer Upgrade Request</strong></div>
                        <asp:Literal ID="litNotificationText" runat="server" />
                    </div>
                </div>
            </asp:Panel>

            <div class="welcome-banner">
                <div class="welcome-label">General User Portal</div>
                <div class="welcome-name">Welcome, <asp:Label ID="lblWelcome" runat="server" />!</div>
                <div class="welcome-sub">Enjoy access to our free courses. Upgrade to Student by purchasing any paid course!</div>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-icon">📚</div>
                    <div class="stat-value"><asp:Label ID="lblEnrolled" runat="server" Text="0" /></div>
                    <div class="stat-label">Enrolled Free Courses</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">✅</div>
                    <div class="stat-value"><asp:Label ID="lblCompleted" runat="server" Text="0" /></div>
                    <div class="stat-label">Completed Lessons</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">⚡</div>
                    <div class="stat-value"><asp:Label ID="lblPoints" runat="server" Text="0" /></div>
                    <div class="stat-label">Total Points</div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon">🏅</div>
                    <div class="stat-value" style="font-size:1rem; padding-top:4px;">
                        <span class="badge-pill"><asp:Label ID="lblBadge" runat="server" Text="Bronze" /></span>
                    </div>
                    <div class="stat-label">Current Badge</div>
                </div>
            </div>

            <div class="section-header">
                <div class="section-title"><span class="section-title-dot"></span> Continue Learning</div>
            </div>

            <asp:Panel ID="pnlCourses" runat="server">
                <div class="course-grid">
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
                                        <span class="progress-label">Completion</span>
                                        <span class="progress-pct"><%# Eval("Progress") %>%</span>
                                    </div>
                                    <div class="progress-bar-bg">
                                        <div class="progress-bar-fill" style="width: <%# Eval("Progress") %>%"></div>
                                    </div>
                                </div>

                                <div class="course-actions">
                                    <a href='CourseDetails.aspx?courseid=<%# Eval("courseid") %>' class="btn-details">Course Details</a>
                                    <a href='LessonViewer.aspx?courseid=<%# Eval("courseid") %>' class="btn-continue">Continue →</a>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmptyCourses" runat="server" Visible="false">
                <div class="empty-state">
                    <div class="empty-state-icon">📭</div>
                    <h3>No active courses</h3>
                    <p>Enroll in a free course to get started.</p>
                    <a href="BrowseCourses.aspx">Browse Free Courses</a>
                </div>
            </asp:Panel>


            <div class="section-header">
                <div class="section-title"><span class="section-title-dot dot-green"></span> Recommended For You</div>
            </div>

            <asp:Panel ID="pnlRecommended" runat="server">
                <div class="course-grid">
                    <asp:Repeater ID="rptRecommended" runat="server">
                        <ItemTemplate>
                            <div class="course-card">
                                <div class="course-top">
                                    <span class="course-category"><%# Eval("category") %></span>
                                    <span class="course-enrolled">New</span>
                                </div>
                                <div class="course-name"><%# Eval("coursename") %></div>
                                <div style="flex:1; font-size:0.85rem; color:var(--text-secondary); line-height:1.6;">
                                    <%# Eval("description").ToString().Length > 90 ? Eval("description").ToString().Substring(0, 90) + "..." : Eval("description") %>
                                </div>
                                
                                <div class="course-actions" style="align-items:center;">
                                    <%# Convert.ToDecimal(Eval("price")) == 0 
                                        ? "<span class=\"price-tag\">FREE</span>" 
                                        : "<span class=\"price-tag\" style=\"background:rgba(37,99,235,0.08);color:var(--primary);border-color:rgba(37,99,235,0.2);\">RM " + string.Format("{0:N2}", Eval("price")) + "</span>" %>
                                    
                                    <a href='CourseDetails.aspx?courseid=<%# Eval("courseid") %>' class="btn-continue" style="margin-left:auto; flex:none;">View Details →</a>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmptyRecommended" runat="server" Visible="false">
                <div class="empty-state">
                    <p>No recommendations available at the moment.</p>
                </div>
            </asp:Panel>

        </div>
    </form>
</body>
</html>