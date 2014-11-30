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
        public LandingPageViewModel()
        {
            //int rand = new Random().Next(cat.SubCategories.Count - 1);
            //int newrand = new Random().Next(cat.SubCategories[rand].Products.Count - 1);
            //CurrentProduct = cat.SubCategories[rand].Products[newrand];
            CurrentProduct = new Item("Clothing One", 20.00);
        }

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
