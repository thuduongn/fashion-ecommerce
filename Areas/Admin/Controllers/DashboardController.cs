using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fashion.Data;
using fashion.Services;
using fashion.Areas.Admin.Models;

namespace fashion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly FashionContext _context;

        public DashboardController(FashionContext context)
        {
            _context = context;
        }

        // GET: DashboardController
        public ActionResult Index()
        {
            if (HttpContext.Session.GetInt32("IdUser") == null)
            {
                return Redirect("/Admin");
            }
            return View();
        }

        public ActionResult Register()
        {
            if (HttpContext.Session.GetInt32("IdUser") != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {   
                var check = _context.Users.FirstOrDefault(u => u.UserName == user.UserName);
                
                if (check == null)
                {
                    user.Pd = PasswordHasher.HashPassword(user.Pd);
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "User name already exists";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Login()
        {
            //Ki?m tra n?u ?ã ??ng nh?p
            if (HttpContext.Session.GetInt32("IdUser") != null)
            {
                return RedirectToAction("Dashboard","Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,Password")] LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(s => s.UserName.Equals(login.UserName));
                if (user != null)
                {
                    if (PasswordHasher.VerifyPassword(user.Pd, login.Password))
                    {
                        HttpContext.Session.SetString("FullName", user.Name);
                        HttpContext.Session.SetString("UserName", user.UserName);
                        HttpContext.Session.SetInt32("IdUser", user.Id);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.error = "Wrong password";
                        return View("Login");
                    }
                }
                else
                {
                    ViewBag.error = "User name not found";
                    return View("Login");
                }
            }
            ViewBag.error = "Login failed";
            return View("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Redirect("/Admin");
        }

        // GET: DashboardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DashboardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashboardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashboardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashboardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
