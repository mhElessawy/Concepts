using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;
using System.Text.Json;

namespace Concept.Controllers
{
    public class PurchaseRecievedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseRecievedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseRecieved
        public async Task<IActionResult> Index(string filter = "all")
        {
            var query = _context.PurchaseRecievedHeaders
                .Include(p => p.Warehouse)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .AsQueryable();

            // Apply filter
            IQueryable<PurchaseRecievedHeader> filteredQuery = filter switch
            {
                "pending" => query.Where(p => p.Approved == 0),    // Not Approved
                "approved" => query.Where(p => p.Approved == 1),   // Approved
                "rejected" => query.Where(p => p.Approved == 2),   // Rejected
                _ => query  // all
            };

            var received = await filteredQuery
                .OrderByDescending(p => p.RecieveDate)
                .ToListAsync();

            // Counts for badges
            ViewBag.AllCount = await _context.PurchaseRecievedHeaders.CountAsync();
            ViewBag.NotApprovedCount = await _context.PurchaseRecievedHeaders.CountAsync(p => p.Approved == 0);
            ViewBag.ApprovedCount = await _context.PurchaseRecievedHeaders.CountAsync(p => p.Approved == 1);
            ViewBag.RejectedCount = await _context.PurchaseRecievedHeaders.CountAsync(p => p.Approved == 2);

            ViewBag.Filter = filter;
            ViewBag.PendingCount = ViewBag.NotApprovedCount;

            return View(received);
        }

        // GET: PurchaseRecieved/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseRecievedHeaders
                .Include(p => p.Warehouse)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            var details = await _context.PurchaseRecievedDetails
                .Include(d => d.SubUOM)
                    .ThenInclude(su => su.UOM)
                .Include(d => d.StoreItem)
                .Where(d => d.PurchaseRecievedHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;
            ViewBag.TotalQuantity = details.Sum(d => d.TotalQuantity);
            ViewBag.TotalBeforeDiscount = details.Sum(d => d.TotalPrice);
            ViewBag.TotalDiscount = details.Sum(d => d.Discount);
            ViewBag.GrandTotal = details.Sum(d => d.NetPrice);

            return View(header);
        }

        // GET: PurchaseRecieved/Create
        public IActionResult Create()
        {
            LoadDropdowns();

            var lastReceived = _context.PurchaseRecievedHeaders
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastReceived != null && !string.IsNullOrEmpty(lastReceived.RecieveNo))
            {
                var parts = lastReceived.RecieveNo.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                {
                    nextNumber = num + 1;
                }
            }

            ViewBag.NextRecieveNo = $"PR-{nextNumber:D4}";
            ViewBag.CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.CurrentTime = DateTime.Now.ToString("HH:mm");

