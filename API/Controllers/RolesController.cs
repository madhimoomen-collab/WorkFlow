using AutoMapper;
using Domain.Commands;
using Domain.DTOs;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class RolesController : GenericController<Role>
    {
        private readonly IMapper _mapper;

        public RolesController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetListGenericQuery<Role>());
            return Ok(_mapper.Map<IEnumerable<RoleDto>>(result));
        }

        [HttpGet("{id:guid}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<Role>(id));
            if (result is null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(_mapper.Map<RoleDto>(result));
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] Role entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<Role>(entity));
            var dto = _mapper.Map<RoleDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public override async Task<IActionResult> Update(Guid id, [FromBody] Role entity)
        {
            entity.Id = id;
            var result = await _mediator.Send(new UpdateGenericCommand<Role>(entity));
            return Ok(_mapper.Map<RoleDto>(result));
        }
    }
}
