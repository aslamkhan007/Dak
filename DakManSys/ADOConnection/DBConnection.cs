using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DakManSys.ADOConnection
{
    public class DBConnection
    {
        public static SqlConnection getConnection()
        {
            string connect = "Data Source=misdev;Initial Catalog=jctdev;User ID=itgrp;Password=power";
            SqlConnection con = new SqlConnection(connect);
            con.Open();
            return con;
        }
    }
}