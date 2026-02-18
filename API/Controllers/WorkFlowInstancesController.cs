using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class WorkFlowInstancesController : GenericController<WorkFlowInstance>
    {
        public WorkFlowInstancesController(IMediator mediator) : base(mediator) { }
    }
}
