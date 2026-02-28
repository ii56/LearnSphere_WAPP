using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace LearnSphere_WAPP.Chatbot
{
    public partial class Chatbot : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Session["usertype"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string userType = Session["usertype"].ToString();

            switch (userType)
            {
                case "Lecturer":
                    Response.Redirect("~/Lecturer/LecturerDashboard.aspx");
                    break;

                case "Student":
                    Response.Redirect("~/Student/StudentDashboard.aspx");
                    break;

                case "Admin":
                    Response.Redirect("~/Admin/AdminDashboard.aspx");
                    break;

                default:
                    Response.Redirect("~/Login.aspx");
                    break;
            }
        }
    }
}