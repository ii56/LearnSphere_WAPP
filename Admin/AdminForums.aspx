<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminForums.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminForums1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forums</title>
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
                <a href="AdminForums.aspx" class="active">Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
                <a href="AdminSyslog.aspx" >Syslog</a>
                <a href="AdminMessage.aspx">
                    <span>✉️</span> Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="nav-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                </a>
                <a href="../Chatbot/AdminChatbotKnowledge.aspx" >Chatbot</a>
            </div>

            <div class="container">

                <!-- ════════════════════════════════════════════
                     PANEL 1 — FORUM LIST
                     ════════════════════════════════════════════ -->
                <asp:Panel ID="pnlForumsList" runat="server">
                    <div class="welcome-banner">
                        <h2 class="welcome-label">Admin Portal</h2>
                        <h2 class="welcome-name">Forums</h2>
                        <h3 class="welcome-sub">View forums, delete forums, and make comments in forums.</h3>
                    </div>

                    <div class="filter-bar">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-input" placeholder="Search course name..." />
                        <asp:DropDownList ID="ddlForumStatus" runat="server" CssClass="filter-input">
                            <asp:ListItem Value="">All Forums</asp:ListItem>
                            <asp:ListItem Value="1">Has Forum</asp:ListItem>
                            <asp:ListItem Value="0">No Forum</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn-primary" OnClick="btnFilter_Click" />
                        <asp:Button ID="btnReset"  runat="server" Text="Reset"        CssClass="btn-secondary" OnClick="btnReset_Click" />
                    </div>

                    <div class="section">
                        <div class="section-header">
                            <div class="section-title">
                                <span class="title-dot dot-green"></span> Your Courses
                            </div>
                        </div>
                        <asp:GridView ID="gvCourses" runat="server"
                            AutoGenerateColumns="False" Width="100%"
                            BorderStyle="None" GridLines="None"
                            OnRowCommand="gvCourses_RowCommand"
                            EmptyDataText="No courses available."
                            DataKeyNames="courseid">
                            <Columns>
                                <asp:BoundField DataField="coursename" HeaderText="Course Name" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <div class="btn-actions">
                                            <asp:Label ID="lblNoForum" runat="server" 
                                                Text="No forum yet" 
                                                CssClass="text-muted"
                                                Visible='<%# !Convert.ToBoolean(Eval("HasForum")) %>' />
                                            <asp:Button ID="btnView" runat="server" Text="View Forum"
                                                CommandName="ViewForum" CommandArgument='<%# Eval("courseid") %>'
                                                CssClass="btn-view"
                                                Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>' />
                                            <asp:Button ID="btnDelete" runat="server" Text="Delete Forum"
                                                CommandName="DeleteForum" CommandArgument='<%# Eval("courseid") %>'
                                                CssClass="btn-danger"
                                                Visible='<%# Convert.ToBoolean(Eval("HasForum")) %>'
                                                OnClientClick="return confirm('Delete this forum and all its posts?');" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Label ID="lblListMessage" runat="server" CssClass="alert" Visible="false" />
                </asp:Panel>

                <!-- ════════════════════════════════════════════
                     PANEL 3 — VIEW FORUM (questions list)
                     ════════════════════════════════════════════ -->
                <asp:Panel ID="pnlViewForum" runat="server" Visible="false">
                    <div class="welcome-banner">
                        <h2 class="welcome-label">Forum</h2>
                        <asp:Label ID="lblForumTitle" runat="server" CssClass="welcome-name" Text="Forums" />
                        <br />
                        <asp:Label ID="lblDescription" runat="server" CssClass="welcome-sub" 
                            Text="View forums, delete forums, and make comments in forums." />
                    </div>

                    <asp:Button ID="btnBackToList" runat="server" Text="← Back to Forums"
                        CssClass="btn-back" OnClick="btnBackToList_Click" CausesValidation="false" />

                    <div class="forum-info-card">
                        <div class="tags-row">
                            <span class="tag-label">Allowed Tags:</span>
                            <asp:Label ID="lblTags" runat="server" />
                        </div>
                    </div>

                    <div class="forum-actions-row">
                        <asp:Button ID="btnAskQuestion" runat="server" Text="+ Ask a Question"
                            CssClass="btn-primary" OnClick="btnAskQuestion_Click" CausesValidation="false" />
                    </div>

                    <!-- Inline Ask Question form -->
                    <asp:Panel ID="pnlAskQuestion" runat="server" Visible="false">
                        <div class="inline-form">
                            <div class="inline-form-title">✏️ Post a New Question</div>

                            <asp:ValidationSummary ID="vsQuestion" runat="server"
                                CssClass="validation-summary" HeaderText="Please fix:"
                                ValidationGroup="questionForm" />

                            <div class="form-group">
                                <label class="form-label">Title *</label>
                                <asp:TextBox ID="txtQuestionTitle" runat="server" CssClass="form-input" MaxLength="150" />
                                <asp:RequiredFieldValidator ControlToValidate="txtQuestionTitle" runat="server"
                                    ErrorMessage="Title is required." CssClass="validation-error"
                                    ValidationGroup="questionForm" Display="Dynamic" />
                                <asp:RegularExpressionValidator ControlToValidate="txtQuestionTitle" runat="server"
                                    ValidationExpression="^[a-zA-Z0-9\s\-\?\!\.,]{3,150}$"
                                    ErrorMessage="Invalid title format (3–150 chars, letters/numbers/punctuation)."
                                    CssClass="validation-error" ValidationGroup="questionForm" Display="Dynamic" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Content *</label>
                                <asp:TextBox ID="txtQuestionContent" runat="server" CssClass="form-input"
                                    TextMode="MultiLine" Rows="5" MaxLength="2000" />
                                <asp:RequiredFieldValidator ControlToValidate="txtQuestionContent" runat="server"
                                    ErrorMessage="Content is required." CssClass="validation-error"
                                    ValidationGroup="questionForm" Display="Dynamic" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Tags (comma-separated)</label>
                                <asp:TextBox ID="txtQuestionTags" runat="server" CssClass="form-input" MaxLength="200"
                                    placeholder="e.g. python, loops, debugging" />
                                <asp:RegularExpressionValidator ControlToValidate="txtQuestionTags" runat="server"
                                    ValidationExpression="^[a-zA-Z0-9,\s\-]*$"
                                    ErrorMessage="Invalid tags format." CssClass="validation-error"
                                    ValidationGroup="questionForm" Display="Dynamic" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Upload Document (PDF, DOCX, ZIP)</label>
                                <asp:FileUpload ID="fileUploadQFile" runat="server" CssClass="form-input" />
                                <asp:RegularExpressionValidator ControlToValidate="fileUploadQFile" runat="server"
                                    ValidationExpression="^.*\.(pdf|docx|zip)$"
                                    ErrorMessage="Only PDF, DOCX or ZIP files allowed."
                                    CssClass="validation-error" ValidationGroup="questionForm" Display="Dynamic" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Upload Image (JPG, PNG)</label>
                                <asp:FileUpload ID="fileUploadQImage" runat="server" CssClass="form-input" />
                                <asp:RegularExpressionValidator ControlToValidate="fileUploadQImage" runat="server"
                                    ValidationExpression="^.*\.(jpg|jpeg|png)$"
                                    ErrorMessage="Only JPG or PNG images allowed."
                                    CssClass="validation-error" ValidationGroup="questionForm" Display="Dynamic" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Video URL</label>
                                <asp:TextBox ID="txtQuestionVideoUrl" runat="server" CssClass="form-input" MaxLength="300"
                                    placeholder="https://..." />
                                <asp:RegularExpressionValidator ControlToValidate="txtQuestionVideoUrl" runat="server"
                                    ValidationExpression="^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-\.~:\/?#\[\]@!\$&amp;'\(\)\*\+,;=]*)?$"
                                    ErrorMessage="Invalid video URL." CssClass="validation-error"
                                    ValidationGroup="questionForm" Display="Dynamic" />
                            </div>
                            <div class="form-btn-row">
                                <asp:Button ID="btnSubmitQuestion" runat="server" Text="Post Question"
                                    CssClass="btn-primary" ValidationGroup="questionForm" OnClick="btnSubmitQuestion_Click" />
                                <asp:Button ID="btnCancelQuestion" runat="server" Text="Cancel"
                                    CssClass="btn-secondary" OnClick="btnCancelQuestion_Click" CausesValidation="false" />
                            </div>
                            <asp:Label ID="lblQuestionFormMsg" runat="server" CssClass="alert" Visible="false" />
                        </div>
                    </asp:Panel>

                    <!-- Questions list -->
                    <asp:Repeater ID="rptQuestions" runat="server" OnItemCommand="rptQuestions_ItemCommand">
                        <ItemTemplate>
                            <div class="question-card">
                                <div class="card-meta">
                                    <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' class="card-user-avatar" alt="" />
                                    <div>
                                        <div class="card-user-name"><%# Server.HtmlEncode(Eval("uname").ToString()) %></div>
                                        <div class="card-date"><%# Convert.ToDateTime(Eval("creationtime")).ToString("dd MMM yyyy") %></div>
                                    </div>
                                </div>
                                <div class="card-question-title"><%# Server.HtmlEncode(Eval("title").ToString()) %></div>
                                <div class="card-preview">
                                    <%# Server.HtmlEncode(Eval("content").ToString().Length > 180
                                        ? Eval("content").ToString().Substring(0,180) + "…"
                                        : Eval("content").ToString()) %>
                                </div>
                                <div class="tags-row" style="margin-bottom:12px;">
                                    <%# FormatTags(Eval("tags")) %>
                                </div>
                                <div class="card-footer-row">
                                    <div class="vote-group">
                                        <asp:LinkButton runat="server" CommandName="Like"
                                            CommandArgument='<%# Eval("postid") %>'
                                            CssClass="vote-btn like-btn" CausesValidation="false">
                                            👍 <%# Eval("upvotes") %>
                                        </asp:LinkButton>
                                        <asp:LinkButton runat="server" CommandName="Dislike"
                                            CommandArgument='<%# Eval("postid") %>'
                                            CssClass="vote-btn dislike-btn" CausesValidation="false">
                                            👎 <%# Eval("downvotes") %>
                                        </asp:LinkButton>
                                    </div>
                                    <asp:LinkButton runat="server" CommandName="ViewDetail"
                                        CommandArgument='<%# Eval("postid") %>'
                                        CssClass="btn-comment" CausesValidation="false">
                                        View &amp; Answer →
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Label ID="lblNoQuestions" runat="server" CssClass="empty-message" Visible="false"
                        Text="No questions yet. Be the first to ask!" />
                    <asp:Label ID="lblViewForumMsg" runat="server" CssClass="alert alert-error" Visible="false" />
                </asp:Panel>

                <!-- ════════════════════════════════════════════
                     PANEL 4 — FORUM DETAIL (question + answers)
                     ════════════════════════════════════════════ -->
                <asp:Panel ID="pnlForumDetail" runat="server" Visible="false">
                    <div class="welcome-banner">
                        <h2 class="welcome-label">Forum</h2>
                        <asp:Label ID="Label1" runat="server" CssClass="welcome-name" Text="Answer" />
                        <br />
                        <asp:Label ID="Label2" runat="server" CssClass="welcome-sub" 
                            Text="View forums, delete forums, and make comments in forums." />
                    </div>

                    <asp:Button ID="btnBackToForum" runat="server" Text="← Back to Forum"
                        CssClass="btn-back" OnClick="btnBackToForum_Click" CausesValidation="false" />

                    <!-- Question detail card -->
                    <div class="detail-card">
                        <div class="card-meta">
                            <asp:Image ID="imgQuestionUser" runat="server" CssClass="card-user-avatar" />
                            <div>
                                <div class="card-user-name"><asp:Label ID="lblQuestionUser" runat="server" /></div>
                                <div class="card-date"><asp:Label ID="lblQuestionDate" runat="server" /></div>
                            </div>
                        </div>
                        <div class="detail-title"><asp:Label ID="lblQuestionTitle" runat="server" /></div>
                        <div class="detail-content"><asp:Label ID="lblQuestionContent" runat="server" /></div>
                        <div class="tags-row" style="margin-top:14px;">
                            <span class="tag-label">Tags:</span>
                            <asp:Literal ID="litTags" runat="server" />
                        </div>
                        <div class="card-footer-row" style="margin-top:16px;">
                            <div class="vote-group">
                                <asp:LinkButton ID="btnLikeQuestion" runat="server"
                                    CssClass="vote-btn like-btn" OnClick="btnLikeQuestion_Click" CausesValidation="false">
                                    👍 <span id="likeCount" runat="server"></span>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDislikeQuestion" runat="server"
                                    CssClass="vote-btn dislike-btn" OnClick="btnDislikeQuestion_Click" CausesValidation="false">
                                    👎 <span id="dislikeCount" runat="server"></span>
                                </asp:LinkButton>
                            </div>
                            <asp:Button ID="btnAnswer" runat="server" Text="+ Add Answer"
                                CssClass="btn-primary" OnClick="btnAnswer_Click" CausesValidation="false" />
                        </div>
                    </div>

                    <!-- Inline Add Answer form -->
                    <asp:Panel ID="pnlAddAnswer" runat="server" Visible="false">
                        <div class="inline-form">
                            <div class="inline-form-title">💬 Write Your Answer</div>

                            <!-- Question preview (matches answer.aspx) -->
                            <div class="question-preview-box" style="margin-bottom:16px;">
                                <asp:Label ID="lblAnswerPreviewTitle"   runat="server" CssClass="question-title-preview" />
                                <asp:Label ID="lblAnswerPreviewContent" runat="server" CssClass="question-content-preview" />
                            </div>

                            <asp:ValidationSummary ID="vsAnswer" runat="server"
                                CssClass="validation-summary" HeaderText="Please fix:"
                                ValidationGroup="answerForm" />

                            <div class="form-group">
                                <label class="form-label">Your Answer *</label>
                                <asp:TextBox ID="txtAnswerContent" runat="server" CssClass="form-input"
                                    TextMode="MultiLine" Rows="6" MaxLength="2000" />
                                <asp:RequiredFieldValidator ControlToValidate="txtAnswerContent" runat="server"
                                    ErrorMessage="Answer cannot be empty." CssClass="validation-error"
                                    ValidationGroup="answerForm" Display="Dynamic" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Upload Document (PDF, DOCX, ZIP)</label>
                                <asp:FileUpload ID="fileUploadAFile" runat="server" CssClass="form-input" />
                                <asp:RegularExpressionValidator ControlToValidate="fileUploadAFile" runat="server"
                                    ValidationExpression="^.*\.(pdf|docx|zip)$"
                                    ErrorMessage="Only PDF, DOCX or ZIP files allowed."
                                    CssClass="validation-error" ValidationGroup="answerForm" Display="Dynamic" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Upload Image (JPG, PNG)</label>
                                <asp:FileUpload ID="fileUploadAImage" runat="server" CssClass="form-input" />
                                <asp:RegularExpressionValidator ControlToValidate="fileUploadAImage" runat="server"
                                    ValidationExpression="^.*\.(jpg|jpeg|png)$"
                                    ErrorMessage="Only JPG or PNG images allowed."
                                    CssClass="validation-error" ValidationGroup="answerForm" Display="Dynamic" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Video URL</label>
                                <asp:TextBox ID="txtAnswerVideoUrl" runat="server" CssClass="form-input" MaxLength="500"
                                    placeholder="https://..." />
                                <asp:RegularExpressionValidator ControlToValidate="txtAnswerVideoUrl" runat="server"
                                    ValidationExpression="^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-\.~:\/?#\[\]@!\$&amp;'\(\)\*\+,;=]*)?$"
                                    ErrorMessage="Invalid video URL." CssClass="validation-error"
                                    ValidationGroup="answerForm" Display="Dynamic" />
                            </div>
                            <div class="form-btn-row">
                                <asp:Button ID="btnSubmitAnswer" runat="server" Text="Submit Answer"
                                    CssClass="btn-primary" ValidationGroup="answerForm" OnClick="btnSubmitAnswer_Click" />
                                <asp:Button ID="btnCancelAnswer" runat="server" Text="Cancel"
                                    CssClass="btn-secondary" OnClick="btnCancelAnswer_Click" CausesValidation="false" />
                            </div>
                            <asp:Label ID="lblAnswerFormMsg" runat="server" CssClass="alert" Visible="false" />
                        </div>
                    </asp:Panel>

                    <!-- Answers list -->
                    <div class="answers-heading">
                        <span class="title-dot dot-blue"></span> Answers
                    </div>

                    <asp:Repeater ID="rptAnswers" runat="server" OnItemCommand="rptAnswers_ItemCommand">
                        <ItemTemplate>
                            <div class="answer-card">
                                <div class="card-meta">
                                    <img src='<%# GetProfileImage(Eval("ProfileImage")) %>' class="card-user-avatar" alt="" />
                                    <div>
                                        <div class="card-user-name"><%# Server.HtmlEncode(Eval("uname").ToString()) %></div>
                                        <div class="card-date"><%# Convert.ToDateTime(Eval("creationtime")).ToString("dd MMM yyyy") %></div>
                                    </div>
                                </div>
                                <div class="card-preview" style="margin-bottom:14px;">
                                    <%# Server.HtmlEncode(Eval("content").ToString()) %>
                                </div>
                                <div class="card-footer-row">
                                    <div class="vote-group">
                                        <asp:LinkButton runat="server" CommandName="LikeAnswer"
                                            CommandArgument='<%# Eval("postid") %>'
                                            CssClass="vote-btn like-btn" CausesValidation="false">
                                            👍 <%# Eval("upvotes") %>
                                        </asp:LinkButton>
                                        <asp:LinkButton runat="server" CommandName="DislikeAnswer"
                                            CommandArgument='<%# Eval("postid") %>'
                                            CssClass="vote-btn dislike-btn" CausesValidation="false">
                                            👎 <%# Eval("downvotes") %>
                                        </asp:LinkButton>
                                    </div>
                                    <asp:Button ID="btnDeleteAnswer" runat="server"
                                        Text="Delete" CssClass="btn-delete"
                                        CommandName="DeletePost"
                                        CommandArgument='<%# Eval("postid") %>'
                                        OnClientClick="return confirm('Delete this answer?');" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Label ID="lblNoAnswers" runat="server" CssClass="empty-message"
                        Text="No answers yet. Be the first to answer!" Visible="false" />
                    <asp:Label ID="lblDetailMessage" runat="server" CssClass="alert alert-error" Visible="false" />
                </asp:Panel>

            </div>
    </form>
</body>
</html>
