﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ELib.Models
{
    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public List<Word> words { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public List<Line> lines { get; set; }
    }

    public class ImageInfoViewModel
    {
        public string language { get; set; }
        public string orientation { get; set; }
        public int textAngle { get; set; }
        public List<Region> regions { get; set; }
    }
}
