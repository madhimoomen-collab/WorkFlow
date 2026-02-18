using MediatR;
using Domain.Queries;
using Domain.Interface;
using Domain.Models;

namespace Domain.Handlers
{
    public class GetGenericHandler<T> : IRequestHandler<GetGenericQuery<T>, T?>
        where T : BaseEntity
    {
        private readonly IGenericRepository<T> _repository;

        public GetGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T?> Handle(GetGenericQuery<T> request, CancellationToken cancellationToken)
        {
            return await _repository.GetAsync(request.Condition, request.Includes);
        }
    }
}