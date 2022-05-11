using AutoMapper;
using EMark.Api.Models.Responses;
using EMark.DataAccess.Entities;

namespace EMark.Application.Mapping
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<GroupModel, Group>().ReverseMap();
        }
    }
}
