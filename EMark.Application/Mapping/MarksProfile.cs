using AutoMapper;
using EMark.Api.Models.Responses;
using EMark.DataAccess.Entities;

namespace EMark.Application.Mapping
{
    public class MarksProfile : Profile
    {
        public MarksProfile()
        {
            CreateMap<MarkColumnModel, MarkColumn>();
            CreateMap<MarkModel, Mark>();
        }
    }
}
