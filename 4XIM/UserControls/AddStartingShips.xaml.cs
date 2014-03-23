using StarShips;
using StarShips.Planets;
using StarShips.Players;
using StarShips.Randomizer;
using StarShips.StarSystems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml.Linq;

namespace _4XIM.UserControls
{
    /// <summary>
    /// Interaction logic for AddStartingShips.xaml
    /// </summary>
    public partial class AddStartingShips : UserControl
    {
        Game GameState;
        ObservableCollection<Ship> ExistingShips = new ObservableCollection<Ship>();
        #region Constructors
        public AddStartingShips()
        {
            InitializeComponent();
        }
        public AddStartingShips(Game gameState)
        {
            this.GameState = gameState;
            InitializeComponent();
            
            //Load Hulls
            GameState.ExistingHulls.Clear();
            XDocument hullDoc = XDocument.Load("ShipHulls.xml");
            GameState.ExistingHulls = ShipHull.GetShipHulls(hullDoc);

            //Load ShipParts
            GameState.ExistingParts.Clear();
            XDocument partDoc = XDocument.Load("ShipParts.xml");
            foreach (var part in ShipPart.GetShipPartList(partDoc, new Ship()))
                GameState.ExistingParts.Add(part);

            initAddShipsWindow();
        }
        #endregion
        

        #region Events
        private void cbxPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxPlayers.SelectedIndex > -1)
            {
                Player p = (Player)cbxPlayers.SelectedItem;
                imgPlayerIcon.Source = p.Icon.Source;
                lbxPlayerShips.ItemsSource = p.Ships;
                lbxPlayerShips.UpdateLayout();
                initShips(p);
            }
        }

        private void cbxShipList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ship s = (Ship)cbxShipList.SelectedItem;
            spShipDetails.Children.Clear();
            ShowShipStatus(s, spShipDetails);
        }

        private void btnRemoveShip_Click(object sender, RoutedEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(lbxPlayerShips, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                Ship s = (Ship)item.DataContext;
                ((ShipCollection)lbxPlayerShips.ItemsSource).Remove(s);
            }
        }

        private void btnShipsDone_Click(object sender, RoutedEventArgs e)
        {
            StarSystem firstPlayerSystem = initGameState();
            StrategicWindow w = new StrategicWindow(GameState, GameState.Players.First(),firstPlayerSystem);
            Switcher.Switch(w);
        }

        private void btnAddShip_Click(object sender, RoutedEventArgs e)
        {
            Ship s = (Ship)cbxShipList.SelectedItem;
            Player p = (Player)cbxPlayers.SelectedItem;
            addShipToPlayer(s, p);
        }
        #endregion

        #region Private Methods
        private void addShipToPlayer(Ship s, Player p)
        {
            int countOfClass = p.Ships.Where(f => f.ClassName == s.ClassName).Count();
            Ship shipToAdd = s.Clone();
            shipToAdd.Name = string.Format("{0} - {1}", s.ClassName, (countOfClass + 1).ToString("000"));
            shipToAdd.Owner = p;
            p.Ships.Add(shipToAdd);
        }

        void ShowShipStatus(Ship ship, StackPanel panel)
        {
            panel.Children.Clear();
            // Name
            addStatusLabel(string.Format("{0} - Class {1}", ship.ClassName, ship.HullType.Name), Brushes.White, panel);
            StackPanel spPoints = new StackPanel();
            spPoints.Orientation = Orientation.Horizontal;
            spPoints.Width = 300;
            // HP
            addStatusLabel(string.Format("Hit Points: {0}  ", ship.HP.ToString()), Brushes.White, spPoints);
            // MP
            addStatusLabel(string.Format("Move Points: {0}", ship.MP.ToString()), Brushes.White, spPoints);
            panel.Children.Add(spPoints);
            // Parts
            addStatusLabel("Equipment:", Brushes.White, panel);
            foreach (var part in ship.Parts)
                addStatusLabel(part.ToString(), (part.IsDestroyed ? Brushes.DarkRed : Brushes.LightSlateGray), panel);
        }
        
        void addStatusLabel(string text, Brush color, StackPanel panel)
        {
            TextBlock lbl = new TextBlock();
            lbl.Text = text;
            lbl.Foreground = color;
            panel.Children.Add(lbl);
        }
        
        void initShips(Player p)
        {
            this.ExistingShips.Clear();
            // load source document, hulls and parts
            XDocument xdoc = XDocument.Load(string.Format("Empires\\{0}\\Ships.xml",p.IconSet));
            foreach (XElement shipElement in xdoc.Descendants("ship"))
            {
                Ship ship = new Ship(shipElement, GameState.ExistingParts, GameState.ExistingHulls, p);
                ship.Origin = new System.Drawing.Point();
                Image img = new Image();
                if (File.Exists(string.Format("Empires\\{0}\\Images\\{1}", p.IconSet, ship.HullType.ImageURL)))
                {
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(string.Format("Empires\\{0}\\Images\\{1}", p.IconSet, ship.HullType.ImageURL), UriKind.Relative); ;
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    img.Source = src;
                }
                img.Height = 32;
                img.Width = 32;
                img.Stretch = Stretch.None;
                img.SetValue(Panel.ZIndexProperty, 10);
                ship.Image = img;
                ExistingShips.Add(ship);
            }
        }
        
        void initAddShipsWindow()
        {
            //test stuff
            foreach (Player p in GameState.Players)
            {
                initShips(p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Hunter"),p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Prey"), p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Prey"), p);
            }

            initShips(GameState.Players.First());
            cbxShipList.ItemsSource = ExistingShips;
            cbxPlayers.ItemsSource = GameState.Players;
            if (cbxPlayers.Items.Count > 0)
                cbxPlayers.SelectedIndex = 0;
        }
        
        private StarSystem initGameState()
        {
            StarSystem result= GameState.StarSystems.First();
            List<StarSystem> StartingSystems = new List<StarSystem>();
            using(RNG rng = new RNG())
            foreach(Player p in GameState.Players)
            {
                StarSystem startingSystem = GameState.StarSystems[rng.d(GameState.StarSystems.Count()-1)];
                while(StartingSystems.Contains(startingSystem))
                    startingSystem = GameState.StarSystems[rng.d(GameState.StarSystems.Count()-1)];
                StartingSystems.Add(startingSystem);
                if (p == GameState.Players.First())
                    result = startingSystem;
                Planet startingPlanet;
                if (startingSystem.Planets.Count < 2)
                    startingPlanet = startingSystem.Planets.First();
                else
                {
                    int index = rng.d(startingSystem.Planets.Count-1);
                    startingPlanet = startingSystem.Planets[index];
                }
                startingPlanet.Owner = p;
                // init planet here later
                foreach(Ship s in p.Ships)
                {
                    s.StrategicSystem = startingSystem;
                    s.StrategicPosition = startingPlanet.StrategicPosition;
                    startingSystem.StrategicLocations[startingPlanet.StrategicPosition.X, startingPlanet.StrategicPosition.Y].Ships.Add(s);
                }
            }
            return result;
        }
        #endregion
        
    }
}
