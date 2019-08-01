using AutoMapper;
using DomainObjects.Blocks;
using DomainObjects.Blocks.Details;
using Web.Models.Blocks;

namespace Web.Profiles
{
    public class MetaBlockProfile : Profile
    {
        public MetaBlockProfile()
        {
            CreateMap<MetaBlock, MetaBlockDto>()
                .ForMember(b => b.Type, opt => opt.MapFrom(b =>BlockType.Meta))
                .ForMember(b => b.Caption, opt => opt.MapFrom(b => string.IsNullOrEmpty(b.Caption) ? "meta" : b.Caption))
                .ForMember(b => b.Frames, opt => opt.MapFrom(b => b.Details.Frames));

            CreateMap<MetaBlockDto, MetaBlock>()
                .ForPath(b => b.Details.Frames, opt => opt.MapFrom(b => b.Frames));

            CreateMap<MetablockFrame, MetablockFrameDto>();

            CreateMap<MetablockFrameDto, MetablockFrame>()
                .ForMember(dto => dto.Blocks, opt => opt.Ignore());
        }
    }
}