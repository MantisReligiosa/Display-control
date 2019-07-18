﻿using DomainObjects.Blocks.Details;
using Helpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace BusinessLogickTests
{
    public class HelpersTest
    {
        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 1)]
        public void TestIndexes(int currentIndex, int expectedNextIndex)
        {
            var helper = new MetablockScheduler
            {
                Frames = new List<MetablockFrame>
                {
                    new MetablockFrame{Index = 1},
                    new MetablockFrame{Index = 2},
                    new MetablockFrame{Index = 3}
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
                    new MetablockFrame{Index = 1, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 10, 0, 0), UseToTime = new DateTime(1, 1, 1, 12, 0, 0)},
                    new MetablockFrame{Index = 2, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 10, 0, 0), UseToTime = new DateTime(1, 1, 1, 12, 0, 0)},
                    new MetablockFrame{Index = 3, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 19, 0, 0), UseToTime = new DateTime(1, 1, 1, 23, 0, 0)},
                    new MetablockFrame{Index = 4, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 10, 0, 0), UseToTime = new DateTime(1, 1, 1, 12, 0, 0)}
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
                    new MetablockFrame{Index = 1, UseInDate = true, DateToUse = new DateTime(1, 1, 3, 10, 0, 0)},
                    new MetablockFrame{Index = 2, UseInDate = true, DateToUse = new DateTime(1, 1, 3, 1, 0, 0)},
                    new MetablockFrame{Index = 3, UseInDate = true, DateToUse = new DateTime(1, 1, 4, 19, 0, 0)},
                    new MetablockFrame{Index = 4, UseInDate = true, DateToUse = new DateTime(1, 1, 3, 0, 0, 0)}
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
                    new MetablockFrame{Index = 1, UseInDayOfWeek = true, UseInWed = true},
                    new MetablockFrame{Index = 2, UseInDayOfWeek = true, UseInWed = true},
                    new MetablockFrame{Index = 3, UseInDayOfWeek = true, },
                    new MetablockFrame{Index = 4, UseInDayOfWeek = true, UseInWed = true}
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
                    new MetablockFrame{Index = 1, UseInDayOfWeek = true, UseInSun = true, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 8, 0, 0), UseToTime = new DateTime(1, 1, 1, 12, 0, 0)},
                    new MetablockFrame{Index = 2, UseInDayOfWeek = true, UseInWed = true, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 8, 0, 0), UseToTime = new DateTime(1, 1, 1, 12, 0, 0)},
                    new MetablockFrame{Index = 3, UseInDayOfWeek = true, UseInSun = true, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 8, 0, 0), UseToTime = new DateTime(1, 1, 1, 12, 0, 0)},
                    new MetablockFrame{Index = 4, UseInDayOfWeek = true, UseInSun = true, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 18, 0, 0), UseToTime = new DateTime(1, 1, 1, 22, 0, 0)},
                    new MetablockFrame{Index = 5, UseInDayOfWeek = true, UseInSun = true, UseInTimeInerval = true, UseFromTime = new DateTime(1, 1, 1, 8, 0, 0), UseToTime = new DateTime(1, 1, 1, 12, 0, 0)},
                }
            };
            var nextFrame = helper.GetNextFrame(new DateTime(1, 1, 7, 10, 0, 0), currentIndex);
            Assert.Equal(expectedNextIndex, nextFrame.Index);
        }
    }
}
