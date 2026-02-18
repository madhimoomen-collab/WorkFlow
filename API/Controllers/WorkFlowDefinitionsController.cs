using Domain.Commands;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class WorkFlowDefinitionsController : GenericController<WorkFlowDefinition>
    {
        public WorkFlowDefinitionsController(IMediator mediator) : base(mediator) { }

        /// <summary>Get a workflow definition with its full graph (nodes + edges)</summary>
        [HttpGet("{id:guid}/graph")]
        public async Task<IActionResult> GetGraph(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<WorkFlowDefinition>(
                condition: x => x.Id == id && !x.IsDeleted,
                includes: q => q.Include(w => w.Nodes)
                                .ThenInclude(n => n.OutgoingEdges)
            ));
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>Get all nodes for a workflow definition</summary>
        [HttpGet("{id:guid}/nodes")]
        public async Task<IActionResult> GetNodes(Guid id)
        {
            var result = await _mediator.Send(new GetListGenericQuery<Node>(
                condition: n => n.WorkFlowDefinitionId == id && !n.IsDeleted
            ));
            return Ok(result);
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

            return CreatedAtAction(nameof(GetById), new { id = newDefinition.Id }, newDefinition);
        }
    }
}
