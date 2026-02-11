using System.Data;

namespace LibraryManagementSystem.Models
{
    public class DashboardModel
    {
        public DataTable Books { get; set; }

        public DataTable issuedbooks { get; set; }

    }
}
