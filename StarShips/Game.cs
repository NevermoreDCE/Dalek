using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using StarShips.Locations;
using StarShips.Players;

namespace StarShips
{
    [Serializable]
    public class Game:ISerializable
    {
        public LocationCollection CombatLocations;
        public PlayerCollection Players;
        public List<ShipHull> ExistingHulls = new List<ShipHull>();
        public List<ShipPart> ExistingParts = new List<ShipPart>();
        public List<Ship> ExistingShips = new List<Ship>();

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CombatLocations", CombatLocations);
            info.AddValue("Players", Players);
            info.AddValue("ExistingHulls", ExistingHulls);
            info.AddValue("ExistingParts", ExistingParts);
            info.AddValue("ExistingShips", ExistingShips);
        }

        public Game()
        {
        }

        public Game(SerializationInfo info, StreamingContext context)
        {
            CombatLocations = (LocationCollection)info.GetValue("CombatLocations", typeof(LocationCollection));
            Players = (PlayerCollection)info.GetValue("Players", typeof(PlayerCollection));
            ExistingHulls = (List<ShipHull>)info.GetValue("ExistingHulls", typeof(List<ShipHull>));
            ExistingParts = (List<ShipPart>)info.GetValue("ExistingParts", typeof(List<ShipPart>));
            ExistingShips = (List<Ship>)info.GetValue("ExistingShips", typeof(List<Ship>));
        }
    }
}
