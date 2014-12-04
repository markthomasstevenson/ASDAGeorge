using ASDAGeorgeApp.Models;
using ASDAGeorgeApp.Views.SubViews;
using Microsoft.Kinect.Toolkit.Controls;
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

namespace ASDAGeorgeApp.Views
{
    /// <summary>
    /// Interaction logic for SubCategoryPage.xaml
    /// </summary>
    public partial class SubCategoryPage : UserControl
    {
        private SubCategory SubCat = null;

        public SubCategoryPage(SubCategory subCategory)
        {
            InitializeComponent();
            this.backText.Text = subCategory.Parent;
            SubCat = subCategory;
            DisplayItems(subCategory.Products);
        }

        public SubCategoryPage()
        {
            InitializeComponent();
            this.backText.Text = "Main Menu";
            DoTheWishlist();
        }

        private void DoTheWishlist()
        {
            if(Collector.Wishlist.Count == 0)
            {
                this.NothingText.Visibility = Visibility.Visible;
                this.ScrollViewer.Visibility = Visibility.Hidden;
            }
            else
            {
                DisplayItems(Collector.Wishlist);
            }
        }

        private void DisplayItems(ObservableCollection<Item> items)
        {
            this.ProductContainer.Children.Clear();
            foreach (Item item in items)
            {
                KinectTileButton newTile = new KinectTileButton();
                newTile.Label = item.Title;
                BitmapImage bitImg = new BitmapImage();
                bitImg.BeginInit();
                if (File.Exists(item.ProductImage))
                    bitImg.UriSource = new Uri(item.ProductImage);
                else
                    bitImg.UriSource = new Uri(@"D:\Documents\Dropbox\University\Third Year\Advanced Human Computer Interaction\Coursework 2\ASDAGeorge\ASDAGeorgeApp\image\marvel_tee_psd.png");
                bitImg.EndInit();
                newTile.Background = new ImageBrush(bitImg);

                newTile.Name = item.UniqueID;

                double ratio = bitImg.Width / bitImg.Height;

                newTile.Height = 400;

                newTile.Width = 200;

                this.ProductContainer.Children.Add(newTile);
            }
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            KinectTileButton newButton = e.OriginalSource as KinectTileButton;
            Item itemFound = Collector.GetItem(newButton.Name);
            Switcher.Switch(new ProductPage(itemFound, SubCat));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.backText.Text == "Main Menu")
                Switcher.Switch(new CategoryListPage());
            else
                Switcher.Switch(new CategoryListPage(SubCat.Parent));
        }
    }
}
