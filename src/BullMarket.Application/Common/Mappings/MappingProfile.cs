using AutoMapper;
using BullMarket.Application.DTOs.Responses;
using BullMarket.Domain.Entities;

namespace BullMarket.Application.Common.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Stock, StockResponse>().ReverseMap();
        }
    }
}
