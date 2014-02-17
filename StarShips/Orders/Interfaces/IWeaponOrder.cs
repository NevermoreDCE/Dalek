using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Orders.Interfaces
{
    public interface IWeaponOrder
    {
        bool IsInRange(Ship ship);
    }
}
