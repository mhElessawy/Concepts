using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index()
        {
            var items = await _context.StoreItems
                .Include(i => i.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(i => i.SubUOM)
                    .ThenInclude(su => su.UOM)
                .Include(i => i.Country)
                .Include(i => i.User)
                .OrderBy(i => i.ItemCode)
                .ToListAsync();

            return View(items);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreItem item)
        {
            // ⚠️ إزالة Navigation Properties من ModelState
            ModelState.Remove("SubCategory");
            ModelState.Remove("Country");
            ModelState.Remove("SubUOM");
            ModelState.Remove("User");

            // 🔍 طباعة الأخطاء للتشخيص
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // التحقق من عدم تكرار ItemCode
                    if (await _context.StoreItems.AnyAsync(i => i.ItemCode == item.ItemCode))
                    {
                        ModelState.AddModelError("ItemCode", "Item Code already exists");
                        LoadDropdowns(item);
                        return View(item);
                    }

                    item.CreatedDate = DateTime.Now;
                    item.ModifiedDate = DateTime.Now;

                    // تأكد أن Navigation Properties = null
                    item.SubCategory = null;
                    item.Country = null;
                    item.SubUOM = null;
                    item.User = null;

                    _context.Add(item);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Item created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Database Error: {ex.InnerException?.Message}");
                    ModelState.AddModelError("", $"Error saving item: {ex.InnerException?.Message ?? ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            LoadDropdowns(item);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.StoreItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            LoadDropdowns(item);
            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreItem item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            ModelState.Remove("SubCategory");
            ModelState.Remove("Country");
            ModelState.Remove("SubUOM");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _context.StoreItems.AnyAsync(i => i.ItemCode == item.ItemCode && i.Id != item.Id))
                    {
                        ModelState.AddModelError("ItemCode", "Item Code already exists");
                        LoadDropdowns(item);
                        return View(item);
                    }

                    item.ModifiedDate = DateTime.Now;
                    item.SubCategory = null;
                    item.Country = null;
                    item.SubUOM = null;
                    item.User = null;

                    _context.Update(item);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Item updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", $"Error updating item: {ex.Message}");
                }
            }

            LoadDropdowns(item);
            return View(item);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.StoreItems
                .Include(i => i.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(i => i.SubUOM)
                    .ThenInclude(su => su.UOM)
                .Include(i => i.Country)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.StoreItems
                .Include(i => i.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(i => i.SubUOM)
                    .ThenInclude(su => su.UOM)
                .Include(i => i.Country)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

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
                _context.StoreItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Item deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ AJAX Method: Get SubCategories by Category
        [HttpGet]
        public JsonResult GetSubCategoriesByCategory(int categoryId)
        {
            var subCategories = _context.DeffSubCategories
                .Where(sc => sc.CategoryId == categoryId && sc.Active)
                .OrderBy(sc => sc.Name)
                .Select(sc => new
                {
                    id = sc.Id,
                    name = sc.Name
                })
                .ToList();

            return Json(subCategories);
        }

        // ✅ AJAX Method: Get SubUOMs by UOM
        [HttpGet]
        public JsonResult GetSubUOMsByUOM(int uomId)
        {
            var subUOMs = _context.DefSubUOMs
                .Where(su => su.UOMId == uomId && su.Active)
                .OrderBy(su => su.SubUOMName)
                .Select(su => new
                {
                    id = su.Id,
                    subUOMName = su.SubUOMName
                })
                .ToList();

            return Json(subUOMs);
        }

        private bool ItemExists(int id)
        {
            return _context.StoreItems.Any(e => e.Id == id);
        }

        // Helper method لتحميل Dropdowns
        private void LoadDropdowns(StoreItem item = null)
        {
            // Categories
            ViewBag.Categories = _context.DeffCategories
                .Where(c => c.Active)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            // SubCategories (كل SubCategories مع معلومات Category)
            ViewBag.SubCategories = _context.DeffSubCategories
                .Include(sc => sc.Category)
                .Where(sc => sc.Active)
                .OrderBy(sc => sc.Category!.Name)
                .ThenBy(sc => sc.Name)
                .ToList();

            // UOMs
            ViewBag.UOMs = _context.DefUOMs
                .Where(u => u.Active)
                .OrderBy(u => u.UOMName)
                .Select(u => new { u.Id, u.UOMName })
                .ToList();

            // SubUOMs (كل SubUOMs مع معلومات UOM)
            ViewBag.SubUOMs = _context.DefSubUOMs
                .Include(su => su.UOM)
                .Where(su => su.Active)
                .OrderBy(su => su.UOM!.UOMName)
                .ThenBy(su => su.SubUOMName)
                .ToList();

            // Countries
            ViewBag.Countries = _context.DeffCountries
                .Where(c => c.Active)
                .OrderBy(c => c.Name)
                .ToList();

            // Users
            ViewBag.Users = _context.UserInfos
                .Where(u => u.Active)
                .OrderBy(u => u.FullName)
                .ToList();
        }
    }
}
