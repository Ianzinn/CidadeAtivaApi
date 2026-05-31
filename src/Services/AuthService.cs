using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CidadeAtivaApi.Data;
using CidadeAtivaApi.DTOs;
using CidadeAtivaApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CidadeAtivaApi.Services
{
    public class AuthService
    {
        private readonly AppDB _db;
        private readonly IConfiguration _config;

        public AuthService(AppDB db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // --- REGISTRAR ---
        public async Task<RespostaLoginDTO?> RegistrarAsync(RegistrarUsuario dto)
        {
            var emailJaExiste = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailJaExiste) return null;

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new RespostaLoginDTO
            {
                Token = GerarToken(user),
                Usuario = ToDto(user)
            };
        }

        // --- LOGIN ---
        public async Task<RespostaLoginDTO?> LoginAsync(LoginUsuario dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null) return null;

            var senhaValida = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!senhaValida) return null;

            return new RespostaLoginDTO
            {
                Token = GerarToken(user),
                Usuario = ToDto(user)
            };
        }

        // Gera o token JWT com os dados do usuário
        private string GerarToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpirationHours"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static RespostaUsuarioDTO ToDto(User u) => new()
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role.ToString(),
            CreatedAt = u.CreatedAt
        };
    }
}
