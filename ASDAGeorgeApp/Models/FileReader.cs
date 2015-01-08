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
            
            fileList.Add(new KeyValuePair<FileChooser,string>(FileChooser.WomenFile, System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\women.csv"));
            fileList.Add(new KeyValuePair<FileChooser,string>(FileChooser.MenFile, System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\men.csv"));

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
                    if (newCat.SubCategories.Count > 0)
                    {
                        int rand = new Random().Next(newCat.SubCategories.Count - 1);
                        int newRand = new Random().Next(newCat.SubCategories[rand].Products.Count - 1);
                        newCat.SamplePicture = newCat.SubCategories[rand].Products[newRand].ProductImage;
                    }
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
                    {
                        newCat.SubCategories.Add(currentSub);
                        if (newCat.SubCategories.Count > 0)
                        {
                            int rand = new Random().Next(newCat.SubCategories.Count - 1);
                            int newRand = new Random().Next(newCat.SubCategories[rand].Products.Count - 1);
                            newCat.SamplePicture = newCat.SubCategories[rand].Products[newRand].ProductImage;
                        }
                    }

                    currentSub = new SubCategory(thisLine[1]);
                    string newstring = newCat.Title;
                    currentSub.Parent = newstring;
                }

                currentProd = new Item();

                if ((currentSub.Title.ToLower() == "dresses" && (thisLine[2] == "1" || thisLine[2] == "4"))
                    || currentSub.Title.ToLower() == "tops" && (int.Parse(thisLine[2]) > 4 && int.Parse(thisLine[2]) < 11)
                    || currentSub.Title.ToLower() == "dresses" && (thisLine[2] == "2" || thisLine[2] == "3" || thisLine[2] == "4" || thisLine[2] == "7" || thisLine[2] == "9"))
                    currentProd.IsThin = true;

                /* Get Image */
                currentProd.ProductImage = System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\ProductImages\\" + thisLine[3];

                /* Get Title */
                currentProd.Title = thisLine[4];

                /* Get Unique ID */
                currentProd.UniqueID = thisLine[5].Replace(" ", "");

                /* Get Price */
                currentProd.Price = double.Parse(thisLine[6].Replace("[POUND]", string.Empty).Replace("£", ""));

                /* Get first bit of description */
                currentProd.Description = thisLine[7].Replace("\"", "");

                for(i = i+1; i <= fileText.Length; i++)
                {
                    thisLine = fileText[i].Split(new char[] { '|' });
                    
                    /* Add next description */
                    if (thisLine[0] != "" && thisLine[0].Contains("•"))
                    {
                        string temp = "";
                        for(int k = 0; k < thisLine[0].Length; k++)
                        {
                            if (thisLine[0][k] == '•')
                                temp += "\r\n";
                            temp += thisLine[0][k];
                        }
                        thisLine[0] = temp;
                    }
                    currentProd.Description += "\r\n" + thisLine[0].Replace("\"", "");
                    
                    /* if end of description */
                    if (thisLine.Length > 1)
                    {
                        /* Add size variants */
                        currentProd.SizeVariants = thisLine[1];

                        /* Add star rating */
                        currentProd.StarRating = thisLine[2] == "No Review" ? 0 : double.Parse(thisLine[2]);

                        /* Add first bit of delivery methods */
                        currentProd.DeliveryMethods = thisLine[3].Replace("\"", "");

                        for (i = i + 1; i <= fileText.Length; i++)
                        {
                            thisLine = fileText[i].Split(new char[] { '|' });
                            
                            /* Add next bit of delivery methods */
                            currentProd.DeliveryMethods += "\r\n" + thisLine[0].Replace("\"", "");

                            if (thisLine[0].Contains("\""))
                            {
                                currentProd.ParentSub = currentSub.Title;
                                currentProd.ParentCat = newCat.Title;
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
