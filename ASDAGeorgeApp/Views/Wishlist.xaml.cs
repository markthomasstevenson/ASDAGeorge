using ASDAGeorgeApp.Interface;
using ASDAGeorgeApp.Models;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
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

namespace ASDAGeorgeApp.Views
{
    /// <summary>
    /// Interaction logic for Wishlist.xaml
    /// </summary>
    public partial class Wishlist : UserControl, ISwitchable
    {
        public Wishlist()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
                {
                    DisplayItems();
                };
        }

        private void DisplayItems()
        {
            // display all the categorys
            this.ProductContainer.Children.Clear();
            if (Collector.Wishlist.Count == 0)
            {
                // this is where you show the empty wishlist text/image
                this.ProductContainer.Children.Clear();
                this.ScrollViewer.Visibility = Visibility.Hidden;
                this.HiddenStuff.Visibility = Visibility.Visible;
            }
            else
            {
                foreach (Item item in Collector.Wishlist)
                {
                    // new button
                    KinectTileButton newTile = new KinectTileButton();
                    // add title
                    newTile.Label = "£" + item.Price.ToString() + "\r\n" + item.Title;
                    // create background image
                    BitmapImage bitImg = new BitmapImage();
                    bitImg.BeginInit();
                    if (File.Exists(item.ProductImage))
                        bitImg.UriSource = new Uri(item.ProductImage);
                    else
                        bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\ProductImages\\tempImage.png");
                    bitImg.EndInit();
                    // attach background image
                    newTile.Background = new ImageBrush(bitImg);
                    // set name
                    newTile.Name = item.UniqueID;
                    // set label background
                    newTile.LabelBackground = (Brush)Application.Current.Resources["LabelBackground"];
                    // set foreground
                    newTile.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdfdfd"));
                    //set font family
                    newTile.FontFamily = new FontFamily("Segoe UI Light");
                    // add height
                    newTile.Height = this.ProductContainer.Height;
                    // add width
                    newTile.Width = 448;
                    newTile.Style = (Style)Application.Current.Resources["ProductListItem"];

                    this.ProductContainer.Children.Add(newTile);
                }
                this.HiddenStuff.Visibility = Visibility.Hidden;
                this.ScrollViewer.Visibility = Visibility.Visible;
            }
        }

        public void UtiliseState(object state)
        {
            throw new NotImplementedException();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LandingPage());
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            KinectTileButton newButton = e.OriginalSource as KinectTileButton;
            Item item = Collector.GetItem(newButton.Name);
            Switcher.Switch(new Product(item, Collector.GetSubCategory(item.ParentSub, item.ParentCat), Collector.GetCategory(item.ParentCat)));
        }

        private void GoToShop(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Category());
        }
    }
}
