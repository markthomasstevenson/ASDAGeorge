using ASDAGeorgeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.ViewModels
{
    public class FeaturedViewModel : BaseViewModel
    {
        private Featured _Feature;
        public Featured Feature
        {
            get { return _Feature; }
            set
            {
                if (_Feature != value)
                {
                    _Feature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
