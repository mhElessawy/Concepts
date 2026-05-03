using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class CostCentersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CostCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CostCenters
        public async Task<IActionResult> Index()
        {
            var costCenters = await _context.DeffCostCenters
                .Include(cc => cc.Parent)
                .OrderBy(cc => cc.CostCenterCode)
                .ToListAsync();
            return View(costCenters);
        }

        // GET: CostCenters/Create
        public IActionResult Create()
        {
            LoadParentDropdown();
            return View();
        }

        // POST: CostCenters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeffCostCenter costCenter)
        {
            ModelState.Remove("Parent");
            ModelState.Remove("Children");

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _context.DeffCostCenters.AnyAsync(cc => cc.CostCenterCode == costCenter.CostCenterCode))
                    {
                        ModelState.AddModelError("CostCenterCode", "Cost Center Code already exists.");
                        LoadParentDropdown(costCenter.ParentId);
                        return View(costCenter);
                    }

                    costCenter.Parent = null;
                    costCenter.Children = null;
                    costCenter.CreatedDate = DateTime.Now;
                    costCenter.ModifiedDate = DateTime.Now;

                    _context.Add(costCenter);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cost Center created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving: " + ex.Message);
                }
            }

            LoadParentDropdown(costCenter.ParentId);
            return View(costCenter);
        }

        // GET: CostCenters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var costCenter = await _context.DeffCostCenters
                .Include(cc => cc.Parent)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (costCenter == null) return NotFound();

            LoadParentDropdown(costCenter.ParentId, costCenter.Id);
            return View(costCenter);
        }

        // POST: CostCenters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeffCostCenter costCenter)
        {
            if (id != costCenter.Id) return NotFound();

            ModelState.Remove("Parent");
            ModelState.Remove("Children");

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _context.DeffCostCenters.AnyAsync(cc => cc.CostCenterCode == costCenter.CostCenterCode && cc.Id != costCenter.Id))
                    {
                        ModelState.AddModelError("CostCenterCode", "Cost Center Code already exists.");
                        LoadParentDropdown(costCenter.ParentId, costCenter.Id);
                        return View(costCenter);
                    }

                    costCenter.Parent = null;
                    costCenter.Children = null;
                    costCenter.ModifiedDate = DateTime.Now;

                    _context.Update(costCenter);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cost Center updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DeffCostCenters.Any(cc => cc.Id == id)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating: " + ex.Message);
                }
            }

            LoadParentDropdown(costCenter.ParentId, costCenter.Id);
            return View(costCenter);
        }

        // GET: CostCenters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var costCenter = await _context.DeffCostCenters
                .Include(cc => cc.Parent)
                .Include(cc => cc.Children)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (costCenter == null) return NotFound();

            return View(costCenter);
        }

        // GET: CostCenters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var costCenter = await _context.DeffCostCenters
                .Include(cc => cc.Parent)
                .Include(cc => cc.Children)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (costCenter == null) return NotFound();

            return View(costCenter);
        }

        // POST: CostCenters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var costCenter = await _context.DeffCostCenters
                .Include(cc => cc.Children)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (costCenter != null)
            {
                if (costCenter.Children.Any())
                {
                    TempData["ErrorMessage"] = "Cannot delete: this Cost Center has child records.";
                    return RedirectToAction(nameof(Index));
                }

                _context.DeffCostCenters.Remove(costCenter);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cost Center deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Get Cost Center info by ID
        [HttpGet]
        public async Task<JsonResult> GetCostCenterInfo(int id)
        {
            var cc = await _context.DeffCostCenters
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cc == null) return Json(null);

            return Json(new
            {
                id = cc.Id,
                code = cc.CostCenterCode,
                name = cc.CostCenterName,
                parentCode = cc.Parent?.CostCenterCode ?? "",
                parentName = cc.Parent?.CostCenterName ?? ""
            });
        }

        // AJAX: Get Cost Center name by ID (used in Child Account screen)
        [HttpGet]
        public async Task<JsonResult> GetCostCenterName(int id)
        {
            var cc = await _context.DeffCostCenters.FindAsync(id);
            if (cc == null) return Json(new { name = "" });
            return Json(new { name = cc.CostCenterName });
        }

        // AJAX: Build tree data for the left panel
        [HttpGet]
        public async Task<JsonResult> GetTree()
        {
            var all = await _context.DeffCostCenters
                .OrderBy(cc => cc.CostCenterCode)
                .Select(cc => new
                {
                    id = cc.Id,
                    text = cc.CostCenterCode + " - " + cc.CostCenterName,
                    parentId = cc.ParentId,
                    active = cc.Active
                })
                .ToListAsync();

            return Json(all);
        }

        private void LoadParentDropdown(int? selectedId = null, int? excludeId = null)
        {
            var query = _context.DeffCostCenters.AsQueryable();
            if (excludeId.HasValue)
                query = query.Where(cc => cc.Id != excludeId.Value);

            ViewBag.Parents = query
                .OrderBy(cc => cc.CostCenterCode)
                .Select(cc => new { cc.Id, Display = cc.CostCenterCode + " - " + cc.CostCenterName })
                .ToList();

            ViewBag.SelectedParentId = selectedId;
        }
    }
}
