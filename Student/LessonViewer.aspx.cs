using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;

namespace LearnSphere_WAPP.Student
{
    public partial class LessonViewer : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null) { Response.Redirect("~/Login.aspx"); return; }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string displayName = "Student";
                if (Session["fname"] != null && Session["fname"].ToString() != "")
                {
                    displayName = Session["fname"].ToString();
                }
                else
                {
                    using (SqlCommand nameCmd = new SqlCommand("SELECT fname FROM [User] WHERE userid = @uid", con))
                    {
                        nameCmd.Parameters.AddWithValue("@uid", Convert.ToInt32(Session["userid"]));
                        object result = nameCmd.ExecuteScalar();
                        if (result != null) { displayName = result.ToString(); Session["fname"] = displayName; }
                    }
                }
                lblHeaderName.Text = displayName;
                lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();
            }

            if (!IsPostBack)
            {
                LoadCourseStructure();
                if (Request.QueryString["lessonId"] != null)
                    LoadLesson(Convert.ToInt32(Request.QueryString["lessonId"]));
            }
        }

        private void LoadCourseStructure()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            int courseId = 0;

            if (Request.QueryString["courseid"] != null)
                courseId = Convert.ToInt32(Request.QueryString["courseid"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // get the course name
                string courseQuery = courseId > 0
                    ? "SELECT courseid, coursename FROM Course WHERE courseid = @cid"
                    : "SELECT TOP 1 c.courseid, c.coursename FROM Enrollment e INNER JOIN Course c ON e.courseid = c.courseid WHERE e.userid = @uid AND e.isactive = 1";

                using (SqlCommand cmd = new SqlCommand(courseQuery, con))
                {
                    if (courseId > 0) cmd.Parameters.AddWithValue("@cid", courseId);
                    else cmd.Parameters.AddWithValue("@uid", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            courseId = Convert.ToInt32(reader["courseid"]);
                            lblCourseName.Text = reader["coursename"].ToString();
                        }
                        else
                        {
                            Response.Redirect("MyCourses.aspx");
                            return;
                        }
                    }
                }

                // load modules and lessons for the sidebar
                try
                {
                    string query = @"
                        SELECT m.moduleid, m.modulename,
                               l.lessonid, l.lessontitle,
                               CASE WHEN lp.progressid IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
                        FROM Module m
                        LEFT JOIN Lesson l ON m.moduleid = l.moduleid
                        LEFT JOIN LessonProgress lp ON l.lessonid = lp.lessonid AND lp.userid = @uid
                        WHERE m.courseid = @courseid
                        ORDER BY m.ordernumber, l.ordernumber";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                        da.SelectCommand.Parameters.AddWithValue("@courseid", courseId);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var modules = dt.AsEnumerable()
                                .GroupBy(row => new {
                                    ModuleId = row.Field<int>("moduleid"),
                                    ModuleName = row.Field<string>("modulename")
                                })
                                .Select(g => new {
                                    ModuleId = g.Key.ModuleId,
                                    ModuleName = g.Key.ModuleName,
                                    Lessons = g.Where(r => !r.IsNull("lessonid"))
                                        .Select(row => new {
                                            LessonId = row.Field<int>("lessonid"),
                                            LessonTitle = row.Field<string>("lessontitle"),
                                            IsCompleted = row.Field<int>("IsCompleted") == 1
                                        }).ToList()
                                }).ToList();

                            rptModules.DataSource = modules;
                            rptModules.DataBind();
                            pnlNoModules.Visible = false;
                            pnlModules.Visible = true;
                        }
                        else
                        {
                            pnlModules.Visible = false;
                            pnlNoModules.Visible = true;
                        }
                    }
                }
                catch
                {
                    pnlModules.Visible = false;
                    pnlNoModules.Visible = true;
                }
            }
        }

        private void LoadLesson(int lessonId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // load the lesson title and description
                    using (SqlCommand cmd = new SqlCommand("SELECT lessontitle, lessondescription FROM Lesson WHERE lessonid = @lid", con))
                    {
                        cmd.Parameters.AddWithValue("@lid", lessonId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblLessonTitle.Text = reader["lessontitle"].ToString();
                                lblLessonDesc.Text = reader["lessondescription"] != null ? reader["lessondescription"].ToString() : "";
                            }
                            else return;
                        }
                    }

                    // load materials (videos and files)
                    try
                    {
                        using (SqlCommand matCmd = new SqlCommand("SELECT materialid, filetype, fileurl, videourl FROM Material WHERE lessonid = @lid", con))
                        {
                            matCmd.Parameters.AddWithValue("@lid", lessonId);
                            using (SqlDataReader matReader = matCmd.ExecuteReader())
                            {
                                DataTable matTable = new DataTable();
                                matTable.Load(matReader);

                                // check if theres a video
                                var videoRow = matTable.AsEnumerable()
                                    .FirstOrDefault(r => r["filetype"].ToString().ToLower() == "video"
                                        && !string.IsNullOrEmpty(r["videourl"]?.ToString()));
                                if (videoRow != null)
                                {
                                    iframeVideo.Attributes["src"] = videoRow["videourl"].ToString();
                                    pnlVideo.Visible = true;
                                    pnlNoVideo.Visible = false;
                                }
                                else
                                {
                                    pnlVideo.Visible = false;
                                    pnlNoVideo.Visible = true;
                                }

                                // check for downloadable files
                                var fileRows = matTable.AsEnumerable()
                                    .Where(r => r["filetype"].ToString().ToLower() != "video"
                                        && !string.IsNullOrEmpty(r["fileurl"]?.ToString()))
                                    .ToList();

                                if (fileRows.Count > 0)
                                {
                                    rptMaterials.DataSource = fileRows.CopyToDataTable();
                                    rptMaterials.DataBind();
                                    pnlFiles.Visible = true;
                                }
                                else { pnlFiles.Visible = false; }
                            }
                        }
                    }
                    catch { pnlVideo.Visible = false; pnlNoVideo.Visible = true; pnlFiles.Visible = false; }

                    // check if this lesson is already completed
                    int userId = Convert.ToInt32(Session["userid"]);
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM LessonProgress WHERE userid=@uid AND lessonid=@lid AND iscompleted=1", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@lid", lessonId);
                        bool done = (int)chk.ExecuteScalar() > 0;
                        btnComplete.Enabled = !done;
                        btnComplete.Text = done ? "Already Completed" : "Mark as Completed (+10 Points)";
                    }

                    pnlLesson.Visible = true;
                    pnlSelectLesson.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading lesson: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["lessonId"] == null) return;
            int lessonId = Convert.ToInt32(Request.QueryString["lessonId"]);
            int userId = Convert.ToInt32(Session["userid"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // make sure they havent already completed this
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM LessonProgress WHERE userid=@uid AND lessonid=@lid", con))
                    {
                        chk.Parameters.AddWithValue("@uid", userId);
                        chk.Parameters.AddWithValue("@lid", lessonId);
                        if ((int)chk.ExecuteScalar() > 0) return;
                    }

                    // insert progress record
                    using (SqlCommand ins = new SqlCommand("INSERT INTO LessonProgress (userid,lessonid,iscompleted,completedtime) VALUES(@uid,@lid,1,@t)", con))
                    {
                        ins.Parameters.AddWithValue("@uid", userId);
                        ins.Parameters.AddWithValue("@lid", lessonId);
                        ins.Parameters.AddWithValue("@t", DateTime.Now);
                        ins.ExecuteNonQuery();
                    }

                    // add 10 points
                    using (SqlCommand pts = new SqlCommand("UPDATE StudentPoints SET totalpoints=totalpoints+10, lastupdated=@t WHERE userid=@uid", con))
                    {
                        pts.Parameters.AddWithValue("@uid", userId);
                        pts.Parameters.AddWithValue("@t", DateTime.Now);
                        pts.ExecuteNonQuery();
                    }

                    // update badge based on new points
                    using (SqlCommand bdg = new SqlCommand(@"
                        UPDATE StudentPoints SET badge =
                            CASE WHEN totalpoints >= 600 THEN 'Diamond'
                                 WHEN totalpoints >= 300 THEN 'Gold'
                                 WHEN totalpoints >= 100 THEN 'Silver'
                                 ELSE 'Bronze' END
                        WHERE userid = @uid", con))
                    {
                        bdg.Parameters.AddWithValue("@uid", userId);
                        bdg.ExecuteNonQuery();
                    }

                    lblMessage.Text = "Lesson completed! You earned 10 points!";
                    lblMessage.Visible = true;
                    btnComplete.Enabled = false;
                    btnComplete.Text = "Already Completed";
                    LoadCourseStructure();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
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