using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Rol zorunludur")]
        public int RoleId { get; set; }
    }
}

