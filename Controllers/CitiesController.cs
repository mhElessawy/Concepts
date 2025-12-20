using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class CitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cities
        public async Task<IActionResult> Index()
        {
            var cities = await _context.DeffCities
                .Include(c => c.Country)
                .Where(c => c.Active)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(cities);
        }

        // GET: Cities/Create
        public IActionResult Create()
        {
            ViewBag.Countries = _context.DeffCountries.Where(c => c.Active).ToList();
            return View();
        }

        // POST: Cities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeffCity city)
        {
            if (ModelState.IsValid)
            {
                city.CreatedDate = DateTime.Now;
                city.ModifiedDate = DateTime.Now;
                _context.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = _context.DeffCountries.Where(c => c.Active).ToList();
            return View(city);
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var city = await _context.DeffCities.FindAsync(id);
            if (city == null) return NotFound();

            ViewBag.Countries = _context.DeffCountries.Where(c => c.Active).ToList();
            return View(city);
        }

        // POST: Cities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeffCity city)
        {
            if (id != city.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    city.ModifiedDate = DateTime.Now;
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = _context.DeffCountries.Where(c => c.Active).ToList();
            return View(city);
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var city = await _context.DeffCities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null) return NotFound();

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.DeffCities.FindAsync(id);
            if (city != null)
            {
                city.Active = false;
                city.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CityExists(int id)
        {
            return _context.DeffCities.Any(e => e.Id == id);
        }
    }
}