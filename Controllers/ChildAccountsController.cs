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
            var accounts = await _context.ChildAccounts
                .Include(a => a.ParentAccount)
                .OrderBy(a => a.AccountNo)
                .ToListAsync();
            return View(accounts);
        }

        // GET: ChildAccounts/GetMainAccountTree
        [HttpGet]
        public async Task<IActionResult> GetMainAccountTree()
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

        // GET: ChildAccounts/GetChildAccount/5
        [HttpGet]
        public async Task<IActionResult> GetChildAccount(int id)
        {
            var account = await _context.ChildAccounts
                .Include(a => a.ParentAccount)
                .Include(a => a.CostCenter)
                .Include(a => a.AccountType)
                .Include(a => a.AccountEffect)
                .Include(a => a.NatureOfAccount)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null) return NotFound();

            return Json(new
            {
                id = account.Id,
                accountNo = account.AccountNo,
                parentAccountId = account.ParentAccountId,
                parentAccountNo = account.ParentAccount?.AccountNo ?? "",
                parentAccountName = account.ParentAccount?.AccountName ?? "",
                accountName = account.AccountName,
                accountTypeId = account.AccountTypeId,
                accountTypeName = account.AccountType?.Name ?? "",
                accountEffectId = account.AccountEffectId,
                accountEffectName = account.AccountEffect?.Name ?? "",
                natureOfAccountId = account.NatureOfAccountId,
                natureOfAccountName = account.NatureOfAccount?.Name ?? "",
                treatsAsBankAccount = account.TreatsAsBankAccount,
                costCenterId = account.CostCenterId,
                costCenterCode = account.CostCenter?.CostCenterCode ?? "",
                costCenterName = account.CostCenter?.CostCenterName ?? "",
                fixedCostCenter = account.FixedCostCenter,
                address = account.Address ?? "",
                tel = account.Tel ?? "",
                mobile = account.Mobile ?? "",
                emailAddress = account.EmailAddress ?? "",
                civilId = account.CivilId ?? "",
                accountLimit = account.AccountLimit,
                receiptLimit = account.ReceiptLimit,
                name = account.Name ?? "",
                note = account.Note ?? "",
                active = account.Active
            });
        }

        // GET: ChildAccounts/GetNextAccountNo?parentId=5
        [HttpGet]
        public async Task<IActionResult> GetNextAccountNo(int parentId)
        {
            var parent = await _context.MainAccounts.FindAsync(parentId);
            if (parent == null)
                return Json(new { accountNo = "" });

            string parentNo = parent.AccountNo;

            var existingNos = await _context.ChildAccounts
                .Where(a => a.ParentAccountId == parentId)
                .Select(a => a.AccountNo)
                .ToListAsync();

            int maxSuffix = 0;
            foreach (var no in existingNos)
            {
                if (no.StartsWith(parentNo) && no.Length == parentNo.Length + 3)
                {
                    string suffix = no.Substring(parentNo.Length);
                    if (int.TryParse(suffix, out int s) && s > maxSuffix)
                        maxSuffix = s;
                }
            }

            string nextSuffix = (maxSuffix + 1).ToString("D3");
            return Json(new { accountNo = parentNo + nextSuffix });
        }

        // GET: ChildAccounts/GetCostCenterTree - for popup in Child Account form
        [HttpGet]
        public async Task<IActionResult> GetCostCenterTree()
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

        // GET: ChildAccounts/Search?accountNo=xxx&accountName=xxx
        [HttpGet]
        public async Task<IActionResult> Search(string? accountNo, string? accountName)
        {
            var query = _context.ChildAccounts
                .Include(a => a.ParentAccount)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(accountNo))
                query = query.Where(a => a.AccountNo.Contains(accountNo));

            if (!string.IsNullOrWhiteSpace(accountName))
                query = query.Where(a => a.AccountName.Contains(accountName));

            var results = await query
                .OrderBy(a => a.AccountNo)
                .Select(a => new
                {
                    id = a.Id,
                    accountNo = a.AccountNo,
                    accountName = a.AccountName,
                    parentAccountNo = a.ParentAccount.AccountNo,
                    parentAccountName = a.ParentAccount.AccountName,
                    active = a.Active
                })
                .ToListAsync();

            return Json(results);
        }

        // GET: ChildAccounts/GetByParent?parentId=5
        [HttpGet]
        public async Task<IActionResult> GetByParent(int parentId)
        {
            var accounts = await _context.ChildAccounts
                .Where(a => a.ParentAccountId == parentId)
                .OrderBy(a => a.AccountNo)
                .Select(a => new
                {
                    id = a.Id,
                    accountNo = a.AccountNo,
                    accountName = a.AccountName,
                    active = a.Active
                })
                .ToListAsync();
            return Json(accounts);
        }

        // POST: ChildAccounts/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            int id,
            string accountNo,
            int parentAccountId,
            string accountName,
            int? accountTypeId,
            int? accountEffectId,
            int? natureOfAccountId,
            bool treatsAsBankAccount,
            int? costCenterId,
            bool fixedCostCenter,
            string? address,
            string? tel,
            string? mobile,
            string? emailAddress,
            string? civilId,
            decimal accountLimit,
            decimal receiptLimit,
            string? name,
            string? note,
            bool active)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountNo))
                    return Json(new { success = false, message = "Account No is required." });
                if (string.IsNullOrWhiteSpace(accountName))
                    return Json(new { success = false, message = "Account Name is required." });
                if (parentAccountId == 0)
                    return Json(new { success = false, message = "Please select a Parent Account from the tree." });
                if (accountTypeId == null)
                    return Json(new { success = false, message = "Account Type is required." });

                var duplicate = await _context.ChildAccounts
                    .AnyAsync(a => a.AccountNo == accountNo && a.Id != id);
                if (duplicate)
                    return Json(new { success = false, message = $"Account No '{accountNo}' already exists." });

                if (id == 0)
                {
                    var account = new ChildAccount
                    {
                        AccountNo = accountNo,
                        ParentAccountId = parentAccountId,
                        AccountName = accountName,
                        AccountTypeId = accountTypeId,
                        AccountEffectId = accountEffectId,
                        NatureOfAccountId = natureOfAccountId,
                        TreatsAsBankAccount = treatsAsBankAccount,
                        CostCenterId = costCenterId,
                        FixedCostCenter = fixedCostCenter,
                        Address = address,
                        Tel = tel,
                        Mobile = mobile,
                        EmailAddress = emailAddress,
                        CivilId = civilId,
                        AccountLimit = accountLimit,
                        ReceiptLimit = receiptLimit,
                        Name = name,
                        Note = note,
                        Active = active,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    _context.ChildAccounts.Add(account);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Child account created successfully.", id = account.Id });
                }
                else
                {
                    var account = await _context.ChildAccounts.FindAsync(id);
                    if (account == null)
                        return Json(new { success = false, message = "Account not found." });

                    account.AccountNo = accountNo;
                    account.ParentAccountId = parentAccountId;
                    account.AccountName = accountName;
                    account.AccountTypeId = accountTypeId;
                    account.AccountEffectId = accountEffectId;
                    account.NatureOfAccountId = natureOfAccountId;
                    account.TreatsAsBankAccount = treatsAsBankAccount;
                    account.CostCenterId = costCenterId;
                    account.FixedCostCenter = fixedCostCenter;
                    account.Address = address;
                    account.Tel = tel;
                    account.Mobile = mobile;
                    account.EmailAddress = emailAddress;
                    account.CivilId = civilId;
                    account.AccountLimit = accountLimit;
                    account.ReceiptLimit = receiptLimit;
                    account.Name = name;
                    account.Note = note;
                    account.Active = active;
                    account.ModifiedDate = DateTime.Now;

                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Child account updated successfully.", id = account.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: ChildAccounts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var account = await _context.ChildAccounts.FindAsync(id);
                if (account == null)
                    return Json(new { success = false, message = "Account not found." });

                _context.ChildAccounts.Remove(account);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Account deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
