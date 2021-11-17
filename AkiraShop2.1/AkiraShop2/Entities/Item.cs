using AkiraShop.Data.Extensions;
using AkiraShop.Data.Models;
using AkiraShop2.Data;
using AkiraShop2.Entities.HelperEntities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Entities
{
    public class Item
    {
        public Item()
        {
            CharactObjectEXACT = new List<ItemCharacteristics>();
            CharactObject = new List<CategoryCharacteristics>();
        }

        public List<List<SelectListItem>> CharacteristicsConvertToSelectList()
        {

            List<List<SelectListItem>> returned = new List<List<SelectListItem>>();

            for (int i = 0; i < CharactObject.Count; i++)
            {
                List<SelectListItem> ValuesSelectList = new List<SelectListItem>();
               

                for (int j = 0; j < CharactObject[i].charactValues_Bool.charactValues.Count; j++)
                {
                    ValuesSelectList.Add(new SelectListItem());
                    ValuesSelectList[j].Text = CharactObject[i].charactValues_Bool.charactValues[j];
                    ValuesSelectList[j].Value = CharactObject[i].charactValues_Bool.charactValues[j];
                    if (CharactObjectEXACT[i].charactItemValue != "")
                    {

                        ValuesSelectList[j].Selected = true;
                    }
                    else
                    {
                        ValuesSelectList[j].Selected = false;
                    }
                    
                }

                returned.Add(ValuesSelectList);
            }
            return returned;
            
        }

        public void ReadContentItem(Category category)
        {
            category.DeSerializeCategory();

            CharactObject = category.CharactObject;
            for (int i = 0; i < category.CharactObject.Count; i++)
            {
                CharactObjectEXACT.Add(new ItemCharacteristics());
                CharactObjectEXACT[i].charactItemName = category.CharactObject[i].charactName;
            }
        }
        
        public void WriteContentItem(IWebHostEnvironment _hostingEnvironment, IFormFile file)
        {
            SerializeItem();

            var fileName = Path.GetFileNameWithoutExtension(Path.GetFileName(file.FileName)) + "_" + Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Item");
            var filePath = Path.Combine(uploads, fileName);
            using (FileStream Streem = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(Streem);
            }

            Image = fileName; // Set the file name
        }

        public void SerializeItem()
        {
            Json_Characterisitcs_exact = JsonConvert.SerializeObject(CharactObjectEXACT.ToDictionary(item => item.charactItemName, item => item.charactItemValue));
        }

        public void DeSerializeItem()
        {
            var keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(Json_Characterisitcs_exact);
            var keys = keyValuePairs.Keys.ToList();
            var values = keyValuePairs.Values.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                CharactObjectEXACT.Add(new ItemCharacteristics());
                CharactObjectEXACT[i].charactItemName = keys[i];
                CharactObjectEXACT[i].charactItemValue = values[i];
            }
        }

        

        public void DeleteImage(IWebHostEnvironment _hostingEnvironment)
        {
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Item");
            var filePath = Path.Combine(uploads, Image);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        public void DecreaseAmount()
        {
            
            if (Amount != 0)
            {
                Amount--;
            }

        }


        //Главные поля сущности Item://
        public int Id { set; get; }

        [Display(Name = "Название товара: ")]
        [Required(ErrorMessage = "Введите название товара!")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Некорректное количество символов")]
        public string Title{ get; set; }

        [Display(Name = "Описание товара: ")]
        [Required(ErrorMessage = "Введите описание товара!")]
        [StringLength(1000, ErrorMessage = "Некорректное количество символов")]
        public string Description { get; set; }

        [Display(Name = "Изображение: ")]
        public string Image { get; set; }

        [Display(Name = "Цена товара: ")]
        [Required(ErrorMessage = "Введите цену товара!")]
        [Range(0, uint.MaxValue, ErrorMessage = "Некорректная цена")]
        public uint Price { get; set; }

        public string Json_Characterisitcs_exact { get; set; }

        [Display(Name = "Категори товара: ")]
        [Required(ErrorMessage = "Выберите категорию товара!")]
        public int CategoryId { get; set; }

        [Display(Name = "Производитель товара: ")]
        [Required(ErrorMessage = "Выберите производителя товара!")]
        public int ManufacturerId { get; set; }

        [Display(Name = "Количество товара: ")]
        [Required(ErrorMessage = "Введите количество!")]
        [Range(0, uint.MaxValue, ErrorMessage = "Некорректное количество")]
        public uint Amount { get; set; }

        [ForeignKey("OrderItem_ItemId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }





        //NOT MAPPED//////////////////////////////////////////////////////////////////////////
        [NotMapped]
        [Display(Name = "Характеристики товара: ")]
        public List<ItemCharacteristics> CharactObjectEXACT { get; set; }

        [NotMapped]
        public List<CategoryCharacteristics> CharactObject { get; set; }

        [NotMapped]
        public virtual ICollection<SelectListItem> Manufacturers { get; set; }

        [NotMapped]
        public virtual ICollection<SelectListItem> Categories { get; set; }

        [NotMapped]
        public virtual List<List<SelectListItem>> Characteristics { get; set; }

        [NotMapped]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [Required(ErrorMessage = "Загрузите корректный файл!")]
        [Display(Name = "Картинка товара: ")]
        public IFormFile ImageFile { set; get; }

        [NotMapped]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [Display(Name = "Картинка товара: ")]
        public IFormFile ImageFile_EDIT { set; get; }
    }
}
