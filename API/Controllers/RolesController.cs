using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class RolesController : GenericController<Role>
    {
        public RolesController(IMediator mediator) : base(mediator) { }
    }
}
