using ASDAGeorgeApp.Models;
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

namespace ASDAGeorgeApp.Views.SubViews
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
            Loaded += MainMenu_Loaded;
        }

        void MainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            KinectTextButton button;

            switch (ParentPage)
            {
                case "shop":
                    button = this.Shop;
                    break;
                case "account":
                    button = this.Account;
                    break;
                case "wishlist":
                    button = this.Wishlist;
                    break;
                case "search":
                    button = this.Search;
                    break;
                default:
                    button = null;
                    break;
            }

            Brush newBrush = (Brush)Application.Current.Resources["MenuGreen"];

            if (button != null)
            {
                try
                {
                    ((Border)button.Content).Background = newBrush;
                }
                catch
                {
                    // do nothing
                }
            }
        }

        public static readonly DependencyProperty ParentPageProperty= 
            DependencyProperty.Register(
            "ParentPage", typeof(string), typeof(MainMenu)
            );

        public string ParentPage
        {
            get { return (string)GetValue(ParentPageProperty); }
            set { SetValue(ParentPageProperty, value); }
        }

        private void GoToPage(UserControl userControl)
        {
            Switcher.Switch(userControl);
        }

        private void Shop_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(new Category());
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(new Account());
        }

        private void Wishlist_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(new Wishlist());
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(new Search());
        }
    }
}
