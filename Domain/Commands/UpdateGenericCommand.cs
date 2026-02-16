using MediatR;
using Domain.Models;

namespace Domain.Commands
{
    public class UpdateGenericCommand<T> : IRequest<T> where T : BaseEntity
    {
        public T Entity { get; set; }

        public UpdateGenericCommand(T entity)
        {
            Entity = entity;
        }
    }
}