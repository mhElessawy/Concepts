using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;
using System.Text.Json;

namespace Concept.Controllers
{
    public class TransferController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransferController(ApplicationDbContext context)
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

        // GET: Transfer
        public async Task<IActionResult> Index(string filter = "all")
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.StoreTransferHeaders
                .Include(t => t.FromWarehouse)
                .Include(t => t.FromDepartment)
                .Include(t => t.ToWarehouse)
                .Include(t => t.ToDepartment)
                .Include(t => t.User)
                .AsQueryable();

            // Apply filter
            IQueryable<StoreTransferHeader> filteredQuery = filter switch
            {
                "pending" => query.Where(t => t.TransferStatus == 0),    // Pending
                "approved" => query.Where(t => t.TransferStatus == 1),   // Approved
                "rejected" => query.Where(t => t.TransferStatus == 2),   // Rejected
                _ => query  // all
            };

            var transfers = await filteredQuery
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();

            // Counts for badges
            ViewBag.AllCount = await _context.StoreTransferHeaders.CountAsync();
            ViewBag.PendingCount = await _context.StoreTransferHeaders.CountAsync(t => t.TransferStatus == 0);
            ViewBag.ApprovedCount = await _context.StoreTransferHeaders.CountAsync(t => t.TransferStatus == 1);
            ViewBag.RejectedCount = await _context.StoreTransferHeaders.CountAsync(t => t.TransferStatus == 2);

            ViewBag.Filter = filter;

