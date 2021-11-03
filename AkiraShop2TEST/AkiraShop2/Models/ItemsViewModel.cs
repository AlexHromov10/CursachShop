using AkiraShop2.Entities;
using AkiraShop2.Entities.HelperEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Models
{
    public class ItemsViewModel
    {
        public ItemsViewModel()
        {
            itemList = new List<Item>();
            
        }

        
        [NotMapped]
        public Category category { get; set; }
        [NotMapped]
        public List<Item> itemList { get; set; }
        
    }
}
