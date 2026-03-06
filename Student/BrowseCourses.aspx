<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BrowseCourses.aspx.cs" Inherits="LearnSphere_WAPP.Student.BrowseCourses" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Browse Courses - LearnSphere</title>
    <link href="https://fonts.googleapis.com/css2?family=Sora:wght@300;400;600;700&family=JetBrains+Mono:wght@400;600&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg: #0b0f1a;
            --surface: #111827;
            --surface2: #1a2235;
            --accent: #e94560;
            --accent2: #f97316;
            --gold: #f59e0b;
            --text: #e8eaf0;
            --muted: #6b7280;
            --border: rgba(255,255,255,0.07);
        }

        * { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'Sora', sans-serif;
            background: var(--bg);
            color: var(--text);
            min-height: 100vh;
        }

        body::before {
            content: '';
            position: fixed;
            top: -50%; left: -50%;
            width: 200%; height: 200%;
            background:
                radial-gradient(ellipse 600px 400px at 20% 20%, rgba(233,69,96,0.06) 0%, transparent 60%),
                radial-gradient(ellipse 500px 300px at 80% 80%, rgba(249,115,22,0.05) 0%, transparent 60%);
            z-index: 0; pointer-events: none;
        }

        /* HEADER */
        .header {
            position: sticky; top: 0; z-index: 100;
            background: rgba(11,15,26,0.85);
            backdrop-filter: blur(16px);
            border-bottom: 1px solid var(--border);
            padding: 0 40px; height: 64px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .logo { display: flex; align-items: center; gap: 10px; }
        .logo-icon {
            width: 32px; height: 32px;
            background: linear-gradient(135deg, var(--accent), var(--accent2));
            border-radius: 8px;
            display: flex; align-items: center; justify-content: center;
            font-size: 16px; font-weight: 700; color: white;
        }
        .logo-text { font-size: 1.2rem; font-weight: 700; letter-spacing: -0.5px; }
        .logo-text span { color: var(--accent); }
        .header-right { display: flex; align-items: center; gap: 16px; }
        .user-pill {
            display: flex; align-items: center; gap: 10px;
            background: var(--surface2);
            border: 1px solid var(--border);
            border-radius: 50px; padding: 6px 16px 6px 6px;
        }
        .user-avatar {
            width: 30px; height: 30px;
            background: linear-gradient(135deg, var(--accent), var(--accent2));
            border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            font-size: 13px; font-weight: 700; color: white;
        }
        .user-name { font-size: 0.85rem; font-weight: 600; }
        .btn-logout {
            background: transparent;
            border: 1px solid rgba(233,69,96,0.4);
            color: var(--accent); padding: 7px 18px; border-radius: 8px;
            font-family: 'Sora', sans-serif; font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: all 0.2s;
        }
        .btn-logout:hover { background: var(--accent); color: white; border-color: var(--accent); }

        /* NAV */
        .nav {
            background: var(--surface);
            border-bottom: 1px solid var(--border);
            padding: 0 40px; display: flex; gap: 4px;
            position: relative; z-index: 1;
        }
        .nav a {
            color: var(--muted); text-decoration: none;
            padding: 14px 18px; font-size: 0.875rem; font-weight: 500;
            display: flex; align-items: center; gap: 8px;
            border-bottom: 2px solid transparent; transition: all 0.2s;
        }
        .nav a:hover { color: var(--text); }
        .nav a.active { color: var(--accent); border-bottom-color: var(--accent); }

        /* MAIN */
        .container {
            max-width: 1200px; margin: 0 auto;
            padding: 36px 40px; position: relative; z-index: 1;
        }

        .page-header { margin-bottom: 32px; animation: fadeUp 0.4s ease both; }
        .page-label {
            font-size: 0.75rem; font-weight: 600;
            letter-spacing: 2px; text-transform: uppercase;
            color: var(--accent); margin-bottom: 8px;
            font-family: 'JetBrains Mono', monospace;
        }
        .page-title { font-size: 1.8rem; font-weight: 700; letter-spacing: -0.5px; }

        .alert {
            padding: 14px 20px; border-radius: 10px;
            font-size: 0.875rem; font-weight: 500;
            margin-bottom: 24px; display: flex; align-items: center; gap: 10px;
            animation: fadeUp 0.3s ease both;
        }
        .alert-success {
            background: rgba(16,185,129,0.1);
            border: 1px solid rgba(16,185,129,0.3);
            color: #10b981;
        }
        .alert-error {
            background: rgba(233,69,96,0.1);
            border: 1px solid rgba(233,69,96,0.3);
            color: var(--accent);
        }

        /* COURSE GRID */
        .course-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 20px;
        }

        .course-card {
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: 16px; padding: 24px;
            display: flex; flex-direction: column;
            transition: transform 0.2s, border-color 0.2s;
            animation: fadeUp 0.5s ease both;
            position: relative; overflow: hidden;
        }
        .course-card::before {
            content: '';
            position: absolute; top: 0; left: 0; right: 0;
            height: 3px;
            background: linear-gradient(90deg, var(--accent), var(--accent2));
            opacity: 0; transition: opacity 0.2s;
        }
        .course-card:hover { transform: translateY(-3px); border-color: rgba(233,69,96,0.3); }
        .course-card:hover::before { opacity: 1; }

        .course-category {
            display: inline-block;
            background: rgba(233,69,96,0.15);
            color: var(--accent);
            font-size: 0.7rem; font-weight: 700;
            letter-spacing: 1px; text-transform: uppercase;
            padding: 4px 12px; border-radius: 20px;
            margin-bottom: 14px; width: fit-content;
        }
        .course-name {
            font-size: 1rem; font-weight: 700;
            margin-bottom: 10px; letter-spacing: -0.3px; line-height: 1.4;
        }
        .course-desc {
            font-size: 0.83rem; color: var(--muted);
            line-height: 1.6; flex: 1; margin-bottom: 18px;
        }
        .course-footer {
            display: flex; align-items: center;
            justify-content: space-between; margin-top: auto;
        }
        .course-price {
            font-family: 'JetBrains Mono', monospace;
            font-size: 1.1rem; font-weight: 700; color: var(--text);
        }
        .course-price.free { color: #10b981; }

        .btn-enroll {
            background: linear-gradient(135deg, var(--accent), var(--accent2));
            color: white; border: none;
            padding: 9px 20px; border-radius: 8px;
            font-family: 'Sora', sans-serif;
            font-size: 0.82rem; font-weight: 600;
            cursor: pointer; transition: opacity 0.2s;
        }
        .btn-enroll:hover { opacity: 0.85; }
        .btn-enroll:disabled {
            background: var(--surface2); color: var(--muted);
            cursor: not-allowed; opacity: 1;
            border: 1px solid var(--border);
        }

        /* ── PAYMENT MODAL ── */
        .modal-overlay {
            display: none;
            position: fixed; inset: 0;
            background: rgba(0,0,0,0.7);
            backdrop-filter: blur(6px);
            z-index: 999;
            align-items: center; justify-content: center;
        }
        .modal-overlay.open { display: flex; }

        .modal {
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: 20px;
            width: 100%; max-width: 460px;
            padding: 36px; position: relative;
            animation: modalIn 0.3s cubic-bezier(0.34,1.56,0.64,1) both;
        }

        @keyframes modalIn {
            from { opacity: 0; transform: scale(0.9) translateY(20px); }
            to   { opacity: 1; transform: scale(1) translateY(0); }
        }

        .modal-close {
            position: absolute; top: 16px; right: 20px;
            background: none; border: none;
            color: var(--muted); font-size: 1.4rem;
            cursor: pointer; transition: color 0.2s;
            line-height: 1;
        }
        .modal-close:hover { color: var(--text); }

        .modal-label {
            font-size: 0.72rem; font-weight: 600;
            letter-spacing: 2px; text-transform: uppercase;
            color: var(--accent); margin-bottom: 8px;
            font-family: 'JetBrains Mono', monospace;
        }
        .modal-title {
            font-size: 1.3rem; font-weight: 700;
            letter-spacing: -0.3px; margin-bottom: 6px;
        }
        .modal-course-name {
            color: var(--muted); font-size: 0.875rem; margin-bottom: 24px;
        }

        .modal-price-box {
            background: var(--surface2);
            border: 1px solid var(--border);
            border-radius: 12px; padding: 16px 20px;
            display: flex; justify-content: space-between; align-items: center;
            margin-bottom: 24px;
        }
        .modal-price-label { font-size: 0.82rem; color: var(--muted); }
        .modal-price-amount {
            font-family: 'JetBrains Mono', monospace;
            font-size: 1.4rem; font-weight: 700; color: var(--gold);
        }

        .form-group { margin-bottom: 16px; }
        .form-label {
            display: block; font-size: 0.78rem; font-weight: 600;
            color: var(--muted); margin-bottom: 8px;
            text-transform: uppercase; letter-spacing: 0.8px;
        }
        .form-input {
            width: 100%; background: var(--surface2);
            border: 1px solid var(--border);
            border-radius: 10px; padding: 12px 16px;
            color: var(--text); font-family: 'Sora', sans-serif;
            font-size: 0.875rem; transition: border-color 0.2s;
            outline: none;
        }
        .form-input:focus { border-color: rgba(233,69,96,0.5); }
        .form-input::placeholder { color: var(--muted); }

        .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }

        .card-icons { display: flex; gap: 8px; margin-bottom: 20px; }
        .card-icon {
            background: var(--surface2); border: 1px solid var(--border);
            border-radius: 6px; padding: 4px 10px;
            font-size: 0.75rem; color: var(--muted); font-weight: 600;
        }

        .btn-pay {
            width: 100%;
            background: linear-gradient(135deg, var(--accent), var(--accent2));
            color: white; border: none;
            padding: 14px; border-radius: 10px;
            font-family: 'Sora', sans-serif;
            font-size: 0.95rem; font-weight: 700;
            cursor: pointer; transition: opacity 0.2s;
            letter-spacing: 0.3px;
        }
        .btn-pay:hover { opacity: 0.88; }

        .modal-secure {
            text-align: center; margin-top: 14px;
            font-size: 0.75rem; color: var(--muted);
            display: flex; align-items: center; justify-content: center; gap: 6px;
        }

        @keyframes fadeUp {
            from { opacity: 0; transform: translateY(16px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        @media (max-width: 900px) {
            .container { padding: 24px 20px; }
            .header, .nav { padding: 0 20px; }
            .modal { margin: 20px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- HEADER -->
        <div class="header">
            <div class="logo">
                <img src="~/LEARNSPHERE.png" runat="server" style="height:40px;width:40px;object-fit:contain;" />
                <div class="logo-text">Learn<span>Sphere</span></div>
            </div>
            <div class="header-right">
                <div class="user-pill">
                    <div class="user-avatar">
                        <asp:Label ID="lblAvatarInitial" runat="server" Text="S" />
                    </div>
                    <span class="user-name"><asp:Label ID="lblHeaderName" runat="server" /></span>
                </div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
            </div>
        </div>

        <!-- NAV -->
        <div class="nav">
            <a href="StudentDashboard.aspx"><span>⊞</span> Dashboard</a>
            <a href="BrowseCourses.aspx" class="active"><span>◎</span> Browse Courses</a>
            <a href="MyCourses.aspx"><span>▤</span> My Courses</a>
            <a href="Gamification.aspx"><span>◆</span> Achievements</a>
        </div>

        <div class="container">
            <div class="page-header">
                <div class="page-label">Catalogue</div>
                <div class="page-title">Browse Courses</div>
            </div>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />

            <!-- Hidden field to pass courseid to modal -->
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
                                <%# Convert.ToBoolean(Eval("IsEnrolled")) ? 
                                    "<button class=\"btn-enroll\" disabled>✓ Enrolled</button>" :
                                    Convert.ToDecimal(Eval("price")) == 0 ?
                                        "<asp:Button />" : "" %>
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

        <!-- PAYMENT MODAL -->
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
                        <input type="text" class="form-input" id="cardCvv" placeholder="•••" maxlength="3" />
                    </div>
                </div>

                <asp:Button ID="btnConfirmPayment" runat="server"
                    Text="Pay Now 🔒"
                    CssClass="btn-pay"
                    OnClick="btnConfirmPayment_Click"
                    OnClientClick="return validatePayment()" />

                <div class="modal-secure">🔒 256-bit SSL encrypted · Payments are simulated</div>
            </div>
        </div>

    </form>

    <script>
        // Open modal when "Buy Now" is clicked (triggered from server setting hidden field)
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
            // Clear fields
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
            var name   = document.getElementById('cardName').value.trim();
            var number = document.getElementById('cardNumber').value.replace(/\s/g, '');
            var expiry = document.getElementById('cardExpiry').value.trim();
            var cvv    = document.getElementById('cardCvv').value.trim();

            if (!name) { alert('Please enter cardholder name.'); return false; }
            if (number.length !== 16) { alert('Please enter a valid 16-digit card number.'); return false; }
            if (expiry.length !== 5) { alert('Please enter a valid expiry date (MM/YY).'); return false; }
            if (cvv.length !== 3) { alert('Please enter a valid 3-digit CVV.'); return false; }
            return true;
        }

        // Close modal if overlay clicked
        document.getElementById('paymentModal').addEventListener('click', function(e) {
            if (e.target === this) closeModal();
        });
    </script>
</body>
</html>