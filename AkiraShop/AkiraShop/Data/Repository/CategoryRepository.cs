using AkiraShop.Data.Interfaces;
using AkiraShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Repository
{
    public class CategoryRepository : IItemsCategory
    {
        private readonly AppDBContent AppDBContent;

        public CategoryRepository(AppDBContent appDBContent)
        {
            this.AppDBContent = appDBContent;
        }

        //GET all categories
        public IEnumerable<Category> AllCategories => AppDBContent.Categories;


        //GET one category
        public Category getObjectCategory(Guid catID) => AppDBContent.Categories.FirstOrDefault(p => p.id == catID);

        //INSERT
        public void InsertCategory(Category cat)
        {
            AppDBContent.Categories.Add(cat);
            AppDBContent.SaveChanges();
        }

        //ADD Item id to category
        public void AddIdToCategory(Guid itemID, Guid catID)
        {

            Category edit = AppDBContent.Categories.FirstOrDefault(p => p.id == catID);
            if (edit != null)
            {
                //edit.items.Add(itemID);
                AppDBContent.SaveChanges();
            }
        }

        //DELETE
        public void DeleteCategory(Category cat)
        {
            AppDBContent.Categories.Remove(cat);
            AppDBContent.SaveChanges();
        }

        //DELETE ALL
        public void DeleteAllCategories()
        {
            AppDBContent.Categories.RemoveRange(AllCategories);
        }


    }
}
