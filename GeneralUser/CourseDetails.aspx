<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseDetails.aspx.cs" Inherits="LearnSphere_WAPP.GeneralUser.CourseDetails" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Details - LearnSphere</title>
    <link href="CourseDetails.css" rel="stylesheet" />
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
                            <img id="imgSidebarProfile" runat="server" class="profile-img" src="~/images/default-user.png" />
                        </div>
                        <div class="profile-info">
                            <div class="profile-name"><%= Session["uname"] != null ? Session["uname"].ToString() : "Guest" %></div>
                            <div class="profile-status">General User</div>
                        </div>
                    </div>
                    <a href="Message.aspx" class="nav-item message-link">
                        Messages <asp:Literal ID="litUnreadBadge" runat="server" />
                    </a>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>

            <div class="main-content">
                
                <div class="back-container">
                    <a href="ViewCourses.aspx" class="btn-back">← Back to Courses</a>
                </div>

                <div class="detail-card">
                    <div class="detail-header-layout">
                        <div class="header-text-area">
                            <asp:Label ID="lblCategory" runat="server" CssClass="detail-category"></asp:Label>
                            <h2 class="detail-title"><asp:Label ID="lblCourseName" runat="server"></asp:Label></h2>
                            
                            <div class="instructor-block">
                                <img src="../images/default-user.png" class="instructor-avatar" />
                                <div class="instructor-info">
                                    <span class="instructor-label">Instructor</span>
                                    <asp:Label ID="lblInstructorName" runat="server" CssClass="instructor-name"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="header-action-area">
                            <div class="price-box">
                                <asp:Label ID="lblPrice" runat="server" CssClass="detail-price"></asp:Label>
                            </div>
                            
                            <asp:Button ID="btnEnroll" runat="server" CssClass="btn-enroll-large" OnClick="btnCourseAction_Click" />
                            
                            <div style="margin-top: 15px;">
                                <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <hr class="divider" />

                    <div class="detail-body">
                        <h3>About This Course</h3>
                        <div class="description-text">
                            <asp:Literal ID="litDescription" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <asp:HiddenField ID="hfCourseData" runat="server" />

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

        <script>
            function openModalWithData(name, price) {
                document.getElementById('modalCourseName').innerText = name;
                document.getElementById('modalPrice').innerText = price === "0" || price === "0.00" ? 'FREE' : 'RM ' + parseFloat(price).toFixed(2);
                document.getElementById('paymentModal').classList.add('open');
            }
            function closeModal() {
                document.getElementById('paymentModal').classList.remove('open');
                document.getElementById('cardName').value = '';
                document.getElementById('cardNumber').value = '';
                document.getElementById('cardExpiry').value = '';
                document.getElementById('cardCvv').value = '';
                document.getElementById('<%= hfCourseData.ClientID %>').value = '';
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

                // Bypass validation if the course is free
                if (document.getElementById('modalPrice').innerText === 'FREE') return true;

                if (!name) { alert('Please enter cardholder name.'); return false; }
                if (number.length !== 16) { alert('Please enter a valid 16-digit card number.'); return false; }
                if (expiry.length !== 5) { alert('Please enter a valid expiry date (MM/YY).'); return false; }
                if (cvv.length !== 3) { alert('Please enter a valid 3-digit CVV.'); return false; }
                return true;
            }

            // Close modal if user clicks outside of it
            document.getElementById('paymentModal').addEventListener('click', function (e) {
                if (e.target === this) closeModal();
            });
        </script>
        
        <script src="https://cdn.botpress.cloud/webchat/v3.6/inject.js"></script>
        <script src="https://files.bpcontent.cloud/2026/02/25/04/20260225040020-WUKR78B4.js" defer></script>
    </form>
</body>
</html>