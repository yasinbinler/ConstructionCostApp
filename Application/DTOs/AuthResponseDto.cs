namespace Application.DTOs
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}

