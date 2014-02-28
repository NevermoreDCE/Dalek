using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Interfaces
{
    public interface IShipPartAction
    {
        int[] ActionValues {get;set;}
        string DoAction(ShipPart caller);
        string ToString();
    }
}
