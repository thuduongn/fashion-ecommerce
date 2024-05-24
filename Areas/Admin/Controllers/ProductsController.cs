using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fashion.Data;
using System.Web;
using System.Collections;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.CodeAnalysis;
namespace fashion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly FashionContext _context;

        public ProductsController(FashionContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(int quantity = 10, int? category = 0, int? status = 3, string? name = null, int page = 1)
        {
            if (HttpContext.Session.GetInt32("IdUser") == null)
            {
                return Redirect("/Admin");
            }
            page = page <=0 ? 1 : page;
            ViewData["page"] = page;
            ViewData["QuantitySelected"] = quantity;
            ViewData["CategorySelected"] = category;
            ViewData["StatusSelected"] = status;
            ViewData["NameSelected"] = name;
            var products = Display(quantity: quantity, category: category, status: status, name: name, numberPage: page);
            ViewBag.Products = products;
            ViewData["Brands"] = _context.Brands.ToList();
            ViewData["Categories"] = _context.Categories.ToList();
            var dataAttr = _context.Attributes.Where(a => a.ParentId == 0).ToList();
            ViewData["AllAttr"] = dataAttr;
            foreach (var data in dataAttr)
            {
                ViewData[data.Name] = _context.Attributes.Where(a => a.ParentId == data.Id);
            }

            return View(/*await fashionContext.ToListAsync()*/);
        }

        public async Task<IActionResult> FilterProduct(int quantity, int ?category, int ?status, string ?name)
        {
            var products = Display(quantity, category, status, name);

            // Return the products as JSON
            return Json(products);
        }

        //public List<dynamic> Display(int quantity, int ?category, int ?status, string ?name)
        //{
        //    var query = _context.Categories
        //    .Include(c => c.LnkProductCategories)
        //        .ThenInclude(lnk => lnk.Product)
        //    .AsQueryable();

        //    if (category != 0)
        //    {
        //        query = query.Where(c => c.Id == category);
        //    }

        //    if (status != 3)
        //    {
        //        if (status == 2)
        //        {
        //            query = query.Where(c => c.LnkProductCategories.Any(lnk => lnk.Product.Deleted == 1));
        //        }
        //        else
        //        {
        //            query = query.Where(c => c.LnkProductCategories.Any(lnk => lnk.Product.Status == status));
        //        }
        //    }

        //    if (name != null)
        //    {
        //        query = query.Where(c => c.LnkProductCategories.Any(lnk => lnk.Product.Name.Contains(name)));
        //    }

        //    var products = query
        //                   .SelectMany(c => c.LnkProductCategories.Select(lnk => new
        //                   {
        //                       Id = lnk.Product.Id,
        //                       ProductName = lnk.Product.Name,
        //                       ProductCategories = c.Name,
        //                       ProductPrice = lnk.Product.Price,
        //                       ProductDate = lnk.Product.UpdatedAt,
        //                       ProductStatus = lnk.Product.Status,
        //                       ProductDeleted = lnk.Product.Deleted,
        //                   }))
        //                   .Take(quantity)
        //                   .ToList();
        //    return products.Cast<dynamic>().ToList();
        //}
        public List<dynamic> Display(int quantity, int? category, int? status, string? name, int numberPage = 1)
        {
            var pageSize = quantity; // Số lượng sản phẩm trên mỗi trang
            var skipAmount =  (numberPage - 1) * pageSize <= 0 ? 0 : (numberPage - 1) * pageSize;

            // Số lượng sản phẩm cần bỏ qua để bắt đầu từ trang được chỉ định

            var query = from categoryEntity in _context.Categories
                        join lnkProductCategory in _context.LnkProductCategories
                            on categoryEntity.Id equals lnkProductCategory.CategoryId
                        join productEntity in _context.Products
                            on lnkProductCategory.ProductId equals productEntity.Id
                        select new
                        {
                            Id = productEntity.Id,
                            ProductName = productEntity.Name,
                            ProductCategories = categoryEntity.Name,
                            ProductPrice = productEntity.Price,
                            ProductDate = productEntity.UpdatedAt,
                            ProductStatus = productEntity.Status,
                            ProductDeleted = productEntity.Deleted,
                            CategoryId = categoryEntity.Id,
                        };


            if (category.HasValue && category != 0)
            {
                query = query.Where(result => result.CategoryId == category.Value);
            }

            if (status.HasValue && status != 3)
            {
                if (status == 2)
                {
                    query = query.Where(result => result.ProductDeleted == 1);
                }
                else
                {
                    query = query.Where(result => result.ProductStatus == status);
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(result => result.ProductName.Contains(name));
            }

            var products = query.Skip(skipAmount).Take(quantity).ToList();
            return products.Cast<dynamic>().ToList();
        }




        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = _context.Brands.ToList();
            ViewData["CategoryId"] = _context.Categories.ToList();


            var product = await _context.Products
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.Product = product;
            List<string> imgList = product.Img.Split(',').ToList();
            ViewBag.Img = imgList;
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == product.BrandId);
            ViewBag.Brand = brand;
            var categories = (from category in _context.Categories
                              join link in _context.LnkProductCategories
                              on category.Id equals link.CategoryId
                              where link.ProductId == id
                              select category)
                 .ToList();
            ViewBag.Categories = categories;
            var dataAttr = _context.Attributes.Where(a => a.ParentId == 0).ToList();
            ViewData["AllAttr"] = dataAttr;
            foreach (var data in dataAttr)
            {
                ViewData[data.Name] = _context.LnkProductAttributes
                                    .Include(lnk => lnk.Attribute)
                                    .Where(lnk => lnk.ProductId == id && lnk.Attribute.ParentId == data.Id)
                                    .Select(lnk => lnk.Attribute)
                                    .ToList();
            }

            //foreach (var item in dataAttr)
            //{
            //    ViewData[item.Name] = _context.Attributes.Where(a => a.ParentId == item.Id);
            //}
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Products/Create
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

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = _context.Brands.ToList();
            ViewData["CategoryId"] = _context.Categories.ToList();
            var dataAttr = _context.Attributes.Where(a => a.ParentId == 0).ToList();
            ViewData["AllAttr"] = dataAttr;
            foreach (var data in dataAttr)
            {
                ViewData[data.Name] = _context.Attributes.Where(a => a.ParentId == data.Id);
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Edit/5
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
            // các tham số có sẵn trong Product rồi thì thôi

            // lấy ra các  tham số truyền lên bằng name không có trong modal

            var files = HttpContext.Request.Form.Files;
            string category = HttpContext.Request.Form["category"];
            var dataAttr = _context.Attributes.Where(a => a.ParentId == 0).ToList();
            Dictionary<string, string> listInputAttr = new Dictionary<string, string>();
            foreach (var data in dataAttr)
            {
                listInputAttr.Add(data.Name, HttpContext.Request.Form[data.Name]);
            }



            //if (ModelState.IsValid)
            //{
            try
            {
                string url = "";
                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        // Lưu tập tin vào ổ đĩa hoặc xử lý tùy ý
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        url += "uploads/" + fileName + ",";
                    }
                }
                if (url != "")
                {
                    product.Img = url.Trim(',');
                }
                _context.Update(product);
                await _context.SaveChangesAsync();

                var oldLnkProductCategories = _context.LnkProductCategories.Where(lnk => lnk.ProductId == id).ToList();
                _context.LnkProductCategories.RemoveRange(oldLnkProductCategories);
                await _context.SaveChangesAsync();


                //var ProductId = product.Id;
                var ProductId = id;
                string[] parts = category.Split(',');

                var lnkProductCategory = new List<LnkProductCategory>();

                for (int i = 0; i < parts.Length; i++)
                {
                    var lnk = new LnkProductCategory();
                    lnk.ProductId = id;
                    lnk.CategoryId = Convert.ToInt32(parts[i]);
                    lnkProductCategory.Add(lnk);
                }
                _context.LnkProductCategories.AddRange(lnkProductCategory);
                _context.SaveChanges();

                var oldProductAttribute = _context.LnkProductAttributes.Where(lnk => lnk.ProductId == id).ToList();
                _context.LnkProductAttributes.RemoveRange(oldProductAttribute);
                _context.SaveChanges();

                //var lnkProductAttribute = new List<LnkProductAttribute>();

                //foreach (var key in listInputAttr.Keys)
                //{
                //    var value = listInputAttr[key];
                //    string[] attrValues = value.Split(',');
                //    for (int i = 0; i < attrValues.Length; i++)
                //    {
                //        var lnkAttr = new LnkProductAttribute();
                //        lnkAttr.ProductId = id;
                //        lnkAttr.AttributeId = Convert.ToInt32(attrValues[i]);
                //        lnkProductAttribute.Add(lnkAttr);
                //    }
                //}


                var lnkProductAttribute = new List<LnkProductAttribute>();

                foreach (var key in listInputAttr.Keys)
                {
                    var value = listInputAttr[key];
                    string[] attrValues = value.Split(',');
                    for (int i = 0; i < attrValues.Length; i++)
                    {
                        var lnkAttr = new LnkProductAttribute();
                        lnkAttr.ProductId = ProductId;
                        lnkAttr.AttributeId = Convert.ToInt32(attrValues[i]);
                        lnkProductAttribute.Add(lnkAttr);
                    }
                }

                _context.LnkProductAttributes.AddRange(lnkProductAttribute);
                _context.SaveChanges();
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
            //}
            return View(product);
        }


        // GET: Admin/Products/Delete/5
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

        // POST: Admin/Products/Delete/5
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Test(Product product, List<IFormFile> files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Xử lý tải lên file (nếu cần)
        //        foreach (var file in files)
        //        {
        //            // Lưu file vào thư mục hoặc lưu trữ tùy thuộc vào yêu cầu
        //        }

        //        // Lưu sản phẩm vào cơ sở dữ liệu
        //        _context.Products.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Test(IFormCollection form)
        //{
        //    string s = "abc";
        //    var files = HttpContext.Request.Form.Files;
        //    string tenTest = form["ten_test"];
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public async Task<IActionResult> Test()
        {
            // Xử lý dữ liệu của product và file_upload ở đây
            var name = HttpContext.Request.Form["name"];
            var slug = HttpContext.Request.Form["slug"];
            var abstracts = HttpContext.Request.Form["abstract"];
            var desc = HttpContext.Request.Form["desc"];
            var price = HttpContext.Request.Form["price"];
            var quantity = HttpContext.Request.Form["quantity"];
            var brand = HttpContext.Request.Form["brands"];
            string category = HttpContext.Request.Form["categories"];
            var files = HttpContext.Request.Form.Files;

            //var size = HttpContext.Request.Form["size"];
            //var color = HttpContext.Request.Form["color"];
            var dataAttr = _context.Attributes.Where(a => a.ParentId == 0).ToList();
            Dictionary<string, string> listInputAttr = new Dictionary<string, string>();
            foreach (var data in dataAttr)
            {
                listInputAttr.Add(data.Name, HttpContext.Request.Form[data.Name]);
            }



            // lưu dữ liệu vào db
            // cần lưu những bảng nào:
            // 1. Product, 2.  lnk_product_attribute, 3. lnk_product_category
            // lưu bảng product
            // nên validate dữ liệu trước khi lưu => bạn tự validate
            var url = "";
            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    // Lưu tập tin vào ổ đĩa hoặc xử lý tùy ý
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    url += "Uploads/" + fileName + ",";
                }
            }
            var product = new Product();
            product.Name = name;
            product.Slug = slug;
            product.Abstract = abstracts;
            product.Description = desc;
            product.Price = Convert.ToInt32(price);
            product.Quantity = Convert.ToInt32(quantity);
            product.BrandId = Convert.ToInt32(brand);
            product.Img = url;

            _context.Add(product);
            _context.SaveChanges();

            // lưu bảng lnk_product_attribute và lnk_product_category
            var ProductId = product.Id;
            string[] parts = category.Split(',');

            var lnkProductCategory = new List<LnkProductCategory>();

            for (int i = 0; i < parts.Length; i++)
            {
                var lnk = new LnkProductCategory();
                lnk.ProductId = ProductId;
                lnk.CategoryId = Convert.ToInt32(parts[i]);
                lnkProductCategory.Add(lnk);
            }
            _context.LnkProductCategories.AddRange(lnkProductCategory);
            _context.SaveChanges();

            var lnkProductAttribute = new List<LnkProductAttribute>();

            foreach (var key in listInputAttr.Keys)
            {
                var value = listInputAttr[key];
                string[] attrValues = value.Split(',');
                for (int i = 0; i < attrValues.Length; i++)
                {
                    var lnkAttr = new LnkProductAttribute();
                    lnkAttr.ProductId = ProductId;
                    lnkAttr.AttributeId = Convert.ToInt32(attrValues[i]);
                    lnkProductAttribute.Add(lnkAttr);
                }
            }
            _context.LnkProductAttributes.AddRange(lnkProductAttribute);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public string Test1(int quantity = 0, int category = 0, int status = 3)
        {
            //var query = _context.Categories
            //.Include(c => c.LnkProductCategories)
            //    .ThenInclude(lnk => lnk.Product)
            //.AsQueryable();

            //if (category != 0)
            //{
            //    query = query.Where(c => c.Id == category);
            //}

            //if (status != 3)
            //{
            //    query = query.Where(c => c.LnkProductCategories.Any(lnk => lnk.Product.Status == status));
            //}

            ////if (name != null)
            ////{
            ////    query = query.Where(c => c.LnkProductCategories.Any(lnk => lnk.Product.Name.Contains(name)));
            ////}

            //var products = query
            //                .SelectMany(c => c.LnkProductCategories.Select(lnk => new {
            //                    ProductId = lnk.Product.Id,
            //                    ProductName = lnk.Product.Name,
            //                    CategoryName = c.Name,
            //                    ProductPrice = lnk.Product.Price,
            //                    ProductDate = lnk.Product.UpdatedAt,
            //                    ProductStatus = lnk.Product.Status,
            //                    ProductDeleted = lnk.Product.Deleted,
            //                }))
            //                .Select(p => p.ProductId) // Selecting only the ProductId
            //                .Distinct() // Selecting distinct ProductIds
            //                .Take(quantity)
            //                .ToQueryString();

            var query = _context.Categories
           .Include(c => c.LnkProductCategories)
               .ThenInclude(lnk => lnk.Product)
           .AsQueryable();

            if (category != 0)
            {
                query = query.Where(c => c.Id == category);
            }

            if (status != 3)
            {
                if (status == 2)
                {
                    query = query.Where(c => c.LnkProductCategories.Any(lnk => lnk.Product.Deleted == 1));
                }
                else
                {
                    query = query.Where(c => c.LnkProductCategories.Any(lnk => lnk.Product.Status == status));
                }
            }

            //if (name != null)
            //{
            //    query = query.Where(c => c.LnkProductCategories.Any(lnk => lnk.Product.Name.Contains(name)));
            //}

            var products = query
                           .SelectMany(c => c.LnkProductCategories.Select(lnk => new {
                               ProductId = lnk.Product.Id,
                               ProductName = lnk.Product.Name,
                               CategoryName = c.Name,
                               ProductPrice = lnk.Product.Price,
                               ProductDate = lnk.Product.UpdatedAt,
                               ProductStatus = lnk.Product.Status,
                               ProductDeleted = lnk.Product.Deleted,
                           }))
                           .Take(quantity)
                           .ToQueryString();


            return products;
        }


    }

}