using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Areas.Admin.Models
{
    public class SearchModel
    {
        public uint? orderId { get; set; }

        public string userEmail { get; set; }
        public string userNumber { get; set; }

        public uint? categoryId { get; set; }
        public string categoryTitle { get; set; }

        public uint? itemId { get; set; }
        public string itemTitle { get; set; }


        

    }
}
