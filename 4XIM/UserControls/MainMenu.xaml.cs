using StarShips;
using StarShips.Planets;
using StarShips.Randomizer;
using StarShips.StarSystems;
using StarShips.Stellars;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _4XIM.UserControls
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl, ISwitchable
    {
        #region Constants
        private const int StrategicGridDimension = 13;
        private const int GalacticGridDimension = 40;
        private const int StartingWarpPointsMax = 5;
        private const int StartingPlanetsMax = 9;
        private const int StartingAsteroidsMax = 4;
        #endregion
        public MainMenu()
        {
            InitializeComponent();
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            Game newGame = initNewGame();
            Switcher.Switch(new StrategicWindow(newGame));
        }

        Game initNewGame()
        {
            Game result = new Game();
            result.StarSystems = new StarSystemCollection();

            // setup systems
            using (StreamReader reader = File.OpenText("Settings\\StarSystems.txt"))
            {
                using (RNG rng = new RNG())
                {
                    for (int i = 0; i < 20; i++)
                    {
                        StarSystem newSystem = new StarSystem(StrategicGridDimension,StrategicGridDimension);
                        
                        // system name
                        newSystem.Name = reader.ReadLine();
                        
                        // galactic location
                        int newX = Convert.ToInt32(rng.d(GalacticGridDimension - 1));
                        int newY = Convert.ToInt32(rng.d(GalacticGridDimension - 1));
                        newSystem.GalacticCoordinates = new System.Drawing.Point(newX, newY);

                        // Star
                        Star newStar = initStar(newSystem.Name);
                        newSystem.StrategicLocations[StrategicGridDimension / 2, StrategicGridDimension / 2].Stellars.Add(newStar);

                        // Planets
                        int numPlanets = rng.d(StartingPlanetsMax);
                        for (int np = 0; np < numPlanets; np++)
                        {
                            AddPlanetToSystem(newSystem,np+1);
                        }
                        result.StarSystems.Add(newSystem);
                    }
                }
            }

            // setup 1-N warp points per system
            foreach(StarSystem system in result.StarSystems)
            {
                int rand = 1;
                using (RNG rng = new RNG())
                {
                    rand = rng.d(StartingWarpPointsMax-1) + 1;
                }
                while(system.GetCountOfWarpPoints()<rand)
                {
                    AddWarpPointToSystem(result, system);    
                }
            }

            return result;

        }

        private void AddPlanetToSystem(StarSystem newSystem, int planetNumber)
        {
            using(RNG rng = new RNG())
            {
                int planetType = rng.d(6);
                string planetURL;
                switch(planetType)
                {
                    case 1:
                        planetURL = "Images\\Planets\\GreenPlanet.png";
                        break;
                    case 2:
                        planetURL = "Images\\Planets\\RedPlanet.png";
                        break;
                    case 3:
                        planetURL = "Images\\Planets\\PurplePlanet.png";
                        break;
                    case 4:
                        planetURL = "Images\\Planets\\BluePlanet.png";
                        break;
                    case 5:
                        planetURL = "Images\\Planets\\GasPlanet.png";
                        break;
                    case 6:
                        planetURL = "Images\\Planets\\GrayPlanet.png";
                        break;
                    default:
                        planetURL = "Images\\Planets\\IcePlanet.png";
                        break;
                }
                Planet newPlanet = new Planet(planetURL);
                newPlanet.Name=string.Format("{0} {1}",newSystem.Name,ToRoman(planetNumber));
                
                bool isOccupied = true;
                int x=1;
                int y = 1;
                while (isOccupied)
                {
                    //determine which edge
                    x = rng.d(StrategicGridDimension - 2);
                    y = rng.d(StrategicGridDimension - 2);

                    if (newSystem.StrategicLocations[x, y].Stellars.Count==0)
                        isOccupied = false;
                }
                
                newPlanet.StrategicPosition = new System.Drawing.Point(x, y);

                newSystem.StrategicLocations[x, y].Stellars.Add(newPlanet);
            }
        }

        private static Star initStar(string name)
        {
            using (RNG rng = new RNG())
            {
                int starColor = rng.d(5);
                string starURL;
                switch (starColor)
                {
                    case 1:
                        starURL = "Images\\Stars\\RedStar.png";
                        break;
                    case 2:
                        starURL = "Images\\Stars\\YellowStar.png";
                        break;
                    case 3:
                        starURL = "Images\\Stars\\PurpleStar.png";
                        break;
                    case 4:
                        starURL = "Images\\Stars\\GreenStar.png";
                        break;
                    case 5:
                        starURL = "Images\\Stars\\BlueStar.png";
                        break;
                    default:
                        starURL = "Images\\Stars\\DarkStar.png";
                        break;
                }
                Star newStar = new Star(starURL);
                newStar.Name = name;
                return newStar;
            }
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

        private static StarSystem NearestSystemNotConnected(Game result, StarSystem system)
        {
            // get top 10% of systems ordered by distance
            int topTenPercent = Convert.ToInt32(0.1 * result.StarSystems.Count());
            List<StarSystem> NearbySystems = result.StarSystems.Where(f => f != system).OrderBy(r => RelativeDistance(system.GalacticCoordinates, r.GalacticCoordinates)).Take(topTenPercent).ToList<StarSystem>();

            // from top, find next not connected
            for (int i = 0; i < NearbySystems.Count; i++)
            {
                if (!StarSystemsAreConnected(system, NearbySystems[i]) && NearbySystems[i].GetCountOfWarpPoints() < StartingWarpPointsMax)
                    return NearbySystems[i];
            }
            return new StarSystem();
        }
        private static double RelativeDistance(System.Drawing.Point source, System.Drawing.Point target)
        {
            return Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2);
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

        private static System.Drawing.Point WarpPointLocation(StarSystem targetSystem)
        {
            int x = -1;
            int y = -1;
            using (RNG rng = new RNG())
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
                            y = rng.d(StrategicGridDimension - 1);
                            break;
                        case 2:
                            x = rng.d(StrategicGridDimension - 1);
                            y = 0;
                            break;
                        case 3:
                            x = StrategicGridDimension - 1;
                            y = rng.d(StrategicGridDimension - 1);
                            break;
                        case 4:
                            x = rng.d(StrategicGridDimension - 1);
                            y = StrategicGridDimension - 1;
                            break;
                    }
                    if (!targetSystem.StrategicLocations[x, y].Stellars.Any(f => f is WarpPoint))
                        isOccupied = false;
                }
            }
            System.Drawing.Point result = new System.Drawing.Point(x, y);
            return result;
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900); 
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }
    }
}
