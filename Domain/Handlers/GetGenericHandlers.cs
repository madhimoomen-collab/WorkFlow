using MediatR;
using Domain.Queries;
using Domain.Interface;
using Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Handlers
{
    /// <summary>
    /// Updated handler to support flexible Expression-based queries
    /// </summary>
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
            // Use the new GetAsync method with IIncludableQueryable support
            return await _repository.GetAsync(request.Condition, request.Includes);
        }
    }
}
