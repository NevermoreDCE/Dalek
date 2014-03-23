using StarShips.Locations;
using StarShips.Planets;
using StarShips.Stellars;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StarShips.StarSystems
{
    [Serializable]
    public class StarSystem : ISerializable
    {
        #region Private Variables
        string _name = "New System";
        System.Drawing.Point _galacticCoordinates = new System.Drawing.Point(-1,-1);
        LocationCollection _strategicLocations;
        System.Windows.Controls.Image _galaxyImage;
        string _galaxyImageURL = "Images\\GalaxySystem.png";
        #endregion

        #region Public Properties
        public string Name { get { return _name; } set { _name = value; } }
        public System.Drawing.Point GalacticCoordinates { get { return _galacticCoordinates; } set { _galacticCoordinates = value; } }
        public LocationCollection StrategicLocations { get { return _strategicLocations; } set { _strategicLocations = value; } }
        public System.Windows.Controls.Image GalaxyImage
        {
            get
            {
                if (_galaxyImage == null)
                {
                    System.Windows.Media.Imaging.BitmapImage src = new System.Windows.Media.Imaging.BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(_galaxyImageURL, UriKind.Relative);
                    src.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    src.EndInit();
                    _galaxyImage = new System.Windows.Controls.Image();
                    _galaxyImage.Source = src;
                    _galaxyImage.Height = 32;
                    _galaxyImage.Width = 32;
                    _galaxyImage.Stretch = System.Windows.Media.Stretch.None;
                    _galaxyImage.SetValue(System.Windows.Controls.Panel.ZIndexProperty, 10);
                }
                return _galaxyImage;
            }
            set { _galaxyImage = value; }
        }
        public List<Planet> Planets
        {
            get
            {
                List<Planet> result = new List<Planet>();
                foreach (Location loc in _strategicLocations)
                    foreach (Planet p in loc.Stellars.Where(f => f is Planet))
                        result.Add(p);
                return result;
            }
        }
        
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods
        public int GetCountOfWarpPoints()
        {
            int result = 0;
            foreach (var loc in _strategicLocations)
                foreach (var stel in loc.Stellars)
                    if (stel is StarShips.Stellars.WarpPoint)
                        result++;
            return result;
        }
        public List<StarSystem> GetConnectedStarSystems()
        {
            List<StarSystem> result = new List<StarSystem>();
            foreach (var loc in _strategicLocations.Where(f => f.Stellars.Any(s => s is StarShips.Stellars.WarpPoint)))
                foreach (var stel in loc.Stellars)
                    if (stel is StarShips.Stellars.WarpPoint)
                        result.Add(((StarShips.Stellars.WarpPoint)stel).LinkedSystem);
            return result;

        }

        public override string ToString()
        {
            return Name;
        }

        public Point GetWarpPointPosition(StarSystem targetSystem)
        {
            Point result = new Point(-1, -1);
            for (int x = 0; x < this.StrategicLocations.GetLength(0); x++)
            {
                for (int y = 0; y < this.StrategicLocations.GetLength(1); y++)
                {
                    if (this.StrategicLocations[x, y].Stellars.Any(s => s is WarpPoint))
                        if (this.StrategicLocations[x, y].Stellars.Any(s => ((WarpPoint)s).LinkedSystem == targetSystem))
                            result = new Point(x, y);
                }
            }
            return result;
        }
        #endregion

        #region Private Methods
        private void initLocations(int GridX, int GridY)
        {
            for (int x = 0; x < GridY; x++)
            {
                for (int y = 0; y < GridY; y++)
                {
                    _strategicLocations[x, y] = new Location();
                }
            }
        }
        #endregion

        #region Constructors
        public StarSystem()
        {

        }
        public StarSystem(int GridX, int GridY)
        {
            _strategicLocations = new LocationCollection(GridX, GridY);
            initLocations(GridX, GridY);
        }
        #endregion
    }
}
