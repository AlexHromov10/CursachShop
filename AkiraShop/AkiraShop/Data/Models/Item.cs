using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Models
{
    public class Item
    {
        public Guid id { set; get; }

        [StringLength(100)]
        [Required(ErrorMessage = "Введите название товара")]
        [Display(Name = "Название товара: ")]
        public string itemName { get; set; }

        [StringLength(1000)]
        [Required(ErrorMessage = "Введите описание товара")]
        [Display(Name = "Описание товара: ")]
        public string Desc { get; set; }

        [StringLength(1000)]
        public string img { get; set; }

        [Range(0, 500000)]
        [Required(ErrorMessage = "Введите цену товара в числах")]
        [Display(Name = "Цена товара: ")]
        public uint price { get; set; }

        [Required]
        public Guid categoryID { get; set; }

       

    }
}
