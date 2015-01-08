using ASDAGeorgeApp.Interface;
using ASDAGeorgeApp.Models;
using ASDAGeorgeApp.ViewModels;
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
    /// Interaction logic for ProductList.xaml
    /// </summary>
    public partial class ProductList : UserControl, ISwitchable
    {
        public ProductList(Models.SubCategory subCat, Models.Category category)
        {
            InitializeComponent();

            this.DataContext = new ProductListViewModel(subCat, category);

            this.TrailText.Text = (this.DataContext as ProductListViewModel).trailText;

            Loaded += (sender, args) =>
                    {
                        DisplayItems();
                    };
        }

        public ProductList(string newIn, Models.Category category)
        {
            InitializeComponent();

            this.DataContext = new ProductListViewModel(newIn, category);

            this.TrailText.Text = (this.DataContext as ProductListViewModel).trailText;

            Loaded += (sender, args) =>
            {
                DisplayItems();
            };
        }

        private void DisplayItems()
        {
            // display all the categorys
            this.ProductContainer.Children.Clear();
            foreach(Item item in ((ProductListViewModel)this.DataContext).Items)
            {
                // new button
                KinectTileButton newTile = new KinectTileButton();
                // add title
                newTile.Label = "£" + item.Price.ToString() + "\r\n" + item.Title;
                // create background image
                BitmapImage bitImg = new BitmapImage();
                bitImg.BeginInit();
                if (File.Exists(item.ProductImage + "_list.png"))
                    bitImg.UriSource = new Uri(item.ProductImage + "_list.png");
                else if (File.Exists(item.ProductImage + "_list.jpg"))
                    bitImg.UriSource = new Uri(item.ProductImage + "_list.jpg");
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
            this.ScrollViewer.Visibility = Visibility.Visible;
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
            ProductListViewModel model = (ProductListViewModel)this.DataContext;
            Switcher.Switch(new Product(item, model.ParentSub, model.ParentCategoryInstance));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ProductListViewModel model = (ProductListViewModel)this.DataContext;
            Models.Category cat = model.ParentCategoryInstance;
            Switcher.Switch(new SubCategory(cat));
        }
    }
}
