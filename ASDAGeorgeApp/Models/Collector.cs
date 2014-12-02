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

        public static ObservableCollection<Item> Wishlist = new ObservableCollection<Item>();

        /// <summary>
        /// Loads the information needed to run the application
        /// </summary>
        public static void LoadInformation()
        {
            Categories = FileReader.ReadAllFiles();
        }

        /// <summary>
        /// Gets a sub category from a name given
        /// </summary>
        /// <param name="name">Name of sub category to find</param>
        /// <returns>Subcategory found, else null</returns>
        public static SubCategory GetSubCategory(string name, string category = "")
        {
            if (category != "")
            {
                foreach (SubCategory subCat in GetCategory(category).SubCategories)
                {
                    if (subCat.Title.ToUpper() == name.ToUpper())
                        return subCat;
                }
            }
            else
            {
                foreach (Category cat in Categories)
                {
                    foreach (SubCategory subCat in cat.SubCategories)
                    {
                        if (subCat.Title.ToUpper() == name.ToUpper())
                            return subCat;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a Category from a name given
        /// </summary>
        /// <param name="name">Name of category to find</param>
        /// <returns>Category found, else null</returns>
        public static Category GetCategory(string name)
        {
            foreach(Category cat in Categories)
            {
                if (cat.Title.ToUpper() == name.ToUpper())
                    return cat;
            }

            return null;
        }
    }
}
