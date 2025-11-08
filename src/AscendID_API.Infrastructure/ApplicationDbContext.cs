using Microsoft.EntityFrameworkCore;
using AscendID_API.Domain.Entities;

namespace AscendID_API.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TempTestRecord> TempTestRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PostgreSQL schema
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<TempTestRecord>(entity =>
        {
            entity.ToTable("temp_test_records"); // Use snake_case for PostgreSQL convention
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityByDefaultColumn(); // PostgreSQL identity column
            
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500);
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            
            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired()
                .HasDefaultValue(true);

            // Add indexes for better performance
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("ix_temp_test_records_is_active");
            
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("ix_temp_test_records_created_at");
        });
    }
}
