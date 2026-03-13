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
    public class WorkFlowInstanceHistoriesController : GenericController<WorkFlowInstanceHistory>
    {
        private readonly IMapper _mapper;

        public WorkFlowInstanceHistoriesController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetListGenericQuery<WorkFlowInstanceHistory>());
            return Ok(_mapper.Map<IEnumerable<WorkFlowInstanceHistoryDto>>(result));
        }

        [HttpGet("{id:guid}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<WorkFlowInstanceHistory>(id));
            if (result is null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(_mapper.Map<WorkFlowInstanceHistoryDto>(result));
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] WorkFlowInstanceHistory entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<WorkFlowInstanceHistory>(entity));
            var dto = _mapper.Map<WorkFlowInstanceHistoryDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public new async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkFlowInstanceHistoryRequest request)
        {
            var existing = await _mediator.Send(new GetGenericQuery<WorkFlowInstanceHistory>(id));
            if (existing is null) return NotFound();
            // Only the comment is editable; node refs and timestamp are immutable
            existing.Comment = request.Comment;
            var result = await _mediator.Send(new UpdateGenericCommand<WorkFlowInstanceHistory>(existing));
            return Ok(_mapper.Map<WorkFlowInstanceHistoryDto>(result));
        }
    }
}
