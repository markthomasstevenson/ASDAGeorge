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
        public ProductPage()
        {
            InitializeComponent();
            DataContext = new ProductPageViewModel();
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

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Switcher.Switch(new CategoryListPage());
        }
    }
}
