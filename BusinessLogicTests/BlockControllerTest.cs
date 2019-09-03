using AutoFixture;
using AutoFixture.Kernel;
using DomainObjects.Blocks;
using DomainObjects.Blocks.Details;
using ExpectedObjects;
using NSubstitute;
using ServiceInterfaces;
using Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BusinessLogicTests
{
    public class BlockControllerTest
    {
        private readonly ISystemController _systemController;
        private IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DisplayBlock> _displayBlockRepository;
        private readonly Fixture _fixture;

        public BlockControllerTest()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new TypeRelay(typeof(DisplayBlock), typeof(TextBlock)));
            _fixture.Customizations.Add(new TypeRelay(typeof(DisplayBlock), typeof(TableBlock)));
            _fixture.Customizations.Add(new TypeRelay(typeof(DisplayBlock), typeof(PictureBlock)));
            _fixture.Customizations.Add(new TypeRelay(typeof(DisplayBlock), typeof(DateTimeBlock)));

            _systemController = Substitute.For<ISystemController>();

            _displayBlockRepository = Substitute.For<IRepository<DisplayBlock>>();

            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.DisplayBlocks.Returns(_displayBlockRepository);

            _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _unitOfWorkFactory.Create().Returns(_unitOfWork);
        }

        [Fact]
        public void SaveExistMetablockWithExistsFramesTest()
        {
            var controller = new BlockController(_systemController, _unitOfWorkFactory);

            var savingMetablock = _fixture
                .Build<MetaBlock>()
                .Without(m => m.MetablockFrame)
                .Without(m => m.MetablockFrameId)
                .Create();

            var dbMetablock = new MetaBlock
            {
                Id = savingMetablock.Id,
                Details = new MetaBlockDetails
                {
                    Id = savingMetablock.Details.Id,
                    Frames = new List<MetablockFrame>(savingMetablock.Details.Frames.Select(f => new MetablockFrame
                    {
                        Id = f.Id,
                        Blocks = new List<DisplayBlock>()
                    }))
                }
            };

            _displayBlockRepository.Get(Arg.Any<object>()).Returns(dbMetablock);

            var actual = controller.SaveMetabLock(savingMetablock);
            var expected = savingMetablock.ToExpectedObject();
            expected.ShouldEqual(actual);
        }

        [Fact]
        public void SaveExistMetablockWithNoExistsFramesTest()
        {
            var controller = new BlockController(_systemController, _unitOfWorkFactory);

            var savingMetablock = _fixture
                .Build<MetaBlock>()
                .Without(m => m.MetablockFrame)
                .Without(m => m.MetablockFrameId)
                .Create();

            var dbMetablock = new MetaBlock
            {
                Id = savingMetablock.Id,
                Details = new MetaBlockDetails
                {
                    Id = savingMetablock.Details.Id,
                    Frames = new List<MetablockFrame>()
                }
            };

            _displayBlockRepository.Get(Arg.Any<object>()).Returns(dbMetablock);

            var actual = controller.SaveMetabLock(savingMetablock);
            var expected = savingMetablock.ToExpectedObject();
            expected.ShouldEqual(actual);
        }
    }
}
