using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperbolicTessellation
{
    public class Edge
    {
        public int v0 { get; set; }
        public int v1 { get; set; }
        public List<int> Neighbors { get; set; }

        public Edge(int v0, int v1)
        {
            this.v0 = v0;
            this.v1 = v1;
            Neighbors = new List<int>();
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge)
            {
                var e = obj as Edge;
                return e.v0 == v0 && e.v1 == v1 || e.v0 == v1 && e.v1 == v0;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Math.Max(v0, v1) * 65535 + Math.Min(v0, v1);
        }
    }
}
