using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CidadeAtivaApi.DTOs
{
    public class RespostaProblemaDTO
    {
        // O "Status" e "Tipo" viram string na resposta (ex: "Buraco") em vez de numero (0)
        // pra ficar mais legivel para quem consome a API

        public Guid Id { get; set; }
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? Tipo { get; set; } 
        public string? Status { get; set; } 
        public string? Bairro { get; set; } 
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public int? UserId { get; set; }
    }
}