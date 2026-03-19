<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminChatbotKnowledge.aspx.cs" Inherits="LearnSphere_WAPP.Admin.AdminChatbotKnowledge" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Chatbot Management</title>

<style>

body{
    font-family:Segoe UI;
    background:#f4f6f9;
}

.container{
    width:1100px;
    margin:auto;
    margin-top:30px;
    background:white;
    padding:25px;
    border-radius:8px;
    box-shadow:0 3px 10px rgba(0,0,0,0.1);
}

.section{
    margin-top:30px;
}

h2{
    margin-bottom:10px;
}

.textbox{
    width:100%;
    padding:10px;
    margin-bottom:12px;
    border:1px solid #ccc;
    border-radius:4px;
}

.btn{
    padding:8px 14px;
    border:none;
    border-radius:4px;
    background:#007bff;
    color:white;
    cursor:pointer;
}

.btn:hover{
    background:#0056b3;
}

.grid{
    width:100%;
    margin-top:20px;
}

.grid th{
    background:#343a40;
    color:white;
    padding:10px;
}

.grid td{
    padding:10px;
    border-bottom:1px solid #eee;
}

.grid tr:hover{
    background:#f1f1f1;
}

.backbtn{
    background:#6c757d;
}

.backbtn:hover{
    background:#545b62;
}

</style>

</head>

<body>

<form id="form1" runat="server">

<div class="container">

<!-- BACK BUTTON -->

<asp:Button 
    ID="btnBack"
    runat="server"
    Text="Back"
    CssClass="btn backbtn"
    OnClientClick="history.back(); return false;" />



<!-- ========================= -->
<!-- KNOWLEDGE MANAGEMENT -->
<!-- ========================= -->

<div class="section">

<h2>Chatbot Knowledge Base</h2>

<asp:HiddenField ID="hfKnowledgeID" runat="server" />

<label>Question</label>
<asp:TextBox ID="txtQuestion" runat="server" CssClass="textbox"></asp:TextBox>

<label>Answer</label>
<asp:TextBox ID="txtAnswer" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="4"></asp:TextBox>

<label>Category</label>
<asp:TextBox ID="txtCategory" runat="server" CssClass="textbox"></asp:TextBox>

<asp:Button 
    ID="btnSaveKnowledge"
    runat="server"
    Text="Add Knowledge"
    CssClass="btn"
    OnClick="btnSaveKnowledge_Click" />


<asp:GridView 
    ID="gvKnowledge"
    runat="server"
    AutoGenerateColumns="False"
    CssClass="grid"
    OnRowCommand="gvKnowledge_RowCommand">

<Columns>

<asp:BoundField DataField="knowledgeID" HeaderText="ID" />
<asp:BoundField DataField="question" HeaderText="Question" />
<asp:BoundField DataField="category" HeaderText="Category" />
<asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
<asp:BoundField DataField="isActive" HeaderText="Active" />

<asp:ButtonField Text="Edit" CommandName="EditKnowledge" ButtonType="Button"/>
<asp:ButtonField Text="Toggle" CommandName="ToggleKnowledge" ButtonType="Button"/>
<asp:ButtonField Text="Delete" CommandName="DeleteKnowledge" ButtonType="Button"/>

</Columns>

</asp:GridView>

</div>



<!-- ========================= -->
<!-- RULE MANAGEMENT -->
<!-- ========================= -->

<div class="section">

<h2>Chatbot Rules</h2>

<asp:HiddenField ID="hfRuleID" runat="server" />

<label>Rule Name</label>
<asp:TextBox ID="txtRuleName" runat="server" CssClass="textbox"></asp:TextBox>

<label>Rule Description</label>
<asp:TextBox ID="txtRuleDescription" runat="server" CssClass="textbox"></asp:TextBox>

<label>Rule Content</label>
<asp:TextBox ID="txtRuleContent" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="4"></asp:TextBox>

<asp:Button 
    ID="btnSaveRule"
    runat="server"
    Text="Add Rule"
    CssClass="btn"
    OnClick="btnSaveRule_Click" />


<asp:GridView 
    ID="gvRules"
    runat="server"
    AutoGenerateColumns="False"
    CssClass="grid"
    OnRowCommand="gvRules_RowCommand">

<Columns>

<asp:BoundField DataField="ruleID" HeaderText="ID"/>
<asp:BoundField DataField="ruleName" HeaderText="Rule Name"/>
<asp:BoundField DataField="CreatedBy" HeaderText="Created By"/>
<asp:BoundField DataField="isActive" HeaderText="Active"/>

<asp:ButtonField Text="Edit" CommandName="EditRule" ButtonType="Button"/>
<asp:ButtonField Text="Toggle" CommandName="ToggleRule" ButtonType="Button"/>
<asp:ButtonField Text="Delete" CommandName="DeleteRule" ButtonType="Button"/>

</Columns>

</asp:GridView>

</div>

</div>

</form>

</body>
</html>