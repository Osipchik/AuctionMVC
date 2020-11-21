using Domain.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser> 
    {
        public DbSet<Lot> Lots { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>()
                .HasMany(b => b.Rates)
                .WithOne()
                .HasForeignKey(i => i.AppUserId);
            
            modelBuilder.Entity<AppUser>()
                .HasMany(b => b.Lots)
                .WithOne(u => u.AppUser)
                .HasForeignKey(k => k.AppUserId);

            modelBuilder.Entity<Lot>()
                .HasMany(r => r.Rates)
                .WithOne()
                .HasForeignKey(i => i.LotId);

            modelBuilder.Entity<Lot>()
                .HasOne(i => i.Category)
                .WithMany(i => i.Lots);

            modelBuilder.Entity<Lot>()
                .HasMany(i => i.Comments)
                .WithOne();
        }
    }
}