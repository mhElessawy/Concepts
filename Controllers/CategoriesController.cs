using Concept.Data;
using Concept.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Concept.Controllers;

public class CategoriesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? name, string? code, bool? active, string? sort)
    {
        name = name?.Trim();
        code = code?.Trim();
        var query = context.DeffCategories.AsNoTracking();

        if (!string.IsNullOrEmpty(name))
            query = query.Where(c => c.Name.Contains(name));
        if (!string.IsNullOrEmpty(code))
            query = query.Where(c => c.Code.Contains(code));
        if (active.HasValue)
            query = query.Where(c => c.Active == active.Value);

        var ordered = sort switch
        {
            "name-desc" => query.OrderByDescending(c => c.Name),
            "code" => query.OrderBy(c => c.Code),
            "code-desc" => query.OrderByDescending(c => c.Code),
            "active" => query.OrderByDescending(c => c.Active).ThenBy(c => c.Name),
            "inactive" => query.OrderBy(c => c.Active).ThenBy(c => c.Name),
            _ => query.OrderBy(c => c.Name)
        };

        ViewData["Name"] = name;
        ViewData["Code"] = code;
        ViewData["Active"] = active;
        ViewData["Sort"] = sort;
        return View(await ordered.ThenBy(c => c.Id).ToListAsync());
    }

    public IActionResult Create() => View(new CategoryForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryForm form)
    {
        await ValidateForm(form);
        if (!ModelState.IsValid)
        {
            ViewData["Account"] = await AccountLabel(form.AccountId);
            return View(form);
        }

        string code;
        do
        {
            var numbers = await context.Database
                .SqlQueryRaw<long>("SELECT NEXT VALUE FOR [CategoryCodeSequence] AS [Value]")
                .ToListAsync();
            code = FormattableString.Invariant($"CAT-{numbers.Single():D6}");
        } while (await context.DeffCategories.AnyAsync(c => c.Code == code));

        context.DeffCategories.Add(new DeffCategory
        {
            Code = code,
            Name = form.Name,
            AccountId = form.AccountId,
            Active = form.Active,
            Description = string.Empty
        });

        if (await SaveCategory()) return RedirectToAction(nameof(Index));

        ViewData["Account"] = await AccountLabel(form.AccountId);
        return View(form);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await context.DeffCategories.FindAsync(id);
        if (category == null) return NotFound();

        ViewData["Account"] = await AccountLabel(category.AccountId);
        return View(category);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await context.DeffCategories.FindAsync(id);
        if (category == null) return NotFound();

        await SetEditViewData(category, category.AccountId);
        return View(new CategoryForm { Name = category.Name, AccountId = category.AccountId, Active = category.Active });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryForm form)
    {
        var category = await context.DeffCategories.FindAsync(id);
        if (category == null) return NotFound();

        await ValidateForm(form, category);
        if (ModelState.IsValid)
        {
            category.Name = form.Name;
            category.AccountId = form.AccountId;
            category.Active = form.Active;
            category.ModifiedDate = DateTime.Now;
            if (await SaveCategory()) return RedirectToAction(nameof(Index));
        }

        await SetEditViewData(category, form.AccountId);
        return View(form);
    }

    [HttpGet]
    public async Task<IActionResult> AccountTree()
    {
        var branches = await context.MainAccounts.OrderBy(a => a.AccountNo)
            .Select(a => new { a.Id, a.ParentAccountId, a.AccountNo, a.AccountName }).ToListAsync();
        var accounts = await context.ChildAccounts.Where(a => a.Active).OrderBy(a => a.AccountNo)
            .Select(a => new { a.Id, a.ParentAccountId, a.AccountNo, a.AccountName }).ToListAsync();
        return Json(new { branches, accounts });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await context.DeffCategories.FindAsync(id);
        return category == null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await context.DeffCategories.FindAsync(id);
        if (category != null && category.Active)
        {
            category.Active = false;
            category.ModifiedDate = DateTime.Now;
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateForm(CategoryForm form, DeffCategory? existing = null)
    {
        if (!ModelState.IsValid) return;

        form.Name = form.Name.Trim();
        int id = existing?.Id ?? 0;
        if (await context.DeffCategories.AnyAsync(c => c.Id != id &&
                EF.Property<string>(c, "NormalizedName") == form.Name))
            ModelState.AddModelError(nameof(form.Name), "This category name is already in use.");

        if (form.AccountId.HasValue && form.AccountId != existing?.AccountId &&
            !await context.ChildAccounts.AnyAsync(a => a.Id == form.AccountId && a.Active))
            ModelState.AddModelError(nameof(form.AccountId), "Select an active account from the account tree.");
    }

    private async Task<bool> SaveCategory()
    {
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 } sql &&
            sql.Message.Contains("IX_Deff_Category_NormalizedName", StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(CategoryForm.Name), "This category name is already in use.");
            return false;
        }
    }

    private async Task SetEditViewData(DeffCategory category, int? accountId)
    {
        ViewData["CategoryId"] = category.Id;
        ViewData["CategoryCode"] = category.Code;
        ViewData["Account"] = await AccountLabel(accountId);
    }

    private async Task<string?> AccountLabel(int? accountId)
    {
        if (!accountId.HasValue) return null;

        return await context.ChildAccounts.Where(a => a.Id == accountId)
            .Select(a => a.AccountNo + " — " + a.AccountName).FirstOrDefaultAsync() ?? accountId.ToString();
    }
}
