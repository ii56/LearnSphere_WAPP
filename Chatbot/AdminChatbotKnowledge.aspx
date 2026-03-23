<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminChatbotKnowledge.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminChatbotKnowledge" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chatbot Management - LearnSphere</title>
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
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
            --accent-orange: #f59e0b;
            --accent-red: #ef4444;
            --accent-cyan: #0891b2;
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
            background: var(--bg-gradient); background-attachment: fixed;
            color: var(--text); min-height: 100vh;
        }

        /* HEADER */
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(255,255,255,0.82); backdrop-filter: blur(20px);
            border-bottom: 1px solid var(--border);
            padding: 0 36px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 12px; }
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
            background: linear-gradient(135deg, #ef4444, var(--accent-purple));
            border-radius: 50%; overflow: hidden; position: relative;
        }
        .user-avatar img { width:100%; height:100%; object-fit:cover; border-radius:50%; position:absolute; top:0; left:0; }
        .user-name { font-size: 0.85rem; font-weight: 600; color: var(--text); }
        .admin-badge {
            display: inline-flex; align-items: center; gap: 5px;
            background: rgba(239,68,68,0.1); border: 1px solid rgba(239,68,68,0.25);
            color: #dc2626; font-size: 0.72rem; font-weight: 700;
            padding: 4px 12px; border-radius: 20px;
        }

        /* NAV */
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

        /* CONTAINER */
        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }

        /* BACK BUTTON */
        .btn-back {
            display:inline-flex; align-items:center; gap:8px;
            background:var(--surface); border:1px solid var(--border);
            color:var(--text-secondary); padding:8px 18px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.83rem; font-weight:600;
            cursor:pointer; transition:all 0.2s; margin-bottom:24px;
        }
        .btn-back:hover { border-color:var(--primary); color:var(--primary); }

        /* TAB SWITCHER */
        .tab-bar {
            display: flex; gap: 0;
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 6px;
            margin-bottom: 28px; width: fit-content;
            box-shadow: var(--shadow-sm);
        }
        .tab-btn {
            padding: 10px 28px; border-radius: 10px; border: none;
            font-family: 'DM Sans', sans-serif; font-size: 0.875rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s; color: var(--text-muted);
            background: transparent; display: flex; align-items: center; gap: 8px;
        }
        .tab-btn:hover { color: var(--text-secondary); background: var(--border-light); }
        .tab-btn.active-knowledge {
            background: linear-gradient(135deg,#7c3aed,#8b5cf6);
            color: white; box-shadow: 0 3px 10px rgba(139,92,246,0.3);
        }
        .tab-btn.active-rules {
            background: linear-gradient(135deg,#0891b2,#06b6d4);
            color: white; box-shadow: 0 3px 10px rgba(8,145,178,0.3);
        }

        /* BANNERS */
        .page-banner {
            border-radius: var(--radius); padding: 28px 36px;
            margin-bottom: 24px; position: relative; overflow: hidden;
            animation: slideDown 0.4s ease both;
        }
        .banner-knowledge { background: linear-gradient(135deg,#7c3aed,#8b5cf6 55%,#a78bfa); box-shadow:0 8px 30px rgba(139,92,246,0.25); }
        .banner-rules     { background: linear-gradient(135deg,#0891b2,#06b6d4 55%,#67e8f9); box-shadow:0 8px 30px rgba(8,145,178,0.28); }
        .page-banner::before {
            content:''; position:absolute; top:-40%; right:-10%; width:280px; height:280px;
            background:radial-gradient(circle,rgba(255,255,255,0.12),transparent 65%);
            border-radius:50%; pointer-events:none;
        }
        .banner-label { font-size:0.72rem; font-weight:700; letter-spacing:2px; text-transform:uppercase; color:rgba(255,255,255,0.75); margin-bottom:6px; font-family:'Space Mono',monospace; }
        .banner-title { font-size:1.5rem; font-weight:700; color:white; margin-bottom:4px; }
        .banner-sub   { color:rgba(255,255,255,0.75); font-size:0.85rem; }

        /* FORM CARD */
        .form-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); padding:28px 32px;
            box-shadow:var(--shadow-sm); margin-bottom:20px;
            animation:slideUp 0.4s ease both;
        }
        .form-card-title {
            font-size:0.95rem; font-weight:700; margin-bottom:20px;
            display:flex; align-items:center; gap:10px; color:var(--text);
        }
        .title-dot { width:8px; height:8px; border-radius:50%; flex-shrink:0; }
        .dot-purple { background:var(--accent-purple); }
        .dot-cyan   { background:var(--accent-cyan); }
        .form-group { margin-bottom:16px; }
        .form-label {
            display:block; font-size:0.75rem; font-weight:600;
            color:var(--text-secondary); text-transform:uppercase;
            letter-spacing:0.5px; margin-bottom:7px;
        }
        .form-input {
            width:100%; background:var(--border-light); border:1px solid var(--border);
            border-radius:var(--radius-sm); padding:10px 14px;
            font-family:'DM Sans',sans-serif; font-size:0.875rem;
            color:var(--text); outline:none; transition:border-color 0.2s; box-sizing:border-box;
        }
        .form-input:focus { border-color:var(--primary); background:white; }
        textarea.form-input { resize:vertical; min-height:90px; line-height:1.5; }
        .btn-row { display:flex; gap:10px; margin-top:20px; flex-wrap:wrap; align-items:center; }

        /* ACTION BUTTONS (form) */
        .btn-primary {
            background:var(--primary); color:white; border:none;
            padding:9px 22px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.83rem; font-weight:600;
            cursor:pointer; transition:background 0.2s;
        }
        .btn-primary:hover { background:#1d4ed8; }
        .btn-secondary {
            background:var(--border-light); color:var(--text-secondary);
            border:1px solid var(--border); padding:9px 22px; border-radius:8px;
            font-family:'DM Sans',sans-serif; font-size:0.83rem; font-weight:600;
            cursor:pointer; transition:all 0.2s;
        }
        .btn-secondary:hover { border-color:var(--primary); color:var(--primary); }

        /* SECTION CARD (table wrapper) */
        .section-card {
            background:var(--surface); border:1px solid var(--border);
            border-radius:var(--radius); box-shadow:var(--shadow-sm);
            overflow:hidden; margin-bottom:20px;
            animation:slideUp 0.5s ease both;
        }
        .section-header {
            padding:16px 24px; border-bottom:1px solid var(--border);
            display:flex; align-items:center; justify-content:space-between;
        }
        .section-title { font-size:0.9rem; font-weight:700; color:var(--text); display:flex; align-items:center; gap:10px; }

        /* TABLE */
        .section-card table { width:100%; border-collapse:collapse; }
        .section-card table th {
            background:var(--border-light); padding:10px 20px; text-align:left;
            font-size:0.72rem; font-weight:700; letter-spacing:1px; text-transform:uppercase;
            color:var(--text-muted); border-bottom:1px solid var(--border);
        }
        .section-card table td {
            padding:12px 20px; font-size:0.875rem; color:var(--text);
            border-bottom:1px solid var(--border-light); vertical-align:middle;
        }
        .section-card table tr:last-child td { border-bottom:none; }
        .section-card table tr:hover td { background:var(--surface-hover); }

        /* GRID ACTION BUTTONS */
        .grid-btn {
            padding: 5px 14px;
            border-radius: 7px;
            font-family: 'DM Sans', sans-serif;
            font-size: 0.77rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s;
            border: 1px solid transparent;
        }
        .grid-btn-edit {
            background: var(--primary-bg);
            color: var(--primary);
            border-color: var(--primary-border);
        }
        .grid-btn-edit:hover { background: var(--primary); color: white; }

        .grid-btn-toggle {
            background: rgba(245,158,11,0.1);
            color: var(--accent-orange);
            border-color: rgba(245,158,11,0.3);
        }
        .grid-btn-toggle:hover { background: var(--accent-orange); color: white; }

        .grid-btn-delete {
            background: rgba(239,68,68,0.08);
            color: var(--accent-red);
            border-color: rgba(239,68,68,0.2);
        }
        .grid-btn-delete:hover { background: var(--accent-red); color: white; }

        /* STATUS BADGES */
        .badge-active   { background:rgba(16,185,129,0.1); color:#059669; border:1px solid rgba(16,185,129,0.25); padding:3px 10px; border-radius:20px; font-size:0.72rem; font-weight:700; }
        .badge-inactive { background:rgba(239,68,68,0.08); color:#ef4444; border:1px solid rgba(239,68,68,0.2); padding:3px 10px; border-radius:20px; font-size:0.72rem; font-weight:700; }

        /* TAB PANEL VISIBILITY */
        .tab-panel { display: none; }
        .tab-panel.visible { display: block; animation: slideUp 0.35s ease both; }

        /* ANIMATIONS */
        @keyframes slideDown { from{opacity:0;transform:translateY(-12px);}to{opacity:1;transform:translateY(0);} }
        @keyframes slideUp   { from{opacity:0;transform:translateY(14px);}to{opacity:1;transform:translateY(0);} }

        @media(max-width:900px){
            .container{padding:20px;}
            .header,.nav{padding:0 20px;}
            .tab-bar { width: 100%; }
            .tab-btn { flex: 1; justify-content: center; }
        }
    </style>
</head>
<body>
<form id="form1" runat="server">

    <!-- HEADER -->
    <div class="header">
        <div class="logo">
            <img src="~/LEARNSPHERE.png" runat="server" />
            <div class="logo-text">Learn<span>Sphere</span></div>
        </div>
        <div class="header-right">
            <span class="admin-badge">🛡 Admin</span>
            <div class="user-pill">
                <div class="user-avatar">
                    <img id="imgAdminProfile" runat="server" src="~/images/default-user.png" />
                </div>
                <span class="user-name"><%= Session["uname"] != null ? Server.HtmlEncode(Session["uname"].ToString()) : "" %></span>
            </div>
        </div>
    </div>

    <!-- NAV -->
    <div class="nav">
        <a href="AdminDashboard.aspx"><span>📊</span> Dashboard</a>
        <a href="AdminChatbotKnowledge.aspx" class="active"><span>🤖</span> Chatbot</a>
    </div>

    <div class="container">

        <asp:Button ID="btnBack" runat="server" Text="← Back"
            CssClass="btn-back" OnClientClick="history.back(); return false;" />

        <!-- TAB SWITCHER -->
        <div class="tab-bar">
            <button type="button" id="tabBtnKnowledge"
                class="tab-btn active-knowledge"
                onclick="switchTab('knowledge')">
                🧠 Knowledge Base
            </button>
            <button type="button" id="tabBtnRules"
                class="tab-btn"
                onclick="switchTab('rules')">
                📋 Rules
            </button>
        </div>

        <!-- Tab 1 for knowledge base -->
        <div id="tabKnowledge" class="tab-panel visible">

            <div class="page-banner banner-knowledge">
                <div class="banner-label">Chatbot Management</div>
                <div class="banner-title">Knowledge Base</div>
                <div class="banner-sub">Add, edit or deactivate question-and-answer entries the chatbot uses to respond.</div>
            </div>

            <asp:HiddenField ID="hfKnowledgeID" runat="server" />

            <div class="form-card">
                <div class="form-card-title">
                    <span class="title-dot dot-purple"></span> Knowledge Entry
                </div>

                <div class="form-group">
                    <label class="form-label">Question *</label>
                    <asp:TextBox ID="txtQuestion" runat="server" CssClass="form-input" MaxLength="500" />
                </div>
                <div class="form-group">
                    <label class="form-label">Answer *</label>
                    <asp:TextBox ID="txtAnswer" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="4" MaxLength="2000" />
                </div>
                <div class="form-group">
                    <label class="form-label">Category</label>
                    <asp:TextBox ID="txtCategory" runat="server" CssClass="form-input" MaxLength="100" />
                </div>

                <div class="btn-row">
                    <asp:Button ID="btnSaveKnowledge" runat="server" Text="Add Knowledge"
                        CssClass="btn-primary" OnClick="btnSaveKnowledge_Click" />
                    <asp:Button runat="server" Text="Clear"
                        CssClass="btn-secondary" CausesValidation="false"
                        OnClientClick="clearKnowledge(); return false;" />
                </div>
            </div>

            <div class="section-card">
                <div class="section-header">
                    <div class="section-title">
                        <span class="title-dot dot-purple"></span> All Knowledge Entries
                    </div>
                </div>
                <asp:GridView ID="gvKnowledge" runat="server"
                    AutoGenerateColumns="False" Width="100%"
                    BorderStyle="None" GridLines="None"
                    OnRowCommand="gvKnowledge_RowCommand"
                    EmptyDataText="No knowledge entries yet.">
                    <Columns>
                        <asp:BoundField DataField="knowledgeID" HeaderText="ID" />
                        <asp:BoundField DataField="question"    HeaderText="Question" />
                        <asp:BoundField DataField="category"    HeaderText="Category" />
                        <asp:BoundField DataField="CreatedBy"   HeaderText="Created By" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# Convert.ToBoolean(Eval("isActive")) ? "badge-active" : "badge-inactive" %>'>
                                    <%# Convert.ToBoolean(Eval("isActive")) ? "Active" : "Inactive" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button runat="server" Text="Edit"
                                    CommandName="EditKnowledge"
                                    CommandArgument='<%# Container.DataItemIndex %>'
                                    CssClass="grid-btn grid-btn-edit" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button runat="server" Text="Toggle"
                                    CommandName="ToggleKnowledge"
                                    CommandArgument='<%# Container.DataItemIndex %>'
                                    CssClass="grid-btn grid-btn-toggle" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button runat="server" Text="Delete"
                                    CommandName="DeleteKnowledge"
                                    CommandArgument='<%# Container.DataItemIndex %>'
                                    CssClass="grid-btn grid-btn-delete"
                                    OnClientClick="return confirm('Delete this entry?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

        </div>

        <!-- Tab 2 for rules -->
        <div id="tabRules" class="tab-panel">

            <div class="page-banner banner-rules">
                <div class="banner-label">Chatbot Management</div>
                <div class="banner-title">Chatbot Rules</div>
                <div class="banner-sub">Define behaviour rules that guide how the chatbot responds in different situations.</div>
            </div>

            <asp:HiddenField ID="hfRuleID" runat="server" />

            <div class="form-card">
                <div class="form-card-title">
                    <span class="title-dot dot-cyan"></span> Rule Entry
                </div>

                <div class="form-group">
                    <label class="form-label">Rule Name *</label>
                    <asp:TextBox ID="txtRuleName" runat="server" CssClass="form-input" MaxLength="200" />
                </div>
                <div class="form-group">
                    <label class="form-label">Rule Description</label>
                    <asp:TextBox ID="txtRuleDescription" runat="server" CssClass="form-input" MaxLength="500" />
                </div>
                <div class="form-group">
                    <label class="form-label">Rule Content *</label>
                    <asp:TextBox ID="txtRuleContent" runat="server" CssClass="form-input"
                        TextMode="MultiLine" Rows="4" MaxLength="2000" />
                </div>

                <div class="btn-row">
                    <asp:Button ID="btnSaveRule" runat="server" Text="Add Rule"
                        CssClass="btn-primary" OnClick="btnSaveRule_Click" />
                    <asp:Button runat="server" Text="Clear"
                        CssClass="btn-secondary" CausesValidation="false"
                        OnClientClick="clearRules(); return false;" />
                </div>
            </div>

            <div class="section-card">
                <div class="section-header">
                    <div class="section-title">
                        <span class="title-dot dot-cyan"></span> All Rules
                    </div>
                </div>
                <asp:GridView ID="gvRules" runat="server"
                    AutoGenerateColumns="False" Width="100%"
                    BorderStyle="None" GridLines="None"
                    OnRowCommand="gvRules_RowCommand"
                    EmptyDataText="No rules defined yet.">
                    <Columns>
                        <asp:BoundField DataField="ruleID"    HeaderText="ID" />
                        <asp:BoundField DataField="ruleName"  HeaderText="Rule Name" />
                        <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# Convert.ToBoolean(Eval("isActive")) ? "badge-active" : "badge-inactive" %>'>
                                    <%# Convert.ToBoolean(Eval("isActive")) ? "Active" : "Inactive" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button runat="server" Text="Edit"
                                    CommandName="EditRule"
                                    CommandArgument='<%# Container.DataItemIndex %>'
                                    CssClass="grid-btn grid-btn-edit" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button runat="server" Text="Toggle"
                                    CommandName="ToggleRule"
                                    CommandArgument='<%# Container.DataItemIndex %>'
                                    CssClass="grid-btn grid-btn-toggle" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button runat="server" Text="Delete"
                                    CommandName="DeleteRule"
                                    CommandArgument='<%# Container.DataItemIndex %>'
                                    CssClass="grid-btn grid-btn-delete"
                                    OnClientClick="return confirm('Delete this rule?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

        </div>

    </div>

    <!-- Remembers which tab was active across postbacks -->
    <input type="hidden" id="activeTab" name="activeTab" value="knowledge" />

</form>

<script>
    // Switch between Knowledge and Rules tabs
    function switchTab(tab) {
        document.getElementById('tabKnowledge').classList.remove('visible');
        document.getElementById('tabRules').classList.remove('visible');
        document.getElementById('tabBtnKnowledge').className = 'tab-btn';
        document.getElementById('tabBtnRules').className = 'tab-btn';

        if (tab === 'knowledge') {
            document.getElementById('tabKnowledge').classList.add('visible');
            document.getElementById('tabBtnKnowledge').className = 'tab-btn active-knowledge';
        } else {
            document.getElementById('tabRules').classList.add('visible');
            document.getElementById('tabBtnRules').className = 'tab-btn active-rules';
        }
        document.getElementById('activeTab').value = tab;
    }

    // Clear the knowledge form fields without a postback
    function clearKnowledge() {
        document.getElementById('<%= txtQuestion.ClientID %>').value = '';
        document.getElementById('<%= txtAnswer.ClientID %>').value = '';
        document.getElementById('<%= txtCategory.ClientID %>').value = '';
        document.getElementById('<%= hfKnowledgeID.ClientID %>').value = '';
        document.getElementById('<%= btnSaveKnowledge.ClientID %>').value = 'Add Knowledge';
    }

    // Clear the rules form fields without a postback
    function clearRules() {
        document.getElementById('<%= txtRuleName.ClientID %>').value = '';
        document.getElementById('<%= txtRuleDescription.ClientID %>').value = '';
        document.getElementById('<%= txtRuleContent.ClientID %>').value = '';
        document.getElementById('<%= hfRuleID.ClientID %>').value = '';
        document.getElementById('<%= btnSaveRule.ClientID %>').value = 'Add Rule';
    }

    // Restore the active tab after a postback so the user stays where they were
    window.onload = function () {
        var saved = document.getElementById('activeTab').value;
        if (saved === 'rules') switchTab('rules');
    };
</script>
</body>
</html>
