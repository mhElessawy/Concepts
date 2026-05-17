using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class VouchersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VouchersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vouchers/Add
        public IActionResult Add()
        {
            return View();
        }

        // GET: Vouchers/GetNextVoucherNo
        [HttpGet]
        public async Task<IActionResult> GetNextVoucherNo()
        {
            int year = DateTime.Now.Year;
            string yearPart = (year % 100).ToString("D2"); // "26" from 2026

            var prefix = $"V{yearPart}";

            var existing = await _context.VoucherHeaders
                .Where(v => v.VoucherNo.StartsWith(prefix))
                .Select(v => v.VoucherNo)
                .ToListAsync();

            int maxSeq = 0;
            foreach (var no in existing)
            {
                string suffix = no.Substring(prefix.Length);
                if (int.TryParse(suffix, out int seq) && seq > maxSeq)
                    maxSeq = seq;
            }

            string nextNo = prefix + (maxSeq + 1).ToString("D4");
            return Json(new { voucherNo = nextNo });
        }

        // GET: Vouchers/GetVoucher?voucherNo=V260001
        [HttpGet]
        public async Task<IActionResult> GetVoucher(string voucherNo)
        {
            var header = await _context.VoucherHeaders
                .Include(v => v.Details)
                    .ThenInclude(d => d.CostCenter)
                .FirstOrDefaultAsync(v => v.VoucherNo == voucherNo);

            if (header == null)
                return Json(new { success = false, message = "Voucher not found." });

            return Json(new
            {
                success = true,
                id = header.Id,
                voucherNo = header.VoucherNo,
                voucherDate = header.VoucherDate.ToString("yyyy-MM-dd"),
                accountingSettlement = header.AccountingSettlement,
                settlementYear = header.SettlementYear,
                statement = header.Statement ?? "",
                posting = header.Posting,
                approved = header.Approved,
                totalDebit = header.TotalDebit,
                totalCredit = header.TotalCredit,
                details = header.Details.Select(d => new
                {
                    id = d.Id,
                    accountNumber = d.AccountNumber,
                    accountName = d.AccountName,
                    natureOfAccount = d.NatureOfAccount,
                    debit = d.Debit,
                    credit = d.Credit,
                    description = d.Description ?? "",
                    costCenterId = d.CostCenterId ?? 0,
                    costCenterName = d.CostCenter != null ? d.CostCenter.CostCenterName : (d.CostCenterName ?? "")
                }).ToList()
            });
        }

        // GET: Vouchers/SearchAccounts?term=xxx
        [HttpGet]
        public async Task<IActionResult> SearchAccounts(string? term)
        {
            var query = _context.ChildAccounts
                .Include(a => a.NatureOfAccount)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(a => a.AccountNo.Contains(term) || a.AccountName.Contains(term));

            var results = await query
                .Where(a => a.Active)
                .OrderBy(a => a.AccountNo)
                .Take(50)
                .Select(a => new
                {
                    id = a.Id,
                    accountNo = a.AccountNo,
                    accountName = a.AccountName,
                    natureOfAccount = a.NatureOfAccount != null ? a.NatureOfAccount.Name : ""
                })
                .ToListAsync();

            return Json(results);
        }

        // GET: Vouchers/GetCostCenters
        [HttpGet]
        public async Task<IActionResult> GetCostCenters()
        {
            var list = await _context.DeffCostCenters
                .Where(c => c.Active)
                .OrderBy(c => c.CostCenterCode)
                .Select(c => new
                {
                    id = c.Id,
                    code = c.CostCenterCode,
                    name = c.CostCenterName
                })
                .ToListAsync();
            return Json(list);
        }

        // POST: Vouchers/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] VoucherSaveRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.VoucherNo))
                    return Json(new { success = false, message = "Voucher No is required." });

                if (request.Details == null || request.Details.Count == 0)
                    return Json(new { success = false, message = "Please add at least one account line." });

                decimal totalDebit = request.Details.Sum(d => d.Debit);
                decimal totalCredit = request.Details.Sum(d => d.Credit);

                if (request.Id == 0)
                {
                    var duplicate = await _context.VoucherHeaders.AnyAsync(v => v.VoucherNo == request.VoucherNo);
                    if (duplicate)
                        return Json(new { success = false, message = $"Voucher No '{request.VoucherNo}' already exists." });

                    var header = new VoucherHeader
                    {
                        VoucherNo = request.VoucherNo,
                        VoucherDate = request.VoucherDate,
                        AccountingSettlement = request.AccountingSettlement,
                        SettlementYear = request.SettlementYear,
                        Statement = request.Statement,
                        Posting = request.Posting,
                        Approved = request.Approved,
                        TotalDebit = totalDebit,
                        TotalCredit = totalCredit,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    foreach (var d in request.Details)
                    {
                        header.Details.Add(new VoucherDetails
                        {
                            AccountNumber = d.AccountNumber,
                            AccountName = d.AccountName,
                            NatureOfAccount = d.NatureOfAccount,
                            Debit = d.Debit,
                            Credit = d.Credit,
                            Description = d.Description,
                            ChildAccountId = d.ChildAccountId > 0 ? d.ChildAccountId : null,
                            CostCenterId = d.CostCenterId > 0 ? d.CostCenterId : null,
                            CostCenterName = d.CostCenterName
                        });
                    }

                    _context.VoucherHeaders.Add(header);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Voucher saved successfully.", id = header.Id });
                }
                else
                {
                    var header = await _context.VoucherHeaders
                        .Include(v => v.Details)
                        .FirstOrDefaultAsync(v => v.Id == request.Id);

                    if (header == null)
                        return Json(new { success = false, message = "Voucher not found." });

                    if (header.VoucherNo != request.VoucherNo)
                    {
                        var duplicate = await _context.VoucherHeaders.AnyAsync(v => v.VoucherNo == request.VoucherNo && v.Id != request.Id);
                        if (duplicate)
                            return Json(new { success = false, message = $"Voucher No '{request.VoucherNo}' already exists." });
                    }

                    header.VoucherNo = request.VoucherNo;
                    header.VoucherDate = request.VoucherDate;
                    header.AccountingSettlement = request.AccountingSettlement;
                    header.SettlementYear = request.SettlementYear;
                    header.Statement = request.Statement;
                    header.Posting = request.Posting;
                    header.Approved = request.Approved;
                    header.TotalDebit = totalDebit;
                    header.TotalCredit = totalCredit;
                    header.ModifiedDate = DateTime.Now;

                    _context.VoucherDetails.RemoveRange(header.Details);
                    header.Details.Clear();

                    foreach (var d in request.Details)
                    {
                        header.Details.Add(new VoucherDetails
                        {
                            AccountNumber = d.AccountNumber,
                            AccountName = d.AccountName,
                            NatureOfAccount = d.NatureOfAccount,
                            Debit = d.Debit,
                            Credit = d.Credit,
                            Description = d.Description,
                            ChildAccountId = d.ChildAccountId > 0 ? d.ChildAccountId : null,
                            CostCenterId = d.CostCenterId > 0 ? d.CostCenterId : null,
                            CostCenterName = d.CostCenterName
                        });
                    }

                    _context.Update(header);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Voucher updated successfully.", id = header.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Vouchers/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var header = await _context.VoucherHeaders.FindAsync(id);
                if (header == null)
                    return Json(new { success = false, message = "Voucher not found." });

                _context.VoucherHeaders.Remove(header);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Voucher deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class VoucherSaveRequest
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; }
        public bool AccountingSettlement { get; set; }
        public int SettlementYear { get; set; }
        public string? Statement { get; set; }
        public bool Posting { get; set; }
        public bool Approved { get; set; }
        public List<VoucherDetailRequest> Details { get; set; } = new();
    }

    public class VoucherDetailRequest
    {
        public int ChildAccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string NatureOfAccount { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
        public int CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
    }
}