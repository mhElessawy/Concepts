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

        // GET: Vouchers
        public async Task<IActionResult> Index()
        {
            var vouchers = await _context.VoucherHeaders
                .Where(v => v.Active)
                .Include(v => v.User)
                .OrderByDescending(v => v.VoucherDate)
                .ToListAsync();
            return View(vouchers);
        }

        // GET: Vouchers/Add
        public IActionResult Add()
        {
            var voucher = new VoucherHeader
            {
                VoucherDate = DateTime.Now,
                VoucherNo = GenerateVoucherNo()
            };
            return View(voucher);
        }

        // POST: Vouchers/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(VoucherHeader voucher)
        {
            if (ModelState.IsValid)
            {
                voucher.CreatedDate = DateTime.Now;
                voucher.ModifiedDate = DateTime.Now;
                voucher.Active = true;
                _context.Add(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: Vouchers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var voucher = await _context.VoucherHeaders
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (voucher == null) return NotFound();

            return View(voucher);
        }

        // POST: Vouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voucher = await _context.VoucherHeaders.FindAsync(id);
            if (voucher != null)
            {
                voucher.Active = false;
                voucher.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private string GenerateVoucherNo()
        {
            var count = _context.VoucherHeaders.Count() + 1;
            return $"VCH-{DateTime.Now:yyyyMM}-{count:D4}";
        }
    }
}
