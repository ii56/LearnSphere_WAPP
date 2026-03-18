<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForumDetail.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.ForumDetail" ValidateRequest="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forum Detail</title>
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
                <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                <a href="../Chatbot/Chatbot.aspx" class="nav-item">Chatbot</a>
                <a href="Forums.aspx" class="nav-item active">Forums</a>
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
                        <div class="profile-name">
                            <%= Server.HtmlEncode(Session["uname"] != null ? Session["uname"].ToString() : "") %>
                        </div>

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
                        <span class="message-badge">
                            <%= Session["unreadCount"] %>
                        </span>
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

            <!-- BACK BUTTON (SAFE) -->
            <div class="back-container">
                <asp:HyperLink ID="lnkBack"
                               runat="server"
                               CssClass="btn-back"
                               Text="← Back to Forum" />
            </div>

            <!-- QUESTION -->
            <div class="modern-card question-detail">

                <div class="card-header">
                    <asp:Image ID="imgQuestionUser"
                               runat="server"
                               CssClass="user-avatar" />

                    <div class="user-info">
                        <asp:Label ID="lblQuestionUser"
                                   runat="server"
                                   CssClass="username" />

                        <asp:Label ID="lblQuestionDate"
                                   runat="server"
                                   CssClass="post-time" />
                    </div>
                </div>

                <div class="card-title">
                    <asp:Label ID="lblQuestionTitle"
                               runat="server" />
                </div>

                <div class="card-preview">
                    <asp:Label ID="lblQuestionContent"
                               runat="server" />
                </div>

                <div class="card-tags">
                    <asp:Literal ID="litTags"
                                 runat="server" />
                </div>

                <div class="card-footer">

                    <div class="stats">
<div class="vote-section">

    <asp:LinkButton runat="server"
        ID="btnLikeQuestion"
        CssClass="vote-btn like-btn"
        OnClick="btnLikeQuestion_Click"
        CausesValidation="false">
        👍 <span id="likeCount" runat="server"></span>
    </asp:LinkButton>

    <asp:LinkButton runat="server"
        ID="btnDislikeQuestion"
        CssClass="vote-btn dislike-btn"
        OnClick="btnDislikeQuestion_Click"
        CausesValidation="false">
        👎 <span id="dislikeCount" runat="server"></span>
    </asp:LinkButton>

</div>
                    </div>

                    <asp:Button ID="btnAnswer"
                                runat="server"
                                Text="Add Answer"
                                CssClass="btn-comment"
                                OnClick="btnAnswer_Click" />
                </div>

            </div>

            <!-- ANSWERS -->
            <h3 style="margin:30px 0 15px 0;">Answers</h3>

<asp:Repeater ID="rptAnswers" runat="server" OnItemCommand="rptAnswers_ItemCommand">

                <ItemTemplate>

                    <div class="modern-card answer-card">

                        <div class="card-header">

                            <img src='<%# GetProfileImage(Eval("ProfileImage")) %>'
                                 class="user-avatar" />

                            <div class="user-info">
                                <div class="username">
                                    <%# Server.HtmlEncode(Eval("uname").ToString()) %>
                                </div>

                                <div class="post-time">
                                    <%# Convert.ToDateTime(Eval("creationtime")).ToString("dd MMM yyyy") %>
                                </div>
                            </div>

                        </div>

                        <div class="card-preview">
                            <%# Server.HtmlEncode(Eval("content").ToString()) %>
                        </div>

                        <div class="card-footer">

                            <div class="stats">
<asp:LinkButton runat="server"
    CommandName="LikeAnswer"
    CommandArgument='<%# Eval("postid") %>'
    CssClass="vote-btn like-btn"
    CausesValidation="false">
    👍 <%# Eval("upvotes") %>
</asp:LinkButton>

<asp:LinkButton runat="server"
    CommandName="DislikeAnswer"
    CommandArgument='<%# Eval("postid") %>'
    CssClass="vote-btn dislike-btn"
    CausesValidation="false">
    👎 <%# Eval("downvotes") %>
</asp:LinkButton>
                            </div>

                            <div class="card-actions">

                                <!-- SAFE DELETE BUTTON -->
                                <asp:Button ID="btnDelete"
                                            runat="server"
                                            Text="Delete"
                                            CssClass="btn-delete"
                                            CommandName="DeletePost"
                                            CommandArgument='<%# Eval("postid") %>'
                                            OnClientClick="return confirm('Are you sure you want to delete this answer?');"
                                            Visible='<%# IsOwner(Eval("userid")) %>' />

                            </div>

                        </div>

                    </div>

                </ItemTemplate>

            </asp:Repeater>

            <!-- EMPTY STATE -->
            <asp:Label ID="lblNoAnswers"
                       runat="server"
                       Text="No answers yet."
                       Visible="false"
                       CssClass="empty-message" />

            <!-- ERROR MESSAGE -->
            <asp:Label ID="lblMessage"
                       runat="server"
                       ForeColor="Red" />

        </div>
    </div>

</form>
</body>
</html>