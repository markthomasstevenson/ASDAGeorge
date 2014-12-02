using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.Models
{
    public class SubCategory : Model
    {
        public SubCategory()
        {
            Products = new ObservableCollection<Item>();
        }

        public SubCategory(string title)
        {
            Title = title;
            Products = new ObservableCollection<Item>();
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Parent;
        public string Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent != value)
                {
                    _Parent = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<Item> _Products;
        public ObservableCollection<Item> Products
        {
            get { return _Products; }
            set
            {
                if (_Products != value)
                {
                    _Products = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
