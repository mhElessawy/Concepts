using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class BanksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Banks
        public async Task<IActionResult> Index()
        {
            var banks = await _context.DefBanks
                .Where(b => b.Active)
                .OrderBy(b => b.BankName)
                .ToListAsync();
            return View(banks);
        }

        // GET: Banks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Banks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefBank bank)
        {
            if (ModelState.IsValid)
            {
                bank.CreatedDate = DateTime.Now;
                bank.ModifiedDate = DateTime.Now;
                _context.Add(bank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bank);
        }

        // GET: Banks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bank = await _context.DefBanks.FindAsync(id);
            if (bank == null) return NotFound();

            return View(bank);
        }

        // POST: Banks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DefBank bank)
        {
            if (id != bank.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    bank.ModifiedDate = DateTime.Now;
                    _context.Update(bank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankExists(bank.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bank);
        }

        // GET: Banks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bank = await _context.DefBanks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bank == null) return NotFound();

            return View(bank);
        }

        // POST: Banks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bank = await _context.DefBanks.FindAsync(id);
            if (bank != null)
            {
                bank.Active = false;
                bank.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BankExists(int id)
        {
            return _context.DefBanks.Any(e => e.Id == id);
        }
    }
}