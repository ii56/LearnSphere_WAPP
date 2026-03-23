<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forums.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.Forums" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Course Forums - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --surface-hover: #f8fafd;
            --primary: #2563eb;
            --primary-bg: rgba(37,99,235,0.08);
            --primary-border: rgba(37,99,235,0.18);
            --accent-purple: #8b5cf6;
            --accent-green: #10b981;
            --accent-orange: #f59e0b;
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

        /* ═══ BANNERS & CONTAINER ═══ */
        .container { max-width: 1000px; margin: 0 auto; padding: 28px 36px; }
        .page-header {
            border-radius: var(--radius); padding: 32px 36px; margin-bottom: 24px;
            position: relative; overflow: hidden; animation: slideDown 0.5s ease both;
        }
        .banner-forums { background: linear-gradient(135deg, #2563eb 0%, #3b82f6 50%, #60a5fa 100%); box-shadow: 0 8px 30px rgba(37,99,235,0.25); }
        .banner-questions { background: linear-gradient(135deg, #059669 0%, #10b981 50%, #34d399 100%); box-shadow: 0 8px 30px rgba(16,185,129,0.25); }
        .banner-detail { background: linear-gradient(135deg, #7c3aed 0%, #8b5cf6 50%, #a78bfa 100%); box-shadow: 0 8px 30px rgba(139,92,246,0.25); }
        .page-header::before {
            content: ''; position: absolute; top: -40%; right: -10%; width: 300px; height: 300px;
            background: radial-gradient(circle, rgba(255,255,255,0.12), transparent 65%); border-radius: 50%;
        }
        .page-header-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px; text-transform: uppercase;
            color: rgba(255,255,255,0.7); margin-bottom: 8px; font-family: 'Space Mono', monospace;
        }
        .page-header-title { font-size: 1.75rem; font-weight: 700; color: white; margin-bottom: 6px; }
        .page-header-sub { color: rgba(255,255,255,0.75); font-size: 0.88rem; }

        .btn-back {
            display: inline-flex; align-items: center; gap: 8px; background: var(--surface);
            border: 1px solid var(--border); color: var(--text-secondary); padding: 8px 18px;
            border-radius: 8px; font-family: 'DM Sans', sans-serif; font-size: 0.83rem;
            font-weight: 600; cursor: pointer; transition: all 0.2s; margin-bottom: 20px;
        }
        .btn-back:hover { border-color: var(--primary); color: var(--primary); }

        .alert { padding: 12px 18px; border-radius: var(--radius-sm); font-size: 0.85rem; margin-bottom: 16px; display: block; }
        .alert-success { background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2); color: #059669; }
        .alert-error { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }

        /* ═══ FORUM LIST & QUESTION LIST ═══ */
        .search-bar { display: flex; gap: 12px; margin-bottom: 20px; animation: slideUp 0.4s ease both; }
        .search-input {
            flex: 1; padding: 10px 16px; border: 1px solid var(--border); border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.88rem; background: var(--surface); outline: none;
        }
        .search-input:focus { border-color: var(--primary); }
        .btn-primary {
            background: var(--primary); color: white; border: none; padding: 10px 22px;
            border-radius: var(--radius-sm); font-family: 'DM Sans', sans-serif; font-size: 0.85rem;
            font-weight: 600; cursor: pointer; transition: background 0.2s; display: inline-flex; align-items: center; gap: 8px;
        }
        .btn-primary:hover { background: #1d4ed8; }

        .list-grid { display: flex; flex-direction: column; gap: 14px; }
        .card-row {
            background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius);
            padding: 22px 26px; box-shadow: var(--shadow-sm); display: flex; align-items: center;
            justify-content: space-between; transition: transform 0.2s, box-shadow 0.2s; text-decoration: none; color: var(--text);
            animation: slideUp 0.4s ease both; cursor: pointer;
        }
        .card-row:hover { transform: translateY(-2px); box-shadow: var(--shadow-md); border-color: var(--primary-border); }
        .card-left { display: flex; gap: 18px; flex: 1; }
        .card-icon {
            width: 48px; height: 48px; background: var(--primary-bg); border-radius: 12px;
            display: flex; align-items: center; justify-content: center; font-size: 1.4rem; flex-shrink: 0;
        }
        .card-info { flex: 1; }
        .card-title { font-size: 1rem; font-weight: 700; margin-bottom: 6px; color: var(--text); }
        .card-desc { font-size: 0.85rem; color: var(--text-secondary); margin-bottom: 10px; line-height: 1.5; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
        .card-meta { display: flex; align-items: center; gap: 12px; }
        .card-course-tag { background: var(--primary-bg); color: var(--primary); padding: 3px 12px; border-radius: 20px; font-size: 0.75rem; font-weight: 600; }
        .card-date { font-size: 0.75rem; color: var(--text-muted); font-family: 'Space Mono', monospace; }
        .card-right { display: flex; align-items: center; gap: 20px; flex-shrink: 0; text-align: center; }
        .stat-num { font-size: 1.2rem; font-weight: 700; font-family: 'Space Mono', monospace; color: var(--primary); }
        .stat-label { font-size: 0.7rem; color: var(--text-muted); font-weight: 600; text-transform: uppercase; }

        .empty-state { text-align: center; padding: 60px 20px; background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); color: var(--text-secondary); }
        .empty-icon { font-size: 3rem; margin-bottom: 14px; opacity: 0.5; }

        /* ═══ DETAIL & ANSWERS ═══ */
        .detail-box {
            background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius);
            padding: 32px; box-shadow: var(--shadow-sm); margin-bottom: 24px; animation: slideDown 0.5s ease both;
        }
        .detail-title { font-size: 1.4rem; font-weight: 700; margin-bottom: 14px; line-height: 1.4; }
        .detail-content { font-size: 0.95rem; color: var(--text-secondary); line-height: 1.7; margin-bottom: 24px; }
        .detail-footer { display: flex; align-items: center; justify-content: space-between; border-top: 1px solid var(--border-light); padding-top: 16px; }
        .author-chip { display: flex; align-items: center; gap: 10px; font-size: 0.85rem; font-weight: 600; color: var(--text); }
        .author-avatar { width: 32px; height: 32px; border-radius: 50%; object-fit: cover; background: var(--border-light); }
        
        .vote-row { display: flex; align-items: center; gap: 8px; }
        .vote-btn {
            background: var(--border-light); border: 1px solid var(--border); color: var(--text-secondary);
            padding: 6px 14px; border-radius: 20px; font-size: 0.85rem; font-weight: 600; cursor: pointer; transition: all 0.2s;
        }
        .vote-btn:hover { border-color: var(--primary); color: var(--primary); background: var(--primary-bg); }
        
        .action-group { display: flex; gap: 8px; }
        .btn-edit-sm { background: rgba(37,99,235,0.08); color: var(--primary); border: 1px solid var(--primary-border); padding: 6px 14px; border-radius: 8px; font-size: 0.8rem; font-weight: 600; cursor: pointer; transition: all 0.2s; }
        .btn-edit-sm:hover { background: var(--primary); color: white; }
        .btn-delete-sm { background: rgba(239,68,68,0.08); color: var(--accent-red); border: 1px solid rgba(239,68,68,0.2); padding: 6px 14px; border-radius: 8px; font-size: 0.8rem; font-weight: 600; cursor: pointer; transition: all 0.2s; }
        .btn-delete-sm:hover { background: var(--accent-red); color: white; }

        .answers-header { font-size: 1.1rem; font-weight: 700; margin-bottom: 16px; display: flex; align-items: center; gap: 10px; }
        .answers-header span { background: var(--primary-bg); color: var(--primary); padding: 3px 12px; border-radius: 20px; font-size: 0.8rem; font-family: 'Space Mono', monospace; }
        
        .answer-card {
            background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius);
            padding: 24px; box-shadow: var(--shadow-sm); margin-bottom: 16px; animation: slideUp 0.4s ease both;
        }

        /* ═══ FORMS (ASK / ANSWER / EDIT) ═══ */
        .form-card {
            background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius);
            padding: 32px; box-shadow: var(--shadow-sm); margin-bottom: 24px; animation: slideUp 0.4s ease both;
        }
        .form-title { font-size: 1.2rem; font-weight: 700; margin-bottom: 20px; }
        .form-group { margin-bottom: 20px; }
        .form-label { display: block; font-size: 0.85rem; font-weight: 600; margin-bottom: 8px; color: var(--text); }
        .form-input, .form-textarea {
            width: 100%; padding: 12px 14px; border: 1px solid var(--border); border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.9rem; color: var(--text); outline: none; transition: border-color 0.2s; box-sizing: border-box;
        }
        .form-input:focus, .form-textarea:focus { border-color: var(--primary); }
        .form-textarea { resize: vertical; min-height: 140px; line-height: 1.6; }
        .form-actions { display: flex; gap: 12px; margin-top: 24px; }
        
        @keyframes slideDown { from { opacity: 0; transform: translateY(-12px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
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
            <a href="Forums.aspx" class="active"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
            <a href="EditProfile.aspx"><span>👤</span> Profile</a>
        </div>

        <div class="container">
            <asp:Label ID="lblGlobalMsg" runat="server" Visible="false" />

            <asp:Panel ID="pnlForumList" runat="server">
                <div class="page-header banner-forums">
                    <div class="page-header-label">Community</div>
                    <div class="page-header-title">Course Forums</div>
                    <div class="page-header-sub">Browse discussions for courses you are enrolled in.</div>
                </div>

                <div class="search-bar">
                    <asp:TextBox ID="txtSearchForums" runat="server" CssClass="search-input" placeholder="Search forums by title or course name..." />
                    <asp:Button ID="btnSearchForums" runat="server" Text="Search" CssClass="btn-primary" OnClick="btnSearchForums_Click" />
                    <asp:Button ID="btnClearForums" runat="server" Text="Clear" CssClass="btn-back" style="margin-bottom:0;" OnClick="btnClearForums_Click" />
                </div>

                <div class="list-grid">
                    <asp:Repeater ID="rptForums" runat="server" OnItemCommand="rptForums_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="OpenForum" CommandArgument='<%# Eval("forumid") %>' CssClass="card-row">
                                <div class="card-left">
                                    <div class="card-icon">💬</div>
                                    <div class="card-info">
                                        <div class="card-title"><%# Eval("title") %></div>
                                        <div class="card-desc"><%# Eval("description") %></div>
                                        <div class="card-meta">
                                            <span class="card-course-tag"><%# Eval("coursename") %></span>
                                            <span class="card-date">Created <%# Convert.ToDateTime(Eval("creationtime")).ToString("MMM dd, yyyy") %></span>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-right">
                                    <div style="text-align: center;">
                                        <div class="stat-num"><%# Eval("postcount") %></div>
                                        <div class="stat-label">Posts</div>
                                    </div>
                                    <span style="color:var(--text-muted); font-size:1.5rem;">→</span>
                                </div>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <asp:Panel ID="pnlEmptyForums" runat="server" Visible="false">
                    <div class="empty-state">
                        <div class="empty-icon">💬</div>
                        <p>No forums found. Enroll in more courses to join discussions!</p>
                    </div>
                </asp:Panel>
            </asp:Panel>


            <asp:Panel ID="pnlQuestionList" runat="server" Visible="false">
                <div class="page-header banner-questions">
                    <div class="page-header-label">Forum</div>
                    <div class="page-header-title"><asp:Label ID="lblQLForumTitle" runat="server" /></div>
                    <div class="page-header-sub"><asp:Label ID="lblQLCourseName" runat="server" /></div>
                </div>

                <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:20px;">
                    <asp:Button ID="btnBackToForums" runat="server" Text="← Back to Forums" CssClass="btn-back" style="margin-bottom:0;" OnClick="btnBackToForums_Click" />
                    <asp:Button ID="btnOpenAsk" runat="server" Text="+ Ask a Question" CssClass="btn-primary" OnClick="btnOpenAsk_Click" />
                </div>

                <div class="list-grid">
                    <asp:Repeater ID="rptQuestions" runat="server" OnItemCommand="rptQuestions_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="OpenQuestion" CommandArgument='<%# Eval("postid") %>' CssClass="card-row">
                                <div class="card-left">
                                    <div class="card-info">
                                        <div class="card-title"><%# Eval("title") %></div>
                                        <div class="card-desc">
                                            <%# Eval("content").ToString().Length > 150 ? Eval("content").ToString().Substring(0, 150) + "..." : Eval("content") %>
                                        </div>
                                        <div class="card-meta">
                                            <div style="display:flex; align-items:center; gap:6px; font-size:0.8rem; font-weight:600; color:var(--text-secondary);">
                                                <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' style="width:20px; height:20px; border-radius:50%; object-fit:cover;" />
                                                <%# Eval("fname") %> <%# Eval("lname") %>
                                            </div>
                                            <span class="card-date"><%# Convert.ToDateTime(Eval("creationtime")).ToString("MMM dd, yyyy") %></span>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-right">
                                    <div style="text-align: center; margin-right:15px;">
                                        <div class="stat-num" style="color:var(--text);"><%# Convert.ToInt32(Eval("upvotes")) - Convert.ToInt32(Eval("downvotes")) %></div>
                                        <div class="stat-label">Votes</div>
                                    </div>
                                    <div style="text-align: center;">
                                        <div class="stat-num"><%# Eval("replycount") %></div>
                                        <div class="stat-label">Replies</div>
                                    </div>
                                </div>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <asp:Panel ID="pnlEmptyQuestions" runat="server" Visible="false">
                    <div class="empty-state">
                        <div class="empty-icon">❓</div>
                        <p>No questions yet. Be the first to ask!</p>
                    </div>
                </asp:Panel>
            </asp:Panel>


            <asp:Panel ID="pnlQuestionDetail" runat="server" Visible="false">
                <div class="page-header banner-detail">
                    <div class="page-header-label">Discussion Thread</div>
                    <div class="page-header-title">Question Details</div>
                    <div class="page-header-sub">Read the full discussion and provide your answers.</div>
                </div>

                <asp:Button ID="btnBackToQuestions" runat="server" Text="← Back to Questions" CssClass="btn-back" OnClick="btnBackToQuestions_Click" />

                <div class="detail-box">
                    <div class="detail-title"><asp:Label ID="lblDetailTitle" runat="server" /></div>
                    <div class="detail-content"><asp:Label ID="lblDetailContent" runat="server" /></div>
                    
                    <asp:Panel ID="pnlDetailAttachment" runat="server" Visible="false" style="margin-bottom:20px;">
                        <asp:HyperLink ID="hlDetailFile" runat="server" Target="_blank" CssClass="btn-back" style="margin-bottom:0; background:var(--primary-bg); color:var(--primary); border-color:var(--primary-border);">
                            📎 View Attached Document
                        </asp:HyperLink>
                    </asp:Panel>

                    <div class="detail-footer">
                        <div class="author-chip">
                            <asp:Image ID="imgDetailAuthor" runat="server" CssClass="author-avatar" />
                            <div>
                                <asp:Label ID="lblDetailAuthorName" runat="server" />
                                <div style="font-size:0.75rem; color:var(--text-muted); font-weight:400; font-family:'Space Mono', monospace;">
                                    <asp:Label ID="lblDetailDate" runat="server" />
                                </div>
                            </div>
                        </div>

                        <div style="display:flex; align-items:center; gap:20px;">
                            <div class="action-group" id="divDetailOwnerActions" runat="server" visible="false">
                                <asp:Button ID="btnEditQuestion" runat="server" Text="✏️ Edit" CssClass="btn-edit-sm" OnClick="btnEditQuestion_Click" />
                                <asp:Button ID="btnDeleteQuestion" runat="server" Text="🗑️ Delete" CssClass="btn-delete-sm" OnClick="btnDeleteQuestion_Click" OnClientClick="return confirm('Delete this question and all its answers?');" />
                            </div>

                            <div class="vote-row">
                                <asp:LinkButton ID="btnUpvoteQ" runat="server" CssClass="vote-btn" OnClick="btnUpvoteQ_Click">
                                    👍 <asp:Label ID="lblUpvotesQ" runat="server" Text="0" />
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDownvoteQ" runat="server" CssClass="vote-btn" OnClick="btnDownvoteQ_Click">
                                    👎 <asp:Label ID="lblDownvotesQ" runat="server" Text="0" />
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="answers-header">
                    Answers <span><asp:Label ID="lblAnswerCount" runat="server" Text="0" /></span>
                </div>

                <asp:Repeater ID="rptAnswers" runat="server" OnItemCommand="rptAnswers_ItemCommand">
                    <ItemTemplate>
                        <div class="answer-card">
                            <div class="detail-content" style="font-size:0.9rem; margin-bottom:16px;">
                                <%# Eval("content") %>
                            </div>
                            
                            <asp:Panel runat="server" Visible='<%# Eval("fileurl") != DBNull.Value && !string.IsNullOrEmpty(Eval("fileurl").ToString()) %>' style="margin-bottom:16px;">
                                <a href='<%# ResolveUrl(Eval("fileurl").ToString()) %>' target="_blank" class="btn-back" style="margin-bottom:0; padding:4px 12px; font-size:0.75rem;">
                                    📎 View Attachment
                                </a>
                            </asp:Panel>

                            <div class="detail-footer">
                                <div class="author-chip">
                                    <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' class="author-avatar" />
                                    <div>
                                        <%# Eval("fname") %> <%# Eval("lname") %>
                                        <div style="font-size:0.7rem; color:var(--text-muted); font-weight:400; font-family:'Space Mono', monospace;">
                                            <%# Convert.ToDateTime(Eval("creationtime")).ToString("MMM dd, yyyy - hh:mm tt") %>
                                        </div>
                                    </div>
                                </div>
                                <div style="display:flex; align-items:center; gap:20px;">
                                    <div class="action-group" visible='<%# IsOwner(Eval("userid")) %>' runat="server">
                                        <asp:Button runat="server" Text="✏️ Edit" CssClass="btn-edit-sm" CommandName="EditAnswer" CommandArgument='<%# Eval("postid") %>' />
                                        <asp:Button runat="server" Text="🗑️ Delete" CssClass="btn-delete-sm" CommandName="DeleteAnswer" CommandArgument='<%# Eval("postid") %>' OnClientClick="return confirm('Delete this answer?');" />
                                    </div>
                                    <div class="vote-row">
                                        <asp:LinkButton runat="server" CssClass="vote-btn" CommandName="Upvote" CommandArgument='<%# Eval("postid") %>'>
                                            👍 <%# Eval("upvotes") %>
                                        </asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="vote-btn" CommandName="Downvote" CommandArgument='<%# Eval("postid") %>'>
                                            👎 <%# Eval("downvotes") %>
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoAnswers" runat="server" Visible="false">
                    <div class="empty-state" style="padding:40px 20px;">
                        <p style="margin:0;">No answers yet. Be the first to help out!</p>
                    </div>
                </asp:Panel>

                <div class="form-card" style="margin-top:30px;">
                    <div class="form-title">Post Your Answer</div>
                    <div class="form-group">
                        <asp:TextBox ID="txtNewAnswer" runat="server" CssClass="form-textarea" TextMode="MultiLine" placeholder="Write your clear and helpful answer here..." />
                    </div>
                    <div class="form-group" style="margin-bottom:0;">
                        <label class="form-label" style="font-size:0.75rem; color:var(--text-muted);">Attach Document (PDF, DOCX, ZIP) - Optional</label>
                        <asp:FileUpload ID="fuAnswerFile" runat="server" CssClass="form-input" style="padding:8px;" />
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnPostAnswer" runat="server" Text="Submit Answer (+5 Pts)" CssClass="btn-primary" OnClick="btnPostAnswer_Click" />
                    </div>
                </div>
            </asp:Panel>


            <asp:Panel ID="pnlPostForm" runat="server" Visible="false">
                <asp:Button ID="btnBackFromForm" runat="server" Text="← Cancel" CssClass="btn-back" OnClick="btnBackFromForm_Click" CausesValidation="false" />

                <div class="form-card">
                    <div class="form-title"><asp:Label ID="lblFormModeTitle" runat="server" /></div>
                    
                    <asp:Panel ID="pnlFormTitleGroup" runat="server" CssClass="form-group">
                        <label class="form-label">Title *</label>
                        <asp:TextBox ID="txtPostTitle" runat="server" CssClass="form-input" MaxLength="300" placeholder="Be specific and clear..." />
                    </asp:Panel>

                    <div class="form-group">
                        <label class="form-label">Content *</label>
                        <asp:TextBox ID="txtPostContent" runat="server" CssClass="form-textarea" TextMode="MultiLine" placeholder="Describe the details..." />
                    </div>

                    <div class="form-group">
                        <label class="form-label">Attach File (PDF, DOCX, ZIP) - Optional</label>
                        <asp:FileUpload ID="fuPostFile" runat="server" CssClass="form-input" style="padding:8px;" />
                    </div>

                    <div class="form-actions">
                        <asp:Button ID="btnSavePost" runat="server" Text="Save Post" CssClass="btn-primary" OnClick="btnSavePost_Click" />
                    </div>
                </div>
            </asp:Panel>

        </div>
        
        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>
</body>
</html>