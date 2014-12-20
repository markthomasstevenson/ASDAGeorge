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
            thisCategory = category;
        }

        public string trailText
        {
            get { return subCategory.Parent.ToUpper() + " / " + subCategory.Title.ToUpper(); }
        }

        public string ParentCategory
        {
            get { return subCategory.Parent; }
        }

        public SubCategory ParentSub
        {
            get { return subCategory; }
        }

        public ObservableCollection<Item> Items
        {
            get { return subCategory.Products; }
        }

        public Category ParentCategoryInstance
        {
            get { return thisCategory; }
        }
    }
}
