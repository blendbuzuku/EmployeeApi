using Microsoft.EntityFrameworkCore;
using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeAttachment> EmployeeAttachments { get; set; }
    public DbSet<EmployeeProfilePicture> EmployeeProfilePictures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Department).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");

            // Relationship with ProfilePicture
            entity.HasOne(e => e.ProfilePicture)
                  .WithOne(p => p.Employee)
                  .HasForeignKey<EmployeeProfilePicture>(p => p.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // EmployeeAttachment configuration
        modelBuilder.Entity<EmployeeAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FileType).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Employee)
                  .WithMany(e => e.Attachments)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // EmployeeProfilePicture configuration
        modelBuilder.Entity<EmployeeProfilePicture>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FileName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FileType).HasMaxLength(50).IsRequired();

            entity.HasOne(p => p.Employee)
                  .WithOne(e => e.ProfilePicture)
                  .HasForeignKey<EmployeeProfilePicture>(p => p.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}