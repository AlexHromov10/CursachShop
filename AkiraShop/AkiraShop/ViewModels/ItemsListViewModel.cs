using AkiraShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.ViewModels
{
    public class ItemsListViewModel
    {
        public IEnumerable<Item> AllItems { get; set; }

        public List<Category> AllCat { get; set; }

        //public IEnumerable<CreateItemModel> MODEL { get; set; }

        public CreateItemModel ItemModel { get; set; }
        public Category CatModel { get; set; }

        public Guid currCategory { get; set; }

    }
}
