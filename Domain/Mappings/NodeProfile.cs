using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace Domain.Mappings
{
    public class NodeProfile : Profile
    {
        public NodeProfile()
        {
            CreateMap<Node, NodeDto>();
        }
    }
}
