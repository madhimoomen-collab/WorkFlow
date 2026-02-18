using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NodesController : GenericController<Node>
    {
        public NodesController(IMediator mediator) : base(mediator) { }
    }
}
