using ASDAGeorgeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.ViewModels
{
    public class ProductPageViewModel : BaseViewModel
    {
        public ProductPageViewModel(Item item, SubCategory subCat, Category cat)
        {
            Product = item;
            ParentSub = subCat;
            ParentCat = cat;
        }

        private SubCategory _ParentSub;
        public SubCategory ParentSub
        {
            get { return _ParentSub; }
            set
            {
                if (_ParentSub != value)
                {
                    _ParentSub = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Category _ParentCat;
        public Category ParentCat
        {
            get { return _ParentCat; }
            set
            {
                if (_ParentCat != value)
                {
                    _ParentCat = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Item _Product;
        public Item Product
        {
            get { return _Product; }
            set
            {
                if(_Product != value)
                {
                    _Product = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
