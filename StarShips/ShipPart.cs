using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips
{
    [Serializable]
    public abstract class ShipPart
    {
        public string Name = "No Name";
        public bool IsDestroyed = false;
        
        public abstract override string ToString();
        public abstract string Repair(int amount);
    }
}
