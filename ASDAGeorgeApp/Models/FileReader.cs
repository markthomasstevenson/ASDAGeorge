using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            
            fileList.Add(new KeyValuePair<FileChooser,string>(FileChooser.WomenFile, System.AppDomain.CurrentDomain.BaseDirectory + "Resources/women.csv"));
            fileList.Add(new KeyValuePair<FileChooser,string>(FileChooser.MenFile, System.AppDomain.CurrentDomain.BaseDirectory + "Resources/men.csv"));

            return fileList;
        }

        public static ObservableCollection<Category> ReadAllFiles()
        {
            ObservableCollection<Category> categories = new ObservableCollection<Category>();

            List<KeyValuePair<FileChooser, string>> files = ReturnFiles();

            foreach(KeyValuePair<FileChooser, string> file in files)
                if(File.Exists(file.Value))
                    categories.Add( GetCategoryFromFile(file.Value) );

            return categories;
        }

        private static Category GetCategoryFromFile(string fileName)
        {
            /* Get all lines in the file */
            string[] fileText = File.ReadAllLines(fileName);

            Category newCat = new Category();
            SubCategory currentSub = null;
            Item currentProd = null;

            /* go through all lines, skipping the first 2 */
            for(int i = 2; i <= fileText.Length; i++)
            {
                if(i == fileText.Length)
                {
                    newCat.SubCategories.Add(currentSub);
                    break;
                }

                string[] thisLine = fileText[i].Split(new char[] { '|' });

                /* If the cat title isn't blank */
                if( thisLine[0] != "" )
                    newCat.Title = thisLine[0];

                /* If the subcategory title isn't blank */
                if (thisLine[1] != "")
                {
                    if (currentSub != null)
                        newCat.SubCategories.Add(currentSub);

                    currentSub = new SubCategory(thisLine[1]);
                }

                currentProd = new Item();

                /* Get Image */
                currentProd.ProductImage = System.AppDomain.CurrentDomain.BaseDirectory + thisLine[3];

                /* Get Title */
                currentProd.Title = thisLine[4];

                /* Get Price */
                currentProd.Price = double.Parse(Regex.Replace(thisLine[6], @"[^\u0000-\u007F]", string.Empty));

                /* Get first bit of description */
                currentProd.Description = thisLine[7].Replace("\"", "");

                for(i = i+1; i <= fileText.Length; i++)
                {
                    thisLine = fileText[i].Split(new char[] { '|' });
                    
                    /* Add next description */
                    currentProd.Description += Regex.Replace(thisLine[0], @"[^\u0000-\u007F]", string.Empty);
                    
                    /* if end of description */
                    if (thisLine.Length > 1)
                    {
                        /* Add size variants */
                        currentProd.SizeVariants = thisLine[1];

                        /* Add star rating */
                        currentProd.StarRating = thisLine[2] == "No Review" ? 0 : double.Parse(thisLine[2]);

                        /* Add first bit of delivery methods */
                        currentProd.DeliveryMethods = Regex.Replace(thisLine[3], @"[^\u0000-\u007F]", string.Empty).Replace("\"", "");

                        for (i = i + 1; i <= fileText.Length; i++)
                        {
                            thisLine = fileText[i].Split(new char[] { '|' });
                            
                            /* Add next bit of delivery methods */
                            currentProd.DeliveryMethods += Regex.Replace(thisLine[0], @"[^\u0000-\u007F]", string.Empty);

                            if (thisLine[0].Contains("\""))
                            {
                                currentSub.Products.Add(currentProd);
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            return newCat;
        }
    }
}
