
using System.ComponentModel.DataAnnotations;

namespace BudgetBook.Models
{
    // gibt an ob es eine einnahme oder eine ausgabe ist
    // wird bei category und bei transaction verwendet
    public enum TransactionType
    {
        [Display(Name = "Einnahme")]
        Income,

        [Display(Name = "Ausgabe")]
        Expense
    }
}
