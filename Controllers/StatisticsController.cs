using System.Globalization;
using BudgetBook.Data;
using BudgetBook.Models;
using BudgetBook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StatisticsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Statistics
        // berechnet einnahmen, ausgaben und saldo für den eingeloggten user
        // im gewählten zeitraum, optional zusätzlich nach typ oder kategorie gefiltert
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, TransactionType? type, int? categoryId)
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (from.HasValue)
            {
                query = query.Where(t => t.BookingDate >= from.Value.Date);
            }

            if (to.HasValue)
            {
                var toExclusive = to.Value.Date.AddDays(1);
                query = query.Where(t => t.BookingDate < toExclusive);
            }

            if (type.HasValue)
            {
                query = query.Where(t => t.Type == type.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            // alles auf einmal laden, danach nur noch im speicher rechnen
            // ist bei so einem kleinen projekt völlig ausreichend
            var transactions = await query.ToListAsync();

            var vm = new StatisticsViewModel
            {
                From = from,
                To = to,
                Type = type,
                CategoryId = categoryId,
                CategoryOptions = await _context.GetCategoryOptionsWithEmptyAsync(),
                TotalIncome = transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount),
                TotalExpense = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount),
            };

            vm.ExpensesByCategory = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category?.Name ?? "ohne kategorie")
                .Select(g => new CategorySummary
                {
                    CategoryName = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(c => c.Amount)
                .ToList();

            var deAt = new CultureInfo("de-AT");
            vm.MonthlyOverview = transactions
                .GroupBy(t => new { t.BookingDate.Year, t.BookingDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new MonthSummary
                {
                    MonthLabel = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy", deAt),
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .ToList();

            return View(vm);
        }
    }
}
