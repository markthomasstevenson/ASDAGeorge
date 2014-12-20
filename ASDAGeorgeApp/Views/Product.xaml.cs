using ASDAGeorgeApp.Interface;
using ASDAGeorgeApp.Models;
using ASDAGeorgeApp.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : UserControl, ISwitchable
    {
        public Product(Item item, Models.SubCategory subCat, Models.Category cat)
        {
            InitializeComponent();
            DataContext = new ProductPageViewModel(item, subCat, cat);

            // set the text for the page
            ((TextBlock)((Border)this.Add2Wishlist.Content).Child).Text = "£" + item.Price + "\r\n" + "+ Wishlist";
            this.TitleText.Text = item.Title;

            //set visibility of the buttons
            if (Collector.Wishlist.Contains(item))
                HideWishlistButton();
        }

        public Item CurrentProduct
        {
            get
            {
                ProductPageViewModel model = DataContext as ProductPageViewModel;
                return model.Product;
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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ProductPageViewModel model = (ProductPageViewModel)this.DataContext;
            Switcher.Switch(new ProductList(model.ParentSub, model.ParentCat));
        }

        private void Add2Wishlist_Click(object sender, RoutedEventArgs e)
        {
            ProductPageViewModel model = (ProductPageViewModel)this.DataContext;
            Collector.Wishlist.Add(model.Product);
            HideWishlistButton();
        }

        private void HideWishlistButton()
        {
            this.Add2Wishlist.Visibility = Visibility.Hidden;
            this.IsAdded2Wishlist.Visibility = Visibility.Visible;
        }

        private void ShowWishlistButton()
        {
            this.Add2Wishlist.Visibility = Visibility.Visible;
            this.IsAdded2Wishlist.Visibility = Visibility.Hidden;
        }

        private void IsAdded2Wishlist_Click(object sender, RoutedEventArgs e)
        {
            ProductPageViewModel model = (ProductPageViewModel)this.DataContext;
            Collector.Wishlist.Remove(model.Product);
            ShowWishlistButton();
        }
    }
}
