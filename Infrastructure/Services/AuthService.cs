using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Email kontrolü
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return null; // Email zaten kayıtlı
            }

            // Rol kontrolü
            var role = await _context.Roles.FindAsync(registerDto.RoleId);
            if (role == null)
            {
                return null; // Geçersiz rol
            }

            // Şifreyi hash'le
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Yeni kullanıcı oluştur
            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                RoleId = registerDto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Kullanıcıyı role ile birlikte yükle
            var createdUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            // JWT token oluştur
            var token = GenerateJwtToken(createdUser!);

            return new AuthResponseDto
            {
                UserId = createdUser!.Id,
                FullName = createdUser.FullName,
                Email = createdUser.Email,
                RoleName = createdUser.Role!.Name,
                Token = token
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Kullanıcıyı bul
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return null; // Kullanıcı bulunamadı
            }

            // Şifreyi kontrol et
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null; // Yanlış şifre
            }

            // JWT token oluştur
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role!.Name,
                Token = token
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role!.Name
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role!.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

