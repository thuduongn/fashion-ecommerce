using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fashion.Data;
using System.Composition;
using NuGet.Protocol.Plugins;

namespace fashion.Controllers
{
    public class ProductsController : Controller
    {
        private readonly FashionContext _context;

        public ProductsController(FashionContext context)
        {
            _context = context;
        }

        // GET: Products/Shop
        public async Task<IActionResult> Shop()
        {
            return View();
        }
        // GET: Products
        public async Task<IActionResult> Index()
        {
            //var fashionContext = _context.Products.Include(p => p.Brand);
            return View(/*await fashionContext.ToListAsync()*/);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var product = await _context.Products
            //    .Include(p => p.Brand)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (product == null)
            //{
            //    return NotFound();
            //}

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            ViewBag.Product = product;
            List<string> imgList = product.Img.Split(',').ToList();
            ViewBag.Img = imgList;

            var dataAttr = _context.Attributes.Where(a => a.ParentId == 0).ToList();
            ViewData["AllAttr"] = dataAttr;
            foreach (var data in dataAttr)
            {
                ViewData[data.Name] = _context.Attributes.Where(a => a.ParentId == data.Id);
            }

            var reviews = _context.Reviews
                         .Include(r => r.Customer) 
                         .Where(r => r.ProductId == id && r.Status == 1 && r.Deleted == 0)
                         .OrderByDescending(r => r.CreatedAt)
                         .ToList();
            ViewBag.Review = reviews;

            var reviewCount = reviews.Count;
            ViewBag.ReviewCount = reviewCount;
            var totalRating = reviews.Sum(r => r.Rate);
            if (reviewCount == 0)
            {
                var avgRating = 0;
                ViewBag.AvgRating = avgRating;
            }
            else
            {
                var avgRating = totalRating / reviewCount;
                ViewBag.AvgRating = avgRating;
                var remainder = avgRating * 10 % reviewCount;
                ViewBag.Remainder = remainder;
            }

            if (HttpContext.Session.GetInt32("IdCustomer") != null)
            {
                var UserId = HttpContext.Session.GetInt32("IdCustomer");
                var customer = _context.Customers.FirstOrDefault(c => c.Id == UserId);
                ViewBag.Customer = customer;
            }

            var productSuggestions = _context.Products
                                    .Include(p => p.LnkProductCategories)
                                    .Where(p => p.LnkProductCategories.Any(lnk => lnk.CategoryId == _context.LnkProductCategories.FirstOrDefault(lnk => lnk.ProductId == id).CategoryId && p.Id != id))
                                    .ToList();
            ViewBag.ProductSuggestions = productSuggestions;
            return View();
        }


        //POST: Products/CreateReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(int? id, int star = 0)
        {
            if (HttpContext.Session.GetInt32("IdCustomer") == null)
            {
                ViewBag.error = "Please Log In to leave review!!";
                return View("~/Views/Home/Login.cshtml");
            }

            var UserId = HttpContext.Session.GetInt32("IdCustomer");
            var content = HttpContext.Request.Form["content"];

            var review = new Review();
            review.Rate = star;
            review.Content = content;
            review.Status = 0;
            review.Deleted = 0;
            review.CustomerId = UserId;
            review.ProductId = id;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Abstract,Description,Img,Slug,Quantity,Status,Deleted,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,BrandId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Abstract,Description,Img,Slug,Quantity,Status,Deleted,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,BrandId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
