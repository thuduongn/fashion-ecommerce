using fashion.Data;
using fashion.Models;
using fashion.Services;
using MailKit;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Microsoft.AspNetCore.Routing.Tree;

namespace fashion.Controllers
{
    public class HomeController : Controller
    {
        private readonly FashionContext _context;

        public HomeController(FashionContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var trendyProducts = GetTrendyProducts();
            ViewBag.TrendyProducts = trendyProducts;
            
            return View();
        }

        private List<Product> GetTrendyProducts()
        {
            var trendyProducts = _context.Products.Include(p => p.Brand).Where(p => p.Deleted == 0 && p.Status == 1).ToList();
            return trendyProducts;
        }

        public async Task<IActionResult> Search(string search)
        {
            var products = await _context.Products
                                     .Where(p => p.Name.Contains(search) && p.Deleted == 0 && p.Status == 1)
                                     .ToListAsync();
            return Json(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Register()
        {
            // Ki?m tra n?u ?ã ??ng nh?p
            if (HttpContext.Session.GetInt32("IdCustomer") != null)
            {
                // ?ã ??ng nh?p, chuy?n h??ng ??n trang chính
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Customers.FirstOrDefault(c => c.Email == customer.Email);
                if (check == null)
                {
                    customer.Pd = PasswordHasher.HashPassword(customer.Pd);
                    // t?o mã kích ho?t
                    var rand = new Random();

                    int randomNumber = rand.Next(1000, 10000);

                    customer.ActivationCode = randomNumber.ToString();

                    string body = "Click <a href='http://localhost:5074//home/activation?email="
                        + customer.Email + "&code=" + randomNumber.ToString() + "'>here</a>" +
                        " to activation account!!!";

                    _context.Customers.Add(customer);
                    _context.SaveChanges();

                    // gui mail thong bao dang ky thanh cong
                    ViewBag.success = "Activation successful!! Please sign in your email to check for activation link";
                    Services.MailService.SendRegistrationEmail(customer.Email, body);
                    return View("Login");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Login()
        {
            //Ki?m tra n?u ?ã ??ng nh?p
            if (HttpContext.Session.GetInt32("IdCustomer") != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(s => s.Email.Equals(login.Email) && s.StatusActive == 1);
                if (customer != null)
                {
                    if (PasswordHasher.VerifyPassword(customer.Pd, login.Password))
                    {
                        HttpContext.Session.SetString("FullName", customer.Name);
                        HttpContext.Session.SetString("Email", customer.Email);
                        HttpContext.Session.SetInt32("IdCustomer", customer.Id);
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
                    ViewBag.error = "Email not found";
                    return View("Login");
                }
            }
            ViewBag.error = "Login failed";
            return View("Login");
        }

        public IActionResult Logout()
        {
            // Xóa các thông tin ??ng nh?p t? Session
            HttpContext.Session.Clear();

            // Chuy?n h??ng ng??i dùng ??n trang ??ng nh?p
            return RedirectToAction("Login", "Home");
        }

        public async Task<IActionResult> Activation(string email, string code)
        {
            // ki?m tra email và password không ???c tr?ng n?u tr?ng thì tr? v? view kèm l?i gì ?ó
            if (email.IsNullOrEmpty() || code.IsNullOrEmpty())
            {
                ViewBag.error = "Cannot activate your account!!";
                return View("Login");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(s => s.Email.Equals(email) && s.ActivationCode == code);
            if (customer != null)
            {
                customer.StatusActive = 1;
                _context.SaveChanges();
                ViewBag.success = "Activation successful!!";
                return View("Login");
            }
            else
            {
                ViewBag.error = "Cannot activate your account!!";
            }

            return View("Login");
        }

        //[HttpPost]
        //public string Test([FromBody] dynamic data)
        //{
        //    string name = data.GetProperty("name").GetString();
        //    return name;
        //}
    }
}
