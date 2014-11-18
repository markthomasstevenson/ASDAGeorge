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
    /// Interaction logic for MainMenuControl.xaml
    /// </summary>
    public partial class MainMenuControl : UserControl
    {
        public MainMenuControl()
        {
            InitializeComponent();
        }

        private void MainLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Switcher.Switch(new LandingPage());
        }

        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            KinectTextButton newButton = e.OriginalSource as KinectTextButton;
            MessageBox.Show("Lol Lol Lol. You clicked " + ((TextBlock)newButton.Content).Text.ToString());
        }
    }
}
