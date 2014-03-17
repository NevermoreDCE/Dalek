using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using StarShips.Locations;
using StarShips.Players;
using StarShips.StarSystems;

namespace StarShips
{
    [Serializable]
    public class Game:ISerializable
    {
        public LocationCollection CombatLocations;
        public StarSystemCollection StarSystems = new StarSystemCollection();
        public PlayerCollection Players;
        public List<ShipHull> ExistingHulls = new List<ShipHull>();
        public List<EidosPart> ExistingParts = new List<EidosPart>();
        public List<Ship> ExistingShips = new List<Ship>();

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CombatLocations", CombatLocations);
            info.AddValue("StarSystems", StarSystems);
            info.AddValue("Players", Players);
            info.AddValue("ExistingHulls", ExistingHulls);
            info.AddValue("ExistingParts", ExistingParts);
            info.AddValue("ExistingShips", ExistingShips);
        }

        
        public Game()
        {
            /* Empty Constructor */
        }

        public Game(SerializationInfo info, StreamingContext context)
        {
            CombatLocations = (LocationCollection)info.GetValue("CombatLocations", typeof(LocationCollection));
            StarSystems = (StarSystemCollection)info.GetValue("StarSystems", typeof(StarSystemCollection));
            Players = (PlayerCollection)info.GetValue("Players", typeof(PlayerCollection));
            ExistingHulls = (List<ShipHull>)info.GetValue("ExistingHulls", typeof(List<ShipHull>));
            ExistingParts = (List<EidosPart>)info.GetValue("ExistingParts", typeof(List<EidosPart>));
            ExistingShips = (List<Ship>)info.GetValue("ExistingShips", typeof(List<Ship>));
        }
    }
}
