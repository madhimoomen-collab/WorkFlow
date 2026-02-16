using MediatR;
using Domain.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Domain.Queries
{
    /// <summary>
    /// Generic query to get a single entity with flexible filtering and includes
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public class GetGenericQuery<T> : IRequest<T?> where T : BaseEntity
    {
        /// <summary>
        /// Filter condition (e.g., x => x.Id == 5 or x => x.Email == "test@test.com")
        /// </summary>
        public Expression<Func<T, bool>> Condition { get; }

        /// <summary>
        /// Include related entities (supports ThenInclude chains)
        /// Example: query => query.Include(x => x.Client).ThenInclude(c => c.Address)
        /// </summary>
        public Func<IQueryable<T>, IIncludableQueryable<T, object?>>? Includes { get; }

        /// <summary>
        /// Constructor for flexible querying
        /// </summary>
        /// <param name="condition">Filter expression</param>
        /// <param name="includes">Optional includes for eager loading</param>
        public GetGenericQuery(
            Expression<Func<T, bool>> condition,
            Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            Includes = includes;
        }

        /// <summary>
        /// Convenience constructor for ID-based queries
        /// </summary>
        /// <param name="id">Entity ID</param>
        public GetGenericQuery(int id)
            : this(entity => entity.Id == id, null)
        {
        }
    }
}
