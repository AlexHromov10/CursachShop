using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Entities
{
    public class OrderItem
    {
        


        public int Id { get; set; }

        public int OrderItem_OrderId{ get; set; }

        public int OrderItem_ItemId { get; set; }

    }
}
