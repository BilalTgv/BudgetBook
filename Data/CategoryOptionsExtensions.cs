using BudgetBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Data
{
    // extension method damit man von überall wo man einen dbcontext hat
    // einfach _context.GetCategoryOptionsAsync() aufrufen kann
    // spart uns die gleiche logik in mehreren controllern
    public static class CategoryOptionsExtensions
    {
        public static async Task<List<SelectListItem>> GetCategoryOptionsAsync(this ApplicationDbContext context)
        {
            var categories = await context.Categories
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

        // gleiche liste, aber mit einer leeren option ganz oben
        // fürs filter dropdown, wo "alle kategorien" die vorauswahl ist
        public static async Task<List<SelectListItem>> GetCategoryOptionsWithEmptyAsync(this ApplicationDbContext context)
        {
            var options = await context.GetCategoryOptionsAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "alle kategorien" });
            return options;
        }
    }
}
