using Mirante.Models.Entities;

namespace Mirante.Data.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Tarefa> TarefaRepository { get; }
        Task<int> CommitAsync();
    }
}
