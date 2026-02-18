using MediatR;
using Domain.Queries;
using Domain.Interface;
using Domain.Models;

namespace Domain.Handlers
{
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
            return await _repository.FindAsync(
                predicate: request.Condition,
                includes: request.Includes,
                orderBy: request.OrderBy
            );
        }
    }
}