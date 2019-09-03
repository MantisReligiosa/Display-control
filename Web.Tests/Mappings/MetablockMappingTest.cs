using AutoFixture;
using DomainObjects.Blocks;
using System.Linq;
using Web.Models.Blocks;
using Web.Profiles;
using Xunit;

namespace Web.Tests.Mappings
{
    public class MetablockMappingTest : IClassFixture<MapFixture<MetaBlockProfile>>
    {
        private readonly MapFixture<MetaBlockProfile> _mapFixture;
        private readonly Fixture _fixture;

        public MetablockMappingTest(MapFixture<MetaBlockProfile> mapFixture)
        {
            _mapFixture = mapFixture;
            _fixture = new Fixture();
        }

        [Fact]
        public void TestMapp()
        {
            var metablockDto = _fixture.Create<MetaBlockDto>();
            var metablock = _mapFixture.GetMapper().Map<MetaBlock>(metablockDto);
            metablock.Details.Frames.ToList().ForEach(frame =>
            {
                var frameDto = metablockDto.Frames.FirstOrDefault(f => f.Id == frame.Id);
                Assert.NotNull(frameDto);
                Assert.Equal(frame.DateToUse, frameDto.DateToUse);
            });
        }
    }
}
