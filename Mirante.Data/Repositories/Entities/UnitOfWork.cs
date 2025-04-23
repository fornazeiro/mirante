using Mirante.Data.Repositories.Interfaces;
using Mirante.Models.Entities;

namespace Mirante.Data.Repositories.Entities
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ToDoDbContext _context;
        private IRepository<Tarefa> _tarefaRepository;

        public UnitOfWork(ToDoDbContext context)
        {
            _context = context;
        }

        public IRepository<Tarefa> TarefaRepository => _tarefaRepository ??= new Repository<Tarefa>(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
