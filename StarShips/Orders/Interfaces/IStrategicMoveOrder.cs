using StarShips.Orders.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Orders.Interfaces
{
    public interface IStrategicMoveOrder
    {
        event OrderDelegates.StrategicShipMoveEvent OnShipMove;
        List<string> ExecuteOrder(Ship ship);
    }
}
