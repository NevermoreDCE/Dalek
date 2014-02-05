using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Locations
{
    class Location
    {
        public string Name;
        public string Search;
        public bool IsBlocked = false;
        public List<Ship> Ships = new List<Ship>();
    }
}
