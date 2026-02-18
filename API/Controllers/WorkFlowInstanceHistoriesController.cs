using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class WorkFlowInstanceHistoriesController : GenericController<WorkFlowInstanceHistory>
    {
        public WorkFlowInstanceHistoriesController(IMediator mediator) : base(mediator) { }
    }
}
