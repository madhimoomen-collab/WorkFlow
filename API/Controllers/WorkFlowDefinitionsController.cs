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
    public class WorkFlowDefinitionsController : GenericController<WorkFlowDefinition>
    {
        private readonly IMapper _mapper;

        public WorkFlowDefinitionsController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetListGenericQuery<WorkFlowDefinition>());
            return Ok(_mapper.Map<IEnumerable<WorkFlowDefinitionDto>>(result));
        }

        [HttpGet("{id:guid}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<WorkFlowDefinition>(id));
            if (result is null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(_mapper.Map<WorkFlowDefinitionDto>(result));
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] WorkFlowDefinition entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<WorkFlowDefinition>(entity));
            var dto = _mapper.Map<WorkFlowDefinitionDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public new async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkFlowDefinitionRequest request)
        {
            var existing = await _mediator.Send(new GetGenericQuery<WorkFlowDefinition>(id));
            if (existing is null) return NotFound();
            existing.Name = request.Name;
            existing.Version = request.Version;
            var result = await _mediator.Send(new UpdateGenericCommand<WorkFlowDefinition>(existing));
            return Ok(_mapper.Map<WorkFlowDefinitionDto>(result));
        }

        /// <summary>Get a workflow definition with its full graph (nodes + edges)</summary>
        [HttpGet("{id:guid}/graph")]
        public async Task<IActionResult> GetGraph(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<WorkFlowDefinition>(
                condition: x => x.Id == id && !x.IsDeleted,
                includes: q => q.Include(w => w.Nodes)
                                .ThenInclude(n => n.OutgoingEdges)
            ));
            if (result is null) return NotFound();
            return Ok(_mapper.Map<WorkFlowDefinitionGraphDto>(result));
        }

        /// <summary>Get all nodes for a workflow definition</summary>
        [HttpGet("{id:guid}/nodes")]
        public async Task<IActionResult> GetNodes(Guid id)
        {
            var result = await _mediator.Send(new GetListGenericQuery<Node>(
                condition: n => n.WorkFlowDefinitionId == id && !n.IsDeleted
            ));
            return Ok(_mapper.Map<IEnumerable<NodeDto>>(result));
        }

        /// <summary>Deep-clone a workflow definition with all nodes and edges</summary>
        [HttpPost("{id:guid}/clone")]
        public async Task<IActionResult> Clone(Guid id)
        {
            var source = await _mediator.Send(new GetGenericQuery<WorkFlowDefinition>(
                condition: x => x.Id == id && !x.IsDeleted,
                includes: q => q.Include(w => w.Nodes).ThenInclude(n => n.OutgoingEdges)
            ));
            if (source is null) return NotFound();

            var newDefinition = new WorkFlowDefinition
            {
                Id = Guid.NewGuid(),
                Name = source.Name,
                Version = source.Version + 1
            };
            await _mediator.Send(new AddGenericCommand<WorkFlowDefinition>(newDefinition));

            var nodeIdMap = new Dictionary<Guid, Guid>();
            foreach (var node in source.Nodes)
            {
                var newNodeId = Guid.NewGuid();
                nodeIdMap[node.Id] = newNodeId;
                await _mediator.Send(new AddGenericCommand<Node>(new Node
                {
                    Id = newNodeId,
                    Name = node.Name,
                    Type = node.Type,
                    Description = node.Description,
                    RoleReq = node.RoleReq,
                    WorkFlowDefinitionId = newDefinition.Id
                }));
            }

            foreach (var node in source.Nodes)
            {
                foreach (var edge in node.OutgoingEdges)
                {
                    await _mediator.Send(new AddGenericCommand<Edge>(new Edge
                    {
                        Id = Guid.NewGuid(),
                        Name = edge.Name,
                        Condition = edge.Condition,
                        NodeId = nodeIdMap[edge.NodeId],
                        TargetId = nodeIdMap[edge.TargetId]
                    }));
                }
            }

            var dto = _mapper.Map<WorkFlowDefinitionDto>(newDefinition);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
    }
}
