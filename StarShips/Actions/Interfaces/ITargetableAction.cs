using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Actions.Interfaces
{
    public interface ITargetableAction
    {
        string SetTarget(ShipPart targetPart,Ship targetShip);
    }
}
