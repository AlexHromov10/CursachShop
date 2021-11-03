using AkiraShop2.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Entities.HelperEntities
{
    public class CategoryCharacteristics : ICharacteristicsProperties
    {
        public CategoryCharacteristics()
        {
            charactValues_Bool = new CategoryCharacteristicsBool_Value();
        }


        [JsonProperty]
        [Required(ErrorMessage = "Введите название характеристики!")]
        [Display(Name = "Название характеристики: ")]
        public string charactName { get; set; }

        public CategoryCharacteristicsBool_Value charactValues_Bool { get; set; }

    }
}
