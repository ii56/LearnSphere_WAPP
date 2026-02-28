<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Preview.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.Preview" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Preview</title>
    <link href="Preview.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
<div class="preview-layout">

    <div class="preview-sidebar">
        <asp:Repeater ID="rptModules" runat="server">
            <ItemTemplate>
                <div class="module-block">
                    <div class="module-title">
                        <%# Eval("modulename") %>
                    </div>

                    <asp:Repeater ID="rptLessons"
                        runat="server"
                        DataSource='<%# Eval("Lessons") %>'>
                        <ItemTemplate>
                            <div class="lesson-item">
                                <a href='Preview.aspx?courseid=<%# Request.QueryString["courseid"] %>&lessonid=<%# Eval("lessonid") %>'>
                                    <%# Eval("lessontitle") %>
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <div class="preview-content">

        <div class="preview-header">
            <asp:Label ID="lblCourseName" runat="server" CssClass="course-title" />

            <div class="progress-container">
                <asp:Literal ID="litProgressBar" runat="server" />
            </div>

            <asp:Button ID="btnBack" runat="server" Text="← Back to View Courses" CssClass="back-btn" OnClick="btnBack_Click" />
        </div>

        <div class="preview-body">
            <asp:PlaceHolder ID="phOverview" runat="server" />
            <asp:PlaceHolder ID="phLesson" runat="server" />
            <asp:PlaceHolder ID="phCompletion" runat="server" />
        </div>

    </div>

</div>

</form>
</body>
</html>
