using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class WorkFlowDefinitionsController : GenericController<WorkFlowDefinition>
    {
        public WorkFlowDefinitionsController(IMediator mediator) : base(mediator) { }
    }
}
