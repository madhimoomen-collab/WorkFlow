using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class EdgesController : GenericController<Edge>
    {
        public EdgesController(IMediator mediator) : base(mediator) { }
    }
}
