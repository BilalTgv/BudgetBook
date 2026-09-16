using BudgetBook.Data;
using BudgetBook.Models;
using BudgetBook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Controllers
{
    // ohne login kommt hier niemand rein, das regelt [Authorize]
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TransactionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transactions
        // zeigt nur die buchungen vom eingeloggten user, sortiert nach
        // datum absteigend, wie es im projektauftrag verlangt wird
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.BookingDate)
                .ToListAsync();

            return View(transactions);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            // wichtig: userid ist teil vom filter, sonst könnte man
            // fremde buchungen einfach über die url aufrufen
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction is null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public async Task<IActionResult> Create()
        {
            var vm = new TransactionFormViewModel
            {
                BookingDate = DateTime.Today,
                CategoryOptions = await GetCategoryOptionsAsync()
            };
            return View(vm);
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionFormViewModel vm)
        {
            await ValidateCategoryMatchesTypeAsync(vm);

            if (!ModelState.IsValid)
            {
                vm.CategoryOptions = await GetCategoryOptionsAsync();
                return View(vm);
            }

            var userId = _userManager.GetUserId(User);

            var transaction = new Transaction
            {
                Amount = vm.Amount,
                BookingDate = vm.BookingDate,
                Type = vm.Type,
                Description = vm.Description,
                CategoryId = vm.CategoryId,
                UserId = userId!,
                CreatedAt = DateTime.UtcNow
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction is null)
            {
                return NotFound();
            }

            var vm = new TransactionFormViewModel
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                BookingDate = transaction.BookingDate,
                Type = transaction.Type,
                Description = transaction.Description,
                CategoryId = transaction.CategoryId,
                CategoryOptions = await GetCategoryOptionsAsync()
            };

            return View(vm);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TransactionFormViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            // zuerst prüfen ob die buchung wirklich dem eingeloggten user
            // gehört, bevor überhaupt irgendwas gespeichert wird
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction is null)
            {
                return NotFound();
            }

            await ValidateCategoryMatchesTypeAsync(vm);

            if (!ModelState.IsValid)
            {
                vm.CategoryOptions = await GetCategoryOptionsAsync();
                return View(vm);
            }

            transaction.Amount = vm.Amount;
            transaction.BookingDate = vm.BookingDate;
            transaction.Type = vm.Type;
            transaction.Description = vm.Description;
            transaction.CategoryId = vm.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/Delete/5
        // zeigt erst eine bestätigungsseite, löscht noch nichts
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction is null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        // erst hier wird wirklich gelöscht, nach der bestätigung
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction is null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // baut die liste für das kategorie dropdown, mit dem typ dahinter
        // damit man im formular sieht ob es eine einnahme oder ausgabe kategorie ist
        private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name + (c.Type == TransactionType.Income ? " (einnahme)" : " (ausgabe)")
                })
                .ToList();
        }

        // geschäftsregel aus dem projektauftrag: eine ausgabe darf nur eine
        // ausgabenkategorie haben, eine einnahme nur eine einnahmenkategorie
        private async Task ValidateCategoryMatchesTypeAsync(TransactionFormViewModel vm)
        {
            var category = await _context.Categories.FindAsync(vm.CategoryId);

            if (category is null)
            {
                ModelState.AddModelError(nameof(vm.CategoryId), "diese kategorie gibt es nicht");
                return;
            }

            if (category.Type != vm.Type)
            {
                ModelState.AddModelError(nameof(vm.CategoryId), "die kategorie passt nicht zum gewählten typ");
            }
        }
    }
}
