using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP.Admin
{
    public partial class AdminForums : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;
        int courseId;
        int forumId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usertype"] == null || (Session["usertype"].ToString() != "Admin" && Session["usertype"].ToString() != "SuperAdmin"))
            {
                Response.Redirect("../Login.aspx");
                return;
            }

            if (!int.TryParse(Request.QueryString["courseid"], out courseId))
                Response.Redirect("AdminForums.aspx");

            if (!IsPostBack)
            {
                LoadForumDetails();
                LoadSidebarProfileImage();
                LoadQuestions();
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
                    sidebarImg.Src = ResolveUrl(imagePath);
                }
                else
                {
                    sidebarImg.Src = ResolveUrl("../images/default-user.png");
                }
            }
        }

        protected string GetProfileImage(object imageObj)
        {
            if (imageObj == null || imageObj == DBNull.Value)
                return ResolveUrl("../images/default-user.png");

            string path = imageObj.ToString();

            if (string.IsNullOrEmpty(path))
                return ResolveUrl("../images/default-user.png");

            return ResolveUrl(path);
        }

        private void LoadForumDetails()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT forumid, title, description, tags
                    FROM CourseForum
                    WHERE courseid = @courseid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    forumId = Convert.ToInt32(reader["forumid"]);
                    lblForumTitle.Text = reader["title"].ToString();
                    lblDescription.Text = reader["description"].ToString();
                    lblTags.Text = reader["tags"]?.ToString();
                }
                else
                {
                    Response.Redirect("AdminForums.aspx");
                }
            }
        }

        private void LoadQuestions()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                                SELECT p.postid,
                                       p.title,
                                       p.content,
                                       p.tags,
                                       p.upvotes,
                                       p.downvotes,
                                       p.creationtime,
                                       u.uname,
                                       u.ProfileImage
                                FROM ForumPost p
                                INNER JOIN CourseForum f ON p.forumid = f.forumid
                                INNER JOIN [User] u ON p.userid = u.userid
                                WHERE f.courseid = @courseid
                                AND p.parentid IS NULL
                                AND p.deletiontime IS NULL
                                ORDER BY p.creationtime DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptQuestions.DataSource = dt;
                rptQuestions.DataBind();
            }
        }

        protected string FormatTags(object tagObj)
        {
            if (tagObj == null) return "";

            string[] tags = tagObj.ToString().Split(',');

            string result = "";

            foreach (string tag in tags)
            {
                result += $"<span>{tag.Trim()}</span>";
            }

            return result;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("../Login.aspx");
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int pid = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Delete")
            {
                int deletePostId = Convert.ToInt32(e.CommandArgument);

                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();

                        string deleteChildQuery = @"
                    UPDATE ForumPost
                    SET deletiontime = GETDATE()
                    WHERE parentid = @pid
                    AND deletiontime IS NULL";

                        SqlCommand cmdChild = new SqlCommand(deleteChildQuery, con);
                        cmdChild.Parameters.Add("@pid", SqlDbType.Int).Value = deletePostId;
                        cmdChild.ExecuteNonQuery();

                        string deleteMainQuery = @"
                    UPDATE ForumPost
                    SET deletiontime = GETDATE()
                    WHERE postid = @pid
                    AND deletiontime IS NULL";

                        SqlCommand cmdMain = new SqlCommand(deleteMainQuery, con);
                        cmdMain.Parameters.Add("@pid", SqlDbType.Int).Value = deletePostId;

                        int rows = cmdMain.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            Response.Write("<script>alert('Delete failed!');</script>");
                            return;
                        }

                        LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Admin Delete Post (PostID: " + deletePostId + ")");
                        LoadForumDetails();
                        LoadQuestions();
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Delete failed!');</script>");
                }
            }
        }

        protected void btnLogout_Click1(object sender, EventArgs e)
        {
            Session.Abandon();
            Request.Cookies.Clear();
            LearnSphere_WAPP.Syslog.action(int.Parse(Session["userid"].ToString()), "Logout system");
            Response.Redirect("../Login.aspx");
        }
    }
}