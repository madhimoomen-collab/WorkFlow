using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace Domain.Interface
{
    /// <summary>
    /// Generic repository interface with flexible querying support
    /// Combines the best of both your implementation and instructor's reference
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get a single entity by ID
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Get all entities (respects soft delete filter)
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Find entities with simple predicate and includes (EXISTING METHOD)
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find entities with predicate and multiple includes (EXISTING METHOD)
        /// </summary>
        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// NEW: Advanced query with IIncludableQueryable support
        /// Allows complex includes like: query => query.Include(x => x.Client).ThenInclude(c => c.Address)
        /// </summary>
        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        /// <summary>
        /// NEW: Get a single entity with advanced includes
        /// </summary>
        Task<T?> GetAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null);

        /// <summary>
        /// Add a new entity
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Update an existing entity
        /// </summary>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Soft delete an entity by ID
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Save changes to database
        /// </summary>
        Task<bool> SaveChangesAsync();
    }
}
