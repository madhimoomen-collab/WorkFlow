using MediatR;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Domain.Queries
{
    /// <summary>
    /// Generic query to get a list of entities with flexible filtering and includes
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public class GetListGenericQuery<T> : IRequest<IEnumerable<T>> where T : BaseEntity
    {
        /// <summary>
        /// Filter condition (null = get all)
        /// Example: x => x.IsActive == true
        /// </summary>
        public Expression<Func<T, bool>>? Condition { get; }

        /// <summary>
        /// Include related entities (supports ThenInclude chains)
        /// Example: query => query.Include(x => x.Book).Include(x => x.Client)
        /// </summary>
        public Func<IQueryable<T>, IIncludableQueryable<T, object?>>? Includes { get; }

        /// <summary>
        /// Optional ordering
        /// Example: query => query.OrderBy(x => x.CreatedDate)
        /// </summary>
        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; }

        /// <summary>
        /// Constructor for flexible querying
        /// </summary>
        /// <param name="condition">Optional filter expression (null = get all)</param>
        /// <param name="includes">Optional includes for eager loading</param>
        /// <param name="orderBy">Optional ordering</param>
        public GetListGenericQuery(
            Expression<Func<T, bool>>? condition = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            Condition = condition;
            Includes = includes;
            OrderBy = orderBy;
        }
    }
}
