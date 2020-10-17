using System.Linq;
using AutoMapper;
using Data;
using Web.DTO;

namespace Web.Mapping
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Lot, LotPreview>()
                .ForMember(dest => dest.Funded,
                    opt => opt.MapFrom(src => src.Rates.OrderByDescending(c => c.CreatedAt).FirstOrDefault().Amount));
        }
    }
}