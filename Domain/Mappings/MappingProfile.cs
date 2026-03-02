using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace Domain.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Edge, EdgeDto>();
            CreateMap<Node, NodeDto>();
            CreateMap<Role, RoleDto>();
            CreateMap<User, UserDto>();
            CreateMap<UserRole, UserRoleDto>();
            CreateMap<WorkFlowDefinition, WorkFlowDefinitionDto>();
            CreateMap<WorkFlowDefinition, WorkFlowDefinitionGraphDto>();
            CreateMap<Node, NodeGraphDto>();
            CreateMap<WorkFlowInstanceHistory, WorkFlowInstanceHistoryDto>();
            CreateMap<WorkFlowInstance, WorkFlowInstanceDto>();
        }
    }
}
