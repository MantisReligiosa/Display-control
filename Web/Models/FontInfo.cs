﻿using System.Collections.Generic;

namespace Web.Models
{
    public class FontInfo
    {
        public IEnumerable<string> Fonts { get; set; }
        public IEnumerable<int> FonSizes { get; set; }
    }
}