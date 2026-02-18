using MediatR;
using Domain.Models;

namespace Domain.Commands
{
    public class DeleteGenericCommand<T> : IRequest<bool> where T : BaseEntity
    {
        public Guid Id { get; set; }

        public DeleteGenericCommand(Guid id)
        {
            Id = id;
        }
    }
}