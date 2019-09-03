using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace TrafficWpf
{
    public class Destination
    {
        public Rectangle rectangle { get; set; }
        public List<Road> connectedRoads = new List<Road>();
        public Destination(Rectangle _rectangle)
        {
            rectangle = _rectangle;
        }
    }
}
