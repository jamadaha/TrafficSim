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

        List<Road[]> paths = new List<Road[]>();
        public Road[] FindRoute()
        {
            paths = new List<Road[]>();
            foreach(Road road in connectedRoads)
            {
                SearchConnectedRoads(road, new List<Road>());
            }
            if (paths.Count > 0)
            {
                int minLength = paths.Min(y => y.Length);
                Road[] shortestPath = paths.First(x => x.Length == minLength);
                return shortestPath;
            }
            else return null;
        }

        void SearchConnectedRoads(Road _node,List<Road> _traversedRoads)
        {
            List<Road> traversedRoads = new List<Road>();
            foreach (Road road in _traversedRoads) traversedRoads.Add(road);
            traversedRoads.Add(_node);

            if (_node.connectedDestination != null)
            {
                paths.Add(traversedRoads.ToArray());
                return;
            }

            foreach (Road road in _node.connectedRoads)
            {
                if (!traversedRoads.Contains(road))
                {
                    SearchConnectedRoads(road, traversedRoads);
                }
            }
        }
    }
}
