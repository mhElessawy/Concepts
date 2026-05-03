using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class ChildAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChildAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChildAccounts
        public async Task<IActionResult> Index()
        {
            var accounts = await _context.DeffChildAccounts
                .Include(ca => ca.CostCenter)
                .Include(ca => ca.ParentAccount)
                .OrderBy(ca => ca.AccountCode)
                .ToListAsync();
            return View(accounts);
        }

        // GET: ChildAccounts/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: ChildAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeffChildAccount account)
        {
            ModelState.Remove("CostCenter");
            ModelState.Remove("ParentAccount");
            ModelState.Remove("Children");

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _context.DeffChildAccounts.AnyAsync(ca => ca.AccountCode == account.AccountCode))
                    {
                        ModelState.AddModelError("AccountCode", "Account Code already exists.");
                        LoadDropdowns(account);
                        return View(account);
                    }

                    account.CostCenter = null;
                    account.ParentAccount = null;
                    account.Children = null;
                    account.CreatedDate = DateTime.Now;
                    account.ModifiedDate = DateTime.Now;

                    _context.Add(account);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Child Account created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving: " + ex.Message);
                }
            }

            LoadDropdowns(account);
            return View(account);
        }

        // GET: ChildAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var account = await _context.DeffChildAccounts
                .Include(ca => ca.CostCenter)
                .Include(ca => ca.ParentAccount)
                .FirstOrDefaultAsync(ca => ca.Id == id);

            if (account == null) return NotFound();

            LoadDropdowns(account);
            return View(account);
        }

        // POST: ChildAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeffChildAccount account)
        {
            if (id != account.Id) return NotFound();

            ModelState.Remove("CostCenter");
            ModelState.Remove("ParentAccount");
            ModelState.Remove("Children");

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _context.DeffChildAccounts.AnyAsync(ca => ca.AccountCode == account.AccountCode && ca.Id != account.Id))
                    {
                        ModelState.AddModelError("AccountCode", "Account Code already exists.");
                        LoadDropdowns(account);
                        return View(account);
                    }

                    account.CostCenter = null;
                    account.ParentAccount = null;
                    account.Children = null;
                    account.ModifiedDate = DateTime.Now;

                    _context.Update(account);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Child Account updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DeffChildAccounts.Any(ca => ca.Id == id)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating: " + ex.Message);
                }
            }

            LoadDropdowns(account);
            return View(account);
        }

        // GET: ChildAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var account = await _context.DeffChildAccounts
                .Include(ca => ca.CostCenter)
                .Include(ca => ca.ParentAccount)
                .Include(ca => ca.Children)
                .FirstOrDefaultAsync(ca => ca.Id == id);

            if (account == null) return NotFound();
            return View(account);
        }

        // GET: ChildAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var account = await _context.DeffChildAccounts
                .Include(ca => ca.CostCenter)
                .Include(ca => ca.ParentAccount)
                .Include(ca => ca.Children)
                .FirstOrDefaultAsync(ca => ca.Id == id);

            if (account == null) return NotFound();
            return View(account);
        }

        // POST: ChildAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.DeffChildAccounts
                .Include(ca => ca.Children)
                .FirstOrDefaultAsync(ca => ca.Id == id);

            if (account != null)
            {
                if (account.Children.Any())
                {
                    TempData["ErrorMessage"] = "Cannot delete: this account has child records.";
                    return RedirectToAction(nameof(Index));
                }

                _context.DeffChildAccounts.Remove(account);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Child Account deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdowns(DeffChildAccount account = null)
        {
            // Cost Centers
            ViewBag.CostCenters = _context.DeffCostCenters
                .Where(cc => cc.Active)
                .OrderBy(cc => cc.CostCenterCode)
                .Select(cc => new { cc.Id, Display = cc.CostCenterCode + " - " + cc.CostCenterName })
                .ToList();

            ViewBag.SelectedCostCenterId = account?.CostCenterId;

            // Parent Accounts (exclude self if editing)
            var query = _context.DeffChildAccounts.AsQueryable();
            if (account?.Id > 0)
                query = query.Where(ca => ca.Id != account.Id);

            ViewBag.ParentAccounts = query
                .OrderBy(ca => ca.AccountCode)
                .Select(ca => new { ca.Id, Display = ca.AccountCode + " - " + ca.AccountName })
                .ToList();

            ViewBag.SelectedParentAccountId = account?.ParentAccountId;

            // Pre-load Cost Center name if set
            if (account?.CostCenterId.HasValue == true)
            {
                var cc = _context.DeffCostCenters.Find(account.CostCenterId.Value);
                ViewBag.CostCenterName = cc?.CostCenterName ?? "";
            }
            else
            {
                ViewBag.CostCenterName = "";
            }
        }
    }
}
