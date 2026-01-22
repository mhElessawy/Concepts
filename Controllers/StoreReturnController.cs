using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;
using System.Text.Json;

namespace Concept.Controllers
{
    public class StoreReturnController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreReturnController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if user is logged in
        private bool IsUserLoggedIn()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId != null && userId > 0;
        }

        // Get current user ID
        private int GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        // GET: StoreReturn
        public async Task<IActionResult> Index(string filter = "all")
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.StoreReturnHeaders
                .Include(r => r.User)
                .AsQueryable();

            // Apply filter
            IQueryable<StoreReturnHeader> filteredQuery = filter switch
            {
                "pending" => query.Where(r => r.ReturnStatus == 0),    // Pending
                "approved" => query.Where(r => r.ReturnStatus == 1),   // Approved
                "rejected" => query.Where(r => r.ReturnStatus == 2),   // Rejected
                _ => query  // all
            };

            var returns = await filteredQuery
                .OrderByDescending(r => r.ReturnDate)
                .ToListAsync();

            // Counts for badges
            ViewBag.AllCount = await _context.StoreReturnHeaders.CountAsync();
            ViewBag.PendingCount = await _context.StoreReturnHeaders.CountAsync(r => r.ReturnStatus == 0);
            ViewBag.ApprovedCount = await _context.StoreReturnHeaders.CountAsync(r => r.ReturnStatus == 1);
            ViewBag.RejectedCount = await _context.StoreReturnHeaders.CountAsync(r => r.ReturnStatus == 2);

            ViewBag.Filter = filter;

            return View(returns);
        }

        // GET: StoreReturn/PendingApproval
        public async Task<IActionResult> PendingApproval()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var pendingReturns = await _context.StoreReturnHeaders
                .Include(r => r.User)
                .Where(r => r.ReturnStatus == 0 && r.Active)
                .OrderByDescending(r => r.ReturnDate)
                .ToListAsync();

            ViewBag.PendingCount = pendingReturns.Count;

            return View(pendingReturns);
        }

        // GET: StoreReturn/Create
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            LoadDropdownData();
            return View();
        }

        // POST: StoreReturn/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreReturnHeader header, string detailsJson)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Set user and dates
                header.UserId = GetCurrentUserId();
                header.CreatedDate = DateTime.Now;
                header.ModifiedDate = DateTime.Now;
                header.Active = true;
                header.ReturnStatus = 0; // Pending

                // Generate Return Code and Number
                var lastReturn = await _context.StoreReturnHeaders
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefaultAsync();

                int nextNumber = (lastReturn?.ReturnNo ?? 0) + 1;
                header.ReturnNo = nextNumber;
                header.ReturnCode = $"RT-{DateTime.Now.Year}-{nextNumber.ToString().PadLeft(6, '0')}";

                // Add header
                _context.StoreReturnHeaders.Add(header);
                await _context.SaveChangesAsync();

                // Parse and add details
                if (!string.IsNullOrEmpty(detailsJson))
                {
                    var details = JsonSerializer.Deserialize<List<StoreReturnDetails>>(detailsJson);
                    if (details != null && details.Any())
                    {
                        foreach (var detail in details)
                        {
                            detail.StoreReturnHeaderId = header.Id;
                            _context.StoreReturnDetails.Add(detail);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["SuccessMessage"] = "تم إنشاء إرجاع المخزون بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطأ في إنشاء إرجاع المخزون: {ex.Message}";
                LoadDropdownData();
                return View(header);
            }
        }

        // GET: StoreReturn/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.StoreReturnHeaders
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            var details = await _context.StoreReturnDetails
                .Include(d => d.Category)
                .Include(d => d.SubCategory)
                .Include(d => d.Item)
                .Include(d => d.UOM)
                .Include(d => d.SubUOM)
                .Where(d => d.StoreReturnHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;

            return View(header);
        }

        // GET: StoreReturn/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.StoreReturnHeaders.FindAsync(id);
            if (header == null)
            {
                return NotFound();
            }

            // Check if return is already approved or rejected
            if (header.ReturnStatus != 0)
            {
                TempData["ErrorMessage"] = "لا يمكن تعديل إرجاع تم الموافقة عليه أو رفضه";
                return RedirectToAction(nameof(Details), new { id });
            }

            var details = await _context.StoreReturnDetails
                .Where(d => d.StoreReturnHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;
            LoadDropdownData();

            return View(header);
        }

        // POST: StoreReturn/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreReturnHeader header, string detailsJson)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != header.Id)
            {
                return NotFound();
            }

            try
            {
                var existingHeader = await _context.StoreReturnHeaders.FindAsync(id);
                if (existingHeader == null)
                {
                    return NotFound();
                }

                // Check if return is already approved or rejected
                if (existingHeader.ReturnStatus != 0)
                {
                    TempData["ErrorMessage"] = "لا يمكن تعديل إرجاع تم الموافقة عليه أو رفضه";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Update header
                existingHeader.ReturnType = header.ReturnType;
                existingHeader.ReturnDate = header.ReturnDate;
                existingHeader.ReturnTime = header.ReturnTime;
                existingHeader.FromWarehouseId = header.FromWarehouseId;
                existingHeader.FromDepartmentId = header.FromDepartmentId;
                existingHeader.ToWarehouseId = header.ToWarehouseId;
                existingHeader.ToDepartmentId = header.ToDepartmentId;
                existingHeader.AdditionalNotes = header.AdditionalNotes;
                existingHeader.ModifiedDate = DateTime.Now;

                // Delete existing details
                var existingDetails = await _context.StoreReturnDetails
                    .Where(d => d.StoreReturnHeaderId == id)
                    .ToListAsync();
                _context.StoreReturnDetails.RemoveRange(existingDetails);

                // Add new details
                if (!string.IsNullOrEmpty(detailsJson))
                {
                    var details = JsonSerializer.Deserialize<List<StoreReturnDetails>>(detailsJson);
                    if (details != null && details.Any())
                    {
                        foreach (var detail in details)
                        {
                            detail.StoreReturnHeaderId = id;
                            _context.StoreReturnDetails.Add(detail);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم تحديث إرجاع المخزون بنجاح!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطأ في تحديث إرجاع المخزون: {ex.Message}";
                LoadDropdownData();
                return View(header);
            }
        }

        // POST: StoreReturn/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var header = await _context.StoreReturnHeaders.FindAsync(id);
                if (header == null)
                {
                    return NotFound();
                }

                // Check if return is already approved
                if (header.ReturnStatus == 1)
                {
                    TempData["ErrorMessage"] = "لا يمكن حذف إرجاع تمت الموافقة عليه";
                    return RedirectToAction(nameof(Index));
                }

                // Soft delete
                header.Active = false;
                header.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم حذف إرجاع المخزون بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطأ في حذف إرجاع المخزون: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: StoreReturn/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var header = await _context.StoreReturnHeaders.FindAsync(id);
                if (header == null)
                {
                    return NotFound();
                }

                // Check if already approved
                if (header.ReturnStatus == 1)
                {
                    TempData["ErrorMessage"] = "تم الموافقة على هذا الإرجاع مسبقاً!";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Get all return details
                var details = await _context.StoreReturnDetails
                    .Where(d => d.StoreReturnHeaderId == id)
                    .ToListAsync();

                // Update inventory for each item
                foreach (var detail in details)
                {
                    var item = await _context.StoreItems.FindAsync(detail.ItemId);
                    if (item != null)
                    {
                        // Subtract the returned quantity from store inventory
                        item.QuantityInStore -= detail.Quantity;

                        // Check if quantity goes negative
                        if (item.QuantityInStore < 0)
                        {
                            TempData["ErrorMessage"] = $"خطأ: كمية الإرجاع للصنف '{item.ItemName}' تتجاوز الكمية المتوفرة في المخزون!";
                            return RedirectToAction(nameof(Details), new { id });
                        }

                        item.ModifiedDate = DateTime.Now;
                        _context.Update(item);
                    }
                }

                header.ReturnStatus = 1; // Approved
                header.ApprovedBy = GetCurrentUserId();
                header.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تمت الموافقة على إرجاع المخزون بنجاح وتم تحديث الكميات!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطأ في الموافقة على إرجاع المخزون: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: StoreReturn/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var header = await _context.StoreReturnHeaders.FindAsync(id);
                if (header == null)
                {
                    return NotFound();
                }

                header.ReturnStatus = 2; // Rejected
                header.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم رفض إرجاع المخزون بنجاح!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطأ في رفض إرجاع المخزون: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // API Methods for AJAX calls

        // GET: StoreReturn/GetWarehouses
        [HttpGet]
        public async Task<JsonResult> GetWarehouses()
        {
            var warehouses = await _context.Warehouses
                .Where(w => w.Active)
                .Select(w => new { w.Id, w.WarehouseName })
                .ToListAsync();

            return Json(warehouses);
        }

        // GET: StoreReturn/GetDepartments
        [HttpGet]
        public async Task<JsonResult> GetDepartments()
        {
            var departments = await _context.DeffDepartments
                .Where(d => d.Active)
                .Select(d => new { d.Id, d.DepartmentName })
                .ToListAsync();

            return Json(departments);
        }

        // GET: StoreReturn/GetCategories
        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            var categories = await _context.DeffCategories
                .Where(c => c.Active)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            return Json(categories);
        }

        // GET: StoreReturn/GetSubCategoriesByCategory/5
        [HttpGet]
        public async Task<JsonResult> GetSubCategoriesByCategory(int categoryId)
        {
            var subCategories = await _context.DeffSubCategories
                .Where(sc => sc.CategoryId == categoryId && sc.Active)
                .Select(sc => new { sc.Id, sc.Name })
                .ToListAsync();

            return Json(subCategories);
        }

        // GET: StoreReturn/GetItemsBySubCategory/5
        [HttpGet]
        public async Task<JsonResult> GetItemsBySubCategory(int subCategoryId)
        {
            var items = await _context.StoreItems
                .Where(i => i.SubCategoryId == subCategoryId && i.Active)
                .Include(i => i.SubUOM)
                .Select(i => new
                {
                    i.Id,
                    i.ItemCode,
                    i.ItemName,
                    SubUOMId = i.SubUOMId ?? 0,
                    SubUOMName = i.SubUOM != null ? i.SubUOM.SubUOMName : ""
                })
                .ToListAsync();

            return Json(items);
        }

        // GET: StoreReturn/GetItemFullDetails/5
        [HttpGet]
        public async Task<JsonResult> GetItemFullDetails(int itemId)
        {
            var item = await _context.StoreItems
                .Where(i => i.Id == itemId)
                .Include(i => i.SubUOM)
                .ThenInclude(su => su.UOM)
                .Include(i => i.SubCategory)
                .ThenInclude(sc => sc.Category)
                .Select(i => new
                {
                    i.Id,
                    i.ItemCode,
                    i.ItemName,
                    CategoryId = i.SubCategory.CategoryId,
                    CategoryName = i.SubCategory.Category.Name,
                    SubCategoryId = i.SubCategoryId ?? 0,
                    SubCategoryName = i.SubCategory != null ? i.SubCategory.Name : "",
                    UOMId = i.SubUOM != null ? i.SubUOM.UOMId : 0,
                    UOMName = i.SubUOM != null && i.SubUOM.UOM != null ? i.SubUOM.UOM.UOMName : "",
                    SubUOMId = i.SubUOMId ?? 0,
                    SubUOMName = i.SubUOM != null ? i.SubUOM.SubUOMName : "",
                    i.PackSize,
                    i.QuantityInStore,
                    i.PurchaseValue
                })
                .FirstOrDefaultAsync();

            return Json(item);
        }

        // GET: StoreReturn/GetUOMs
        [HttpGet]
        public async Task<JsonResult> GetUOMs()
        {
            var uoms = await _context.DefUOMs
                .Where(u => u.Active)
                .Select(u => new { u.Id, u.UOMName })
                .ToListAsync();

            return Json(uoms);
        }

        // GET: StoreReturn/GetSubUOMsByUOM/5
        [HttpGet]
        public async Task<JsonResult> GetSubUOMsByUOM(int uomId)
        {
            var subUOMs = await _context.DefSubUOMs
                .Where(su => su.UOMId == uomId && su.Active)
                .Select(su => new { su.Id, su.SubUOMName })
                .ToListAsync();

            return Json(subUOMs);
        }

        // GET: StoreReturn/GetBatchesByItem/5
        [HttpGet]
        public async Task<JsonResult> GetBatchesByItem(int itemId)
        {
            var batches = await _context.PurchaseRecievedDetails
                .Where(p => p.ItemId == itemId)
                .Select(p => new
                {
                    BatchNo = p.PurchaseRecievedHeader.BatchNo.ToString(),
                    ReceivedDate = p.PurchaseRecievedHeader.RecieveDate,
                    ExpiredDate = p.ExpiredDate,
                    AvailableQuantity = p.RecieveQuantity
                })
                .Distinct()
                .OrderByDescending(b => b.ReceivedDate)
                .ToListAsync();

            return Json(batches);
        }

        // Helper method to load dropdown data
        private void LoadDropdownData()
        {
            ViewBag.Warehouses = new SelectList(
                _context.Warehouses.Where(w => w.Active).ToList(),
                "Id",
                "WarehouseName"
            );

            ViewBag.Departments = new SelectList(
                _context.DeffDepartments.Where(d => d.Active).ToList(),
                "Id",
                "DepartmentName"
            );

            ViewBag.Categories = new SelectList(
                _context.DeffCategories.Where(c => c.Active).ToList(),
                "Id",
                "Name"
            );

            ViewBag.SubCategories = new SelectList(
                _context.DeffSubCategories.Where(sc => sc.Active).ToList(),
                "Id",
                "Name"
            );

            ViewBag.UOMs = new SelectList(
                _context.DefUOMs.Where(u => u.Active).ToList(),
                "Id",
                "UOMName"
            );

            ViewBag.SubUOMs = new SelectList(
                _context.DefSubUOMs.Where(su => su.Active).ToList(),
                "Id",
                "SubUOMName"
            );

            ViewBag.Users = new SelectList(
                _context.UserInfos.Where(u => u.Active).ToList(),
                "Id",
                "FullName"
            );
        }
    }
}
