using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class OpeningVouchersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpeningVouchersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vouchers = await _context.OpeningVoucherHeaders
                .Where(v => v.Active)
                .OrderByDescending(v => v.VoucherDate)
                .ThenByDescending(v => v.VoucherNo)
                .ToListAsync();
            return View(vouchers);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNextVoucherNo()
        {
            int year = DateTime.Now.Year;
            string yearPart = (year % 100).ToString("D2");
            var prefix = $"OV{yearPart}";

            var existing = await _context.OpeningVoucherHeaders
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

        [HttpGet]
        public async Task<IActionResult> GetVoucher(string voucherNo)
        {
            var header = await _context.OpeningVoucherHeaders
                .Include(v => v.Details)
                    .ThenInclude(d => d.CostCenter)
                .FirstOrDefaultAsync(v => v.VoucherNo == voucherNo);

            if (header == null)
                return Json(new { success = false, message = "Opening Voucher not found." });

            return Json(new
            {
                success = true,
                id = header.Id,
                voucherNo = header.VoucherNo,
                voucherDate = header.VoucherDate.ToString("yyyy-MM-dd"),
                relayVoucher = header.RelayVoucher,
                statement = header.Statement ?? "",
                document = header.Document ?? "",
                note = header.Note ?? "",
                totalDebit = header.TotalDebit,
                totalCredit = header.TotalCredit,
                details = header.Details.Select(d => new
                {
                    id = d.Id,
                    accountNumber = d.AccountNumber,
                    accountName = d.AccountName,
                    debit = d.Debit,
                    credit = d.Credit,
                    description = d.Description ?? "",
                    costCenterId = d.CostCenterId ?? 0,
                    costCenterName = d.CostCenter != null ? d.CostCenter.CostCenterName : (d.CostCenterName ?? "")
                }).ToList()
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchAccounts(string? term)
        {
            var query = _context.ChildAccounts.AsQueryable();

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
                    accountName = a.AccountName
                })
                .ToListAsync();

            return Json(results);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] OpeningVoucherSaveRequest request)
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
                    var duplicate = await _context.OpeningVoucherHeaders.AnyAsync(v => v.VoucherNo == request.VoucherNo);
                    if (duplicate)
                        return Json(new { success = false, message = $"Voucher No '{request.VoucherNo}' already exists." });

                    var header = new OpeningVoucherHeader
                    {
                        VoucherNo = request.VoucherNo,
                        VoucherDate = request.VoucherDate,
                        RelayVoucher = request.RelayVoucher,
                        Statement = request.Statement,
                        Document = request.Document,
                        Note = request.Note,
                        TotalDebit = totalDebit,
                        TotalCredit = totalCredit,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    foreach (var d in request.Details)
                    {
                        header.Details.Add(new OpeningVoucherDetails
                        {
                            AccountNumber = d.AccountNumber,
                            AccountName = d.AccountName,
                            Debit = d.Debit,
                            Credit = d.Credit,
                            Description = d.Description,
                            ChildAccountId = d.ChildAccountId > 0 ? d.ChildAccountId : null,
                            CostCenterId = d.CostCenterId > 0 ? d.CostCenterId : null,
                            CostCenterName = d.CostCenterName
                        });
                    }

                    _context.OpeningVoucherHeaders.Add(header);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Opening Voucher saved successfully.", id = header.Id });
                }
                else
                {
                    var header = await _context.OpeningVoucherHeaders
                        .Include(v => v.Details)
                        .FirstOrDefaultAsync(v => v.Id == request.Id);

                    if (header == null)
                        return Json(new { success = false, message = "Opening Voucher not found." });

                    if (header.VoucherNo != request.VoucherNo)
                    {
                        var duplicate = await _context.OpeningVoucherHeaders.AnyAsync(v => v.VoucherNo == request.VoucherNo && v.Id != request.Id);
                        if (duplicate)
                            return Json(new { success = false, message = $"Voucher No '{request.VoucherNo}' already exists." });
                    }

                    header.VoucherNo = request.VoucherNo;
                    header.VoucherDate = request.VoucherDate;
                    header.RelayVoucher = request.RelayVoucher;
                    header.Statement = request.Statement;
                    header.Document = request.Document;
                    header.Note = request.Note;
                    header.TotalDebit = totalDebit;
                    header.TotalCredit = totalCredit;
                    header.ModifiedDate = DateTime.Now;

                    _context.OpeningVoucherDetails.RemoveRange(header.Details);
                    header.Details.Clear();

                    foreach (var d in request.Details)
                    {
                        header.Details.Add(new OpeningVoucherDetails
                        {
                            AccountNumber = d.AccountNumber,
                            AccountName = d.AccountName,
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
                    return Json(new { success = true, message = "Opening Voucher updated successfully.", id = header.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var header = await _context.OpeningVoucherHeaders.FindAsync(id);
                if (header == null)
                    return Json(new { success = false, message = "Opening Voucher not found." });

                _context.OpeningVoucherHeaders.Remove(header);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Opening Voucher deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class OpeningVoucherSaveRequest
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; }
        public bool RelayVoucher { get; set; }
        public string? Statement { get; set; }
        public string? Document { get; set; }
        public string? Note { get; set; }
        public List<OpeningVoucherDetailRequest> Details { get; set; } = new();
    }

    public class OpeningVoucherDetailRequest
    {
        public int ChildAccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
        public int CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
    }
}