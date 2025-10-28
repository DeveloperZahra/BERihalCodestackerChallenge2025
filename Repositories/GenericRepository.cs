using BERihalCodestackerChallenge2025.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class // Generic repository implementation for CRUD operations

    {
        private readonly AppDbContext _context; // Database context
        private readonly DbSet<T> _dbSet; // DbSet for the entity type

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}