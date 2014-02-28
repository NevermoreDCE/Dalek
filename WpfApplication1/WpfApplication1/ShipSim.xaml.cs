using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StarShips;
using StarShips.Locations;
using StarShips.Orders;
using StarShips.Orders.Delegates;
using StarShips.Orders.Interfaces;
using StarShips.Parts;
using StarShips.Players;
using StarShips.Randomizer;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace WPFPathfinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShipSim : Window
    {
        #region Constants
        const int GridDimensionX = 25;
        const int GridDimensionY = 25;
        #endregion

        #region Variables
        Game GameState = new Game();

        Image[,] contextImages = new Image[GridDimensionX, GridDimensionY];
        Ship currentShip;
        Player currentPlayer;
        Image highlightCurrentShip;
        Image moveTarget;
        Image explosionImg;
        BitmapImage contextMenuTransparency;
        Queue<Action> AnimationQueue = new Queue<Action>();
        ObservableCollection<Ship> SelectedLocShips = new ObservableCollection<Ship>();
        ImageBrush buttonup;
        ImageBrush buttondown;
        #endregion

        public ShipSim()
        {
            InitializeComponent();
        }
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameState.CombatLocations = new LocationCollection(GridDimensionX, GridDimensionY);
            GameState.Players = new PlayerCollection();
            initImages();
            initButtons();
            initLocations();
            initGrid();
            SpaceX.AddPlayerWindow addPlayerWindow = new SpaceX.AddPlayerWindow(GameState.Players);
            bool? addPlayerBool = addPlayerWindow.ShowDialog();
            if (addPlayerBool==true)
            {
                SpaceX.AddShipsWindow addShipsWindow = new SpaceX.AddShipsWindow(GameState);
                addShipsWindow.ShowDialog();
            }
            initShipDetails();
            BuildMap();
        }

        void BuildMap()
        {
            g.Children.Clear();
            currentPlayer = GameState.Players[0];
            txbCurrentPlayer.Text = currentPlayer.Name;
            imgCurrentPlayerIcon.Source = currentPlayer.Icon.Source;
            currentShip = currentPlayer.Ships[0];

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

                        if (GameState.CombatLocations[x, y].IsBlocked)
                        {
                            try
                            {
                                AddBlockedImage(x, y);
                            }
                            catch { MessageBox.Show("error in blocked images"); }
                        }
                    }
                }
            }
            
            // highlight next player's ship
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);

            ShowShipStatus(currentShip, spCurrentShip);
            lbxTargetShips.ItemsSource = SelectedLocShips;
            lbxTargetShips.UpdateLayout();
        }

        #region Initalize Methods
        void initShipDetails()
        {
            int RangeSize = ((GridDimensionX - 1) / GameState.Players.Count);
            using (RNG rng = new RNG())
            {
                for (int i = 0; i < GameState.Players.Count; i++)
                {
                    Player player = GameState.Players[i];
                    int XRangeMin = RangeSize * i;
                    int XRangeMax = (RangeSize * (i + 1)) - 1;

                    foreach (Ship ship in player.Ships)
                    {   
                        bool validLoc = false;
                        int x = 0;
                        int y = 0;
                        while (!validLoc)
                        {
                            x = rng.d(GridDimensionX - 1);
                            y = rng.d(GridDimensionY - 1);
                            if (!GameState.CombatLocations[x, y].IsBlocked && XRangeMin <= x && x <= XRangeMax)
                                validLoc = true;
                        }
                        GameState.CombatLocations[x, y].Ships.Add(ship);
                        ship.Origin = new System.Drawing.Point(x, y);
                        ship.Position = new System.Drawing.Point(x, y);

                        ship.OnShipDestroyed += new StarShips.Delegates.ShipDelegates.ShipDestroyedEvent(onShipDestroyedHandler);
                    }
                }
            }
        }

        void initGameState()
        {
            g.Children.Clear();
            initHandlers();
            currentPlayer = GameState.Players[0];
            txbCurrentPlayer.Text = currentPlayer.Name;
            imgCurrentPlayerIcon.Source = currentPlayer.Icon.Source;
            currentShip = currentPlayer.Ships[0];

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

                        if (GameState.CombatLocations[x, y].IsBlocked)
                        {
                            AddBlockedImage(x, y);
                        }
                    }
                }
            }
            
            // highlight next player's ship
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);

            ShowShipStatus(currentShip, spCurrentShip);
            lbxTargetShips.ItemsSource = SelectedLocShips;
            lbxTargetShips.UpdateLayout();
        }

        void initHandlers()
        {
            foreach (Player p in GameState.Players)
                initHandlers(p);
                
        }

        void initHandlers(Player p)
        {
            foreach (Ship s in p.Ships)
            {
                s.OnShipDestroyed -= new StarShips.Delegates.ShipDelegates.ShipDestroyedEvent(onShipDestroyedHandler);
                s.OnShipDestroyed += new StarShips.Delegates.ShipDelegates.ShipDestroyedEvent(onShipDestroyedHandler);
                foreach (IWeaponOrder wo in s.Orders.Where(f => f is IWeaponOrder))
                {
                    wo.OnWeaponFired -= new OrderDelegates.WeaponFiredEvent(onWeaponFiredHandler);
                    wo.OnWeaponFired += new OrderDelegates.WeaponFiredEvent(onWeaponFiredHandler);
                }
                foreach (IMoveOrder mo in s.Orders.Where(f => f is IMoveOrder))
                {
                    mo.OnShipMove -= new OrderDelegates.ShipMoveEvent(onShipMoveHandler);
                    mo.OnShipMove += new OrderDelegates.ShipMoveEvent(onShipMoveHandler);
                }
            }
        }

        void initLocations()
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
                        GameState.CombatLocations[x, y] = newLoc;
                    }
                }
            }
        }

        void initGrid()
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
        
        void initImages()
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
            explosionImg.Source = expSrc;
            explosionImg.Height = 32;
            explosionImg.Width = 32;
            explosionImg.Stretch = Stretch.None;
            explosionImg.SetValue(Panel.ZIndexProperty, 200);

            contextMenuTransparency = new BitmapImage();
            contextMenuTransparency.BeginInit();
            contextMenuTransparency.UriSource = new Uri("contextDetector.png", UriKind.Relative);
            contextMenuTransparency.CacheOption = BitmapCacheOption.OnLoad;
            contextMenuTransparency.EndInit();

            
            BitmapImage buttonupSrc = new BitmapImage();
            buttonupSrc.BeginInit();
            buttonupSrc.UriSource = new Uri("Images\\buttonup.png", UriKind.Relative);
            buttonupSrc.CacheOption = BitmapCacheOption.OnLoad;
            buttonupSrc.EndInit();
            buttonup = new ImageBrush(buttonupSrc);

            BitmapImage buttondownSrc = new BitmapImage();
            buttondownSrc.BeginInit();
            buttondownSrc.UriSource = new Uri("Images\\buttondown.png", UriKind.Relative);
            buttondownSrc.CacheOption = BitmapCacheOption.OnLoad;
            buttondownSrc.EndInit();
            buttondown = new ImageBrush(buttondownSrc);
        }

        void initButtons()
        {
            btnEndTurn.Background = buttonup;
            btnNextShip.Background = buttonup;
            btnSaveGame.Background = buttonup;
            btnLoadGame.Background = buttonup;
        }
        #endregion

        #region Image Management Methods
        #region ContextMenu
        private void RefreshContextMenuImages()
        {
            foreach (var p in GameState.Players)
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
            ContextMenu menu = new System.Windows.Controls.ContextMenu();
            Separator s = new Separator();
            Image img = new Image();
            
            img = new Image();
            img.Source = contextMenuTransparency;
            img.Height = 32;
            img.Width = 32;
            img.Stretch = Stretch.None;
            img.SetValue(Panel.ZIndexProperty, 1000);
            img.MouseDown += new System.Windows.Input.MouseButtonEventHandler(ContextMenuImage_MouseDown);

            MenuItem MenuMoveTo = new MenuItem();
            MenuMoveTo.Header = string.Format("Move To ({0},{1})", x, y);
            MenuMoveTo.CommandParameter = img;
            MenuMoveTo.Click += new RoutedEventHandler(MenuMoveTo_Click);
            menu.Items.Add(MenuMoveTo);

            s = new Separator();
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
            foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                foreach (Ship ship in p.Ships.Where(f=>!f.IsDestroyed))
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

                        List<double> ranges = new List<double>();
                        foreach (WeaponPart part in GameState.ExistingParts.Where(f => f is WeaponPart))
                            if (!ranges.Contains(part.Range) && part.Range > 0)
                                ranges.Add(part.Range);
                        ranges.Sort();
                        foreach (double range in ranges)
                        {
                            MenuItem move = new MenuItem();
                            move.Header = string.Format("At {0}", Convert.ToInt32(range));
                            move.CommandParameter = new Tuple<Ship, int>(ship, Convert.ToInt32(range));
                            move.Click += new RoutedEventHandler(MenuMoveToShip_Click);
                            MenuMoveToShip.Items.Add(move);
                        }

                        menu.Items.Add(MenuMoveToShip);
                    }
                }
        }
        private void AddWeaponsToMenu(int x, int y, ContextMenu menu)
        {
            foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                foreach (Ship ship in p.Ships.Where(f=>!f.IsDestroyed))
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
            foreach (var shipInfo in GameState.CombatLocations[x, y].Ships)
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
            if(explosionImg.Parent!=null)
                ((Grid)explosionImg.Parent).Children.Remove(explosionImg);
            Grid.SetRow(explosionImg, shipToExplode.Position.X);
            Grid.SetColumn(explosionImg, shipToExplode.Position.Y);
            g.Children.Add(explosionImg);
            Action<Image, Ship> removeExpImage = new Action<Image, Ship>(removeExplosionImage);
            RemoveExplosion(removeExpImage, explosionImg, shipToExplode);
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

        #region Animation Queue
        public void NextAnimation()
        {
            if (AnimationQueue.Count > 0)
            {
                Action action = AnimationQueue.Dequeue();
                action();
            }
        }
        #endregion

        #region Event Handlers
        #region Orders
        public void onShipMoveHandler(object sender, EventArgs e, Image shipImage, System.Drawing.Point sourceLoc, System.Drawing.Point targetLoc, bool weaponsFiredAlready)
        {
            Action<Image, System.Drawing.Point> moveact = new Action<Image, System.Drawing.Point>(moveShipImageAction);
            RefreshContextMenuImages(sourceLoc, targetLoc);
            int delay = 200;
            AnimationQueue.Enqueue(() =>
            {
                MoveShipImageWithDelay(moveact, shipImage, targetLoc, delay);
            });
            
        }
        public void onWeaponFiredHandler(object sender, EventArgs e, System.Drawing.Point sourceLoc, System.Drawing.Point targetLoc, string firingType)
        {
            // draw beam with offset and delay
            AnimationQueue.Enqueue(() =>
                {
                    DrawShipFiringBeam(sourceLoc, targetLoc);
                });
        }
        #endregion

        public void onShipDestroyedHandler(object sender, EventArgs e, Ship shipDestroyed)
        {
            statusWindow.Items.Insert(0, string.Format("{0} is Destroyed!", shipDestroyed.Name));
            AnimationQueue.Enqueue(() =>
                {
                    AddExplosionImage(shipDestroyed);
                });
        }

        #region Context Menu Items
        void MenuMoveTo_Click(object sender, RoutedEventArgs e)
        {
            RemoveMoveTargetImage();
            Image img = (Image)((MenuItem)e.Source).CommandParameter;
            int x = Grid.GetRow(img);
            int y = Grid.GetColumn(img);
            MoveToLocation mtl = new MoveToLocation(new System.Drawing.Point(x, y), GameState.CombatLocations);
            mtl.OnShipMove += new OrderDelegates.ShipMoveEvent(onShipMoveHandler);
            currentShip.Orders.Add(mtl);
            AddMoveTargetImage(x, y);
            ShowShipStatus();
        }
        void MenuMoveToShip_Click(object sender, RoutedEventArgs e)
        {
            Tuple<Ship, int> moveToRange = (Tuple<Ship, int>)((MenuItem)sender).CommandParameter;
            MoveToShipAtRange mtsar = new MoveToShipAtRange(moveToRange.Item1, moveToRange.Item2, GameState.CombatLocations);
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
        void ClearWeaponOrders_Click(object sender, RoutedEventArgs e)
        {
            currentShip.Orders.RemoveAll(f => f is IWeaponOrder);
            statusWindow.Items.Insert(0, string.Format("Cleared Weapon Orders from {0}", currentShip.Name));
        }
        void ContextMenuImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Drawing.Point targetLoc = new System.Drawing.Point(Grid.GetRow((Image)sender),Grid.GetColumn((Image)sender));
            SelectedLocShips.Clear();
            foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                foreach(Ship s in p.Ships.Where(f=>!f.IsDestroyed))
                    if(s.Position==targetLoc)
                        SelectedLocShips.Add(s);
            lbxTargetShips.UpdateLayout();
        }
        void lbxTargetShips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxTargetShips.SelectedItem != null)
            {
                Ship selected = (Ship)lbxTargetShips.SelectedItem;
                ShowShipStatus(selected, spTargetShip);
            }
        }
        #endregion
        
        #region Buttons
        private void imgButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border b = (Border)sender;
            b.Background = buttonup;
        }
        private void btnEndTurn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border b = (Border)sender;
            b.Background = buttondown;
            EndTurn();
        }
        private void btnNextShip_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border b = (Border)sender;
            b.Background = buttondown;
            currentShip = currentPlayer.Ships.GetNextShip();
            ShowShipStatus();
            RefreshContextMenuImages();
            AddHighlightImage(currentShip.Position.X, currentShip.Position.Y);
        }
        private void btnSaveGame_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border b = (Border)sender;
            b.Background = buttondown;
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.FileName = "NewGame";
            sfd.DefaultExt = ".sav";
            sfd.Filter = "Save Games (.sav)|*.sav";
            Nullable<bool> result = sfd.ShowDialog();
            if (result == true)
            {
                string fileName = sfd.FileName;
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, GameState);
                stream.Flush();
                MessageBox.Show(string.Format("Save Complete! Size: {0}",stream.Length));
                stream.Close();
            }
        }
        private void btnLoadGame_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border b = (Border)sender;
            b.Background = buttondown;
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.FileName = "NewGame";
            ofd.DefaultExt = ".sav";
            ofd.Filter = "Save Games (.sav)|*.sav";
            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                string fileName = ofd.FileName;
                System.Diagnostics.Debug.WriteLine(fileName);
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                stream.Position = 0;
                MessageBox.Show(string.Format("size: {0}, Filename: {1}", stream.Length,((FileStream)stream).Name));
                GameState = (Game)formatter.Deserialize(stream);
                stream.Close();
                
                
                initImages();
                initButtons();
                initGameState();
            }
        }

        #endregion
        #endregion

        #region Logic Methods
        private void moveShipImageAction(Image shipImage, System.Drawing.Point targetLoc)
        {
            RemoveShipImage(shipImage);
            AddShipImage(targetLoc.X, targetLoc.Y, shipImage);
            System.Diagnostics.Debug.WriteLine("Resolving Move Action");
        }
        private void DrawShipFiringBeam(System.Drawing.Point sourceLoc, System.Drawing.Point targetLoc)
        {
            using (RNG rng = new RNG())
            {
                // random origin and target (to offset beams)
                Point origin = new Point(((sourceLoc.Y + 1) * 32) - (8 + rng.d(16)), ((sourceLoc.X + 1) * 32) + (rng.d(16)));
                Point target = new Point(((targetLoc.Y + 1) * 32) - (8 + rng.d(16)), ((targetLoc.X + 1) * 32) + (rng.d(16)));
                System.Windows.Shapes.Path beam = new System.Windows.Shapes.Path();
                beam.Stroke = Brushes.Red;
                beam.StrokeThickness = 1;
                Point from = TransformToVisual(c).Transform(origin);
                Point to = TransformToVisual(c).Transform(target);
                beam.Data = new LineGeometry(from, to);
                // add and remove actions
                Action<System.Windows.Shapes.Path> addWeaponAction = new Action<System.Windows.Shapes.Path>(addWeaponPath);
                Action<System.Windows.Shapes.Path> removeWeaponAction = new Action<System.Windows.Shapes.Path>(removeWeaponPath);
                // random firing delay, 300-600ms
                int firingdelay = 100 + rng.d(100);
                DisplayBeamFiring(addWeaponAction, removeWeaponAction, beam, firingdelay);
            }
        }
        private void addWeaponPath(System.Windows.Shapes.Path pathToShow)
        {
            c.Children.Add(pathToShow);
        }
        private void removeWeaponPath(System.Windows.Shapes.Path pathToRemove)
        {
            ((Canvas)pathToRemove.Parent).Children.Remove(pathToRemove);
            System.Diagnostics.Debug.WriteLine("Resolving Fired");
        }
        
        private void EndTurn()
        {
            // process end-of-turn actions
            if (GameState.Players[GameState.Players.Count - 1] == currentPlayer)
                ProcessTurnResults();

            // set current to next player and first ship
            currentPlayer = GameState.Players.Next();
            currentPlayer.Ships.ResetIndex();
            currentShip = currentPlayer.Ships.GetNextShip();
            txbCurrentPlayer.Text = currentPlayer.Name;
            imgCurrentPlayerIcon.Source = currentPlayer.Icon.Source;

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

            if (currentPlayer.IsAI)
            {
                currentPlayer.ExecuteAI(GameState);
                initHandlers(currentPlayer);
                EndTurn();
            }
            
        }
        void ProcessTurnResults()
        {
            List<string> results = new List<string>();
            for (int impulse = 0; impulse < 30; impulse++)
            {
                foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                    foreach (Ship s in p.Ships.Where(f => !f.IsDestroyed))
                    {
                        results = new List<string>();
                        results = s.ExecuteOrders(impulse);
                        foreach (string r in results.Where(f => f != string.Empty))
                            statusWindow.Items.Insert(0, string.Format("{0}: {1}: {2}",p,s,r));
                    }
            }
            foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                foreach (Ship s in p.Ships.Where(f => !f.IsDestroyed))
                {
                    results = new List<string>();
                    results = s.EndOfTurn();
                    foreach (string r in results.Where(f => f != string.Empty))
                        statusWindow.Items.Insert(0, string.Format("{0}: {1}: {2}", p, s, r));
                }
            NextAnimation();

            foreach (Player p in GameState.Players.Where(f => !f.IsDefeated))
                if (!p.Ships.Any(f => !f.IsDestroyed))
                {
                    MessageBox.Show(string.Format("{0} is Defeated!", p.Name), "Player Defeated");
                    p.IsDefeated = true;
                }
            if (GameState.Players.Where(f => !f.IsDefeated).Count() <= 1)
            {
                if (GameState.Players.Where(f => !f.IsDefeated).Count() <= 0)
                    MessageBox.Show("Stalemate! All Players Defeated!", "Victory!");
                else
                    MessageBox.Show(string.Format("{0} is Victorious!", GameState.Players.First(f => !f.IsDefeated).Name), "Victory!");
            }
        }
        
        #endregion

        #region DelayedExecution
        public void MoveShipImageWithDelay(Action<Image, System.Drawing.Point> action, Image shipImage, System.Drawing.Point targetLoc, int delay = 100)
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
                action(shipImage, targetLoc);
                NextAnimation();
            };

            dispatcherTimer.Tick += handler;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(delay);
            dispatcherTimer.Start();
        }
        public void DisplayBeamFiring(Action<System.Windows.Shapes.Path> AddWeaponAction, Action<System.Windows.Shapes.Path> RemoveWeaponAction, System.Windows.Shapes.Path WeaponPath, int delay = 100)
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
        public void RemoveBeamFiring(Action<System.Windows.Shapes.Path> RemoveWeaponAction, System.Windows.Shapes.Path WeaponPath, int delay = 300)
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
                NextAnimation();
            };

            dispatcherTimer.Tick += handler;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(delay);
            dispatcherTimer.Start();
        }
        public void RemoveExplosion(Action<Image, Ship> RemoveExplosionAction, Image explosionImage, Ship shipToExplode, int delay = 300)
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
                RemoveExplosionAction(explosionImage, shipToExplode);
                NextAnimation();
            };

            dispatcherTimer.Tick += handler;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(delay);
            dispatcherTimer.Start();
        }
        #endregion

        
    }
}
