using System.ComponentModel.DataAnnotations;

namespace CidadeAtivaApi.DTOs
{
    public class RegistrarUsuario
    {
        [Required(ErrorMessage = " O nome é obrigatório ")]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = " O e-mail é obrigatório ")]
        [EmailAddress(ErrorMessage = " E-mail inválido ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = " A senha é obrigatória ")]
        [MinLength(6, ErrorMessage = " Mínimo de 6 caracteres ")]
        public string? Password { get; set; }
    }
}
