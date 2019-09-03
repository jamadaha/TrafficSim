using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TrafficWpf
{
    public class Source
    {
        public Ellipse ellipse { get; set; }
        public List<Road> connectedRoads = new List<Road>();
        public Source(Ellipse _ellipse)
        {
            ellipse = _ellipse;
        }

        public void SpawnUnit()
        {
            //Unit newUnit = new Unit();
        }

        public void FindRoute()
        {
            foreach (Road road in connectedRoads)
            {
                road.line.Fill = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
