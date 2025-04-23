using Mirante.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mirante.Models.DTOs
{
    public class TarefaDTO
    {
        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        [Required]
        public StatusTarefa Status { get; set; }

        public DateTime DataVencimento { get; set; }
    }
}
