using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LearnSphere_WAPP
{
    public class Syslog
    {
        static public void action(int UserID, string action)
        {
            DateTime curr = DateTime.Now;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LearnSphereDB"].ConnectionString);
            con.Open();

            string query = "insert into Syslog (userid, action, dateTime) values (@UserID, @Action, @DateTime)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@DateTime", curr);
            cmd.ExecuteNonQuery();

            con.Close();
        } 
    }
}