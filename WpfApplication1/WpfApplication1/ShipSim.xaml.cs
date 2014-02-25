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
using StarShips.Players;

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
        PlayerCollection Players = new PlayerCollection();
        Image[,] contextImages = new Image[GridDimensionX, GridDimensionY];
        Ship currentShip;
        Player currentPlayer;
        Image highlightCurrentShip;
        Image moveTarget;
        Image explosionImg;
        BitmapImage contextMenuTransparency;
        
        public ShipSim()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SpaceX.AddPlayerWindow addPlayerWindow = new SpaceX.AddPlayerWindow(Players);
            bool? addPlayerBool = addPlayerWindow.ShowDialog();
            if ((addPlayerBool.HasValue) ? addPlayerBool.Value : false)
            {
                SpaceX.AddShipsWindow addShipsWindow = new SpaceX.AddShipsWindow(Players);
                addShipsWindow.ShowDialog();
            }
            RefreshMap();
        }

        private void RefreshMap()
        {
            g.Children.Clear();
            initGrid();
            initLocations();
            initShipDetails();
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
            ShowShipStatus(currentShip, spCurrentShip);
        }

        #region Initalize Methods
        private void initShipDetails()
        {
            int RangeSize = ((GridDimensionX-1)/Players.Count);
            using (RNG rng = new RNG())
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    Player player = Players[i];
                    int XRangeMin = RangeSize * i;
                    int XRangeMax = RangeSize * (i + 1)-1;

                    foreach (Ship ship in player.Ships)
                    {
                        bool validLoc = false;
                        int x = 0;
                        int y = 0;
                        while (!validLoc)
                        {
                            x = rng.d(GridDimensionX - 1);
                            y = rng.d(GridDimensionY - 1);
                            if (!locations[x, y].IsBlocked && XRangeMin <= x && x <= XRangeMax)
                                validLoc = true;
                        }
                        locations[x, y].Ships.Add(ship);
                        ship.Origin=new System.Drawing.Point(x,y);
                        ship.Position = new System.Drawing.Point(x, y);
                        ship.OnShipDestroyed+=new StarShips.Delegates.ShipDelegates.ShipDestroyedEvent(onShipDestroyedHandler);
                    }
                }
            }
            currentPlayer = Players[0];
            txbCurrentPlayer.Text = currentPlayer.Name;
            imgCurrentPlayerIcon.Source = currentPlayer.Icon.Source;
            currentShip = currentPlayer.Ships[0];

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

            explosionImg = new Image();
            BitmapImage expSrc = new BitmapImage();
            expSrc.BeginInit();
            expSrc.UriSource = new Uri("Explosion.png", UriKind.Relative);
            expSrc.CacheOption = BitmapCacheOption.OnLoad;
            expSrc.EndInit();
            moveTarget.Source = expSrc;
            moveTarget.Height = 32;
            moveTarget.Width = 32;
            moveTarget.Stretch = Stretch.None;
            moveTarget.SetValue(Panel.ZIndexProperty, 200);

            contextMenuTransparency = new BitmapImage();
            contextMenuTransparency.BeginInit();
            contextMenuTransparency.UriSource = new Uri("contextDetector.png", UriKind.Relative);
            contextMenuTransparency.CacheOption = BitmapCacheOption.OnLoad;
            contextMenuTransparency.EndInit();
        }

        #endregion

        #region Image Management Methods
        #region ContextMenu
        private void RefreshContextMenuImages()
        {
            foreach (var p in Players)
                foreach (var s in p.Ships)
                    RefreshContextMenuImages(s.Position);
        }
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
            Separator s = new Separator();
            menu.Items.Add(s);

            AddShipsToMenu(x, y, menu);

            MenuItem ClearMoveOrders = new MenuItem();
            ClearMoveOrders.Header = "Clear Move Orders";
            ClearMoveOrders.Click += new RoutedEventHandler(ClearMoveOrders_Click);
            menu.Items.Add(ClearMoveOrders);


            s = new Separator();
            menu.Items.Add(s);

            AddWeaponsToMenu(x, y, menu);

            MenuItem ClearWeaponOrders = new MenuItem();
            ClearWeaponOrders.Header = "Clear Weapon Orders";
            ClearWeaponOrders.Click += new RoutedEventHandler(ClearWeaponOrders_Click);
            menu.Items.Add(ClearWeaponOrders);
            

            img.ContextMenu = menu;

            contextImages[x, y] = img;
            Grid.SetRow(img, x);
            Grid.SetColumn(img, y);
            g.Children.Add(img);
        }

        
        private void AddShipsToMenu(int x, int y, ContextMenu menu)
        {
            foreach (Player p in Players)
                foreach (Ship ship in p.Ships)
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
                    }
                }
        }
        private void AddWeaponsToMenu(int x, int y, ContextMenu menu)
        {
            foreach(Player p in Players)
                foreach (Ship ship in p.Ships)
                {
                    if (ship.Position == new System.Drawing.Point(x, y) && ship != currentShip && !currentPlayer.Ships.Any(f=>f==ship))
                    {
                        MenuItem MenuFireOnShip = new MenuItem();
                        MenuFireOnShip.Header = string.Format("Fire on {0}", ship.Name);
                        foreach (WeaponPart weapon in currentShip.Equipment.Where(f => f is WeaponPart && !f.IsDestroyed))
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
                        foreach (WeaponPart weapon in currentShip.Equipment.Where(f => f is WeaponPart && !f.IsDestroyed))
                            allWeaponList.Add(weapon);
                        fireAll.CommandParameter = new Tuple<Ship, List<WeaponPart>>(ship, allWeaponList);
                        fireAll.Click += new RoutedEventHandler(MenuWeapon_Click);
                        MenuFireOnShip.Items.Add(fireAll);
                        menu.Items.Add(MenuFireOnShip);
                    }
                }
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
                AddShipImage(x, y, shipInfo.Image);
            }
        }

        private void AddShipImage(int x, int y,Image shipImage)
        {
            Grid.SetRow(shipImage, x);
            Grid.SetColumn(shipImage, y);
            g.Children.Add(shipImage);
        }
        private void RemoveShipImage(Image shipImage)
        {
            ((Grid)shipImage.Parent).Children.Remove(shipImage);
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

        private void AddExplosionImage(Ship shipToExplode)
        {
            Grid.SetRow(explosionImg, shipToExplode.Position.X);
            Grid.SetColumn(explosionImg, shipToExplode.Position.Y);
            g.Children.Add(explosionImg);
            Action<Image, Ship> removeExpImage = new Action<Image, Ship>(removeExplosionImage);
            DelayedExecutionService.RemoveExplosion(removeExpImage, explosionImg, shipToExplode);
        }
        private void removeExplosionImage(Image explosionImage, Ship shipToExplode)
        {
            ((Grid)explosionImg.Parent).Children.Remove(explosionImg);
            ((Grid)shipToExplode.Image.Parent).Children.Remove(shipToExplode.Image);
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

        void ShowShipStatus()
        {
            ShowShipStatus(currentShip, spCurrentShip);
        }
        void ShowShipStatus(Ship ship, StackPanel panel)
        {
            panel.Children.Clear();
            // Name & Icon
            StackPanel titlebar = new StackPanel();
            titlebar.Orientation = Orientation.Horizontal;
            titlebar.VerticalAlignment = VerticalAlignment.Center;
            Image img = new Image();
            img.Source = ship.Image.Source;
            img.Height = 32;
            img.Width = 32;
            img.Stretch = Stretch.None;
            titlebar.Children.Add(img);
            panel.Children.Add(titlebar);
            addStatusLabel(string.Format("{0} ({1} {2})", ship.Name,ship.ClassName, ship.Position), Brushes.White, titlebar);
            // HP
            addStatusLabel(string.Format("Hit Points: {0}", ship.HP.ToString()), Brushes.White, panel);
            // MP
            addStatusLabel(string.Format("Move Points: {0}", ship.MP.ToString()), Brushes.White, panel);
            // Orders Header
            if (currentPlayer.Ships.Any(f => f == ship)) //owned by current player
            {
                addStatusLabel("Current Orders:", Brushes.White, panel);
                foreach (var order in ship.Orders)
                    addStatusLabel(order.ToString(), Brushes.Wheat, panel);
            }
            // Parts
            addStatusLabel("Equipment:", Brushes.White, panel);
            foreach (var part in ship.Equipment)
                addStatusLabel(part.ToString(), (part.IsDestroyed ? Brushes.DarkRed : Brushes.LightSlateGray), panel);
        }
        void addStatusLabel(string text, Brush color, StackPanel panel)
        {
            TextBlock lbl = new TextBlock();
            lbl.Text = text;
            lbl.Foreground = color;
            lbl.VerticalAlignment = VerticalAlignment.Center;
            panel.Children.Add(lbl);
        }
        #endregion

        #region Event Handlers
        #region Orders
        public void onShipMoveHandler(object sender, EventArgs e, Image shipImage, System.Drawing.Point sourceLoc, System.Drawing.Point targetLoc, bool weaponsFiredAlready)
        {
            Action<Image, System.Drawing.Point> moveact = new Action<Image, System.Drawing.Point>(moveShipImageAction);
            RefreshContextMenuImages(sourceLoc, targetLoc);
            int delay = 200;
            if (weaponsFiredAlready)
                delay += 600;
            DelayedExecutionService.MoveShipImageWithDelay(moveact, shipImage, targetLoc, delay);
            
        }
        private void moveShipImageAction(Image shipImage,System.Drawing.Point targetLoc)
        {
            RemoveShipImage(shipImage);
            AddShipImage(targetLoc.X, targetLoc.Y, shipImage);
            System.Diagnostics.Debug.WriteLine("Resolving Move Action");
        }
        public void onWeaponFiredHandler(object sender, EventArgs e, System.Drawing.Point sourceLoc, System.Drawing.Point targetLoc, string firingType)
        {
            // draw beam with offset and delay
            DrawShipFiringBeam(sourceLoc, targetLoc);
        }
        private void DrawShipFiringBeam(System.Drawing.Point sourceLoc, System.Drawing.Point targetLoc)
        {
            using (RNG rng = new RNG())
            {
                // random origin and target (to offset beams)
                Point origin = new Point(((sourceLoc.Y + 1) * 32) - (8 + rng.d(16)), ((sourceLoc.X + 1) * 32) + (rng.d(16)));
                Point target = new Point(((targetLoc.Y + 1) * 32) - (8 + rng.d(16)), ((targetLoc.X + 1) * 32) + (rng.d(16)));
                Path beam = new Path();
                beam.Stroke = Brushes.Red;
                beam.StrokeThickness = 1;
                Point from = TransformToVisual(c).Transform(origin);
                Point to = TransformToVisual(c).Transform(target);
                beam.Data = new LineGeometry(from, to);
                // add and remove actions
                Action<Path> addWeaponAction = new Action<Path>(addWeaponPath);
                Action<Path> removeWeaponAction = new Action<Path>(removeWeaponPath);
                // random firing delay, 300-600ms
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
            System.Diagnostics.Debug.WriteLine("Resolving Fired");
        }
        #endregion

        public void onShipDestroyedHandler(object sender, EventArgs e, Ship shipDestroyed)
        {
            statusWindow.Items.Insert(0, string.Format("{0} is Destroyed!", shipDestroyed.Name));
            AddExplosionImage(shipDestroyed);
            System.Windows.MessageBox.Show(string.Format("Victory! {0} is destroyed!", shipDestroyed.Name));
        }

        #region Menu Items
        void MenuMoveTo_Click(object sender, RoutedEventArgs e)
        {
            RemoveMoveTargetImage();
            Image img = (Image)((MenuItem)e.Source).CommandParameter;
            int x = Grid.GetRow(img);
            int y = Grid.GetColumn(img);
            MoveToLocation mtl = new MoveToLocation(new System.Drawing.Point(x, y), locations);
            mtl.OnShipMove += new OrderDelegates.ShipMoveEvent(onShipMoveHandler);
            currentShip.Orders.Add(mtl);
            AddMoveTargetImage(x, y);
            ShowShipStatus();
        }
        void MenuMoveToShip_Click(object sender, RoutedEventArgs e)
        {
            Tuple<Ship, int> moveToRange = (Tuple<Ship, int>)((MenuItem)sender).CommandParameter;
            MoveToShipAtRange mtsar = new MoveToShipAtRange(moveToRange.Item1, moveToRange.Item2, locations);
            mtsar.OnShipMove += new OrderDelegates.ShipMoveEvent(onShipMoveHandler);
            currentShip.Orders.Add(mtsar);
            ShowShipStatus();
        }
        void ClearMoveOrders_Click(object sender, RoutedEventArgs e)
        {
            RemoveMoveTargetImage();
            currentShip.Orders.RemoveAll(f => f is IMoveOrder);
            statusWindow.Items.Insert(0, string.Format("Cleared Move Orders from {0}", currentShip.Name));
            ShowShipStatus();
        }
        void ClearWeaponOrders_Click(object sender, RoutedEventArgs e)
        {
            currentShip.Orders.RemoveAll(f => f is IWeaponOrder);
            statusWindow.Items.Insert(0, string.Format("Cleared Weapon Orders from {0}", currentShip.Name));
        }
        void ContextMenuImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Drawing.Point targetLoc = new System.Drawing.Point(Grid.GetRow((Image)sender),Grid.GetColumn((Image)sender));
            List<Ship> localShips = new List<Ship>();
            foreach(Player p in Players)
                foreach(Ship s in p.Ships)
                    if(s.Position==targetLoc)
                        localShips.Add(s);
            if (localShips.Count>0)
            {
                Ship ship = localShips.First();
                ShowShipStatus(ship, spTargetShip);
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
            }
            ShowShipStatus();
        }
        #endregion
        
        #region Buttons
        private void btnEndTurn_Click(object sender, RoutedEventArgs e)
        {
            //// process end-of-turn actions
            if (Players[Players.Count - 1] == currentPlayer)
                ProcessTurnResults();

            // set current to next player and first ship
            currentPlayer = Players.Next();
            currentPlayer.Ships.ResetIndex();
            currentShip = currentPlayer.Ships.GetNextShip();
            
            // perform Start Of Turn for current player's ships
            foreach (Ship s in currentPlayer.Ships)
                s.StartOfTurn();
            
            // highlight next player's ship
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
            
            // clear movement target image
            RemoveMoveTargetImage();
            
            // refresh menu options for ships
            RefreshContextMenuImages();

            // show current ship status
            ShowShipStatus(currentShip, spCurrentShip);

            // clear target window
            spTargetShip.Children.Clear();

        }

        void ProcessTurnResults()
        {
            List<string> results = new List<string>();
            for (int impulse = 0; impulse < 30; impulse++)
            {
                foreach (Player p in Players)
                    foreach (Ship s in p.Ships.Where(f=>!f.IsDestroyed))
                    {
                        results = new List<string>();
                        results = s.ExecuteOrders(impulse);
                        foreach (string r in results.Where(f=>f!=string.Empty))
                            statusWindow.Items.Insert(0, r);
                    }
            }
            foreach (Player p in Players)
                foreach (Ship s in p.Ships.Where(f=>!f.IsDestroyed))
                {
                    results = new List<string>();
                    results = s.EndOfTurn();
                    foreach (string r in results.Where(f => f != string.Empty))
                        statusWindow.Items.Insert(0, r);
                }
        }
        
        private void btnNextShip_Click(object sender, RoutedEventArgs e)
        {
            currentShip = currentPlayer.Ships.GetNextShip();
            ShowShipStatus();
            RefreshContextMenuImages();
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
        }
        #endregion
        #endregion

    }

    public static class DelayedExecutionService
    {
        public static void MoveShipImageWithDelay(Action<Image, System.Drawing.Point> action, Image shipImage, System.Drawing.Point targetLoc, int delay = 200)
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
                action(shipImage,targetLoc);
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
        public static void RemoveExplosion(Action<Image, Ship> RemoveExplosionAction, Image explosionImage, Ship shipToExplode, int delay = 1000)
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
                RemoveExplosionAction(explosionImage,shipToExplode);
            };

            dispatcherTimer.Tick += handler;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(delay);
            dispatcherTimer.Start();
        }
    }
}
