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


namespace SpaceX
{
    /// <summary>
    /// Interaction logic for AddShipsWindow.xaml
    /// </summary>
    public partial class AddShipsWindow : Window
    {
        PlayerCollection SourcePlayers = new PlayerCollection();
        ObservableCollection<Ship> ExistingShips = new ObservableCollection<Ship>();

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

        private static void addShipToPlayer(Ship s, Player p)
        {
            int countOfClass = p.Ships.Where(f => f.ClassName == s.ClassName).Count();
            Ship shipToAdd = s.Clone();
            shipToAdd.Name = string.Format("{0} - {1}", s.ClassName, (countOfClass + 1).ToString("000"));
            shipToAdd.Owner = p;
            p.Ships.Add(shipToAdd);
        }

        private void cbxShipList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ship s = (Ship)cbxShipList.SelectedItem;
            spShipDetails.Children.Clear();
            ShowShipStatus(s, spShipDetails);
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
        
        private void initShips(Player p)
        {
            ExistingShips.Clear();
            // load source document, hulls and parts
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
                src.UriSource = new Uri(string.Format("Images\\Empires\\{0}\\{1}",p.IconSet,ship.HullType.ImageURL), UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                img.Source = src;
                img.Height = 32;
                img.Width = 32;
                img.Stretch = Stretch.None;
                img.SetValue(Panel.ZIndexProperty, 10);
                ship.Image = img;
                //ship.OnShipDestroyed += new StarShips.Delegates.ShipDelegates.ShipDestroyedEvent(onShipDestroyedHandler);
                ExistingShips.Add(ship);
            }
        }

        #region Constructors
        void initAddShipsWindow(PlayerCollection playerList)
        {
            InitializeComponent();
            this.SourcePlayers = playerList;
            initShips(SourcePlayers.First());
            //test stuff
            foreach (Player p in SourcePlayers)
            {
                initShips(p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Hunter"),p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Prey"), p);
                addShipToPlayer(ExistingShips.First(f => f.ClassName == "Prey"), p);
            }


            cbxShipList.ItemsSource = ExistingShips;
            cbxPlayers.ItemsSource = SourcePlayers;
            if (cbxPlayers.Items.Count > 0)
                cbxPlayers.SelectedIndex = 0;
        }
        public AddShipsWindow()
        {
            initAddShipsWindow(new PlayerCollection());
        }

        
        public AddShipsWindow(PlayerCollection Players)
        {
            initAddShipsWindow(Players);
        }
        #endregion
    }
}
