using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : GenericController<User>
    {
        public UsersController(IMediator mediator) : base(mediator) { }

        /// <summary>Get all roles for a user</summary>
        [HttpGet("{id:guid}/roles")]
        public async Task<IActionResult> GetRoles(Guid id)
        {
            var result = await _mediator.Send(new GetListGenericQuery<UserRole>(
                condition: ur => ur.UserId == id && !ur.IsDeleted,
                includes: q => q.Include(ur => ur.Role)
            ));
            return Ok(result);
        }
    }
}