            return View(transfers);
        }

        // GET: Transfer/PendingApproval
        public async Task<IActionResult> PendingApproval()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var pendingTransfers = await _context.StoreTransferHeaders
                .Include(t => t.FromWarehouse)
                .Include(t => t.FromDepartment)
                .Include(t => t.ToWarehouse)
                .Include(t => t.ToDepartment)
                .Include(t => t.User)
                .Where(t => t.TransferStatus == 0 && t.Active)
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();

            ViewBag.PendingCount = pendingTransfers.Count;

            return View(pendingTransfers);
        }

        // GET: Transfer/Create
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            LoadDropdownData();
            return View();
        }

        // POST: Transfer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreTransferHeader header, string detailsJson)
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
                header.TransferStatus = 0; // Pending

                // Generate Transfer Code and Number
                var lastTransfer = await _context.StoreTransferHeaders
                    .OrderByDescending(t => t.Id)
                    .FirstOrDefaultAsync();

                int nextNumber = (lastTransfer?.TransferNo ?? 0) + 1;
                header.TransferNo = nextNumber;
                header.TransferCode = $"TR-{DateTime.Now.Year}-{nextNumber.ToString().PadLeft(6, '0')}";

                // From/To fields can now be 0 (no validation needed)

                // Add header
                _context.StoreTransferHeaders.Add(header);
                await _context.SaveChangesAsync();

                // Parse and add details
                if (!string.IsNullOrEmpty(detailsJson))
                {
                    var details = JsonSerializer.Deserialize<List<StoreTransferDetails>>(detailsJson);
                    if (details != null && details.Any())
                    {
                        foreach (var detail in details)
                        {
                            detail.StoreTransferHeaderId = header.Id;
                            _context.StoreTransferDetails.Add(detail);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["SuccessMessage"] = "Transfer created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating transfer: {ex.Message}";
                LoadDropdownData();
                return View(header);
            }
        }

        // GET: Transfer/Details/5
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

            var header = await _context.StoreTransferHeaders
                .Include(t => t.FromWarehouse)
                .Include(t => t.FromDepartment)
                .Include(t => t.ToWarehouse)
                .Include(t => t.ToDepartment)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            var details = await _context.StoreTransferDetails
                .Include(d => d.Category)
                .Include(d => d.SubCategory)
                .Include(d => d.Item)
                .Include(d => d.UOM)
                .Include(d => d.SubUOM)
                .Where(d => d.StoreTransferHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;

            return View(header);
        }

        // GET: Transfer/Edit/5
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

            var header = await _context.StoreTransferHeaders.FindAsync(id);
            if (header == null)
            {
                return NotFound();
            }

            // Check if transfer is already approved or rejected
            if (header.TransferStatus != 0)
            {
                TempData["ErrorMessage"] = "Cannot edit approved or rejected transfers";
                return RedirectToAction(nameof(Details), new { id });
            }

            var details = await _context.StoreTransferDetails
                .Where(d => d.StoreTransferHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;
            LoadDropdownData();

            return View(header);
        }

        // POST: Transfer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreTransferHeader header, string detailsJson)
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
                var existingHeader = await _context.StoreTransferHeaders.FindAsync(id);
                if (existingHeader == null)
                {
                    return NotFound();
                }

                // Check if transfer is already approved or rejected
                if (existingHeader.TransferStatus != 0)
                {
                    TempData["ErrorMessage"] = "Cannot edit approved or rejected transfers";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Update header
                existingHeader.TransferType = header.TransferType;
                existingHeader.TransferDate = header.TransferDate;
                existingHeader.TransferTime = header.TransferTime;
                existingHeader.FromWarehouseId = header.FromWarehouseId;
                existingHeader.FromDepartmentId = header.FromDepartmentId;
                existingHeader.ToWarehouseId = header.ToWarehouseId;
                existingHeader.ToDepartmentId = header.ToDepartmentId;
                existingHeader.RequestedBy = header.RequestedBy;
                existingHeader.AprovedBy = header.AprovedBy;
                existingHeader.AdditionalNotes = header.AdditionalNotes;
                existingHeader.ModifiedDate = DateTime.Now;

                // From/To fields can now be 0 (no validation needed)

                // Delete existing details
                var existingDetails = await _context.StoreTransferDetails
                    .Where(d => d.StoreTransferHeaderId == id)
                    .ToListAsync();
                _context.StoreTransferDetails.RemoveRange(existingDetails);

                // Add new details
                if (!string.IsNullOrEmpty(detailsJson))
                {
                    var details = JsonSerializer.Deserialize<List<StoreTransferDetails>>(detailsJson);
                    if (details != null && details.Any())
                    {
                        foreach (var detail in details)
                        {
                            detail.StoreTransferHeaderId = id;
                            _context.StoreTransferDetails.Add(detail);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Transfer updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating transfer: {ex.Message}";
                LoadDropdownData();
                return View(header);
            }
        }

        // POST: Transfer/Delete/5
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
                var header = await _context.StoreTransferHeaders.FindAsync(id);
                if (header == null)
                {
                    return NotFound();
                }

                // Check if transfer is already approved
                if (header.TransferStatus == 1)
                {
                    TempData["ErrorMessage"] = "Cannot delete approved transfers";
                    return RedirectToAction(nameof(Index));
                }

                // Soft delete
                header.Active = false;
                header.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Transfer deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting transfer: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Transfer/Approve/5
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
                var header = await _context.StoreTransferHeaders.FindAsync(id);
                if (header == null)
                {
                    return NotFound();
                }

                header.TransferStatus = 1; // Approved
                header.AprovedBy = GetCurrentUserId();
                header.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Transfer approved successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving transfer: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Transfer/Reject/5
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
                var header = await _context.StoreTransferHeaders.FindAsync(id);
                if (header == null)
                {
                    return NotFound();
                }

                header.TransferStatus = 2; // Rejected
                header.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Transfer rejected successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting transfer: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // API Methods for AJAX calls

        // GET: Transfer/GetWarehouses
        [HttpGet]
        public async Task<JsonResult> GetWarehouses()
        {
            var warehouses = await _context.Warehouses
                .Where(w => w.Active)
                .Select(w => new { w.Id, w.WarehouseName })
                .ToListAsync();

            return Json(warehouses);
        }

        // GET: Transfer/GetDepartments
        [HttpGet]
        public async Task<JsonResult> GetDepartments()
        {
            var departments = await _context.DeffDepartments
                .Where(d => d.Active)
                .Select(d => new { d.Id, d.DepartmentName })
                .ToListAsync();

            return Json(departments);
        }

        // GET: Transfer/GetSubCategories
        [HttpGet]
        public async Task<JsonResult> GetSubCategories()
        {
            var subCategories = await _context.DeffSubCategories
                .Where(sc => sc.Active)
                .Include(sc => sc.Category)
                .Select(sc => new { sc.Id, sc.Name, CategoryName = sc.Category.Name })
                .ToListAsync();

            return Json(subCategories);
        }

        // GET: Transfer/GetItemsBySubCategory/5
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

        // GET: Transfer/GetItemDetails/5
        [HttpGet]
        public async Task<JsonResult> GetItemDetails(int itemId)
        {
            var item = await _context.StoreItems
                .Where(i => i.Id == itemId)
                .Include(i => i.SubUOM)
                .Select(i => new
                {
                    i.Id,
                    i.ItemCode,
                    i.ItemName,
                    SubUOMId = i.SubUOMId ?? 0,
                    SubUOMName = i.SubUOM != null ? i.SubUOM.SubUOMName : "",
                    i.PackSize,
                    i.QuantityInStore
                })
                .FirstOrDefaultAsync();

            return Json(item);
        }

        // GET: Transfer/GetSubUOMs
        [HttpGet]
        public async Task<JsonResult> GetSubUOMs()
        {
            var subUOMs = await _context.DefSubUOMs
                .Where(su => su.Active)
                .Include(su => su.UOM)
                .Select(su => new
                {
                    su.Id,
                    su.SubUOMName,
                    UOMName = su.UOM.UOMName,
                    UOMId = su.UOMId
                })
                .ToListAsync();

            return Json(subUOMs);
        }

        // GET: Transfer/GetCategories
        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            var categories = await _context.DeffCategories
                .Where(c => c.Active)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            return Json(categories);
        }

        // GET: Transfer/GetSubCategoriesByCategory/5
        [HttpGet]
        public async Task<JsonResult> GetSubCategoriesByCategory(int categoryId)
        {
            var subCategories = await _context.DeffSubCategories
                .Where(sc => sc.CategoryId == categoryId && sc.Active)
                .Select(sc => new { sc.Id, sc.Name })
                .ToListAsync();

            return Json(subCategories);
        }

        // GET: Transfer/GetUOMs
        [HttpGet]
        public async Task<JsonResult> GetUOMs()
        {
            var uoms = await _context.DefUOMs
                .Where(u => u.Active)
                .Select(u => new { u.Id, u.UOMName })
                .ToListAsync();

            return Json(uoms);
        }

        // GET: Transfer/GetSubUOMsByUOM/5
        [HttpGet]
        public async Task<JsonResult> GetSubUOMsByUOM(int uomId)
        {
            var subUOMs = await _context.DefSubUOMs
                .Where(su => su.UOMId == uomId && su.Active)
                .Select(su => new { su.Id, su.SubUOMName })
                .ToListAsync();

            return Json(subUOMs);
        }

        // GET: Transfer/GetBatchesByItem/5
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

        // GET: Transfer/GetItemFullDetails/5
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
