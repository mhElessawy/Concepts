using Concept.Data;
using Concept.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Concept.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.UserInfos
                .Include(u => u.JobTitle)
                .Include(u => u.Department)
                .OrderBy(u => u.UserCode)
                .ToListAsync();

            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.UserInfos
                .Include(u => u.JobTitle)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["JobTitles"] = new SelectList(_context.DeffJobTitles.Where(j => j.Active), "Id", "JobName");
            ViewData["Departments"] = new SelectList(_context.DeffDepartments.Where(d => d.Active), "Id", "DepartmentName");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserInfo userInfo)
        {
            if (ModelState.IsValid)
            {
                // Check if UserCode already exists
                if (await _context.UserInfos.AnyAsync(u => u.UserCode == userInfo.UserCode))
                {
                    ModelState.AddModelError("UserCode", "User Code already exists");
                    ViewData["JobTitles"] = new SelectList(_context.DeffJobTitles.Where(j => j.Active), "Id", "JobName", userInfo.JobTitleId);
                    ViewData["Departments"] = new SelectList(_context.DeffDepartments.Where(d => d.Active), "Id", "DepartmentName", userInfo.DepartmentId);
                    return View(userInfo);
                }

                // Check if UserName already exists
                if (await _context.UserInfos.AnyAsync(u => u.UserName == userInfo.UserName))
                {
                    ModelState.AddModelError("UserName", "Username already exists");
                    ViewData["JobTitles"] = new SelectList(_context.DeffJobTitles.Where(j => j.Active), "Id", "JobName", userInfo.JobTitleId);
                    ViewData["Departments"] = new SelectList(_context.DeffDepartments.Where(d => d.Active), "Id", "DepartmentName", userInfo.DepartmentId);
                    return View(userInfo);
                }

                userInfo.CreatedDate = DateTime.Now;
                userInfo.ModifiedDate = DateTime.Now;

                _context.Add(userInfo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["JobTitles"] = new SelectList(_context.DeffJobTitles.Where(j => j.Active), "Id", "JobName", userInfo.JobTitleId);
            ViewData["Departments"] = new SelectList(_context.DeffDepartments.Where(d => d.Active), "Id", "DepartmentName", userInfo.DepartmentId);
            return View(userInfo);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.UserInfos.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["JobTitles"] = new SelectList(_context.DeffJobTitles.Where(j => j.Active), "Id", "JobName", user.JobTitleId);
            ViewData["Departments"] = new SelectList(_context.DeffDepartments.Where(d => d.Active), "Id", "DepartmentName", user.DepartmentId);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserCode,FullName,UserName,UserPassword,JobTitleId,DepartmentId,PurchaseOrderAuthorise,CanApprovePurchaseOrders,Active,CreatedDate")] UserInfo userInfo)
        {
            if (id != userInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if UserCode already exists (excluding current user)
                    if (await _context.UserInfos.AnyAsync(u => u.UserCode == userInfo.UserCode && u.Id != userInfo.Id))
                    {
                        ModelState.AddModelError("UserCode", "User Code already exists");
                        ViewData["JobTitles"] = new SelectList(_context.DeffJobTitles.Where(j => j.Active), "Id", "JobName", userInfo.JobTitleId);
                        ViewData["Departments"] = new SelectList(_context.DeffDepartments.Where(d => d.Active), "Id", "DepartmentName", userInfo.DepartmentId);
                        return View(userInfo);
                    }

                    // Check if UserName already exists (excluding current user)
                    if (await _context.UserInfos.AnyAsync(u => u.UserName == userInfo.UserName && u.Id != userInfo.Id))
                    {
                        ModelState.AddModelError("UserName", "Username already exists");
                        ViewData["JobTitles"] = new SelectList(_context.DeffJobTitles.Where(j => j.Active), "Id", "JobName", userInfo.JobTitleId);
                        ViewData["Departments"] = new SelectList(_context.DeffDepartments.Where(d => d.Active), "Id", "DepartmentName", userInfo.DepartmentId);
                        return View(userInfo);
                    }

                    userInfo.ModifiedDate = DateTime.Now;
                    _context.Update(userInfo);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "User updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userInfo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["JobTitles"] = new SelectList(_context.DeffJobTitles.Where(j => j.Active), "Id", "JobName", userInfo.JobTitleId);
            ViewData["Departments"] = new SelectList(_context.DeffDepartments.Where(d => d.Active), "Id", "DepartmentName", userInfo.DepartmentId);
            return View(userInfo);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.UserInfos
                .Include(u => u.JobTitle)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.UserInfos.FindAsync(id);
            if (user != null)
            {
                _context.UserInfos.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.UserInfos.Any(e => e.Id == id);
        }

        // GET: Users/ChangePassword/5
        public async Task<IActionResult> ChangePassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.UserInfos.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel
            {
                UserId = user.Id,
                UserCode = user.UserCode,
                FullName = user.FullName
            };

            return View(model);
        }

        // POST: Users/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.UserInfos.FindAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserPassword = model.NewPassword;
                user.ModifiedDate = DateTime.Now;

                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }

    // ViewModel for Change Password
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}