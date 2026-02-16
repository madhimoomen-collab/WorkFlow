using MediatR;
using Domain.Models;

namespace Domain.Commands
{
    public class AddGenericCommand<T> : IRequest<T> where T : BaseEntity
    {
        public T Entity { get; set; }

        public AddGenericCommand(T entity)
        {
            Entity = entity;
        }
    }
}