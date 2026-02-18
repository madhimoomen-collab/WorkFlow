using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserRolesController : GenericController<UserRole>
    {
        public UserRolesController(IMediator mediator) : base(mediator) { }
    }
}
