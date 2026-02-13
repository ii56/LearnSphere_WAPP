using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LearnSphere_WAPP
{
	public partial class TestConnection : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

        protected void btnTest_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    lblResult.Text = "Database Connected Successfully!";
                    lblResult.ForeColor = System.Drawing.Color.Green;
                }
                catch (Exception ex)
                {
                    lblResult.Text = "Connection Failed: " + ex.Message;
                    lblResult.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}