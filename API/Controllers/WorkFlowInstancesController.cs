using Domain.Commands;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class WorkFlowInstancesController : GenericController<WorkFlowInstance>
    {
        public WorkFlowInstancesController(IMediator mediator) : base(mediator) { }

        /// <summary>Get all active (non-completed, non-cancelled) instances</summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _mediator.Send(new GetListGenericQuery<WorkFlowInstance>(
                condition: i => !i.IsDeleted
                             && i.Status != "Completed"
                             && i.Status != "Cancelled"
            ));
            return Ok(result);
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
            return Ok(result);
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
                             && n.Type == "Start" && !n.IsDeleted
            ));
            var startNode = startNodes.FirstOrDefault();
            if (startNode is null)
                return Conflict(new { message = "No Start node found for this workflow." });

            instance.NodeId = startNode.Id;
            instance.Status = "InProgress";
            var result = await _mediator.Send(new UpdateGenericCommand<WorkFlowInstance>(instance));
            return Ok(result);
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
            if (targetNode.Type == "End")
            {
                instance.Status = "Completed";
                instance.CompletedAt = DateTime.UtcNow;
            }

            var result = await _mediator.Send(new UpdateGenericCommand<WorkFlowInstance>(instance));
            return Ok(result);
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
            return Ok(result);
        }
    }

    public record AdvanceRequest(Guid EdgeId);
}
