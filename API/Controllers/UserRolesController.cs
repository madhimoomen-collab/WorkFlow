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
    public class UserRolesController : GenericController<UserRole>
    {
        private readonly IMapper _mapper;

        public UserRolesController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            // ← include Role + filter out soft-deleted records
            var result = await _mediator.Send(new GetListGenericQuery<UserRole>(
                condition: ur => !ur.IsDeleted,
                includes: q => q.Include(ur => ur.Role)
            ));
            return Ok(_mapper.Map<IEnumerable<UserRoleDto>>(result));
        }

        [HttpGet("{id:guid}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<UserRole>(id));
            if (result is null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(_mapper.Map<UserRoleDto>(result));
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] UserRole entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<UserRole>(entity));
            var dto = _mapper.Map<UserRoleDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public override async Task<IActionResult> Update(Guid id, [FromBody] UserRole entity)
        {
            entity.Id = id;
            var result = await _mediator.Send(new UpdateGenericCommand<UserRole>(entity));
            return Ok(_mapper.Map<UserRoleDto>(result));
        }
    }
}