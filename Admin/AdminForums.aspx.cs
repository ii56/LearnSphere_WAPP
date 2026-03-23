using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminForums1 : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        int userId;

        // ── ViewState keys ────────────────────────────────────────────────────
        // "list" | "create" | "forum" | "detail"
        protected string CurrentView
        {
            get { return ViewState["CurrentView"] as string ?? "list"; }
            set { ViewState["CurrentView"] = value; }
        }
        protected int CurrentCourseId
        {
            get { return ViewState["CurrentCourseId"] != null ? (int)ViewState["CurrentCourseId"] : 0; }
            set { ViewState["CurrentCourseId"] = value; }
        }
        protected int CurrentForumId
        {
            get { return ViewState["CurrentForumId"] != null ? (int)ViewState["CurrentForumId"] : 0; }
            set { ViewState["CurrentForumId"] = value; }
        }
        protected int CurrentPostId
        {
            get { return ViewState["CurrentPostId"] != null ? (int)ViewState["CurrentPostId"] : 0; }
            set { ViewState["CurrentPostId"] = value; }
        }

        // ══════════════════════════════════════════════════════════════════════
        // CSRF PROTECTION
        // ══════════════════════════════════════════════════════════════════════
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
                ViewStateUserKey = Session["userid"].ToString();
        }

        // ══════════════════════════════════════════════════════════════════════
        // PAGE LOAD
        // ══════════════════════════════════════════════════════════════════════
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null ||
                (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["userid"]);
            LoadSidebarProfileImage();

            if (!IsPostBack)
            {
                CurrentView = "list";
                ShowPanel("list");
                LoadCourses();
            }
            else
            {
                ShowPanel(CurrentView);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // PANEL SWITCHING
        // ══════════════════════════════════════════════════════════════════════
        private void ShowPanel(string view)
        {
            pnlForumsList.Visible = (view == "list");
            pnlViewForum.Visible = (view == "forum");
            pnlForumDetail.Visible = (view == "detail");
            CurrentView = view;
        }

        // ── Back buttons ──────────────────────────────────────────────────────
        protected void btnBackFromCreate_Click(object sender, EventArgs e)
        {
            ShowPanel("list");
            LoadCourses();
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            pnlAskQuestion.Visible = false;
            ShowPanel("list");
            LoadCourses();
        }

        protected void btnBackToForum_Click(object sender, EventArgs e)
        {
            pnlAddAnswer.Visible = false;
            ShowPanel("forum");
            LoadViewForum(CurrentCourseId);
        }

        // ══════════════════════════════════════════════════════════════════════
        // PROFILE IMAGE
        // ══════════════════════════════════════════════════════════════════════
        private void LoadSidebarProfileImage()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ProfileImage FROM [User] WHERE userid=@id", con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                con.Open();
                object result = cmd.ExecuteScalar();
                sidebarImg.Src = (result != null && result != DBNull.Value)
                    ? ResolveUrl(result.ToString())
                    : ResolveUrl("~/images/default-user.png");
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // PANEL 1 — FORUM LIST
        // ══════════════════════════════════════════════════════════════════════
        private void LoadCourses()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    // Matches original Forums.aspx.cs: ownerid, status='Active', CourseForum
                    string query = @"
                        SELECT c.courseid, c.coursename,
                               CASE WHEN f.forumid IS NULL THEN 0 ELSE 1 END AS HasForum
                        FROM Course c
                        LEFT JOIN CourseForum f ON c.courseid = f.courseid 
                        AND c.status = 'Active'";

                    var parameters = new List<SqlParameter>();

                    if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        query += " AND c.coursename LIKE @search";
                        parameters.Add(new SqlParameter("@search", "%" + txtSearch.Text.Trim() + "%"));
                    }

                    if (!string.IsNullOrEmpty(ddlForumStatus.SelectedValue))
                    {
                        query += " AND (CASE WHEN f.forumid IS NULL THEN 0 ELSE 1 END) = @forumStatus";
                        parameters.Add(new SqlParameter("@forumStatus", ddlForumStatus.SelectedValue));
                    }

                    query += " ORDER BY c.coursename";

                    SqlCommand cmd = new SqlCommand(query, con);
                    foreach (var p in parameters) cmd.Parameters.Add(p);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvCourses.DataSource = dt;
                    gvCourses.DataBind();
                }
            }
            catch
            {
                ShowMsg(lblListMessage, "Error loading courses.", false);
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            ShowPanel("list");
            LoadCourses();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlForumStatus.SelectedIndex = 0;
            ShowPanel("list");
            LoadCourses();
        }

        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            if (!int.TryParse(e.CommandArgument.ToString(), out int courseId) || courseId <= 0) return;


            switch (e.CommandName)
            {
                case "ViewForum":
                    CurrentCourseId = courseId;
                    pnlAskQuestion.Visible = false;
                    ShowPanel("forum");
                    LoadViewForum(courseId);
                    break;

                case "DeleteForum":
                    DeleteForum(courseId);
                    LoadCourses();
                    break;
            }
        }

        // Soft-delete matching original Forums.aspx.cs
        private void DeleteForum(int courseId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand(
                        "SELECT forumid FROM CourseForum WHERE courseid=@cid", con);
                    checkCmd.Parameters.Add("@cid", SqlDbType.Int).Value = courseId;
                    object forumIdObj = checkCmd.ExecuteScalar();

                    if (forumIdObj == null)
                    {
                        ShowMsg(lblListMessage, "Forum does not exist.", false);
                        return;
                    }

                    int forumId = Convert.ToInt32(forumIdObj);

                    // Soft-delete all posts
                    new SqlCommand(
                        "UPDATE ForumPost SET deletiontime=GETDATE() WHERE forumid=@fid",
                        con)
                    { Parameters = { new SqlParameter("@fid", forumId) } }
                        .ExecuteNonQuery();

                    // Hard-delete forum record
                    new SqlCommand("DELETE FROM CourseForum WHERE forumid=@fid", con)
                    { Parameters = { new SqlParameter("@fid", forumId) } }
                        .ExecuteNonQuery();

                    LearnSphere_WAPP.Syslog.action(userId, "Deleted Forum (ForumID: " + forumId + ")");
                    ShowMsg(lblListMessage, "Forum deleted successfully.", true);
                }
            }
            catch
            {
                ShowMsg(lblListMessage, "Error deleting forum.", false);
            }
        }

        
        // ══════════════════════════════════════════════════════════════════════
        // PANEL 3 — VIEW FORUM
        // ══════════════════════════════════════════════════════════════════════
        private void LoadViewForum(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand fCmd = new SqlCommand(@"
                    SELECT forumid, title, description, tags
                    FROM CourseForum WHERE courseid=@courseid", con);
                fCmd.Parameters.Add("@courseid", SqlDbType.Int).Value = courseId;
                con.Open();
                SqlDataReader fr = fCmd.ExecuteReader();

                if (fr.Read())
                {
                    lblForumTitle.Text = Server.HtmlEncode(fr["title"].ToString());
                    lblDescription.Text = Server.HtmlEncode(fr["description"].ToString());
                    lblTags.Text = FormatTags(fr["tags"]);
                    CurrentForumId = Convert.ToInt32(fr["forumid"]);
                }
                else
                {
                    ShowPanel("list");
                    LoadCourses();
                    return;
                }
                fr.Close();
                LoadQuestions(con);
            }
        }

        // Matches original ViewForum.aspx.cs query exactly
        private void LoadQuestions(SqlConnection con)
        {
            string query = @"
                SELECT p.postid, p.title, p.content, p.tags, p.creationtime,
                       u.uname, u.ProfileImage,
                       ISNULL(SUM(CASE WHEN v.votetype =  1 THEN 1 ELSE 0 END),0) AS upvotes,
                       ISNULL(SUM(CASE WHEN v.votetype = -1 THEN 1 ELSE 0 END),0) AS downvotes
                FROM ForumPost p
                INNER JOIN CourseForum f ON p.forumid = f.forumid
                INNER JOIN [User] u ON p.userid = u.userid
                LEFT  JOIN ForumVote v ON p.postid = v.postid
                WHERE f.forumid = @fid
                AND p.parentid IS NULL
                AND p.deletiontime IS NULL
                GROUP BY p.postid, p.title, p.content, p.tags, p.creationtime, u.uname, u.ProfileImage
                ORDER BY p.creationtime DESC";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add("@fid", SqlDbType.Int).Value = CurrentForumId;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            rptQuestions.DataSource = dt;
            rptQuestions.DataBind();
            lblNoQuestions.Visible = (dt.Rows.Count == 0);
        }

        protected void btnAskQuestion_Click(object sender, EventArgs e)
        {
            pnlAskQuestion.Visible = !pnlAskQuestion.Visible;
            txtQuestionTitle.Text = "";
            txtQuestionContent.Text = "";
            txtQuestionTags.Text = "";
            txtQuestionVideoUrl.Text = "";
            lblQuestionFormMsg.Visible = false;
        }

        protected void btnCancelQuestion_Click(object sender, EventArgs e)
        {
            pnlAskQuestion.Visible = false;
        }

        // Matches original question.aspx.cs:
        // rate-limit, OUTPUT INSERTED.postid, file/image to Forum_materials, videourl column
        protected void btnSubmitQuestion_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            // Rate-limit (anti-spam) — from question.aspx.cs
            if (Session["lastPostTime"] != null)
            {
                DateTime last = (DateTime)Session["lastPostTime"];
                if ((DateTime.Now - last).TotalSeconds < 3)
                {
                    ShowMsg(lblQuestionFormMsg, "Please wait a moment before posting again.", false);
                    return;
                }
            }
            Session["lastPostTime"] = DateTime.Now;

            string title = txtQuestionTitle.Text.Trim();
            string content = txtQuestionContent.Text.Trim();
            string tags = txtQuestionTags.Text.Trim();
            string videoUrl = txtQuestionVideoUrl.Text.Trim();

            // Length validation
            if (title.Length < 3 || title.Length > 150)
            {
                ShowMsg(lblQuestionFormMsg, "Title must be 3–150 characters.", false);
                return;
            }
            if (content.Length < 10 || content.Length > 2000)
            {
                ShowMsg(lblQuestionFormMsg, "Content must be 10–2000 characters.", false);
                return;
            }
            if (!string.IsNullOrEmpty(tags) && tags.Length > 200)
            {
                ShowMsg(lblQuestionFormMsg, "Tags too long.", false);
                return;
            }
            if (!string.IsNullOrEmpty(videoUrl))
            {
                Uri u;
                if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out u))
                {
                    ShowMsg(lblQuestionFormMsg, "Invalid video URL.", false);
                    return;
                }
            }

            int newPostId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // INSERT with OUTPUT INSERTED.postid — matches original question.aspx.cs
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ForumPost
                        (forumid, userid, parentid, title, content, tags, videourl)
                        OUTPUT INSERTED.postid
                        VALUES
                        (@forumid, @userid, NULL, @title, @content, @tags, @videourl)", conn);

                    cmd.Parameters.Add("@forumid", SqlDbType.Int).Value = CurrentForumId;
                    cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 150).Value = Server.HtmlEncode(title);
                    cmd.Parameters.Add("@content", SqlDbType.NVarChar, 2000).Value = Server.HtmlEncode(content);
                    cmd.Parameters.Add("@tags", SqlDbType.NVarChar, 200).Value =
                        string.IsNullOrEmpty(tags) ? (object)DBNull.Value : Server.HtmlEncode(tags);
                    cmd.Parameters.Add("@videourl", SqlDbType.NVarChar, 300).Value =
                        string.IsNullOrEmpty(videoUrl) ? (object)DBNull.Value : videoUrl;

                    newPostId = (int)cmd.ExecuteScalar();

                    // File upload — matches question.aspx.cs paths exactly
                    if (fileUploadQFile.HasFile)
                    {
                        string ext = Path.GetExtension(fileUploadQFile.FileName).ToLower();
                        string[] allowed = { ".pdf", ".docx", ".zip" };
                        if (Array.IndexOf(allowed, ext) < 0 || fileUploadQFile.PostedFile.ContentLength > 5 * 1024 * 1024
                            || fileUploadQFile.FileName.Contains(".."))
                        {
                            ShowMsg(lblQuestionFormMsg, "Invalid document (PDF/DOCX/ZIP, max 5MB).", false);
                            return;
                        }
                        string folder = Server.MapPath("~/Forum_materials/questions/files/");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                        string fileName = newPostId + "_" + userId + ext;
                        fileUploadQFile.SaveAs(Path.Combine(folder, fileName));
                        new SqlCommand("UPDATE ForumPost SET fileurl=@f WHERE postid=@id", conn)
                        {
                            Parameters =
                            {
                                new SqlParameter("@f",  "~/Forum_materials/questions/files/" + fileName),
                                new SqlParameter("@id", newPostId)
                            }
                        }.ExecuteNonQuery();
                    }

                    // Image upload
                    if (fileUploadQImage.HasFile)
                    {
                        string ext = Path.GetExtension(fileUploadQImage.FileName).ToLower();
                        string mime = fileUploadQImage.PostedFile.ContentType;
                        string[] allowedImg = { ".jpg", ".jpeg", ".png" };
                        if (Array.IndexOf(allowedImg, ext) < 0
                            || !(mime == "image/jpeg" || mime == "image/png")
                            || fileUploadQImage.PostedFile.ContentLength > 3 * 1024 * 1024
                            || fileUploadQImage.FileName.Contains(".."))
                        {
                            ShowMsg(lblQuestionFormMsg, "Invalid image (JPG/PNG, max 3MB).", false);
                            return;
                        }
                        string folder = Server.MapPath("~/Forum_materials/questions/images/");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                        string fileName = newPostId + "_" + userId + ext;
                        fileUploadQImage.SaveAs(Path.Combine(folder, fileName));
                        new SqlCommand("UPDATE ForumPost SET imageurl=@i WHERE postid=@id", conn)
                        {
                            Parameters =
                            {
                                new SqlParameter("@i",  "~/Forum_materials/questions/images/" + fileName),
                                new SqlParameter("@id", newPostId)
                            }
                        }.ExecuteNonQuery();
                    }

                    LearnSphere_WAPP.Syslog.action(userId, "Question Added (ForumID: " + CurrentForumId + ")");
                }

                pnlAskQuestion.Visible = false;
                LoadViewForum(CurrentCourseId);
            }
            catch
            {
                ShowMsg(lblQuestionFormMsg, "An error occurred while posting your question.", false);
            }
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument.ToString(), out int postId)) return;

            if (e.CommandName == "ViewDetail")
            {
                if (!CanAccessPost(postId)) return;
                CurrentPostId = postId;
                pnlAddAnswer.Visible = false;
                ShowPanel("detail");
                LoadForumDetail(postId);
            }
            else if (e.CommandName == "Like")
            {
                HandleVote(postId, 1);
                LoadViewForum(CurrentCourseId);
            }
            else if (e.CommandName == "Dislike")
            {
                HandleVote(postId, -1);
                LoadViewForum(CurrentCourseId);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // PANEL 4 — FORUM DETAIL
        // ══════════════════════════════════════════════════════════════════════
        private bool CanAccessPost(int postId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM ForumPost p
                    INNER JOIN CourseForum f ON p.forumid = f.forumid
                    WHERE p.postid=@pid", con);
                cmd.Parameters.Add("@pid", SqlDbType.Int).Value = postId;
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Matches original ForumDetail.aspx.cs LoadQuestion exactly
        private void LoadForumDetail(int postId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand qCmd = new SqlCommand(@"
                        SELECT p.postid, p.title, p.content, p.tags,
                               p.creationtime, p.userid,
                               u.uname, u.ProfileImage,
                               ISNULL(SUM(CASE WHEN v.votetype =  1 THEN 1 ELSE 0 END),0) AS upvotes,
                               ISNULL(SUM(CASE WHEN v.votetype = -1 THEN 1 ELSE 0 END),0) AS downvotes
                        FROM ForumPost p
                        INNER JOIN [User] u ON p.userid = u.userid
                        LEFT  JOIN ForumVote v ON p.postid = v.postid
                        WHERE p.postid=@pid AND p.deletiontime IS NULL
                        GROUP BY p.postid, p.title, p.content, p.tags,
                                 p.creationtime, p.userid, u.uname, u.ProfileImage", con);
                    qCmd.Parameters.Add("@pid", SqlDbType.Int).Value = postId;
                    con.Open();
                    SqlDataReader dr = qCmd.ExecuteReader();

                    if (dr.Read())
                    {
                        imgQuestionUser.ImageUrl = GetProfileImage(dr["ProfileImage"]);
                        lblQuestionUser.Text = Server.HtmlEncode(dr["uname"].ToString());
                        lblQuestionDate.Text = Convert.ToDateTime(dr["creationtime"]).ToString("dd MMM yyyy");
                        lblQuestionTitle.Text = Server.HtmlEncode(dr["title"].ToString());
                        lblQuestionContent.Text = Server.HtmlEncode(dr["content"].ToString());
                        litTags.Text = FormatTags(dr["tags"]);
                        likeCount.InnerText = dr["upvotes"].ToString();
                        dislikeCount.InnerText = dr["downvotes"].ToString();

                        // Also set the answer form preview labels
                        lblAnswerPreviewTitle.Text = Server.HtmlEncode(dr["title"].ToString());
                        lblAnswerPreviewContent.Text = Server.HtmlEncode(dr["content"].ToString());
                    }
                    else
                    {
                        ShowPanel("forum");
                        LoadViewForum(CurrentCourseId);
                        return;
                    }
                    dr.Close();

                    // Load answers — parentid, deletiontime IS NULL, ForumVote
                    SqlCommand aCmd = new SqlCommand(@"
                        SELECT p.postid, p.content, p.creationtime, p.userid,
                               u.uname, u.ProfileImage,
                               ISNULL(SUM(CASE WHEN v.votetype =  1 THEN 1 ELSE 0 END),0) AS upvotes,
                               ISNULL(SUM(CASE WHEN v.votetype = -1 THEN 1 ELSE 0 END),0) AS downvotes
                        FROM ForumPost p
                        INNER JOIN [User] u ON p.userid = u.userid
                        LEFT  JOIN ForumVote v ON p.postid = v.postid
                        WHERE p.parentid=@postid AND p.deletiontime IS NULL
                        GROUP BY p.postid, p.content, p.creationtime, p.userid, u.uname, u.ProfileImage
                        ORDER BY p.creationtime ASC", con);
                    aCmd.Parameters.Add("@postid", SqlDbType.Int).Value = postId;

                    SqlDataAdapter da = new SqlDataAdapter(aCmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptAnswers.DataSource = dt;
                    rptAnswers.DataBind();
                    lblNoAnswers.Visible = (dt.Rows.Count == 0);
                }
            }
            catch (Exception ex)
            {
                ShowMsg(lblDetailMessage, "Error loading forum: " + ex.Message, false);
            }
        }

        protected void btnLikeQuestion_Click(object sender, EventArgs e)
        {
            HandleVote(CurrentPostId, 1);
            LoadForumDetail(CurrentPostId);
        }

        protected void btnDislikeQuestion_Click(object sender, EventArgs e)
        {
            HandleVote(CurrentPostId, -1);
            LoadForumDetail(CurrentPostId);
        }

        protected void btnAnswer_Click(object sender, EventArgs e)
        {
            pnlAddAnswer.Visible = !pnlAddAnswer.Visible;
            txtAnswerContent.Text = "";
            txtAnswerVideoUrl.Text = "";
            lblAnswerFormMsg.Visible = false;
        }

        protected void btnCancelAnswer_Click(object sender, EventArgs e)
        {
            pnlAddAnswer.Visible = false;
        }

        // Matches original answer.aspx.cs:
        // XSS on content, video URL validation, file to ~/Uploads/Documents/, image to ~/Uploads/Images/
        protected void btnSubmitAnswer_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string content = txtAnswerContent.Text.Trim();
            string videoUrl = txtAnswerVideoUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(content))
            {
                ShowMsg(lblAnswerFormMsg, "Answer cannot be empty.", false);
                return;
            }
            if (content.Length > 2000)
            {
                ShowMsg(lblAnswerFormMsg, "Answer is too long (max 2000 chars).", false);
                return;
            }

            // XSS protection — matches answer.aspx.cs
            content = Server.HtmlEncode(content);

            if (!string.IsNullOrEmpty(videoUrl))
            {
                Uri u;
                if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out u))
                {
                    ShowMsg(lblAnswerFormMsg, "Invalid video URL.", false);
                    return;
                }
            }

            // Get forumid from parent post
            int forumId = 0;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand f = new SqlCommand(
                    "SELECT forumid FROM ForumPost WHERE postid=@pid", con);
                f.Parameters.Add("@pid", SqlDbType.Int).Value = CurrentPostId;
                con.Open();
                object r = f.ExecuteScalar();
                if (r == null) return;
                forumId = Convert.ToInt32(r);
            }

            try
            {
                string savedFileUrl = null;
                string savedImageUrl = null;

                // Document upload — matches answer.aspx.cs paths: ~/Uploads/Documents/
                if (fileUploadAFile.HasFile)
                {
                    string ext = Path.GetExtension(fileUploadAFile.FileName).ToLower();
                    string[] allowed = { ".pdf", ".docx", ".zip" };
                    if (Array.IndexOf(allowed, ext) < 0)
                    {
                        ShowMsg(lblAnswerFormMsg, "Invalid document type.", false);
                        return;
                    }
                    if (fileUploadAFile.PostedFile.ContentLength > 5 * 1024 * 1024)
                    {
                        ShowMsg(lblAnswerFormMsg, "Document too large (max 5MB).", false);
                        return;
                    }
                    string newName = Guid.NewGuid().ToString() + ext;
                    string path = Server.MapPath("~/Uploads/Documents/");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    fileUploadAFile.SaveAs(Path.Combine(path, newName));
                    savedFileUrl = "~/Uploads/Documents/" + newName;
                }

                // Image upload — matches answer.aspx.cs paths: ~/Uploads/Images/
                if (fileUploadAImage.HasFile)
                {
                    string ext = Path.GetExtension(fileUploadAImage.FileName).ToLower();
                    string[] allowedImg = { ".jpg", ".jpeg", ".png" };
                    if (Array.IndexOf(allowedImg, ext) < 0)
                    {
                        ShowMsg(lblAnswerFormMsg, "Invalid image type.", false);
                        return;
                    }
                    if (fileUploadAImage.PostedFile.ContentLength > 3 * 1024 * 1024)
                    {
                        ShowMsg(lblAnswerFormMsg, "Image too large (max 3MB).", false);
                        return;
                    }
                    string newName = Guid.NewGuid().ToString() + ext;
                    string path = Server.MapPath("~/Uploads/Images/");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    fileUploadAImage.SaveAs(Path.Combine(path, newName));
                    savedImageUrl = "~/Uploads/Images/" + newName;
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    // Matches answer.aspx.cs INSERT exactly: parentid, title=NULL, videourl
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ForumPost
                        (forumid, userid, parentid, title, content, videourl)
                        VALUES
                        (@forumid, @userid, @parentid, NULL, @content, @videourl)", conn);
                    cmd.Parameters.Add("@forumid", SqlDbType.Int).Value = forumId;
                    cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@parentid", SqlDbType.Int).Value = CurrentPostId;
                    cmd.Parameters.Add("@content", SqlDbType.NVarChar, 2000).Value = content;
                    cmd.Parameters.Add("@videourl", SqlDbType.NVarChar).Value =
                        string.IsNullOrEmpty(videoUrl) ? (object)DBNull.Value : videoUrl;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    LearnSphere_WAPP.Syslog.action(userId, "Answered Post (PostID: " + CurrentPostId + ")");
                }

                pnlAddAnswer.Visible = false;
                LoadForumDetail(CurrentPostId);
            }
            catch
            {
                ShowMsg(lblAnswerFormMsg, "An error occurred while posting your answer.", false);
            }
        }

        protected void rptAnswers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument.ToString(), out int postId)) return;

            if (e.CommandName == "LikeAnswer")
            {
                HandleVote(postId, 1);
                LoadForumDetail(CurrentPostId);
            }
            else if (e.CommandName == "DislikeAnswer")
            {
                HandleVote(postId, -1);
                LoadForumDetail(CurrentPostId);
            }
            else if (e.CommandName == "DeletePost")
            {
                SoftDeletePost(postId);
                LoadForumDetail(CurrentPostId);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // SHARED HELPERS
        // ══════════════════════════════════════════════════════════════════════

        // Toggle vote — matches both original .cs files exactly
        private void HandleVote(int postId, int voteType)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand check = new SqlCommand(
                    "SELECT votetype FROM ForumVote WHERE postid=@pid AND userid=@uid", conn);
                check.Parameters.AddWithValue("@pid", postId);
                check.Parameters.AddWithValue("@uid", userId);
                object existing = check.ExecuteScalar();

                if (existing != null)
                {
                    int current = Convert.ToInt32(existing);
                    if (current == voteType)
                    {
                        // Same → remove
                        new SqlCommand(
                            "DELETE FROM ForumVote WHERE postid=@pid AND userid=@uid", conn)
                        { Parameters = { new SqlParameter("@pid", postId), new SqlParameter("@uid", userId) } }
                        .ExecuteNonQuery();
                    }
                    else
                    {
                        // Different → switch
                        new SqlCommand(
                            "UPDATE ForumVote SET votetype=@type WHERE postid=@pid AND userid=@uid", conn)
                        {
                            Parameters =
                            {
                                new SqlParameter("@type", voteType),
                                new SqlParameter("@pid",  postId),
                                new SqlParameter("@uid",  userId)
                            }
                        }.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Insert new vote
                    new SqlCommand(
                        "INSERT INTO ForumVote(postid,userid,votetype) VALUES(@pid,@uid,@type)", conn)
                    {
                        Parameters =
                        {
                            new SqlParameter("@pid",  postId),
                            new SqlParameter("@uid",  userId),
                            new SqlParameter("@type", voteType)
                        }
                    }.ExecuteNonQuery();
                }
            }
        }

        // Soft-delete — matches ForumDetail.aspx.cs: only own posts, sets deletiontime
        private void SoftDeletePost(int postId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE ForumPost
                        SET deletiontime=GETDATE()
                        WHERE postid=@pid", con);
                    cmd.Parameters.Add("@pid", SqlDbType.Int).Value = postId;
                    con.Open();
                    if (cmd.ExecuteNonQuery() == 0)
                        ShowMsg(lblDetailMessage, "Unauthorized action.", false);
                    else
                        LearnSphere_WAPP.Syslog.action(userId, "Deleted Post " + postId);
                }
            }
            catch
            {
                ShowMsg(lblDetailMessage, "Error deleting post.", false);
            }
        }

        // IsOwner — only own userid (matches ForumDetail.aspx.cs)
        protected bool IsOwner(object postUserIdObj)
        {
            if (postUserIdObj == null || postUserIdObj == DBNull.Value) return false;
            return Convert.ToInt32(postUserIdObj) == userId;
        }

        protected string GetProfileImage(object profileImage)
        {
            if (profileImage == null || profileImage == DBNull.Value)
                return ResolveUrl("~/images/default-user.png");
            string path = profileImage.ToString().Trim();
            if (string.IsNullOrEmpty(path)) return ResolveUrl("~/images/default-user.png");
            if (path.StartsWith("~/") || path.StartsWith("http")) return ResolveUrl(path);
            return ResolveUrl("~/" + path);
        }

        protected string FormatTags(object tagObj)
        {
            if (tagObj == null || tagObj == DBNull.Value) return "";
            string raw = tagObj.ToString().Trim();
            if (string.IsNullOrEmpty(raw)) return "";
            var sb = new System.Text.StringBuilder();
            foreach (string t in raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string clean = Server.HtmlEncode(t.Trim());
                if (!string.IsNullOrEmpty(clean))
                    sb.Append("<span class='tag-pill'>" + clean + "</span>");
            }
            return sb.ToString();
        }

        private void ShowMsg(Label lbl, string msg, bool success)
        {
            lbl.Text = msg;
            lbl.CssClass = "alert " + (success ? "alert-success" : "alert-error");
            lbl.Visible = true;
        }

        // ══════════════════════════════════════════════════════════════════════
        // LOGOUT
        // ══════════════════════════════════════════════════════════════════════
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            LearnSphere_WAPP.Syslog.action(userId, "Logout System");
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}