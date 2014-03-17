using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace StarShips.Orders.Delegates
{
    public static class OrderDelegates
    {
        public delegate void TacticalShipMoveEvent(object sender, EventArgs e, System.Windows.Controls.Image shipImage, Point sourceLoc, Point targetLoc, bool weaponsFiredAlready);
        public delegate void TacticalWeaponFiredEvent(object sender, EventArgs e, Point sourceLoc, Point targetLoc, string firingType);
    }
}
