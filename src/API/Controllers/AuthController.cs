using CidadeAtivaApi.DTOs;
using CidadeAtivaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CidadeAtivaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;
        public AuthController(AuthService service) => _service = service;


        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuario dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _service.RegistrarAsync(dto);
            if (resultado is null)
                return Conflict(new { mensagem = "E-mail já cadastrado" });

            return CreatedAtAction(nameof(Registrar), resultado);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuario dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _service.LoginAsync(dto);
            if (resultado is null)
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos" });

            return Ok(resultado);
        }


        // Logout é stateless em JWT — o cliente descarta o token
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout() => Ok(new { mensagem = "Logout realizado com sucesso" });
    }
}
