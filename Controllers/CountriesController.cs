using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            var countries = await _context.DeffCountries
                .Where(c => c.Active)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(countries);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeffCountry country)
        {
            if (ModelState.IsValid)
            {
                country.CreatedDate = DateTime.Now;
                country.ModifiedDate = DateTime.Now;
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var country = await _context.DeffCountries.FindAsync(id);
            if (country == null) return NotFound();

            return View(country);
        }

        // POST: Countries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeffCountry country)
        {
            if (id != country.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    country.ModifiedDate = DateTime.Now;
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var country = await _context.DeffCountries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null) return NotFound();

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.DeffCountries.FindAsync(id);
            if (country != null)
            {
                country.Active = false;
                country.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return _context.DeffCountries.Any(e => e.Id == id);
        }
    }
}