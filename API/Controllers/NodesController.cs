using AutoMapper;
using Domain.Commands;
using Domain.DTOs;
using Domain.DTOs.Requests;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class NodesController : GenericController<Node>
    {
        private readonly IMapper _mapper;

        public NodesController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetListGenericQuery<Node>());
            return Ok(_mapper.Map<IEnumerable<NodeDto>>(result));
        }

        [HttpGet("{id:guid}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<Node>(id));
            if (result is null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(_mapper.Map<NodeDto>(result));
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] Node entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<Node>(entity));
            var dto = _mapper.Map<NodeDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public new async Task<IActionResult> Update(Guid id, [FromBody] UpdateNodeRequest request)
        {
            var existing = await _mediator.Send(new GetGenericQuery<Node>(id));
            if (existing is null) return NotFound();
            existing.Name = request.Name;
            existing.Type = request.Type;
            existing.Description = request.Description;
            existing.RoleReq = request.RoleReq;
            // WorkFlowDefinitionId is intentionally NOT updatable
            var result = await _mediator.Send(new UpdateGenericCommand<Node>(existing));
            return Ok(_mapper.Map<NodeDto>(result));
        }

        /// <summary>Get all outgoing edges for a node, including target node</summary>
        [HttpGet("{id:guid}/outgoingedges")]
        public async Task<IActionResult> GetOutgoingEdges(Guid id)
        {
            var result = await _mediator.Send(new GetListGenericQuery<Edge>(
                condition: e => e.NodeId == id && !e.IsDeleted,
                includes: q => q.Include(e => e.TargetNode)
            ));
            return Ok(_mapper.Map<IEnumerable<EdgeDto>>(result));
        }
    }
}
