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
        }

        public Item CurrentProduct
        {
            get
            {
                LandingPageViewModel model = DataContext as LandingPageViewModel;
                return model.CurrentProduct;
            }
        }

        private void MainLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Switcher.Switch(new CategoryListPage());
        }
    }
}
