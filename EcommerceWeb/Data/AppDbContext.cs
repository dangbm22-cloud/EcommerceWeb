using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EcommerceWeb.Models;

namespace EcommerceWeb.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CPUDetail> CPUDetails { get; set; }
        public DbSet<VGADetail> VGADetails { get; set; }
        public DbSet<RAMDetail> RAMDetails { get; set; }
        public DbSet<MainboardDetail> MainboardDetails { get; set; }
        public DbSet<PSUDetail> PSUDetails { get; set; }
        public DbSet<PCBuildDetail> PCBuildDetails { get; set; }
    }
}
