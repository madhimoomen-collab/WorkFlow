using Domain.Commands;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class GenericController<T> : ControllerBase where T : BaseEntity
    {
        protected readonly IMediator _mediator;

        protected GenericController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all records (non-deleted)</summary>
        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetListGenericQuery<T>());
            return Ok(result);
        }

        /// <summary>Get a single record by Guid ID</summary>
        [HttpGet("{id:guid}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGenericQuery<T>(id));
            if (result == null) return NotFound(new { message = $"Record with id '{id}' not found." });
            return Ok(result);
        }

        /// <summary>Create a new record</summary>
        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] T entity)
        {
            entity.Id = Guid.NewGuid();
            var result = await _mediator.Send(new AddGenericCommand<T>(entity));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update an existing record</summary>
        [HttpPut("{id:guid}")]
        public virtual async Task<IActionResult> Update(Guid id, [FromBody] T entity)
        {
            entity.Id = id;
            var result = await _mediator.Send(new UpdateGenericCommand<T>(entity));
            return Ok(result);
        }

        /// <summary>Soft-delete a record</summary>
        [HttpDelete("{id:guid}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteGenericCommand<T>(id));
            if (!result) return NotFound(new { message = $"Record with id '{id}' not found." });
            return NoContent();
        }
    }
}
