using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficWpf
{
    class Unit
    {
        public Road[] route { get; set; }

        public Unit (Road[] _route)
        {
            route = _route;
        }
    }
}
