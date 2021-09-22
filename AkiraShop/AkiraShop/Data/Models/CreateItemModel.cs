using AkiraShop.Data.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Models
{
    public class CreateItemModel
    {
        [BindProperty]
        public Item Item { get; set; }

        [BindProperty]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [Required(ErrorMessage = "Загрузите корректный файл!")]
        [Display(Name = "Название товара: ")]
        public IFormFile Image { set; get; }


    }

}
