// Repositories/Implementations/GenericRepository.cs
using System.Linq.Expressions;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IGenericRepository<T> // Generic repository interface defining CRUD operations
    {
        Task AddAsync(T entity, CancellationToken ct = default); // Add a new entity to the database
        void Delete(T entity); // Delete an entity from the database
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default); // Find entities matching a given predicate
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default); // Retrieve all entities of type T
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default); // Retrieve an entity by its ID
        void Update(T entity); // Update an existing entity in the database
    }
}