            return View();
        }

        // POST: PurchaseRecieved/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseRecievedHeader header, string detailsJson)
        {
            try
            {
                ModelState.Remove("Warehouse");
                ModelState.Remove("User");
                ModelState.Remove("Vender");
                ModelState.Remove("RecieveNo");

                if (ModelState.IsValid)
                {
                    // Auto-generate Receive Number
                    var lastReceived = await _context.PurchaseRecievedHeaders
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefaultAsync();

                    int nextNumber = 1;
                    if (lastReceived != null && !string.IsNullOrEmpty(lastReceived.RecieveNo))
                    {
                        var parts = lastReceived.RecieveNo.Split('-');
                        if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                        {
                            nextNumber = num + 1;
                        }
                    }

                    header.RecieveNo = $"PR-{nextNumber:D4}";
                    header.Approved = 0;        // Not Approved
                    header.CreatedDate = DateTime.Now;
                    header.ModifiedDate = DateTime.Now;
                    header.Warehouse = null;
                    header.User = null;
                    header.Vender = null;

                    _context.Add(header);
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(detailsJson))
                    {
                        var details = JsonSerializer.Deserialize<List<PurchaseRecievedDetails>>(detailsJson);

                        if (details != null && details.Any())
                        {
                            foreach (var detail in details)
                            {
                                detail.PurchaseRecievedHeaderId = header.Id;
                                detail.PurchaseRecievedHeader = null;
                                detail.SubUOM = null;
                                detail.StoreItem = null;

                                _context.PurchaseRecievedDetails.Add(detail);
                            }

                            await _context.SaveChangesAsync();
                        }
                    }

                    TempData["SuccessMessage"] = "Purchase Received created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                LoadDropdowns();
                return View(header);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating purchase received: {ex.Message}");
                TempData["ErrorMessage"] = "Error creating purchase received";
                LoadDropdowns();
                return View(header);
            }
        }

        // GET: PurchaseRecieved/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseRecievedHeaders.FindAsync(id);
            if (header == null)
            {
                return NotFound();
            }

            // Prevent editing if approved or rejected
            if (header.Approved != 0)
            {
                TempData["ErrorMessage"] = "Cannot edit approved or rejected records";
                return RedirectToAction(nameof(Index));
            }

            var details = await _context.PurchaseRecievedDetails
                .Include(d => d.SubUOM)
                .Include(d => d.StoreItem)
                .Where(d => d.PurchaseRecievedHeaderId == id)
                .OrderBy(d => d.Id)
                .Select(d => new
                {
                    d.Id,
                    d.ItemId,
                    d.OrderQuantity,
                    d.RecieveQuantity,
                    d.PendingQuantity,
                    d.FreeQuantity,
                    d.TotalQuantity,
                    d.UnitPrice,
                    d.TotalPrice,
                    d.Discount,
                    d.NetPrice,
                    d.SubUOMId,
                    d.PackSize,
                    d.ValueOrUnit,
                    d.ExpiredDate,
                    Item = d.StoreItem != null ? new
                    {
                        d.StoreItem.Id,
                        d.StoreItem.ItemCode,
                        d.StoreItem.ItemName
                    } : null,
                    SubUOM = d.SubUOM != null ? new
                    {
                        d.SubUOM.Id,
                        d.SubUOM.Code,
                        d.SubUOM.UOMId
                    } : null
                })
                .ToListAsync();

            ViewBag.ExistingDetails = JsonSerializer.Serialize(details);
            LoadDropdowns();

            return View(header);
        }

        // POST: PurchaseRecieved/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseRecievedHeader header, string detailsJson)
        {
            if (id != header.Id)
            {
                return NotFound();
            }

            try
            {
                ModelState.Remove("Warehouse");
                ModelState.Remove("User");
                ModelState.Remove("Vender");

                if (ModelState.IsValid)
                {
                    var existingHeader = await _context.PurchaseRecievedHeaders.FindAsync(id);
                    if (existingHeader == null)
                    {
                        return NotFound();
                    }

                    // Prevent editing if approved or rejected
                    if (existingHeader.Approved != 0)
                    {
                        TempData["ErrorMessage"] = "Cannot edit approved or rejected records";
                        return RedirectToAction(nameof(Index));
                    }

                    // Update header
                    existingHeader.RecieveDate = header.RecieveDate;
                    existingHeader.RecieveTime = header.RecieveTime;
                    existingHeader.BatchNo = header.BatchNo;
                    existingHeader.WarehouseId = header.WarehouseId;
                    existingHeader.VenderId = header.VenderId;
                    existingHeader.PurchaseOrderHeaderId = header.PurchaseOrderHeaderId;
                    existingHeader.VenderInvoiceNo = header.VenderInvoiceNo;
                    existingHeader.PaymentTerms = header.PaymentTerms;
                    existingHeader.UserId = header.UserId;
                    existingHeader.AdditionalNotes = header.AdditionalNotes;
                    existingHeader.ModifiedDate = DateTime.Now;

                    // Delete old details
                    var oldDetails = _context.PurchaseRecievedDetails
                        .Where(d => d.PurchaseRecievedHeaderId == id);
                    _context.PurchaseRecievedDetails.RemoveRange(oldDetails);

                    // Add new details
                    if (!string.IsNullOrEmpty(detailsJson))
                    {
                        var details = JsonSerializer.Deserialize<List<PurchaseRecievedDetails>>(detailsJson);

                        if (details != null && details.Any())
                        {
                            foreach (var detail in details)
                            {
                                detail.Id = 0;
                                detail.PurchaseRecievedHeaderId = id;
                                detail.PurchaseRecievedHeader = null;
                                detail.SubUOM = null;
                                detail.StoreItem = null;

                                _context.PurchaseRecievedDetails.Add(detail);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Purchase Received updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                LoadDropdowns();
                return View(header);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating purchase received: {ex.Message}");
                TempData["ErrorMessage"] = "Error updating purchase received";
                LoadDropdowns();
                return View(header);
            }
        }

        // GET: PurchaseRecieved/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var header = await _context.PurchaseRecievedHeaders
                .Include(p => p.Warehouse)
                .Include(p => p.User)
                .Include(p => p.Vender)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (header == null)
            {
                return NotFound();
            }

            var details = await _context.PurchaseRecievedDetails
                .Include(d => d.SubUOM)
                .Include(d => d.StoreItem)
                .Where(d => d.PurchaseRecievedHeaderId == id)
                .ToListAsync();

            ViewBag.Details = details;

            return View(header);
        }

        // POST: PurchaseRecieved/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var header = await _context.PurchaseRecievedHeaders.FindAsync(id);
                if (header != null)
                {
                    // Soft delete
                    header.Active = false;
                    header.ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Purchase Received deleted successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting purchase received: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting purchase received";
                return RedirectToAction(nameof(Index));
            }
        }

        // Helper method to load dropdowns
        private void LoadDropdowns()
        {
            ViewBag.Warehouses = new SelectList(_context.Warehouses
                .Where(w => w.Active)
                .OrderBy(w => w.WarehouseName), "Id", "WarehouseName");

            ViewBag.Venders = new SelectList(_context.Venders
                .Where(v => v.Active)
                .OrderBy(v => v.VenderName), "Id", "VenderName");

            ViewBag.Users = new SelectList(_context.UserInfos
                .Where(u => u.Active)
                .OrderBy(u => u.FullName), "Id", "FullName");

            ViewBag.PurchaseOrders = new SelectList(_context.PurchaseOrderHeaders
                .Where(p => p.Active && p.Approved == 1)
                .OrderByDescending(p => p.PurchaseDate), "Id", "PurchaseCode");

            ViewBag.Items = new SelectList(_context.StoreItems
                .Where(i => i.Active)
                .OrderBy(i => i.ItemName), "Id", "ItemName");

            ViewBag.SubUOMs = new SelectList(_context.DefSubUOMs
                .Include(s => s.UOM)
                .OrderBy(s => s.Code)
                .Select(s => new
                {
                    s.Id,
                    DisplayName = s.Code + " (" + s.UOM.Name + ")"
                }), "Id", "DisplayName");
        }

        // API endpoint to get Purchase Order details
        [HttpGet]
        public async Task<JsonResult> GetPurchaseOrderDetails(int purchaseOrderId)
        {
            var details = await _context.PurchaseOrderDetails
                .Include(d => d.Item)
                .Include(d => d.SubUOM)
                .Where(d => d.PurchaseOrderHeaderId == purchaseOrderId)
                .Select(d => new
                {
                    d.ItemId,
                    ItemName = d.Item.ItemName,
                    ItemCode = d.Item.ItemCode,
                    OrderQuantity = d.Quantity,
                    d.Price,
                    d.SubUnitId,
                    SubUOMCode = d.SubUOM.Code,
                    d.PackSize,
                    d.ValueOrUnit
                })
                .ToListAsync();

            return Json(details);
        }
    }
}
