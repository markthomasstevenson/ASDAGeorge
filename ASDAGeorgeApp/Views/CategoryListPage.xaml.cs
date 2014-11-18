using ASDAGeorgeApp.Interface;
using ASDAGeorgeApp.Models;
using ASDAGeorgeApp.Views;
using Microsoft.Kinect.Toolkit.Controls;
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

namespace ASDAGeorgeApp
{
    /// <summary>
    /// Interaction logic for CategoryListPage.xaml
    /// </summary>
    public partial class CategoryListPage : UserControl, ISwitchable
    {
        public CategoryListPage()
        {
            InitializeComponent();
        }

        public void UtiliseState(object state)
        {
            throw new NotImplementedException();
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ProductPage());
        }
    }
}
