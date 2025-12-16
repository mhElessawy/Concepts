using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;

namespace Concept.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new DashboardViewModel
            {
                TotalItems = await _context.StoreItems.CountAsync(),
                TotalCategories = await _context.DeffCategories.CountAsync(),
                TotalValue = await _context.StoreItems.SumAsync(i => i.PurchaseValue * i.QuantityInStore),
                LowStockItems = await _context.StoreItems
                    .Where(i => i.QuantityInStore < 10)
                    .CountAsync(),
                RecentItems = await _context.StoreItems
                    .Include(i => i.SubCategory)
                    .ThenInclude(sc => sc.Category)
                    .OrderByDescending(i => i.CreatedDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }

    // Dashboard ViewModel
    public class DashboardViewModel
    {
        public int TotalItems { get; set; }
        public int TotalCategories { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockItems { get; set; }
        public List<StoreItem> RecentItems { get; set; }
    }
}
