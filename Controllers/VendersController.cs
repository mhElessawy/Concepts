using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class VendersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VendersController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Venders
        public async Task<IActionResult> Index()
        {
            var vendors = await _context.Venders
                .Include(v => v.City)
                    .ThenInclude(c => c.Country)
                .Include(v => v.JobTitle)
                .Include(v => v.Bank)
                .Include(v => v.CostCenter)
                .OrderBy(v => v.VenderCode)
                .ToListAsync();

            return View(vendors);
        }
        // GET: Vendors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Venders
                .Include(v => v.City)
                    .ThenInclude(c => c.Country)
                .Include(v => v.JobTitle)
                .Include(v => v.Bank)
                .Include(v => v.CostCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }
        // GET: Vendors/Create
        public async Task<IActionResult> Create()
        {
            LoadDropdowns();
            var nextCode = await GenerateNextVenderCode();
            ViewBag.NextVenderCode = nextCode;
            return View();
        }
        // POST: Vendors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vender vendor)
        {
            // إزالة Navigation Properties من ModelState
            ModelState.Remove("City");
            ModelState.Remove("JobTitle");
            ModelState.Remove("Bank");
            ModelState.Remove("CostCenter");
            ModelState.Remove("VenderCode");
            if (ModelState.IsValid)
            {
                try
                {
                    // Auto-generate VenderCode
                    vendor.VenderCode = await GenerateNextVenderCode();

                    vendor.CreatedDate = DateTime.Now;
                    vendor.ModifiedDate = DateTime.Now;

                    // تأكد أن Navigation Properties = null
                    vendor.City = null;
                    vendor.JobTitle = null;
                    vendor.Bank = null;
                    vendor.CostCenter = null;

                    _context.Add(vendor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vendor created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.InnerException?.Message}");
                    ModelState.AddModelError("", "Error saving vendor");
                }
            }
            LoadDropdowns(vendor);
            ViewBag.NextVenderCode = await GenerateNextVenderCode();
            return View(vendor);
        }
        // GET: Vendors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Venders.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            LoadDropdowns(vendor);
            return View(vendor);
        }
        // POST: Vendors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vender vendor)
        {
            if (id != vendor.Id)
            {
                return NotFound();
            }

            ModelState.Remove("City");
            ModelState.Remove("JobTitle");
            ModelState.Remove("Bank");
            ModelState.Remove("CostCenter");

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _context.Venders.AnyAsync(v => v.VenderCode == vendor.VenderCode && v.Id != vendor.Id))
                    {
                        ModelState.AddModelError("VenderCode", "Vendor Code already exists");
                        LoadDropdowns(vendor);
                        return View(vendor);
                    }

                    vendor.ModifiedDate = DateTime.Now;
                    vendor.City = null;
                    vendor.JobTitle = null;
                    vendor.Bank = null;
                    vendor.CostCenter = null;

                    _context.Update(vendor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vendor updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorExists(vendor.Id))
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
                    ModelState.AddModelError("", "Error updating vendor");
                }
            }

            LoadDropdowns(vendor);
            return View(vendor);
        }
        // GET: Vendors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Venders
                .Include(v => v.City)
                    .ThenInclude(c => c.Country)
                .Include(v => v.JobTitle)
                .Include(v => v.Bank)
                .Include(v => v.CostCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }
        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendor = await _context.Venders.FindAsync(id);
            if (vendor != null)
            {
                _context.Venders.Remove(vendor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vendor deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
        // AJAX: Get Cities by Country
        [HttpGet]
        public JsonResult GetCitiesByCountry(int countryId)
        {
            var cities = _context.DeffCities
                .Where(c => c.CountryId == countryId && c.Active)
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name
                })
                .ToList();

            return Json(cities);
        }
        private bool VendorExists(int id)
        {
            return _context.Venders.Any(e => e.Id == id);
        }
        private void LoadDropdowns(Vender vendor = null)
        {
            // Countries
            ViewBag.Countries = _context.DeffCountries
                .Where(c => c.Active)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            // Cities (All)
            ViewBag.Cities = _context.DeffCities
                .Include(c => c.Country)
                .Where(c => c.Active)
                .OrderBy(c => c.Country.Name)
                .ThenBy(c => c.Name)
                .ToList();

            // Job Titles
            ViewBag.JobTitles = new SelectList(
                _context.DeffJobTitles.Where(j => j.Active).OrderBy(j => j.JobName),
                "Id",
                "JobName",
                vendor?.JobTitleId
            );

            // Banks
            ViewBag.Banks = new SelectList(
                _context.DefBanks.Where(b => b.Active).OrderBy(b => b.BankName),
                "Id",
                "BankName",
                vendor?.BankId
            );

            // Cost Centers
            ViewBag.CostCenters = new SelectList(
                _context.DeffCostCenters.Where(cc => cc.Active).OrderBy(cc => cc.CostCenterName),
                "Id",
                "CostCenterName",
                vendor?.CostCenterId
            );
        }

        private async Task<string> GenerateNextVenderCode()
        {
            var lastVender = await _context.Venders
                .OrderByDescending(v => v.VenderCode)
                .FirstOrDefaultAsync();

            if (lastVender == null)
                return "V001";

            // Extract numeric part from code like "V001"
            var code = lastVender.VenderCode;
            var numericPart = new string(code.Where(char.IsDigit).ToArray());

            if (int.TryParse(numericPart, out int lastNumber))
            {
                return $"V{(lastNumber + 1).ToString().PadLeft(3, '0')}";
            }

            // Fallback: count existing vendors + 1
            var count = await _context.Venders.CountAsync();
            return $"V{(count + 1).ToString().PadLeft(3, '0')}";
        }
    }
}