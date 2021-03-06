﻿using AutoMapper;
using DomainObjects.Blocks;
using DomainObjects.Blocks.Details;
using Web.Models.Blocks;

namespace Web.Profiles
{
    public class PictureBlockProfile : Profile
    {
        public PictureBlockProfile()
        {
            CreateMap<PictureBlock, PictureBlockDto>()
                .ForMember(b => b.Type, opt => opt.MapFrom(b => "picture"))
                .ForMember(b => b.Base64Src, opt => opt.MapFrom(b => b.Details.Base64Image));

            CreateMap<PictureBlockDto, PictureBlock>()
                .ForMember(b => b.Details, opt => opt.MapFrom(b => new PictureBlockDetails
                {
                    Base64Image = b.Base64Src
                }));
        }
    }
}