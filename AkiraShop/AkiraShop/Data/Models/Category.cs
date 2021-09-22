using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Models
{
    public class Category
    {

        public Guid id { set; get; }

        [StringLength(100)]
        [Required(ErrorMessage ="Введите название категории")]
        public string categoryName{ set; get; }

        [StringLength(1000)]
        [Required(ErrorMessage = "Введите описание категории")]
        public string desc { set; get; }

        //public json characteristics { get; set; }

    }

    
}
