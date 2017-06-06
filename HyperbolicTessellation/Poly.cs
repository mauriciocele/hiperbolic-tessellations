using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperbolicTessellation
{
    public class Poly : IComparable<Poly>
    {
        public int level { get; set; }
        public int center { get; set; }
        public List<Edge> edges { get; set; }

        public Poly()
        {
            level = -1;
            center = -1;
            edges = new List<Edge>();
        }

        public int CompareTo(Poly other)
        {
            return center - other.center;
        }
    }
}
