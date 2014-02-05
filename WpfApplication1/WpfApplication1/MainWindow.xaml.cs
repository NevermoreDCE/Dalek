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
using StarShips.Randomizer;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int GridDimensionX = 25;
        const int GridDimensionY = 25;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
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
            
            
            Image img;
            using (RNG rng = new RNG())
            {
                for (int x = 0; x < GridDimensionX; x++)
                {
                    for (int y = 0; y < GridDimensionY; y++)
                    {
                        img = new Image();
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        string filename = string.Format("star_field{0}.png", rng.d(4));
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

                        Label lbl = new Label();
                        lbl.Content = string.Format("{0},{1}", x, y);
                        lbl.Foreground = Brushes.Yellow;
                        lbl.SetValue(Panel.ZIndexProperty, 99);
                        lbl.FontSize = 8;
                        Grid.SetRow(lbl, x);
                        Grid.SetColumn(lbl, y);
                        g.Children.Add(lbl);
                    }
                }
            }
        }

        //void i_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    int[] ArrayLocation = new int[2];
        //    for (int i = 0; i < images.GetLength(1); i++)
        //    {
        //        for (int j = 0; j < images.GetLength(0); j++)
        //        {
        //            if (images[j, i].Equals(e.Source))
        //            {
        //                ArrayLocation[0] = i;
        //                ArrayLocation[1] = j;
        //            }
        //        }
                
        //    }
        //    if (ArrayLocation != null)
        //    {
        //        int x = ArrayLocation[0];
        //        int y = ArrayLocation[1];
        //        label1.Content = string.Format("(X={0},Y={1}) (H={2},W={3})", x, y, images[y, x].ActualHeight, images[y, x].ActualWidth);
        //    }
        //}
    }
}
