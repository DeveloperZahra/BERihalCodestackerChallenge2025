using System.Linq.Expressions;

namespace BERihalCodestackerChallenge2025.Repositories // Generic repository interface for CRUD operations
{
    public interface IGenericRepository<T> // Generic repository interface for CRUD operations
    {
        Task AddAsync(T entity); // Add a new entity
        void Delete(T entity); // Delete an entity
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate); // Find entities matching a given predicate
        Task<IEnumerable<T>> GetAllAsync(); // Retrieve all entities
        Task<T?> GetByIdAsync(int id); // Retrieve an entity by its primary key
        Task SaveAsync(); // Save changes to the database
        void Update(T entity); // Update an existing entity
    }
}