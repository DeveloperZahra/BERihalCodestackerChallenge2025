using BERihalCodestackerChallenge2025.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class // Generic repository implementation for CRUD operations

    {
        private readonly AppDbContext _context; // Database context
        private readonly DbSet<T> _dbSet; // DbSet for the entity type

        public GenericRepository(AppDbContext context) // Constructor accepting the database context
        {
            _context = context; // Initialize the database context
            _dbSet = _context.Set<T>(); // Initialize the DbSet for the entity type
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync(); // Retrieve all entities of type T

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id); // Retrieve an entity by its primary key

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) // Find entities matching a given predicate
            => await _dbSet.Where(predicate).ToListAsync(); // Use LINQ to filter entities

        public async Task AddAsync(T entity) // Add a new entity to the DbSet
        {
            await _dbSet.AddAsync(entity); // Asynchronously add the entity
        }

        public void Update(T entity) // Update an existing entity in the DbSet
        {
            _dbSet.Update(entity); // Update the entity
        }

        public void Delete(T entity) // Delete an entity from the DbSet
        {
            _dbSet.Remove(entity); // Remove the entity
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync(); // Save changes to the database asynchronously
    }
}