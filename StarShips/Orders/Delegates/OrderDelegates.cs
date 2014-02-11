using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace StarShips.Orders.Delegates
{
    public static class OrderDelegates
    {
        public delegate void ShipMoveEvent(object sender, OrderEventArgs e, Ship shipToMove, bool endAtLocation);
        public delegate void WeaponFiredEvent(object sender, EventArgs e, Point sourceLoc, Point targetLoc, string firingType);
    }
}
