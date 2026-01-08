using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Concept.Data;

using Concept.Models;



namespace Concept.Controllers

{

    public class LocationsController : Controller

    {

        private readonly ApplicationDbContext _context;



        public LocationsController(ApplicationDbContext context)

        {

            _context = context;

        }



        // GET: Locations

        public async Task<IActionResult> Index()

        {

            var locations = await _context.DeffLocations

                .Where(l => l.Active)

                .OrderBy(l => l.LocationName)

                .ToListAsync();

            return View(locations);

        }



        // GET: Locations/Details/5

        public async Task<IActionResult> Details(int? id)

        {

            if (id == null)

            {

                return NotFound();

            }



            var location = await _context.DeffLocations

                .FirstOrDefaultAsync(m => m.Id == id);



            if (location == null)

            {

                return NotFound();

            }



            return View(location);

        }



        // GET: Locations/Create

        public IActionResult Create()

        {

            return View();

        }



        // POST: Locations/Create

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(DeffLocation location)

        {

            try

            {

                if (ModelState.IsValid)

                {

                    location.CreatedDate = DateTime.Now;

                    location.ModifiedDate = DateTime.Now;

                    _context.Add(location);

                    await _context.SaveChangesAsync();



                    TempData["SuccessMessage"] = "Location created successfully!";

                    return RedirectToAction(nameof(Index));

                }

                return View(location);

            }

            catch (Exception ex)

            {

                Console.WriteLine($"Error creating location: {ex.Message}");

                TempData["ErrorMessage"] = "Error creating location";

                return View(location);

            }

        }



        // GET: Locations/Edit/5

        public async Task<IActionResult> Edit(int? id)

        {

            if (id == null)

            {

                return NotFound();

            }



            var location = await _context.DeffLocations.FindAsync(id);

            if (location == null)

            {

                return NotFound();

            }



            return View(location);

        }



        // POST: Locations/Edit/5

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, DeffLocation location)

        {

            if (id != location.Id)

            {

                return NotFound();

            }



            try

            {

                if (ModelState.IsValid)

                {

                    location.ModifiedDate = DateTime.Now;

                    _context.Update(location);

                    await _context.SaveChangesAsync();



                    TempData["SuccessMessage"] = "Location updated successfully!";

                    return RedirectToAction(nameof(Index));

                }

                return View(location);

            }

            catch (DbUpdateConcurrencyException)

            {

                if (!LocationExists(location.Id))

                {

                    return NotFound();

                }

                else

                {

                    throw;

                }

            }

            catch (Exception ex)

            {

                Console.WriteLine($"Error updating location: {ex.Message}");

                TempData["ErrorMessage"] = "Error updating location";

                return View(location);

            }

        }



        // GET: Locations/Delete/5

        public async Task<IActionResult> Delete(int? id)

        {

            if (id == null)

            {

                return NotFound();

            }



            var location = await _context.DeffLocations

                .FirstOrDefaultAsync(m => m.Id == id);



            if (location == null)

            {

                return NotFound();

            }



            return View(location);

        }



        // POST: Locations/Delete/5

        [HttpPost, ActionName("Delete")]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)

        {

            try

            {

                var location = await _context.DeffLocations.FindAsync(id);

                if (location != null)

                {

                    // Soft delete

                    location.Active = false;

                    location.ModifiedDate = DateTime.Now;

                    await _context.SaveChangesAsync();



                    TempData["SuccessMessage"] = "Location deleted successfully!";

                }



                return RedirectToAction(nameof(Index));

            }

            catch (Exception ex)

            {

                Console.WriteLine($"Error deleting location: {ex.Message}");

                TempData["ErrorMessage"] = "Error deleting location. It may be in use.";

                return RedirectToAction(nameof(Index));

            }

        }



        private bool LocationExists(int id)

        {

            return _context.DeffLocations.Any(e => e.Id == id);

        }

    }

}