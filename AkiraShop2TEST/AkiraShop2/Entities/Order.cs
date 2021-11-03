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
        }


        public int Id { get; set; }

        [Required]
        public string UserOrderId { get; set; }

        [Required]
        public string Status { get; set; }

        [ForeignKey("OrderItem_OrderId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }


        [NotMapped]
        [Display(Name ="Товары заказа")]
        public List<Item> ItemsForOrder { get; set; }
    }
}
