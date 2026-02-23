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

    <!-- SIDEBAR -->
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

        <asp:Button 
            ID="btnNewChat" 
            runat="server"
            Text="+ New Chat"
            CssClass="new-chat-btn"
            OnClick="btnNewChat_Click" />

        <div class="chat-history-title">Chat History</div>

        <!-- Conversation list will be generated from C# -->
        <div id="sidebarConversations" 
             class="chat-history" 
             runat="server">
        </div>
    </div>

    <!-- MAIN CHAT AREA -->
    <div class="chat-main">

        <div class="chat-header">
            <div class="chat-title">LearnSphere AI Assistant</div>
        </div>

        <div id="chatOutput" 
             class="chat-messages" 
             runat="server">
        </div>

        <div class="chat-input-area">
            <asp:TextBox ID="txtQuestion"
                runat="server"
                CssClass="chat-input"
                placeholder="How can I help you?"
                AutoCompleteType="Disabled" />

            <asp:Button ID="btnSend"
                runat="server"
                Text="➤"
                CssClass="send-btn"
                OnClick="btnSend_Click1" />
        </div>

    </div>

</div>

<script>
    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll(".markdown-content").forEach(el => {
            el.innerHTML = marked.parse(el.innerText);
        });
    });
</script>

</form>
</body>
</html>
