using Mirante.Models.DTOs;
using Mirante.Models.Entities;
using Mirante.Models.Enums;

namespace Mirante.Services.Interfaces
{
    public  interface ITarefaService
    {
        Task<Tarefa> GetTarefaByIdAsync(int id);
        Task<IEnumerable<Tarefa>> GetAllTarefasAsync(StatusTarefa? status, DateTime? dataVencimento);
        Task<Tarefa> CreateTarefaAsync(TarefaDTO tarefaDto);
        Task UpdateTarefaAsync(int id, TarefaDTO tarefaDto);
        Task DeleteTarefaAsync(int id);
    }
}
