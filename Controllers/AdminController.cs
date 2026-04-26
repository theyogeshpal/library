using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace LibraryManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        DatabaseConnection connection;


        public AdminController(DatabaseConnection connection)
        {
            this.connection = connection;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (HttpContext.Session.GetString("librarian") == null)
            {
                context.Result = RedirectToAction("index", ("Library"));
            }
        }
        public IActionResult Index()
        {
            DashboardModel dashboard = new DashboardModel();

            string cmd1 = "select * from Books";
            DataTable dt1 = connection.Executeselect(cmd1);
            dashboard.Books = dt1;
            ViewBag.Totalbook = dt1.Rows.Count.ToString();

            string cmd2 = "select * from issuedbooks";
            DataTable dt2 = connection.Executeselect(cmd2);
            dashboard.issuedbooks = dt2;
            ViewBag.Issuecount = dt2.Rows.Count.ToString();

            string cmd3 = "select * from overduebooks";
            DataTable dt3 = connection.Executeselect(cmd3);
            dashboard.OverdueBooks = dt3;
            ViewBag.OverdueCount = dt3.Rows.Count.ToString();

            string cmd4 = "select count(*) from librarian";
            DataTable dt4 = connection.Executeselect(cmd4);
            ViewBag.TotalMembers = dt4.Rows[0][0].ToString();

            return View(dashboard);
        }

        public IActionResult TotalAddBook()
        {
            string cmd = "select * from Books";
            DataTable dataTable = new DataTable();
            dataTable = connection.Executeselect(cmd);
            return View(dataTable);
        }

        [HttpPost]
        public IActionResult Addbook(IFormCollection form)
        {
            string a = form["bookid"];
            string b = form["bookname"];
            string c = form["author"];
            string d = form["copy"];
            string e = form["available"];

            //checking whether the book is already present or not

            string cmd1 = "select * from Books where bookID='" + a + "'";
            DataTable dt1 = new DataTable();
            dt1 = connection.Executeselect(cmd1);
            if (dt1.Rows.Count > 0)
            {
                TempData["bookadd"] = "Book with same book ID already exists. Please alot another ID";
                return RedirectToAction("TotalAddBook");
            }
            else
            {
                string cmd = "insert into Books(bookID,title,author,copies,available)values('" + a + "','" + b + "','" + c + "','" + d + "','" + e + "')";

                if (connection.ExecuteIUD(cmd))
                {
                    TempData["bookadd"] = "Book Added Successfully";
                    return RedirectToAction("TotalAddBook");
                }
                else
                {
                    TempData["bookadd"] = "Error! Please Retry";
                    return RedirectToAction("TotalAddBook");
                }
            } 
        }

        public IActionResult DeleteBook(int id) 
        {
            string cmd = "delete from Books where id='" + id + "'";
            if (connection.ExecuteIUD(cmd))
            {
                TempData["bookadd"] = "Book Deleted Successfully";
                return RedirectToAction("TotalAddBook");
            }
            else
            {
                TempData["bookadd"] = "Can't Delete Please Retry";
                return RedirectToAction("TotalAddBook");
            }
        }

        [HttpPost]
        public IActionResult EditBook(IFormCollection form)
        {
            string id = form["editid"];
            string bookname = form["editbookname"];
            string author = form["editauthor"];
            string copies = form["editcopy"];
            string available = form["editavailable"];

            string cmd = "update Books set title='" + bookname + "', author='" + author + "', copies='" + copies + "', available='" + available + "' where id='" + id + "'";
            if (connection.ExecuteIUD(cmd))
            {
                TempData["bookadd"] = "Book Updated Successfully";
            }
            else
            {
                TempData["bookadd"] = "Error updating book. Please retry.";
            }
            return RedirectToAction("TotalAddBook");
        }

        public IActionResult SearchBooks(string query)
        {
            string cmd = "select * from Books where title like '%" + query + "%' or author like '%" + query + "%' or bookID like '%" + query + "%'";
            DataTable dt = connection.Executeselect(cmd);
            ViewBag.SearchQuery = query;
            TempData["bookadd"] = $"Found {dt.Rows.Count} result(s) for '{query}'";
            return View("TotalAddBook", dt);
        }

        public IActionResult issuedbook()
        {
            string cmd = "select * from issuedbooks";
            DataTable dt = new DataTable();
            dt = connection.Executeselect(cmd);
            return View(dt);
        }

        [HttpPost]
        public IActionResult issuebook(IFormCollection form)
        {
            string a = form["bookid"];
            string b = form["stuname"];
            string c = form["mobile"];
            try
            {
                string cmd1 = "select * from Books where bookID='" + a + "'";
                DataTable dt = new DataTable();
                dt = connection.Executeselect(cmd1);

                int aval = int.Parse(dt.Rows[0]["available"].ToString());

                if (aval == 0)
                {
                    TempData["issuedpage"] = "Book is out of stock";
                    return RedirectToAction("issuedbook");
                }
                else
                {
                    string d = dt.Rows[0]["title"].ToString();
                    string e = dt.Rows[0]["author"].ToString();

                    DateTime todaysdate = DateTime.Today;

                    string f = todaysdate.ToShortDateString();
                    string g = form["duedate"];

                    string h = "0";

                    string cmd = "insert into issuedbooks(bookid,stuname,stumobile,bookname,author,issuedate,duedate,status)values('" + a + "','" + b + "','" + c + "','" + d + "','" + e + "','" + f + "','" + g + "','" + h + "')";

                    if (connection.ExecuteIUD(cmd))
                    {
                        int left = aval - 1;
                        TempData["issuedpage"] = "Book Issued Successfully";
                        string cmmd2 = "Update Books set available='" + left + "' where bookID='" + a + "'";
                        if (connection.ExecuteIUD(cmmd2))
                        {
                            return RedirectToAction("issuedbook");
                        }
                        else
                        {
                            TempData["issuedpage"] = "Uncaught Error while issuing book";
                            return RedirectToAction("issuedbook");
                        }


                    }
                    else
                    {
                        TempData["issuedpage"] = "Uncaught Error while issuing book";
                        return RedirectToAction("issuedbook");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["issuedpage"] = "Book is out of stock";
                return RedirectToAction("issuedbook");
            }
              
        }

        
        public IActionResult returned(int id)
        {
            string a = "1";
            string cmd = "Update issuedbooks set status='" + a + "' where bookid='" + id + "'";
            if (connection.ExecuteIUD(cmd))
            {
                //updating book inventory means adding the book to library store start
                string cmd2 = "select * from Books where bookID='" + id + "'";
                DataTable dt1 = connection.Executeselect(cmd2);
                int count = int.Parse(dt1.Rows[0]["available"].ToString());
                int up = count + 1;

                string cmd3 = "Update Books set available='" + up + "' where bookID='" + id + "'";
                if (connection.ExecuteIUD(cmd3))
                {
                    TempData["issuedpage"] = "Book Received Successfully";
                    return RedirectToAction("issuedbook");
                }
                else
                {
                    return RedirectToAction("issuedbook");
                }
            }
            else
            {
                TempData["issuedpage"] = "Uncaught Error";
                return RedirectToAction("issuedbook");
            }

           
                 
            
        }
        public IActionResult OverDueData()
        {
            string cmd = "select * from overduebooks";
            DataTable dt = connection.Executeselect(cmd);
            return View(dt);
        }

        public IActionResult logoutAdmin()
        {
            HttpContext.Session.Remove("librarian");
            return RedirectToAction("librarianLogin", "Library");
        }

    }
}


        





