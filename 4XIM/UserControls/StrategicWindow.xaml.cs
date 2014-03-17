using StarShips;
using StarShips.Locations;
using StarShips.Randomizer;
using StarShips.StarSystems;
using StarShips.Stellars;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _4XIM.UserControls
{
    /// <summary>
    /// Interaction logic for StrategicWindow.xaml
    /// </summary>
    public partial class StrategicWindow : UserControl, ISwitchable
    {
        #region Error Logging
        private static void Logger(Exception ex)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Logs"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logs");
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("Exception:");
                sw.WriteLine(string.Format("Message: {0}", ex.Message));
                sw.WriteLine(string.Format("TargetSite: {0}", ex.TargetSite));
                sw.WriteLine(string.Format("Source: {0}", ex.Source));
                sw.WriteLine(string.Format("StackTrace: {0}", ex.StackTrace));
                if (ex.Data.Count > 0)
                {
                    sw.WriteLine("Data");
                    foreach (var data in ex.Data)
                        sw.WriteLine(data.ToString());
                }
                if (ex.InnerException != null)
                    Logger(ex.InnerException, sw);

            }

        }
        private static void Logger(Exception ex, StreamWriter sw)
        {
            sw.WriteLine("Inner Exception:");
            sw.WriteLine(string.Format("Message: {0}", ex.Message));
            sw.WriteLine(string.Format("TargetSite: {0}", ex.TargetSite));
            sw.WriteLine(string.Format("Source: {0}", ex.Source));
            sw.WriteLine(string.Format("StackTrace: {0}", ex.StackTrace));
            sw.WriteLine("Data");
            foreach (var data in ex.Data)
                sw.WriteLine(data.ToString());
            if (ex.InnerException != null)
                Logger(ex.InnerException, sw);
        }
        #endregion

        #region Constants
        private const int StrategicGridDimension = 13;
        private const int GalacticGridDimension = 40;
        
        #endregion

        #region Private Variables
        Game GameState;
        bool LogEverything = false;
        BitmapImage contextMenuTransparency;
        #endregion

        #region Constructor
        public StrategicWindow(Game gameState)
        {
            initImages();
            this.GameState = gameState;
            InitializeComponent();
            initGalaxyMap();
        }
        #endregion

        #region Initialize
        void initImages()
        {
            contextMenuTransparency = new BitmapImage();
            contextMenuTransparency.BeginInit();
            contextMenuTransparency.UriSource = new Uri("Images\\contextDetector.png", UriKind.Relative);
            contextMenuTransparency.CacheOption = BitmapCacheOption.OnLoad;
            contextMenuTransparency.EndInit();
        }
        void initGrid(Grid g, int X, int Y)
        {
            try
            {
                g.Children.Clear();
                for (int i = 0; i < X; i++)
                {
                    RowDefinition r = new RowDefinition();
                    r.Height = new GridLength(32);
                    g.RowDefinitions.Add(r);
                }
                for (int i = 0; i < Y; i++)
                {
                    ColumnDefinition c = new ColumnDefinition();
                    c.Width = new GridLength(32);
                    g.ColumnDefinitions.Add(c);
                }
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        void initGalaxyMap()
        {
            initGrid(grdGalaxyView, GalacticGridDimension, GalacticGridDimension);
            using (RNG rng = new RNG())
            {
                for (int x = 0; x < GalacticGridDimension; x++)
                {
                    for (int y = 0; y < GalacticGridDimension; y++)
                    {
                        AddBackgroundImage(grdGalaxyView, x, y, rng.d(4),false);
                        AddGalaxyGridImage(grdGalaxyView, x, y);
                        AddContextMenuImage(grdGalaxyView, x, y, GalaxyContextMenuImage_MouseDown);

                        if (GameState.StarSystems.Any(f=>f.GalacticCoordinates.X==x&&f.GalacticCoordinates.Y==y))
                        {
                            try
                            {
                                AddSystemImages(grdGalaxyView,x, y);
                            }
                            catch { MessageBox.Show("error in system images"); }
                        }
                        
                    }
                }
            }
            AddWarpPointPaths(grdGalaxyView);
        }
        private void AddWarpPointPaths(Grid grdGalaxyView)
        {
            List<Tuple<WarpPoint,WarpPoint>> WarpPointPairs = new List<Tuple<WarpPoint,WarpPoint>>();
            foreach(StarSystem system in GameState.StarSystems)
                foreach(Location loc in system.StrategicLocations)
                    foreach(WarpPoint ed in loc.Stellars.Where(f=>f is WarpPoint))
                        if(!WarpPointPairs.Contains(new Tuple<WarpPoint,WarpPoint>(ed,ed.LinkedWarpPoint)) && !WarpPointPairs.Contains(new Tuple<WarpPoint,WarpPoint>(ed.LinkedWarpPoint,ed)))
                            WarpPointPairs.Add(new Tuple<WarpPoint,WarpPoint>(ed,ed.LinkedWarpPoint));
            foreach(var pair in WarpPointPairs)
                DrawWarpPointPath(pair.Item1.LinkedSystem.GalacticCoordinates,pair.Item2.LinkedSystem.GalacticCoordinates);
        }
        void initSystemMap(StarSystem system)
        {
            initGrid(grdLocalSystem, StrategicGridDimension, StrategicGridDimension);
            txbSystemName.Text = system.Name;
            using (RNG rng = new RNG())
            {
                for (int x = 0; x < StrategicGridDimension; x++)
                {
                    for (int y = 0; y < StrategicGridDimension; y++)
                    {
                        AddBackgroundImage(grdLocalSystem, x, y, rng.d(4),true);

                        AddContextMenuImage(grdLocalSystem, x, y, LocalContextMenuImage_MouseDown);

                        if (system.StrategicLocations[x,y].Stellars.Count>0)
                        {
                            try
                            {
                                AddStellars(system.StrategicLocations[x,y], x, y);
                            }
                            catch { MessageBox.Show("error in system images"); }
                        }

                    }
                }
            }
        }
        private void AddStellars(Location location, int x, int y)
        {
            foreach(Eidos stellar in location.Stellars)
            {
                Grid.SetRow(stellar.Image, x);
                Grid.SetColumn(stellar.Image, y);
                if (stellar.Image.Parent != null)
                {
                    Panel parent = (Panel)stellar.Image.Parent;
                    parent.Children.Remove(stellar.Image);
                }
                grdLocalSystem.Children.Add(stellar.Image);
                TextBlock tb = new TextBlock();
                tb.Text = stellar.Name;
                tb.Foreground = Brushes.Yellow;
                tb.FontSize = 8;
                tb.VerticalAlignment = VerticalAlignment.Bottom;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.SetValue(Panel.ZIndexProperty, 900);
                Grid.SetRow(tb, x);
                Grid.SetColumn(tb, y);
                grdLocalSystem.Children.Add(tb);
            }
        }
        #endregion

        #region Path Methods
        private void DrawWarpPointPath(System.Drawing.Point sourceLoc, System.Drawing.Point targetLoc)
        {
            try
            {
                using (RNG rng = new RNG())
                {
                    try
                    {
                        // random origin and target (to offset beams)
                        Point origin = new Point(((sourceLoc.Y + 1) * 32) - 16, ((sourceLoc.X) * 32) + 16);
                        Point target = new Point(((targetLoc.Y + 1) * 32) - 16, ((targetLoc.X) * 32) + 16);
                        System.Windows.Shapes.Path warpPath = new System.Windows.Shapes.Path();
                        warpPath.Stroke = Brushes.Gray;
                        warpPath.StrokeThickness = 1;
                        Point from = grdGalaxyView.TransformToVisual(cnvGalaxyView).Transform(origin);
                        Point to = grdGalaxyView.TransformToVisual(cnvGalaxyView).Transform(target);
                        warpPath.Data = new LineGeometry(from, to);
                        cnvGalaxyView.Children.Add(warpPath);
                    }
                    catch (Exception ex)
                    {
                        if (LogEverything)
                            Logger(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        
        #endregion

        #region Image Management Methods
        private void AddBackgroundImage(Grid g, int x, int y, int imageNumber, bool includeGrid)
        {
            try
            {
                Image img = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                string filename = string.Format("Images\\{0}star_field{1}.png", (includeGrid ? string.Empty : "Galaxy_"), imageNumber);
                src.UriSource = new Uri(filename, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                img.Source = src;
                img.Height = 32;
                img.Width = 32;
                img.Stretch = Stretch.None;
                Grid.SetRow(img, x);
                Grid.SetColumn(img, y);
                g.Children.Add(img);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        private void AddGalaxyGridImage(Grid g, int x, int y)
        {
            try
            {
                Image img = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri("Images\\GalaxyGrid.png", UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                img.Source = src;
                img.Height = 32;
                img.Width = 32;
                img.Stretch = Stretch.None;
                img.SetValue(Panel.ZIndexProperty, 2);
                Grid.SetRow(img, x);
                Grid.SetColumn(img, y);
                g.Children.Add(img);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        private void AddContextMenuImage(Grid g, int x, int y, MouseButtonEventHandler MouseDownEventHandler)
        {
            try
            {
                Image img = new Image();

                img = new Image();
                img.Source = contextMenuTransparency;
                img.Height = 32;
                img.Width = 32;
                img.Stretch = Stretch.None;
                img.SetValue(Panel.ZIndexProperty, 1000);
                img.MouseDown += new MouseButtonEventHandler(MouseDownEventHandler);

                Grid.SetRow(img, x);
                Grid.SetColumn(img, y);
                g.Children.Add(img);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        private void AddSystemImages(Grid g, int x, int y)
        {
            foreach (StarSystem system in GameState.StarSystems.Where(f => f.GalacticCoordinates.X == x && f.GalacticCoordinates.Y == y))
            {
                Grid.SetRow(system.GalaxyImage, x);
                Grid.SetColumn(system.GalaxyImage, y);
                g.Children.Add(system.GalaxyImage);
                TextBlock tb = new TextBlock();
                tb.Text = system.Name;
                tb.Foreground = Brushes.Yellow;
                tb.FontSize = 8;
                tb.VerticalAlignment = VerticalAlignment.Bottom;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.SetValue(Panel.ZIndexProperty, 900);
                Grid.SetRow(tb, x);
                Grid.SetColumn(tb, y);
                g.Children.Add(tb);
            }
        }
        #endregion

        #region Events
        private void lbxTargetShips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        void GalaxyContextMenuImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Drawing.Point targetLoc = new System.Drawing.Point(Grid.GetRow((Image)sender), Grid.GetColumn((Image)sender));
            if (GameState.StarSystems.Any(f => f.GalacticCoordinates.X == targetLoc.X && f.GalacticCoordinates.Y == targetLoc.Y))
            {
                StarSystem system = GameState.StarSystems.First(f => f.GalacticCoordinates.X == targetLoc.X && f.GalacticCoordinates.Y == targetLoc.Y);
                if (system != null)
                    initSystemMap(system);
            }
            lbxTargetShips.UpdateLayout();
        }
        void LocalContextMenuImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Drawing.Point targetLoc = new System.Drawing.Point(Grid.GetRow((Image)sender), Grid.GetColumn((Image)sender));
            if (GameState.StarSystems.Any(f => f.GalacticCoordinates.X == targetLoc.X && f.GalacticCoordinates.Y == targetLoc.Y))
            {
                StarSystem system = GameState.StarSystems.First(f => f.GalacticCoordinates.X == targetLoc.X && f.GalacticCoordinates.Y == targetLoc.Y);
                if (system != null)
                    initSystemMap(system);
            }
            lbxTargetShips.UpdateLayout();
        }
       #endregion

        #region ISwitchable
        public void UtilizeState(object state)
        {
            this.GameState = (Game)state;
        }
        #endregion
    }
}
