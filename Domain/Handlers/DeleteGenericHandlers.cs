using MediatR;
using Domain.Commands;
using Domain.Interface;
using Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Handlers
{
    public class DeleteGenericHandler<T> : IRequestHandler<DeleteGenericCommand<T>, bool>
        where T : BaseEntity
    {
        private readonly IGenericRepository<T> _repository;

        public DeleteGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteGenericCommand<T> request, CancellationToken cancellationToken)
        {
            var result = await _repository.DeleteAsync(request.Id);
            if (result)
            {
                await _repository.SaveChangesAsync();
            }
            return result;
        }
    }
}