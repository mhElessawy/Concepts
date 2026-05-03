using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class MainAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MainAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MainAccounts
        public async Task<IActionResult> Index()
        {
            var accounts = await _context.MainAccounts
                .Include(a => a.ParentAccount)
                .Include(a => a.Children)
                .OrderBy(a => a.AccountNo)
                .ToListAsync();

            return View(accounts);
        }

        // GET: MainAccounts/GetAccount/5
        [HttpGet]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _context.MainAccounts
                .Include(a => a.ParentAccount)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
                return NotFound();

            return Json(new
            {
                id = account.Id,
                accountNo = account.AccountNo,
                accountName = account.AccountName,
                accountEffect = account.AccountEffect,
                treatsAsCashAccount = account.TreatsAsCashAccount,
                active = account.Active,
                openAccountDate = account.OpenAccountDate.ToString("yyyy/MM/dd"),
                note = account.Note,
                parentAccountId = account.ParentAccountId,
                parentAccountNo = account.ParentAccount?.AccountNo ?? "",
                parentAccountName = account.ParentAccount?.AccountName ?? ""
            });
        }

        // GET: MainAccounts/GetParentByNo?no=xxx
        [HttpGet]
        public async Task<IActionResult> GetParentByNo(string no)
        {
            if (string.IsNullOrWhiteSpace(no))
                return Json(new { found = false });

            var account = await _context.MainAccounts
                .FirstOrDefaultAsync(a => a.AccountNo == no);

            if (account == null)
                return Json(new { found = false });

            return Json(new
            {
                found = true,
                id = account.Id,
                accountNo = account.AccountNo,
                accountName = account.AccountName
            });
        }

        // GET: MainAccounts/GetNextAccountNo?parentId=5  (omit or 0 for root)
        [HttpGet]
        public async Task<IActionResult> GetNextAccountNo(int? parentId)
        {
            if (parentId == null || parentId == 0)
            {
                var rootNos = await _context.MainAccounts
                    .Where(a => a.ParentAccountId == null)
                    .Select(a => a.AccountNo)
                    .ToListAsync();

                int maxNo = 0;
                foreach (var no in rootNos)
                    if (int.TryParse(no, out int n) && n > maxNo)
                        maxNo = n;

                return Json(new { accountNo = (maxNo + 1).ToString() });
            }
            else
            {
                var parent = await _context.MainAccounts.FindAsync(parentId.Value);
                if (parent == null)
                    return Json(new { accountNo = "" });

                string parentNo = parent.AccountNo;

                var siblingNos = await _context.MainAccounts
                    .Where(a => a.ParentAccountId == parentId.Value)
                    .Select(a => a.AccountNo)
                    .ToListAsync();

                int maxSuffix = 0;
                foreach (var no in siblingNos)
                {
                    if (no.StartsWith(parentNo) && no.Length == parentNo.Length + 2)
                    {
                        string suffix = no.Substring(parentNo.Length);
                        if (int.TryParse(suffix, out int s) && s > maxSuffix)
                            maxSuffix = s;
                    }
                }

                string nextSuffix = (maxSuffix + 1).ToString("D2");
                return Json(new { accountNo = parentNo + nextSuffix });
            }
        }

        // POST: MainAccounts/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            int id,
            string accountNo,
            string accountName,
            string accountEffect,
            bool treatsAsCashAccount,
            bool active,
            DateTime openAccountDate,
            string? note,
            int? parentAccountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountNo) || string.IsNullOrWhiteSpace(accountName))
                    return Json(new { success = false, message = "Account No and Account Name are required." });

                // Check for duplicate AccountNo (excluding current record on edit)
                var duplicate = await _context.MainAccounts
                    .AnyAsync(a => a.AccountNo == accountNo && a.Id != id);
                if (duplicate)
                    return Json(new { success = false, message = $"Account No '{accountNo}' already exists." });

                if (id == 0)
                {
                    var account = new MainAccount
                    {
                        AccountNo = accountNo,
                        AccountName = accountName,
                        AccountEffect = accountEffect,
                        TreatsAsCashAccount = treatsAsCashAccount,
                        Active = active,
                        OpenAccountDate = openAccountDate,
                        Note = note,
                        ParentAccountId = parentAccountId,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    _context.MainAccounts.Add(account);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Account created successfully.", id = account.Id });
                }
                else
                {
                    var account = await _context.MainAccounts.FindAsync(id);
                    if (account == null)
                        return Json(new { success = false, message = "Account not found." });

                    account.AccountNo = accountNo;
                    account.AccountName = accountName;
                    account.AccountEffect = accountEffect;
                    account.TreatsAsCashAccount = treatsAsCashAccount;
                    account.Active = active;
                    account.OpenAccountDate = openAccountDate;
                    account.Note = note;
                    account.ParentAccountId = parentAccountId;
                    account.ModifiedDate = DateTime.Now;

                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Account updated successfully.", id = account.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: MainAccounts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var account = await _context.MainAccounts
                    .Include(a => a.Children)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (account == null)
                    return Json(new { success = false, message = "Account not found." });

                if (account.Children.Any())
                    return Json(new { success = false, message = "Cannot delete account with sub-accounts. Delete sub-accounts first." });

                _context.MainAccounts.Remove(account);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Account deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: MainAccounts/GetTree - returns accounts as JSON tree
        [HttpGet]
        public async Task<IActionResult> GetTree()
        {
            var accounts = await _context.MainAccounts
                .OrderBy(a => a.AccountNo)
                .Select(a => new
                {
                    id = a.Id,
                    accountNo = a.AccountNo,
                    accountName = a.AccountName,
                    active = a.Active,
                    parentAccountId = a.ParentAccountId,
                    hasChildren = a.Children.Any()
                })
                .ToListAsync();

            return Json(accounts);
        }
    }
}