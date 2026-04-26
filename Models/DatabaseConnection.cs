using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryManagementSystem.Models
{
    public class DatabaseConnection : IDisposable
    {
        private readonly SqlConnection _connection;
        private bool _disposed = false;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("myconnection"));
        }

        private async Task EnsureConnectionOpenAsync()
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
        }

        public async Task<bool> ExecuteIUDAsync(string sql, params SqlParameter[] parameters)
        {
            await EnsureConnectionOpenAsync();
            using (SqlCommand cmd = new SqlCommand(sql, _connection))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                
                try
                {
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<DataTable> ExecuteSelectAsync(string sql, params SqlParameter[] parameters)
        {
            await EnsureConnectionOpenAsync();
            using (SqlCommand cmd = new SqlCommand(sql, _connection))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    return dt;
                }
            }
        }

        // Keep synchronous methods for compatibility but mark as obsolete or refactor later
        public bool ExecuteIUD(string sql)
        {
            if (_connection.State != ConnectionState.Open) _connection.Open();
            using (SqlCommand cmd = new SqlCommand(sql, _connection))
            {
                try
                {
                    int row = cmd.ExecuteNonQuery();
                    return row > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public DataTable Executeselect(string sql)
        {
            if (_connection.State != ConnectionState.Open) _connection.Open();
            using (SqlDataAdapter sda = new SqlDataAdapter(sql, _connection))
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                    {
                        if (_connection.State == ConnectionState.Open)
                            _connection.Close();
                        _connection.Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }
}

