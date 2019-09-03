using AutoMapper;
using System;

namespace Web.Tests.Mappings
{
    public class MapFixture<TProfile> : IDisposable
        where TProfile : Profile, new()
    {
        public MapFixture()
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new TProfile()));
        }

        public IMapper GetMapper()
        {
            return Mapper.Instance;
        }

        public void Dispose()
        {
        }
    }
}
