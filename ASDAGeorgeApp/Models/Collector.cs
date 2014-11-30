using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.Models
{
    public static class Collector
    {
        public static  ObservableCollection<Category> Categories = null;

        public static void LoadInformation()
        {
            Categories = FileReader.ReadAllFiles();
        }
    }
}
