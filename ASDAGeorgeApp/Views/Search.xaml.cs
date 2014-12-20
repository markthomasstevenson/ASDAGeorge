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
using System.Windows.Threading;

namespace ASDAGeorgeApp.Views
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl, ISwitchable
    {
        private DispatcherTimer Timer;

        private string lastSearch = "";

        public Search()
        {
            InitializeComponent();

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(1000);
            Timer.Tick += CheckForSearchResults;

            Loaded += (sender, args) =>
            {
                BitmapImage bitImg = new BitmapImage();
                bitImg.BeginInit();
                int rand = new Random().Next(0, 5);
                if (rand == 0)
                    bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\SearchImage.jpg");
                else if (rand == 1)
                    bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\SearchImageTwo.jpg");
                else if (rand == 2)
                    bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\SearchImageThree.jpg");
                else if (rand == 3)
                    bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\SearchImageFour.jpg");
                else
                    bitImg.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\CustomImages\\SearchImageFive.jpg");
                bitImg.EndInit();
                // attach background image
                this.SearchImage.Source = bitImg;

                DisplayItems(false);
                Timer.Start();

            };
        }

        void CheckForSearchResults(object sender, EventArgs e)
        {
            if (Collector.lastSearchTerm != "")
            {
                if (Collector.lastSearchTerm != lastSearch)
                {
                    lastSearch = Collector.lastSearchTerm;
                    DisplayItems(true);
                }
            }
            else
                DisplayItems(false);
        }

        private void DisplayItems(bool show)
        {
            if (show)
            {
                this.CountText.Text = Collector.Search.Count.ToString() + " RESULTS FOR";
                this.SearchTermText.Text = Collector.lastSearchTerm;

                //show
                this.ProductContainer.Children.Clear();
                foreach (Item item in Collector.Search)
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
            else
            {
                this.CountText.Text = "";
                this.SearchTermText.Text = "SEARCH";

                if(Collector.IsListening)
                {
                    this.IsListeningBox.Visibility = Visibility.Visible;
                    this.IsNotListeningBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.IsListeningBox.Visibility = Visibility.Hidden;
                    this.IsNotListeningBox.Visibility = Visibility.Visible;
                }

                //hide
                this.ProductContainer.Children.Clear();
                this.HiddenStuff.Visibility = Visibility.Visible;
                this.ScrollViewer.Visibility = Visibility.Hidden;
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
            KinectTileButton buttonClicked = e.OriginalSource as KinectTileButton;
            Item item = Collector.GetItem(buttonClicked.Name);

            Switcher.Switch(new Product(item, Collector.GetSubCategory(item.ParentSub, item.ParentCat), Collector.GetCategory(item.ParentCat)));
        }
    }
}
