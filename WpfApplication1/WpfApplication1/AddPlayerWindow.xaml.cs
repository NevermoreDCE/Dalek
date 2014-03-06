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
        private int _numValue = 10;
        public int AggressivenessValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtNum.Text = value.ToString();
            }
        }

        private void initIconSets()
        {
            DirectoryInfo dir = new DirectoryInfo("Empires");
            var empires = dir.EnumerateDirectories();
            List<string> empireNames = new List<string>();
            foreach (var emp in empires)
            {
                if(emp.Name!="Default")
                    empireNames.Add(emp.Name);
            }
            cbxIconSet.ItemsSource = empireNames;
        }

        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            string PlayerName = tbxPlayerName.Text;
            string EmpireName = tbxEmpireName.Text;
            string IconSet = cbxIconSet.SelectedValue.ToString();
            bool IsAI = (chbIsAI.IsChecked==true?true:false);
            int Aggressiveness = AggressivenessValue;
            Player p = new Player(string.Format("{0}{1}",PlayerName,(IsAI?string.Format(" (AI:{0})",AggressivenessValue):string.Empty)), EmpireName, IconSet, IsAI, AggressivenessValue);
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
                src.UriSource = new Uri(string.Format("Empires\\{0}\\Images\\{0}.png", empireName), UriKind.Relative);
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
        void initAddPlayerWindow(PlayerCollection playerList)
        {
            this.Players = playerList;
            Players.Clear();
            Player one = new Player("Frank", "Furters", "Necrons",false,0);
            Players.Add(one);
            Player two = new Player("Joe (AI:4)", "Shmoes", "Orks",true,4);
            Players.Add(two);
            InitializeComponent();
            initIconSets();
            lbxPlayerList.ItemsSource = Players;
            lbxPlayerList.UpdateLayout();
        }
        public AddPlayerWindow()
        {
            initAddPlayerWindow(new PlayerCollection());
        }
        public AddPlayerWindow(PlayerCollection PlayerList)
        {
            initAddPlayerWindow(PlayerList);
        }
        #endregion

        
        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            AggressivenessValue++;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            AggressivenessValue--;
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(txtNum.Text, out _numValue))
                txtNum.Text = _numValue.ToString();
        }

        private void chbIsAI_Checked(object sender, RoutedEventArgs e)
        {
            if (chbIsAI.IsChecked == true)
            {
                txtNum.IsEnabled = true;
                cmdUp.IsEnabled = true;
                cmdDown.IsEnabled = true;
            }
            else
            {
                txtNum.IsEnabled = false;
                cmdUp.IsEnabled = false;
                cmdDown.IsEnabled = false;
            }
        }

    }
}
