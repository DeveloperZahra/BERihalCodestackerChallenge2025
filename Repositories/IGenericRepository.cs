using System.Linq.Expressions;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IGenericRepository<T>
    {
        Task AddAsync(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task SaveAsync();
        void Update(T entity);
    }
}