using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.Models
{
    public class Featured : Model
    {
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

        private string _Tagline;
        public string Tagline
        {
            get { return _Tagline; }
            set
            {
                if (_Tagline != value)
                {
                    _Tagline = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Category _Category;
        public Category Category
        {
            get { return _Category; }
            set
            {
                if (_Category != value)
                {
                    _Category = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Item _SampleItem;
        public Item SampleItem
        {
            get { return _SampleItem; }
            set
            {
                if (_SampleItem != value)
                {
                    _SampleItem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Brand;
        public string Brand
        {
            get { return _Brand; }
            set
            {
                if (_Brand != value)
                {
                    _Brand = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
