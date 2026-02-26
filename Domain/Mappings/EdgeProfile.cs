using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace Domain.Mappings
{
    public class EdgeProfile : Profile
    {
        public EdgeProfile()
        {
            CreateMap<Edge, EdgeDto>();
        }
    }
}
