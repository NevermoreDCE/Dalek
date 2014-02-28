using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Orders.Delegates;

namespace StarShips.Orders.Interfaces
{
    public interface IMoveOrder
    {
        event OrderDelegates.ShipMoveEvent OnShipMove;
        List<string> ExecuteOrder(Ship ship, int impulse);
    }
}
