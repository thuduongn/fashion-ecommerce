using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fashion.Data;
using Attribute = fashion.Data.Attribute;
using Microsoft.Extensions.Primitives;

namespace fashion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AttributesController : Controller
    {
        private readonly FashionContext _context;

        public AttributesController(FashionContext context)
        {
            _context = context;
        }

        // GET: Admin/Attributes
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("IdUser") == null)
            {
                return Redirect("/Admin");
            }
            return View(await _context.Attributes.ToListAsync());
        }

        // GET: Admin/Attributes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attribute = await _context.Attributes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attribute == null)
            {
                return NotFound();
            }

            return View(attribute);
        }

        // GET: Admin/Attributes/Create
        public IActionResult Create()
        {
            var ListAttribute = _context.Attributes.ToList();
            ViewBag.ListAttribute = ListAttribute;
            return View();
        }

        // POST: Admin/Attributes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Description, int ParentId)
        {
            if (ModelState.IsValid)
            {
                var attribute = new Attribute();
                attribute.Name = Name;
                attribute.Description = Description;
                attribute.ParentId = ParentId;
                _context.Add(attribute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Admin/Attributes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ListAttribute = _context.Attributes.ToList();
            ViewBag.ListAttribute = ListAttribute;
            var attribute = await _context.Attributes.FindAsync(id);
            if (attribute == null)
            {
                return NotFound();
            }
            return View(attribute);
        }

        // POST: Admin/Attributes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ParentId,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] Attribute attribute)
        {
            if (id != attribute.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attribute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttributeExists(attribute.Id))
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
            return View(attribute);
        }

        // GET: Admin/Attributes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attribute = await _context.Attributes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attribute == null)
            {
                return NotFound();
            }

            return View(attribute);
        }

        // POST: Admin/Attributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attribute = await _context.Attributes.FindAsync(id);
            if (attribute != null)
            {
                _context.Attributes.Remove(attribute);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttributeExists(int id)
        {
            return _context.Attributes.Any(e => e.Id == id);
        }

        private bool IsValidString(string value, int maxLength)
        {
            return value is StringValues && value.Length > 0 && value.Length <= maxLength;
        }

        [HttpPost]
        public async Task<IActionResult> Test()
        {
            var name = HttpContext.Request.Form["name"];
            var desc = HttpContext.Request.Form["desc"];

            if (!IsValidString(name, 50) ||
                !IsValidString(desc, 2000))
            {
                return NotFound();
            };
            return RedirectToAction("Index");
        }
    }
}
