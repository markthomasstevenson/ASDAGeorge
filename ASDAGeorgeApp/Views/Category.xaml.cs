using ASDAGeorgeApp.Interface;
using ASDAGeorgeApp.Models;
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
    /// Interaction logic for Category.xaml
    /// </summary>
    public partial class Category : UserControl, ISwitchable
    {
        public Category()
        {
            InitializeComponent();

            DisplayItems();
        }

        private void DisplayItems()
        {
            // display all the categorys
            this.ProductContainer.Children.Clear();
            foreach(Models.Category category in Collector.Categories)
            {
                // new button
                KinectTileButton newTile = new KinectTileButton();
                // add title
                newTile.Label = category.Title;
                // create background image
                BitmapImage bitImg = new BitmapImage();
                bitImg.BeginInit();
                if (category.Title.ToLower().Contains("women"))
                    bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\womens.jpg");
                else if (category.Title.ToLower().Contains("men"))
                    bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\mens.jpg");
                bitImg.EndInit();
                // attach background image
                newTile.Background = new ImageBrush(bitImg);
                // add name
                newTile.Name = category.Title;
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

                this.ProductContainer.Children.Add(newTile);
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
            Models.Category catFound = Collector.GetCategory(newButton.Name);
            Switcher.Switch(new SubCategory(catFound));
        }
    }
}
