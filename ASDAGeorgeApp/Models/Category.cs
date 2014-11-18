using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.Models
{
    public class Category : Model
    {
        public Category(string title, List<SubCategory> subCategories)
        {
            Title = title;
            SubCategories = subCategories;
            int rand = new Random().Next(subCategories.Count - 1);
            int newRand = new Random().Next(subCategories[rand].Products.Count - 1);
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

        private Category _Parent;
        public Category Parent
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

        private List<SubCategory> _SubCategories;
        public List<SubCategory> SubCategories
        {
            get { return _SubCategories; }
            set
            {
                if (_SubCategories != value)
                {
                    _SubCategories = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _SamplePicture;
        public string SamplePicture
        {
            get { return _SamplePicture; }
            set
            {
                if (_SamplePicture != value)
                {
                    _SamplePicture = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<Filter> _Filters;
        public List<Filter> Filters
        {
            get { return _Filters; }
            set
            {
                if (_Filters != value)
                {
                    _Filters = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _PromotionalBanner;
        public string PromotionalBanner
        {
            get { return _PromotionalBanner; }
            set
            {
                if (_PromotionalBanner != value)
                {
                    _PromotionalBanner = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _Hidden;
        public bool Hidden
        {
            get { return _Hidden; }
            set
            {
                if (_Hidden != value)
                {
                    _Hidden = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<Item> _Products;
        public List<Item> Products
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
