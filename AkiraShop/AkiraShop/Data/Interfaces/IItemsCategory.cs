using AkiraShop.Data.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Interfaces
{
    public interface IItemsCategory
    {

        IEnumerable<Category> AllCategories { get; }

        Category getObjectCategory(Guid catID);

        public void InsertCategory(Category cat);

        public void DeleteCategory(Category cat);

        public void AddIdToCategory(Guid itemID, Guid catID);

        public void DeleteAllCategories();
    }
}
