using StarShips;
using StarShips.Randomizer;
using StarShips.StarSystems;
using StarShips.Stellars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemPathfindingTest
{
    public partial class Form1 : Form
    {
        Game GameState;
        public Form1()
        {
            InitializeComponent();
            //initGame();
            GameState = initNewGame();
            List<StarSystem> sourceStart = new List<StarSystem>();
            foreach (var system in GameState.StarSystems)
                sourceStart.Add(system);
            List<StarSystem> sourceEnd = new List<StarSystem>();
            foreach (var system in GameState.StarSystems)
                sourceEnd.Add(system);
            ddlEnd.DataSource = sourceEnd;
            ddlEnd.DisplayMember = "Name";
            ddlStart.DataSource = sourceStart;
            ddlStart.DisplayMember = "Name";
        }

        private const int StrategicGridDimension = 13;
        private const int GalacticGridDimension = 25000;
        private const int StartingWarpPointsMax = 6;

        Game initNewGame()
        {
            Game result = new Game();
            result.StarSystems = new StarSystemCollection();

            // setup systems
            using (RNG rng = new RNG())
            {
                for (int i = 0; i < 10000; i++)
                {
                    StarSystem newSystem = new StarSystem(StrategicGridDimension, StrategicGridDimension);

                    // system name
                    newSystem.Name = (i + 1).ToString();

                    // galactic location
                    int newX = Convert.ToInt32(rng.d(GalacticGridDimension - 1));
                    int newY = Convert.ToInt32(rng.d(GalacticGridDimension - 1));
                    newSystem.GalacticCoordinates = new System.Drawing.Point(newX, newY);

                    result.StarSystems.Add(newSystem);
                    Debug.WriteLine(string.Format("Building system {0} complete...", newSystem.Name));
                }
            }
            

            // setup 1-N warp points per system
            foreach (StarSystem system in result.StarSystems)
            {
                int rand = 1;
                using (RNG rng = new RNG())
                {
                    rand = rng.d(StartingWarpPointsMax - 1) + 1;
                }
                Debug.WriteLine("Building {0} warp points in system {1}...", rand, system.Name);
                while (system.GetCountOfWarpPoints() < rand)
                {
                    AddWarpPointToSystem(result, system);
                }
            }

            return result;

        }

        private static System.Drawing.Point WarpPointLocation(StarSystem targetSystem)
        {
            int x = -1;
            int y = -1;
            using (StarShips.Randomizer.RNG rng = new StarShips.Randomizer.RNG())
            {
                bool isOccupied = true;
                while (isOccupied)
                {
                    //determine which edge
                    int side = rng.d(4);

                    switch (side)
                    {
                        case 1:
                            x = 0;
                            y = rng.d(12);
                            break;
                        case 2:
                            x = rng.d(12);
                            y = 0;
                            break;
                        case 3:
                            x = 12;
                            y = rng.d(12);
                            break;
                        case 4:
                            x = rng.d(12);
                            y = 12;
                            break;
                    }
                    if (!targetSystem.StrategicLocations[x, y].Stellars.Any(f => f is WarpPoint))
                        isOccupied = false;
                }
            }
            System.Drawing.Point result = new System.Drawing.Point(x, y);
            return result;
        }
        private static void AddWarpPointToSystem(Game result, StarSystem system)
        {
            StarSystem targetSystem = new StarSystem();
            targetSystem = NearestSystemNotConnected(result, system);

            // create points and assign opposite systems
            WarpPoint thisSide = new WarpPoint();
            WarpPoint thatSide = new WarpPoint();

            // This side of the Warp Point
            thisSide.StrategicPosition = WarpPointLocation(system);
            thisSide.Name = targetSystem.Name;
            thisSide.StrategicSystem = system;
            thisSide.LinkedSystem = targetSystem;
            thisSide.LinkedWarpPoint = thatSide;
            system.StrategicLocations[thisSide.StrategicPosition.X, thisSide.StrategicPosition.Y].Stellars.Add(thisSide);

            // Other side of the Warp Point
            thatSide.StrategicPosition = WarpPointLocation(targetSystem);
            thatSide.Name = system.Name;
            thatSide.StrategicSystem = targetSystem;
            thatSide.LinkedSystem = system;
            thatSide.LinkedWarpPoint = thisSide;
            targetSystem.StrategicLocations[thatSide.StrategicPosition.X, thatSide.StrategicPosition.Y].Stellars.Add(thatSide);
        }
        private static double NotReallyDistanceButShouldDo(Point source, Point target)
        {
            return Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2);
        }
        private static StarSystem NearestSystemNotConnected(Game result, StarSystem system)
        {
            
            int topTenPercent = Convert.ToInt32(0.1 * result.StarSystems.Count());
            List<StarSystem> NearbySystems = result.StarSystems.Where(f => f != system).OrderBy(r => NotReallyDistanceButShouldDo(system.GalacticCoordinates, r.GalacticCoordinates)).Take(topTenPercent).ToList<StarSystem>();

            // from top, find next not connected
            for (int i = 0; i < NearbySystems.Count; i++)
            {
                if (!StarSystemsAreConnected(system, NearbySystems[i]) && NearbySystems[i].GetCountOfWarpPoints() < StartingWarpPointsMax)
                    return NearbySystems[i];
            }
            return new StarSystem();
        }

        private static bool StarSystemsAreConnected(StarSystem sourceSystem, StarSystem targetSystem)
        {
            if (sourceSystem.StrategicLocations.Any(
                f => f.Stellars.Any(
                    z => z is WarpPoint
                        && ((WarpPoint)z).LinkedSystem == targetSystem)))
                return true;
            return false;
        }


        private void btnGo_Click(object sender, EventArgs e)
        {
            lblResult.Text = "Path: ";
            StarSystem target = (StarSystem)ddlEnd.SelectedItem;
            StarSystem source = (StarSystem)ddlStart.SelectedItem;
            lblResult.Text += source.Name;
            StarSystem current = source;
            StarSystem next;
            while(current!=target)
            {
                next = GetNextSystemBFS(current, target);
                lblResult.Text += " => " + next.Name;
                current = next;
            }
        }

        public StarSystem GetNextSystemDFS(StarSystem currentSystem, StarSystem targetSystem)
        {
            if (currentSystem == targetSystem)
                return currentSystem;
            int countOfJumps = 99;
            StarSystem bestSystem = currentSystem;
            foreach (var system in currentSystem.GetConnectedStarSystems())
            {
                List<StarSystem> newPath = new List<StarSystem>();
                newPath.Add(system);
                int jumps = getNextSystem(system, targetSystem, newPath);
                string path="Top";
                foreach (var sys in newPath)
                    path += "=>" + sys.Name;
                Debug.WriteLine(string.Format("Path: {0} Jumps: {1}", path, jumps));
                if (jumps < countOfJumps)
                {
                    countOfJumps = jumps;
                    bestSystem = system;
                }
            }
            return bestSystem;
        }
        private int getNextSystem(StarSystem currentSystem, StarSystem targetSystem, List<StarSystem> pathVisited)
        {
            if (currentSystem == targetSystem)
                return 0;
            int countOfJumps = 99;
            StarSystem bestSystem = currentSystem;
            foreach (var system in currentSystem.GetConnectedStarSystems().Where(f => !pathVisited.Contains(f)))
            {
                List<StarSystem> newPath = new List<StarSystem>();
                foreach (StarSystem s in pathVisited)
                    newPath.Add(s);
                newPath.Add(system);
                int jumps = getNextSystem(system, targetSystem, newPath);
                string path = "";
                foreach (var sys in newPath)
                    path += "=>" + sys.Name;
                Debug.WriteLine(string.Format("Path: {0} Jumps: {1}", path, jumps));
                //Debug.WriteLine(string.Format("From: {0} To: {1} Jumps: {2}", system.Name, targetSystem.Name, jumps.Item1));
                if (jumps < countOfJumps)
                {
                    countOfJumps = jumps;
                    bestSystem = system;
                }
            }
            return countOfJumps + 1;
        }

        public StarSystem GetNextSystemBFS(StarSystem startingSystem, StarSystem targetSystem)
        {
            // Players might call for path to current system
            if (startingSystem == targetSystem)
                return startingSystem;

            // Queue needs to store path-thus-far and current system
            Queue<Tuple<List<StarSystem>, StarSystem>> StarSystemQueue = new Queue<Tuple<List<StarSystem>, StarSystem>>();
            
            // Need to track visited systems to prevent infinite loops
            List<StarSystem> visitedSystems = new List<StarSystem>();
            // Starting system is already visited
            visitedSystems.Add(startingSystem);

            // For connected systems that we haven't already visited
            foreach (StarSystem system in startingSystem.GetConnectedStarSystems().Where(f => !visitedSystems.Contains(f)))
            {
                List<StarSystem> pathList = new List<StarSystem>();
                pathList.Add(system);
                // Add to visited systems so it's not evaluated in the loop
                visitedSystems.Add(system);
                // Enqueue the path & system
                StarSystemQueue.Enqueue(new Tuple<List<StarSystem>, StarSystem>(pathList, system));
            }
            // Loop til there's an answer or all paths are exausted
            while(StarSystemQueue.Count>0)
            {
                // Grab current from the queue
                Tuple<List<StarSystem>,StarSystem> currentSystem = StarSystemQueue.Dequeue();
                
                // If current is the target, return the first system from the path
                if (currentSystem.Item2 == targetSystem)
                    return currentSystem.Item1.First();

                // For connected systems that we haven't already visited
                foreach (StarSystem system in currentSystem.Item2.GetConnectedStarSystems().Where(f => !visitedSystems.Contains(f)))
                {
                    // rebuild path list to prevent changing other paths by reference
                    List<StarSystem> pathList = new List<StarSystem>();
                    foreach (var previous in currentSystem.Item1)
                        pathList.Add(previous);
                    pathList.Add(system); // add new system to path
                    visitedSystems.Add(system); // add new system to visited
                    // Enqueue the path & system
                    StarSystemQueue.Enqueue(new Tuple<List<StarSystem>, StarSystem>(pathList, system));
                }
            }
            // No valid answer at this point, return starting system and handle in outer code
            return startingSystem;
        }
    }
}
