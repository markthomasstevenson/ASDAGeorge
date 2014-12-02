using ASDAGeorgeApp.Interface;
using ASDAGeorgeApp.Models;
using ASDAGeorgeApp.Views;
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
using System.Windows.Shapes;

namespace ASDAGeorgeApp.Views
{
    /// <summary>
    /// Interaction logic for CategoryListPage.xaml
    /// </summary>
    public partial class CategoryListPage : UserControl, ISwitchable
    {
        public CategoryListPage()
        {
            InitializeComponent();
            MainViewer.Visibility = Visibility.Hidden;
        }

        public CategoryListPage(string category)
        {
            InitializeComponent();
            DoTheCategory(category);
        }

        private void MainMenuClick(object sender, RoutedEventArgs e)
        {
            try
            {
                KinectTextButton newButton = e.OriginalSource as KinectTextButton;
                Image newImage = (Image)newButton.Content;
                Switcher.Switch(new LandingPage());
            }
            catch
            {
                KinectTextButton newButton = e.OriginalSource as KinectTextButton;
                DoTheCategory(((TextBlock)newButton.Content).Text);
            }
        }

        public void DoTheCategory(string category)
        {
            Category newCat = Collector.GetCategory(category);
            if(newCat != null)
            {
                this.ProductPanel.Children.Clear();
                foreach(SubCategory subCat in newCat.SubCategories)
                {
                    KinectTileButton newTile = new KinectTileButton();
                    newTile.Label = subCat.Title;
                    BitmapImage bitImg = new BitmapImage();
                    bitImg.BeginInit();
                    if (File.Exists(subCat.Products[new Random().Next(subCat.Products.Count - 1)].ProductImage))
                        bitImg.UriSource = new Uri(subCat.Products[new Random().Next(subCat.Products.Count - 1)].ProductImage);
                    else
                        bitImg.UriSource = new Uri(@"D:\Documents\Dropbox\University\Third Year\Advanced Human Computer Interaction\Coursework 2\ASDAGeorge\ASDAGeorgeApp\image\marvel_tee_psd.png");
                    bitImg.EndInit();
                    newTile.Background = new ImageBrush(bitImg);

                    newTile.Name = subCat.Parent;

                    this.ProductPanel.Children.Add(newTile);
                }

                this.MainViewer.Visibility = Visibility.Visible;
            }
        }

        public void UtiliseState(object state)
        {
            throw new NotImplementedException();
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            KinectTileButton button = e.OriginalSource as KinectTileButton;
            SubCategory subCat = Collector.GetSubCategory(button.Label.ToString(), button.Name);
            Switcher.Switch(new SubCategoryPage(subCat));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LandingPage());
        }
    }
}
