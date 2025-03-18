using Microsoft.EntityFrameworkCore;
using RangShala.Models;

namespace RangShala.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<BillingInfo> BillingInfos { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<JoinApplication> JoinApplications { get; set; }
        public DbSet<OtpRecord> OtpRecords { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Artwork>()
                .Property(a => a.PaintingPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);
        }
    }
}