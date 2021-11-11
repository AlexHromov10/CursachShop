using AkiraShop2.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Entities
{
    public class Order
    {
        public Order()
        {
            ItemsForOrder = new List<Item>();
            items_with_amounts = new Dictionary<Item, int>();
        }

        public async Task InitOrder(ApplicationDbContext _context)
        {
            if (this.OrderItems != null)
            {

                foreach (var orderItem in this.OrderItems)
                {
                    Item item = await _context.Item.FirstOrDefaultAsync(i => i.Id == orderItem.OrderItem_ItemId);
                    if (item != null)
                    {
                        this.ItemsForOrder.Add(item);
                    }
                }
                this.items_with_amounts = this.itemToBuy_amount(this.ItemsForOrder);
            }
        }

        public int? GetAmountInOrder(int itemId, string status)
        {
            if (Status == status)
            {
                int amount = 0;
                foreach (var item in OrderItems)
                {
                    if (item.OrderItem_ItemId == itemId)
                    {
                        amount++;
                    }
                }
                return amount;
            }
            return null;
        }

        public List<int> NotAvalibleItemsIDS_cart(List<Item> items) 
        {
            List<int> result = new List<int>();
            foreach (var item in items)
            {
                if (GetAmountInOrder(item.Id,"CART") == item.Amount)
                {
                    result.Add(item.Id);
                }
            }
            return result;
        }
        public List<int> NotAvalible_wishList(List<Item> items)
        {
            List<int> result = new List<int>();
            foreach (var item in items)
            {
                if (GetAmountInOrder(item.Id,"WISH_LIST") > 0)
                {
                    result.Add(item.Id);
                }
            }
            return result;
        }

        public async Task AddToCart(Item item1, ApplicationDbContext _context)
        {
            if (GetAmountInOrder(item1.Id,"CART") < item1.Amount)
            {
                //item1.DecreaseAmount();

                OrderItem orderItem = new OrderItem { OrderItem_ItemId = item1.Id, OrderItem_OrderId = this.Id };

                await _context.AddAsync(orderItem);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddToWaitList(Item item1, ApplicationDbContext _context)
        {
            OrderItem orderItem = new OrderItem { OrderItem_ItemId = item1.Id, OrderItem_OrderId = this.Id };

            await _context.AddAsync(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task order_Delete(ApplicationDbContext _context)
        {
            foreach (var orderItem in OrderItems)
            {
                Item item = await _context.Item.FirstOrDefaultAsync(i => i.Id == orderItem.OrderItem_ItemId);
                if (item != null)
                {
                    item.Amount++;
                }
            }
            _context.Remove(this);

            await _context.SaveChangesAsync();
        }

        public Dictionary<Item,int> itemToBuy_amount(List<Item> itemsToBuy)
        {
            Dictionary<Item, int> result = new Dictionary<Item, int>();
            foreach (var item in itemsToBuy)
            {
                if (!result.Keys.Any(u => u == item))
                {
                    var count = itemsToBuy.Count(i => i == item);
                    result.Add(item, count);
                }
                
                
            }
            return result;
        }

        public List<Item> order_checkForAvalibleAmount(Dictionary<Item, int> items_amount)
        {

            List<Item> notAvalibleItems = new List<Item>();

            foreach (var item in items_amount.Keys)
            {
                if (items_amount[item] > item.Amount)
                {
                    notAvalibleItems.Add(item);
                }
            }
            return notAvalibleItems;
        }

        public async Task order_Create(ApplicationDbContext _context)
        {

            foreach (var orderItem in this.OrderItems)
            {
                Item item = await _context.Item.FirstOrDefaultAsync(i => i.Id == orderItem.OrderItem_ItemId);
                if (item != null)
                {
                    item.Amount--;
                }
            }

            this.Status = "formed";

            Order new_cart = new Order { UserOrderId = this.UserOrderId, Status = "CART" };

            _context.Update(this);
            await _context.AddAsync(new_cart);
            await _context.SaveChangesAsync();
        }

        public int Id { get; set; }

        [Required]
        public string UserOrderId { get; set; }

        [Required]
        public string Status { get; set; }

        [ForeignKey("OrderItem_OrderId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }



        [NotMapped]
        public List<Item> ItemsForOrder { get; set; }
        [NotMapped]
        public Dictionary<Item, int> items_with_amounts { get; set; }

    }
}
