using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UsersController : GenericController<User>
    {
        public UsersController(IMediator mediator) : base(mediator) { }
    }
}
