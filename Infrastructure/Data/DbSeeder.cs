using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(AppDbContext context)
        {
            // Rolleri kontrol et ve ekle
            if (!await context.Roles.AnyAsync())
            {
                // Identity insert hatası yaşamamak için Id alanını EF'e bırakalım
                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "Sistem Kullanıcısı" },
                    new Role { Name = "Müşteri" },
                    new Role { Name = "Tedarikçi" }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            // Admin kullanıcısını kontrol et ve ekle
            if (!await context.Users.AnyAsync(u => u.Email == "admin@constructioncost.com"))
            {
                // Rolleri isimden Id'ye eşle
                var roleMap = await context.Roles.ToDictionaryAsync(r => r.Name, r => r.Id);
                var adminRoleId = roleMap["Admin"];

                var adminUser = new User
                {
                    FullName = "Admin User",
                    Email = "admin@constructioncost.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Varsayılan şifre
                    RoleId = adminRoleId // Admin rolü
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }

            // Sistem Kullanıcısı test hesabı
            if (!await context.Users.AnyAsync(u => u.Email == "system@constructioncost.com"))
            {
                var systemRoleId = await context.Roles
                    .Where(r => r.Name == "Sistem Kullanıcısı")
                    .Select(r => r.Id)
                    .FirstAsync();

                var systemUser = new User
                {
                    FullName = "Sistem Kullanıcısı Test",
                    Email = "system@constructioncost.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("System123!"),
                    RoleId = systemRoleId
                };

                await context.Users.AddAsync(systemUser);
                await context.SaveChangesAsync();
            }

            // Müşteri test hesabı
            if (!await context.Users.AnyAsync(u => u.Email == "customer@constructioncost.com"))
            {
                var customerRoleId = await context.Roles
                    .Where(r => r.Name == "Müşteri")
                    .Select(r => r.Id)
                    .FirstAsync();

                var customerUser = new User
                {
                    FullName = "Müşteri Test",
                    Email = "customer@constructioncost.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                    RoleId = customerRoleId
                };

                await context.Users.AddAsync(customerUser);
                await context.SaveChangesAsync();
            }

            // Tedarikçi test hesabı
            if (!await context.Users.AnyAsync(u => u.Email == "supplier@constructioncost.com"))
            {
                var supplierRoleId = await context.Roles
                    .Where(r => r.Name == "Tedarikçi")
                    .Select(r => r.Id)
                    .FirstAsync();

                var supplierUser = new User
                {
                    FullName = "Tedarikçi Test",
                    Email = "supplier@constructioncost.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Supplier123!"),
                    RoleId = supplierRoleId
                };

                await context.Users.AddAsync(supplierUser);
                await context.SaveChangesAsync();
            }
        }
    }
}

