using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace Domain.Mappings
{
    public class WorkFlowInstanceHistoryProfile : Profile
    {
        public WorkFlowInstanceHistoryProfile()
        {
            CreateMap<WorkFlowInstanceHistory, WorkFlowInstanceHistoryDto>();
        }
    }
}
