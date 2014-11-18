using System;
using System.Collections.Generic;
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

        public static void ReadFile(FileChooser file)
        {
            string fileString = ReturnFiles()[(int)file].Value;
            XDocument doc = XDocument.Load(fileString);
            List<Category> categories = LoadXMLFromFile(doc.Descendants("table").Elements("row"));
        }

        private static List<Category> LoadXMLFromFile(IEnumerable<XElement> enumerable)
        {
            /* Create list of children */
            List<Category> categories = new List<Category>();

            foreach (XElement x in enumerable)
            {
                /* Skip row if not data */
                string firstCellData = x.Element("cell").Element("data").Value;
                if( firstCellData == "Category" ||
                    firstCellData == "Approximate Knowledge Data Request")
                    continue;

                /* Skip row if not category row */
                if(x.Element("cell").Attribute("ss:Index") != null)
                    continue;

                /* Create a new category */
                /* Get list of cell data */
                List<XElement> data = x.Elements("data").ToList();
                Category task =
                    new Category(
                        data[0].Value,
                        GetAllSubCategories(data[1]))
                        uID,
                        x.Attribute("name").Value,
                        bool.Parse(x.Attribute("blocked").Value),
                        float.Parse(x.Attribute("percentage").Value),
                        x.Attribute("desc").Value,
                        (TaskNodeAssigned)int.Parse(x.Attribute("assigned").Value));

                task.BugTitles = GetBugReportsFor(task.UID);

                /* Add rest of this children */
                task.SetChildren(LoadTaskXML(x.Elements("task")));

                /* Add the task to the list */
                children.Add(task);
            }

            /* Return the children */
            return children;
        }
    }
}
