using System.ComponentModel.DataAnnotations;
using CidadeAtivaApi.Models.Enum;

namespace CidadeAtivaApi.DTOs
{
    public class AlterarStatusDTO
    {
        [Required(ErrorMessage = " O status é obrigatório ")]
        [EnumDataType(typeof(StatusProblema), ErrorMessage = " Status inválido ")]
        public StatusProblema Status { get; set; }
    }
}
