using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Entities.HelperEntities
{
    public class CategoryCharacteristicsBool_Value
    {
        public CategoryCharacteristicsBool_Value()
        {
            charactValues = new List<string>();

            //filter = new Filter();
            //filter.exactValue = new List<bool>();

            charactValues.Add(string.Empty);
        }
        /*
        public void AddFilter(int count)
        {
            for (int i = 0; i < count; i++)
            {
                filter.exactValue.Add(false);
            }
        }
        */
        [JsonProperty]
        [Required(ErrorMessage = "Введите значения характеристики!")]
        [Display(Name = "Значения характеристики: ")]
        public List<string> charactValues { get; set; }

        [JsonProperty]
        [Required]
        [Display(Name = "Числовые значения или нет")]
        public bool isNumeric { get; set; }

        //[NotMapped]
        //public Filter filter { get; set; }

        
    }
}
