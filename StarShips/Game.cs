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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CombatLocations", CombatLocations);
            info.AddValue("Players", Players);
        }

        public Game()
        {
        }

        public Game(SerializationInfo info, StreamingContext context)
        {
            CombatLocations = (LocationCollection)info.GetValue("CombatLocations", typeof(LocationCollection));
            Players = (PlayerCollection)info.GetValue("Players", typeof(PlayerCollection));
        }
    }

    [Serializable]
    public class GameTest : ISerializable
    {
        //public LocationCollection CombatLocations;
        public PlayerCollection Players;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("CombatLocations", CombatLocations);
            info.AddValue("Players", Players);
        }

        public GameTest()
        {
        }

        public GameTest(SerializationInfo info, StreamingContext context)
        {
            //CombatLocations = (LocationCollection)info.GetValue("CombatLocations", typeof(LocationCollection));
            Players = (PlayerCollection)info.GetValue("Players", typeof(PlayerCollection));
        }
    }
}
