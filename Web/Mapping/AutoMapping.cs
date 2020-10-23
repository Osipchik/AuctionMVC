using System.Linq;
using AutoMapper;
using Data;
using Web.DTO;
using Web.DTO.Comment;
using Web.DTO.Lot;

namespace Web.Mapping
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Lot, LotPreview>()
                .ForMember(dest => dest.Funded,
                    opt => opt.MapFrom(src => src.Rates.OrderByDescending(c => c.CreatedAt).FirstOrDefault().Amount));

            CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.AppUser.UserName));
        }
    }
}