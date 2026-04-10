using InvoiceManagement.Domain.Entitites;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagement.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Number)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.CustomerName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.TotalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.HasMany(x => x.Items)
                    .WithOne(x => x.Invoice)
                    .HasForeignKey(x => x.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.UnitPrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.TotalItemAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.Justification)
                    .HasMaxLength(500);
            });
        }
    }
}
