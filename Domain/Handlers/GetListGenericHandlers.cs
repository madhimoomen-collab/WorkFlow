using MediatR;
using Domain.Queries;
using Domain.Interface;
using Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Handlers
{
    /// <summary>
    /// Updated handler to support flexible Expression-based list queries
    /// </summary>
    public class GetListGenericHandler<T> : IRequestHandler<GetListGenericQuery<T>, IEnumerable<T>>
        where T : BaseEntity
    {
        private readonly IGenericRepository<T> _repository;

        public GetListGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<T>> Handle(GetListGenericQuery<T> request, CancellationToken cancellationToken)
        {
            // Use the new advanced FindAsync method with IIncludableQueryable support
            return await _repository.FindAsync(
                predicate: request.Condition,
                includes: request.Includes,
                orderBy: request.OrderBy
            );
        }
    }
}
