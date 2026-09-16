using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetBook.Models
{
    // eine buchung ist entweder eine einnahme oder eine ausgabe
    // gehört immer genau zu einem user und einer kategorie
    public class Transaction
    {
        public int Id { get; set; }

        // betrag muss größer als 0 sein, steht so im projektauftrag
        // nie double oder float für geld nehmen, immer decimal
        [Range(0.01, 999999.99)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        public TransactionType Type { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        // fremdschlüssel zur category
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // fremdschlüssel zum user, wird im controller aus dem
        // eingeloggten benutzer gesetzt, nicht aus dem formular
        public string UserId { get; set; } = string.Empty;

        // wird automatisch beim anlegen gesetzt, für sortierung und logs praktisch
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
