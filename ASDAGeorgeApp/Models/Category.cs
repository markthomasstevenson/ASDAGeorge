using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.Models
{
    public class Category : Model
    {
        public Category()
        {
            SubCategories = new ObservableCollection<SubCategory>();
        }

        public Category(string title, ObservableCollection<SubCategory> subCategories = null)
        {
            Title = title;
            if (subCategories == null)
                subCategories = new ObservableCollection<SubCategory>();
            SubCategories = subCategories;
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

        private ObservableCollection<SubCategory> _SubCategories;
        public ObservableCollection<SubCategory> SubCategories
        {
            get { return _SubCategories; }
            set
            {
                if (_SubCategories != value)
                {
                    _SubCategories = value;
                    NotifyPropertyChanged();
                    if (_SubCategories.Count > 0)
                    {
                        int rand = new Random().Next(_SubCategories.Count - 1);
                        int newRand = new Random().Next(_SubCategories[rand].Products.Count - 1);
                        SamplePicture = _SubCategories[rand].Products[newRand].ProductImage;
                    }
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

        private ObservableCollection<Filter> _Filters;
        public ObservableCollection<Filter> Filters
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

        public ObservableCollection<Item> Products
        {
            get { return GetAllProducts(); }
        }

        private ObservableCollection<Item> GetAllProducts()
        {
            ObservableCollection<Item> result = new ObservableCollection<Item>();
            if (SubCategories == null || SubCategories.Count < 1)
                return result;

            foreach(SubCategory subCat in SubCategories)
            {
                foreach(Item item in subCat.Products)
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
