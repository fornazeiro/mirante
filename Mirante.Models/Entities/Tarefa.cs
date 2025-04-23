using Mirante.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirante.Models.Entities
{
    public class Tarefa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        [Required]
        public StatusTarefa Status { get; set; }

        public DateTime DataVencimento { get; set; }
    }
}
