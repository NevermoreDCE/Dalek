using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Orders.Delegates;

namespace StarShips.Orders.Interfaces
{
    public interface IWeaponOrder
    {
        event OrderDelegates.WeaponFiredEvent OnWeaponFired;
        bool IsInRange(Ship ship);
        List<string> ExecuteOrder(Ship ship);
    }
}
