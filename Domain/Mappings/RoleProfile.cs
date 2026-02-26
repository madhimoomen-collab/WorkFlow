using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace Domain.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>();
        }
    }
}
