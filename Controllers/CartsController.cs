using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fashion.Data;
using Org.BouncyCastle.Asn1.Ocsp;

namespace fashion.Controllers
{
    public class CartsController : Controller
    {
        private readonly FashionContext _context;

        public CartsController(FashionContext context)
        {
            _context = context;
        }

        

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("IdCustomer") == null)
            {
                //đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("Login", "Home");
            }
            var cusId = HttpContext.Session.GetInt32("IdCustomer");
            //Console.WriteLine(cusId);
            var cart = _context.Carts.Include(c => c.Product).Where(c => c.CustomerId == cusId);
            ViewBag.ListProductInCart = cart;
            return View(/*await fashionContext.ToListAsync()*/);
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantity,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,ProductId,CustomerId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", cart.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", cart.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,ProductId,CustomerId")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", cart.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, string type = "plus", int quantity = 0, int detailQuantity = 0)
        {
            if (HttpContext.Session.GetInt32("IdCustomer") == null)
            {
                // Đã đăng nhập, chuyển hướng đến trang chính
                return Json(new { success = false, content = "Please Log In to add products to Cart!!" });
            }
            if (id == null)
            {
                return NotFound();
            }
            var UserId = HttpContext.Session.GetInt32("IdCustomer");
            var cart = _context.Carts.FirstOrDefault(c => c.ProductId == id && c.CustomerId == UserId);
            var productQuantity = _context.Products.FirstOrDefault(p => p.Id == id)?.Quantity;

            if (cart is not null)
            {
                if (type.Equals("plus"))
                {
                    if (detailQuantity != 0)
                    {
                        cart.Quantity += detailQuantity;
                    }
                    else
                    {
                        cart.Quantity = cart.Quantity + 1;
                    }

                    if (cart.Quantity > productQuantity)
                    {
                        cart.Quantity = cart.Quantity - 1;
                        return Json(new { success = false, content = "Cannot exceed the product limit!!" });
                    }
                }
                else if (type.Equals("minus"))
                {
                    cart.Quantity = cart.Quantity - 1;
                }
                else
                {
                    cart.Quantity = quantity;
                    if (cart.Quantity > productQuantity)
                    {
                        cart.Quantity = cart.Quantity - 1;
                        return Json(new { success = false, content = "Cannot exceed the product limit!!" });
                    }
                }
                _context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                var new_cart = new Cart();
                new_cart.CustomerId = HttpContext.Session.GetInt32("IdCustomer");
                new_cart.ProductId = id;
                if (detailQuantity == 0) 
                {
                    new_cart.Quantity = 1;
                }
                else
                {
                    new_cart.Quantity = detailQuantity;
                    if (new_cart.Quantity > productQuantity)
                    {
                        new_cart.Quantity = new_cart.Quantity - 1;
                        return Json(new { success = false, content = "Cannot exceed the product limit!!" });
                    }
                }
                _context.Carts.Add(new_cart);
                if (_context.SaveChanges() == 1)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }

            //return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> GetCartCount()
        {
            var UserId = HttpContext.Session.GetInt32("IdCustomer");
            var cartCount = _context.Carts
                .Where(c => c.CustomerId == UserId)
                .Sum(c => c.Quantity);
            return Json(new { cartCount });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var UserId = HttpContext.Session.GetInt32("IdCustomer");
            var cart = _context.Carts.FirstOrDefault(c => c.ProductId == id && c.CustomerId == UserId);
            if (cart is not null)
            {
                _context.Carts.Remove(cart);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> GetCartSummary()
        {
            var cusId = HttpContext.Session.GetInt32("IdCustomer");
            var cart = _context.Carts.Include(c => c.Product).Where(c => c.CustomerId == cusId).ToList();
            decimal subTotal = 0;
            foreach (var item in cart) 
            {
                decimal price = item.Product.Price ?? 0;                                       
                int quantity = item.Quantity ?? 0;
                subTotal += price * quantity;
            }
            decimal totalSummary = subTotal + 10;
            return Json(new { subTotal, totalSummary });

        }

        // GET: Carts/Checkout
        public async Task<IActionResult> Checkout()
        {
            //var fashionContext = _context.Carts.Include(c => c.Customer).Include(c => c.Product);
            if (HttpContext.Session.GetInt32("IdCustomer") == null)
            {
                //đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("Login", "Home");
            }
            var cusId = HttpContext.Session.GetInt32("IdCustomer");
            var customer = _context.Customers.Where(c => c.Id == cusId).FirstOrDefault();
            ViewBag.Customer = customer;
            var cart = _context.Carts.Include(c => c.Product).Where(c => c.CustomerId == cusId).ToList();
            ViewBag.ListProductInCart = cart;
            if (cart.Count == 0)
            {
                ViewBag.error = "There need to be at least 1 product in the cart.";
                return RedirectToAction("Index", "Carts");
            }

            foreach (var item in cart)
            {
                if (item.Quantity > item.Product.Quantity)
                {
                    ViewBag.error = $"Quantity for {item.Product.Name} exceeds the available stock limit.";
                    item.Quantity = item.Product.Quantity;
                }
            }

            return View(/*await fashionContext.ToListAsync()*/);
        }

        public async Task<IActionResult> PlaceOrder(string note)
        {
            //var fashionContext = _context.Carts.Include(c => c.Customer).Include(c => c.Product);
            if (HttpContext.Session.GetInt32("IdCustomer") == null)
            {
                //đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("Login", "Home");
            }
            var cusId = HttpContext.Session.GetInt32("IdCustomer");
            //var customer = _context.Customers.Where(c => c.Id == cusId).FirstOrDefault();
            //ViewBag.Customer = customer;
            var cart = _context.Carts.Include(c => c.Product).Where(c => c.CustomerId == cusId).ToList();

            //if (cart.Count == 0)
            //{
            //    ViewBag.error = "There need to be at least 1 product in the cart.";
            //    return RedirectToAction("Index", "Carts");
            //}
            var sum = 0;
            foreach (var item in cart)
            {
                if (item.Quantity > item.Product.Quantity)
                {
                    var content = "Sản phẩm " + item.Product.Name + " không đủ số lượng";
                    return Json(new { success = false, content = content });
                }
                if(item.Product.Status != 1 || item.Product.Deleted == 1)
                {
                    var content = "Sản phẩm " + item.Product.Name + " không tồn tại";
                    return Json(new { success = false, content = content });
                }
                var quantity = item.Quantity ?? 0; // Chuyển đổi từ int? sang int
                var price = item.Product?.Price ?? 0; // Chuyển đổi từ int? sang int
                sum += quantity * price;
            }
            var fee = 20000;
            var totalPrice = sum + fee; //sum + fee
            DateTime currentTime = DateTime.Now;
            DateTime currentTimeWithoutMilliseconds = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);

            long secondsSinceEpoch = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

            Order od = new Order();
            od.TotalPrice = totalPrice;
            od.Note = note;
            od.Fee = fee;
            od.CreatedAt = (int)secondsSinceEpoch;
            od.CustomerId = cusId;
            od.OrderDate = currentTimeWithoutMilliseconds;
            od.Status = 1;

            _context.Orders.Add(od);
            _context.SaveChanges();
            var idOrder = od.Id;
            // move dữ liệu từ bảng cart sang orderdetail
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in cart)
            {
                OrderDetail tmp = new OrderDetail();
                tmp.OrderId = idOrder;
                tmp.ProductId = item.ProductId;
                tmp.Quantity = item.Quantity;
                tmp.Price = item.Product.Price;
                tmp.CreatedAt = (int)secondsSinceEpoch;
                orderDetails.Add(tmp);
               
                // giảm số lượng
                var product = _context.Products.Where(p => p.Id == item.Product.Id).FirstOrDefault();
                product.Quantity -= item.Quantity;
                _context.Products.Update(product);
                _context.SaveChanges();

                // xoa khoi cart
                var cartItemToRemove = _context.Carts.FirstOrDefault(c => c.ProductId == item.ProductId);
                if (cartItemToRemove != null)
                {
                    _context.Carts.Remove(cartItemToRemove);
                    _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                }
            }
            _context.OrderDetails.AddRange(orderDetails);
            _context.SaveChanges();

            // lưu lại logs order history
            OrderHistory oh = new OrderHistory();
            oh.OrderId = idOrder;
            oh.Description = "Customer create order";
            oh.CreatedAt = (int)secondsSinceEpoch;
            _context.OrderHistories.Add(oh);
            _context.SaveChanges();

            // bổ xung thêm gửi mail 
            return Json(new { success = true, content = "OK!" }); ;
        }

    }
}