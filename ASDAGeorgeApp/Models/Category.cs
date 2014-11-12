using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.Models
{
    public class Category : Model
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

        private List<Category> _SubCategories;
        public List<Category> SubCategories
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

        private string _TagLine;
        public string TagLine
        {
            get { return _TagLine; }
            set
            {
                if (_TagLine != value)
                {
                    _TagLine = value;
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
    }
}
