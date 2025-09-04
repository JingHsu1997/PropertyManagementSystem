using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.Core.Models;

namespace PropertyManagementSystem.Data
{
    public class PropertyDbContext : DbContext
    {
        public PropertyDbContext(DbContextOptions<PropertyDbContext> options) : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Property Entity Configuration
            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.District)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.AreaInSquareMeters)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.PropertyType)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500);

                // Index for better search performance
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.District);
                entity.HasIndex(e => e.PropertyType);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Price);
            });

            // PropertyImage Entity Configuration
            modelBuilder.Entity<PropertyImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.AltText)
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Foreign Key Relationship
                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Images)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index for better performance
                entity.HasIndex(e => e.PropertyId);
            });
        }
    }
}
