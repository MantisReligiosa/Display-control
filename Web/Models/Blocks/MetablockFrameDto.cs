using System;
using System.Collections.Generic;

namespace Web.Models.Blocks
{
    public class MetablockFrameDto
    {
        public Guid Id { get; set; }
        public List<BlockDto> Blocks { get; set; }
        public int Index { get; set; }
        public int Duration { get; set; }
        public bool UseInTimeInterval { get; set; }
        public DateTime? UseFromTime { get; set; }
        public DateTime? UseToTime { get; set; }
        public bool UseInDayOfWeek { get; set; }
        /// <summary>
        /// Пн
        /// </summary>
        public bool UseInMon { get; set; }
        /// <summary>
        /// Вт
        /// </summary>
        public bool UseInTue { get; set; }
        /// <summary>
        /// Ср
        /// </summary>
        public bool UseInWed { get; set; }
        /// <summary>
        /// Чт
        /// </summary>
        public bool UseInThu { get; set; }
        /// <summary>
        /// Пт
        /// </summary>
        public bool UseInFri { get; set; }
        /// <summary>
        /// Сб
        /// </summary>
        public bool UseInSat { get; set; }
        /// <summary>
        /// Вс
        /// </summary>
        public bool UseInSun { get; set; }
        public bool UseInDate { get; set; }
        public DateTime? DateToUse { get; set; }
    }
}