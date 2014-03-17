using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Runtime.Serialization;

namespace StarShips.Locations
{
    [Serializable]
    public class Location:ISerializable
    {
        public string Name;
        public bool IsBlocked = false;
        List<Ship> _ships = new List<Ship>();
        public List<Ship> Ships
        {
            get { return _ships; }
            set { _ships = value; }
        }
        List<Eidos> _stellars = new List<Eidos>();

        public List<Eidos> Stellars
        {
            get { return _stellars; }
            set { _stellars = value; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("IsBlocked", IsBlocked);
            info.AddValue("Ships", Ships);
        }

        public Location()
        {

        }
        public Location(SerializationInfo info, StreamingContext context)
        {
            Name = (string)info.GetValue("Name", typeof(string));
            IsBlocked = (bool)info.GetValue("IsBlocked", typeof(bool));
            Ships = (List<Ship>)info.GetValue("Ships", typeof(List<Ship>));
        }
    }
}
