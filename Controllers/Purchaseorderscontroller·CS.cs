using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;
using System.Text.Json;

namespace Concept.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // التحقق من صلاحية الموافقة
        private bool CanApprove()
        {
            // ✅ Get UserId as Int32 (matches SetInt32)
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null || userId == 0)
                return false;

            // ✅ Use userId.Value to get the actual int
            var user = _context.UserInfos.Find(userId.Value);
            return user != null && user.CanApprovePurchaseOrders && user.Active;
        }

        // GET: PurchaseOrders (مع فلترة)
        public async Task<IActionResult> Index(string filter = "all")
        {
            var query = _context.PurchaseOrderHeaders
                .Include(p => p.Department)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .AsQueryable();

            // تطبيق الفلتر
            IQueryable<PurchaseOrderHeader> filteredQuery = filter switch
            {
                "pending" => query.Where(p => p.Approved == 0),    // Not Approved
                "approved" => query.Where(p => p.Approved == 1),   // Approved
                "rejected" => query.Where(p => p.Approved == 2),   // Rejected
                _ => query  // all
            };

            var orders = await filteredQuery
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            // Counts للـ badges
            ViewBag.AllCount = await _context.PurchaseOrderHeaders.CountAsync();
            ViewBag.NotApprovedCount = await _context.PurchaseOrderHeaders.CountAsync(p => p.Approved == 0);
            ViewBag.ApprovedCount = await _context.PurchaseOrderHeaders.CountAsync(p => p.Approved == 1);
            ViewBag.RejectedCount = await _context.PurchaseOrderHeaders.CountAsync(p => p.Approved == 2);

            ViewBag.Filter = filter;
            ViewBag.CanApprove = CanApprove();
            ViewBag.PendingCount = ViewBag.NotApprovedCount;

            return View(orders);
        }

        // GET: PurchaseOrders/PendingApproval
        public async Task<IActionResult> PendingApproval()
        {
            if (!CanApprove())
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page";
                return RedirectToAction(nameof(Index));
            }

            var pendingOrders = await _context.PurchaseOrderHeaders
                .Include(p => p.Department)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .Where(p => p.Approved == 0 && p.Active)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return View(pendingOrders);
        }

        // GET: PurchaseOrders/ApprovalDetails/5
        public async Task<IActionResult> ApprovalDetails(int? id)
        {
            if (!CanApprove())
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page";
                return RedirectToAction(nameof(Index));
            }

            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseOrderHeaders
                .Include(p => p.Department)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            var details = await _context.PurchaseOrderDetails
                .Include(d => d.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(d => d.Item)
                .Include(d => d.SubUOM)
                    .ThenInclude(su => su.UOM)
                .Where(d => d.PurchaseOrderHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;
            ViewBag.GrandTotal = details.Sum(d => d.NetPrice);

            return View(header);
        }

        // POST: PurchaseOrders/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string approvalNotes)
        {
            if (!CanApprove())
            {
                TempData["ErrorMessage"] = "You don't have permission to approve orders";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var order = await _context.PurchaseOrderHeaders.FindAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                order.Approved = 1;
                order.PurchaseStatus = 1;
                order.ModifiedDate = DateTime.Now;

                if (!string.IsNullOrEmpty(approvalNotes))
                {
                    order.AdditionalNotes = (order.AdditionalNotes ?? "") + "\n[Approval Notes]: " + approvalNotes;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Purchase Order {order.PurchaseCode} has been approved successfully!";
                return RedirectToAction(nameof(PendingApproval));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving order: {ex.Message}");
                TempData["ErrorMessage"] = "Error approving purchase order";
                return RedirectToAction(nameof(PendingApproval));
            }
        }

        // POST: PurchaseOrders/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string rejectionReason)
        {
            if (!CanApprove())
            {
                TempData["ErrorMessage"] = "You don't have permission to reject orders";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var order = await _context.PurchaseOrderHeaders.FindAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                order.Approved = 2;
                order.PurchaseStatus = 2;
                order.ModifiedDate = DateTime.Now;

                if (!string.IsNullOrEmpty(rejectionReason))
                {
                    order.AdditionalNotes = (order.AdditionalNotes ?? "") + "\n[Rejection Reason]: " + rejectionReason;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Purchase Order {order.PurchaseCode} has been rejected.";
                return RedirectToAction(nameof(PendingApproval));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejecting order: {ex.Message}");
                TempData["ErrorMessage"] = "Error rejecting purchase order";
                return RedirectToAction(nameof(PendingApproval));
            }
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseOrderHeaders
                .Include(p => p.Department)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            var details = await _context.PurchaseOrderDetails
                .Include(d => d.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(d => d.Item)
                .Include(d => d.SubUOM)
                    .ThenInclude(su => su.UOM)
                .Where(d => d.PurchaseOrderHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;
            ViewBag.TotalQuantity = details.Sum(d => d.Quantity);
            ViewBag.TotalBeforeDiscount = details.Sum(d => d.TotalPrice);
            ViewBag.TotalDiscount = details.Sum(d => d.Discount);
            ViewBag.GrandTotal = details.Sum(d => d.NetPrice);

            return View(header);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create()
        {
            LoadDropdowns();

            var lastOrder = _context.PurchaseOrderHeaders
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastOrder != null && !string.IsNullOrEmpty(lastOrder.PurchaseCode))
            {
                var parts = lastOrder.PurchaseCode.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                {
                    nextNumber = num + 1;
                }
            }

            ViewBag.NextPurchaseCode = $"PO-{nextNumber:D4}";
            ViewBag.CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.CurrentTime = DateTime.Now.ToString("HH:mm");

            return View();
        }

        // POST: PurchaseOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderHeader header, string detailsJson)
        {
            try
            {
                ModelState.Remove("Department");
                ModelState.Remove("User");
                ModelState.Remove("Vender");
                ModelState.Remove("PurchaseCode"); // Remove validation for auto-generated field

                if (ModelState.IsValid)
                {
                    // Auto-generate Purchase Code
                    var lastOrder = await _context.PurchaseOrderHeaders
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefaultAsync();

                    int nextNumber = 1;
                    if (lastOrder != null && !string.IsNullOrEmpty(lastOrder.PurchaseCode))
                    {
                        var parts = lastOrder.PurchaseCode.Split('-');
                        if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                        {
                            nextNumber = num + 1;
                        }
                    }

                    header.PurchaseCode = $"PO-{nextNumber:D4}";

                    // Force Status to Pending
                    header.PurchaseStatus = 0;  // Pending
                    header.Approved = 0;        // Not Approved
                    header.CreatedDate = DateTime.Now;
                    header.ModifiedDate = DateTime.Now;
                    header.Department = null;
                    header.User = null;
                    header.Vender = null;

                    _context.Add(header);
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(detailsJson))
                    {
                        var details = JsonSerializer.Deserialize<List<PurchaseOrderDetails>>(detailsJson);

                        if (details != null && details.Any())
                        {
                            foreach (var detail in details)
                            {
                                detail.PurchaseOrderHeaderId = header.Id;
                                detail.PurchaseOrderHeader = null;
                                detail.SubCategory = null;
                                detail.Item = null;
                                detail.SubUOM = null;

                                _context.PurchaseOrderDetails.Add(detail);
                            }

                            await _context.SaveChangesAsync();
                        }
                    }

                    TempData["SuccessMessage"] = "Purchase Order created successfully and pending approval!";
                    return RedirectToAction(nameof(Index));
                }

                LoadDropdowns();
                return View(header);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating purchase order: {ex.Message}");
                TempData["ErrorMessage"] = "Error creating purchase order";
                LoadDropdowns();
                return View(header);
            }
        }

        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseOrderHeaders.FindAsync(id);
            if (header == null)
            {
                return NotFound();
            }

            // منع التعديل إذا تم الموافقة أو الرفض
            if (header.Approved != 0)
            {
                TempData["ErrorMessage"] = "Cannot edit approved or rejected orders";
                return RedirectToAction(nameof(Index));
            }

            // Load details and project to anonymous object to avoid circular reference ✅
            var details = await _context.PurchaseOrderDetails
                .Include(d => d.SubCategory)
                .Include(d => d.Item)
                .Include(d => d.SubUOM)
                .Where(d => d.PurchaseOrderHeaderId == id)
                .OrderBy(d => d.Id)
                .Select(d => new
                {
                    d.Id,
                    d.SubCategoryId,
                    d.ItemId,
                    d.AvQuantity,
                    d.AvMoney,
                    d.Price,
                    d.Quantity,
                    d.TotalPrice,
                    d.Discount,
                    d.NetPrice,
                    d.SubUnitId,
                    d.PackSize,
                    d.ValueOrUnit,
                    d.freeQuantity,
                    Item = d.Item != null ? new
                    {
                        d.Item.Id,
                        d.Item.ItemCode,
                        d.Item.ItemName
                    } : null,
                    SubCategory = d.SubCategory != null ? new
                    {
                        d.SubCategory.Id,
                        d.SubCategory.Name
                    } : null,
                    SubUOM = d.SubUOM != null ? new
                    {
                        d.SubUOM.Id,
                        d.SubUOM.SubUOMName
                    } : null
                })
                .ToListAsync();

            // Serialize with case-sensitive property names ✅
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null  // Keep original property names (PascalCase)
            };

            ViewBag.ExistingDetails = JsonSerializer.Serialize(details, jsonOptions);
            LoadDropdowns(header);

            return View(header);
        }

        // POST: PurchaseOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseOrderHeader header, string detailsJson)
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
                    // التحقق من عدم الموافقة/الرفض
                    var existingOrder = await _context.PurchaseOrderHeaders.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingOrder != null && existingOrder.Approved != 0)
                    {
                        TempData["ErrorMessage"] = "Cannot edit approved or rejected orders";
                        return RedirectToAction(nameof(Index));
                    }

                    if (await _context.PurchaseOrderHeaders.AnyAsync(p => p.PurchaseCode == header.PurchaseCode && p.Id != header.Id))
                    {
                        TempData["ErrorMessage"] = "Purchase Code already exists";
                        LoadDropdowns(header);
                        return View(header);
                    }

                    header.ModifiedDate = DateTime.Now;
                    header.Department = null;
                    header.User = null;
                    header.Vender = null;

                    _context.Update(header);

                    var oldDetails = await _context.PurchaseOrderDetails
                        .Where(d => d.PurchaseOrderHeaderId == id)
                        .ToListAsync();

                    _context.PurchaseOrderDetails.RemoveRange(oldDetails);

                    if (!string.IsNullOrEmpty(detailsJson))
                    {
                        var details = JsonSerializer.Deserialize<List<PurchaseOrderDetails>>(detailsJson);

                        if (details != null && details.Any())
                        {
                            foreach (var detail in details)
                            {
                                detail.Id = 0;
                                detail.PurchaseOrderHeaderId = header.Id;
                                detail.PurchaseOrderHeader = null;
                                detail.SubCategory = null;
                                detail.Item = null;
                                detail.SubUOM = null;

                                _context.PurchaseOrderDetails.Add(detail);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Purchase Order updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(header.Id))
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
                    Console.WriteLine($"Error updating purchase order: {ex.Message}");
                    TempData["ErrorMessage"] = "Error updating purchase order";
                }
            }

            LoadDropdowns(header);
            return View(header);
        }

      
        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseOrderHeaders
                .Include(p => p.Department)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            var details = await _context.PurchaseOrderDetails
                .Include(d => d.SubCategory)
                .Include(d => d.Item)
                .Include(d => d.SubUOM)
                .Where(d => d.PurchaseOrderHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;
            ViewBag.GrandTotal = details.Sum(d => d.NetPrice);

            return View(header);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var header = await _context.PurchaseOrderHeaders.FindAsync(id);
                if (header != null)
                {
                    var details = await _context.PurchaseOrderDetails
                        .Where(d => d.PurchaseOrderHeaderId == id)
                        .ToListAsync();

                    _context.PurchaseOrderDetails.RemoveRange(details);
                    _context.PurchaseOrderHeaders.Remove(header);

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Purchase Order deleted successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting purchase order: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting purchase order";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX: Get Item Details
        [HttpGet]
        public JsonResult GetItemDetails(int itemId)
        {
            var item = _context.StoreItems
                .Where(i => i.Id == itemId)
                .Select(i => new
                {
                    id = i.Id,
                    code = i.ItemCode,
                    name = i.ItemName,
                    packSize = i.PackSize,
                    quantityInStore = i.QuantityInStore,
                    purchaseValue = i.PurchaseValue,
                    saleValue = i.SaleValue
                })
                .FirstOrDefault();

            return Json(item);
        }

        // AJAX Methods
        [HttpGet]
        public JsonResult GetSubCategoriesByCategory(int categoryId)
        {
            var subCategories = _context.DeffSubCategories
                .Where(sc => sc.CategoryId == categoryId && sc.Active)
                .OrderBy(sc => sc.Name)
                .Select(sc => new { id = sc.Id, name = sc.Name })
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
                    packSize = i.PackSize,
                    quantityInStore = i.QuantityInStore,
                    purchaseValue = i.PurchaseValue
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
                .Select(su => new { id = su.Id, name = su.SubUOMName })
                .ToList();

            return Json(subUOMs);
        }

        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrderHeaders.Any(e => e.Id == id);
        }

        private void LoadDropdowns(PurchaseOrderHeader header = null)
        {
            ViewBag.Departments = new SelectList(
                _context.DeffDepartments.Where(d => d.Active).OrderBy(d => d.DepartmentName),
                "Id", "DepartmentName", header?.DepartmentId
            );

            ViewBag.Users = new SelectList(
                _context.UserInfos.Where(u => u.Active).OrderBy(u => u.FullName),
                "Id", "FullName", header?.UserId
            );

            ViewBag.Vendors = new SelectList(
                _context.Venders.Where(v => v.Active).OrderBy(v => v.VenderName),
                "Id", "VenderName", header?.VenderId
            );

            ViewBag.Categories = _context.DeffCategories
                .Where(c => c.Active)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            ViewBag.UOMs = _context.DefUOMs
                .Where(u => u.Active)
                .OrderBy(u => u.UOMName)
                .Select(u => new { u.Id, u.UOMName })
                .ToList();
        }
    }
}