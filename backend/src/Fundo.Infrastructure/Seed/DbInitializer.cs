using Fundo.Domain.Entities;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Seed;

public static class DbInitializer
{
    public static void Initialize(LoanDbContext context)
    {
        // Apply any pending migrations
        context.Database.Migrate();

        SeedUsers(context);
        SeedLoans(context);
    }

    private static void SeedUsers(LoanDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var users = new User[]
        {
            new User
            {
                Username = "admin",
                Email = "admin@fundo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "demo",
                Email = "demo@fundo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@123"),
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }

    private static void SeedLoans(LoanDbContext context)
    {
        if (context.Loans.Any())
        {
            return;
        }

        var loans = new Loan[]
        {
            new Loan
            {
                Amount = 10000.00m,
                CurrentBalance = 8500.00m,
                ApplicantName = "John Doe",
                Status = LoanStatus.Active
            },
            new Loan
            {
                Amount = 25000.00m,
                CurrentBalance = 25000.00m,
                ApplicantName = "Jane Smith",
                Status = LoanStatus.Active
            },
            new Loan
            {
                Amount = 15000.00m,
                CurrentBalance = 0.00m,
                ApplicantName = "Bob Johnson",
                Status = LoanStatus.Paid
            },
            new Loan
            {
                Amount = 50000.00m,
                CurrentBalance = 42000.00m,
                ApplicantName = "Alice Williams",
                Status = LoanStatus.Active
            },
            new Loan
            {
                Amount = 7500.00m,
                CurrentBalance = 3200.00m,
                ApplicantName = "Charlie Brown",
                Status = LoanStatus.Active
            }
        };

        context.Loans.AddRange(loans);
        context.SaveChanges();
    }
}
