using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Delegates
{
    public static class ShipDelegates
    {
        public delegate void ShipDestroyedEvent(object sender, EventArgs e, Ship destroyedShip);
    }
}
