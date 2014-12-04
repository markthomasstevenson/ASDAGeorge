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
using System.Windows.Shapes;

namespace ASDAGeorgeApp.Views
{
    /// <summary>
    /// Interaction logic for ProductPage.xaml
    /// </summary>
    public partial class ProductPage : UserControl, ISwitchable
    {
        private SubCategory SubCat = null;

        public ProductPage(Item item, SubCategory subCat = null)
        {
            InitializeComponent();
            DataContext = new ProductPageViewModel(item);
            if(subCat == null)
            {
                subCat = Collector.GetSubCategory(item.ParentSub, item.ParentCat);
            }
            SubCat = subCat;
            this.BackToString.Text = subCat.Parent + " > " + subCat.Title;
            this.DescriptionText.Text = item.Description.Replace("[BULLET]", "•");
            this.DeliveryText.Text = item.DeliveryMethods.Replace("[POUND]", "£");

            if(Collector.Wishlist.Contains(item))
            {
                this.Wishlist.Visibility = Visibility.Hidden;
                this.WishlistRemove.Visibility = Visibility.Visible;
            }
        }

        public Item Product
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new SubCategoryPage(SubCat));
        }

        private void Wishlist_Click(object sender, RoutedEventArgs e)
        {
            Collector.Wishlist.Add(Product);
            Switcher.Switch(new SubCategoryPage());
        }

        private void WishlistRemove_Click(object sender, RoutedEventArgs e)
        {
            Collector.Wishlist.Remove(Product);
            Switcher.Switch(new SubCategoryPage());
        }
    }
}
