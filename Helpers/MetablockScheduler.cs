using DomainObjects.Blocks.Details;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class MetablockScheduler
    {
        public List<MetablockFrame> Frames { get; set; }

        public MetablockFrame GetNextFrame(DateTime dateTime, int currentFrameIndex)
        {
            var dayOfWeek = dateTime.DayOfWeek;
            var frames = Frames
                .Where(f => f.UseInTimeInterval ? f.UseFromTime.HasValue 
                            && f.UseToTime.HasValue 
                            && (f.UseFromTime.Value.TimeOfDay <= dateTime.TimeOfDay) 
                            && (dateTime.TimeOfDay <= f.UseToTime.Value.TimeOfDay) : true)
                .Where(f => f.UseInDate ? f.DateToUse.HasValue 
                            && f.DateToUse.Value.Date.Equals(dateTime.Date) : true)
                .Where(f => f.UseInDayOfWeek ? (f.UseInMon && dayOfWeek.Equals(DayOfWeek.Monday))
                            || (f.UseInTue && dayOfWeek.Equals(DayOfWeek.Tuesday))
                            || (f.UseInWed && dayOfWeek.Equals(DayOfWeek.Wednesday))
                            || (f.UseInThu && dayOfWeek.Equals(DayOfWeek.Thursday))
                            || (f.UseInFri && dayOfWeek.Equals(DayOfWeek.Friday))
                            || (f.UseInSat && dayOfWeek.Equals(DayOfWeek.Saturday))
                            || (f.UseInSun && dayOfWeek.Equals(DayOfWeek.Sunday)) : true);
            var frameToShow = frames.FirstOrDefault(f => f.Index > currentFrameIndex);
            if (frameToShow == null)
            {
                frameToShow = frames.FirstOrDefault();
            }
            return frameToShow;
        }
    }
}
