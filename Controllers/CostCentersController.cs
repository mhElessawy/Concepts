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
            var list = await _context.DeffCostCenters
                .Include(c => c.ParentCostCenter)
                .OrderBy(c => c.CostCenterCode)
                .ToListAsync();
            return View(list);
        }

        // GET: CostCenters/GetTree
        [HttpGet]
        public async Task<IActionResult> GetTree()
        {
            var list = await _context.DeffCostCenters
                .OrderBy(c => c.CostCenterCode)
                .Select(c => new
                {
                    id = c.Id,
                    code = c.CostCenterCode,
                    name = c.CostCenterName,
                    active = c.Active,
                    parentId = c.ParentCostCenterId,
                    hasChildren = c.Children.Any()
                })
                .ToListAsync();
            return Json(list);
        }

        // GET: CostCenters/GetCostCenter/5
        [HttpGet]
        public async Task<IActionResult> GetCostCenter(int id)
        {
            var cc = await _context.DeffCostCenters
                .Include(c => c.ParentCostCenter)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cc == null) return NotFound();

            return Json(new
            {
                id = cc.Id,
                code = cc.CostCenterCode,
                name = cc.CostCenterName,
                parentId = cc.ParentCostCenterId,
                parentCode = cc.ParentCostCenter?.CostCenterCode ?? "",
                parentName = cc.ParentCostCenter?.CostCenterName ?? "",
                date = cc.Date.ToString("yyyy-MM-dd"),
                active = cc.Active,
                target = cc.Target ?? ""
            });
        }

        // GET: CostCenters/GetNextCode?parentId=5   (omit for root)
        [HttpGet]
        public async Task<IActionResult> GetNextCode(int? parentId)
        {
            if (parentId == null || parentId == 0)
            {
                var rootCodes = await _context.DeffCostCenters
                    .Where(c => c.ParentCostCenterId == null)
                    .Select(c => c.CostCenterCode)
                    .ToListAsync();

                int maxNo = 0;
                foreach (var code in rootCodes)
                    if (int.TryParse(code, out int n) && n > maxNo)
                        maxNo = n;

                return Json(new { code = (maxNo + 1).ToString() });
            }
            else
            {
                var parent = await _context.DeffCostCenters.FindAsync(parentId.Value);
                if (parent == null) return Json(new { code = "" });

                string parentCode = parent.CostCenterCode;

                var siblingCodes = await _context.DeffCostCenters
                    .Where(c => c.ParentCostCenterId == parentId.Value)
                    .Select(c => c.CostCenterCode)
                    .ToListAsync();

                int maxSuffix = 0;
                foreach (var code in siblingCodes)
                {
                    if (code.StartsWith(parentCode) && code.Length == parentCode.Length + 2)
                    {
                        string suffix = code.Substring(parentCode.Length);
                        if (int.TryParse(suffix, out int s) && s > maxSuffix)
                            maxSuffix = s;
                    }
                }

                return Json(new { code = parentCode + (maxSuffix + 1).ToString("D2") });
            }
        }

        // GET: CostCenters/GetByCode?code=xxx
        [HttpGet]
        public async Task<IActionResult> GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Json(new { found = false });

            var cc = await _context.DeffCostCenters
                .FirstOrDefaultAsync(c => c.CostCenterCode == code);

            if (cc == null) return Json(new { found = false });

            return Json(new { found = true, id = cc.Id, code = cc.CostCenterCode, name = cc.CostCenterName });
        }

        // POST: CostCenters/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            int id,
            string code,
            string name,
            int? parentCostCenterId,
            DateTime date,
            bool active,
            string? target)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return Json(new { success = false, message = "Code is required." });
                if (string.IsNullOrWhiteSpace(name))
                    return Json(new { success = false, message = "Name is required." });

                var duplicate = await _context.DeffCostCenters
                    .AnyAsync(c => c.CostCenterCode == code && c.Id != id);
                if (duplicate)
                    return Json(new { success = false, message = $"Code '{code}' already exists." });

                if (id == 0)
                {
                    var cc = new DeffCostCenter
                    {
                        CostCenterCode = code,
                        CostCenterName = name,
                        ParentCostCenterId = parentCostCenterId,
                        Date = date,
                        Active = active,
                        Target = target,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    _context.DeffCostCenters.Add(cc);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cost Center created successfully.", id = cc.Id });
                }
                else
                {
                    var cc = await _context.DeffCostCenters.FindAsync(id);
                    if (cc == null) return Json(new { success = false, message = "Not found." });

                    cc.CostCenterCode = code;
                    cc.CostCenterName = name;
                    cc.ParentCostCenterId = parentCostCenterId;
                    cc.Date = date;
                    cc.Active = active;
                    cc.Target = target;
                    cc.ModifiedDate = DateTime.Now;

                    _context.Update(cc);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cost Center updated successfully.", id = cc.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: CostCenters/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var cc = await _context.DeffCostCenters
                    .Include(c => c.Children)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cc == null) return Json(new { success = false, message = "Not found." });
                if (cc.Children.Any()) return Json(new { success = false, message = "Cannot delete: has sub cost centers." });

                _context.DeffCostCenters.Remove(cc);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}