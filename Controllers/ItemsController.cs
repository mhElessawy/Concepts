using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index(string searchString)
        {
            var items = _context.StoreItems
                .Include(i => i.SubCategory)
                .ThenInclude(sc => sc.Category)
                .Include(i => i.SubUOM)
                .Where(i => i.Active);

            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.ItemName.Contains(searchString) || s.ItemCode.Contains(searchString));
            }

            return View(await items.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.StoreItems
                .Include(i => i.SubCategory)
                .ThenInclude(sc => sc.Category)
                .Include(i => i.SubUOM)
                .Include(i => i.City)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();
            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewBag.SubCategories = _context.DeffSubCategories
                .Include(sc => sc.Category)
                .Where(sc => sc.Active)
                .ToList();
            ViewBag.SubUOMs = _context.DefSubUOMs
                .Include(su => su.UOM)
                .Where(su => su.Active)
                .ToList();
            ViewBag.Cities = _context.DeffCities.Where(c => c.Active).ToList();

            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreItem item)
        {
            if (ModelState.IsValid)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubCategories = _context.DeffSubCategories.Include(sc => sc.Category).Where(sc => sc.Active).ToList();
            ViewBag.SubUOMs = _context.DefSubUOMs.Include(su => su.UOM).Where(su => su.Active).ToList();
            ViewBag.Cities = _context.DeffCities.Where(c => c.Active).ToList();

            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.StoreItems.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.SubCategories = _context.DeffSubCategories.Include(sc => sc.Category).Where(sc => sc.Active).ToList();
            ViewBag.SubUOMs = _context.DefSubUOMs.Include(su => su.UOM).Where(su => su.Active).ToList();
            ViewBag.Cities = _context.DeffCities.Where(c => c.Active).ToList();

            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreItem item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    item.ModifiedDate = DateTime.Now;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubCategories = _context.DeffSubCategories.Include(sc => sc.Category).Where(sc => sc.Active).ToList();
            ViewBag.SubUOMs = _context.DefSubUOMs.Include(su => su.UOM).Where(su => su.Active).ToList();
            ViewBag.Cities = _context.DeffCities.Where(c => c.Active).ToList();

            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.StoreItems
                .Include(i => i.SubCategory)
                .ThenInclude(sc => sc.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.StoreItems.FindAsync(id);
            if (item != null)
            {
                item.Active = false;
                item.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.StoreItems.Any(e => e.Id == id);
        }
    }
}