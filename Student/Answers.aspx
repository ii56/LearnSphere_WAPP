<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Answers.aspx.cs" Inherits="LearnSphere_WAPP.Student.Answers" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Answers - LearnSphere</title>
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
            --text: #1e293b;
            --text-secondary: #64748b;
            --text-muted: #94a3b8;
            --border: #e2e8f0;
            --border-light: #f1f5f9;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04);
            --shadow-md: 0 4px 12px rgba(0,0,0,0.06);
            --radius: 14px;
            --radius-sm: 10px;
        }
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: 'DM Sans', sans-serif;
            background: var(--bg-gradient);
            background-attachment: fixed;
            color: var(--text); min-height: 100vh;
        }
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82);
            backdrop-filter: blur(20px);
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
            border-radius: 50%; display: flex; align-items: center;
            justify-content: center; font-size: 13px; font-weight: 700; color: white;
        }
        .user-name { font-size: 0.85rem; font-weight: 600; }
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
            color: var(--text-muted); text-decoration: none;
            padding: 14px 18px; font-size: 0.85rem; font-weight: 600;
            display: flex; align-items: center; gap: 8px;
            border-bottom: 2.5px solid transparent; transition: all 0.2s;
        }
        .nav a:hover { color: var(--text-secondary); }
        .nav a.active { color: var(--primary); border-bottom-color: var(--primary); }
        .container { max-width: 860px; margin: 0 auto; padding: 28px 36px; }

        .breadcrumb {
            display: flex; align-items: center; gap: 8px;
            font-size: 0.82rem; color: var(--text-muted); margin-bottom: 20px;
        }
        .breadcrumb a { color: var(--primary); text-decoration: none; }
        .breadcrumb a:hover { text-decoration: underline; }

        /* the original question at the top */
        .question-box {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 28px;
            box-shadow: var(--shadow-sm); margin-bottom: 24px;
            animation: slideDown 0.5s ease both;
        }
        .question-box-label {
            font-size: 0.7rem; font-weight: 700; letter-spacing: 1.5px;
            text-transform: uppercase; color: var(--primary);
            margin-bottom: 10px; font-family: 'Space Mono', monospace;
        }
        .question-box-title { font-size: 1.3rem; font-weight: 700; margin-bottom: 14px; line-height: 1.4; }
        .question-box-content { font-size: 0.9rem; color: var(--text-secondary); line-height: 1.7; margin-bottom: 16px; }
        .question-box-footer { display: flex; align-items: center; justify-content: space-between; }
        .question-box-meta { display: flex; align-items: center; gap: 12px; }
        .author-chip {
            display: flex; align-items: center; gap: 8px;
            font-size: 0.8rem; font-weight: 600; color: var(--text-secondary);
        }
        .author-avatar {
            width: 28px; height: 28px;
            background: linear-gradient(135deg, var(--primary), var(--accent-purple));
            border-radius: 50%; display: flex; align-items: center;
            justify-content: center; font-size: 0.65rem; font-weight: 700; color: white;
        }
        .post-date { font-size: 0.75rem; color: var(--text-muted); font-family: 'Space Mono', monospace; }

        .vote-row { display: flex; align-items: center; gap: 8px; }
        .vote-btn {
            background: var(--border-light); border: 1px solid var(--border);
            color: var(--text-secondary); padding: 5px 14px;
            border-radius: 20px; font-size: 0.8rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s; font-family: 'DM Sans', sans-serif;
        }
        .vote-btn:hover { border-color: var(--primary); color: var(--primary); background: var(--primary-bg); }
        .vote-count { font-size: 0.82rem; color: var(--text-muted); font-family: 'Space Mono', monospace; }

        /* answers section */
        .answers-section-title {
            font-size: 0.95rem; font-weight: 700; margin-bottom: 14px;
            display: flex; align-items: center; gap: 8px;
        }
        .answers-section-title span {
            background: var(--primary-bg); color: var(--primary);
            padding: 2px 10px; border-radius: 20px;
            font-size: 0.75rem; font-family: 'Space Mono', monospace;
        }

        .answer-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 22px 26px;
            box-shadow: var(--shadow-sm); margin-bottom: 12px;
            animation: slideUp 0.4s ease both;
        }
        .answer-content { font-size: 0.88rem; color: var(--text); line-height: 1.7; margin-bottom: 14px; }
        .answer-footer { display: flex; align-items: center; justify-content: space-between; }
        .answer-meta { display: flex; align-items: center; gap: 10px; }

        /* post answer form */
        .post-answer-box {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 28px;
            box-shadow: var(--shadow-sm); margin-top: 24px;
            animation: slideUp 0.5s ease both;
        }
        .post-answer-title { font-size: 1rem; font-weight: 700; margin-bottom: 16px; }

        .form-textarea {
            width: 100%; padding: 12px 14px; min-height: 120px;
            border: 1px solid var(--border); border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.88rem;
            color: var(--text); outline: none; resize: vertical; line-height: 1.6;
            transition: border-color 0.2s;
        }
        .form-textarea:focus { border-color: var(--primary); }

        .error-msg { color: #dc2626; font-size: 0.78rem; margin-top: 4px; display: block; }

        .btn-submit {
            background: var(--primary); color: white; border: none;
            padding: 10px 26px; border-radius: var(--radius-sm); margin-top: 12px;
            font-family: 'DM Sans', sans-serif; font-size: 0.88rem; font-weight: 600;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-submit:hover { background: #1d4ed8; }

        .alert { padding: 12px 18px; border-radius: var(--radius-sm); font-size: 0.85rem; margin-bottom: 16px; }
        .alert-danger { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
        .alert-success { background: #f0fdf4; color: #16a34a; border: 1px solid #bbf7d0; }

        .empty-answers {
            text-align: center; padding: 30px;
            background: var(--border-light); border-radius: var(--radius-sm);
            color: var(--text-muted); font-size: 0.88rem; margin-bottom: 12px;
        }

        @keyframes slideDown { from { opacity: 0; transform: translateY(-12px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(14px); } to { opacity: 1; transform: translateY(0); } }
        @media (max-width: 768px) {
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
        }
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
                        <asp:Label ID="lblAvatarInitial" runat="server" Text="S" />
                    </div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="nav">
            <a href="StudentDashboard.aspx"><span>📊</span> Dashboard</a>
            <a href="BrowseCourses.aspx"><span>📚</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span>🎓</span> My Courses</a>
            <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
            <a href="Forums.aspx" class="active"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
            <a href="StudentProfile.aspx"><span>👤</span> Profile</a>
        </div>

        <div class="container">

            <div class="breadcrumb">
                <a href="Forums.aspx">Forums</a>
                <span>›</span>
                <a href="#" id="linkBack" runat="server">Questions</a>
                <span>›</span>
                <span>View Answer</span>
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger" Visible="false" />

            <!-- the question itself -->
            <asp:Panel ID="pnlQuestion" runat="server">
                <div class="question-box">
                    <div class="question-box-label">Question</div>
                    <div class="question-box-title"><asp:Label ID="lblTitle" runat="server" /></div>
                    <div class="question-box-content"><asp:Label ID="lblContent" runat="server" /></div>
                    <div class="question-box-footer">
                        <div class="question-box-meta">
                            <div class="author-chip">
                                <div class="author-avatar"><asp:Label ID="lblAuthorInitial" runat="server" /></div>
                                <asp:Label ID="lblAuthorName" runat="server" />
                            </div>
                            <span class="post-date"><asp:Label ID="lblPostDate" runat="server" /></span>
                        </div>
                        <div class="vote-row">
                            <asp:Button ID="btnUpvote" runat="server" Text="👍 Upvote" CssClass="vote-btn" OnClick="btnUpvote_Click" />
                            <span class="vote-count"><asp:Label ID="lblUpvotes" runat="server" Text="0" /></span>
                            <asp:Button ID="btnDownvote" runat="server" Text="👎 Downvote" CssClass="vote-btn" OnClick="btnDownvote_Click" />
                            <span class="vote-count"><asp:Label ID="lblDownvotes" runat="server" Text="0" /></span>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- answers list -->
            <div class="answers-section-title">
                Answers <span><asp:Label ID="lblAnswerCount" runat="server" Text="0" /></span>
            </div>

            <asp:Panel ID="pnlNoAnswers" runat="server" Visible="false">
                <div class="empty-answers">No answers yet. Be the first to answer!</div>
            </asp:Panel>

            <asp:Repeater ID="rptAnswers" runat="server">
                <ItemTemplate>
                    <div class="answer-card">
                        <div class="answer-content"><%# Eval("content") %></div>
                        <div class="answer-footer">
                            <div class="answer-meta">
                                <div class="author-chip">
                                    <div class="author-avatar"><%# Eval("fname").ToString().Substring(0,1).ToUpper() %></div>
                                    <%# Eval("fname") + " " + Eval("lname") %>
                                </div>
                                <span class="post-date"><%# Convert.ToDateTime(Eval("creationtime")).ToString("MMM dd, yyyy") %></span>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <!-- post an answer form -->
            <div class="post-answer-box">
                <div class="post-answer-title">Post Your Answer</div>

                <asp:Label ID="lblAnswerMsg" runat="server" Visible="false" />

                <asp:TextBox ID="txtAnswer" runat="server" CssClass="form-textarea" TextMode="MultiLine"
                    placeholder="Write your answer here..." />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAnswer"
                    ErrorMessage="Please write something before submitting." CssClass="error-msg" Display="Dynamic" />

                <asp:Button ID="btnPostAnswer" runat="server" Text="Post Answer" CssClass="btn-submit" OnClick="btnPostAnswer_Click" />
            </div>

        </div>
    </form>
</body>
</html>
