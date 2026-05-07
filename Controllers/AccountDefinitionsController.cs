using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class AccountDefinitionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountDefinitionsController(ApplicationDbContext context) => _context = context;

        // ===================== ACCOUNT TYPES =====================

        public async Task<IActionResult> AccountTypes()
            => View(await _context.DefAccountTypes.OrderBy(x => x.Code).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetAccountTypes()
            => Json(await _context.DefAccountTypes.Where(x => x.Active)
                .OrderBy(x => x.Code)
                .Select(x => new { x.Id, x.Code, x.Name })
                .ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAccountType(int id, string code, string name, bool active)
            => await SaveDef(_context.DefAccountTypes, id, code, name, active,
                () => new DefAccountType { Code = code, Name = name, Active = active, CreatedDate = DateTime.Now });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountType(int id)
            => await DeleteDef(_context.DefAccountTypes, id);

        // ===================== ACCOUNT EFFECTS =====================

        public async Task<IActionResult> AccountEffects()
            => View(await _context.DefAccountEffects.OrderBy(x => x.Code).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetAccountEffects()
            => Json(await _context.DefAccountEffects.Where(x => x.Active)
                .OrderBy(x => x.Code)
                .Select(x => new { x.Id, x.Code, x.Name })
                .ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAccountEffect(int id, string code, string name, bool active)
            => await SaveDef(_context.DefAccountEffects, id, code, name, active,
                () => new DefAccountEffect { Code = code, Name = name, Active = active, CreatedDate = DateTime.Now });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountEffect(int id)
            => await DeleteDef(_context.DefAccountEffects, id);

        // ===================== NATURE OF ACCOUNT =====================

        public async Task<IActionResult> NatureOfAccounts()
            => View(await _context.DefNatureOfAccounts.OrderBy(x => x.Code).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetNatureOfAccounts()
            => Json(await _context.DefNatureOfAccounts.Where(x => x.Active)
                .OrderBy(x => x.Code)
                .Select(x => new { x.Id, x.Code, x.Name })
                .ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNatureOfAccount(int id, string code, string name, bool active)
            => await SaveDef(_context.DefNatureOfAccounts, id, code, name, active,
                () => new DefNatureOfAccount { Code = code, Name = name, Active = active, CreatedDate = DateTime.Now });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNatureOfAccount(int id)
            => await DeleteDef(_context.DefNatureOfAccounts, id);

        // ===================== SHARED HELPERS =====================

        private async Task<IActionResult> SaveDef<T>(
            DbSet<T> dbSet, int id, string code, string name, bool active,
            Func<T> factory) where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code)) return Json(new { success = false, message = "Code is required." });
                if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Name is required." });

                if (id == 0)
                {
                    var entity = factory();
                    dbSet.Add(entity);
                    await _context.SaveChangesAsync();
                    var newId = (int)typeof(T).GetProperty("Id")!.GetValue(entity)!;
                    return Json(new { success = true, message = "Saved successfully.", id = newId });
                }
                else
                {
                    var entity = await dbSet.FindAsync(id);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                    typeof(T).GetProperty("Code")!.SetValue(entity, code);
                    typeof(T).GetProperty("Name")!.SetValue(entity, name);
                    typeof(T).GetProperty("Active")!.SetValue(entity, active);
                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Updated successfully.", id });
                }
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                if (msg.Contains("UNIQUE") || msg.Contains("duplicate"))
                    return Json(new { success = false, message = $"Code '{code}' already exists." });
                return Json(new { success = false, message = msg });
            }
        }

        private async Task<IActionResult> DeleteDef<T>(DbSet<T> dbSet, int id) where T : class
        {
            try
            {
                var entity = await dbSet.FindAsync(id);
                if (entity == null) return Json(new { success = false, message = "Not found." });
                dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cannot delete: " + (ex.InnerException?.Message ?? ex.Message) });
            }
        }
    }
}