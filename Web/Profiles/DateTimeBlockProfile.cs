using AutoMapper;
using DomainObjects.Blocks;
using DomainObjects.Blocks.Details;
using Web.Models.Blocks;

namespace Web.Profiles
{
    public class DateTimeBlockProfile : Profile
    {
        public DateTimeBlockProfile()
        {
            CreateMap<DateTimeBlock, DateTimeBlockDto>()
                .ForMember(b => b.Type, opt => opt.MapFrom(b => BlockType.Datetime))
                .ForMember(b => b.BackColor, opt => opt.MapFrom(b => b.Details.BackColor))
                .ForMember(b => b.TextColor, opt => opt.MapFrom(b => b.Details.TextColor))
                .ForMember(b => b.Font, opt => opt.MapFrom(b => b.Details.FontName))
                .ForMember(b => b.FontSize, opt => opt.MapFrom(b => b.Details.FontSize))
                .ForMember(b => b.FontIndex, opt => opt.MapFrom(b => b.Details.FontIndex))
                .ForMember(b => b.Format, opt => opt.MapFrom(b => b.Details.Format))
                .ForMember(b => b.Align, opt => opt.MapFrom(b => b.Details.Align))
                .ForMember(b => b.Italic, opt => opt.MapFrom(b => b.Details.Italic))
                .ForMember(b => b.Bold, opt => opt.MapFrom(b => b.Details.Bold))

                .ForMember(b => b.Caption, opt => opt.MapFrom(b => string.IsNullOrEmpty(b.Caption) ? "datetime" : b.Caption));

            CreateMap<DateTimeBlockDto, DateTimeBlock>()
                .ForMember(b => b.Details, opt => opt.MapFrom(b => new DateTimeBlockDetails
                {
                    BackColor = b.BackColor,
                    TextColor = b.TextColor,
                    FontName = b.Font,
                    FontSize = b.FontSize,
                    FontIndex = b.FontIndex,
                    Align = b.Align,
                    Italic = b.Italic,
                    Bold = b.Bold,
                    Format = b.Format
                }));
        }
    }
}