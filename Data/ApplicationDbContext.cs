using LaboGrocery.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LaboGrocery.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Ya existentes en tu proyecto (ajusta si tus nombres difieren)
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Language> Languages => Set<Language>();

        // NUEVOS para pedidos
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relación Order 1..N OrderLine
            builder.Entity<Order>()
                   .HasMany(o => o.Lines)
                   .WithOne(ol => ol.Order)
                   .HasForeignKey(ol => ol.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // (Opcional) tipos de columna para SQL Server; con SQLite se ignora
            builder.Entity<Order>().Property(o => o.Total).HasColumnType("decimal(18,2)");
            builder.Entity<OrderLine>().Property(ol => ol.Price).HasColumnType("decimal(18,2)");
        }
    }
}
