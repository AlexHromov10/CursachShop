using AkiraShop2.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Models
{
    //[JsonObject]
    public class ItemCharacteristics
    {
        public ItemCharacteristics()
        {
            charactItemName = string.Empty;
            charactItemValue = string.Empty;
        }
        [JsonProperty]

        [Display(Name = "Название характеристики: ")]
        public string charactItemName { get; set; }

        [JsonProperty]
        [Display(Name = "Значение характеристики: ")]
        public string charactItemValue { get; set; }
 
    }
}
