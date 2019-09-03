using DomainObjects.Blocks.Details;
using Helpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace BusinessLogicTests
{
    public class HelpersTest
    {
        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 1)]
        public void TestNoConditions(int currentIndex, int expectedNextIndex)
        {
            var helper = new MetablockScheduler
            {
                Frames = new List<MetablockFrame>
                {
                    new MetablockFrame{Index = 1, Duration = 1},
                    new MetablockFrame{Index = 2, Duration = 1},
                    new MetablockFrame{Index = 3, Duration = 1}
                }
            };
            var nextFrame = helper.GetNextFrame(new DateTime(), currentIndex);
            Assert.Equal(expectedNextIndex, nextFrame.Index);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        [InlineData(3, 4)]
        [InlineData(4, 1)]
        public void TestTimeInterval(int currentIndex, int expectedNextIndex)
        {
            var helper = new MetablockScheduler
            {
                Frames = new List<MetablockFrame>
                {
                    new MetablockFrame{Index = 1, UseInTimeInterval = true, UseFromTime = new TimeSpan(10,0,0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 2, UseInTimeInterval = true, UseFromTime = new TimeSpan(10, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 3, UseInTimeInterval = true, UseFromTime = new TimeSpan(19, 0, 0), UseToTime = new TimeSpan(23, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 4, UseInTimeInterval = true, UseFromTime = new TimeSpan(10, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1}
                }
            };
            var nextFrame = helper.GetNextFrame(new DateTime(1, 1, 1, 11, 0, 0), currentIndex);
            Assert.Equal(expectedNextIndex, nextFrame.Index);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        [InlineData(3, 4)]
        [InlineData(4, 1)]
        public void TestDate(int currentIndex, int expectedNextIndex)
        {
            var helper = new MetablockScheduler
            {
                Frames = new List<MetablockFrame>
                {
                    new MetablockFrame{Index = 1, UseInDate = true, DateToUse = new DateTime(1, 1, 3, 10, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 2, UseInDate = true, DateToUse = new DateTime(1, 1, 3, 1, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 3, UseInDate = true, DateToUse = new DateTime(1, 1, 4, 19, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 4, UseInDate = true, DateToUse = new DateTime(1, 1, 3, 0, 0, 0), Duration = 1}
                }
            };
            var nextFrame = helper.GetNextFrame(new DateTime(1, 1, 3, 11, 0, 0), currentIndex);
            Assert.Equal(expectedNextIndex, nextFrame.Index);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        [InlineData(3, 4)]
        [InlineData(4, 1)]
        public void TestDayOfWeek(int currentIndex, int expectedNextIndex)
        {
            var helper = new MetablockScheduler
            {
                Frames = new List<MetablockFrame>
                {
                    new MetablockFrame{Index = 1, UseInDayOfWeek = true, UseInWed = true, Duration = 1},
                    new MetablockFrame{Index = 2, UseInDayOfWeek = true, UseInWed = true, Duration = 1},
                    new MetablockFrame{Index = 3, UseInDayOfWeek = true, Duration = 1 },
                    new MetablockFrame{Index = 4, UseInDayOfWeek = true, UseInWed = true, Duration = 1}
                }
            };
            var nextFrame = helper.GetNextFrame(new DateTime(1, 1, 3, 11, 0, 0), currentIndex);
            Assert.Equal(expectedNextIndex, nextFrame.Index);
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 3)]
        [InlineData(3, 5)]
        [InlineData(4, 5)]
        [InlineData(5, 1)]
        public void TestSundayMorning(int currentIndex, int expectedNextIndex)
        {
            var helper = new MetablockScheduler
            {
                Frames = new List<MetablockFrame>
                {
                    new MetablockFrame{Index = 1, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(8, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 2, UseInDayOfWeek = true, UseInWed = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(8, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 3, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(8, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 4, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(18, 0, 0), UseToTime = new TimeSpan(22, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 5, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(8, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                }
            };
            var nextFrame = helper.GetNextFrame(new DateTime(1, 1, 7, 10, 0, 0), currentIndex);
            Assert.Equal(expectedNextIndex, nextFrame.Index);
        }

        [Fact]
        public void TestNull()
        {
            var helper = new MetablockScheduler
            {
                Frames = new List<MetablockFrame>
                {
                    new MetablockFrame{Index = 1, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(8, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 2, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(8, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 3, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(8, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 4, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(18, 0, 0), UseToTime = new TimeSpan(22, 0, 0), Duration = 1},
                    new MetablockFrame{Index = 5, UseInDayOfWeek = true, UseInSun = true, UseInTimeInterval = true, UseFromTime = new TimeSpan(8, 0, 0), UseToTime = new TimeSpan(12, 0, 0), Duration = 1},
                }
            };
            var nextFrame = helper.GetNextFrame(new DateTime(1, 1, 1, 10, 0, 0), 1);
            Assert.Null(nextFrame);
        }

        [Fact]
        public void TestGoToFirst()
        {
            var date = new DateTime(2020, 1, 1);
            var helper = new MetablockScheduler
            {
                Frames = new List<MetablockFrame>
                {
                    new MetablockFrame{Index = 1, UseInDate = true, DateToUse = date, Duration = 1},
                    new MetablockFrame{Index = 2, Duration = 1 },
                    new MetablockFrame{Index = 3, UseInDate = true, DateToUse = date, Duration = 1},
                    new MetablockFrame{Index = 4, Duration = 1 },
                    new MetablockFrame{Index = 5, UseInDate = true, DateToUse = date, Duration = 1}
                }
            };

            var nextFrame = helper.GetNextFrame(date, 5);
            Assert.Equal(1, nextFrame.Index);
        }

        [Fact]
        public void TestInnerException()
        {
            var innedExceptionName = "123";
            var innedException = new Exception(innedExceptionName);
            var exception = new Exception("1", new Exception("2", new Exception("3", innedException)));
            Assert.Equal(innedExceptionName, exception.GetInnerException().Message);
        }
    }
}
