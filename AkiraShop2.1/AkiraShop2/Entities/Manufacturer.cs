using AkiraShop2.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Entities
{
    public class Manufacturer : IPrimaryProperties
    {
        //[Key]
        public int Id { get; set; }

        [Display(Name = "Название производителя: ")]
        [Required(ErrorMessage = "Введите название производителя!")]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; }

        [Display(Name = "Описание производителя: ")]
        [Required(ErrorMessage = "Введите описание производителя!")]
        [StringLength(200, MinimumLength = 2)]
        public string Descriprions { get; set; }






        [ForeignKey("ManufacturerId")]
        public virtual ICollection<Item> Items { get; set; }
    }
}
