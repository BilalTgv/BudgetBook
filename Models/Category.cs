using System.ComponentModel.DataAnnotations;

namespace BudgetBook.Models
{
    // eine kategorie ist entweder für einnahmen oder für ausgaben
    // die admin person legt die kategorien an, die user wählen dann nur aus
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // income oder expense, wird beim erstellen einer buchung gebraucht
        // damit man nur passende kategorien anzeigt
        public TransactionType Type { get; set; }

        // falls man eine kategorie nicht löschen will aber sie nicht mehr
        // in der auswahl zeigen will, einfach auf false setzen
        public bool IsActive { get; set; } = true;

        // eine kategorie kann viele buchungen haben
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
