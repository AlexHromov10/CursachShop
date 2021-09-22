using AkiraShop.Data.Interfaces;
using AkiraShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Repository
{
    public class ItemsRepository : IAllItems
    {
        private readonly AppDBContent AppDBContent;

        public ItemsRepository(AppDBContent appDBContent)
        {
            this.AppDBContent = appDBContent;
        }

        //GET all items
        public IEnumerable<Item> AllItems => AppDBContent.Items;

        //GET one item
        public Item getObjectItem(Guid itemID) => AppDBContent.Items.FirstOrDefault(p => p.id == itemID);

        //INSERT
        public void InsertItem(Item item)
        {
            AppDBContent.Items.Add(item);
            AppDBContent.SaveChanges();
        }

        //DELETE
        public void DeleteItem(Item item)
        {
            AppDBContent.Items.Remove(item);
            AppDBContent.SaveChanges();
        }
    }
}
