using MediatR;
using Domain.Models;

namespace Domain.Commands
{
    public class DeleteGenericCommand<T> : IRequest<bool> where T : BaseEntity
    {
        public int Id { get; set; }

        public DeleteGenericCommand(int id)
        {
            Id = id;
        }
    }
}