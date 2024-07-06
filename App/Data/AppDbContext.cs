using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Goal> Goals { get; set; }
        public DbSet<Progress> Progresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Goal>()
                .HasIndex(e => e.Id)
                .IsUnique();
                
            modelBuilder.Entity<Progress>()
                .HasIndex(p => new { p.Id, p.GoalId, p.Date })
                .IsUnique();
            
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Goal)
                .WithMany(u => u.Progresses)
                .HasForeignKey(p => p.GoalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
