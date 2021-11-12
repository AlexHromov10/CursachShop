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
using System.Web;

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

        //GET стартовый
        [HttpGet("Shop/List/{categoryId:int}")]
        public async Task<IActionResult> Index(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            List<Item> item_list = await _context.Item.Where(a => a.CategoryId == categoryId).ToListAsync();

            Category this_category;
            this_category = await _context.Category.FindAsync(categoryId);
            if (this_category == null)
            {
                return NotFound();
            }

            ViewBag.CategoryTitle = this_category.Title;
            this_category.DeSerializeCategory();
            
            FullFilter filters = new FullFilter();
            await filters.getManufacturers(item_list, _context);
            filters.InitFilters(this_category);

            List<int> notAvalible_Items = new List<int>();
            List<int> notAvalible_WishList = new List<int>();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                Order cart = await _context.Order.Include(o=>o.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == userId && i.Status == "CART");
                notAvalible_Items = cart.NotAvalibleItemsIDS_cart(item_list);

                Order wish_list = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == userId && i.Status == "WISH_LIST");
                notAvalible_WishList = wish_list.NotAvalible_wishList(item_list);
            }
            

            ItemsViewModel items_category = new ItemsViewModel { itemList = item_list, 
                category = this_category, 
                filter = filters, 
                NOT_AVALIBLE_itemListIDS = notAvalible_Items,
                NOT_AVALIBLE_waitList = notAvalible_WishList};
            items_category.filter.getMaxMinPrice(items_category.itemList);


            return View(items_category);
        }

        //POST: получение модели и запаковывание ее в URL для обработки в GET (Index)
        [ActionName("Filter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(FullFilter filter, int categoryId, int? ID, int? ID_wishlist)
        {
            ViewBag.CategoryId = categoryId;

            //добавить в корзину
            if (ID != null)
            {
                var item1 = await _context.Item.FindAsync(ID);
                if (item1.Amount > 0)
                {
                    Order cart = await _context.Order.Include(f => f.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "CART");
                    await cart.AddToCart(item1, _context);

                    int? jump_to = ID;
                    ViewBag.JumpToDivId = "DivId_item(" + jump_to + ")";
                }
                
            }

            if (ID_wishlist != null)
            {
                var item1 = await _context.Item.FindAsync(ID_wishlist);
                
                    Order cart = await _context.Order.Include(f => f.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "WISH_LIST");
                    await cart.AddToWaitList(item1, _context);

                    int? jump_to = ID_wishlist;
                    ViewBag.JumpToDivId = "DivId_item(" + jump_to + ")";


            }

            //Получение категории для обработки в функции FilterToUrl
            var this_category = await _context.Category.FindAsync(categoryId);
            this_category.DeSerializeCategory();
            filter.CharactObject = this_category.CharactObject;

            List<Item> forManuf = await _context.Item.Where(a => a.CategoryId == categoryId).ToListAsync();
            await filter.getManufacturers(forManuf, _context);

            //
            var result = filter.FilterToUrl();
            //

            //Упаковка полученной query в правильный url и сам редерикт
            if (result.Count != 0)
            {
                /*
                if (ID != null)
                {
                    result.Add("scrollId", ID.ToString());
                }
                if (ID_wishlist != null)
                {
                    result.Add("scrollId", ID_wishlist.ToString());
                }
                */

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
            Category this_category = await _context.Category.FirstOrDefaultAsync(id => id.Id == categoryId);
            this_category.DeSerializeCategory();

            //Получение модели фильтра из URL
            FullFilter fullFilter = new FullFilter();
            await fullFilter.UrlToFilter(filterrr, this_category, _context, ViewData);
            //

            fullFilter.CharactObject = this_category.CharactObject;

            //Получение всех товаров данной категории для последующего 
            List<Item> item_list = await _context.Item.Where(a => a.CategoryId == categoryId).ToListAsync();
            foreach (Item item in item_list)
            {
                item.DeSerializeItem();
                item.CharactObject = this_category.CharactObject;
            }

            List<Item> filtered = new List<Item>(item_list);

            filtered = await fullFilter.FilterOnGet(ViewData,filtered,_context);

            List<int> notAvalible_Items = new List<int>();
            List<int> notAvalible_WishList = new List<int>();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                Order cart = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == userId && i.Status == "CART");
                notAvalible_Items = cart.NotAvalibleItemsIDS_cart(item_list);

                Order wish_list = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == userId && i.Status == "WISH_LIST");
                notAvalible_WishList = wish_list.NotAvalible_wishList(item_list);
            }

            ItemsViewModel items_category = new ItemsViewModel
            {
                itemList = filtered,
                category = this_category,
                filter = fullFilter,
                NOT_AVALIBLE_itemListIDS = notAvalible_Items,
                NOT_AVALIBLE_waitList = notAvalible_WishList
            };
            items_category.filter.getMaxMinPrice(items_category.itemList);
            //items_category.filter.Manufacturers = new List<Manufacturer>();
            await items_category.filter.getManufacturers(item_list, _context);


            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryTitle = this_category.Title;

            return View("Index", items_category);

        }



        
    }
}
