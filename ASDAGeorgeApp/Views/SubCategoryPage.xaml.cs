using ASDAGeorgeApp.Models;
using ASDAGeorgeApp.Views.SubViews;
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
    /// Interaction logic for SubCategoryPage.xaml
    /// </summary>
    public partial class SubCategoryPage : UserControl
    {
        private string Cat = "";

        public SubCategoryPage(SubCategory subCategory)
        {
            InitializeComponent();
            this.backText.Text = subCategory.Parent;
            Cat = subCategory.Parent;
            DoTheProducts(subCategory);
        }

        private void DoTheProducts(SubCategory subCat)
        {
            this.ProductContainer.Children.Clear();

            foreach(Item item in subCat.Products)
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

                newTile.Name = subCat.Title + "_" + subCat.Parent;

                this.ProductContainer.Children.Add(newTile);
            }
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            KinectTileButton newButton = e.OriginalSource as KinectTileButton;
            SubCategory subCat = Collector.GetSubCategory(newButton.Name.Split(new char[] { '_' })[0], newButton.Name.Split(new char[] { '_' })[1]);
            
            Item itemFound = null;
            foreach(Item item in subCat.Products)
            {
                if (item.Title == newButton.Label.ToString())
                    itemFound = item;
            }

            Switcher.Switch(new ProductPage(itemFound, subCat));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new CategoryListPage(Cat));
        }
    }
}
