using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem.Controllers
{
    public class LibraryController : Controller
    {
        private readonly DatabaseConnection connection;

        public LibraryController(DatabaseConnection connection)
        {
            this.connection = connection;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult librarianLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> checkLogin(IFormCollection form)
        {
            string a = form["username"];
            string b = form["password"];

            string cmd = "select * from librarian where username=@user and password=@pass";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", a),
                new SqlParameter("@pass", b)
            };

            DataTable dt = await connection.ExecuteSelectAsync(cmd, parameters);
            if (dt.Rows.Count > 0)
            {
                HttpContext.Session.SetString("librarian", a);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["loginerror"] = "Entered Details did not match with data in Database";
                return RedirectToAction("librarianLogin");
            }
        }
    }
}

