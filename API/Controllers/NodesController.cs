using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class NodesController : GenericController<Node>
    {
        public NodesController(IMediator mediator) : base(mediator) { }

        /// <summary>Get all outgoing edges for a node, including target node</summary>
        [HttpGet("{id:guid}/outgoingedges")]
        public async Task<IActionResult> GetOutgoingEdges(Guid id)
        {
            var result = await _mediator.Send(new GetListGenericQuery<Edge>(
                condition: e => e.NodeId == id && !e.IsDeleted,
                includes: q => q.Include(e => e.TargetNode)
            ));
            return Ok(result);
        }
    }
}
