using BudgetBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Data
{
    // das ist die datei die dotnet new mvc -au Individual automatisch erzeugt
    // wir müssen hier nur die zwei dbsets und die beziehungen dazugeben
    // achtung: die using zeilen und der konstruktor unten müssen genau so bleiben
    // wie es der scaffolder generiert hat, hier nur als beispiel gezeigt
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // die zwei neuen tabellen für unser projekt
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // eine transaction gehört zu genau einer category
            // wenn man eine category löscht wollen wir nicht dass die
            // buchungen automatisch mitgelöscht werden, darum restrict
            builder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // eine transaction gehört zu genau einem user
            // hier kein navigation property beim user nötig, reicht die id
            builder.Entity<Transaction>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
