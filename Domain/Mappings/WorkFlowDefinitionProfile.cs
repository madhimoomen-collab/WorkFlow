using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace Domain.Mappings
{
    public class WorkFlowDefinitionProfile : Profile
    {
        public WorkFlowDefinitionProfile()
        {
            CreateMap<WorkFlowDefinition, WorkFlowDefinitionDto>();
            CreateMap<WorkFlowDefinition, WorkFlowDefinitionGraphDto>();
            CreateMap<Node, NodeGraphDto>();
        }
    }
}
