// Repositories/Implementations/GenericRepository.cs
using BERihalCodestackerChallenge2025.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class // Generic repository implementation for CRUD operations
    {
        protected readonly AppDbContext _db; // Database context
        protected readonly DbSet<T> _set; // DbSet for the entity type T
        public GenericRepository(AppDbContext db) // Constructor accepting the database context
        {
            _db = db;
            _set = db.Set<T>(); // Initialize the DbSet for the entity type T
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) // Retrieve an entity by its ID
            => await _set.FindAsync(new object?[] { id }, ct); // Use FindAsync to locate the entity by its primary key

      

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default) // Retrieve all entities of type T
            => await _set.AsNoTracking().ToListAsync(ct); // Use AsNoTracking for read-only operations and convert to a list

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) // Find entities matching a given predicate
            => await _set.AsNoTracking().Where(predicate).ToListAsync(ct); // Use the predicate to filter entities and convert to a list

        public virtual async Task AddAsync(T entity, CancellationToken ct = default) // Add a new entity to the database
            => await _set.AddAsync(entity, ct); // Use AddAsync to add the entity to the DbSet

        public virtual void Update(T entity) => _set.Update(entity); // Update an existing entity in the database

        public virtual void Delete(T entity) => _set.Remove(entity); // Delete an entity from the database


       

    }
}