using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class SubUOMController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubUOMController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubUOM
        public async Task<IActionResult> Index()
        {
            var subUOMs = await _context.DefSubUOMs
                .Include(su => su.UOM)
                .Where(su => su.Active)
                .OrderBy(su => su.SubUOMName)
                .ToListAsync();
            return View(subUOMs);
        }

        // GET: SubUOM/Create
        public IActionResult Create()
        {
            ViewBag.UOMs = _context.DefUOMs.Where(u => u.Active).ToList();
            return View();
        }

        // POST: SubUOM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefSubUOM subUOM)
        {
            if (ModelState.IsValid)
            {
                subUOM.CreatedDate = DateTime.Now;
                subUOM.ModifiedDate = DateTime.Now;
                _context.Add(subUOM);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UOMs = _context.DefUOMs.Where(u => u.Active).ToList();
            return View(subUOM);
        }

        // GET: SubUOM/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subUOM = await _context.DefSubUOMs.FindAsync(id);
            if (subUOM == null) return NotFound();

            ViewBag.UOMs = _context.DefUOMs.Where(u => u.Active).ToList();
            return View(subUOM);
        }

        // POST: SubUOM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DefSubUOM subUOM)
        {
            if (id != subUOM.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    subUOM.ModifiedDate = DateTime.Now;
                    _context.Update(subUOM);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubUOMExists(subUOM.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UOMs = _context.DefUOMs.Where(u => u.Active).ToList();
            return View(subUOM);
        }

        // GET: SubUOM/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var subUOM = await _context.DefSubUOMs
                .Include(su => su.UOM)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subUOM == null) return NotFound();

            return View(subUOM);
        }

        // POST: SubUOM/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subUOM = await _context.DefSubUOMs.FindAsync(id);
            if (subUOM != null)
            {
                subUOM.Active = false;
                subUOM.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubUOMExists(int id)
        {
            return _context.DefSubUOMs.Any(e => e.Id == id);
        }
    }
}
