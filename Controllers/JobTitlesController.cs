using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class JobTitlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobTitlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JobTitles
        public async Task<IActionResult> Index()
        {
            var jobTitles = await _context.DeffJobTitles
                
                .Where(jt => jt.Active)
                .OrderBy(jt => jt.JobName)
                .ToListAsync();
            return View(jobTitles);
        }

        // GET: JobTitles/Create
        public IActionResult Create()
        {
            ViewBag.Departments = _context.DeffDepartments.Where(d => d.Active).ToList();
            return View();
        }

        // POST: JobTitles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeffJobTitle jobTitle)
        {
            if (ModelState.IsValid)
            {
                jobTitle.CreatedDate = DateTime.Now;
                jobTitle.ModifiedDate = DateTime.Now;
                _context.Add(jobTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = _context.DeffDepartments.Where(d => d.Active).ToList();
            return View(jobTitle);
        }

        // GET: JobTitles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var jobTitle = await _context.DeffJobTitles.FindAsync(id);
            if (jobTitle == null) return NotFound();

            ViewBag.Departments = _context.DeffDepartments.Where(d => d.Active).ToList();
            return View(jobTitle);
        }

        // POST: JobTitles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeffJobTitle jobTitle)
        {
            if (id != jobTitle.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    jobTitle.ModifiedDate = DateTime.Now;
                    _context.Update(jobTitle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobTitleExists(jobTitle.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = _context.DeffDepartments.Where(d => d.Active).ToList();
            return View(jobTitle);
        }

        // GET: JobTitles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var jobTitle = await _context.DeffJobTitles
                
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobTitle == null) return NotFound();

            return View(jobTitle);
        }

        // POST: JobTitles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobTitle = await _context.DeffJobTitles.FindAsync(id);
            if (jobTitle != null)
            {
                jobTitle.Active = false;
                jobTitle.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool JobTitleExists(int id)
        {
            return _context.DeffJobTitles.Any(e => e.Id == id);
        }
    }
}