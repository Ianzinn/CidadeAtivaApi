using CidadeAtivaApi.Models.Enum;

namespace CidadeAtivaApi.Models
{
    public class ProblamasUrbano
    {
        // GUID: é um Unique identifer pro ID, parecido da forma que funciona no SQL
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Titulo { get; set; }
        public string? Descricao { get; set; }

        // Importanto os Enum
        public TipoProblema Tipo { get; set; }
        public StatusProblema Status { get; set; } = StatusProblema.Aberto;

        public string? Bairro { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime? AtualizadoEm { get; set; }

        // Relacionamento com o usuário que criou o chamado
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}

