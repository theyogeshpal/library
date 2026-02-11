using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlTypes;

namespace LibraryManagementSystem.Models
{
    public class DatabaseConnection 
    {
        SqlConnection con;

        public DatabaseConnection(IConfiguration configuration)
        {
            con = new SqlConnection(configuration.GetConnectionString("myconnection"));
            con.Open();
        }

        public bool ExecuteIUD(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, con);
            int row = 0;
            try
            {
                row = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                row = 0;
            }

            return row > 0;
        }

        public DataTable Executeselect(string sql)
        {
            SqlDataAdapter sda = new SqlDataAdapter(sql, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

        
    }
}
