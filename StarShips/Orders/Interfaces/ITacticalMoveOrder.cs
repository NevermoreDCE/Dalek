using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Orders.Delegates;

namespace StarShips.Orders.Interfaces
{
    public interface ITacticalMoveOrder
    {
        event OrderDelegates.TacticalShipMoveEvent OnShipMove;
        List<string> ExecuteOrder(Ship ship, int impulse);
    }
}
