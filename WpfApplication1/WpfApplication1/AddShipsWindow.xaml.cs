using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StarShips.Players;
using System.Xml.Linq;
using StarShips;
using System.Collections.ObjectModel;
using System.IO;


namespace SpaceX
{
    /// <summary>
    /// Interaction logic for AddShipsWindow.xaml
    /// </summary>
    public partial class AddShipsWindow : Window
    {
        Game gameState;
        ObservableCollection<Ship> ExistingShips = new ObservableCollection<Ship>();

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
            this.DialogResult = true;
            this.Close();
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
            foreach (var part in ship.Equipment)
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
            XDocument xdoc = XDocument.Load("Ships.xml");
            foreach (XElement shipElement in xdoc.Descendants("ship"))
            {
                Ship ship = new Ship(shipElement, gameState.ExistingParts, gameState.ExistingHulls);
                ship.Origin = new System.Drawing.Point();
                Image img = new Image();
                if (File.Exists(string.Format("Images\\Empires\\{0}\\{1}", p.IconSet, ship.HullType.ImageURL)))
                {
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(string.Format("Images\\Empires\\{0}\\{1}", p.IconSet, ship.HullType.ImageURL), UriKind.Relative);;
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
            InitializeComponent();
            initShips(gameState.Players.First());
            //test stuff
            foreach (Player p in gameState.Players)
            {
                initShips(p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Hunter"),p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Prey"), p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Prey"), p);
            }


            cbxShipList.ItemsSource = ExistingShips;
            cbxPlayers.ItemsSource = gameState.Players;
            if (cbxPlayers.Items.Count > 0)
                cbxPlayers.SelectedIndex = 0;
        }
        #endregion
        #region Constructors
        public AddShipsWindow(Game GameState)
        {
            this.gameState = GameState;
            
            
            //Load Hulls
            gameState.ExistingHulls.Clear();
            XDocument hullDoc = XDocument.Load("ShipHulls.xml");
            gameState.ExistingHulls = ShipHull.GetShipHulls(hullDoc);

            //Load Parts
            gameState.ExistingParts.Clear();
            XDocument partDoc = XDocument.Load("ShipParts.xml");
            gameState.ExistingParts = ShipPart.GetShipPartList(partDoc, new Ship());

            //Load Ships
            gameState.ExistingShips.Clear();
            XDocument xdoc = XDocument.Load("Ships.xml");
            foreach (XElement shipElement in xdoc.Descendants("ship"))
            {
                Ship ship = new Ship(shipElement, gameState.ExistingParts, gameState.ExistingHulls);
                gameState.ExistingShips.Add(ship);
            }

            initAddShipsWindow();
        }
        #endregion
    }
}
