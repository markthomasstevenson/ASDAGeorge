using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.Models
{
    public abstract class Item : Model
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

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _Price;
        public double Price
        {
            get { return _Price; }
            set
            {
                if (_Price != value)
                {
                    _Price = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _WasPrice;
        public double WasPrice
        {
            get { return _WasPrice; }
            set
            {
                if (_WasPrice != value)
                {
                    _WasPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<DeliveryOptions> _DeliveryOptions;
        public List<DeliveryOptions> DeliveryOptions
        {
            get { return _DeliveryOptions; }
            set
            {
                if (_DeliveryOptions != value)
                {
                    _DeliveryOptions = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
