<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Database.aspx.cs" Inherits="LearnSphere_WAPP.Admin.Database" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Database</title>
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
                <a href="Database.aspx" class="active">Database</a>
                <a href="AdminForums.aspx" >Forums</a>
                <a href="AdminEditProfile.aspx" >Edit Profile</a>
                <a href="AdminSyslog.aspx" >Syslog</a>
                <a href="AdminMessage.aspx">
                    Messaging
                    <% if (Session["unreadCount"] != null && (int)Session["unreadCount"] > 0) { %>
                        <span class="nav-badge"><%= Session["unreadCount"] %></span>
                    <% } %>
                </a>
                <a href="../Chatbot/AdminChatbotKnowledge.aspx" >Chatbot</a>
            </div>

            <div class="container">
                <div class="welcome-banner">
                    <h2 class="welcome-label">Admin Portal</h2>
                    <h2 class="welcome-name">Database Management</h2>
                    <h3 class="welcome-sub">Manage data in system database</h3>
                </div>

                <div class="section section-actions">
                    <div class="section-header section-title">
                        <div class="section-title">
                            <span class="section-title-dot dot-purple"></span>
                            <asp:Label id="dbTitle" runat="server">Select Database</asp:Label>
                        </div>
                    </div>
                    <asp:Button ID="btnVerification" runat="server" Text="Verification" OnClick="btnVerification_Click" CssClass="btn-database" />
                    <asp:Button ID="btnUser" runat="server" Text="Users" OnClick="btnUser_Click" CssClass="btn-database" />
                    <asp:Button ID="btnInvoice" runat="server" Text="Invoice" OnClick="btnInvoice_Click" CssClass="btn-database" />
                    <asp:Button ID="btnReceipt" runat="server" Text="Receipt" OnClick="btnReceipt_Click" CssClass="btn-database" />
                    <asp:Button ID="btnModule" runat="server" Text="Module" OnClick="btnModule_Click" CssClass="btn-database" />
                    <asp:Button ID="btnLesson" runat="server" Text="Lesson" OnClick="btnLesson_Click" CssClass="btn-database" />
                    <asp:Button ID="btnMaterial" runat="server" Text="Material" OnClick="btnMaterial_Click" CssClass="btn-database" />
                </div>

                <div class="section">
                    <asp:GridView ID="gvVerification" runat="server" AutoGenerateColumns="False" CssClass="gridview" AllowPaging="True" PageSize="8" 
                        OnPageIndexChanging="gv_PageIndexChanging" OnRowCommand="gvVerification_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="requestid" HeaderText="Request ID" />
                            <asp:BoundField DataField="userid" HeaderText="User ID" />
                            <asp:BoundField DataField="requestedrole" HeaderText="Requested Role" />
                            <asp:BoundField DataField="requesttime" HeaderText="Request Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:BoundField DataField="status" HeaderText="Status" />

                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button 
                                        ID="btnView" 
                                        runat="server" 
                                        Text="View" 
                                        CommandName="ViewVerification"
                                        CommandArgument='<%# Eval("requestid") %>' 
                                        CssClass="btn-view"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <asp:GridView ID="gvUser" runat="server" AutoGenerateColumns="False" CssClass="gridview" AllowPaging="True" PageSize="8"
                        OnRowCommand="gvUser_RowCommand" OnPageIndexChanging="gv_PageIndexChanging">
    
                        <Columns>
                            <asp:BoundField DataField="userid" HeaderText="User ID" />
                            <asp:BoundField DataField="uname" HeaderText="Username" />
                            <asp:BoundField DataField="fname" HeaderText="First Name" />
                            <asp:BoundField DataField="lname" HeaderText="Last Name" />
                            <asp:BoundField DataField="age" HeaderText="Age" />
                            <asp:BoundField DataField="gender" HeaderText="Gender" />
                            <asp:BoundField DataField="usertype" HeaderText="User Type" />
                            <asp:BoundField DataField="status" HeaderText="Status" />

                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button 
                                        ID="btnView" 
                                        runat="server" 
                                        Text="View" 
                                        CommandName="ViewUser"
                                        CommandArgument='<%# Eval("userid") %>' 
                                        CssClass="btn-view"/>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>

                    <asp:GridView ID="gvInvoice" runat="server" AutoGenerateColumns="False" CssClass="gridview" AllowPaging="True" PageSize="8" 
                        OnRowCommand="gvInvoice_RowCommand" OnPageIndexChanging="gv_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="invid" HeaderText="Invoice ID" />
                            <asp:BoundField DataField="userid" HeaderText="User ID" />
                            <asp:BoundField DataField="courseid" HeaderText="Course ID" />
                            <asp:BoundField DataField="amount" HeaderText="Amount" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="overdue" HeaderText="Overdue" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="creationtime" HeaderText="Creation Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnView" runat="server" Text="View" CommandName="ViewInvoice" CommandArgument='<%# Eval("invid") %>' CssClass="btn-view"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <asp:GridView ID="gvReceipt" runat="server" AutoGenerateColumns="False" CssClass="gridview" AllowPaging="True" PageSize="8"
                        OnRowCommand="gvReceipt_RowCommand" OnPageIndexChanging="gv_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="recid" HeaderText="Receipt ID" />
                            <asp:BoundField DataField="invid" HeaderText="Invoice ID" />
                            <asp:BoundField DataField="amount" HeaderText="Amount" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="creationtime" HeaderText="Creation Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnView" runat="server" Text="View" CommandName="ViewReceipt"
                                        CommandArgument='<%# Eval("recid") %>' CssClass="btn-view"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                     <asp:GridView ID="gvModule" runat="server" AutoGenerateColumns="False" CssClass="gridview" AllowPaging="True" PageSize="8"
                        OnRowCommand="gvModule_RowCommand" OnPageIndexChanging="gv_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="moduleid" HeaderText="Module ID" />
                            <asp:BoundField DataField="courseid" HeaderText="Course ID" />
                            <asp:BoundField DataField="modulename" HeaderText="Module Name" />
                            <asp:BoundField DataField="creationtime" HeaderText="Creation Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:BoundField DataField="ordernumber" HeaderText="Order Number" />
                            <asp:BoundField DataField="deletiontime" HeaderText="Deletion Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnView" runat="server" Text="View" CommandName="ViewModule"
                                        CommandArgument='<%# Eval("moduleid") %>' CssClass="btn-view"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <asp:GridView ID="gvLesson" runat="server" AutoGenerateColumns="False" CssClass="gridview" AllowPaging="True" PageSize="8"
                        OnRowCommand="gvLesson_RowCommand" OnPageIndexChanging="gv_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="lessonid" HeaderText="Lesson ID" />
                            <asp:BoundField DataField="moduleid" HeaderText="Module ID" />
                            <asp:BoundField DataField="lessontitle" HeaderText="Lesson Title" />
                            <asp:BoundField DataField="creationtime" HeaderText="Creation Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:BoundField DataField="ordernumber" HeaderText="Order Number" />
                            <asp:BoundField DataField="deletiontime" HeaderText="Deletion Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnView" runat="server" Text="View" CommandName="ViewLesson"
                                        CommandArgument='<%# Eval("lessonid") %>' CssClass="btn-view"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <asp:GridView ID="gvMaterial" runat="server" AutoGenerateColumns="False" CssClass="gridview" AllowPaging="True" PageSize="8"
                        OnRowCommand="gvMaterial_RowCommand" OnPageIndexChanging="gv_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="materialid" HeaderText="Material ID" />
                            <asp:BoundField DataField="lessonid" HeaderText="Lesson ID" />
                            <asp:BoundField DataField="clickcount" HeaderText="Click Count" />
                            <asp:BoundField DataField="filetype" HeaderText="File Type" />
                            <asp:BoundField DataField="fileurl" HeaderText="File URL" />
                            <asp:BoundField DataField="videourl" HeaderText="Video URL" />
                            <asp:BoundField DataField="uploadtime" HeaderText="Upload Time" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnView" runat="server" Text="View" CommandName="ViewMaterial"
                                        CommandArgument='<%# Eval("materialid") %>' CssClass="btn-view"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </div>
            </div>
        <asp:Panel ID="pnlDetail" runat="server" CssClass="modal" Style="display:none" onkeypress="handleEnter(event)">

            <asp:Button ID="btnCloseModal" runat="server" Style="display:none" OnClick="btnCloseModal_Click" />
            
            <div class="modal-content">

                <span class="close" style="cursor:pointer"
                    onclick="document.getElementById('<%= btnCloseModal.ClientID %>').click();">&times;</span>

                <!-- USER PANEL -->
                <asp:Panel ID="pnlUserDetail" runat="server" Visible="false">
                    <h3>User Detail</h3>

                    <asp:HiddenField ID="hfUserId" runat="server" />

                    Username: <asp:TextBox ID="txtUname" runat="server" /><br />
                    First Name: <asp:TextBox ID="txtFname" runat="server" /><br />
                    Last Name: <asp:TextBox ID="txtLname" runat="server" /><br />
                    Age: <asp:TextBox ID="txtAge" runat="server" /><br />
                    Gender: <asp:TextBox ID="txtGender" runat="server" /><br />
                    User Type: <asp:TextBox ID="txtUserType" runat="server" /><br />
                    Status: <asp:TextBox ID="txtStatus" runat="server" /><br />

                    <asp:Button ID="btnUpdateUser" runat="server" Text="Update" OnClick="btnUpdateUser_Click" CssClass="btn-view"/>
                    <asp:Button ID="btnDeleteUser" runat="server" Text="Delete" OnClick="btnDeleteUser_Click" CssClass="btn-delete"/>
                </asp:Panel>


                <!-- VERIFICATION PANEL -->
                <asp:Panel ID="pnlVerificationDetail" runat="server" Visible="false">
                    <h3>Verification Detail</h3>

                    <asp:HiddenField ID="hfRequestId" runat="server" />

                    User ID: <asp:TextBox ID="txtVUserId" runat="server" ReadOnly="true"/><br />
                    Requested Role: <asp:TextBox ID="txtRequestedRole" runat="server" /><br />
                    Status: <asp:TextBox ID="txtVStatus" runat="server" /><br />
                    Remarks: <asp:TextBox ID="txtRemarks" runat="server" /><br />

                    <asp:Button ID="btnUpdateVerification" runat="server" Text="Update" OnClick="btnUpdateVerification_Click" CssClass="btn-view"/>
                    <asp:Button ID="btnDeleteVerification" runat="server" Text="Delete" OnClick="btnDeleteVerification_Click" CssClass="btn-delete"/>
                </asp:Panel>

                <!-- Invoice Panel -->
                <asp:Panel ID="pnlInvoiceDetail" runat="server" Visible="false">
                    <h3>Invoice Detail</h3>
                    <asp:HiddenField ID="hfInvoiceId" runat="server" />
                    User ID: <asp:TextBox ID="txtIUserId" runat="server" /><br />
                    Course ID: <asp:TextBox ID="txtICourseId" runat="server" /><br />
                    Amount: <asp:TextBox ID="txtIAmount" runat="server" /><br />
                    Overdue: <asp:TextBox ID="txtIOverdue" runat="server" /><br />
                    Duration: <asp:TextBox ID="txtIDuration" runat="server" /><br />
                    Creation Time: <asp:TextBox ID="txtICreationTime" runat="server" /><br />
                    Deadline: <asp:TextBox ID="txtIDeadline" runat="server" /><br />
                    Settlement Time: <asp:TextBox ID="txtISettlementTime" runat="server" /><br />
                    <asp:Button ID="btnUpdateInvoice" runat="server" Text="Update" OnClick="btnUpdateInvoice_Click" CssClass="btn-view"/>
                    <asp:Button ID="btnDeleteInvoice" runat="server" Text="Delete" OnClick="btnDeleteInvoice_Click" CssClass="btn-delete"/>
                </asp:Panel>

                <!-- Receipt Panel -->
                <asp:Panel ID="pnlReceiptDetail" runat="server" Visible="false">
                    <h3>Receipt Detail</h3>
                    <asp:HiddenField ID="hfReceiptId" runat="server" />
                    Invoice ID: <asp:TextBox ID="txtRInvoiceId" runat="server" /><br />
                    Amount: <asp:TextBox ID="txtRAmount" runat="server" /><br />
                    Creation Time: <asp:TextBox ID="txtRCreationTime" runat="server" /><br />
                    <asp:Button ID="btnUpdateReceipt" runat="server" Text="Update" OnClick="btnUpdateReceipt_Click" CssClass="btn-view"/>
                    <asp:Button ID="btnDeleteReceipt" runat="server" Text="Delete" OnClick="btnDeleteReceipt_Click" CssClass="btn-delete"/>
                </asp:Panel>

                <!-- Module Panel -->
                <asp:Panel ID="pnlModuleDetail" runat="server" Visible="false">
                    <h3>Module Detail</h3>
                    <asp:HiddenField ID="hfModuleId" runat="server" />
                    Course ID: <asp:TextBox ID="txtMCourseId" runat="server" /><br />
                    Module Name: <asp:TextBox ID="txtMName" runat="server" /><br />
                    Creation Time: <asp:TextBox ID="txtMCreationTime" runat="server" /><br />
                    Order Number: <asp:TextBox ID="txtMOrderNumber" runat="server" /><br />
                    Deletion Time: <asp:TextBox ID="txtMDeletionTime" runat="server" /><br />
                    <asp:Button ID="btnUpdateModule" runat="server" Text="Update" OnClick="btnUpdateModule_Click" CssClass="btn-view"/>
                    <asp:Button ID="btnDeleteModule" runat="server" Text="Delete" OnClick="btnDeleteModule_Click" CssClass="btn-delete"/>
                </asp:Panel>

                <!-- Lesson Panel -->
                <asp:Panel ID="pnlLessonDetail" runat="server" Visible="false">
                    <h3>Lesson Detail</h3>
                    <asp:HiddenField ID="hfLessonId" runat="server" />
                    Module ID: <asp:TextBox ID="txtLModuleId" runat="server" /><br />
                    Lesson Title: <asp:TextBox ID="txtLTitle" runat="server" /><br />
                    Description: <asp:TextBox ID="txtLDescription" runat="server" /><br />
                    Video URL: <asp:TextBox ID="txtLVideoUrl" runat="server" /><br />
                    File URL: <asp:TextBox ID="txtLFileUrl" runat="server" /><br />
                    Creation Time: <asp:TextBox ID="txtLCreationTime" runat="server" /><br />
                    Order Number: <asp:TextBox ID="txtLOrderNumber" runat="server" /><br />
                    Deletion Time: <asp:TextBox ID="txtLDeletionTime" runat="server" /><br />
                    <asp:Button ID="btnUpdateLesson" runat="server" Text="Update" OnClick="btnUpdateLesson_Click" CssClass="btn-view"/>
                    <asp:Button ID="btnDeleteLesson" runat="server" Text="Delete" OnClick="btnDeleteLesson_Click" CssClass="btn-delete"/>
                </asp:Panel>

                <!-- Material Panel -->
                <asp:Panel ID="pnlMaterialDetail" runat="server" Visible="false">
                    <h3>Material Detail</h3>
                    <asp:HiddenField ID="hfMaterialId" runat="server" />
                    Lesson ID: <asp:TextBox ID="txtMLessonId" runat="server" /><br />
                    Click Count: <asp:TextBox ID="txtMClickCount" runat="server" /><br />
                    File Type: <asp:TextBox ID="txtMFileType" runat="server" /><br />
                    File URL: <asp:TextBox ID="txtMFileUrl" runat="server" /><br />
                    Video URL: <asp:TextBox ID="txtMVideoUrl" runat="server" /><br />
                    Upload Time: <asp:TextBox ID="txtMUploadTime" runat="server" /><br />
                    <asp:Button ID="btnUpdateMaterial" runat="server" Text="Update" OnClick="btnUpdateMaterial_Click" CssClass="btn-view"/>
                    <asp:Button ID="btnDeleteMaterial" runat="server" Text="Delete" OnClick="btnDeleteMaterial_Click" CssClass="btn-delete"/>
                </asp:Panel>

            </div>
        </asp:Panel>
    </form>
