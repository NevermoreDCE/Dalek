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
        public List<Ship> Ships = new List<Ship>();

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
