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
using System.IO;
using System.Collections.ObjectModel;

namespace SpaceX
{
    /// <summary>
    /// Interaction logic for AddPlayerWindow.xaml
    /// </summary>
    public partial class AddPlayerWindow : Window
    {
        PlayerCollection Players = new PlayerCollection();
        
        private void initIconSets()
        {
            DirectoryInfo dir = new DirectoryInfo("Images/Empires");
            var empires = dir.EnumerateDirectories();
            List<string> empireNames = new List<string>();
            foreach (var emp in empires)
            {
                empireNames.Add(emp.Name);
            }
            cbxIconSet.ItemsSource = empireNames;
        }

        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            string PlayerName = tbxPlayerName.Text;
            string EmpireName = tbxEmpireName.Text;
            string IconSet = cbxIconSet.SelectedValue.ToString();
            Player p = new Player(PlayerName, EmpireName, IconSet);
            Players.Add(p);
            lbxPlayerList.UpdateLayout();
            tbxPlayerName.Text = string.Empty;
            tbxEmpireName.Text = string.Empty;
            cbxIconSet.SelectedIndex = -1;
        }

        private void btnPlayerDone_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cbxIconSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // update preview
            spPreview.Children.Clear();
            Label lbl = new Label();
            lbl.Content="Preview";
            lbl.Width=100;
            lbl.Height=70;
            lbl.Foreground = Brushes.White;
            spPreview.Children.Add(lbl);
            if (cbxIconSet.SelectedIndex >= 0)
            {
                string empireName = cbxIconSet.SelectedValue.ToString();
                Image img = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(string.Format("Images\\Empires\\{0}\\{0}.png", empireName), UriKind.Relative);
                src.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                src.EndInit();
                img.Source = src;
                img.Height = 32;
                img.Width = 32;
                img.Stretch = System.Windows.Media.Stretch.None;
                img.SetValue(System.Windows.Controls.Panel.ZIndexProperty, 10);
                spPreview.Children.Add(img);
            }
        }

        private void btnDelPlayer_Click(object sender, RoutedEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(lbxPlayerList, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                Player p = (Player)item.DataContext;
                Players.Remove(p);
            }
        }

        #region Constructors
        public AddPlayerWindow()
        {
            InitializeComponent();
            initIconSets();
            lbxPlayerList.ItemsSource = Players;
            lbxPlayerList.UpdateLayout();
        }
        public AddPlayerWindow(PlayerCollection PlayerList)
        {
            this.Players = PlayerList;
            InitializeComponent();
            initIconSets();
            lbxPlayerList.ItemsSource = Players;
            lbxPlayerList.UpdateLayout();
        }
        #endregion
    }
}
