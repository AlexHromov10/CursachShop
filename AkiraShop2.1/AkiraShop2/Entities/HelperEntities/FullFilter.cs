using AkiraShop2.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
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
        }

        public async Task getManufacturers(List<Item> item_list, ApplicationDbContext _context)
        {
            List<Item> manIds = item_list.GroupBy(i => i.ManufacturerId).Select(grp => grp.First()).ToList();
            Manufacturers = new List<Manufacturer>();

            foreach (var item in manIds)
            {
                Manufacturers.Add(await _context.Manufacturer.FirstOrDefaultAsync(m => m.Id == item.ManufacturerId));
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

        public List<bool> toListBool(List<string> valuesFilter, List<string> valuesInit)
        {
            List<bool> exactValue = new List<bool>();

            for (int i = 0; i < valuesInit.Count; i++)
            {
                exactValue.Add(false);
                for (int j = 0; j < valuesFilter.Count; j++)
                {
                    if (valuesInit[i] == valuesFilter[j])
                    {
                        exactValue[i] = true;
                    }
                }

            }

            return exactValue;
        }

        public List<Item> FilterOut(List<Item> item_list, string charactName, double? from, double? to)
        {
            List<Item> result_item_list = new List<Item>();

            foreach (Item item in item_list)
            {
                bool check_if_match = false;

                foreach (var item_charact in item.CharactObjectEXACT)
                {
                    if (item_charact.charactItemName == charactName)
                    {
                        if (Double.Parse(item_charact.charactItemValue) >= from && Double.Parse(item_charact.charactItemValue) <= to)
                        {
                            check_if_match = true;
                        }
                        else
                        {
                            check_if_match = false;
                        }
                    }
                }
                if (check_if_match)
                {
                    result_item_list.Add(item);
                }
            }

            return result_item_list;
        }

        public List<Item> FilterExact(List<Item> item_list, string charactName, List<string> exactValue)
        {
            List<string> characteristics = exactValue;

            List<Item> result_item_list = new List<Item>();

            foreach (Item item in item_list)
            {

                foreach (var item_charact in item.CharactObjectEXACT)
                {
                    if (item_charact.charactItemName == charactName)
                    {
                        foreach (var value in characteristics)
                        {
                            if (item_charact.charactItemValue == value)
                            {

                                if (result_item_list.Any(u => u == item))
                                {
                                    break;
                                }
                                else
                                {
                                    result_item_list.Add(item);
                                }

                            }
                            else
                            {
                                //check_if_match = false;
                            }
                        }

                    }
                }

            }

            return result_item_list;
        }



        public Dictionary<string, string> FilterToUrl()
        {
            //Создание результирующих полей
            Dictionary<string, string> result = new Dictionary<string, string>();
            var queryParams = new Dictionary<string, List<string>>();

            //Получение в Dictionary<string,List<string>>
            //
            //Прием производителей
            List<string> valuesManufacture = new List<string>();
            for (int i = 0; i < this.ManufacturerId.Count; i++)
            {
                if (this.ManufacturerId[i] == true)
                {
                    valuesManufacture.Add(this.Manufacturers[i].Title);
                }
            }
            if (valuesManufacture.Count != 0)
            {

                queryParams.Add("manufacturers", valuesManufacture);
            }

            //Прием цены
            if (this.Price_from != null && this.Price_to != null)
            {
                queryParams.Add("price", new List<string> { this.Price_from.Value.ToString(), this.Price_to.Value.ToString() });
            }
            if ((this.Price_from == null && this.Price_to != null) || (this.Price_from != null && this.Price_to == null))
            {
            }

            //Прием всех остальных параметров
            for (int i = 0; i < this.Filters.Count; i++)
            {
                if (this.CharactObject[i].charactValues_Bool.isNumeric == true)
                {
                    if (this.Filters[i].from != null && this.Filters[i].to != null)
                    {
                        queryParams.Add(this.CharactObject[i].charactName, new List<string> { this.Filters[i].from.Value.ToString(), this.Filters[i].to.Value.ToString() });
                    }
                    if ((this.Filters[i].from == null && this.Filters[i].to != null) || (this.Filters[i].from != null && this.Filters[i].to == null))
                    {
                    }
                }
                else
                {
                    List<string> valuess = new List<string>();
                    for (int j = 0; j < this.Filters[i].exactValue.Count; j++)
                    {
                        if (this.Filters[i].exactValue[j] == true)
                        {
                            valuess.Add(this.CharactObject[i].charactValues_Bool.charactValues[j]);
                        }
                    }

                    queryParams.Add(this.CharactObject[i].charactName, valuess);
                }

            }

            //Перевод в Dictionary<string,string>
            List<string> keys = queryParams.Keys.ToList();
            List<string> values = new List<string>();
            foreach (var item in queryParams.Values)
            {

                var r = string.Join(";", item);
                values.Add(r);
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (values[i] != "")
                {
                    result.Add(keys[i], values[i]);
                }

            }
            return result;
        }

        public async Task UrlToFilter(Dictionary<string, string> source, Category this_category, ApplicationDbContext _context, ViewDataDictionary ViewData)
        {
            //Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

            this.InitFilters(this_category);

            List<Item> item_list = await _context.Item.Where(a => a.CategoryId == this_category.Id).ToListAsync();
            List<Item> manIds = item_list.GroupBy(i => i.ManufacturerId).Select(grp => grp.First()).ToList();
            List<string> allManufacturers = new List<string>();
            foreach (var item in manIds)
            {
                Manufacturer buffman = await _context.Manufacturer.FirstOrDefaultAsync(m => m.Id == item.ManufacturerId);
                allManufacturers.Add(buffman.Title);
            }

            List<string> keys = source.Keys.ToList();
            List<List<string>> values = new List<List<string>>();
            foreach (var item in source.Values)
            {
                values.Add(item.Split(";").ToList());
            }


            for (int m = 0; m < keys.Count; m++)
            {
                if (keys[m] == "price")
                {
                    this.Price_from = UInt32.Parse(values[m].First());
                    this.Price_to = UInt32.Parse(values[m].Last());
                }

                if (keys[m] == "manufacturers")
                {

                    for (int i = 0; i < values[m].Count; i++)
                    {
                        this.ManufacturerId = toListBool(values[m], allManufacturers);
                        this.Manufacturers.Add(new Manufacturer { Title = values[m][i] });
                    }

                }
                /*
                if (keys[m] == "scrollId")
                {
                    int? jump_to = Int32.Parse(values[m].First());
                    ViewData.Add("JumpToDivId", "DivId_item(" + jump_to + ")");
                }
                */
                for (int i = 0; i < this_category.CharactObject.Count; i++)
                {
                    if (this_category.CharactObject[i].charactName == keys[m])
                    {
                        if (this_category.CharactObject[i].charactValues_Bool.isNumeric == true)
                        {
                            this.Filters[i] = (new Filter() { from = Double.Parse(values[m].First()), to = Double.Parse(values[m].Last()) });
                        }
                        else
                        {
                            this.Filters[i].exactValue = toListBool(values[m], this.CharactObject[i].charactValues_Bool.charactValues);
                            this.Filters[i].exactValueString = values[m];
                        }
                    }
                }
            }
            //return
        }

        public async Task<List<Item>> FilterOnGet(ViewDataDictionary ViewData, List<Item> filtered, ApplicationDbContext _context)
        {
            if (this.Price_from != null && this.Price_to != null)
            {
                List<Item> filtered_new = new List<Item>();
                filtered_new.AddRange(filtered.Where(a => a.Price >= this.Price_from && a.Price <= this.Price_to));
                filtered = filtered_new;
            }
            if ((this.Price_from == null && this.Price_to != null) || (this.Price_from != null && this.Price_to == null))
            {
                ViewData.Add("priceERROR", "Заполните оба поля!");
            }


            if (this.Manufacturers != null && this.Manufacturers.Count != 0)
            {
                List<Item> filtered_new = new List<Item>();
                for (int i = 0; i < this.Manufacturers.Count; i++)
                {
                    this.Manufacturers[i] = await _context.Manufacturer.FirstOrDefaultAsync(a => a.Title == this.Manufacturers[i].Title);

                }
                foreach (var man in this.Manufacturers)
                {
                    filtered_new.AddRange(filtered.Where(a => a.ManufacturerId == man.Id));

                }

                filtered = filtered_new;
            }


            for (int i = 0; i < this.CharactObject.Count; i++)
            {
                string charactName = this.CharactObject[i].charactName;
                if (this.CharactObject[i].charactValues_Bool.isNumeric == true)
                {
                    double? from = this.Filters[i].from;
                    double? to = this.Filters[i].to;

                    if (from != null && to != null)
                    {
                        List<Item> filtered_new = new List<Item>();
                        filtered_new.AddRange(FilterOut(filtered, charactName, from, to));
                        filtered = filtered_new;
                    }
                    if ((from == null && to != null) || (from != null && to == null))
                    {
                        ViewData.Add(this.CharactObject[i].charactName, "Заполните оба поля!");
                    }
                }
                else
                {
                    if (this.Filters[i].exactValueString != null)
                    {
                        List<Item> filtered_new = new List<Item>();
                        filtered_new.AddRange(FilterExact(filtered, charactName, this.Filters[i].exactValueString));

                        filtered = filtered_new;
                    }
                }
            }
            return filtered;
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
