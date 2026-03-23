using AutoMapper;
using Domain.Commands;
using Domain.DTOs;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class EdgesController : GenericController<Edge>
    {
        private readonly IMapper _mapper;

        public EdgesController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetListGenericQuery<Edge>());
            return Ok(_mapper.Map<IEnumerable<EdgeDto>>(result));
        }

        [HttpGet("{id:guid}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<Edge>(id));
            if (result is null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(_mapper.Map<EdgeDto>(result));
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] Edge entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<Edge>(entity));
            var dto = _mapper.Map<EdgeDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public override async Task<IActionResult> Update(Guid id, [FromBody] Edge entity)
        {
            entity.Id = id;
            var result = await _mediator.Send(new UpdateGenericCommand<Edge>(entity));
            return Ok(_mapper.Map<EdgeDto>(result));
        }
    }
}
