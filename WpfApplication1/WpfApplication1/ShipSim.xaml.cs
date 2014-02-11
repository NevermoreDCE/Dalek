using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StarShips;
using StarShips.Locations;
using StarShips.Randomizer;
using StarShips.Orders;
using StarShips.Orders.Delegates;
using StarShips.Orders.Interfaces;
using System.Windows.Shapes;
using System.Xml.Linq;
using StarShips.Parts;

namespace WPFPathfinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShipSim : Window
    {
        const int GridDimensionX = 25;
        const int GridDimensionY = 25;
        LocationCollection locations = new LocationCollection(GridDimensionX, GridDimensionY);
        Image[,] contextImages = new Image[GridDimensionX, GridDimensionY];
        List<Ship> Ships = new List<Ship>();
        Ship currentShip;
        List<Label> PathTaken = new List<Label>();
        Image highlightCurrentShip;
        Image moveTarget;
        BitmapImage contextMenuTransparency;
        
        public ShipSim()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshMap();
        }

        private void RefreshMap()
        {
            g.Children.Clear();
            initShips();
            initGrid();
            initLocations();
            initShipLocations();
            initImages();


            using (RNG rng = new RNG())
            {
                for (int x = 0; x < GridDimensionX; x++)
                {
                    for (int y = 0; y < GridDimensionY; y++)
                    {
                        AddBackgroundImage(x, y, rng.d(4));
                        AddContextMenuImage(x, y);
                        AddShipImages(x, y);
                        //AddLabel(x, y);

                        if (locations[x, y].IsBlocked)
                        {
                            AddBlockedImage(x, y);
                        }
                    }
                }
            }
            btnEndPlayer2Turn.IsEnabled = false;
            ShowShipStatus(currentShip, spPlayer1);
        }

        #region Initalize Methods
        private void initShipLocations()
        {
            using (RNG rng = new RNG())
            {
                Ship hunter = Ships.First(f => f.Name == "Hunter");
                Ship prey = Ships.First(f => f.Name == "Prey");
                bool validHunter = false;
                int randX = 0;
                int randY = 0;
                while (!validHunter)
                {
                    randX = rng.d(GridDimensionX - 1);
                    randY = rng.d(GridDimensionY - 1);
                    if (!locations[randX, randY].IsBlocked)
                        validHunter = true;
                }
                locations[randX, randY].Ships.Add(hunter);
                hunter.Origin = new System.Drawing.Point(randX, randY);
                hunter.Position = new System.Drawing.Point(randX, randY);

                bool validLoc = false;
                int newX = 0;
                int newY = 0;
                while (!validLoc)
                {
                    newX = rng.d(GridDimensionX - 1);
                    newY = rng.d(GridDimensionY - 1);

                    if ((Math.Abs(randX - newX) >= 5 || Math.Abs(randY - newY) >= 5) && !locations[newX, newY].IsBlocked)
                        validLoc = true;
                }
                locations[newX, newY].Ships.Add(prey);
                prey.Origin = new System.Drawing.Point(newX, newY);
                prey.Position = new System.Drawing.Point(newX, newY);
            }

            // highlight next player's ship
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
            
        }

        private void initLocations()
        {
            Location newLoc;
            for (int x = 0; x < GridDimensionX; x++)
            {
                for (int y = 0; y < GridDimensionY; y++)
                {
                    using (RNG rng = new RNG())
                    {
                        newLoc = new Location();
                        if (rng.d100() > 95)
                            newLoc.IsBlocked = true;
                        newLoc.Name = string.Format("Loc {0},{1}{2}",
                            x.ToString(),
                            y.ToString(),
                            (newLoc.IsBlocked ? Environment.NewLine + " (Blocked)" : string.Empty));
                        locations[x, y] = newLoc;
                    }
                }
            }
        }

        private void initGrid()
        {
            for (int i = 0; i < GridDimensionX; i++)
            {
                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(32);
                g.RowDefinitions.Add(r);
            }
            for (int i = 0; i < GridDimensionY; i++)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(32);
                g.ColumnDefinitions.Add(c);
            }
        }
        
        private void initShips()
        {
            XDocument xdoc = XDocument.Load("Ships.xml");
            List<ShipHull> ExistingHulls;
            List<ShipPart> ExistingParts;
            XDocument doc = XDocument.Load("ShipHulls.xml");
            ExistingHulls = ShipHull.GetShipHulls(doc);
            doc = XDocument.Load("ShipParts.xml");
            ExistingParts = ShipPart.GetShipPartList(doc, new Ship());

            foreach (XElement shipElement in xdoc.Descendants("ship"))
            {
                Ship ship = new Ship(shipElement, ExistingParts, ExistingHulls);
                ship.Origin = new System.Drawing.Point();
                Image img = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(ship.HullType.ImageURL, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                img.Source = src;
                img.Height = 32;
                img.Width = 32;
                img.Stretch = Stretch.None;
                img.SetValue(Panel.ZIndexProperty, 10);
                ship.Image = img;
                Ships.Add(ship);
            }

  
            // set ship to next player
            currentShip = Ships.First(f => f.Name == "Hunter");
            // reset movement on next player's ship
            currentShip.MP.Current = currentShip.MP.Max;
        }

        private void initImages()
        {
            moveTarget = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            string filename = "moveTarget.png";
            src.UriSource = new Uri(filename, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            moveTarget.Source = src;
            moveTarget.Height = 32;
            moveTarget.Width = 32;
            moveTarget.Stretch = Stretch.None;
            moveTarget.SetValue(Panel.ZIndexProperty, 101);


            contextMenuTransparency = new BitmapImage();
            contextMenuTransparency.BeginInit();
            contextMenuTransparency.UriSource = new Uri("contextDetector.png", UriKind.Relative);
            contextMenuTransparency.CacheOption = BitmapCacheOption.OnLoad;
            contextMenuTransparency.EndInit();
        }
        #endregion

        #region Image Management Methods
        #region ContextMenu
        private void RefreshContextMenuImages(System.Drawing.Point location)
        {
            RemoveContextMenuImage(location);
            AddContextMenuImage(location);
        }
        private void RefreshContextMenuImages(System.Drawing.Point origin, System.Drawing.Point next)
        {
            RemoveContextMenuImage(origin);
            AddContextMenuImage(origin);
            RemoveContextMenuImage(next);
            AddContextMenuImage(next);
        }
        private void AddContextMenuImage(System.Drawing.Point point)
        {
            AddContextMenuImage(point.X, point.Y);
        }
        private void AddContextMenuImage(int x, int y)
        {
            Image img = new Image();
            img.Source = contextMenuTransparency;
            img.Height = 32;
            img.Width = 32;
            img.Stretch = Stretch.None;
            img.SetValue(Panel.ZIndexProperty, 1000);
            img.MouseDown += new System.Windows.Input.MouseButtonEventHandler(ContextMenuImage_MouseDown);

            ContextMenu menu = new System.Windows.Controls.ContextMenu();
            MenuItem MenuMoveTo = new MenuItem();
            Image movetoicon = new Image();
            BitmapImage movetoiconsrc = new BitmapImage();
            movetoiconsrc.BeginInit();
            movetoiconsrc.UriSource = new Uri("moveTargetIcon.png", UriKind.Relative);
            movetoiconsrc.CacheOption = BitmapCacheOption.OnLoad;
            movetoiconsrc.EndInit();
            MenuMoveTo.Icon = movetoicon;
            MenuMoveTo.Header = string.Format("Move To ({0},{1})", x, y);
            MenuMoveTo.CommandParameter = img;
            MenuMoveTo.Click += new RoutedEventHandler(MenuMoveTo_Click);
            menu.Items.Add(MenuMoveTo);

            AddShipsToMenu(x, y, menu);

            MenuItem ClearMoveOrders = new MenuItem();
            ClearMoveOrders.Header = "Clear Move Orders";
            ClearMoveOrders.Click += new RoutedEventHandler(ClearMoveOrders_Click);
            menu.Items.Add(ClearMoveOrders);
            
            img.ContextMenu = menu;

            contextImages[x, y] = img;
            Grid.SetRow(img, x);
            Grid.SetColumn(img, y);
            g.Children.Add(img);
        }

        private void AddShipsToMenu(int x, int y, ContextMenu menu)
        {
            foreach (Ship ship in Ships)
            {
                if (ship.Position == new System.Drawing.Point(x, y) && ship != currentShip)
                {
                    MenuItem MenuMoveToShip = new MenuItem();
                    MenuMoveToShip.Header = string.Format("Move To {0}", ship.Name);
                    MenuItem MoveToZero = new MenuItem();
                    MoveToZero.Header = "At 0";
                    MoveToZero.CommandParameter = new Tuple<Ship, int>(ship, 0);
                    MoveToZero.Click += new RoutedEventHandler(MenuMoveToShip_Click);
                    MenuMoveToShip.Items.Add(MoveToZero);
                    MenuItem MoveToTwo = new MenuItem();
                    MoveToTwo.Header = "At 2";
                    MoveToTwo.CommandParameter = new Tuple<Ship, int>(ship, 2);
                    MoveToTwo.Click += new RoutedEventHandler(MenuMoveToShip_Click);
                    MenuMoveToShip.Items.Add(MoveToTwo);
                    MenuItem MoveToFive = new MenuItem();
                    MoveToFive.Header = "At 5";
                    MoveToFive.CommandParameter = new Tuple<Ship, int>(ship, 5);
                    MoveToFive.Click += new RoutedEventHandler(MenuMoveToShip_Click);
                    MenuMoveToShip.Items.Add(MoveToFive);

                    menu.Items.Add(MenuMoveToShip);
                    AddWeaponsToMenu(ship, menu);
                }
            }
        }

        private void AddWeaponsToMenu(Ship ship, ContextMenu menu)
        {
            MenuItem MenuFireOnShip = new MenuItem();
            MenuFireOnShip.Header = string.Format("Fire on {0}", ship.Name);
            foreach (WeaponPart weapon in currentShip.Equipment.Where(f => f is WeaponPart))
            {
                MenuItem MenuWeapon = new MenuItem();
                MenuWeapon.Header = string.Format("With {0}", weapon.Name);
                List<WeaponPart> weaponList = new List<WeaponPart>();
                weaponList.Add(weapon);
                MenuWeapon.CommandParameter = new Tuple<Ship, List<WeaponPart>>(ship, weaponList);
                MenuWeapon.Click += new RoutedEventHandler(MenuWeapon_Click);
                MenuFireOnShip.Items.Add(MenuWeapon);
            }
            MenuItem fireAll = new MenuItem();
            fireAll.Header = "With All Weapons";
            List<WeaponPart> allWeaponList = new List<WeaponPart>();
            foreach (WeaponPart weapon in currentShip.Equipment.Where(f => f is WeaponPart))
                allWeaponList.Add(weapon);
            fireAll.CommandParameter = new Tuple<Ship, List<WeaponPart>>(ship, allWeaponList);
            fireAll.Click += new RoutedEventHandler(MenuWeapon_Click);
            MenuFireOnShip.Items.Add(fireAll);
            menu.Items.Add(MenuFireOnShip);
        }

        private void RemoveContextMenuImage(System.Drawing.Point point)
        {
            RemoveContextMenuImage(point.X, point.Y);
        }
        private void RemoveContextMenuImage(int x, int y)
        {
            Image i = contextImages[x,y];
            ((Grid)i.Parent).Children.Remove(i);
        }
        #endregion

        #region Ships
        private void AddShipImages(int x, int y)
        {
            foreach (var shipInfo in locations[x, y].Ships)
            {
                AddShipImage(x, y, shipInfo);
            }
        }

        private void AddShipImage(int x, int y, Ship shipInfo)
        {
            Image img = shipInfo.Image;
            Grid.SetRow(img, x);
            Grid.SetColumn(img, y);
            g.Children.Add(img);
        }
        private void RemoveShipImage(Ship shipInfo)
        {
            ((Grid)shipInfo.Image.Parent).Children.Remove(shipInfo.Image);
        }
        #endregion

        #region Map Images
        private void AddBackgroundImage(int x, int y, int imageNumber)
        {
            Image img = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            string filename = string.Format("star_field{0}.png", imageNumber);
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

        private void AddBlockedImage(int x, int y)
        {
            Image img = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            string filename = "asteroid1.png";
            src.UriSource = new Uri(filename, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            img.Source = src;
            img.Height = 32;
            img.Width = 32;
            img.Stretch = Stretch.None;
            img.SetValue(Panel.ZIndexProperty, 10);
            Grid.SetRow(img, x);
            Grid.SetColumn(img, y);
            g.Children.Add(img);
        }
        #endregion

        #region Helper Images
        private void AddHighlightImage(int x, int y)
        {
            RemoveHighlightImage();
            if(highlightCurrentShip==null)
            {
                highlightCurrentShip = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                string filename = "highlight.png";
                src.UriSource = new Uri(filename, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                highlightCurrentShip.Source = src;
                highlightCurrentShip.Height = 32;
                highlightCurrentShip.Width = 32;
                highlightCurrentShip.Stretch = Stretch.None;
                highlightCurrentShip.SetValue(Panel.ZIndexProperty, 101);
            }
            Grid.SetRow(highlightCurrentShip, x);
            Grid.SetColumn(highlightCurrentShip, y);
            g.Children.Add(highlightCurrentShip);
        }
        private void RemoveHighlightImage()
        {
            if (highlightCurrentShip != null)
                if (highlightCurrentShip.Parent != null)
                    ((Grid)highlightCurrentShip.Parent).Children.Remove(highlightCurrentShip);
        }

        private void AddMoveTargetImage(int x, int y)
        {
            RemoveMoveTargetImage();
            if (moveTarget == null)
            {
                moveTarget = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                string filename = "moveTarget.png";
                src.UriSource = new Uri(filename, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                moveTarget.Source = src;
                moveTarget.Height = 32;
                moveTarget.Width = 32;
                moveTarget.Stretch = Stretch.None;
                moveTarget.SetValue(Panel.ZIndexProperty, 101);
            }
            Grid.SetRow(moveTarget, x);
            Grid.SetColumn(moveTarget, y);
            g.Children.Add(moveTarget);
        }
        private void RemoveMoveTargetImage()
        {
            if (moveTarget != null)
                if (moveTarget.Parent != null)
                    ((Grid)moveTarget.Parent).Children.Remove(moveTarget);
        }
        #endregion
        #endregion

        #region Label Management Methods
        private Label AddLabel(int x, int y)
        {
            return AddLabel(x, y, string.Format("{0},{1}", x, y), Brushes.Yellow);
        }
        private Label AddLabel(int x, int y, string content, Brush foreground)
        {
            Label lbl = new Label();
            lbl.Content = content;
            lbl.Foreground = foreground;
            lbl.SetValue(Panel.ZIndexProperty, 99);
            lbl.FontSize = 8;
            Grid.SetRow(lbl, x);
            Grid.SetColumn(lbl, y);
            g.Children.Add(lbl);
            return lbl;
        }

        private void AddPathLabel(System.Drawing.Point current)
        {
            PathTaken.Add(AddLabel(current.X, current.Y, "X", Brushes.GreenYellow));
        }

        private void ResetPathLabels()
        {
            foreach (Label path in PathTaken)
            {
                ((Grid)path.Parent).Children.Remove(path);
            }
            PathTaken = new List<Label>();
        }

        void ShowShipStatus(Ship ship, StackPanel panel)
        {
            panel.Children.Clear();
            // Name
            addStatusLabel(string.Format("{0} ({1})", ship.Name, ship.Position), Brushes.White, panel);
            // HP
            addStatusLabel(string.Format("Hit Points: {0}", ship.HP.ToString()), Brushes.White, panel);
            // MP
            addStatusLabel(string.Format("Move Points: {0}", ship.MP.ToString()), Brushes.White, panel);
            // Orders Header
            addStatusLabel("Current Orders:", Brushes.White, panel);
            foreach (var order in ship.Orders)
                addStatusLabel(order.ToString(), Brushes.Wheat, panel);
            // Parts
            addStatusLabel("Equipment:", Brushes.White, panel);
            foreach (var part in ship.Equipment)
                addStatusLabel(part.ToString(), (part.IsDestroyed ? Brushes.DarkRed : Brushes.LightSlateGray), panel);
        }
        void addStatusLabel(string text, Brush color, StackPanel panel)
        {
            Label lbl = new Label();
            lbl.Content = text;
            lbl.Foreground = color;
            panel.Children.Add(lbl);
        }

        #endregion

        #region Pathfinding Methods
        private void FindPath()
        {
            Ship Hunter = Ships.First(f => f.Name == "Hunter");
            Ship Prey = Ships.First(f => f.Name == "Prey");
            while (Hunter.Position != Prey.Position)
            {
                FindOneStep();
                
            }
        }
        private void FindOneStep()
        {
            Ship Hunter = Ships.First(f => f.Name == "Hunter");
            Ship Prey = Ships.First(f => f.Name == "Prey");
            System.Drawing.Point current = Hunter.Position;
            AddPathLabel(current);
            RemoveShipImage(Hunter);
            System.Drawing.Point next = locations.MoveShipToPoint(Hunter, Prey.Position);
            AddShipImage(next.X, next.Y, Hunter);
            
            //lblPathTaken.Content += Environment.NewLine + string.Format("{0}, {1}", next.X, next.Y);
            Hunter.MP.Current = Hunter.MP.Max;
        }
        #endregion

        #region Event Handlers
        #region Orders
        public void onShipMoveHandler(object sender, OrderEventArgs e, Ship shipToMove, bool endAtLocation)
        {
            System.Drawing.Point targetLoc = (System.Drawing.Point)e.OrderValues[0];
            Action<Ship, System.Drawing.Point> moveact = new Action<Ship, System.Drawing.Point>(moveShipAction);
            DelayedExecutionService.MoveShipIconWithDelay(moveact, shipToMove, targetLoc);
             
            if (shipToMove.Position == targetLoc && endAtLocation)
                shipToMove.CompletedOrders.Add((ShipOrder)sender);
        }
        private void moveShipAction(Ship shipToMove, System.Drawing.Point targetLoc)
        {
            if (shipToMove.MP.Current > 0 && shipToMove.Position != targetLoc)
            {
                RemoveShipImage(shipToMove);
                System.Drawing.Point origin = shipToMove.Position;
                System.Drawing.Point next = locations.MoveShipToPoint(shipToMove, targetLoc);
                RefreshContextMenuImages(origin, next);
                AddShipImage(next.X, next.Y, shipToMove);
            }
        }
        public void onWeaponFiredHandler(object sender, EventArgs e, System.Drawing.Point sourceLoc, System.Drawing.Point targetLoc, string firingType)
        {
            DrawShipFiringBeam(sourceLoc, targetLoc);
            ShowShipStatus(Ships.First(f => f.Position == targetLoc), spPlayer2);
        }
        private void DrawShipFiringBeam(System.Drawing.Point firingShip, System.Drawing.Point targetShip)
        {
            using (RNG rng = new RNG())
            {
                Point origin = new Point(((firingShip.Y + 1) * 32) - (8 + rng.d(16)), ((firingShip.X + 1) * 32) + (rng.d(16)));
                Point target = new Point(((targetShip.Y + 1) * 32) - (8 + rng.d(16)), ((targetShip.X + 1) * 32) + (rng.d(16)));
                Path beam = new Path();
                beam.Stroke = Brushes.Red;
                beam.StrokeThickness = 1;
                Point from = TransformToVisual(c).Transform(origin);
                Point to = TransformToVisual(c).Transform(target);
                beam.Data = new LineGeometry(from, to);
                Action<Path> addWeaponAction = new Action<Path>(addWeaponPath);
                Action<Path> removeWeaponAction = new Action<Path>(removeWeaponPath);
                int firingdelay = 300 + rng.d(300);
                DelayedExecutionService.DisplayBeamFiring(addWeaponAction, removeWeaponAction, beam, firingdelay);
            }
        }
        private void addWeaponPath(Path pathToShow)
        {
            c.Children.Add(pathToShow);
        }
        private void removeWeaponPath(Path pathToRemove)
        {
            ((Canvas)pathToRemove.Parent).Children.Remove(pathToRemove);
        }
        #endregion
        #region Menu Items
        void MenuMoveTo_Click(object sender, RoutedEventArgs e)
        {
            RemoveMoveTargetImage();
            Image img = (Image)((MenuItem)e.Source).CommandParameter;
            int x = Grid.GetRow(img);
            int y = Grid.GetColumn(img);
            MoveToLocation mtl = new MoveToLocation(new System.Drawing.Point(x, y));
            mtl.OnShipMove += new OrderDelegates.ShipMoveEvent(onShipMoveHandler);
            currentShip.Orders.Add(mtl);
            AddMoveTargetImage(x, y);
            mtl.ExecuteOrder(currentShip);
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
        }
        void MenuMoveToShip_Click(object sender, RoutedEventArgs e)
        {
            Tuple<Ship, int> moveToRange = (Tuple<Ship, int>)((MenuItem)sender).CommandParameter;
            MoveToShipAtRange mtsar = new MoveToShipAtRange(moveToRange.Item1, moveToRange.Item2, locations);
            mtsar.OnShipMove += new OrderDelegates.ShipMoveEvent(onShipMoveHandler);
            currentShip.Orders.Add(mtsar);
            mtsar.ExecuteOrder(currentShip);
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
        }
        void ClearMoveOrders_Click(object sender, RoutedEventArgs e)
        {
            RemoveMoveTargetImage();
            currentShip.Orders.RemoveAll(f => f is IMoveOrder);
        }
        void ContextMenuImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Drawing.Point targetLoc = new System.Drawing.Point(Grid.GetRow((Image)sender),Grid.GetColumn((Image)sender));
            if (Ships.Any(f => f.Position == targetLoc))
            {
                Ship ship = Ships.First(f => f.Position == targetLoc);
                ShowShipStatus(ship, spPlayer2);
            }
        }
        void MenuWeapon_Click(object sender, RoutedEventArgs e)
        {
            Tuple<Ship, List<WeaponPart>> fireWeapons = (Tuple<Ship, List<WeaponPart>>)((MenuItem)sender).CommandParameter;
            foreach (WeaponPart weapon in fireWeapons.Item2)
            {
                FireWeaponAtTarget fwat = new FireWeaponAtTarget(weapon, fireWeapons.Item1);
                fwat.OnWeaponFired += new OrderDelegates.WeaponFiredEvent(onWeaponFiredHandler);
                currentShip.Orders.Add(fwat);
                fwat.ExecuteOrder(currentShip);
            }
        }
        #endregion
        #endregion

        #region Buttons
        private void btnRefreshMap_Click(object sender, RoutedEventArgs e)
        {
            RefreshMap();
        }

        private void btnFindPath_Click(object sender, RoutedEventArgs e)
        {
            FindPath();
        }
        
        private void btnFindOneStep_Click(object sender, RoutedEventArgs e)
        {
            FindOneStep();
        }

        private void btnResetShipLocations_Click(object sender, RoutedEventArgs e)
        {
            Ship Hunter = Ships.First(f => f.Name == "Hunter");
            Ship Prey = Ships.First(f => f.Name == "Prey");
            locations[Hunter.Position.X, Hunter.Position.Y].Ships.Remove(Hunter);
            RemoveShipImage(Hunter);
            locations[Hunter.Origin.X, Hunter.Origin.Y].Ships.Add(Hunter);
            AddShipImage(Hunter.Origin.X, Hunter.Origin.Y, Hunter);
            Hunter.Position = Hunter.Origin;
            ResetPathLabels();
            //lblPathTaken.Content = "Path Taken:";
        }
        
        private void btnEndPlayer1Turn_Click(object sender, RoutedEventArgs e)
        {
            endPlayer1();
        }

        private void endPlayer1()
        {
            ShowShipStatus(currentShip, spPlayer1);

            // swap player turn buttons
            btnEndPlayer1Turn.IsEnabled = false;
            btnEndPlayer2Turn.IsEnabled = true;
            // remove highlight of current ship
            RemoveHighlightImage();
            // set ship to next player
            currentShip = Ships.First(f => f.Name == "Prey");
            // reset movement on next player's ship
            currentShip.MP.Current = currentShip.MP.Max;
            // highlight next player's ship
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
            // clear up movement target image
            RemoveMoveTargetImage();
            currentShip.ExecuteOrders();
            ShowShipStatus(currentShip, spPlayer2);
        }

        private void btnEndPlayer2Turn_Click(object sender, RoutedEventArgs e)
        {
            endPlayer2();
        }

        private void endPlayer2()
        {
            ShowShipStatus(currentShip, spPlayer2);
            // swap player turn buttons
            btnEndPlayer1Turn.IsEnabled = true;
            btnEndPlayer2Turn.IsEnabled = false;
            // remove highlight of current ship
            RemoveHighlightImage();
            // set ship to next player
            currentShip = Ships.First(f => f.Name == "Hunter");
            // reset movement on next player's ship
            currentShip.MP.Current = currentShip.MP.Max;
            // highlight next player's ship
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
            // clear up movement target image
            RemoveMoveTargetImage();
            currentShip.ExecuteOrders();
            ShowShipStatus(currentShip, spPlayer1);
        }

        private void btnEndTurn_Click(object sender, RoutedEventArgs e)
        {
            // swap player turn buttons
            btnEndPlayer1Turn.IsEnabled = !btnEndPlayer1Turn.IsEnabled;
            btnEndPlayer2Turn.IsEnabled = !btnEndPlayer2Turn.IsEnabled;
            // remove highlight of current ship
            RemoveHighlightImage();
            // set ship to next player
            currentShip = Ships.First(f => f != currentShip);
            // reset movement on next player's ship
            currentShip.MP.Current = currentShip.MP.Max;
            // highlight next player's ship
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
            // clear up movement target image
            RemoveMoveTargetImage();
            currentShip.ExecuteOrders();
            foreach (Ship ship in Ships)
                RefreshContextMenuImages(ship.Position);
            ShowShipStatus(currentShip, spPlayer1);
            spPlayer2.Children.Clear();

        }

        #endregion

        
        private void btnFireWeapon_Click(object sender, RoutedEventArgs e)
        {
            Ship firingShip = currentShip;
            Ship targetShip = Ships.First(f => f != currentShip);
            for (int i = 0; i < 10; i++)
            {
                DrawShipFiringBeam(firingShip.Position, targetShip.Position);
            }
        }
    }

    public static class DelayedExecutionService
    {
        public static void MoveShipIconWithDelay(Action<Ship, System.Drawing.Point> action, Ship shipToMove, System.Drawing.Point targetLoc, int delay = 200)
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Render);
            
            EventHandler handler = null;
            handler = (sender, e) =>
            {
                // Runs until ship out of Movement or ship at target Location
                if (shipToMove.MP.Current <= 0 || shipToMove.Position == targetLoc)
                {
                    // Stop the timer so it won't keep executing every X seconds
                    // and also avoid keeping the handler in memory.
                    dispatcherTimer.Tick -= handler;
                    dispatcherTimer.Stop();
                }
                else
                // Perform the action.
                action(shipToMove,targetLoc);
            };

            dispatcherTimer.Tick += handler;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(delay);
            dispatcherTimer.Start();
        }

        public static void DisplayBeamFiring(Action<Path> AddWeaponAction, Action<Path> RemoveWeaponAction, Path WeaponPath, int delay = 500)
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Render);

            EventHandler handler = null;
            handler = (sender, e) =>
            {
                // Stop the timer so it won't keep executing every X seconds
                // and also avoid keeping the handler in memory.
                dispatcherTimer.Tick -= handler;
                dispatcherTimer.Stop();
                // Perform the action.
                AddWeaponAction(WeaponPath);
                RemoveBeamFiring(RemoveWeaponAction, WeaponPath);
            };

            dispatcherTimer.Tick += handler;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(delay);
            dispatcherTimer.Start();
        }

        public static void RemoveBeamFiring(Action<Path> RemoveWeaponAction, Path WeaponPath, int delay = 500)
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Render);

            EventHandler handler = null;
            handler = (sender, e) =>
            {
                // Stop the timer so it won't keep executing every X seconds
                // and also avoid keeping the handler in memory.
                dispatcherTimer.Tick -= handler;
                dispatcherTimer.Stop();
                // Perform the action.
                RemoveWeaponAction(WeaponPath);
            };

            dispatcherTimer.Tick += handler;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(delay);
            dispatcherTimer.Start();
        }
    }
}
