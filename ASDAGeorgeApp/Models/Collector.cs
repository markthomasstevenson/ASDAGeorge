using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;

namespace ASDAGeorgeApp.Models
{
    public static class Collector
    {
        public static  ObservableCollection<Category> Categories = null;

        public static ObservableCollection<Item> Wishlist = null;

        /// <summary>
        /// Get the most suitable speech recognizer
        /// </summary>
        /// <returns>The recogniser if found, null otherwise</returns>
        public static RecognizerInfo GetKinectRecognizer()
        {
            foreach(RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                    return recognizer;
            }

            return null;
        }

        /// <summary>
        /// Loads the information needed to run the application
        /// </summary>
        public static void LoadInformation()
        {
            Categories = FileReader.ReadAllFiles();
            Wishlist = new ObservableCollection<Item>();
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

        /// <summary>
        /// Gets the Item from the unique ID given
        /// </summary>
        /// <param name="UID">Unique ID to find</param>
        /// <returns>The found item, else null</returns>
        public static Item GetItem(string UID)
        {
            foreach(Category cat in Categories)
            {
                foreach(SubCategory sub in cat.SubCategories)
                {
                    foreach(Item item in sub.Products)
                    {
                        if (item.UniqueID == UID)
                            return item;
                    }
                }
            }
            return null;
        }
    }
}
