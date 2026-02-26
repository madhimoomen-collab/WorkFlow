using AutoMapper;
using Domain.Commands;
using Domain.DTOs;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : GenericController<User>
    {
        private readonly IMapper _mapper;

        public UsersController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetListGenericQuery<User>());
            return Ok(_mapper.Map<IEnumerable<UserDto>>(result));
        }

        [HttpGet("{id:guid}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<User>(id));
            if (result is null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(_mapper.Map<UserDto>(result));
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] User entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<User>(entity));
            var dto = _mapper.Map<UserDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public override async Task<IActionResult> Update(Guid id, [FromBody] User entity)
        {
            entity.Id = id;
            var result = await _mediator.Send(new UpdateGenericCommand<User>(entity));
            return Ok(_mapper.Map<UserDto>(result));
        }

        /// <summary>Get all roles for a user</summary>
        [HttpGet("{id:guid}/roles")]
        public async Task<IActionResult> GetRoles(Guid id)
        {
            var result = await _mediator.Send(new GetListGenericQuery<UserRole>(
                condition: ur => ur.UserId == id && !ur.IsDeleted,
                includes: q => q.Include(ur => ur.Role)
            ));
            return Ok(_mapper.Map<IEnumerable<UserRoleDto>>(result));
        }
    }
}
