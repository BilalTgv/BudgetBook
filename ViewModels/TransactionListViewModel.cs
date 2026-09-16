using BudgetBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetBook.ViewModels
{
    // hält die gefilterte liste plus die aktuell gewählten filterwerte
    // damit das formular nach dem absenden wieder die gleiche auswahl zeigt
    public class TransactionListViewModel
    {
        public List<Transaction> Transactions { get; set; } = new();

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public TransactionType? Type { get; set; }
        public int? CategoryId { get; set; }

        public List<SelectListItem> CategoryOptions { get; set; } = new();
    }
}
