using Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalRecord>()
            .HasOne(m => m.User)
            .WithMany(u => u.MedicalRecords)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}