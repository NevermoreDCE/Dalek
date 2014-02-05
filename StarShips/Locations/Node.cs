using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace StarShips.Locations
{
    class Node : IComparable<Node>
    {
        public Point Loc;
        public int F { get { return G + H; } }
        public int G;
        public int H;
        public Node Parent;
        public Node()
        {
        }
        public Node(int x, int y)
        {
            this.Loc = new Point(x, y);
        }

        public int CompareTo(Node other)
        {
            if (this.Loc.X > other.Loc.X || this.Loc.Y > other.Loc.Y) return 1;
            if (this.Loc.X == other.Loc.X && this.Loc.Y == other.Loc.Y) return 0;
            return -1;
        }
        public override string ToString()
        {
            return string.Format("{0},{1}({2},{3},{4})", Loc.X, Loc.Y, this.H, this.G, this.F);
        }
    }
}
