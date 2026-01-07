using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;
using System.Text.Json;

namespace Concept.Controllers
{
    public class PurchaseRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.PurchaseRequestHeaders
                .Include(p => p.Department)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .OrderByDescending(p => p.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        // GET: PurchaseRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseRequestHeaders
                .Include(p => p.Department)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            // جلب الـ Details
            var details = await _context.PurchaseRequestDetails
                .Include(d => d.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(d => d.Item)
                .Include(d => d.SubUOM)
                    .ThenInclude(su => su.UOM)
                .Where(d => d.PurchaseRequestHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;

            return View(header);
        }

        // GET: PurchaseRequests/Create
        public IActionResult Create()
        {
            LoadDropdowns();

            // توليد RequestCode تلقائي
            var lastRequest = _context.PurchaseRequestHeaders
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastRequest != null && !string.IsNullOrEmpty(lastRequest.RequestCode))
            {
                var parts = lastRequest.RequestCode.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                {
                    nextNumber = num + 1;
                }
            }

            ViewBag.NextRequestCode = $"PR-{nextNumber:D4}";
            ViewBag.CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.CurrentTime = DateTime.Now.ToString("HH:mm");

            return View();
        }

        // POST: PurchaseRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseRequestHeader header, string detailsJson)
        {
            try
            {
                // إزالة Navigation Properties من ModelState
                ModelState.Remove("Department");
                ModelState.Remove("User");
                ModelState.Remove("Vender");

                if (ModelState.IsValid)
                {
                    // التحقق من عدم تكرار RequestCode
                    if (await _context.PurchaseRequestHeaders.AnyAsync(p => p.RequestCode == header.RequestCode))
                    {
                        TempData["ErrorMessage"] = "Request Code already exists";
                        LoadDropdowns();
                        return View(header);
                    }

                    // حفظ Header
                    header.CreatedDate = DateTime.Now;
                    header.ModifiedDate = DateTime.Now;
                    header.Department = null;
                    header.User = null;
                    header.Vender = null;

                    _context.Add(header);
                    await _context.SaveChangesAsync();

                    // حفظ Details
                    if (!string.IsNullOrEmpty(detailsJson))
                    {
                        var details = JsonSerializer.Deserialize<List<PurchaseRequestDetails>>(detailsJson);

                        if (details != null && details.Any())
                        {
                            foreach (var detail in details)
                            {
                                detail.PurchaseRequestHeaderId = header.Id;
                                detail.PurchaseRequestHeader = null;
                                detail.SubCategory = null;
                                detail.Item = null;
                                detail.SubUOM = null;

                                _context.PurchaseRequestDetails.Add(detail);
                            }

                            await _context.SaveChangesAsync();
                        }
                    }

                    TempData["SuccessMessage"] = "Purchase Request created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                LoadDropdowns();
                return View(header);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating purchase request: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                TempData["ErrorMessage"] = "Error creating purchase request";
                LoadDropdowns();
                return View(header);
            }
        }

        // GET: PurchaseRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseRequestHeaders.FindAsync(id);
            if (header == null)
            {
                return NotFound();
            }

            // جلب الـ Details
            var details = await _context.PurchaseRequestDetails
                .Where(d => d.PurchaseRequestHeaderId == id)
                .ToListAsync();

            ViewBag.ExistingDetails = JsonSerializer.Serialize(details);
            LoadDropdowns(header);

            return View(header);
        }

        // POST: PurchaseRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseRequestHeader header, string detailsJson)
        {
            if (id != header.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Department");
            ModelState.Remove("User");
            ModelState.Remove("Vender");

            if (ModelState.IsValid)
            {
                try
                {
                    // التحقق من عدم تكرار RequestCode
                    if (await _context.PurchaseRequestHeaders.AnyAsync(p => p.RequestCode == header.RequestCode && p.Id != header.Id))
                    {
                        TempData["ErrorMessage"] = "Request Code already exists";
                        LoadDropdowns(header);
                        return View(header);
                    }

                    header.ModifiedDate = DateTime.Now;
                    header.Department = null;
                    header.User = null;
                    header.Vender = null;

                    _context.Update(header);

                    // حذف الـ Details القديمة
                    var oldDetails = await _context.PurchaseRequestDetails
                        .Where(d => d.PurchaseRequestHeaderId == id)
                        .ToListAsync();

                    _context.PurchaseRequestDetails.RemoveRange(oldDetails);

                    // إضافة الـ Details الجديدة
                    if (!string.IsNullOrEmpty(detailsJson))
                    {
                        var details = JsonSerializer.Deserialize<List<PurchaseRequestDetails>>(detailsJson);

                        if (details != null && details.Any())
                        {
                            foreach (var detail in details)
                            {
                                detail.Id = 0; // Reset ID for new records
                                detail.PurchaseRequestHeaderId = header.Id;
                                detail.PurchaseRequestHeader = null;
                                detail.SubCategory = null;
                                detail.Item = null;
                                detail.SubUOM = null;

                                _context.PurchaseRequestDetails.Add(detail);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Purchase Request updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseRequestExists(header.Id))
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
                    Console.WriteLine($"Error updating purchase request: {ex.Message}");
                    TempData["ErrorMessage"] = "Error updating purchase request";
                }
            }

            LoadDropdowns(header);
            return View(header);
        }

        // GET: PurchaseRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseRequestHeaders
                .Include(p => p.Department)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            // جلب الـ Details للعرض
            var details = await _context.PurchaseRequestDetails
                .Include(d => d.SubCategory)
                .Include(d => d.Item)
                .Include(d => d.SubUOM)
                .Where(d => d.PurchaseRequestHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;

            return View(header);
        }

        // POST: PurchaseRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var header = await _context.PurchaseRequestHeaders.FindAsync(id);
                if (header != null)
                {
                    // حذف الـ Details أولاً
                    var details = await _context.PurchaseRequestDetails
                        .Where(d => d.PurchaseRequestHeaderId == id)
                        .ToListAsync();

                    _context.PurchaseRequestDetails.RemoveRange(details);

                    // ثم حذف الـ Header
                    _context.PurchaseRequestHeaders.Remove(header);

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Purchase Request deleted successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting purchase request: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting purchase request";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX Methods for Cascading Dropdowns
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

        [HttpGet]
        public JsonResult GetItemsBySubCategory(int subCategoryId)
        {
            var items = _context.StoreItems
                .Where(i => i.SubCategoryId == subCategoryId && i.Active)
                .OrderBy(i => i.ItemName)
                .Select(i => new
                {
                    id = i.Id,
                    code = i.ItemCode,
                    name = i.ItemName,
                    packSize = i.PackSize
                })
                .ToList();

            return Json(items);
        }

        [HttpGet]
        public JsonResult GetSubUOMsByUOM(int uomId)
        {
            var subUOMs = _context.DefSubUOMs
                .Where(su => su.UOMId == uomId && su.Active)
                .OrderBy(su => su.SubUOMName)
                .Select(su => new
                {
                    id = su.Id,
                    name = su.SubUOMName
                })
                .ToList();

            return Json(subUOMs);
        }

        private bool PurchaseRequestExists(int id)
        {
            return _context.PurchaseRequestHeaders.Any(e => e.Id == id);
        }

        private void LoadDropdowns(PurchaseRequestHeader header = null)
        {
            // Departments
            ViewBag.Departments = new SelectList(
                _context.DeffDepartments.Where(d => d.Active).OrderBy(d => d.DepartmentName),
                "Id",
                "DepartmentName",
                header?.DepartmentId
            );

            // Users
            ViewBag.Users = new SelectList(
                _context.UserInfos.Where(u => u.Active).OrderBy(u => u.FullName),
                "Id",
                "FullName",
                header?.UserId
            );

            // Vendors
            ViewBag.Vendors = new SelectList(
                _context.Venders.Where(v => v.Active).OrderBy(v => v.VenderName),
                "Id",
                "VenderName",
                header?.VenderId
            );

            // Categories
            ViewBag.Categories = _context.DeffCategories
                .Where(c => c.Active)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            // UOMs
            ViewBag.UOMs = _context.DefUOMs
                .Where(u => u.Active)
                .OrderBy(u => u.UOMName)
                .Select(u => new { u.Id, u.UOMName })
                .ToList();

            // Status options
            ViewBag.StatusOptions = new SelectList(new[]
            {
                new { Value = 0, Text = "Pending" },
                new { Value = 1, Text = "Approved" },
                new { Value = 2, Text = "Rejected" }
            }, "Value", "Text", header?.RequestedStatus);

            // Approved options
            ViewBag.ApprovedOptions = new SelectList(new[]
            {
                new { Value = 0, Text = "Not Approved" },
                new { Value = 1, Text = "Approved" },
                new { Value = 2, Text = "Rejected" }
            }, "Value", "Text", header?.Approved);
        }
    }
}

