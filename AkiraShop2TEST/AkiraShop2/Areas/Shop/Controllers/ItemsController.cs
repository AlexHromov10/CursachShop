using AkiraShop2.Controllers;
using AkiraShop2.Data;
using AkiraShop2.Entities;
using AkiraShop2.Entities.HelperEntities;
using AkiraShop2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
    public class ItemsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public ItemsController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
            _logger = logger;
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

        public List<Item> FilterExact(List<Item> item_list,string charactName,List<bool> exactValue)
        {
            List<string> characteristics = new List<string>();

            foreach (var item in item_list)
            {
                foreach (var item_charact in item.CharactObject)
                {
                    if (item_charact.charactName == charactName)
                    {
                        for (int i = 0; i<item_charact.charactValues_Bool.charactValues.Count; i++)
                        {
                            if (exactValue[i])
                            {
                                if (characteristics.Any(u => u == item_charact.charactValues_Bool.charactValues[i]))
                                {
                                    break;
                                }
                                else
                                {
                                    characteristics.Add(item_charact.charactValues_Bool.charactValues[i]);
                                }
                                
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }


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

        public async Task<IActionResult> Index(int categoryId, string categoryTitle)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryTitle = categoryTitle;

            List<Item> item_list = new List<Item>();

            
            item_list = await (from item in _context.Item
                                   where item.CategoryId == categoryId
                                   select new Item
                                   {
                                       Amount = item.Amount,
                                       Id = item.Id,
                                       Title = item.Title,
                                       Description = item.Description,
                                       Price = item.Price,
                                       Json_Characterisitcs_exact = item.Json_Characterisitcs_exact,
                                       Image = item.Image,
                                       ManufacturerId = item.ManufacturerId,
                                       CategoryId = item.CategoryId
                                   }).ToListAsync();
            foreach (Item item in item_list)
            {
                item.DeSerializeItem();
            }
            



            var this_category = await _context.Category.FindAsync(categoryId);

            ItemsViewModel items_category = new ItemsViewModel { itemList = item_list, category = this_category };

            items_category.category.DeSerializeCategory();

            foreach (var charact in items_category.category.CharactObject)
            {
                charact.charactValues_Bool.AddFilter(charact.charactValues_Bool.charactValues.Count);
            }

            return View(items_category);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int? categoryId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);

            item.DecreaseAmount();

            Order order = _context.Order.FirstOrDefault(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "CART");

            OrderItem orderItem = new OrderItem { OrderItem_ItemId = id.Value, OrderItem_OrderId = order.Id };

            _context.Add(orderItem);
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { categoryId = categoryId.Value });
        }

        [ActionName("Filter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(ItemsViewModel filter)
        {
            //int categoryId = filter.category.Id;
            //int categoryId2 = ViewBag.CategoryId;
            Category match_category = _context.Category.FirstOrDefault(m => m.Id == filter.category.Id);
            match_category.DeSerializeCategory();

            List<Item> item_list = await (from item in _context.Item
                                          where item.CategoryId == filter.category.Id
                                          select new Item
                                          {
                                              Amount = item.Amount,
                                              Id = item.Id,
                                              Title = item.Title,
                                              Description = item.Description,
                                              Price = item.Price,
                                              Json_Characterisitcs_exact = item.Json_Characterisitcs_exact,
                                              Image = item.Image,
                                              ManufacturerId = item.ManufacturerId,
                                              CategoryId = item.CategoryId
                                          }).ToListAsync();

            foreach (Item item in item_list)
            {
                item.DeSerializeItem();
                //item.CharactObject = filter.category.CharactObject;
                

                item.CharactObject = match_category.CharactObject;
                
                //item.ReadContentItem(match_category);
            }

            List<Item> filtered = item_list;

            foreach (var charact in filter.category.CharactObject)
            {
                string charactName = charact.charactName;

                if (charact.charactValues_Bool.isNumeric == true)
                {
                    double? from = charact.charactValues_Bool.filter.from;
                    double? to = charact.charactValues_Bool.filter.to;

                    if (from != null && to != null)
                    {
                        List<Item> filtered_new = new List<Item>();
                        filtered_new.AddRange(FilterOut(filtered, charactName, from, to));

                        filtered = filtered_new;

                    }
                }
                else
                {
                    //List<bool> exactValue = charact.charactValues_Bool.filter.exactValue;
                    if (charact.charactValues_Bool.filter.exactValue.Any(u => u == true))
                    {
                        List<Item> filtered_new = new List<Item>();
                        filtered_new.AddRange(FilterExact(filtered, charactName, charact.charactValues_Bool.filter.exactValue));

                        filtered = filtered_new;
                    }
                    else
                    {

                    }
                }
            }

            ItemsViewModel items_category = new ItemsViewModel { itemList = filtered, category = match_category };

            //items_category.category.DeSerializeCategory();


            for (int i = 0; i < items_category.category.CharactObject.Count; i++)
            {
                items_category.category.CharactObject[i].charactValues_Bool.filter = filter.category.CharactObject[i].charactValues_Bool.filter;
            }

            ViewBag.CategoryId = filter.category.Id;
            ViewBag.CategoryTitle = filter.category.Title;
            return View("Index",items_category);


            //return Ok(filtered.ToString());
            //return RedirectToAction(nameof(Index), new { list_item = filtered, categoryId = filter.category.Id, categoryTitle = filter.category.Title});
        }


    }
}
