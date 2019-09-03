using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace TrafficWpf
{
    public class Road
    {
        public Line line { get; set; }
        public double width { get; set; }
        public int depth { get; set; }

        public List<Road> connectedRoads = new List<Road>();
        public Source connectedSource { get; set; }
        public Destination connectedDestination { get; set; }

        public Road (Line _line, double _width)
        {
            line = _line;
            width = _width;
        }
    }
}
