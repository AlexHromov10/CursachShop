using AkiraShop2.Controllers;
using AkiraShop2.Data;
using AkiraShop2.Data.Extensions;
using AkiraShop2.Entities;
using AkiraShop2.Entities.HelperEntities;
using AkiraShop2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AkiraShop2.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class ListController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public ListController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
            _logger = logger;
        }

        

        public Dictionary<string, string> FilterToUrl(FullFilter filter)
        {
            //Создание результирующих полей
            Dictionary<string, string> result = new Dictionary<string, string>();
            var queryParams = new Dictionary<string, List<string>>();

            //Получение в Dictionary<string,List<string>>
            //
            //Прием производителей
            List<string> valuesManufacture = new List<string>();
            for (int i = 0; i < filter.ManufacturerId.Count; i++)
            {
                if (filter.ManufacturerId[i]== true)
                {
                    valuesManufacture.Add(filter.Manufacturers[i].Title);
                }
            }
            if (valuesManufacture.Count != 0)
            {
                
                queryParams.Add("manufacturers", valuesManufacture);
            }

            //Прием цены
            if (filter.Price_from != null && filter.Price_to!= null)
            {
                queryParams.Add("price", new List<string> { filter.Price_from.Value.ToString(), filter.Price_to.Value.ToString() });
            }
            if ((filter.Price_from == null && filter.Price_to != null) || (filter.Price_from != null && filter.Price_to == null))
            {
                ViewData.Add("priceERROR", "Заполните оба поля!");
            }

            //Прием всех остальных параметров
            for (int i = 0; i < filter.Filters.Count; i++)
            {
                if (filter.CharactObject[i].charactValues_Bool.isNumeric == true)
                {
                    if (filter.Filters[i].from != null && filter.Filters[i].to != null)
                    {
                        queryParams.Add(filter.CharactObject[i].charactName, new List<string> { filter.Filters[i].from.Value.ToString(), filter.Filters[i].to.Value.ToString() });
                    }
                    if ((filter.Filters[i].from == null && filter.Filters[i].to != null) || (filter.Filters[i].from != null && filter.Filters[i].to == null))
                    {
                        ViewData.Add(filter.CharactObject[0].charactName, "Заполните оба поля!");
                    }
                }
                else
                {
                    List<string> valuess = new List<string>();
                    for (int j = 0; j < filter.Filters[i].exactValue.Count; j++)
                    {
                        if (filter.Filters[i].exactValue[j] == true)
                        {
                            valuess.Add(filter.CharactObject[i].charactValues_Bool.charactValues[j]);
                        }
                    }

                    queryParams.Add(filter.CharactObject[i].charactName, valuess);
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

        public FullFilter UrlToFilter(Dictionary<string, string> source, Category this_category)
        {
            //Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

            FullFilter filters = new FullFilter();
            filters.InitFilters(this_category);

            List<Item> item_list = _context.Item.Where(a => a.CategoryId == this_category.Id).ToList();
            List<Item> manIds = item_list.GroupBy(i => i.ManufacturerId).Select(grp => grp.First()).ToList();
            List<string> allManufacturers = new List<string>(); 
            foreach (var item in manIds)
            {
                allManufacturers.Add(_context.Manufacturer.FirstOrDefault(m => m.Id == item.ManufacturerId).Title);
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
                    filters.Price_from = UInt32.Parse(values[m].First());
                    filters.Price_to = UInt32.Parse(values[m].Last());
                }

                if (keys[m] == "manufacturers")
                {

                    for (int i = 0; i < values[m].Count; i++)
                    {
                        filters.ManufacturerId = toListBool(values[m], allManufacturers);
                        filters.Manufacturers.Add(new Manufacturer { Title = values[m][i]});
                    }
                    
                }

                for (int i = 0; i < this_category.CharactObject.Count; i++)
                {
                    if (this_category.CharactObject[i].charactName == keys[m])
                    {
                        if (this_category.CharactObject[i].charactValues_Bool.isNumeric == true)
                        {
                            filters.Filters[i] = (new Filter() { from = Double.Parse(values[m].First()), to = Double.Parse(values[m].Last()) });
                        }
                        else
                        {
                            filters.Filters[i].exactValue = toListBool(values[m], filters.CharactObject[i].charactValues_Bool.charactValues);
                            filters.Filters[i].exactValueString = values[m];
                        }
                    }
                }
            }
            return filters;
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


        public List<Item> FilterOut(List<Item> item_list,string charactName, double? from, double? to)
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

        public List<Item> FilterExact(List<Item> item_list,string charactName,List<string> exactValue)
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




        
        //GET стартовый
        [HttpGet("Shop/List/{categoryId:int}")]
        public async Task<IActionResult> Index(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            List<Item> item_list = _context.Item.Where(a => a.CategoryId == categoryId).ToList();

            Category this_category;
            this_category = await _context.Category.FindAsync(categoryId);
            if (this_category == null)
            {
                return NotFound();
            }

            ViewBag.CategoryTitle = this_category.Title;
            this_category.DeSerializeCategory();
            
            FullFilter filters = new FullFilter();
            filters.getManufacturers(item_list, _context);
            filters.InitFilters(this_category);


            ItemsViewModel items_category = new ItemsViewModel { itemList = item_list, category = this_category, filter = filters };
            items_category.filter.getMaxMinPrice(items_category.itemList);


            return View(items_category);
        }

        //POST: получение модели и запаковывание ее в URL для обработки в GET (Index)
        [ActionName("Filter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(FullFilter filter, int categoryId, int? ID)
        {
            ViewBag.CategoryId = categoryId;

            if (ID != null)
            {
                var item1 = await _context.Item.FindAsync(ID);
                if (item1.Amount > 0)
                {
                    item1.DecreaseAmount();

                    Order order = _context.Order.FirstOrDefault(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "CART");

                    OrderItem orderItem = new OrderItem { OrderItem_ItemId = item1.Id, OrderItem_OrderId = order.Id };

                    _context.Add(orderItem);

                    await _context.SaveChangesAsync();

                    int? jump_to = ID;
                    ViewBag.JumpToDivId = "DivId_item(" + jump_to + ")";
                }
                else
                {
                    
                }
                
            }

            //Получение категории для обработки в функции FilterToUrl
            var this_category = await _context.Category.FindAsync(categoryId);
            this_category.DeSerializeCategory();
            filter.CharactObject = this_category.CharactObject;

            List<Item> forManuf = _context.Item.Where(a => a.CategoryId == categoryId).ToList();
            filter.getManufacturers(forManuf, _context);

            //Отправка модели фильтра в GET запрос
            var result = FilterToUrl(filter);
            //

            //Упаковка полученной query в правильный url и сам редерикт
            if (result.Count != 0)
            {
                string url = QueryHelpers.AddQueryString(categoryId + "/filter", result);
                return Redirect(url);
            }
            else
            {
                //Если ни один фильтр не активирован
                return RedirectToAction(nameof(Index), new { categoryId = categoryId });
            }
        }

        //GET ФИЛЬТРАЦИЯ!!!!!
        [HttpGet("Shop/List/{categoryId}/{filter}")]
        public async Task<IActionResult> Index(int categoryId, [FromQuery] Dictionary<string, string> filterrr)
        {
            //Получение категории для функции UrlToFilter
            Category this_category = _context.Category.FirstOrDefault(id => id.Id == categoryId);
            this_category.DeSerializeCategory();

            //Получение модели фильтра из URL
            FullFilter fullFilter = UrlToFilter(filterrr, this_category);
            //
            fullFilter.CharactObject = this_category.CharactObject;

            //Получение всех товаров данной категории для последующего 
            List<Item> item_list = _context.Item.Where(a => a.CategoryId == categoryId).ToList();
            foreach (Item item in item_list)
            {
                item.DeSerializeItem();
                item.CharactObject = this_category.CharactObject;
            }

            List<Item> filtered = new List<Item>(item_list);

            if (fullFilter.Price_from != null && fullFilter.Price_to != null)
            {
                List<Item> filtered_new = new List<Item>();
                filtered_new.AddRange(filtered.Where(a => a.Price >= fullFilter.Price_from && a.Price <= fullFilter.Price_to));
                filtered = filtered_new;
            }
            if ((fullFilter.Price_from == null && fullFilter.Price_to != null) || (fullFilter.Price_from != null && fullFilter.Price_to == null))
            {
                ViewData.Add("priceERROR", "Заполните оба поля!");
            }
                      

            if (fullFilter.Manufacturers != null && fullFilter.Manufacturers.Count != 0)
            {
                List<Item> filtered_new = new List<Item>();
                for (int i = 0; i< fullFilter.Manufacturers.Count; i++)
                {
                    fullFilter.Manufacturers[i] = _context.Manufacturer.FirstOrDefault(a => a.Title == fullFilter.Manufacturers[i].Title);
                    
                }
                foreach (var man in fullFilter.Manufacturers)
                {
                    filtered_new.AddRange(filtered.Where(a => a.ManufacturerId == man.Id));

                }

                filtered = filtered_new;
            }


            for (int i = 0; i < fullFilter.CharactObject.Count; i++)
            {
                string charactName = fullFilter.CharactObject[i].charactName;
                if (fullFilter.CharactObject[i].charactValues_Bool.isNumeric == true)
                {
                    double? from = fullFilter.Filters[i].from;
                    double? to = fullFilter.Filters[i].to;

                    if (from != null && to != null)
                    {
                        List<Item> filtered_new = new List<Item>();
                        filtered_new.AddRange(FilterOut(filtered, charactName, from, to));
                        filtered = filtered_new;
                    }
                    if ((from == null && to != null) || (from != null && to == null))
                    {
                        ViewData.Add(fullFilter.CharactObject[i].charactName, "Заполните оба поля!");
                    }
                }
                else
                {
                    if (fullFilter.Filters[i].exactValueString != null)
                    {
                        List<Item> filtered_new = new List<Item>();
                        filtered_new.AddRange(FilterExact(filtered, charactName, fullFilter.Filters[i].exactValueString));

                        filtered = filtered_new;
                    }
                }
            }

            ItemsViewModel items_category = new ItemsViewModel { itemList = filtered, category = this_category, filter = fullFilter };
            items_category.filter.getMaxMinPrice(items_category.itemList);
            //items_category.filter.Manufacturers = new List<Manufacturer>();
            items_category.filter.getManufacturers(item_list, _context);


            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryTitle = this_category.Title;

            return View("Index", items_category);

        }



        
    }
}
