using AkiraShop2.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Entities.HelperEntities
{
    public class FullFilter
    {
        public void InitFilters(Category this_category)
        {
            this.CharactObject = this_category.CharactObject;

            for (int i = 0; i < this.CharactObject.Count; i++)
            {
                this.Filters.Add(new Filter());
                this.Filters[i].exactValue = new List<bool>();
                for (int j = 0; j < this.CharactObject[i].charactValues_Bool.charactValues.Count; j++)
                {
                    this.Filters[i].exactValue.Add(false);
                }
                
            }
            for (int i = 0; i < this.Manufacturers.Count; i++)
            {
                this.ManufacturerId.Add(false);
            }
        }


        public void getManufacturers(List<Item> item_list, ApplicationDbContext _context)
        {
            List<Item> manIds = item_list.GroupBy(i => i.ManufacturerId).Select(grp => grp.First()).ToList();
            Manufacturers = new List<Manufacturer>();

            foreach (var item in manIds)
            {
                Manufacturers.Add(_context.Manufacturer.FirstOrDefault(m => m.Id == item.ManufacturerId));
            }

            if (ManufacturerId.Count == 0)
            {
                for (int i = 0; i < this.Manufacturers.Count; i++)
                {
                    this.ManufacturerId.Add(false);
                }
            }

            foreach (Item item in item_list)
            {
                item.DeSerializeItem();
            };
        }

        public void getMaxMinPrice(List<Item> item_list)
        {
            if (item_list != null && item_list.Count != 0)
            {
                List<Item> buff_list = new List<Item>(item_list);
                buff_list.Sort((x, y) =>
                    x.Price.CompareTo(y.Price));

                maxMinPrice.Add(buff_list.First().Price);
                maxMinPrice.Add(buff_list.Last().Price);
            }
            else
            {
                maxMinPrice.Add(null);
                maxMinPrice.Add(null);
            }
            
        }



        public FullFilter()
        {
            Filters = new List<Filter>();
            maxMinPrice = new List<uint?>();
            Manufacturers = new List<Manufacturer>();
            ManufacturerId = new List<bool>();
        }
        [NotMapped]
        public List<Filter> Filters { get; set; }
        [NotMapped]
        public List<CategoryCharacteristics> CharactObject { get; set; }


        [NotMapped]
        public List<uint?> maxMinPrice { get; set; }
        [NotMapped]
        [Display(Name = "От")]
        public uint? Price_from { get; set; }
        [NotMapped]
        [Display(Name = "До")]
        public uint? Price_to { get; set; }



        [NotMapped]
        public List<Manufacturer> Manufacturers { get; set; }
        [NotMapped]
        public List<bool> ManufacturerId { get; set; }

    }
}
