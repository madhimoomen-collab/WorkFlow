using AutoMapper;
using Domain.Commands;
using Domain.DTOs;
using Domain.DTOs.Requests;
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
        public new async Task<IActionResult> Update(Guid id, [FromBody] UpdateEdgeRequest request)
        {
            var existing = await _mediator.Send(new GetGenericQuery<Edge>(id));
            if (existing is null) return NotFound();

            // Validate that the new TargetId exists
            var targetNode = await _mediator.Send(new GetGenericQuery<Node>(request.TargetId));
            if (targetNode is null) return BadRequest(new { message = "TargetId does not point to a valid node." });

            existing.Name = request.Name;
            existing.Condition = request.Condition;
            existing.TargetId = request.TargetId;
            // NodeId (source) is intentionally NOT updatable
            var result = await _mediator.Send(new UpdateGenericCommand<Edge>(existing));
            return Ok(_mapper.Map<EdgeDto>(result));
        }
    }
}
