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
    /// Interaction logic for SubCategory.xaml
    /// </summary>
    public partial class SubCategory : UserControl, ISwitchable
    {
        private Models.Category SelectedCategory;

        public SubCategory(Models.Category category)
        {
            InitializeComponent();

            this.TrailText.Text = category.Title.ToUpper();

            SelectedCategory = category;
            DoTheCategory();
        }

        public void DoTheCategory()
        {
            if (SelectedCategory != null)
            {
                this.ProductContainer.Children.Clear();
                bool first = true;

                foreach (Models.SubCategory subCat in SelectedCategory.SubCategories)
                {
                    // create tile button
                    KinectTileButton newTile = new KinectTileButton();
                    // add label
                    newTile.Label = subCat.Title;
                    // create image
                    BitmapImage bitImg = new BitmapImage();
                    bitImg.BeginInit();
                    if (SelectedCategory.Title.ToLower().Contains("women"))
                    {
                        if (subCat.Title.ToLower().Contains("coats"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\WomensCategories\\womens_coats.jpg");
                        else if (subCat.Title.ToLower().Contains("dresses"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\WomensCategories\\womens_dresses.jpg");
                        else if (subCat.Title.ToLower().Contains("skirts"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\WomensCategories\\womens_skirts.jpg");
                        else if (subCat.Title.ToLower().Contains("tops"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\WomensCategories\\womens_tops.jpg");
                        else if (subCat.Title.ToLower().Contains("jeans"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\WomensCategories\\womens_jeans.jpg");
                        else
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\ProductImages\\tempImage.png");
                    }
                    else
                    {

                        if (subCat.Title.ToLower().Contains("coats"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\MensCategories\\mens_coats.jpg");
                        else if (subCat.Title.ToLower().Contains("jeans"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\MensCategories\\mens_jeans.jpg");
                        else if (subCat.Title.ToLower().Contains("suits"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\MensCategories\\mens_suits.jpg");
                        else if (subCat.Title.ToLower().Contains("shirts"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\MensCategories\\mens_shirts.jpg");
                        else if (subCat.Title.ToLower().Contains("shorts"))
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\MensCategories\\mens_swimwear.jpg");
                        else
                            bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\ProductImages\\tempImage.png");
                    }
                    bitImg.EndInit();
                    // add image
                    newTile.Background = new ImageBrush(bitImg);
                    // add name
                    newTile.Name = subCat.Title;
                    // set label background
                    newTile.LabelBackground = (Brush)Application.Current.Resources["LabelBackground"];
                    // set foreground
                    newTile.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdfdfd"));
                    //set font family
                    newTile.FontFamily = new FontFamily("Segoe UI Light");


                    // set height, width, and margin
                    if(first)
                    {
                        newTile.Height = 448;
                        newTile.Width = 448;
                        first = false;
                    }
                    else
                    {
                        newTile.Height = 220;
                        newTile.Width = 220;
                        newTile.Margin = new Thickness(8, 0, 0, 4);
                    }

                    // add to container
                    this.ProductContainer.Children.Add(newTile);
                }

                this.ProductContainer.Visibility = Visibility.Visible;
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
            Switcher.Switch(new ProductList(Collector.GetSubCategory(newButton.Name, SelectedCategory.Title), SelectedCategory));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Category());
        }
    }
}
