using AutoMapper;
using CardService.Application.Common.Models.Requests;
using CardService.Application.Common.Models.Responses;
using CardService.Application.UseCases.Card.Commands;
using CardService.Application.UseCases.Card.Queries;
using CardService.Domain.Entities;

namespace CardService.Application.Common.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCardCommand, CardEntity>();
            CreateMap<CardEntity, CardResponseModel>()
                .ForMember(dest => dest.CardId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<PagedList<CardEntity>, PagedList<CardResponseModel>>();

            CreateMap<CardFilter, GetCardsQuery>();
        }
    }
}
