using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class UOMController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UOMController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UOM
        public async Task<IActionResult> Index()
        {
            var uoms = await _context.DefUOMs
                .Where(u => u.Active)
                .OrderBy(u => u.UOMName)
                .ToListAsync();
            return View(uoms);
        }

        // GET: UOM/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UOM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefUOM uom)
        {
            if (ModelState.IsValid)
            {
                uom.CreatedDate = DateTime.Now;
                uom.ModifiedDate = DateTime.Now;
                _context.Add(uom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uom);
        }

        // GET: UOM/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var uom = await _context.DefUOMs.FindAsync(id);
            if (uom == null) return NotFound();

            return View(uom);
        }

        // POST: UOM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DefUOM uom)
        {
            if (id != uom.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    uom.ModifiedDate = DateTime.Now;
                    _context.Update(uom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UOMExists(uom.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(uom);
        }

        // GET: UOM/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var uom = await _context.DefUOMs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uom == null) return NotFound();

            return View(uom);
        }

        // POST: UOM/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uom = await _context.DefUOMs.FindAsync(id);
            if (uom != null)
            {
                uom.Active = false;
                uom.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UOMExists(int id)
        {
            return _context.DefUOMs.Any(e => e.Id == id);
        }
    }
}