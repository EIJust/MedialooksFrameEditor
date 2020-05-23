using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MedialooksFrameEditor.Services
{
    public class CurveLine
    {
        public CurveLine()
        {
            PenSize = 1.0f;
            PenColor = Color.Black;
            PenPath = new List<Point>();
        }

        public float PenSize { get; set; }
        public Color PenColor { get; set; }
        public List<Point> PenPath { get; set; }
    }
}
