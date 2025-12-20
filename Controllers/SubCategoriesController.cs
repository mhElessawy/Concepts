using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubCategories
        public async Task<IActionResult> Index()
        {
            var subCategories = await _context.DeffSubCategories
                .Include(sc => sc.Category)
                .Where(sc => sc.Active)
                .OrderBy(sc => sc.Name)
                .ToListAsync();
            return View(subCategories);
        }

        // GET: SubCategories/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.DeffCategories.Where(c => c.Active).ToList();
            return View();
        }

        // POST: SubCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeffSubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                subCategory.CreatedDate = DateTime.Now;
                subCategory.ModifiedDate = DateTime.Now;
                _context.Add(subCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.DeffCategories.Where(c => c.Active).ToList();
            return View(subCategory);
        }

        // GET: SubCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subCategory = await _context.DeffSubCategories.FindAsync(id);
            if (subCategory == null) return NotFound();

            ViewBag.Categories = _context.DeffCategories.Where(c => c.Active).ToList();
            return View(subCategory);
        }

        // POST: SubCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeffSubCategory subCategory)
        {
            if (id != subCategory.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    subCategory.ModifiedDate = DateTime.Now;
                    _context.Update(subCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubCategoryExists(subCategory.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.DeffCategories.Where(c => c.Active).ToList();
            return View(subCategory);
        }

        // GET: SubCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var subCategory = await _context.DeffSubCategories
                .Include(sc => sc.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subCategory == null) return NotFound();

            return View(subCategory);
        }

        // POST: SubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _context.DeffSubCategories.FindAsync(id);
            if (subCategory != null)
            {
                subCategory.Active = false;
                subCategory.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubCategoryExists(int id)
        {
            return _context.DeffSubCategories.Any(e => e.Id == id);
        }
    }
}