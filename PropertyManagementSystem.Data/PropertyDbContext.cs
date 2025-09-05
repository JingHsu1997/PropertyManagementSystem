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

                entity.Property(e => e.Area)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Bedrooms)
                    .IsRequired()
                    .HasColumnType("tinyint");

                entity.Property(e => e.Bathrooms)
                    .IsRequired()
                    .HasColumnType("tinyint");

                entity.Property(e => e.TypeId)
                    .IsRequired();

                entity.Property(e => e.StatusId)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                // Index for better search performance
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.District);
                entity.HasIndex(e => e.TypeId);
                entity.HasIndex(e => e.StatusId);
                entity.HasIndex(e => e.Price);
                entity.HasIndex(e => e.IsDeleted);
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

                entity.Property(e => e.SortOrder)
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
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
