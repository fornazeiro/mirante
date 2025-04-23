using Microsoft.EntityFrameworkCore;
using Mirante.Data.Repositories.Interfaces;

namespace Mirante.Data.Repositories.Entities
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ToDoDbContext _context;

        public Repository(ToDoDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }


        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity); ;
        }
    }
}
