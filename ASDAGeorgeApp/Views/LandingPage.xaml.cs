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
    /// Interaction logic for LandingPage.xaml
    /// </summary>
    public partial class LandingPage : UserControl
    {
        public LandingPage()
        {
            InitializeComponent();

            DataContext = new LandingPageViewModel();

            this.TitleTextOne.Text = "20% off all";
            this.TitleTextTwo.Text = CurrentProduct.ParentCat + " " + CurrentProduct.ParentSub;
            ((TextBlock)((Border)this.ShopLink.Content).Child).Text = "Shop " + CurrentProduct.ParentSub;
        }

        public Item CurrentProduct
        {
            get
            {
                LandingPageViewModel model = DataContext as LandingPageViewModel;
                return model.CurrentProduct;
            }
        }

        private void ShopLink_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ProductList(Collector.GetSubCategory(CurrentProduct.ParentSub, CurrentProduct.ParentCat), Collector.GetCategory(CurrentProduct.ParentCat)));
        }
    }
}
