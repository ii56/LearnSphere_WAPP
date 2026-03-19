<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewCourses.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.ViewCourses" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Browse Courses - LearnSphere</title>
    <link href="ViewCourse.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="layout">
            
            <div class="sidebar">
                <div>
                    <div class="sidebar-title">LearnSphere</div>
                    <a href="GeneralDashboard.aspx" class="nav-item">Dashboard</a>
                    <a href="ViewCourses.aspx" class="nav-item active">Browse Courses</a>
                    <a href="MyCourse.aspx" class="nav-item">My Learning</a>
                    <a href="Forums.aspx" class="nav-item">Course Forums</a>
                    <a href="EditProfile.aspx" class="nav-item">Edit Profile</a>
                </div>

                <div class="sidebar-profile">
                    <div class="profile-box not-verified">
                        <div class="profile-img-wrapper">
                            <img id="imgSidebarProfile" runat="server" class="profile-img" src="../Assets/default-avatar.png" alt="Profile Image" />
                        </div>

                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] != null ? Session["uname"].ToString() : "Guest" %></div>
                            <div class="profile-status">General User</div>
                        </div>
                    </div>

                    <a href="Message.aspx" class="nav-item message-link">
                        Messages
                        <asp:Literal ID="litUnreadBadge" runat="server"></asp:Literal>
                    </a>

                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">
                <div class="page-header">
                    <div>
                        <h2>Explore Courses</h2>
                        <span class="section-sub">Find the perfect course to enhance your skills.</span>
                    </div>
                </div>

                <div class="filter-bar">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="modern-input filter-input" Placeholder="Search by name..." />
                    
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="modern-input filter-input">
                        <asp:ListItem Value="">All Categories</asp:ListItem>
                        <asp:ListItem>AI</asp:ListItem>
                        <asp:ListItem>Programming</asp:ListItem>
                        <asp:ListItem>Machine Learning</asp:ListItem>
                        <asp:ListItem>Data Science</asp:ListItem>
                    </asp:DropDownList>

                    <asp:TextBox ID="txtMaxPrice" runat="server" CssClass="modern-input filter-input" Placeholder="Max Price ($)" TextMode="Number" />

                    <asp:Button ID="btnFilter" runat="server" Text="Search" CssClass="btn-filter-action" OnClick="btnFilter_Click" />
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn-neutral" OnClick="btnReset_Click" />
                </div>

                <div class="courses-card">
                    <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" 
                        CssClass="courses-table" DataKeyNames="courseid" OnRowCommand="gvCourses_RowCommand"
                        EmptyDataText="No courses found matching your criteria." GridLines="None">
                        <Columns>
                            
                            <asp:BoundField DataField="coursename" HeaderText="Course Title" />
                            <asp:BoundField DataField="lecturerName" HeaderText="Instructor" />
                            <asp:BoundField DataField="category" HeaderText="Category" />
                            
                            <asp:TemplateField HeaderText="Price">
                                <ItemTemplate>
                                    <span class="course-price-tag">
                                        <%# Convert.ToDecimal(Eval("price")) == 0 ? "Free" : "RM " + Eval("price", "{0:F2}") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    
                                    <asp:Button ID="btnViewDetails" runat="server" Text="View Details" 
                                        CssClass="btn-view-details" 
                                        CommandName="ViewDetails" 
                                        CommandArgument='<%# Eval("courseid") %>' />
                                    
                                    <asp:Button ID="btnEnroll" runat="server" 
                                        Text='<%# Convert.ToDecimal(Eval("price")) == 0 ? "Enroll Free" : "Buy Now" %>'
                                        CssClass="btn-enroll-main" 
                                        CommandName="ViewDetails" 
                                        CommandArgument='<%# Eval("courseid") %>'
                                        Visible='<%# !Convert.ToBoolean(Eval("IsEnrolled")) %>' />

                                    <span class="enrolled-badge" runat="server" visible='<%# Convert.ToBoolean(Eval("IsEnrolled")) %>'>
                                        ✓ Enrolled
                                    </span>

                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    
                    <div style="margin-top: 15px;">
                        <asp:Label ID="lblMessage" runat="server" Font-Bold="true" />
                    </div>
                </div>
            </div>
        </div>

        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>
</body>
</html>