<script>
    window.onclick = function (event) {
        var modal = document.getElementById('<%= pnlDetail.ClientID %>');
    var closeBtn = document.getElementById('<%= btnCloseModal.ClientID %>');
        if (event.target === modal) {
            closeBtn.click();
        }
    }

    function handleEnter(event) {
        if (event.key === "Enter") {
            event.preventDefault();

            var userPanel = document.getElementById('<%= pnlUserDetail.ClientID %>');
        var verPanel = document.getElementById('<%= pnlVerificationDetail.ClientID %>');
        var invoicePanel = document.getElementById('<%= pnlInvoiceDetail.ClientID %>');
        var receiptPanel = document.getElementById('<%= pnlReceiptDetail.ClientID %>');
        var modulePanel = document.getElementById('<%= pnlModuleDetail.ClientID %>');
        var lessonPanel = document.getElementById('<%= pnlLessonDetail.ClientID %>');
        var materialPanel = document.getElementById('<%= pnlMaterialDetail.ClientID %>');

        if (userPanel.style.display !== "none") {
            document.getElementById('<%= btnUpdateUser.ClientID %>').click();
        } else if (verPanel.style.display !== "none") {
            document.getElementById('<%= btnUpdateVerification.ClientID %>').click();
        } else if (invoicePanel.style.display !== "none") {
            document.getElementById('<%= btnUpdateInvoice.ClientID %>').click();
        } else if (receiptPanel.style.display !== "none") {
            document.getElementById('<%= btnUpdateReceipt.ClientID %>').click();
        } else if (modulePanel.style.display !== "none") {
            document.getElementById('<%= btnUpdateModule.ClientID %>').click();
        } else if (lessonPanel.style.display !== "none") {
            document.getElementById('<%= btnUpdateLesson.ClientID %>').click();
        } else if (materialPanel.style.display !== "none") {
                document.getElementById('<%= btnUpdateMaterial.ClientID %>').click();
            }
        }
    }
</script>
</body>
</html>
