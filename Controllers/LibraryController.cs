using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Runtime.InteropServices;

namespace LibraryManagementSystem.Controllers
{
    
    public class LibraryController : Controller
    {
        DatabaseConnection connection;
        

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
        public IActionResult checkLogin(IFormCollection form)
        {
            string a = form["username"];
            string b = form["password"];

            string cmd = "select * from librarian where username='" + a + "' and password='" + b + "'";
            DataTable dt = new DataTable();
            dt = connection.Executeselect(cmd);
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
