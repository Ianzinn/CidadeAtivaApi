using CidadeAtivaApi.Models.Enum;

namespace CidadeAtivaApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProblamasUrbano> Problemas { get; set; } = [];
    }
}
