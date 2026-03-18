<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddModules.aspx.cs" Inherits="LearnSphere_WAPP.Lecturer.AddModules" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Modules</title>
    <link href="CreateCourse.css" rel="stylesheet" />
</head>

<body>

    <form id="form1" runat="server">

        <div class="layout">

            <!-- SIDEBAR -->
            <div class="sidebar">

                <div>
                    <div class="sidebar-title">
                        LearnSphere
                    </div>

                    <a href="LecturerDashboard.aspx" class="nav-item">
                        Dashboard
                    </a>

                    <a href="CreateCourse.aspx" class="nav-item active">
                        Create Course
                    </a>

                    <a href="ViewCourses.aspx" class="nav-item">
                        View Courses
                    </a>

                    <a href="EditProfile.aspx" class="nav-item">
                        Edit Profile
                    </a>

                    <a href="Forums.aspx" class="nav-item">
                        Forums
                    </a>
                </div>


                <div class="sidebar-profile">

                    <div class="profile-box <%= (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") ? "verified" : "not-verified" %>">

                        <div class="profile-img-wrapper">

                            <img id="imgSidebarProfile" runat="server" class="profile-img" />

                            <% if (Session["usertype"] != null && Session["usertype"].ToString() == "Lecturer") { %>

                                <div class="verification-badge">
                                    ✔
                                </div>

                            <% } %>

                        </div>


                        <div class="profile-info">

                            <div class="profile-name">
                                <%= Session["uname"] %>
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



                    <asp:Button
                        ID="btnLogout"
                        runat="server"
                        Text="Logout"
                        CssClass="logout-btn"
                        OnClick="btnLogout_Click" />

                </div>

            </div>



            <!-- MAIN CONTENT -->
            <div class="main-content">

                <h2>
                    Add Modules -
                    <asp:Label ID="lblCourseTitle" runat="server"></asp:Label>
                </h2>

                <div class="step-indicator">

                    <div class="step <%= ViewState["Step"] != null && ViewState["Step"].ToString() == "1" ? "active" : "" %>">
                        <div class="circle">1</div>
                        <span>Course Details</span>
                    </div>

                    <div class="step <%= ViewState["Step"] != null && ViewState["Step"].ToString() == "2" ? "active" : "" %>">
                        <div class="circle">2</div>
                        <span>Modules</span>
                    </div>

                    <div class="step <%= ViewState["Step"] != null && ViewState["Step"].ToString() == "3" ? "active" : "" %>">
                        <div class="circle">3</div>
                        <span>Lessons</span>
                    </div>

                    <div class="step <%= ViewState["Step"] != null && ViewState["Step"].ToString() == "4" ? "active" : "" %>">
                        <div class="circle">4</div>
                        <span>Publish</span>
                    </div>

                </div>



                <div class="form-card-modern">

                    <!-- VALIDATION SUMMARY -->
                    <asp:ValidationSummary
                        ID="ValidationSummary1"
                        runat="server"
                        ValidationGroup="moduleForm"
                        CssClass="validation-summary"
                        HeaderText="Please fix the following errors:" />


                    <!-- MODULE NAME -->
                    <label>
                        Module Name *
                    </label>

                    <asp:TextBox
                        ID="txtModuleName"
                        runat="server"
                        CssClass="modern-input"
                        MaxLength="100">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="reqModuleName"
                        runat="server"
                        ControlToValidate="txtModuleName"
                        ErrorMessage="Module name is required."
                        Display="Dynamic"
                        CssClass="validation-error"
                        ValidationGroup="moduleForm" />

<asp:RegularExpressionValidator
    ID="regexModuleName"
    runat="server"
    ControlToValidate="txtModuleName"
    ValidationExpression="^[a-zA-Z0-9 .-]+$"
    ErrorMessage="Only letters, numbers, spaces, dots and dashes allowed."
    CssClass="validation-error"
    ValidationGroup="moduleForm" />


                    <!-- MODULE DESCRIPTION -->
                    <label>
                        Module Description
                    </label>

                    <asp:TextBox
                        ID="txtModuleDesc"
                        runat="server"
                        TextMode="MultiLine"
                        Rows="3"
                        CssClass="modern-input"
                        MaxLength="1000">
                    </asp:TextBox>


                    <!-- ADD MODULE BUTTON -->
                    <asp:Button
                        ID="btnAddModule"
                        runat="server"
                        Text="Add Module"
                        CssClass="btn-modern"
                        ValidationGroup="moduleForm"
                        OnClick="btnAddModule_Click" />


                    <asp:Label
                        ID="lblMessage"
                        runat="server">
                    </asp:Label>

                </div>


                <br />


                <!-- MODULE LIST -->
                <div class="courses-section">

                    <h3>
                        Modules Added
                    </h3>

                    <asp:GridView
                        ID="gvModules"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="modern-table"
                        OnRowCommand="gvModules_RowCommand"
                        DataKeyNames="moduleid">

                        <Columns>

                            <asp:BoundField
                                DataField="modulename"
                                HeaderText="Module Name" />

                            <asp:BoundField
                                DataField="moduledescription"
                                HeaderText="Description" />


                            <asp:TemplateField HeaderText="Actions">

                                <ItemTemplate>

                                    <asp:Button
                                        ID="btnAddLessons"
                                        runat="server"
                                        Text="Add Lessons"
                                        CssClass="btn-modern-small"
                                        CommandName="AddLessons"
                                        CommandArgument='<%# Eval("moduleid") %>' />

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>


                    <br />


                    <asp:Button
                        ID="btnContinue"
                        runat="server"
                        Text="Continue to Lessons"
                        CssClass="btn-modern"
                        OnClick="btnContinue_Click" />


                    <br />


                    <asp:Button
                        ID="btnBackToCourse"
                        runat="server"
                        Text="← Back to Course Details"
                        CssClass="btn-modern-secondary"
                        Width="264px"
                        OnClick="btnBackToCourse_Click" />

                </div>

            </div>

        </div>

    </form>

</body>
</html>