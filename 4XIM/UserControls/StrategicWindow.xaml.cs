using StarShips;
using StarShips.Locations;
using StarShips.Orders.Strategic;
using StarShips.Players;
using StarShips.Randomizer;
using StarShips.StarSystems;
using StarShips.Stellars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Player currentPlayer;
        public StarSystem currentSystem;
        public Ship currentShip;
        ObservableCollection<Ship> SelectedShipList = new ObservableCollection<Ship>();
        Image[,] SystemContextImages;
        #endregion

        #region Constructor
        public StrategicWindow(Game gameState, Player currentPlayer = null, StarSystem currentSystem = null, Ship currentShip = null)
        {
            InitializeComponent();
            #region Bindings
            lbxTargetShips.ItemsSource = SelectedShipList;
            #endregion
            initImages();
            this.GameState = gameState;
            this.currentPlayer = currentPlayer;
            this.currentSystem = currentSystem;
            this.currentShip = currentShip;
            
            initGalaxyMap();
            ShowSystemMap(currentSystem);
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
                        AddGalaxyGridImage(x, y);
                        AddGalaxyContextMenuImage(x, y, GalaxyContextMenuImage_MouseDown);

                        if (GameState.StarSystems.Any(f=>f.GalacticCoordinates.X==x&&f.GalacticCoordinates.Y==y))
                        {
                            try
                            {
                                AddSystemImages(x, y);
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
        #endregion

        #region Show Methods
        void ShowSystemMap(StarSystem system)
        {
            initGrid(grdLocalSystem, StrategicGridDimension, StrategicGridDimension);
            SystemContextImages = new Image[StrategicGridDimension, StrategicGridDimension];
            txbSystemName.Text = system.Name;
            using (RNG rng = new RNG())
            {
                for (int x = 0; x < StrategicGridDimension; x++)
                {
                    for (int y = 0; y < StrategicGridDimension; y++)
                    {
                        AddBackgroundImage(grdLocalSystem, x, y, rng.d(4), true);
                        AddShipImages(system, x, y);
                        AddSystemContextMenuImage(x, y, LocalContextMenuImage_MouseDown);

                        if (system.StrategicLocations[x, y].Stellars.Count > 0)
                        {
                            try
                            {
                                AddStellars(system.StrategicLocations[x, y], x, y);
                            }
                            catch { MessageBox.Show("error in system images"); }
                        }

                    }
                }
            }
        }
        private void AddStellars(Location location, int x, int y)
        {
            foreach (Eidos stellar in location.Stellars)
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
        void ShowSelectedCurrentShip(Ship ship)
        {
            imgCurrentShipPlayerIcon.Source = ship.Owner.Icon.Source;
            imgCurrentShipShipIcon.Source = ship.Image.Source;
            txtCurrentShipName.Text = string.Format("Name: {0} ({1}-Class {2})", ship.Name, ship.ClassName, ship.HullType.Name);
            txtCurrentShipLocation.Text = string.Format("Location: {0} ({1},{2})", ship.StrategicSystem.Name, ship.StrategicPosition.X, ship.StrategicPosition.Y);
            txtCurrentShipHP.Text = string.Format("HP: {0}/{1}", ship.HP.Current, ship.HP.Max);
            txtCurrentShipMP.Text = string.Format("MP: {0}/{1}", ship.MP.Current, ship.MP.Max);
            currentShip = ship;
            RefreshSystemContextMenuImages();
        }
        void ClearSelectedCurrentShip()
        {
            imgCurrentShipPlayerIcon.Source = null;
            imgCurrentShipShipIcon.Source = null;
            txtCurrentShipName.Text = string.Empty;
            txtCurrentShipLocation.Text = string.Empty;
            txtCurrentShipHP.Text = string.Empty;
            txtCurrentShipMP.Text = string.Empty;
            currentShip = null;
        }
        void ShowShipStatus(Ship ship)
        {
            try
            {
                spSelectedEidos.Children.Clear();
                Grid nameGrid = new Grid();
                nameGrid.Width = spSelectedEidos.ActualWidth;
                nameGrid.VerticalAlignment = VerticalAlignment.Center;
                nameGrid.HorizontalAlignment = HorizontalAlignment.Center;
                // row
                RowDefinition rowName = new RowDefinition();
                rowName.Height = new GridLength(32); 
                nameGrid.RowDefinitions.Add(rowName);
                // column: owner icon
                ColumnDefinition columnIcon = new ColumnDefinition();
                columnIcon.Width = new GridLength(32);
                nameGrid.ColumnDefinitions.Add(columnIcon);
                // column: ship icon
                ColumnDefinition columnShip = new ColumnDefinition();
                columnShip.Width = new GridLength(32);
                nameGrid.ColumnDefinitions.Add(columnShip);
                // column: name
                ColumnDefinition columnName = new ColumnDefinition();
                columnName.Width = new GridLength(1, GridUnitType.Star);
                nameGrid.ColumnDefinitions.Add(columnName);
                // column: position
                ColumnDefinition columnPosition = new ColumnDefinition();
                columnPosition.Width = new GridLength(1, GridUnitType.Auto);
                nameGrid.ColumnDefinitions.Add(columnPosition);
                //owner icon
                Image icon = new Image();
                icon.Source = ship.Owner.Icon.Source;
                icon.Height = 32;
                icon.Width = 32;
                icon.Stretch = Stretch.None;
                Grid.SetColumn(icon, 0);
                Grid.SetRow(icon, 0);
                nameGrid.Children.Add(icon);
                //ship icon
                Image img = new Image();
                img.Source = ship.Image.Source;
                img.Height = 32;
                img.Width = 32;
                img.Stretch = Stretch.None;
                Grid.SetColumn(img, 1);
                Grid.SetRow(img, 0);
                nameGrid.Children.Add(img);
                //ship name
                TextBlock txtName = new TextBlock();
                txtName.Text = ship.Name;
                txtName.Padding = new Thickness(5, 0, 5, 0);
                txtName.Foreground = Brushes.White;
                Grid.SetColumn(txtName, 2);
                Grid.SetRow(txtName, 0);
                nameGrid.Children.Add(txtName);
                //ship position
                TextBlock txtPosition = new TextBlock();
                txtPosition.Text = string.Format("{0} ({1},{2})", ship.StrategicSystem.Name, ship.StrategicPosition.X, ship.StrategicPosition.Y);
                txtPosition.Padding = new Thickness(5, 0, 5, 0);
                txtPosition.Foreground = Brushes.White;
                Grid.SetColumn(txtPosition, 3);
                Grid.SetRow(txtPosition, 0);
                nameGrid.Children.Add(txtPosition);
                spSelectedEidos.Children.Add(nameGrid);

                Grid classGrid = new Grid();
                classGrid.Width = spSelectedEidos.ActualWidth;
                classGrid.VerticalAlignment = VerticalAlignment.Center;
                classGrid.HorizontalAlignment = HorizontalAlignment.Center;
                // row: owner
                RowDefinition rowOwner = new RowDefinition();
                rowOwner.Height = new GridLength(1, GridUnitType.Auto);
                classGrid.RowDefinitions.Add(rowOwner);
                // row: class/hull
                RowDefinition rowClass = new RowDefinition();
                rowClass.Height = new GridLength(1, GridUnitType.Auto);
                classGrid.RowDefinitions.Add(rowClass);
                // row: hp/mp
                RowDefinition rowHP = new RowDefinition();
                rowHP.Height = new GridLength(1, GridUnitType.Auto);
                classGrid.RowDefinitions.Add(rowHP);
                // column: HP
                ColumnDefinition columnHP = new ColumnDefinition();
                columnHP.Width = new GridLength(1, GridUnitType.Star);
                classGrid.ColumnDefinitions.Add(columnHP);
                // column: MP
                ColumnDefinition columnMP = new ColumnDefinition();
                columnMP.Width = new GridLength(1, GridUnitType.Star);
                classGrid.ColumnDefinitions.Add(columnMP);
                //owner
                TextBlock txtOwner = new TextBlock();
                txtOwner.Text = string.Format("{0} Empire ({1})", ship.Owner.EmpireName, ship.Owner.Name);
                txtOwner.Foreground = Brushes.White;
                Grid.SetColumn(txtOwner, 0);
                Grid.SetRow(txtOwner, 0);
                Grid.SetColumnSpan(txtOwner, 2);
                classGrid.Children.Add(txtOwner);
                //class/hull
                TextBlock txtClass = new TextBlock();
                txtClass.Text = string.Format("{0}-Class {1}", ship.ClassName, ship.HullType.Name);
                txtClass.Foreground = Brushes.White;
                Grid.SetColumn(txtClass, 0);
                Grid.SetRow(txtClass, 1);
                Grid.SetColumnSpan(txtClass, 2);
                classGrid.Children.Add(txtClass);
                //hp
                TextBlock txtHP = new TextBlock();
                txtHP.Text = string.Format("HP: {0}/{1}", ship.HP.Current,ship.HP.Max);
                txtHP.Foreground = Brushes.White;
                Grid.SetColumn(txtHP, 0);
                Grid.SetRow(txtHP, 2);
                classGrid.Children.Add(txtHP);
                //mp
                TextBlock txtMP = new TextBlock();
                txtMP.Text = string.Format("Move: {0}/{1}", ship.MP.Current,ship.MP.Max);
                txtMP.Foreground = Brushes.White;
                Grid.SetColumn(txtMP, 1);
                Grid.SetRow(txtMP, 2);
                classGrid.Children.Add(txtMP);
                spSelectedEidos.Children.Add(classGrid);
                
                if(currentPlayer.Ships.Any(f=>f == ship))
                {
                    // Ship Orders
                    Grid orderGrid = new Grid();
                    orderGrid.Width = spSelectedEidos.ActualWidth;
                    orderGrid.VerticalAlignment = VerticalAlignment.Center;
                    spSelectedEidos.Children.Add(orderGrid);
                    ColumnDefinition columnOrder = new ColumnDefinition();
                    columnOrder.Width = new GridLength(1, GridUnitType.Star);
                    orderGrid.ColumnDefinitions.Add(columnOrder);
                    RowDefinition rowHeader = new RowDefinition();
                    rowHeader.Height = new GridLength(1, GridUnitType.Auto);
                    orderGrid.RowDefinitions.Add(rowHeader);
                    TextBlock txtOrders = new TextBlock();
                    txtOrders.Text = "Orders:";
                    txtOrders.FontSize = 16;
                    txtOrders.Foreground = Brushes.CornflowerBlue;
                    Grid.SetRow(txtOrders, 0);
                    Grid.SetColumn(txtOrders, 0);
                    orderGrid.Children.Add(txtOrders);

                    if(ship.Orders.Count==0)
                    {
                        RowDefinition rowNoOrder = new RowDefinition();
                        rowNoOrder.Height = new GridLength(1, GridUnitType.Auto);
                        orderGrid.RowDefinitions.Add(rowNoOrder);
                        TextBlock txtNoOrders = new TextBlock();
                        txtNoOrders.Text = "No Orders";
                        txtNoOrders.Foreground = Brushes.CornflowerBlue;
                        Grid.SetRow(txtNoOrders, 1);
                        Grid.SetColumn(txtNoOrders, 0);
                        orderGrid.Children.Add(txtNoOrders);
                    }
                    else
                        for (int i = 0; i < ship.Orders.Count; i++)
                        {
                            ShipOrder order = ship.Orders[i];
                            RowDefinition rowOrder = new RowDefinition();
                            rowOrder.Height = new GridLength(1, GridUnitType.Auto);
                            orderGrid.RowDefinitions.Add(rowOrder);
                            Border borderOrder = new Border();
                            ContextMenu menu = new ContextMenu();
                            MenuItem menuRemoveOrder = new MenuItem();
                            menuRemoveOrder.Header = string.Format("Remove Order: {0}", order.ToString());
                            menuRemoveOrder.CommandParameter = new Tuple<Ship,ShipOrder>(ship,order);
                            menuRemoveOrder.Click += menuRemoveOrder_Click;
                            menu.Items.Add(menuRemoveOrder);
                            borderOrder.ContextMenu = menu;
                            TextBlock txtOrder = new TextBlock();
                            txtOrder.Text = order.ToString();
                            txtOrder.Foreground = Brushes.CornflowerBlue;
                            borderOrder.Child = txtOrder;
                            Grid.SetColumn(borderOrder, 0);
                            Grid.SetRow(borderOrder, i + 1);
                            orderGrid.Children.Add(borderOrder);
                        }

                    // Equipment
                    Grid partGrid = new Grid();
                    partGrid.Width = spSelectedEidos.ActualWidth;
                    partGrid.VerticalAlignment = VerticalAlignment.Center;
                    spSelectedEidos.Children.Add(partGrid);
                    ColumnDefinition columnParts = new ColumnDefinition();
                    columnParts.Width = new GridLength(1, GridUnitType.Star);
                    partGrid.ColumnDefinitions.Add(columnParts);
                    RowDefinition rowPartsHeader = new RowDefinition();
                    rowPartsHeader.Height = new GridLength(1, GridUnitType.Auto);
                    partGrid.RowDefinitions.Add(rowPartsHeader);
                    TextBlock txtParts = new TextBlock();
                    txtParts.Text = "Equipment:";
                    txtParts.FontSize = 16;
                    txtParts.Foreground = Brushes.LightSlateGray;
                    Grid.SetRow(txtParts, 0);
                    Grid.SetColumn(txtParts, 0);
                    partGrid.Children.Add(txtParts);
                    for (int i = 0; i < ship.Parts.Count; i++)
                    {
                        ShipPart part = ship.Parts[i];
                        RowDefinition rowPart = new RowDefinition();
                        rowPart.Height = new GridLength(1, GridUnitType.Auto);
                        partGrid.RowDefinitions.Add(rowPart);
                        TextBlock txtPart = new TextBlock();
                        txtPart.Text = part.ToString();
                        txtPart.Foreground = (part.IsDestroyed ? Brushes.DarkRed : Brushes.LightSlateGray);
                        Grid.SetColumn(txtPart, 0);
                        Grid.SetRow(txtPart, i + 1);
                        partGrid.Children.Add(txtPart);
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
        #region Shared
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
        #endregion
        #region Galaxy
        private void AddGalaxyGridImage(int x, int y)
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
                grdGalaxyView.Children.Add(img);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        private void AddGalaxyContextMenuImage(int x, int y, MouseButtonEventHandler MouseDownEventHandler)
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
                grdGalaxyView.Children.Add(img);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        private void AddSystemImages(int x, int y)
        {
            foreach (StarSystem system in GameState.StarSystems.Where(f => f.GalacticCoordinates.X == x && f.GalacticCoordinates.Y == y))
            {
                Grid.SetRow(system.GalaxyImage, x);
                Grid.SetColumn(system.GalaxyImage, y);
                grdGalaxyView.Children.Add(system.GalaxyImage);
                TextBlock tb = new TextBlock();
                tb.Text = system.Name;
                tb.Foreground = Brushes.Yellow;
                tb.FontSize = 8;
                tb.VerticalAlignment = VerticalAlignment.Bottom;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.SetValue(Panel.ZIndexProperty, 900);
                Grid.SetRow(tb, x);
                Grid.SetColumn(tb, y);
                grdGalaxyView.Children.Add(tb);
            }
        }
        #endregion
        #region System
        private void AddSystemContextMenuImage(int x, int y, MouseButtonEventHandler MouseDownEventHandler)
        {
            try
            {
                ContextMenu menu = new System.Windows.Controls.ContextMenu();
                Image img = new Image();

                img = new Image();
                img.Source = contextMenuTransparency;
                img.Height = 32;
                img.Width = 32;
                img.Stretch = Stretch.None;
                img.SetValue(Panel.ZIndexProperty, 1000);
                img.MouseDown += new MouseButtonEventHandler(MouseDownEventHandler);

                if (currentShip != null)
                {
                    MenuItem MenuMoveTo = new MenuItem();
                    string descr = string.Empty;
                    if (currentShip.StrategicSystem != currentSystem)
                        descr = string.Format("Move To {0} ({1},{2})",currentSystem.Name, x, y);
                    else
                        descr = string.Format("Move To ({0},{1})", x, y);
                    MenuMoveTo.Header = descr;
                    MenuMoveTo.CommandParameter = new Tuple<StarSystem,System.Drawing.Point>(currentSystem,new System.Drawing.Point(x,y));
                    MenuMoveTo.Click += new RoutedEventHandler(MenuMoveTo_Click);
                    menu.Items.Add(MenuMoveTo);

                    if(currentSystem.StrategicLocations[x,y].Stellars.Any(f=>f is WarpPoint))
                    {
                        // implement "move to and warp" later
                    }
                }

                // add "Attack" later

                img.ContextMenu = menu;
                SystemContextImages[x, y] = img;
                Grid.SetRow(img, x);
                Grid.SetColumn(img, y);
                grdLocalSystem.Children.Add(img);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        void RefreshSystemContextMenuImages()
        {
            for (int x = 0; x < StrategicGridDimension; x++)
            {
                for (int y = 0; y < StrategicGridDimension; y++)
                {
                    Image i = SystemContextImages[x, y];
                    ((Grid)i.Parent).Children.Remove(i);
                    AddSystemContextMenuImage(x, y, LocalContextMenuImage_MouseDown);
                }
            }
        }
        #endregion
        #region Ships
        private void AddShipImages(StarSystem system, int x, int y)
        {
            try
            {
                foreach (var shipInfo in system.StrategicLocations[x, y].Ships)
                {
                    AddShipImage(x, y, shipInfo.Image);
                }
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        private void AddShipImage(int x, int y, Image shipImage)
        {
            try
            {
                AddShipImage(x, y, shipImage, 0.0);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        private void AddShipImage(int x, int y, Image shipImage, double rotationDegrees)
        {
            try
            {
                if (shipImage.Parent != null)
                    ((Grid)shipImage.Parent).Children.Remove(shipImage);
                Grid.SetRow(shipImage, x);
                Grid.SetColumn(shipImage, y);
                if (rotationDegrees != 0.0)
                {
                    RotateTransform rt = new RotateTransform();
                    rt.CenterX = (shipImage.Width / 2);
                    rt.CenterY = (shipImage.Height / 2);
                    rt.Angle = rotationDegrees;
                    shipImage.RenderTransform = rt;
                }
                grdLocalSystem.Children.Add(shipImage);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        private void RemoveShipImage(Image shipImage)
        {
            try
            {
                ((Grid)shipImage.Parent).Children.Remove(shipImage);
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        #endregion
        #endregion

        #region Logic Methods
        private void endTurn()
        {
            // process end-of-turn actions
            if (GameState.Players[GameState.Players.Count - 1] == currentPlayer)
                ProcessTurnResults();

            // set current to next player and first ship
            currentPlayer = GameState.Players.Next();
            currentPlayer.Ships.ResetIndex();
            currentShip = null;
            //txbCurrentPlayer.Text = currentPlayer.Name;
            //imgCurrentPlayerIcon.Source = currentPlayer.Icon.Source;

            // perform Start Of Turn for current player's ships
            foreach (Ship s in currentPlayer.Ships)
                s.StartOfTurn();

            // refresh menu options for ships
            RefreshSystemContextMenuImages();
            ClearSelectedCurrentShip();
            
            //if (currentPlayer.IsAI)
            //{
            //    try
            //    {
            //        currentPlayer.ExecuteAI(GameState);
            //        initHandlers(currentPlayer);
            //        EndTurn();
            //    }
            //    catch (Exception ex)
            //    {
            //        if (LogEverything)
            //            Logger(ex);
            //    }
            //}
        }

        void ProcessTurnResults()
        {
            try
            {
                List<string> TurnResults = new List<string>();
                List<string> results = new List<string>();
                
                // exec orders
                foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                    foreach (Ship s in p.Ships.Where(f => !f.IsDestroyed))
                    {
                        try
                        {
                            results = new List<string>();
                            results = s.ExecuteStrategicOrders();
                            foreach (string r in results.Where(f => f != string.Empty))
                                TurnResults.Add(string.Format("{0}: {1}: {2}", p, s, r));
                        }
                        catch (Exception ex)
                        {
                            if (LogEverything)
                                Logger(ex);
                        }
                    }
                   
                // end of turn
                foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                    foreach (Ship s in p.Ships.Where(f => !f.IsDestroyed))
                    {
                        try
                        {
                            results = new List<string>();
                            results = s.EndOfTurn();
                            foreach (string r in results.Where(f => f != string.Empty))
                                TurnResults.Add(string.Format("{0}: {1}: {2}", p, s, r));
                        }
                        catch (Exception ex)
                        {
                            if (LogEverything)
                                Logger(ex);
                        }
                    }

                //check for defeat/victory
                //foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                //    if (!p.Ships.Any(f => !f.IsDestroyed))
                //    {
                //        try
                //        {
                //            AnimationQueue.Enqueue(() =>
                //            {
                //                try
                //                {
                //                    MessageBox.Show(string.Format("{0} is Defeated!", p.Name), "Player Defeated");
                //                    NextAnimation();
                //                }
                //                catch (Exception ex)
                //                {
                //                    if (LogEverything)
                //                        Logger(ex);
                //                }
                //            });
                //            p.IsDefeated = true;
                //        }
                //        catch (Exception ex)
                //        {
                //            if (LogEverything)
                //                Logger(ex);
                //        }
                //    }
                //if (GameState.Players.Where(f => !f.IsDefeated).Count() <= 1)
                //{
                //    try
                //    {
                //        if (GameState.Players.Where(f => !f.IsDefeated).Count() <= 0)
                //        {
                //            try
                //            {
                //                AnimationQueue.Enqueue(() =>
                //                {
                //                    try
                //                    {
                //                        MessageBox.Show("Stalemate! All Players Defeated!", "Victory!");
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        if (LogEverything)
                //                            Logger(ex);
                //                    }
                //                });
                //            }
                //            catch (Exception ex)
                //            {
                //                if (LogEverything)
                //                    Logger(ex);
                //            }
                //        }
                //        else
                //        {
                //            try
                //            {
                //                AnimationQueue.Enqueue(() =>
                //                {
                //                    try
                //                    {
                //                        MessageBox.Show(string.Format("{0} is Victorious!", GameState.Players.First(f => !f.IsDefeated).Name), "Victory!");
                //                        NextAnimation();
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        if (LogEverything)
                //                            Logger(ex);
                //                    }
                //                });
                //            }
                //            catch (Exception ex)
                //            {
                //                if (LogEverything)
                //                    Logger(ex);
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        if (LogEverything)
                //            Logger(ex);
                //    }
                //}
                
                // begin animation queue
                //NextAnimation();
            }
            catch (Exception ex)
            {
                if (LogEverything)
                    Logger(ex);
            }
        }
        #endregion

        #region Events
        private void lbxTargetShips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxTargetShips.SelectedItem != null)
            {
                Ship selected = (Ship)lbxTargetShips.SelectedItem;
                ShowShipStatus(selected);
            }
        }
        private void lbxTargetShips_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lbxTargetShips.SelectedItem!=null)
            {
                Ship selected = (Ship)lbxTargetShips.SelectedItem;
                ShowSelectedCurrentShip(selected);
            }
        }
        void GalaxyContextMenuImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Drawing.Point targetLoc = new System.Drawing.Point(Grid.GetRow((Image)sender), Grid.GetColumn((Image)sender));
            if (GameState.StarSystems.Any(f => f.GalacticCoordinates.X == targetLoc.X && f.GalacticCoordinates.Y == targetLoc.Y))
            {
                StarSystem system = GameState.StarSystems.First(f => f.GalacticCoordinates.X == targetLoc.X && f.GalacticCoordinates.Y == targetLoc.Y);
                if (system != null)
                {
                    currentSystem = system;
                    ShowSystemMap(system);
                }
            }
            lbxTargetShips.ItemsSource = null;
            lbxTargetShips.UpdateLayout();
        }
        void LocalContextMenuImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Drawing.Point targetLoc = new System.Drawing.Point(Grid.GetRow((Image)sender), Grid.GetColumn((Image)sender));
            SelectedShipList.Clear();
            foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                foreach (Ship s in p.Ships.Where(f => !f.IsDestroyed))
                    if (s.StrategicPosition == targetLoc && s.StrategicSystem==currentSystem)
                        SelectedShipList.Add(s);
            lbxTargetShips.UpdateLayout();
        }
        void menuRemoveOrder_Click(object sender, RoutedEventArgs e)
        {
            Tuple<Ship, ShipOrder> order = (Tuple<Ship, ShipOrder>)((MenuItem)e.Source).CommandParameter;
            order.Item1.Orders.Remove(order.Item2);
            ShowShipStatus(order.Item1);
        }
        void MenuMoveTo_Click(object sender, RoutedEventArgs e)
        {
            if(currentShip!=null)
            {
                Tuple<StarSystem, System.Drawing.Point> target = (Tuple<StarSystem, System.Drawing.Point>)((MenuItem)e.Source).CommandParameter;
                MoveToLocationInSystem mtlis = new MoveToLocationInSystem(target.Item2, target.Item1);
                currentShip.Orders.Add(mtlis);
                ShowShipStatus(currentShip);
            }
        }
        #endregion

        #region ISwitchable
        public void UtilizeState(object state)
        {
            this.GameState = (Game)state;
        }
        public void EndTurn()
        {
            this.endTurn();
        }
        #endregion
    }
}
