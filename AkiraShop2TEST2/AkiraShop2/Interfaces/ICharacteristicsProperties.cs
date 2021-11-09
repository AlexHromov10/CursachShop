using AkiraShop2.Entities.HelperEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Interfaces
{
    public interface ICharacteristicsProperties
    {
        string charactName { get; set; }
        public CategoryCharacteristicsBool_Value charactValues_Bool { get; set; }
    }
}
