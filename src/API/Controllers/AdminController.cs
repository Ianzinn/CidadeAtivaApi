using CidadeAtivaApi.DTOs;
using CidadeAtivaApi.Models.Enum;
using CidadeAtivaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CidadeAtivaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ProblemasService _service;
        public AdminController(ProblemasService service) => _service = service;


        // Lista todos os chamados — com filtro opcional por status
        [HttpGet("chamados")]
        public async Task<IActionResult> GetChamados([FromQuery] StatusProblema? status)
        {
            var lista = await _service.GetAllAsync(null, status, isAdmin: true);
            return Ok(lista);
        }


        // Visualiza um chamado específico
        [HttpGet("chamados/{id:guid}")]
        public async Task<IActionResult> GetChamado(Guid id)
        {
            var chamado = await _service.GetChamadoAsync(id);
            if (chamado is null)
                return NotFound(new { mensagem = $"Chamado {id} não encontrado" });

            return Ok(chamado);
        }


        // Altera o status de um chamado
        [HttpPatch("chamados/{id:guid}/status")]
        public async Task<IActionResult> AlterarStatus(Guid id, [FromBody] AlterarStatusDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var atualizado = await _service.AlterarStatusAsync(id, dto.Status);
            if (atualizado is null)
                return NotFound(new { mensagem = $"Chamado {id} não encontrado" });

            return Ok(atualizado);
        }


        // Encerra (finaliza) um chamado
        [HttpPatch("chamados/{id:guid}/finalizar")]
        public async Task<IActionResult> Finalizar(Guid id)
        {
            var atualizado = await _service.AlterarStatusAsync(id, StatusProblema.Resolvido);
            if (atualizado is null)
                return NotFound(new { mensagem = $"Chamado {id} não encontrado" });

            return Ok(atualizado);
        }


        // Remove um chamado
        [HttpDelete("chamados/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var removido = await _service.DeleteAsync(id);
            if (!removido)
                return NotFound(new { mensagem = $"Chamado {id} não encontrado" });

            return NoContent();
        }
    }
}
