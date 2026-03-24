<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AttemptExam.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.AttemptExam" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Attempt Exam - LearnSphere</title>

    <!-- SAME STYLE AS LESSON VIEWER -->
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&display=swap" rel="stylesheet">

    <style>
        body { font-family: 'DM Sans'; background:#eef3f9; margin:0; }

        .header {
            background:#fff; padding:15px 30px;
            display:flex; justify-content:space-between;
            border-bottom:1px solid #ddd;
        }

        .content {
            max-width:900px;
            margin:40px auto;
        }

        .card {
            background:#fff;
            padding:30px;
            border-radius:12px;
            box-shadow:0 4px 12px rgba(0,0,0,0.06);
        }

        .question-title {
            font-size:18px;
            font-weight:600;
            margin-bottom:20px;
        }

        .option {
            margin:10px 0;
        }

        .nav-buttons {
            margin-top:25px;
            display:flex;
            justify-content:space-between;
        }

        .btn {
            padding:10px 20px;
            border:none;
            border-radius:8px;
            cursor:pointer;
            font-weight:600;
        }

        .btn-primary { background:#2563eb; color:white; }
        .btn-secondary { background:#e2e8f0; }
        .btn-success { background:#10b981; color:white; }

        .review-box {
            margin-top:20px;
            padding:15px;
            border:1px solid #ddd;
            border-radius:10px;
        }

        .correct { color:green; font-weight:600; }
        .wrong { color:red; font-weight:600; }

    </style>
</head>

<body>
<form runat="server">

    <div class="header">
        <div><b>Attempt Exam</b></div>
        <asp:Button ID="btnBack" runat="server" Text="← Back" CssClass="btn btn-secondary" OnClick="btnBack_Click"/>
    </div>

    <div class="content">

        <!-- QUESTION PANEL -->
        <asp:Panel ID="pnlExam" runat="server">
            <div class="card">

                <div>
                    Question <asp:Label ID="lblQNo" runat="server"/> /
                    <asp:Label ID="lblTotal" runat="server"/>
                </div>

                <div class="question-title">
                    <asp:Label ID="lblQuestion" runat="server"/>
                </div>

                <asp:RadioButtonList ID="rblOptions" runat="server">
                </asp:RadioButtonList>

                <div class="nav-buttons">
                    <asp:Button ID="btnPrev" runat="server" Text="← Previous"
                        CssClass="btn btn-secondary" OnClick="btnPrev_Click"/>

                    <asp:Button ID="btnNext" runat="server" Text="Next →"
                        CssClass="btn btn-primary" OnClick="btnNext_Click"/>

                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Exam"
                        CssClass="btn btn-success" OnClick="btnSubmit_Click"/>
                </div>

            </div>
        </asp:Panel>

        <!-- RESULT PANEL -->
        <asp:Panel ID="pnlResult" runat="server" Visible="false">
            <div class="card">
                <h2>Exam Result</h2>

                <p>
                    Your Score:
                    <b><asp:Label ID="lblScore" runat="server"/></b>
                </p>

                <asp:Button ID="btnReview" runat="server"
                    Text="Review Answers"
                    CssClass="btn btn-primary"
                    OnClick="btnReview_Click"/>
            </div>
        </asp:Panel>

        <!-- REVIEW PANEL -->
        <asp:Panel ID="pnlReview" runat="server" Visible="false">
            <div class="card">
                <h2>Review Answers</h2>

                <asp:Repeater ID="rptReview" runat="server">
                    <ItemTemplate>
                        <div class="review-box">
                            <b>Q:</b> <%# Eval("questiontext") %><br/>

                            Your Answer:
                            <span class='<%# Eval("IsCorrect").ToString()=="True" ? "correct" : "wrong" %>'>
                                <%# Eval("UserAnswer") %>
                            </span><br/>

                            Correct Answer:
                            <span class="correct"><%# Eval("CorrectAnswer") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <br />
                <asp:Button ID="btnBackLesson" runat="server"
                    Text="← Back to Lesson"
                    CssClass="btn btn-secondary"
                    OnClick="btnBackLesson_Click"/>
            </div>
        </asp:Panel>

    </div>

</form>
</body>
</html>