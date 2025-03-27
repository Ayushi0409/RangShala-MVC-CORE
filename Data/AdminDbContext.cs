using Microsoft.EntityFrameworkCore;
using RangShala.Models;

namespace RangShala.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}