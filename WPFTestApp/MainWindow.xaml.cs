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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFTestApp.UserControls;

namespace WPFTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PageSwitcher : Window
    {
        public PageSwitcher()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new MainMenu());
        }

        public void Navigate(UserControl nextPage)
        {
            cprMain.Content = nextPage;
        }

        public void Navigate(UserControl nextPage, object state)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISWitchable! " + nextPage.Name.ToString());

        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate(new MainMenu());
        }

        private void btnPage2_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate(new Page2());
        }

    }
}
