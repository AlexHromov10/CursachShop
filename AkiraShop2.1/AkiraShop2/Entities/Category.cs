using AkiraShop.Data.Extensions;
using AkiraShop2.Entities.HelperEntities;
using AkiraShop2.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class Category : IPrimaryProperties
    {
        public Category()
        {
            CharactObject = new List<CategoryCharacteristics>();
        }

        public void ReadContentCategory()
        {
            CharactObject.Add(new CategoryCharacteristics());
            DeSerializeCategory();
            foreach (var item in CharactObject)
            {
                item.charactValues_Bool.charactValues.RemoveAt(0);
            }
        }

        public void WriteContentCategory(IWebHostEnvironment _hostingEnvironment, IFormFile file)
        {
            //Json_Characterisitcs_model = JsonConvert.SerializeObject(CharactObject);

            SerializeCategory();

            var fileName = Path.GetFileNameWithoutExtension(Path.GetFileName(file.FileName)) + "_" + Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Category");
            var filePath = Path.Combine(uploads, fileName);
            using (FileStream Streem = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(Streem);
            }

            Image = fileName; // Set the file name

        }

        public void SerializeCategory()
        {
            Dictionary<string, CategoryCharacteristicsBool_Value> keyValuePairs = new Dictionary<string, CategoryCharacteristicsBool_Value>();


            for (int i = 0; i < CharactObject.Count; i++)
            {
                if (CharactObject[i].charactValues_Bool.isNumeric == true)
                {
                    List<string> forSort = new List<string>(CharactObject[i].charactValues_Bool.charactValues.OrderBy(x => double.Parse(x)));
                    CharactObject[i].charactValues_Bool.charactValues = forSort;
                }
                keyValuePairs.Add(CharactObject[i].charactName, CharactObject[i].charactValues_Bool);
            }

            Json_Characterisitcs_model = JsonConvert.SerializeObject(keyValuePairs);
        }

        public void DeSerializeCategory()
        {
            //Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();
            var keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, CategoryCharacteristicsBool_Value>>(Json_Characterisitcs_model);

            var keys = keyValuePairs.Keys.ToList();
            var values = keyValuePairs.Values.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                CharactObject.Add(new CategoryCharacteristics());
                
                CharactObject[i].charactName = keys[i];
                CharactObject[i].charactValues_Bool = values[i];
                CharactObject[i].charactValues_Bool.charactValues.RemoveAt(0);
            }

        }

        public void DeleteImage(IWebHostEnvironment _hostingEnvironment)
        {
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Category");
            var filePath = Path.Combine(uploads, Image);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

            }
        }

        public List<int> CheckForNumeric()
        {
            List<int> errorIds = new List<int>();
            for (int i = 0; i < CharactObject.Count; i++)
            {
                if (CharactObject[i].charactValues_Bool.isNumeric == true)
                {
                    foreach (var charact in CharactObject[i].charactValues_Bool.charactValues)
                    {
                        double n;
                        bool isNumeric = double.TryParse(charact, out n);
                        if (!isNumeric)
                        {
                            errorIds.Add(i);
                            break;
                        }
                    }
                }
            }
            return errorIds;
        }

        //Главные поля сущности Category://
        public int Id { get; set; }
        
        [Display(Name = "Название категории: ")]
        [Required(ErrorMessage = "Введите название категории!")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Некорректное количество символов")]
        public string Title { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Некорректное количество символов")]
        [Display(Name = "Описание категории: ")]
        [Required(ErrorMessage = "Введите описание категории!")]
        public string Descriprions { get; set; }

        [Display(Name = "Изображение: ")]
        public string Image { get; set; }

        public string Json_Characterisitcs_model { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ICollection<Item> Items { get; set; }





        //NOT MAPPED//////////////////////////////////////////////////////////////////////////
        [NotMapped]
        [Display(Name = "Характеристики категории: ")]
        public List<CategoryCharacteristics> CharactObject { get; set; }

        [NotMapped]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [Required(ErrorMessage = "Загрузите корректный файл!")]
        [Display(Name = "Изображение категории: ")]
        public IFormFile ImageFile { set; get; }

        [NotMapped]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [Display(Name = "Изображение категории: ")]
        public IFormFile ImageFile_EDIT { set; get; }

    }
}
