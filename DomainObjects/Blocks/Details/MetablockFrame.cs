﻿using System.Collections.Generic;

namespace DomainObjects.Blocks.Details
{
    public class MetablockFrame : Identity, ICopyable<MetablockFrame>
    {
        public ICollection<DisplayBlock> Blocks { get; set; }

        public int Index { get; set; }

        public int Duration { get; set; }

        public MetablockFrame() { }

        public MetablockFrame(MetablockFrame source)
        {
            CopyFrom(source);
        }

        public void CopyFrom(MetablockFrame source)
        {
            Index = source.Index;
            Duration = source.Duration;
        }
    }
}
