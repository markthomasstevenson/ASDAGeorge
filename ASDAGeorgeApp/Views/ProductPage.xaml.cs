using ASDAGeorgeApp.Interface;
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
using System.Windows.Shapes;

namespace ASDAGeorgeApp
{
    /// <summary>
    /// Interaction logic for ProductPage.xaml
    /// </summary>
    public partial class ProductPage : UserControl, ISwitchable
    {
        public ProductPage()
        {
            InitializeComponent();
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
