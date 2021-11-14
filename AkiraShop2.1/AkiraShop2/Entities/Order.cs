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
                        this.items_with_amounts.Add(item, orderItem.OrderItem_Amount);
                    }

                }
            }
        }

        public List<int> NotAvalibleItemsIDS_cart(List<Item> items) 
        {
            List<int> result = new List<int>();
            foreach (var item in items)
            {
                foreach (var order_item in this.OrderItems)
                {
                    if (order_item.OrderItem_ItemId == item.Id && order_item.OrderItem_Amount == item.Amount)
                    {
                        result.Add(item.Id);
                    }
                }
            }
            
            return result;
        }
        public List<int> NotAvalible_wishList(List<Item> items)
        {
            List<int> result = new List<int>();
            foreach (var item in items)
            {
                foreach (var order_item in this.OrderItems)
                {
                    if (order_item.OrderItem_ItemId == item.Id && order_item.OrderItem_Amount > 0)
                    {
                        result.Add(item.Id);
                    }
                }
            }

            return result;
        }

        public async Task AddToCart(Item item, ApplicationDbContext _context)
        {
            OrderItem target = this.OrderItems.FirstOrDefault(oi => oi.OrderItem_ItemId == item.Id);

            if (target!=null)
            {
                if (target.OrderItem_Amount < item.Amount)
                {
                    target.OrderItem_Amount++;
                    await _context.SaveChangesAsync();
                    return;
                }
                return;
            }
            else
            {
                OrderItem orderItem = new OrderItem { OrderItem_ItemId = item.Id, OrderItem_OrderId = this.Id, OrderItem_Amount = 1 };

                await _context.AddAsync(orderItem);
                await _context.SaveChangesAsync();
                return;
            }
        }


        public async Task AddToWaitList(Item item1, ApplicationDbContext _context)
        {
            OrderItem orderItem = new OrderItem { OrderItem_ItemId = item1.Id, OrderItem_OrderId = this.Id, OrderItem_Amount = 1 };

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
                    item.Amount = item.Amount + ((uint)orderItem.OrderItem_Amount);
                }
            }
            _context.Remove(this);

            await _context.SaveChangesAsync();
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

        public async Task RemoveNotAvalibleItems(List<Item> notAvalibleItems,Order wait_list, ApplicationDbContext _context)
        {
            foreach (var NOitem in notAvalibleItems)
            {
                if (!wait_list.ItemsForOrder.Any(i => i == NOitem))
                {
                    _context.OrderItem.Add(new OrderItem { OrderItem_ItemId = NOitem.Id, OrderItem_OrderId = wait_list.Id, OrderItem_Amount = 1 });

                    wait_list.ItemsForOrder.Add(NOitem);


                }
                this.ItemsForOrder.RemoveAll(i => i.Id == NOitem.Id);
                OrderItem toRemove = await _context.OrderItem.FirstOrDefaultAsync(i => i.OrderItem_ItemId == NOitem.Id && i.OrderItem_OrderId == this.Id && i.OrderItem_Amount == this.items_with_amounts[NOitem]);
                this.items_with_amounts.Remove(NOitem);
                _context.OrderItem.Remove(toRemove);
            }
            await _context.SaveChangesAsync();
        }

        public async Task order_Create(ApplicationDbContext _context)
        {

            foreach (var orderItem in this.OrderItems)
            {
                Item item = await _context.Item.FirstOrDefaultAsync(i => i.Id == orderItem.OrderItem_ItemId);
                if (item != null)
                {
                    item.Amount = item.Amount - (uint)orderItem.OrderItem_Amount;
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
