using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public class Project
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }

    public class Product
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public string Name { get; set; } = null!;
        public double Quantity { get; set; }
        public ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();
    }

    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal MarketUnitPrice { get; set; }
        public decimal CompanyUnitPrice { get; set; }
        public ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();
    }

    public class ProjectMaterial
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int MaterialId { get; set; }
        public Material? Material { get; set; }
        public double QuantityNeeded { get; set; }
    }

    public class Offer
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public decimal TotalMarketCost { get; set; }
        public decimal TotalCompanyOffer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
