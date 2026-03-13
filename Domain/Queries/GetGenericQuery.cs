using MediatR;
using Domain.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Domain.Queries
{
    public class GetGenericQuery<T> : IRequest<T?> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Condition { get; }
        public Func<IQueryable<T>, IIncludableQueryable<T, object?>>? Includes { get; }

        public GetGenericQuery(
            Expression<Func<T, bool>> condition,
            Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            Includes = includes;
        }

        /// <summary>Convenience constructor for Guid-based ID queries</summary>
        public GetGenericQuery(Guid id)
            : this(entity => entity.Id == id && !entity.IsDeleted, null)
        {
        }
    }
}