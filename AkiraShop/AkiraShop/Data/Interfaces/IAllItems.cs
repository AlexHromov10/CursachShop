using AkiraShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Interfaces
{
    public interface IAllItems
    {

        IEnumerable<Item> AllItems { get;}

        Item getObjectItem(Guid itemID);

        void InsertItem(Item item);

        void DeleteItem(Item item);
    }
}
