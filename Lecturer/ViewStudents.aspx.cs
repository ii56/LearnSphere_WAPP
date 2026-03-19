using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Lecturer
{
    public partial class ViewStudents : System.Web.UI.Page
    {
        private string connStr;

        protected void Page_Load(object sender, EventArgs e)
        {
            connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"]?.ConnectionString;

            if (Session["usertype"] == null || Session["usertype"].ToString() != "Lecturer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // THEN normal page load
            if (!IsPostBack)
            {
                if (Request.QueryString["courseId"] != null)
                {
                    int courseId = Convert.ToInt32(Request.QueryString["courseId"]);
                    LoadSidebarProfileImage();
                    LoadStudents(courseId);
                }
            }
        }

        private void LoadSidebarProfileImage()
        {
            if (Session["userid"] == null)
                return;

            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString))
            {
                string query = "SELECT ProfileImage FROM [User] WHERE userid = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", userId);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string imagePath = result.ToString();
                    Session["profileImage"] = imagePath;
                    imgSidebarProfile.Src = ResolveUrl(imagePath);
                }
                else
                {
                    imgSidebarProfile.Src = ResolveUrl("~/images/default-user.png");
                }
            }
        }

        private void LoadStudents(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
SELECT DISTINCT
    u.userid,
    u.uname,
    u.fname,
    u.lname,
    u.email,
    u.age,
    u.gender,
    e.enrolldate AS EnrolledOn
FROM Enrollment e
INNER JOIN Course c  ON e.courseid = c.courseid
INNER JOIN [User] u  ON e.userid   = u.userid
WHERE e.courseid  = @courseId
  AND e.isactive  = 1
  AND u.usertype  = 'Student'
  AND u.deletiontime IS NULL
  AND u.status    = 'Active'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@courseId", courseId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvStudents.DataSource = dt;
                    gvStudents.DataBind();
                }
                else
                {
                    lblMessage.Text = "No enrolled students found.";
                }
            }
        }

        protected void gvStudents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteStudent" || e.CommandName == "ViewReceipt")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(gvStudents.DataKeys[index].Value);
                int courseId = Convert.ToInt32(Request.QueryString["courseId"]);

                if (e.CommandName == "DeleteStudent")
                {
                    RemoveEnrollment(userId, courseId);
                    LoadStudents(courseId);
                }
                else if (e.CommandName == "ViewReceipt")
                {
                    GenerateReceipt(userId, courseId);
                }
            }
        }

        private void RemoveEnrollment(int userId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Remove from Enrollment table
                SqlCommand deleteEnrollment = new SqlCommand(
                    "DELETE FROM Enrollment WHERE userid=@uid AND courseid=@cid", con);
                deleteEnrollment.Parameters.AddWithValue("@uid", userId);
                deleteEnrollment.Parameters.AddWithValue("@cid", courseId);
                deleteEnrollment.ExecuteNonQuery();

                // Also clean up Invoice/Receipt if they exist
                string getInvoiceQuery = "SELECT invid FROM Invoice WHERE userid=@uid AND courseid=@cid";
                SqlCommand getCmd = new SqlCommand(getInvoiceQuery, con);
                getCmd.Parameters.AddWithValue("@uid", userId);
                getCmd.Parameters.AddWithValue("@cid", courseId);

                object invoiceIdObj = getCmd.ExecuteScalar();
                if (invoiceIdObj != null)
                {
                    int invoiceId = Convert.ToInt32(invoiceIdObj);

                    new SqlCommand("DELETE FROM Receipt WHERE invid=@invid", con)
                    { Parameters = { new SqlParameter("@invid", invoiceId) } }
                        .ExecuteNonQuery();

                    new SqlCommand("DELETE FROM Invoice WHERE invid=@invid", con)
                    { Parameters = { new SqlParameter("@invid", invoiceId) } }
                        .ExecuteNonQuery();
                }
            }

            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Studen removed from course (CourseID: " + courseId + ", UserID: " + userId + ")");
            lblMessage.Text = "Student removed from course.";
        }

        private void GenerateReceipt(int userId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = @"
SELECT 
    u.fname + ' ' + u.lname AS StudentName,
    c.coursename,
    ISNULL(i.amount, 0) AS amount,
    ISNULL(i.creationtime, e.enrolldate) AS creationtime
FROM Enrollment e
INNER JOIN [User] u ON e.userid = u.userid
INNER JOIN Course c ON e.courseid = c.courseid
LEFT JOIN Invoice i ON e.userid = i.userid AND e.courseid = i.courseid
WHERE e.userid = @uid AND e.courseid = @cid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@cid", courseId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string studentName = reader["StudentName"].ToString();
                    string courseName = reader["coursename"].ToString();
                    decimal amount = Convert.ToDecimal(reader["amount"]);
                    DateTime date = Convert.ToDateTime(reader["creationtime"]);

                    GeneratePdf(studentName, courseName, amount, date);
                }
                else
                {
                    lblMessage.Text = "No receipt data found for user: " + userId + " course: " + courseId;
                    return;
                }
            }
        }

        private void GeneratePdf(string studentName, string courseName, decimal amount, DateTime date)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=Receipt.pdf");

            Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter.GetInstance(doc, Response.OutputStream);

            doc.Open();

            // 🔹 TITLE
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            Paragraph title = new Paragraph("LearnSphere Receipt", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            doc.Add(new Paragraph(" ")); // spacing

            // 🔹 SUBTITLE
            Font subFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Paragraph subtitle = new Paragraph("Official Payment Receipt", subFont);
            subtitle.Alignment = Element.ALIGN_CENTER;
            doc.Add(subtitle);

            doc.Add(new Paragraph("\n"));

            // 🔹 TABLE (MAIN CONTENT)
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 30f, 70f });

            Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            // Helper function
            void AddRow(string label, string value)
            {
                PdfPCell cell1 = new PdfPCell(new Phrase(label, labelFont));
                cell1.Border = Rectangle.NO_BORDER;
                cell1.Padding = 8;

                PdfPCell cell2 = new PdfPCell(new Phrase(value, valueFont));
                cell2.Border = Rectangle.NO_BORDER;
                cell2.Padding = 8;

                table.AddCell(cell1);
                table.AddCell(cell2);
            }

            AddRow("Student Name:", studentName);
            AddRow("Course Name:", courseName);
            AddRow("Amount Paid:", "RM " + amount.ToString("N2"));
            AddRow("Payment Date:", date.ToString("dd MMM yyyy"));

            doc.Add(table);

            doc.Add(new Paragraph("\n"));

            // 🔹 DIVIDER LINE
            PdfPTable line = new PdfPTable(1);
            line.WidthPercentage = 100;
            PdfPCell lineCell = new PdfPCell(new Phrase(""));
            lineCell.BorderWidthBottom = 1f;
            lineCell.Border = Rectangle.BOTTOM_BORDER;
            line.AddCell(lineCell);
            doc.Add(line);

            doc.Add(new Paragraph("\n"));

            // 🔹 FOOTER
            Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10);
            Paragraph footer = new Paragraph("Thank you for your payment.\nThis is a system-generated receipt.", footerFont);
            footer.Alignment = Element.ALIGN_CENTER;

            doc.Add(footer);

            doc.Close();

            Response.Flush();
            Response.End();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["courseId"] == null)
                return;

            int courseId = Convert.ToInt32(Request.QueryString["courseId"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string courseNameQuery = "SELECT coursename FROM Course WHERE courseid = @cid";
                SqlCommand courseCmd = new SqlCommand(courseNameQuery, con);
                courseCmd.Parameters.AddWithValue("@cid", courseId);

                string courseName = courseCmd.ExecuteScalar()?.ToString() ?? "Course";

                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    courseName = courseName.Replace(c, '_');
                }

                courseName = courseName.Replace(" ", "_");

                string query = @"
    SELECT DISTINCT
        u.userid       AS [User ID],
        u.uname        AS [Username],
        u.fname        AS [First Name],
        u.lname        AS [Last Name],
        u.email        AS [Email],
        u.age          AS [Age],
        u.gender       AS [Gender],
        e.enrolldate   AS [Enrolled On]
    FROM Enrollment e
    INNER JOIN Course c  ON e.courseid = c.courseid
    INNER JOIN [User] u  ON e.userid   = u.userid
    LEFT  JOIN Invoice i ON e.userid   = i.userid AND e.courseid = i.courseid
    LEFT  JOIN Receipt r ON i.invid    = r.invid
    WHERE e.courseid  = @courseId
      AND e.isactive  = 1
      AND u.usertype  = 'Student'
      AND u.deletiontime IS NULL
      AND u.status    = 'Active'
      AND (
            c.price = 0
            OR (c.price > 0 AND r.invid IS NOT NULL)
          )";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@courseId", courseId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                string fileName = $"{courseName}_EnrolledStudents_{DateTime.Now:ddMMMyyyy}.xls";

                ExportToExcel(dt, fileName);
                LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Export student information csv file");
            }
        }

        private void ExportToExcel(DataTable dt, string fileName)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    GridView gv = new GridView();
                    gv.DataSource = dt;
                    gv.DataBind();
                    gv.RenderControl(hw);

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewCourses.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("~/Login.aspx");
        }
    }
}