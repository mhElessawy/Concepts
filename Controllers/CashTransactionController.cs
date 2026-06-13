using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class CashTransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashTransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CashTransaction/Add
        public IActionResult Add()
        {
            return View();
        }

        // GET: CashTransaction/GetNextInvoiceNo
        [HttpGet]
        public async Task<IActionResult> GetNextInvoiceNo()
        {
            try
            {
                int year = DateTime.Now.Year;
                string prefix = $"PV{(year % 100):D2}";   // e.g.  PV26

                var existing = await _context.CashTransactionHeaders
                    .Where(h => h.InvoiceNo.StartsWith(prefix))
                    .Select(h => h.InvoiceNo)
                    .ToListAsync();

                int maxSeq = 0;
                foreach (var no in existing)
                {
                    string suffix = no.Substring(prefix.Length);
                    if (int.TryParse(suffix, out int seq) && seq > maxSeq)
                        maxSeq = seq;
                }

                return Json(new { success = true, invoiceNo = $"{prefix}{(maxSeq + 1):D4}" }); // PV260001, PV260002 …
            }
            catch (Exception ex)
            {
                return Json(new { success = false, invoiceNo = (string?)null, message = ex.Message });
            }
        }

        // GET: CashTransaction/GetTransaction?invoiceNo=CW260001
        [HttpGet]
        public async Task<IActionResult> GetTransaction(string invoiceNo)
        {
            var header = await _context.CashTransactionHeaders
                .Include(h => h.Details)
                .FirstOrDefaultAsync(h => h.InvoiceNo == invoiceNo);

            if (header == null)
                return Json(new { success = false, message = "Invoice not found." });

            return Json(new
            {
                success = true,
                id = header.Id,
                invoiceNo = header.InvoiceNo,
                transactionType = header.TransactionType,
                cashName = header.CashName,
                accountInfo = header.AccountInfo,
                payTo = header.PayTo,
                relatedVoucherNo = header.RelatedVoucherNo ?? "",
                amount = header.Amount,
                discount = header.Discount,
                amountAfterDiscount = header.AmountAfterDiscount,
                discountNote = header.DiscountNote ?? "",
                paymentMethod = header.PaymentMethod,
                transactionDate = header.TransactionDate.ToString("yyyy-MM-dd"),
                note = header.Note ?? "",
                details = header.Details.Select(d => new
                {
                    id = d.Id,
                    entityName = d.EntityName ?? "",
                    entityCode = d.EntityCode ?? "",
                    entityId = d.EntityId ?? 0,
                    invoiceNo = d.InvoiceNo ?? "",
                    receivedDate = d.ReceivedDate.HasValue ? d.ReceivedDate.Value.ToString("yyyy-MM-dd") : "",
                    department = d.Department ?? "",
                    costCenter = d.CostCenter ?? "",
                    amount = d.Amount,
                    note = d.Note ?? ""
                }).ToList()
            });
        }

        // GET: CashTransaction/GetCashAccounts  (used for Cash Name dropdown - ChildAccounts where AccountType = Cash In Hand)
        [HttpGet]
        public async Task<IActionResult> GetCashAccounts()
        {
            var list = await _context.ChildAccounts
                .Include(a => a.AccountType)
                .Where(a => a.Active && a.AccountType != null &&
                            a.AccountType.Name.ToLower() == "cash in hand")
                .OrderBy(a => a.AccountNo)
                .Select(a => new { id = a.Id, accountNo = a.AccountNo, accountName = a.AccountName })
                .ToListAsync();
            return Json(list);
        }

        // GET: CashTransaction/GetNextRelatedVoucherNo
        [HttpGet]
        public async Task<IActionResult> GetNextRelatedVoucherNo()
        {
            int year = DateTime.Now.Year;
            string yearPart = (year % 100).ToString("D2");
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

        // GET: CashTransaction/GetDepartments
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var list = await _context.DeffDepartments
                .Where(d => d.Active)
                .OrderBy(d => d.DepartmentCode)
                .Select(d => new { id = d.Id, code = d.DepartmentCode, name = d.DepartmentName })
                .ToListAsync();
            return Json(list);
        }

        // GET: CashTransaction/GetCostCenters
        [HttpGet]
        public async Task<IActionResult> GetCostCenters()
        {
            var list = await _context.DeffCostCenters
                .Where(c => c.Active)
                .OrderBy(c => c.CostCenterCode)
                .Select(c => new { id = c.Id, code = c.CostCenterCode, name = c.CostCenterName })
                .ToListAsync();
            return Json(list);
        }

        // GET: CashTransaction/SearchAccounts?term=xxx&costCenterId=0
        [HttpGet]
        public async Task<IActionResult> SearchAccounts(string? term, int? costCenterId)
        {
            var query = _context.ChildAccounts
                .Include(a => a.ParentAccount)
                .Include(a => a.NatureOfAccount)
                .Include(a => a.CostCenter)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(a => a.AccountNo.Contains(term) || a.AccountName.Contains(term));

            if (costCenterId.HasValue && costCenterId.Value > 0)
                query = query.Where(a => a.CostCenterId == costCenterId.Value);

            var results = await query
                .Where(a => a.Active)
                .OrderBy(a => a.AccountNo)
                .Take(100)
                .Select(a => new
                {
                    id = a.Id,
                    accountNo = a.AccountNo,
                    accountName = a.AccountName,
                    parentName = a.ParentAccount != null ? a.ParentAccount.AccountName : "",
                    natureOfAccount = a.NatureOfAccount != null ? a.NatureOfAccount.Name : "",
                    costCenter = a.CostCenter != null ? a.CostCenter.CostCenterName : ""
                })
                .ToListAsync();

            return Json(results);
        }

        // GET: CashTransaction/GetSuppliers
        [HttpGet]
        public async Task<IActionResult> GetSuppliers(string? term)
        {
            var query = _context.Venders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(v => v.VenderCode.Contains(term) || v.VenderName.Contains(term));

            var results = await query
                .Where(v => v.Active)
                .OrderBy(v => v.VenderName)
                .Take(100)
                .Select(v => new
                {
                    id = v.Id,
                    code = v.VenderCode,
                    name = v.VenderName
                })
                .ToListAsync();

            return Json(results);
        }

        // POST: CashTransaction/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] CashTransactionSaveRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.InvoiceNo))
                    return Json(new { success = false, message = "Invoice No is required." });

                if (request.Id == 0)
                {
                    var duplicate = await _context.CashTransactionHeaders.AnyAsync(h => h.InvoiceNo == request.InvoiceNo);
                    if (duplicate)
                        return Json(new { success = false, message = $"Invoice No '{request.InvoiceNo}' already exists." });

                    var header = new CashTransactionHeader
                    {
                        InvoiceNo = request.InvoiceNo,
                        TransactionType = request.TransactionType,
                        CashName = request.CashName ?? "",
                        AccountInfo = request.AccountInfo ?? "",
                        PayTo = request.PayTo ?? "Suppliers",
                        RelatedVoucherNo = request.RelatedVoucherNo,
                        Amount = request.Amount,
                        Discount = request.Discount,
                        AmountAfterDiscount = request.AmountAfterDiscount,
                        DiscountNote = request.DiscountNote,
                        PaymentMethod = request.PaymentMethod ?? "Cash",
                        TransactionDate = request.TransactionDate,
                        Note = request.Note,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    foreach (var d in request.Details ?? new List<CashTransactionDetailRequest>())
                    {
                        header.Details.Add(new CashTransactionDetail
                        {
                            EntityName = d.EntityName,
                            EntityCode = d.EntityCode,
                            EntityId = d.EntityId > 0 ? d.EntityId : null,
                            InvoiceNo = d.InvoiceNo,
                            ReceivedDate = string.IsNullOrEmpty(d.ReceivedDate) ? null : DateTime.Parse(d.ReceivedDate),
                            Department = d.Department,
                            CostCenter = d.CostCenter,
                            Amount = d.Amount,
                            Note = d.Note
                        });
                    }

                    _context.CashTransactionHeaders.Add(header);
                    await _context.SaveChangesAsync();

                    if (header.PayTo == "Account Tree" && !string.IsNullOrEmpty(header.RelatedVoucherNo))
                    {
                        var voucher = await BuildVoucherAsync(header, header.Details.ToList());
                        if (voucher != null)
                        {
                            _context.VoucherHeaders.Add(voucher);
                            await _context.SaveChangesAsync();
                        }
                    }

                    return Json(new { success = true, message = "Saved successfully.", id = header.Id });
                }
                else
                {
                    var header = await _context.CashTransactionHeaders
                        .Include(h => h.Details)
                        .FirstOrDefaultAsync(h => h.Id == request.Id);

                    if (header == null)
                        return Json(new { success = false, message = "Record not found." });

                    if (header.InvoiceNo != request.InvoiceNo)
                    {
                        var duplicate = await _context.CashTransactionHeaders.AnyAsync(h => h.InvoiceNo == request.InvoiceNo && h.Id != request.Id);
                        if (duplicate)
                            return Json(new { success = false, message = $"Invoice No '{request.InvoiceNo}' already exists." });
                    }

                    string? existingVoucherNo = header.RelatedVoucherNo;

                    header.InvoiceNo = request.InvoiceNo;
                    header.TransactionType = request.TransactionType;
                    header.CashName = request.CashName ?? "";
                    header.AccountInfo = request.AccountInfo ?? "";
                    header.PayTo = request.PayTo ?? "Suppliers";
                    header.RelatedVoucherNo = request.RelatedVoucherNo ?? existingVoucherNo;
                    header.Amount = request.Amount;
                    header.Discount = request.Discount;
                    header.AmountAfterDiscount = request.AmountAfterDiscount;
                    header.DiscountNote = request.DiscountNote;
                    header.PaymentMethod = request.PaymentMethod ?? "Cash";
                    header.TransactionDate = request.TransactionDate;
                    header.Note = request.Note;
                    header.ModifiedDate = DateTime.Now;

                    _context.CashTransactionDetails.RemoveRange(header.Details);
                    header.Details.Clear();

                    foreach (var d in request.Details ?? new List<CashTransactionDetailRequest>())
                    {
                        header.Details.Add(new CashTransactionDetail
                        {
                            EntityName = d.EntityName,
                            EntityCode = d.EntityCode,
                            EntityId = d.EntityId > 0 ? d.EntityId : null,
                            InvoiceNo = d.InvoiceNo,
                            ReceivedDate = string.IsNullOrEmpty(d.ReceivedDate) ? null : DateTime.Parse(d.ReceivedDate),
                            Department = d.Department,
                            CostCenter = d.CostCenter,
                            Amount = d.Amount,
                            Note = d.Note
                        });
                    }

                    _context.Update(header);
                    await _context.SaveChangesAsync();

                    if (header.PayTo == "Account Tree" && !string.IsNullOrEmpty(header.RelatedVoucherNo))
                    {
                        // Delete old voucher then recreate with updated data
                        if (!string.IsNullOrEmpty(existingVoucherNo))
                        {
                            var oldVoucher = await _context.VoucherHeaders
                                .FirstOrDefaultAsync(v => v.VoucherNo == existingVoucherNo);
                            if (oldVoucher != null)
                            {
                                _context.VoucherHeaders.Remove(oldVoucher);
                                await _context.SaveChangesAsync();
                            }
                        }

                        var voucher = await BuildVoucherAsync(header, header.Details.ToList());
                        if (voucher != null)
                        {
                            _context.VoucherHeaders.Add(voucher);
                            await _context.SaveChangesAsync();
                        }
                    }

                    return Json(new { success = true, message = "Updated successfully.", id = header.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: CashTransaction/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var header = await _context.CashTransactionHeaders.FindAsync(id);
                if (header == null)
                    return Json(new { success = false, message = "Record not found." });

                // Delete associated voucher if any
                if (!string.IsNullOrEmpty(header.RelatedVoucherNo))
                {
                    var voucher = await _context.VoucherHeaders
                        .FirstOrDefaultAsync(v => v.VoucherNo == header.RelatedVoucherNo);
                    if (voucher != null)
                        _context.VoucherHeaders.Remove(voucher);
                }

                _context.CashTransactionHeaders.Remove(header);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<VoucherHeader?> BuildVoucherAsync(CashTransactionHeader header, List<CashTransactionDetail> details)
        {
            // CashName stores ChildAccount.AccountNo
            ChildAccount? cashChildAcct = null;
            if (!string.IsNullOrEmpty(header.CashName))
                cashChildAcct = await _context.ChildAccounts
                    .Include(a => a.NatureOfAccount)
                    .FirstOrDefaultAsync(a => a.AccountNo == header.CashName);

            // Discount account
            ChildAccount? discountAcct = null;
            if (header.Discount > 0)
                discountAcct = await _context.ChildAccounts
                    .Include(a => a.NatureOfAccount)
                    .FirstOrDefaultAsync(a => a.AccountNo == "101010101003");

            decimal totalDebit = details.Sum(d => d.Amount);
            decimal totalCredit = header.AmountAfterDiscount + (header.Discount > 0 ? header.Discount : 0);

            var voucher = new VoucherHeader
            {
                VoucherNo = header.RelatedVoucherNo!,
                VoucherDate = header.TransactionDate,
                Statement = $"Cash With Draw - {header.InvoiceNo}",
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            // Debit entries from Account Tree Lines
            foreach (var d in details)
            {
                ChildAccount? lineAcct = d.EntityId.HasValue
                    ? await _context.ChildAccounts
                        .Include(a => a.NatureOfAccount)
                        .FirstOrDefaultAsync(a => a.Id == d.EntityId.Value)
                    : null;

                int? costCenterId = null;
                if (!string.IsNullOrEmpty(d.CostCenter))
                {
                    var cc = await _context.DeffCostCenters
                        .FirstOrDefaultAsync(c => c.CostCenterName == d.CostCenter);
                    costCenterId = cc?.Id;
                }

                voucher.Details.Add(new VoucherDetails
                {
                    ChildAccountId = d.EntityId,
                    AccountNumber = d.EntityCode ?? "",
                    AccountName = d.EntityName ?? "",
                    NatureOfAccount = lineAcct?.NatureOfAccount?.Name ?? "",
                    Debit = d.Amount,
                    Credit = 0,
                    Description = d.Note,
                    CostCenterId = costCenterId,
                    CostCenterName = d.CostCenter
                });
            }

            // Credit entry: Cash Name account
            voucher.Details.Add(new VoucherDetails
            {
                ChildAccountId = cashChildAcct?.Id,
                AccountNumber = cashChildAcct?.AccountNo ?? "",
                AccountName = cashChildAcct?.AccountName ?? "",
                NatureOfAccount = cashChildAcct?.NatureOfAccount?.Name ?? "",
                Debit = 0,
                Credit = header.AmountAfterDiscount,
                Description = header.InvoiceNo
            });

            // Credit entry: Discount account (101010101003)
            if (header.Discount > 0)
            {
                voucher.Details.Add(new VoucherDetails
                {
                    ChildAccountId = discountAcct?.Id,
                    AccountNumber = discountAcct?.AccountNo ?? "101010101003",
                    AccountName = discountAcct?.AccountName ?? "Discount",
                    NatureOfAccount = discountAcct?.NatureOfAccount?.Name ?? "",
                    Debit = 0,
                    Credit = header.Discount,
                    Description = header.DiscountNote
                });
            }

            return voucher;
        }
    }

    public class CashTransactionSaveRequest
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public string TransactionType { get; set; } = "CashWithDraw";
        public string? CashName { get; set; }
        public string? AccountInfo { get; set; }
        public string? PayTo { get; set; }
        public string? RelatedVoucherNo { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public string? DiscountNote { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Note { get; set; }
        public List<CashTransactionDetailRequest>? Details { get; set; }
    }

    public class CashTransactionDetailRequest
    {
        public string? EntityName { get; set; }
        public string? EntityCode { get; set; }
        public int EntityId { get; set; }
        public string? InvoiceNo { get; set; }
        public string? ReceivedDate { get; set; }
        public string? Department { get; set; }
        public string? CostCenter { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}