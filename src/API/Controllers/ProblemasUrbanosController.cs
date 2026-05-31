using System.Security.Claims;
using CidadeAtivaApi.DTOs;
using CidadeAtivaApi.Models.Enum;
using CidadeAtivaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CidadeAtivaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProblemasUrbanosController : ControllerBase
    {
        private readonly ProblemasService _service;
        public ProblemasUrbanosController(ProblemasService service) => _service = service;

        // Extrai o ID do usuário autenticado do token JWT
        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsAdmin() =>
            User.IsInRole("Admin");


        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] TipoProblema? tipo,
            [FromQuery] StatusProblema? status)
        {
            var lista = await _service.GetAllAsync(tipo, status, GetUserId(), IsAdmin());
            return Ok(lista);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var problema = await _service.GetByIdAsync(id, GetUserId(), IsAdmin());
            if (problema is null)
                return NotFound(new { mensagem = $"ID {id} não encontrado" });

            return Ok(problema);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriarProblema dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var criado = await _service.CreateAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }


        // Apenas Admin pode alterar dados e status de chamados
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarProblema dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var atualizado = await _service.UpdateAsync(id, dto);
            if (atualizado is null)
                return NotFound(new { mensagem = $"Problema {id} não encontrado" });

            return Ok(atualizado);
        }


        // Apenas Admin pode remover chamados
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var removido = await _service.DeleteAsync(id);
            if (!removido)
                return NotFound(new { mensagem = $"Problema {id} não encontrado" });

            return NoContent();
        }
    }
}
