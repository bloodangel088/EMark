using AutoMapper;
using EMark.Api.Models.Enums;
using EMark.Api.Models.Requests;
using EMark.Api.Models.Responses;
using EMark.DataAccess.Entities;
using EMark.DataAccess.Entities.Enums;

namespace EMark.Application.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<Role, RoleModel>().ReverseMap();

            CreateMap<User, UserModel>();

            CreateMap<UserRegisterModel, Teacher>()
                .ForMember(teacher => teacher.Role, rule => rule.MapFrom(_ => Role.Teacher));
            CreateMap<UserRegisterModel, Student>()
                .ForMember(student => student.Role, rule => rule.MapFrom(_ => Role.Student));
            CreateMap<UserRegisterModel, User>()
                .ForMember(user => user.PasswordHash, rule => rule.MapFrom(dto => BCrypt.Net.BCrypt.HashPassword(dto.Password)))
                .IncludeAllDerived();
            CreateMap<User, UserUpdateModel>().ReverseMap();
        }
    }
}