using ASDAGeorgeApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDAGeorgeApp.ViewModels
{
    public class ProductListViewModel : BaseViewModel
    {
        private SubCategory subCategory;
        private Category thisCategory;

        public ProductListViewModel(SubCategory subCat, Category category)
        {
            subCategory = subCat;
            Items = subCat.Products;
            thisCategory = category;
        }

        public ProductListViewModel(string newIn, Category category)
        {
            subCategory = null;
            Items = Collector.GetAllItems(category);
            thisCategory = category;
        }

        public string trailText
        {
            get 
            {
                if (subCategory != null)
                    return thisCategory.Title.ToUpper() + " / " + subCategory.Title.ToUpper();
                else
                    return thisCategory.Title.ToUpper() + " / " + "NEW IN";
            }
        }

        public string ParentCategory
        {
            get { return subCategory.Parent; }
        }

        public SubCategory ParentSub
        {
            get { return subCategory; }
        }

        public ObservableCollection<Item> Items;

        public Category ParentCategoryInstance
        {
            get { return thisCategory; }
        }
    }
}
