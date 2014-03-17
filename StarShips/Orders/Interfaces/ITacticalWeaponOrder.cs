using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Orders.Delegates;

namespace StarShips.Orders.Interfaces
{
    public interface ITacticalWeaponOrder
    {
        event OrderDelegates.TacticalWeaponFiredEvent OnWeaponFired;
        bool IsInRange(Ship ship);
        List<string> ExecuteOrder(Ship ship);
    }
}
