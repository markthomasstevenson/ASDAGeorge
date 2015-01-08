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

            //new boys
            KinectTileButton boysButton = new KinectTileButton();
            boysButton.Label = "Boys";
            BitmapImage boyImage = new BitmapImage();
            boyImage.BeginInit();
            boyImage.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\boys.jpg");
            boyImage.EndInit();
            boysButton.Background = new ImageBrush(boyImage);
            boysButton.Name = "boys";
            boysButton.LabelBackground = (Brush)Application.Current.Resources["LabelBackground"];
            boysButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdfdfd"));
            boysButton.FontFamily = new FontFamily("Segoe UI Light");
            boysButton.Height = this.ProductContainer.Height;
            boysButton.Width = 448;
            boysButton.Opacity = 0.7;
            this.ProductContainer.Children.Add(boysButton);

            //new girls
            KinectTileButton girlsButton = new KinectTileButton();
            girlsButton.Label = "Girls";
            BitmapImage girlImage = new BitmapImage();
            girlImage.BeginInit();
            girlImage.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\girls.jpg");
            girlImage.EndInit();
            girlsButton.Background = new ImageBrush(girlImage);
            girlsButton.Name = "girls";
            girlsButton.LabelBackground = (Brush)Application.Current.Resources["LabelBackground"];
            girlsButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdfdfd"));
            girlsButton.FontFamily = new FontFamily("Segoe UI Light");
            girlsButton.Height = this.ProductContainer.Height;
            girlsButton.Width = 448;
            girlsButton.Opacity = 0.7;
            this.ProductContainer.Children.Add(girlsButton);
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
            if(catFound != null)
                Switcher.Switch(new SubCategory(catFound));
        }
    }
}
