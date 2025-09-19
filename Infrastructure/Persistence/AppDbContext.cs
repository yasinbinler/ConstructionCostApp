using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Material> Materials => Set<Material>();
        public DbSet<ProjectMaterial> ProjectMaterials => Set<ProjectMaterial>();
        public DbSet<Offer> Offers => Set<Offer>();
    }
}
