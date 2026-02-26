using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace Domain.Mappings
{
    public class UserRoleProfile : Profile
    {
        public UserRoleProfile()
        {
            CreateMap<UserRole, UserRoleDto>();
        }
    }
}
