using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.GeneralUser
{
    public partial class Forums : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

        // ViewState properties to track where the user is
        protected string CurrentView
        {
            get { return ViewState["CurrentView"] as string ?? "forumList"; }
            set { ViewState["CurrentView"] = value; }
        }
        protected int CurrentForumId
        {
            get { return ViewState["CurrentForumId"] != null ? (int)ViewState["CurrentForumId"] : 0; }
            set { ViewState["CurrentForumId"] = value; }
        }
        protected int CurrentQuestionId
        {
            get { return ViewState["CurrentQuestionId"] != null ? (int)ViewState["CurrentQuestionId"] : 0; }
            set { ViewState["CurrentQuestionId"] = value; }
        }
        protected int EditPostId
        {
            get { return ViewState["EditPostId"] != null ? (int)ViewState["EditPostId"] : 0; }
            set { ViewState["EditPostId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null || Session["usertype"] == null || Session["usertype"].ToString() != "General")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSidebarProfile();
                ShowView("forumList");
                LoadForums("");
            }
        }

        private void LoadSidebarProfile()
        {
            int userId = Convert.ToInt32(Session["userid"]);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string displayName = "User";
                using (SqlCommand cmd = new SqlCommand("SELECT fname, ProfileImage FROM [User] WHERE userid = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            displayName = reader["fname"].ToString();
                            string imgPath = reader["ProfileImage"] != DBNull.Value ? reader["ProfileImage"].ToString() : "";

                            if (!string.IsNullOrEmpty(imgPath))
                            {
                                imgHeaderAvatar.ImageUrl = ResolveUrl(imgPath);
                                imgHeaderAvatar.Visible = true;
                                lblAvatarInitial.Visible = false;
                            }
                            else
                            {
                                imgHeaderAvatar.Visible = false;
                                lblAvatarInitial.Text = displayName.Substring(0, 1).ToUpper();
                                lblAvatarInitial.Visible = true;
                            }
                        }
                    }
                }
                lblHeaderName.Text = displayName;
            }
        }

        private void ShowView(string viewName)
        {
            CurrentView = viewName;
            pnlForumList.Visible = (viewName == "forumList");
            pnlQuestionList.Visible = (viewName == "questionList");
            pnlQuestionDetail.Visible = (viewName == "questionDetail");
            pnlPostForm.Visible = (viewName == "postForm");
            lblGlobalMsg.Visible = false;
        }

        private void ShowMsg(string text, bool success)
        {
            lblGlobalMsg.Text = text;
            lblGlobalMsg.CssClass = success ? "alert alert-success" : "alert alert-error";
            lblGlobalMsg.Visible = true;
        }

        protected string GetProfileImage(object imageObj)
        {
            if (imageObj == null || imageObj == DBNull.Value || string.IsNullOrEmpty(imageObj.ToString().Trim()))
                return ResolveUrl("~/images/default-user.png");
            return ResolveUrl(imageObj.ToString());
        }

        protected bool IsOwner(object postUserIdObj)
        {
            if (postUserIdObj == null || postUserIdObj == DBNull.Value) return false;
            return Convert.ToInt32(postUserIdObj) == Convert.ToInt32(Session["userid"]);
        }

        // ========================== 1. FORUM LIST ==========================
        private void LoadForums(string searchTerm)
        {
            int userId = Convert.ToInt32(Session["userid"]);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT f.forumid, f.title, f.description, f.creationtime, c.coursename,
                           (SELECT COUNT(*) FROM ForumPost fp WHERE fp.forumid = f.forumid AND fp.parentid IS NULL AND fp.deletiontime IS NULL) AS postcount
                    FROM CourseForum f
                    INNER JOIN Course c ON f.courseid = c.courseid
                    INNER JOIN Enrollment e ON c.courseid = e.courseid
                    WHERE e.userid = @uid AND e.isactive = 1 AND f.deletiontime IS NULL";

                if (!string.IsNullOrEmpty(searchTerm))
                    query += " AND (f.title LIKE @search OR c.coursename LIKE @search)";

                query += " ORDER BY f.creationtime DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                    if (!string.IsNullOrEmpty(searchTerm))
                        da.SelectCommand.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptForums.DataSource = dt;
                    rptForums.DataBind();
                    pnlEmptyForums.Visible = (dt.Rows.Count == 0);
                }
            }
        }

        protected void btnSearchForums_Click(object sender, EventArgs e) { LoadForums(txtSearchForums.Text.Trim()); }
        protected void btnClearForums_Click(object sender, EventArgs e) { txtSearchForums.Text = ""; LoadForums(""); }

        protected void rptForums_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "OpenForum")
            {
                CurrentForumId = Convert.ToInt32(e.CommandArgument);
                LoadQuestionsList();
                ShowView("questionList");
            }
        }


        // ========================== 2. QUESTION LIST ==========================
        private void LoadQuestionsList()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Get Forum Header Info
                using (SqlCommand cmd = new SqlCommand("SELECT f.title, c.coursename FROM CourseForum f INNER JOIN Course c ON f.courseid = c.courseid WHERE f.forumid = @fid", con))
                {
                    cmd.Parameters.AddWithValue("@fid", CurrentForumId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblQLForumTitle.Text = reader["title"].ToString();
                            lblQLCourseName.Text = reader["coursename"].ToString();
                        }
                    }
                }

                // Get Questions
                string query = @"
                    SELECT p.postid, p.title, p.content, p.creationtime, p.upvotes, p.downvotes,
                           u.fname, u.lname, u.ProfileImage,
                           (SELECT COUNT(*) FROM ForumPost r WHERE r.parentid = p.postid AND r.deletiontime IS NULL) AS replycount
                    FROM ForumPost p
                    INNER JOIN [User] u ON p.userid = u.userid
                    WHERE p.forumid = @fid AND p.parentid IS NULL AND p.deletiontime IS NULL
                    ORDER BY p.creationtime DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@fid", CurrentForumId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptQuestions.DataSource = dt;
                    rptQuestions.DataBind();
                    pnlEmptyQuestions.Visible = (dt.Rows.Count == 0);
                }
            }
        }

        protected void btnBackToForums_Click(object sender, EventArgs e)
        {
            ShowView("forumList");
            LoadForums("");
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "OpenQuestion")
            {
                CurrentQuestionId = Convert.ToInt32(e.CommandArgument);
                LoadQuestionDetail();
                ShowView("questionDetail");
            }
        }


        // ========================== 3. QUESTION DETAILS & ANSWERS ==========================
        private void LoadQuestionDetail()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 1. Load Main Question
                string qQuery = @"
                    SELECT p.title, p.content, p.creationtime, p.userid, p.upvotes, p.downvotes, p.fileurl,
                           u.fname, u.lname, u.ProfileImage
                    FROM ForumPost p
                    INNER JOIN [User] u ON p.userid = u.userid
                    WHERE p.postid = @pid AND p.deletiontime IS NULL";

                using (SqlCommand cmd = new SqlCommand(qQuery, con))
                {
                    cmd.Parameters.AddWithValue("@pid", CurrentQuestionId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblDetailTitle.Text = Server.HtmlEncode(reader["title"].ToString());
                            lblDetailContent.Text = Server.HtmlEncode(reader["content"].ToString()).Replace("\n", "<br/>");
                            lblDetailAuthorName.Text = reader["fname"].ToString() + " " + reader["lname"].ToString();
                            lblDetailDate.Text = Convert.ToDateTime(reader["creationtime"]).ToString("MMM dd, yyyy - hh:mm tt");
                            imgDetailAuthor.ImageUrl = GetProfileImage(reader["ProfileImage"]);
                            lblUpvotesQ.Text = reader["upvotes"].ToString();
                            lblDownvotesQ.Text = reader["downvotes"].ToString();

                            string fileUrl = reader["fileurl"] != DBNull.Value ? reader["fileurl"].ToString() : "";
                            if (!string.IsNullOrEmpty(fileUrl))
                            {
                                pnlDetailAttachment.Visible = true;
                                hlDetailFile.NavigateUrl = ResolveUrl(fileUrl);
                            }
                            else { pnlDetailAttachment.Visible = false; }

                            divDetailOwnerActions.Visible = IsOwner(reader["userid"]);
                        }
                        else
                        {
                            ShowView("questionList");
                            return;
                        }
                    }
                }

                // 2. Load Answers
                string aQuery = @"
                    SELECT p.postid, p.content, p.creationtime, p.userid, p.upvotes, p.downvotes, p.fileurl,
                           u.fname, u.lname, u.ProfileImage
                    FROM ForumPost p
                    INNER JOIN [User] u ON p.userid = u.userid
                    WHERE p.parentid = @pid AND p.deletiontime IS NULL
                    ORDER BY p.creationtime ASC";

                using (SqlDataAdapter da = new SqlDataAdapter(aQuery, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@pid", CurrentQuestionId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    lblAnswerCount.Text = dt.Rows.Count.ToString();
                    rptAnswers.DataSource = dt;
                    rptAnswers.DataBind();
                    pnlNoAnswers.Visible = (dt.Rows.Count == 0);
                }
            }
        }

        protected void btnBackToQuestions_Click(object sender, EventArgs e)
        {
            ShowView("questionList");
            LoadQuestionsList();
        }

        // Handle Voting on Main Question
        protected void btnUpvoteQ_Click(object sender, EventArgs e) { HandleVote(CurrentQuestionId, 1); LoadQuestionDetail(); }
        protected void btnDownvoteQ_Click(object sender, EventArgs e) { HandleVote(CurrentQuestionId, -1); LoadQuestionDetail(); }

        // Handle Actions on Answers
        protected void rptAnswers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int answerId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Upvote") { HandleVote(answerId, 1); LoadQuestionDetail(); }
            else if (e.CommandName == "Downvote") { HandleVote(answerId, -1); LoadQuestionDetail(); }
            else if (e.CommandName == "DeleteAnswer") { DeletePost(answerId); LoadQuestionDetail(); }
            else if (e.CommandName == "EditAnswer") { OpenForm(false, answerId); }
        }

        // Post a brand new answer
        protected void btnPostAnswer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewAnswer.Text)) return;
            SavePost(false, 0, null, txtNewAnswer.Text.Trim(), fuAnswerFile);
            txtNewAnswer.Text = "";

            // Gamification: Give 5 points for answering!
            AwardPoints(Convert.ToInt32(Session["userid"]), 5);
            ShowMsg("Answer posted! You earned +5 Points.", true);
            LoadQuestionDetail();
        }

        // Edit/Delete Main Question Triggers
        protected void btnEditQuestion_Click(object sender, EventArgs e) { OpenForm(true, CurrentQuestionId); }
        protected void btnDeleteQuestion_Click(object sender, EventArgs e)
        {
            DeletePost(CurrentQuestionId);
            ShowView("questionList");
            LoadQuestionsList();
        }


        // ========================== 4. SHARED FORMS & LOGIC ==========================
        protected void btnOpenAsk_Click(object sender, EventArgs e)
        {
            OpenForm(true, 0); // true = IsQuestion, 0 = New
        }

        private void OpenForm(bool isQuestion, int postIdToEdit)
        {
            EditPostId = postIdToEdit;
            txtPostTitle.Text = "";
            txtPostContent.Text = "";
            pnlFormTitleGroup.Visible = isQuestion;

            if (postIdToEdit == 0)
            {
                lblFormModeTitle.Text = isQuestion ? "Ask a New Question" : "Write an Answer";
                btnSavePost.Text = isQuestion ? "Post Question (+2 Pts)" : "Submit Answer";
            }
            else
            {
                lblFormModeTitle.Text = "Edit Post";
                btnSavePost.Text = "Save Changes";

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("SELECT title, content FROM ForumPost WHERE postid=@pid", con);
                    cmd.Parameters.AddWithValue("@pid", postIdToEdit);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (isQuestion) txtPostTitle.Text = reader["title"].ToString();
                            txtPostContent.Text = reader["content"].ToString();
                        }
                    }
                }
            }
            ShowView("postForm");
        }

        protected void btnSavePost_Click(object sender, EventArgs e)
        {
            bool isQuestion = pnlFormTitleGroup.Visible;
            string title = isQuestion ? txtPostTitle.Text.Trim() : null;
            string content = txtPostContent.Text.Trim();

            if ((isQuestion && string.IsNullOrWhiteSpace(title)) || string.IsNullOrWhiteSpace(content))
            {
                ShowMsg("Please fill in all required fields.", false);
                return;
            }

            SavePost(isQuestion, EditPostId, title, content, fuPostFile);

            if (EditPostId == 0 && isQuestion)
            {
                // Gamification: Give 2 points for asking a question!
                AwardPoints(Convert.ToInt32(Session["userid"]), 2);
                ShowMsg("Question posted! You earned +2 Points.", true);
                LoadQuestionsList();
                ShowView("questionList");
            }
            else
            {
                LoadQuestionDetail();
                ShowView("questionDetail");
            }
        }

        protected void btnBackFromForm_Click(object sender, EventArgs e)
        {
            if (pnlFormTitleGroup.Visible && EditPostId == 0) { ShowView("questionList"); LoadQuestionsList(); }
            else { ShowView("questionDetail"); LoadQuestionDetail(); }
        }

        // Shared Logic: Insert or Update Post + Handle File Upload
        private void SavePost(bool isQuestion, int postIdToEdit, string title, string content, FileUpload uploadControl)
        {
            int userId = Convert.ToInt32(Session["userid"]);
            string fileUrl = null;

            if (uploadControl.HasFile)
            {
                string ext = Path.GetExtension(uploadControl.FileName).ToLower();
                if (ext != ".pdf" && ext != ".docx" && ext != ".zip")
                {
                    ShowMsg("Invalid file type.", false);
                    return;
                }
                string folder = Server.MapPath("~/Forum_materials/files/");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string fileName = Guid.NewGuid().ToString() + ext;
                uploadControl.SaveAs(Path.Combine(folder, fileName));
                fileUrl = "~/Forum_materials/files/" + fileName;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                if (postIdToEdit == 0) // INSERT
                {
                    string sql = @"INSERT INTO ForumPost (forumid, userid, parentid, title, content, fileurl, creationtime) 
                                   VALUES (@fid, @uid, @pid, @title, @content, @furl, GETDATE())";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@fid", CurrentForumId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@pid", isQuestion ? (object)DBNull.Value : CurrentQuestionId);
                    cmd.Parameters.AddWithValue("@title", isQuestion ? title : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@content", content);
                    cmd.Parameters.AddWithValue("@furl", fileUrl != null ? fileUrl : (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                else // UPDATE
                {
                    string sql = "UPDATE ForumPost SET content=@content, lastupdated=GETDATE() ";
                    if (isQuestion) sql += ", title=@title ";
                    if (fileUrl != null) sql += ", fileurl=@furl ";
                    sql += "WHERE postid=@postid AND userid=@uid";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@content", content);
                    if (isQuestion) cmd.Parameters.AddWithValue("@title", title);
                    if (fileUrl != null) cmd.Parameters.AddWithValue("@furl", fileUrl);
                    cmd.Parameters.AddWithValue("@postid", postIdToEdit);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeletePost(int postId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE ForumPost SET deletiontime=GETDATE() WHERE postid=@pid AND userid=@uid", con);
                cmd.Parameters.AddWithValue("@pid", postId);
                cmd.Parameters.AddWithValue("@uid", Convert.ToInt32(Session["userid"]));
                cmd.ExecuteNonQuery();
            }
        }

        private void HandleVote(int targetPostId, int voteType)
        {
            int userId = Convert.ToInt32(Session["userid"]);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand check = new SqlCommand("SELECT votetype FROM ForumVote WHERE postid=@pid AND userid=@uid", con);
                check.Parameters.AddWithValue("@pid", targetPostId);
                check.Parameters.AddWithValue("@uid", userId);
                object existing = check.ExecuteScalar();

                if (existing != null)
                {
                    if (Convert.ToInt32(existing) == voteType)
                    {
                        // Remove vote
                        new SqlCommand("DELETE FROM ForumVote WHERE postid=@pid AND userid=@uid", con)
                        { Parameters = { new SqlParameter("@pid", targetPostId), new SqlParameter("@uid", userId) } }.ExecuteNonQuery();
                    }
                    else
                    {
                        // Switch vote
                        new SqlCommand("UPDATE ForumVote SET votetype=@type WHERE postid=@pid AND userid=@uid", con)
                        { Parameters = { new SqlParameter("@type", voteType), new SqlParameter("@pid", targetPostId), new SqlParameter("@uid", userId) } }.ExecuteNonQuery();
                    }
                }
                else
                {
                    // New vote
                    new SqlCommand("INSERT INTO ForumVote (postid, userid, votetype) VALUES (@pid, @uid, @type)", con)
                    { Parameters = { new SqlParameter("@pid", targetPostId), new SqlParameter("@uid", userId), new SqlParameter("@type", voteType) } }.ExecuteNonQuery();
                }

                // Update post totals
                new SqlCommand(@"
                    UPDATE ForumPost 
                    SET upvotes = (SELECT COUNT(*) FROM ForumVote WHERE postid=@pid AND votetype=1),
                        downvotes = (SELECT COUNT(*) FROM ForumVote WHERE postid=@pid AND votetype=-1)
                    WHERE postid=@pid", con)
                { Parameters = { new SqlParameter("@pid", targetPostId) } }.ExecuteNonQuery();
            }
        }

        // ========================== GAMIFICATION LOGIC ==========================
        private void AwardPoints(int userId, int pointsToAdd)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand pts = new SqlCommand(@"
                    UPDATE StudentPoints SET totalpoints = totalpoints + @pts, lastupdated = GETDATE() WHERE userid = @uid;
                    IF @@ROWCOUNT = 0
                        INSERT INTO StudentPoints (userid, totalpoints, badge, lastupdated) VALUES (@uid, @pts, 'Bronze', GETDATE());
                    ", con))
                {
                    pts.Parameters.AddWithValue("@uid", userId);
                    pts.Parameters.AddWithValue("@pts", pointsToAdd);
                    pts.ExecuteNonQuery();
                }

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