using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace Domain.Mappings
{
    public class WorkFlowInstanceProfile : Profile
    {
        public WorkFlowInstanceProfile()
        {
            CreateMap<WorkFlowInstance, WorkFlowInstanceDto>();
        }
    }
}
