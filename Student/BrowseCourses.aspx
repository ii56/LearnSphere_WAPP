<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BrowseCourses.aspx.cs" Inherits="LearnSphere_WAPP.Student.BrowseCourses" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Browse Courses - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #eef3f9;
            --bg-gradient: linear-gradient(135deg, #dbe9f9 0%, #e8eef6 40%, #f0e8f5 100%);
            --surface: #ffffff;
            --primary: #2563eb;
            --primary-bg: rgba(37,99,235,0.08);
            --accent-orange: #f59e0b;
            --accent-green: #10b981;
            --accent-purple: #8b5cf6;
            --text: #1e293b;
            --text-secondary: #64748b;
            --text-muted: #94a3b8;
            --border: #e2e8f0;
            --border-light: #f1f5f9;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04), 0 1px 2px rgba(0,0,0,0.03);
            --shadow-md: 0 4px 12px rgba(0,0,0,0.06), 0 2px 4px rgba(0,0,0,0.03);
            --radius: 14px;
            --radius-sm: 10px;
        }
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: 'DM Sans', sans-serif;
            background: var(--bg-gradient); background-attachment: fixed;
            color: var(--text); min-height: 100vh;
        }

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
            background: linear-gradient(135deg, var(--primary), var(--accent-purple));
            border-radius: 50%; display: flex; align-items: center; justify-content: center;
            font-size: 13px; font-weight: 700; color: white;
        }
        .user-name { font-size: 0.85rem; font-weight: 600; }
        .btn-logout {
            background: transparent; border: 1px solid var(--border);
            color: var(--text-secondary); padding: 7px 18px; border-radius: 8px;
            font-family: 'DM Sans', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-logout:hover { background: #fef2f2; color: #ef4444; border-color: #fecaca; }

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

        .container { max-width: 1140px; margin: 0 auto; padding: 28px 36px; }

        .page-header { margin-bottom: 24px; animation: slideUp 0.4s ease both; }
        .page-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: var(--primary); margin-bottom: 6px;
            font-family: 'Space Mono', monospace;
        }
        .page-title { font-size: 1.6rem; font-weight: 700; letter-spacing: -0.3px; }

        .alert {
            padding: 14px 20px; border-radius: var(--radius-sm);
            font-size: 0.875rem; font-weight: 500; margin-bottom: 20px;
            display: flex; align-items: center; gap: 10px;
        }
        .alert-success { background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2); color: #059669; }
        .alert-error { background: rgba(239,68,68,0.08); border: 1px solid rgba(239,68,68,0.2); color: #dc2626; }

        .course-grid {
            display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 18px;
        }
        .course-card {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 24px;
            display: flex; flex-direction: column;
            box-shadow: var(--shadow-sm); transition: transform 0.2s, box-shadow 0.2s;
            animation: slideUp 0.5s ease both;
        }
        .course-card:hover { transform: translateY(-3px); box-shadow: var(--shadow-md); }
        .course-category {
            display: inline-block; background: var(--primary-bg); color: var(--primary);
            font-size: 0.7rem; font-weight: 700; letter-spacing: 0.8px; text-transform: uppercase;
            padding: 4px 12px; border-radius: 20px; margin-bottom: 14px; width: fit-content;
        }
        .course-name { font-size: 1rem; font-weight: 700; margin-bottom: 10px; line-height: 1.4; }
        .course-desc { font-size: 0.83rem; color: var(--text-secondary); line-height: 1.6; flex: 1; margin-bottom: 18px; }
        .course-footer { display: flex; align-items: center; justify-content: space-between; margin-top: auto; }
        .course-price { font-family: 'Space Mono', monospace; font-size: 1.05rem; font-weight: 700; color: var(--text); }
        .course-price.free { color: var(--accent-green); }
        .btn-enroll {
            background: var(--primary); color: white; border: none;
            padding: 9px 20px; border-radius: 8px; font-family: 'DM Sans', sans-serif;
            font-size: 0.82rem; font-weight: 600; cursor: pointer; transition: background 0.2s;
        }
        .btn-enroll:hover { background: #1d4ed8; }
        .btn-enroll:disabled {
            background: var(--border-light); color: var(--text-muted);
            cursor: not-allowed; border: 1px solid var(--border);
        }

        /* payment modal */
        .modal-overlay {
            display: none; position: fixed; inset: 0;
            background: rgba(0,0,0,0.4); backdrop-filter: blur(4px);
            z-index: 999; align-items: center; justify-content: center;
        }
        .modal-overlay.open { display: flex; }
        .modal {
            background: var(--surface); border: 1px solid var(--border);
            border-radius: 20px; width: 100%; max-width: 440px;
            padding: 32px; position: relative;
            box-shadow: 0 20px 60px rgba(0,0,0,0.15);
            animation: modalIn 0.3s ease both;
        }
        @keyframes modalIn {
            from { opacity: 0; transform: scale(0.95) translateY(10px); }
            to { opacity: 1; transform: scale(1) translateY(0); }
        }
        .modal-close {
            position: absolute; top: 14px; right: 18px;
            background: none; border: none; color: var(--text-muted);
            font-size: 1.3rem; cursor: pointer;
        }
        .modal-close:hover { color: var(--text); }
        .modal-label {
            font-size: 0.72rem; font-weight: 700; letter-spacing: 2px;
            text-transform: uppercase; color: var(--primary); margin-bottom: 6px;
            font-family: 'Space Mono', monospace;
        }
        .modal-title { font-size: 1.2rem; font-weight: 700; margin-bottom: 4px; }
        .modal-course-name { color: var(--text-secondary); font-size: 0.85rem; margin-bottom: 20px; }
        .modal-price-box {
            background: var(--border-light); border: 1px solid var(--border);
            border-radius: 12px; padding: 14px 18px;
            display: flex; justify-content: space-between; align-items: center;
            margin-bottom: 20px;
        }
        .modal-price-label { font-size: 0.82rem; color: var(--text-secondary); }
        .modal-price-amount {
            font-family: 'Space Mono', monospace;
            font-size: 1.3rem; font-weight: 700; color: var(--accent-orange);
        }
        .form-group { margin-bottom: 14px; }
        .form-label {
            display: block; font-size: 0.78rem; font-weight: 600;
            color: var(--text-secondary); margin-bottom: 6px;
            text-transform: uppercase; letter-spacing: 0.5px;
        }
        .form-input {
            width: 100%; background: var(--border-light); border: 1px solid var(--border);
            border-radius: var(--radius-sm); padding: 11px 14px;
            color: var(--text); font-family: 'DM Sans', sans-serif;
            font-size: 0.875rem; outline: none; transition: border-color 0.2s;
        }
        .form-input:focus { border-color: var(--primary); }
        .form-input::placeholder { color: var(--text-muted); }
        .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
        .card-icons { display: flex; gap: 8px; margin-bottom: 18px; }
        .card-icon {
            background: var(--border-light); border: 1px solid var(--border);
            border-radius: 6px; padding: 4px 10px;
            font-size: 0.72rem; color: var(--text-secondary); font-weight: 700;
        }
        .btn-pay {
            width: 100%; background: var(--primary); color: white; border: none;
            padding: 13px; border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.92rem; font-weight: 700;
            cursor: pointer; transition: background 0.2s;
        }
        .btn-pay:hover { background: #1d4ed8; }
        .modal-secure {
            text-align: center; margin-top: 12px;
            font-size: 0.72rem; color: var(--text-muted);
        }

        @keyframes slideUp {
            from { opacity: 0; transform: translateY(14px); }
            to { opacity: 1; transform: translateY(0); }
        }
        @media (max-width: 900px) {
            .container { padding: 20px; }
            .header, .nav { padding: 0 20px; }
            .modal { margin: 20px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <div class="user-pill">
                    <div class="user-avatar"><asp:Label ID="lblAvatarInitial" runat="server" Text="S" /></div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="nav">
            <a href="StudentDashboard.aspx"><span>📊</span> Dashboard</a>
            <a href="BrowseCourses.aspx" class="active"><span>📚</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span>🎓</span> My Courses</a>
            <a href="Gamification.aspx"><span>🏆</span> Achievements</a>
            <a href="StudentProfile.aspx"><span>👤</span> Profile</a>
            <a href="Forums.aspx"><span>💬</span> Forums</a>
            <a href="Messaging.aspx"><span>✉️</span> Messages</a>
        </div>

        <div class="container">
            <div class="page-header">
                <div class="page-label">Catalogue</div>
                <div class="page-title">Browse Courses</div>
            </div>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />
            <asp:HiddenField ID="hfCourseId" runat="server" />

            <div class="course-grid">
                <asp:Repeater ID="rptCourses" runat="server" OnItemCommand="rptCourses_ItemCommand">
                    <ItemTemplate>
                        <div class="course-card">
                            <span class="course-category"><%# Eval("category") %></span>
                            <div class="course-name"><%# Eval("coursename") %></div>
                            <div class="course-desc"><%# Eval("description") %></div>
                            <div class="course-footer">
                                <div class='<%# Convert.ToDecimal(Eval("price")) == 0 ? "course-price free" : "course-price" %>'>
                                    <%# Convert.ToDecimal(Eval("price")) == 0 ? "FREE" : "RM " + string.Format("{0:F2}", Eval("price")) %>
                                </div>
                                <asp:Button ID="btnEnroll" runat="server"
                                    Text='<%# Convert.ToBoolean(Eval("IsEnrolled")) ? "✓ Enrolled" : Convert.ToDecimal(Eval("price")) == 0 ? "Enroll Free" : "Buy Now" %>'
                                    CommandName='<%# Convert.ToDecimal(Eval("price")) == 0 ? "EnrollFree" : "OpenPayment" %>'
                                    CommandArgument='<%# Eval("courseid") + "|" + Eval("coursename") + "|" + Eval("price") %>'
                                    CssClass="btn-enroll"
                                    Enabled='<%# !Convert.ToBoolean(Eval("IsEnrolled")) %>' />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- payment modal -->
        <div class="modal-overlay" id="paymentModal">
            <div class="modal">
                <button class="modal-close" onclick="closeModal()" type="button">✕</button>
                <div class="modal-label">Secure Checkout</div>
                <div class="modal-title">Complete Payment</div>
                <div class="modal-course-name" id="modalCourseName"></div>
                <div class="modal-price-box">
                    <span class="modal-price-label">Total Amount</span>
                    <span class="modal-price-amount" id="modalPrice"></span>
                </div>
                <div class="card-icons">
                    <span class="card-icon">VISA</span>
                    <span class="card-icon">MC</span>
                    <span class="card-icon">FPX</span>
                </div>
                <div class="form-group">
                    <label class="form-label">Cardholder Name</label>
                    <input type="text" class="form-input" id="cardName" placeholder="e.g. Ahmad bin Ali" />
                </div>
                <div class="form-group">
                    <label class="form-label">Card Number</label>
                    <input type="text" class="form-input" id="cardNumber" placeholder="1234 5678 9012 3456" maxlength="19" oninput="formatCard(this)" />
                </div>
                <div class="form-row">
                    <div class="form-group">
                        <label class="form-label">Expiry Date</label>
                        <input type="text" class="form-input" id="cardExpiry" placeholder="MM/YY" maxlength="5" oninput="formatExpiry(this)" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">CVV</label>
                        <input type="text" class="form-input" id="cardCvv" placeholder="123" maxlength="3" />
                    </div>
                </div>
                <asp:Button ID="btnConfirmPayment" runat="server" Text="Pay Now" CssClass="btn-pay"
                    OnClick="btnConfirmPayment_Click" OnClientClick="return validatePayment()" />
                <div class="modal-secure">Payments are simulated for demo purposes</div>
            </div>
        </div>
    </form>

    <script>
        window.onload = function () {
            var courseData = document.getElementById('<%= hfCourseId.ClientID %>').value;
            if (courseData && courseData !== '') {
                var parts = courseData.split('|');
                openModalWithData(parts[0], parts[1], parts[2]);
            }
        };
        function openModalWithData(id, name, price) {
            document.getElementById('modalCourseName').innerText = name;
            document.getElementById('modalPrice').innerText = 'RM ' + parseFloat(price).toFixed(2);
            document.getElementById('paymentModal').classList.add('open');
        }
        function closeModal() {
            document.getElementById('paymentModal').classList.remove('open');
            document.getElementById('<%= hfCourseId.ClientID %>').value = '';
            document.getElementById('cardName').value = '';
            document.getElementById('cardNumber').value = '';
            document.getElementById('cardExpiry').value = '';
            document.getElementById('cardCvv').value = '';
        }
        function formatCard(input) {
            var val = input.value.replace(/\D/g, '').substring(0, 16);
            input.value = val.replace(/(.{4})/g, '$1 ').trim();
        }
        function formatExpiry(input) {
            var val = input.value.replace(/\D/g, '').substring(0, 4);
            if (val.length >= 3) val = val.substring(0, 2) + '/' + val.substring(2);
            input.value = val;
        }
        function validatePayment() {
            var name = document.getElementById('cardName').value.trim();
            var number = document.getElementById('cardNumber').value.replace(/\s/g, '');
            var expiry = document.getElementById('cardExpiry').value.trim();
            var cvv = document.getElementById('cardCvv').value.trim();
            if (!name) { alert('Please enter cardholder name.'); return false; }
            if (number.length !== 16) { alert('Please enter a valid 16-digit card number.'); return false; }
            if (expiry.length !== 5) { alert('Please enter a valid expiry date (MM/YY).'); return false; }
            if (cvv.length !== 3) { alert('Please enter a valid 3-digit CVV.'); return false; }
            return true;
        }
        document.getElementById('paymentModal').addEventListener('click', function (e) {
            if (e.target === this) closeModal();
        });
    </script>
</body>
</html>
