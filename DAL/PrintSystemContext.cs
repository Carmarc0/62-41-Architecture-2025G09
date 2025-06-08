using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    /// <summary>
    /// Database context for Print System - supports LocalDB and Azure SQL
    /// </summary>
    public class PrintSystemContext : DbContext
    {
        public PrintSystemContext(DbContextOptions<PrintSystemContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserQuota> UserQuotas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Uid)
                .IsUnique();

            modelBuilder.Entity<UserQuota>()
                .HasIndex(uq => uq.Username)
                .IsUnique();

            // Seed data - only functional test accounts
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "SCoppey",
                    Uid = "12345678",
                    FirstName = "Stephane",
                    LastName = "Coppey"
                },
                new User
                {
                    Id = 2,
                    Username = "JDupont",
                    Uid = "87654321",
                    FirstName = "Jean",
                    LastName = "Dupont"
                }
            );

            // Initial quotas - only for functional accounts
            modelBuilder.Entity<UserQuota>().HasData(
                new UserQuota
                {
                    Id = 1,
                    Username = "SCoppey",
                    TotalPages = 25.0f, // 2 CHF
                    LastUpdated = DateTime.Now
                },
                new UserQuota
                {
                    Id = 2,
                    Username = "JDupont",
                    TotalPages = 12.5f, // 1 CHF
                    LastUpdated = DateTime.Now
                }
            );
        }
    }
}