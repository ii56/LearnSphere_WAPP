<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Chatbot.aspx.cs" Inherits="LearnSphere_WAPP.Chatbot.Chatbot" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>LearnSphere Chatbot</title>
    <link href="Chatbot.css" rel="stylesheet" type="text/css" />
    <script src="https://cdn.jsdelivr.net/npm/marked/marked.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">

        <div class="chat-container">

<div class="sidebar">
    <div class="profile-section">
        <div class="avatar"></div>
        <div class="username">Welcome</div>
    </div>

    <asp:Button 
        ID="btnBack" 
        runat="server"
        Text="← Go Back"
        CssClass="back-btn"
        OnClick="btnBack_Click" />
</div>

            <div class="chat-main">

                <div class="chat-header">
                    <div class="chat-title">LearnSphere AI Assistant</div>
                </div>

                <div id="bp-embedded-webchat" style="flex:1;"></div>

            </div>

        </div>

        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>

    </form>
</body>
</html>
