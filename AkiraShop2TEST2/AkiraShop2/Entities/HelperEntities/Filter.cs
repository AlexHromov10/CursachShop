using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Entities.HelperEntities
{
    public class Filter
    {
        public Filter()
        {
            //exactValue = new List<bool>();
        }

        public List<bool> exactValue { get; set; }

        public List<string> exactValueString { get; set; }

        [Display(Name = "От")]
        public double? from { get; set; }
        [Display(Name = "До")]
        public double? to { get; set; }
    }
}
