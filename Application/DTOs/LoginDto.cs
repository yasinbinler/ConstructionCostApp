using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; } = null!;
    }
}

