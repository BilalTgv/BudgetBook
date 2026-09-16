using BudgetBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Data
{
    // diese klasse füllt die datenbank beim start mit ein paar kategorien
    // und ein paar testbuchungen, damit man gleich was zum anschauen hat
    // wird einmal aus der Program.cs aufgerufen
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            await context.Database.MigrateAsync();

            // kategorien nur anlegen wenn noch keine da sind
            if (!await context.Categories.AnyAsync())
            {
                var neueKategorien = new List<Category>
                {
                    new Category { Name = "Gehalt", Type = TransactionType.Income, IsActive = true },
                    new Category { Name = "Sonstige einnahmen", Type = TransactionType.Income, IsActive = true },
                    new Category { Name = "Lebensmittel", Type = TransactionType.Expense, IsActive = true },
                    new Category { Name = "Miete", Type = TransactionType.Expense, IsActive = true },
                    new Category { Name = "Freizeit", Type = TransactionType.Expense, IsActive = true },
                };

                context.Categories.AddRange(neueKategorien);
                await context.SaveChangesAsync();
            }

            // testbuchungen nur anlegen wenn noch keine da sind
            if (await context.Transactions.AnyAsync())
            {
                return;
            }

            // testbuchungen brauchen einen echten user, weil userid ein
            // foreign key ist, darum schauen wir ob schon jemand registriert ist
            // wenn noch niemand da ist, überspringen wir das einfach
            // (einfach die app nochmal starten, nachdem du dich registriert hast,
            // dann werden die testbuchungen automatisch nachgeholt)
            var firstUser = await context.Users.FirstOrDefaultAsync();
            if (firstUser is null)
            {
                return;
            }
            var testUserId = firstUser.Id;

            var gehalt = await context.Categories.FirstAsync(c => c.Name == "Gehalt");
            var lebensmittel = await context.Categories.FirstAsync(c => c.Name == "Lebensmittel");
            var miete = await context.Categories.FirstAsync(c => c.Name == "Miete");

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Amount = 1800.00m,
                    BookingDate = DateTime.Today.AddDays(-20),
                    Type = TransactionType.Income,
                    Description = "monatsgehalt",
                    CategoryId = gehalt.Id,
                    UserId = testUserId
                },
                new Transaction
                {
                    Amount = 650.00m,
                    BookingDate = DateTime.Today.AddDays(-15),
                    Type = TransactionType.Expense,
                    Description = "miete september",
                    CategoryId = miete.Id,
                    UserId = testUserId
                },
                new Transaction
                {
                    Amount = 45.30m,
                    BookingDate = DateTime.Today.AddDays(-3),
                    Type = TransactionType.Expense,
                    Description = "einkauf supermarkt",
                    CategoryId = lebensmittel.Id,
                    UserId = testUserId
                },
            };

            context.Transactions.AddRange(transactions);
            await context.SaveChangesAsync();
        }
    }
}
