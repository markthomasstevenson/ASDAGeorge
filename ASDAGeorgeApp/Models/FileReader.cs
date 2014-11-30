using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ASDAGeorgeApp.Models
{
    public static class FileReader
    {
        public enum FileChooser
	    {
	        WomenFile = 0,
            MenFile
	    }

        public static List<KeyValuePair<FileChooser, string>> ReturnFiles()
        {
            List<KeyValuePair<FileChooser, string>> fileList = new List<KeyValuePair<FileChooser,string>>();
            
            fileList.Add(new KeyValuePair<FileChooser,string>(FileChooser.WomenFile, System.AppDomain.CurrentDomain.BaseDirectory + "Resources/women.xml"));
            fileList.Add(new KeyValuePair<FileChooser,string>(FileChooser.MenFile, System.AppDomain.CurrentDomain.BaseDirectory + "Resources/men.xml"));

            return fileList;
        }

        public static ObservableCollection<Category> ReadFile(FileChooser file)
        {
            string fileString = ReturnFiles()[(int)file].Value;
            XDocument doc = XDocument.Load(fileString);
            //return LoadXMLFromFile(doc.Descendants("table").Elements("row"));
            return new ObservableCollection<Category>();
        }

        public static ObservableCollection<Category> ReadAllFiles()
        {
            ObservableCollection<Category> categories = new ObservableCollection<Category>();
            ObservableCollection<Category> currCat = new ObservableCollection<Category>();

            List<KeyValuePair<FileChooser, string>> files = ReturnFiles();
            foreach(KeyValuePair<FileChooser, string> file in files)
            {
                XDocument doc = XDocument.Load(file.Value);
                IEnumerable<XElement> enumer = doc.Descendants("Workbook");
                IEnumerable<XElement> enum1 = enumer.Elements("Worksheet");
                IEnumerable<XElement> enum2 = enum1.Elements("Table");
                IEnumerable<XElement> enum3 = enum2.Elements("Row");
                currCat = LoadXMLFromFile(enum3);
                
                foreach(Category cat in currCat)
                {
                    categories.Add(cat);
                }
            }

            return categories;
        }

        private static ObservableCollection<Category> LoadXMLFromFile(IEnumerable<XElement> enumerable)
        {
            /* Create list of children */
            ObservableCollection<Category> categories = new ObservableCollection<Category>();

            Category currCat = null;
            SubCategory currSub = null;
            Item currItem = null;

            foreach (XElement x in enumerable)
            {
                int cat = 0;
                int subCat = 1;
                int image = 3;
                int title = 4;
                int price = 6;
                int desc = 7;
                int size = 8;
                int review = 9;
                int delivery = 10;

                /* Skip row if not data */
                List<XElement> data = x.Elements("data").ToList();
                if( data[0].Value == "Category" ||
                    data[0].Value == "Approximate Knowledge Data Request")
                    continue;

                string ssIndex;
                if(x.Element("cell").Attribute("ss:Index") == null)
                    ssIndex = "";
                else
                    ssIndex = x.Element("cell").Attribute("ss:Index").Value;

                if (ssIndex == "")
                {
                    if(currCat != null)
                    {
                        currSub.Products.Add(currItem);
                        currCat.SubCategories.Add(currSub);
                        categories.Add(currCat);
                    }
                    currCat = new Category(data[cat].Value);
                    currSub = new SubCategory(data[subCat].Value);
                    currItem = new Item(data[title].Value);
                }
                else if(ssIndex == "2")
                {
                    subCat--; image--; title--; price--; desc--; size--; review--; delivery--;
                    if(currSub != null)
                    {
                        currSub.Products.Add(currItem);
                        currCat.SubCategories.Add(currSub);
                    }
                    currSub = new SubCategory(data[subCat].Value);
                    currItem = new Item(data[title].Value);
                }
                else if(ssIndex == "3")
                {
                    image -= 2; title -= 2; price -= 2; desc -= 2; size -= 2; review -= 2; delivery -= 2;
                    if (currItem != null)
                        currSub.Products.Add(currItem);
                    currItem = new Item(data[title].Value);
                }

                currItem.Price = int.Parse(data[price].Value);
                currItem.Description = data[desc].Value;
                currItem.SizeVariants = data[size].Value;
                int reviewData;
                if (data[review].Value == "No Review")
                    reviewData = -1;
                else
                    reviewData = int.Parse(data[review].Value);
                currItem.StarRating = reviewData;
                currItem.DeliveryMethods = data[delivery].Value;
            }

            /* Return the list */
            return categories;
        }
    }
}
