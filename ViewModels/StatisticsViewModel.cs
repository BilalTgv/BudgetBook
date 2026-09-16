using BudgetBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetBook.ViewModels
{
    public class StatisticsViewModel
    {
        // filterwerte, kommen als query parameter in der url
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public TransactionType? Type { get; set; }
        public int? CategoryId { get; set; }

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        // berechnete kennzahlen im gewählten zeitraum
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;

        public bool HasData => TotalIncome > 0 || TotalExpense > 0;

        public List<CategorySummary> ExpensesByCategory { get; set; } = new();
        public List<MonthSummary> MonthlyOverview { get; set; } = new();
    }

    // eine zeile in der tabelle "ausgaben nach kategorie"
    public class CategorySummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    // eine zeile in der monatsübersicht
    public class MonthSummary
    {
        public string MonthLabel { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }
}
