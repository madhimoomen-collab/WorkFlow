using MediatR;
using Domain.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Domain.Queries
{
    public class GetListGenericQuery<T> : IRequest<IEnumerable<T>> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Condition { get; }
        public Func<IQueryable<T>, IIncludableQueryable<T, object?>>? Includes { get; }
        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; }

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