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
    public class OrdersController : Controller
    {
        private readonly FashionContext _context;

        public OrdersController(FashionContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("IdUser") == null)
            {
                return Redirect("/Admin");
            }
            var orders = _context.Orders.Include(o => o.Customer);

            return View(await orders.ToListAsync());
        }

        // GET: Admin/Orders/Order_Status
        public async Task<IActionResult> OrderStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            var finalHandler = _context.OrderHistories.Include(o => o.User).Where(o => o.OrderId == id).OrderByDescending(o => o.Id).FirstOrDefault();
            if(finalHandler == null)
            {
                ViewBag.FinalHandler = null;
            }
            else
            {
                ViewBag.FinalHandler = finalHandler;
            }
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/Order_Invoice
        public async Task<IActionResult> OrderHistory(int ?id)
        {
            var histories = _context.OrderHistories.Include(o => o.User).Include( o=> o.Order).Where(o => o.OrderId == id);
            ViewBag.OrderId = id;
            return View(await histories.ToListAsync());
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TotalPrice,Note,Fee,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,CustomerId,OrderDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", order.CustomerId);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", order.CustomerId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TotalPrice,Note,Fee,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,CustomerId,OrderDate")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", order.CustomerId);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        // GET: Admin/Orders/Order_Status
        public async Task<IActionResult> AddOrderHistory(int id, int status)
        {
            if (id == null)
            {
                return NotFound();
            }

            var od = _context.Orders.Where(o => o.Id == id).FirstOrDefault();
            od.Status = status;
            _context.Orders.Update(od);
            _context.SaveChanges();

            // lưu lại logs ở bảng order history
            DateTime currentTime = DateTime.Now;
            long secondsSinceEpoch = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

            var userId = HttpContext.Session.GetInt32("IdUser");
            OrderHistory oh = new OrderHistory();
            oh.Description = "Update status order to " + status;
            oh.OrderId = id;
            oh.UserId = userId;
            oh.CreatedAt = (int)secondsSinceEpoch;
            _context.OrderHistories.Add(oh);
            _context.SaveChanges();

            return Redirect("/Admin/Orders/OrderStatus/"+id);
        }

    }
}
