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

                KinectTileButton newIn = new KinectTileButton();
                newIn.Label = "New In";
                newIn.Width = 448;
                newIn.Height = 448;
                BitmapImage newInImg = new BitmapImage();
                newInImg.BeginInit();
                if (SelectedCategory.Title.ToLower().Contains("women"))
                    newInImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\WomensCategories\\womens_new_in.jpg");
                else
                    newInImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\MensCategories\\mens_new_in.jpg");

                newInImg.EndInit();
                // add image
                newIn.Background = new ImageBrush(newInImg);
                // add name
                newIn.Name = "NewIn";
                // set label background
                newIn.LabelBackground = (Brush)Application.Current.Resources["LabelBackground"];
                // set foreground
                newIn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdfdfd"));
                //set font family
                newIn.FontFamily = new FontFamily("Segoe UI Light");

                this.ProductContainer.Children.Add(newIn);

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
                    newTile.Height = 220;
                    newTile.Width = 220;
                    newTile.Margin = new Thickness(8, 0, 0, 4);

                    // add to container
                    this.ProductContainer.Children.Add(newTile);
                }

                if (SelectedCategory.Title.ToLower().Contains("mens"))
                {
                    List<string> files = new List<string>();
                    files.Add("mens_accessories");
                    files.Add("mens_jumpers");
                    files.Add("mens_nightwear");
                    files.Add("mens_onesie");
                    files.Add("mens_socks");
                    files.Add("mens_sweatshirts");
                    files.Add("mens_ties");
                    files.Add("mens_trousers");
                    files.Add("mens_tshirts");
                    files.Add("mens_underwear");

                    string catName = "Mens";
                    NewInDisplay(files, catName);
                }
                else
                {

                    List<string> files = new List<string>();
                    files.Add("womens_accessories");
                    files.Add("womens_hats");
                    files.Add("womens_jumpers");
                    files.Add("womens_jumpsuit");
                    files.Add("womens_leggings");
                    files.Add("womens_maternity");
                    files.Add("womens_nightwear");
                    files.Add("womens_onesie");
                    files.Add("womens_plus_size");
                    files.Add("womens_shirts");
                    files.Add("womens_swimwear");
                    files.Add("womens_trousers");
                    files.Add("womens_tunics");

                    string catName = "Womens";
                    NewInDisplay(files, catName);
                }

                this.ProductContainer.Visibility = Visibility.Visible;
            }
        }

        private void NewInDisplay(List<string> files, string catName)
        {

            foreach (string file in files)
            {
                // create tile button
                KinectTileButton newTile = new KinectTileButton();
                // add label
                string label = file.Replace("mens_", "").Replace("womens_", "");
                label = char.ToUpper(label[0]) + label.Substring(1);
                newTile.Label = label;
                // create image
                BitmapImage bitImg = new BitmapImage();
                bitImg.BeginInit();
                bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\" + catName + "Categories\\" + file + ".jpg");
                bitImg.EndInit();
                // add image
                newTile.Background = new ImageBrush(bitImg);
                // add name
                newTile.Name = label;
                // set label background
                newTile.LabelBackground = (Brush)Application.Current.Resources["LabelBackground"];
                // set foreground
                newTile.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdfdfd"));
                //set font family
                newTile.FontFamily = new FontFamily("Segoe UI Light");
                newTile.Opacity = 0.6;

                // set height, width, and margin
                newTile.Height = 220;
                newTile.Width = 220;
                newTile.Margin = new Thickness(8, 0, 0, 4);

                // add to container
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
            if (newButton.Name == "NewIn")
                Switcher.Switch(new ProductList("NewIn", SelectedCategory));
            else
            {
                Models.SubCategory subCat = Collector.GetSubCategory(newButton.Name, SelectedCategory.Title);
                if(subCat != null)
                    Switcher.Switch(new ProductList(subCat, SelectedCategory));
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Category());
        }
    }
}
