using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace StarShips.Locations
{
    public class Location
    {
        public string Name;
        public string Search;
        public bool IsBlocked = false;
        public List<Ship> Ships = new List<Ship>();
    }
}
