using MediatR;
using Domain.Commands;
using Domain.Interface;
using Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Handlers
{
    public class UpdateGenericHandler<T> : IRequestHandler<UpdateGenericCommand<T>, T>
        where T : BaseEntity
    {
        private readonly IGenericRepository<T> _repository;

        public UpdateGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> Handle(UpdateGenericCommand<T> request, CancellationToken cancellationToken)
        {
            request.Entity.UpdatedDate = DateTime.Now;
            var result = await _repository.UpdateAsync(request.Entity);
            await _repository.SaveChangesAsync();
            return result;
        }
    }
}