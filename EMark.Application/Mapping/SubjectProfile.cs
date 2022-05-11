using AutoMapper;
using EMark.Api.Models.Responses;
using EMark.DataAccess.Entities;

namespace EMark.Application.Mapping
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<SubjectModel, Subject>().ReverseMap();
        }
    }
}
