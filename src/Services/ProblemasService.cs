using CidadeAtivaApi.Data;
using CidadeAtivaApi.DTOs;
using CidadeAtivaApi.Models.Enum;
using CidadeAtivaApi.Models;
using Microsoft.EntityFrameworkCore;


namespace CidadeAtivaApi.Services
{
    public class ProblemasService
    {
        private readonly AppDB _db;
        public ProblemasService(AppDB db) => _db = db;

        // --- GET ALL ---
        // Admin vê todos; usuário comum vê apenas os seus próprios
        public async Task<List<RespostaProblemaDTO>> GetAllAsync(
            TipoProblema? tipo,
            StatusProblema? status,
            int? userId = null,
            bool isAdmin = false)
        {
            var query = _db.Problamas.AsQueryable();

            if (!isAdmin && userId.HasValue)
                query = query.Where(p => p.UserId == userId);

            if (tipo.HasValue)
                query = query.Where(p => p.Tipo == tipo.Value);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            var lista = await query
                .OrderByDescending(p => p.CriadoEm)
                .ToListAsync();

            return lista.Select(p => ToDto(p)).ToList();
        }

        // --- GET BY ID ---
        // Admin acessa qualquer chamado; usuário comum acessa apenas os seus
        public async Task<RespostaProblemaDTO?> GetByIdAsync(Guid id, int? userId = null, bool isAdmin = false)
        {
            var query = _db.Problamas.AsQueryable();

            if (!isAdmin && userId.HasValue)
                query = query.Where(p => p.UserId == userId);

            var problema = await query.FirstOrDefaultAsync(p => p.Id == id);
            return problema is null ? null : ToDto(problema);
        }

        // --- CREATE ---
        public async Task<RespostaProblemaDTO> CreateAsync(CriarProblema dto, int userId)
        {
            var problemaTask = new ProblamasUrbano
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Tipo = dto.Tipo,
                Bairro = dto.Bairro,
                UserId = userId
            };

            _db.Problamas.Add(problemaTask);
            await _db.SaveChangesAsync();
            return ToDto(problemaTask);
        }

        // --- UPDATE (apenas Admin) ---
        public async Task<RespostaProblemaDTO?> UpdateAsync(Guid id, AtualizarProblema dto)
        {
            var problema = await _db.Problamas.FindAsync(id);
            if (problema is null) return null;

            problema.Titulo = dto.Titulo;
            problema.Descricao = dto.Descricao;
            problema.Tipo = dto.Tipo;
            problema.Status = dto.Status;
            problema.Bairro = dto.Bairro;
            problema.AtualizadoEm = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ToDto(problema);
        }

        // --- GET CHAMADO (Admin — acessa qualquer) ---
        public async Task<RespostaProblemaDTO?> GetChamadoAsync(Guid id)
        {
            var problema = await _db.Problamas.FindAsync(id);
            return problema is null ? null : ToDto(problema);
        }

        // --- ALTERAR STATUS (apenas Admin) ---
        public async Task<RespostaProblemaDTO?> AlterarStatusAsync(Guid id, StatusProblema novoStatus)
        {
            var problema = await _db.Problamas.FindAsync(id);
            if (problema is null) return null;

            problema.Status = novoStatus;
            problema.AtualizadoEm = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ToDto(problema);
        }

        // --- DELETE (apenas Admin) ---
        public async Task<bool> DeleteAsync(Guid id)
        {
            var problema = await _db.Problamas.FindAsync(id);
            if (problema is null) return false;

            _db.Problamas.Remove(problema);
            await _db.SaveChangesAsync();
            return true;
        }

        private static RespostaProblemaDTO ToDto(ProblamasUrbano p) => new()
        {
            Id = p.Id,
            Titulo = p.Titulo,
            Descricao = p.Descricao,
            Tipo = p.Tipo.ToString(),
            Status = p.Status.ToString(),
            Bairro = p.Bairro,
            CriadoEm = p.CriadoEm,
            AtualizadoEm = p.AtualizadoEm,
            UserId = p.UserId
        };
    }
}
