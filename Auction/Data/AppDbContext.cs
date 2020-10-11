using Auction.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auction.Data
{
    public class AppDbContext : IdentityDbContext<AppUser> 
    {
        public DbSet<Lot> Lots { get; set; }
        public DbSet<Rate> Rates { get; set; }
        // public DbSet<AppUser> AppUsers { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Database.EnsureDeleted();
            Database.EnsureCreated();
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
        }
    }
}