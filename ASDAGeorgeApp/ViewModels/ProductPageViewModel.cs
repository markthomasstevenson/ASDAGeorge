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
