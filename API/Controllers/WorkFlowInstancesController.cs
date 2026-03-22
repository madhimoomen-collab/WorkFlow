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
    public class WorkFlowInstancesController : GenericController<WorkFlowInstance>
    {
        private readonly IMapper _mapper;

        public WorkFlowInstancesController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetListGenericQuery<WorkFlowInstance>(
                condition: i => !i.IsDeleted,
                includes: q => q.Include(i => i.CurrentNode)
            ));
            return Ok(_mapper.Map<IEnumerable<WorkFlowInstanceDto>>(result));
        }

        [HttpGet("{id:guid}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<WorkFlowInstance>(id));
            if (result is null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(_mapper.Map<WorkFlowInstanceDto>(result));
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] WorkFlowInstance entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<WorkFlowInstance>(entity));
            var dto = _mapper.Map<WorkFlowInstanceDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public override async Task<IActionResult> Update(Guid id, [FromBody] WorkFlowInstance entity)
        {
            entity.Id = id;
            var result = await _mediator.Send(new UpdateGenericCommand<WorkFlowInstance>(entity));
            return Ok(_mapper.Map<WorkFlowInstanceDto>(result));
        }

        /// <summary>Get all active (non-completed, non-cancelled) instances</summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _mediator.Send(new GetListGenericQuery<WorkFlowInstance>(
                condition: i => !i.IsDeleted
                             && i.Status != "Completed"
                             && i.Status != "Cancelled"
            ));
            return Ok(_mapper.Map<IEnumerable<WorkFlowInstanceDto>>(result));
        }

        /// <summary>Get the full transition history of an instance</summary>
        [HttpGet("{id:guid}/history")]
        public async Task<IActionResult> GetHistory(Guid id)
        {
            var result = await _mediator.Send(new GetListGenericQuery<WorkFlowInstanceHistory>(
                condition: h => h.WorkFlowInstanceId == id,
                includes: q => q.Include(h => h.FromNode).Include(h => h.ToNode),
                orderBy: q => q.OrderBy(h => h.TransitionedAt)
            ));
            return Ok(_mapper.Map<IEnumerable<WorkFlowInstanceHistoryDto>>(result));
        }

        /// <summary>Start a pending workflow instance</summary>
        [HttpPost("{id:guid}/start")]
        public async Task<IActionResult> Start(Guid id)
        {
            var instance = await _mediator.Send(new GetGenericQuery<WorkFlowInstance>(id));
            if (instance is null) return NotFound();
            if (instance.Status != "Pending")
                return Conflict(new { message = "Instance is not in Pending status." });

            var startNodes = await _mediator.Send(new GetListGenericQuery<Node>(
                condition: n => n.WorkFlowDefinitionId == instance.WorkFlowDefinitionId
                             && n.Type.ToUpper() == "START" && !n.IsDeleted
            ));
            var startNode = startNodes.FirstOrDefault();
            if (startNode is null)
                return Conflict(new { message = "No Start node found for this workflow." });

            instance.NodeId = startNode.Id;
            instance.Status = "InProgress";
            var result = await _mediator.Send(new UpdateGenericCommand<WorkFlowInstance>(instance));
            return Ok(_mapper.Map<WorkFlowInstanceDto>(result));
        }

        /// <summary>Advance an in-progress instance along an edge</summary>
        [HttpPost("{id:guid}/advance")]
        public async Task<IActionResult> Advance(Guid id, [FromBody] AdvanceRequest request)
        {
            var instance = await _mediator.Send(new GetGenericQuery<WorkFlowInstance>(id));
            if (instance is null) return NotFound();
            if (instance.Status != "InProgress")
                return Conflict(new { message = "Instance is not in InProgress status." });

            var edge = await _mediator.Send(new GetGenericQuery<Edge>(request.EdgeId));
            if (edge is null) return NotFound(new { message = "Edge not found." });
            if (edge.NodeId != instance.NodeId)
                return Conflict(new { message = "Edge does not belong to the current node." });

            var targetNode = await _mediator.Send(new GetGenericQuery<Node>(edge.TargetId));
            if (targetNode is null) return NotFound(new { message = "Target node not found." });

            await _mediator.Send(new AddGenericCommand<WorkFlowInstanceHistory>(new WorkFlowInstanceHistory
            {
                Id = Guid.NewGuid(),
                WorkFlowInstanceId = instance.Id,
                FromNodeId = instance.NodeId,
                ToNodeId = edge.TargetId,
                TransitionedAt = DateTime.UtcNow
            }));

            instance.NodeId = edge.TargetId;
            if (targetNode.Type.Equals("End", StringComparison.OrdinalIgnoreCase))
            {
                instance.Status = "Completed";
                instance.CompletedAt = DateTime.UtcNow;
            }

            var result = await _mediator.Send(new UpdateGenericCommand<WorkFlowInstance>(instance));
            return Ok(_mapper.Map<WorkFlowInstanceDto>(result));
        }

        /// <summary>Cancel a pending or in-progress instance</summary>
        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var instance = await _mediator.Send(new GetGenericQuery<WorkFlowInstance>(id));
            if (instance is null) return NotFound();
            if (instance.Status is "Completed" or "Cancelled")
                return Conflict(new { message = "Instance is already completed or cancelled." });

            instance.Status = "Cancelled";
            var result = await _mediator.Send(new UpdateGenericCommand<WorkFlowInstance>(instance));
            return Ok(_mapper.Map<WorkFlowInstanceDto>(result));
        }
    }

    public record AdvanceRequest(Guid EdgeId);
}
