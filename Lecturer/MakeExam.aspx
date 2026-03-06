<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MakeExam.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.MakeExam" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Exam</title>
    <link href="courses.css" rel="stylesheet" />
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
                    <a href="ViewCourses.aspx" class="nav-item active">View Courses</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                    <a href="Forums.aspx" class="nav-item">Forums</a>
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
                            <div class="profile-name"><%= Session["uname"] %></div>
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
                            <span class="message-badge"><%= Session["unreadCount"] %></span>
                        <% } %>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>


    <!-- MAIN CONTENT -->

    <div class="main-content">

        <div class="form-card-modern">

            <h2>Create Exam</h2>

            <p class="sub-text">Create a module exam or course exam for your students.</p>


            <!-- Exam Type -->

            <label>Exam Type</label>

            <asp:DropDownList ID="ddlExamType" runat="server"
                CssClass="modern-input"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlExamType_SelectedIndexChanged">

                <asp:ListItem Text="Select Exam Type" Value=""></asp:ListItem>
                <asp:ListItem Text="Module Exam" Value="module"></asp:ListItem>
                <asp:ListItem Text="Course Exam" Value="course"></asp:ListItem>

            </asp:DropDownList>


            <!-- Target -->

            <label>Select Module / Course</label>

            <asp:DropDownList ID="ddlTarget"
                runat="server"
                CssClass="modern-input">

                <asp:ListItem Text="Select Module or Course" Value=""></asp:ListItem>

            </asp:DropDownList>


            <!-- Exam Title -->

            <label>Exam Title</label>

            <asp:TextBox ID="txtExamTitle"
                runat="server"
                CssClass="modern-input">
            </asp:TextBox>


            <hr />


            <!-- QUESTION INPUT -->

            <h3>Add Question</h3>

            <label>Question</label>

            <asp:TextBox ID="txtQuestion"
                runat="server"
                TextMode="MultiLine"
                CssClass="modern-input">
            </asp:TextBox>


            <label>Option A</label>

            <asp:TextBox ID="txtA"
                runat="server"
                CssClass="modern-input">
            </asp:TextBox>


            <label>Option B</label>

            <asp:TextBox ID="txtB"
                runat="server"
                CssClass="modern-input">
            </asp:TextBox>


            <label>Option C</label>

            <asp:TextBox ID="txtC"
                runat="server"
                CssClass="modern-input">
            </asp:TextBox>


            <label>Option D</label>

            <asp:TextBox ID="txtD"
                runat="server"
                CssClass="modern-input">
            </asp:TextBox>


            <label>Correct Answer</label>

            <asp:DropDownList ID="ddlCorrect"
                runat="server"
                CssClass="modern-input">

                <asp:ListItem Text="A" Value="A"></asp:ListItem>
                <asp:ListItem Text="B" Value="B"></asp:ListItem>
                <asp:ListItem Text="C" Value="C"></asp:ListItem>
                <asp:ListItem Text="D" Value="D"></asp:ListItem>

            </asp:DropDownList>


            <label>Marks</label>

            <asp:TextBox ID="txtMarks"
                runat="server"
                CssClass="modern-input"
                Text="1">
            </asp:TextBox>


            <asp:Button ID="btnAddQuestion"
                runat="server"
                Text="Add Question"
                CssClass="btn-modern"
                OnClick="btnAddQuestion_Click" />

            <asp:Button ID="btnReview"
                runat="server"
                Text="Review Questions"
                CssClass="btn-modern-secondary"
                OnClick="btnReview_Click" />


            <hr />


            <!-- QUESTION LIST -->

        <asp:DropDownList ID="ddlQuestionFilter"
            runat="server"
            CssClass="modern-input"
            AutoPostBack="true"
            OnSelectedIndexChanged="ddlQuestionFilter_SelectedIndexChanged">

            <asp:ListItem Text="All Questions" Value="all"></asp:ListItem>
            <asp:ListItem Text="Module Questions" Value="module"></asp:ListItem>
            <asp:ListItem Text="Course Questions" Value="course"></asp:ListItem>

        </asp:DropDownList>

        <asp:GridView ID="gvQuestions"
            runat="server"
            CssClass="modern-table"
            AutoGenerateColumns="true"
            AutoGenerateSelectButton="true"
            OnSelectedIndexChanged="gvQuestions_SelectedIndexChanged">
        </asp:GridView>


            <hr />


            <!-- ACTION BUTTONS -->

            <asp:RequiredFieldValidator
                ControlToValidate="ddlExamType"
                InitialValue=""
                ErrorMessage="Select exam type"
                ForeColor="Red"
                runat="server" />

            <asp:RequiredFieldValidator
                ControlToValidate="txtExamTitle"
                ErrorMessage="Enter exam title"
                ForeColor="Red"
                runat="server" />

            <asp:Button ID="btnPublish"
                runat="server"
                Text="Publish Exam"
                CssClass="btn-modern"
                OnClick="btnPublish_Click" />

            <asp:Button ID="btnDraft"
                runat="server"
                Text="Save Draft"
                CssClass="btn-modern-secondary"
                OnClick="btnDraft_Click" />

            <asp:Button ID="btnCancel"
                runat="server"
                Text="Cancel"
                CssClass="btn-delete"
                OnClick="btnCancel_Click" />

        </div>

    </div>

</div>

</form>

</body>
</html>