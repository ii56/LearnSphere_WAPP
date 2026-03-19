using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.UI;
using Org.BouncyCastle.Asn1.X509;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace LearnSphere_WAPP.Student
{
    public partial class StudentDashboard : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
                LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            int userId = Convert.ToInt32(Session["userid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // get the student's first name to display
                string displayName = "Student";
                if (Session["fname"] != null && Session["fname"].ToString() != "")
                {
                    displayName = Session["fname"].ToString();
                }
                else
                {
                    using (SqlCommand nameCmd = new SqlCommand("SELECT fname FROM [User] WHERE userid = @uid", con))
                    {
                        nameCmd.Parameters.AddWithValue("@uid", userId);
                        object result = nameCmd.ExecuteScalar();
                        if (result != null)
                        {
                            displayName = result.ToString();
                            Session["fname"] = displayName;
                        }
                    }
                }

                lblWelcome.Text = displayName;
                lblHeaderName.Text = displayName;
                lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();

                // enrolled courses count
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Enrollment WHERE userid = @uid AND isactive = 1", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    lblEnrolled.Text = cmd.ExecuteScalar().ToString();
                }

                // completed lessons count
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM LessonProgress WHERE userid = @uid AND iscompleted = 1", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        lblCompleted.Text = cmd.ExecuteScalar().ToString();
                    }
                }
                catch { lblCompleted.Text = "0"; }

                // points and badge
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT totalpoints, badge FROM StudentPoints WHERE userid = @uid", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblPoints.Text = reader["totalpoints"].ToString();
                                lblBadge.Text = reader["badge"] != null ? reader["badge"].ToString() : "Bronze";
                            }
                            else
                            {
                                lblPoints.Text = "0";
                                lblBadge.Text = "Bronze";
                            }
                        }
                    }

                    // if student doesnt have a points row yet, create one
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM StudentPoints WHERE userid = @uid", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        if ((int)chk.ExecuteScalar() == 0)
                        {
                            using (SqlCommand ins = new SqlCommand(
                                "INSERT INTO StudentPoints (userid, totalpoints, badge, lastupdated) VALUES (@uid, 0, 'Bronze', @now)", con))
                            {
                                ins.Parameters.AddWithValue("@uid", userId);
                                ins.Parameters.AddWithValue("@now", DateTime.Now);
                                ins.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch
                {
                    lblPoints.Text = "0";
                    lblBadge.Text = "Bronze";
                }

                // load enrolled courses for the table
                string query = @"SELECT c.courseid, c.coursename, c.category, e.enrolldate
                FROM Course c
                INNER JOIN Enrollment e ON c.courseid = e.courseid
                WHERE e.userid = @uid AND e.isactive = 1 AND c.status = 'Active'
                ORDER BY e.enrolldate DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        gvCourses.DataSource = dt;
                        gvCourses.DataBind();
                        lblCourseCount.Text = dt.Rows.Count + " course" + (dt.Rows.Count != 1 ? "s" : "");
                        pnlEmpty.Visible = false;
                        pnlCourses.Visible = true;
                    }
                    else
                    {
                        pnlCourses.Visible = false;
                        pnlEmpty.Visible = true;
                        lblCourseCount.Text = "0 courses";
                    }
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}