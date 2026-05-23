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
            int year     = DateTime.Now.Year;
            string prefix = $"CT{(year % 100):D2}";   // e.g.  CT26

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

            return Json(new { invoiceNo = $"{prefix}{(maxSeq + 1):D4}" }); // CT260001, CT260002 …
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
                    amount = d.Amount,
                    note = d.Note ?? ""
                }).ToList()
            });
        }

        // GET: CashTransaction/GetAccountTypes  (used for Cash Name dropdown)
        [HttpGet]
        public async Task<IActionResult> GetAccountTypes()
        {
            var list = await _context.DefAccountTypes
                .Where(a => a.Active)
                .OrderBy(a => a.Code)
                .Select(a => new { id = a.Id, code = a.Code, name = a.Name })
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

        // GET: CashTransaction/SearchAccounts?term=xxx
        [HttpGet]
        public async Task<IActionResult> SearchAccounts(string? term)
        {
            var query = _context.ChildAccounts
                .Include(a => a.ParentAccount)
                .Include(a => a.NatureOfAccount)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(a => a.AccountNo.Contains(term) || a.AccountName.Contains(term));

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
                    natureOfAccount = a.NatureOfAccount != null ? a.NatureOfAccount.Name : ""
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
                            Amount = d.Amount,
                            Note = d.Note
                        });
                    }

                    _context.CashTransactionHeaders.Add(header);
                    await _context.SaveChangesAsync();
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

                    header.InvoiceNo = request.InvoiceNo;
                    header.TransactionType = request.TransactionType;
                    header.CashName = request.CashName ?? "";
                    header.AccountInfo = request.AccountInfo ?? "";
                    header.PayTo = request.PayTo ?? "Suppliers";
                    header.RelatedVoucherNo = request.RelatedVoucherNo;
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
                            Amount = d.Amount,
                            Note = d.Note
                        });
                    }

                    _context.Update(header);
                    await _context.SaveChangesAsync();
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

                _context.CashTransactionHeaders.Remove(header);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}