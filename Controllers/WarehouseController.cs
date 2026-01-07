using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Warehouse
        public async Task<IActionResult> Index(string searchString)
        {
            var warehouses = _context.Warehouses
                .Include(w => w.Location)
                .Include(w => w.Country)
                .Include(w => w.User)
                .Where(w => w.Active)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                warehouses = warehouses.Where(w =>
                    w.WarehouseCode.Contains(searchString) ||
                    w.WarehouseName.Contains(searchString) ||
                    w.ManagerName.Contains(searchString));
            }

            var result = await warehouses
                .OrderBy(w => w.WarehouseName)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.TotalCount = await _context.Warehouses.CountAsync(w => w.Active);

            return View(result);
        }

        // GET: Warehouse/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .Include(w => w.Location)
                .Include(w => w.Country)
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // GET: Warehouse/Create
        public IActionResult Create()
        {
            LoadDropdowns();

            // Auto-generate Warehouse Code
            var lastWarehouse = _context.Warehouses
                .OrderByDescending(w => w.Id)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastWarehouse != null && !string.IsNullOrEmpty(lastWarehouse.WarehouseCode))
            {
                var parts = lastWarehouse.WarehouseCode.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                {
                    nextNumber = num + 1;
                }
            }

            ViewBag.NextWarehouseCode = $"WH-{nextNumber:D4}";

            return View();
        }

        // POST: Warehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Warehouse warehouse)
        {
            try
            {
                ModelState.Remove("Location");
                ModelState.Remove("User");
                ModelState.Remove("Country");
                ModelState.Remove("WarehouseCode");

                if (ModelState.IsValid)
                {
                    // Auto-generate Warehouse Code
                    var lastWarehouse = await _context.Warehouses
                        .OrderByDescending(w => w.Id)
                        .FirstOrDefaultAsync();

                    int nextNumber = 1;
                    if (lastWarehouse != null && !string.IsNullOrEmpty(lastWarehouse.WarehouseCode))
                    {
                        var parts = lastWarehouse.WarehouseCode.Split('-');
                        if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                        {
                            nextNumber = num + 1;
                        }
                    }

                    warehouse.WarehouseCode = $"WH-{nextNumber:D4}";
                    warehouse.CreatedDate = DateTime.Now;
                    warehouse.ModifiedDate = DateTime.Now;
                    warehouse.Location = null;
                    warehouse.User = null;
                    warehouse.Country = null;

                    _context.Add(warehouse);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Warehouse created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                LoadDropdowns();
                return View(warehouse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating warehouse: {ex.Message}");
                TempData["ErrorMessage"] = "Error creating warehouse";
                LoadDropdowns();
                return View(warehouse);
            }
        }

        // GET: Warehouse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            LoadDropdowns();
            return View(warehouse);
        }

        // POST: Warehouse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Warehouse warehouse)
        {
            if (id != warehouse.Id)
            {
                return NotFound();
            }

            try
            {
                ModelState.Remove("Location");
                ModelState.Remove("User");
                ModelState.Remove("Country");

                if (ModelState.IsValid)
                {
                    var existingWarehouse = await _context.Warehouses.FindAsync(id);
                    if (existingWarehouse == null)
                    {
                        return NotFound();
                    }

                    // Update properties
                    existingWarehouse.WarehouseName = warehouse.WarehouseName;
                    existingWarehouse.Active = warehouse.Active;
                    existingWarehouse.AccountId = warehouse.AccountId;
                    existingWarehouse.CostId = warehouse.CostId;
                    existingWarehouse.Description = warehouse.Description;
                    existingWarehouse.LocationId = warehouse.LocationId;
                    existingWarehouse.UserId = warehouse.UserId;
                    existingWarehouse.IVM = warehouse.IVM;
                    existingWarehouse.WarehouseType = warehouse.WarehouseType;
                    existingWarehouse.CountryId = warehouse.CountryId;
                    existingWarehouse.ManagerName = warehouse.ManagerName;
                    existingWarehouse.ManagerNumber = warehouse.ManagerNumber;
                    existingWarehouse.AdditionalNote = warehouse.AdditionalNote;
                    existingWarehouse.ModifiedDate = DateTime.Now;

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Warehouse updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                LoadDropdowns();
                return View(warehouse);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(warehouse.Id))
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
                Console.WriteLine($"Error updating warehouse: {ex.Message}");
                TempData["ErrorMessage"] = "Error updating warehouse";
                LoadDropdowns();
                return View(warehouse);
            }
        }

        // GET: Warehouse/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .Include(w => w.Location)
                .Include(w => w.Country)
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: Warehouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse != null)
                {
                    // Soft delete
                    warehouse.Active = false;
                    warehouse.ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Warehouse deleted successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting warehouse: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting warehouse. It may be in use.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.Id == id);
        }

        private void LoadDropdowns()
        {
            ViewBag.Locations = new SelectList(_context.DeffLocations
                .Where(l => l.Active)
                .OrderBy(l => l.LocationName), "Id", "LocationName");

            ViewBag.Countries = new SelectList(_context.DeffCountries
                .Where(c => c.Active)
                .OrderBy(c => c.Name), "Id", "Name");

            ViewBag.Users = new SelectList(_context.UserInfos
                .Where(u => u.Active)
                .OrderBy(u => u.FullName), "Id", "FullName");
        }
    }
}
