using System.ComponentModel.DataAnnotations;
using BudgetBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetBook.ViewModels
{
    // dieses viewmodel wird für das formular zum anlegen und bearbeiten
    // einer buchung verwendet, es ist kein direktes db model weil wir
    // hier noch die liste der kategorien für das dropdown dazu brauchen
    public class TransactionFormViewModel
    {
        public int Id { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "der betrag muss größer als 0 sein")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "bitte ein datum angeben")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "bitte einnahme oder ausgabe auswählen")]
        [Display(Name = "Typ")]
        public TransactionType Type { get; set; }

        [StringLength(200, ErrorMessage = "die beschreibung darf höchstens 200 zeichen haben")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "bitte eine kategorie auswählen")]
        public int CategoryId { get; set; }

        // wird im controller befüllt, ist keine echte eingabe vom user
        public List<SelectListItem> CategoryOptions { get; set; } = new();
    }
}
