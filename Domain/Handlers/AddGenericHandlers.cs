using MediatR;
using Domain.Commands;
using Domain.Interface;
using Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Handlers
{
    public class AddGenericHandler<T> : IRequestHandler<AddGenericCommand<T>, T>
        where T : BaseEntity
    {
        private readonly IGenericRepository<T> _repository;

        public AddGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> Handle(AddGenericCommand<T> request, CancellationToken cancellationToken)
        {
            var result = await _repository.AddAsync(request.Entity);
            await _repository.SaveChangesAsync();
            return result;
        }
    }
}