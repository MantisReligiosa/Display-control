using AutoMapper;
using DomainObjects.Blocks;
using DomainObjects.Blocks.Details;
using Web.Models.Blocks;

namespace Web.Profiles
{
    public class TextBlockProfile : Profile
    {
        public TextBlockProfile()
        {
            CreateMap<TextBlock, TextBlockDto>()
                .ForMember(b => b.Type, opt => opt.MapFrom(b => BlockType.Text))
                .ForMember(b => b.Text, opt => opt.MapFrom(b => b.Details.Text))
                .ForMember(b => b.BackColor, opt => opt.MapFrom(b => b.Details.BackColor))
                .ForMember(b => b.TextColor, opt => opt.MapFrom(b => b.Details.TextColor))
                .ForMember(b => b.Font, opt => opt.MapFrom(b => b.Details.FontName))
                .ForMember(b => b.FontSize, opt => opt.MapFrom(b => b.Details.FontSize))
                .ForMember(b => b.FontIndex, opt => opt.MapFrom(b => b.Details.FontIndex))
                .ForMember(b => b.Align, opt => opt.MapFrom(b => b.Details.Align))
                .ForMember(b => b.Italic, opt => opt.MapFrom(b => b.Details.Italic))
                .ForMember(b => b.Bold, opt => opt.MapFrom(b => b.Details.Bold))
                .ForMember(b => b.Caption, opt => opt.MapFrom(b => string.IsNullOrEmpty(b.Caption) ? "text" : b.Caption));

            CreateMap<TextBlockDto, TextBlock>()
                .ForMember(b => b.Details, opt => opt.MapFrom(b => new TextBlockDetails
                {
                    Text = b.Text,
                    BackColor = b.BackColor,
                    TextColor = b.TextColor,
                    FontName = b.Font,
                    FontSize = b.FontSize,
                    FontIndex = b.FontIndex,
                    Align = b.Align,
                    Italic = b.Italic,
                    Bold = b.Bold
                }));
        }
    }
}