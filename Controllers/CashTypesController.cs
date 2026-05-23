using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class CashTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CashTypes
        public IActionResult Index()
        {
            return View();
        }

        // GET: CashTypes/GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.CashTypes
                .OrderBy(c => c.Code)
                .Select(c => new
                {
                    id        = c.Id,
                    code      = c.Code,
                    name      = c.Name,
                    invoiceNo = c.InvoiceNo ?? "",
                    active    = c.Active
                })
                .ToListAsync();
            return Json(list);
        }

        // POST: CashTypes/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] CashTypeSaveRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.Code))
                    return Json(new { success = false, message = "Code is required." });
                if (string.IsNullOrWhiteSpace(req.Name))
                    return Json(new { success = false, message = "Name is required." });

                if (req.Id == 0)
                {
                    var dup = await _context.CashTypes.AnyAsync(c => c.Code == req.Code);
                    if (dup)
                        return Json(new { success = false, message = $"Code '{req.Code}' already exists." });

                    var ct = new CashType
                    {
                        Code      = req.Code,
                        Name      = req.Name,
                        InvoiceNo = req.InvoiceNo,
                        Active    = req.Active,
                        CreatedDate  = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    _context.CashTypes.Add(ct);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Saved.", id = ct.Id });
                }
                else
                {
                    var ct = await _context.CashTypes.FindAsync(req.Id);
                    if (ct == null)
                        return Json(new { success = false, message = "Record not found." });

                    if (ct.Code != req.Code)
                    {
                        var dup = await _context.CashTypes.AnyAsync(c => c.Code == req.Code && c.Id != req.Id);
                        if (dup)
                            return Json(new { success = false, message = $"Code '{req.Code}' already exists." });
                    }

                    ct.Code         = req.Code;
                    ct.Name         = req.Name;
                    ct.InvoiceNo    = req.InvoiceNo;
                    ct.Active       = req.Active;
                    ct.ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Updated.", id = ct.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: CashTypes/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ct = await _context.CashTypes.FindAsync(id);
                if (ct == null)
                    return Json(new { success = false, message = "Not found." });
                _context.CashTypes.Remove(ct);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class CashTypeSaveRequest
    {
        public int     Id        { get; set; }
        public string  Code      { get; set; } = string.Empty;
        public string  Name      { get; set; } = string.Empty;
        public string? InvoiceNo { get; set; }
        public bool    Active    { get; set; } = true;
    }
}
