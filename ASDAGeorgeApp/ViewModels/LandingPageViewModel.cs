using ASDAGeorgeApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.ViewModels
{
    public class LandingPageViewModel : BaseViewModel
    {
        private Item _CurrentProduct;
        public Item CurrentProduct
        {
            get { return _CurrentProduct; }
            set
            {
                if (_CurrentProduct != value)
                {
                    _CurrentProduct = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<Item> _AllProducts;
        public ObservableCollection<Item> AllProducts
        {
            get { return _AllProducts; }
            set
            {
                if (_AllProducts != value)
                {
                    _AllProducts = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
