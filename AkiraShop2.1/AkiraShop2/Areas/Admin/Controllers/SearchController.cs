using AkiraShop2.Areas.Admin.Models;
using AkiraShop2.Data;
using AkiraShop2.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AkiraShop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SearchController(ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
        }

        // GET: Admin/Search
        public IActionResult Index()
        {
            SearchModel searchModel = new SearchModel();
            return View(searchModel);
        }

        //Ужас не смотреть слабонервным
        [ActionName("Search")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(SearchModel searchModel, string action)
        {
            //Order
            if (action == "Order_id")
            {
                if (searchModel.orderId == null)
                {
                    ModelState.AddModelError("orderId", "Введите ID заказа");
                    return View("Index",searchModel);
                }
                var order = await _context.Order.FirstOrDefaultAsync(c => c.Id == searchModel.orderId);
                if (order != null)
                {
                    return RedirectToAction("Details", "Order", new { orderId = order.Id });
                }
                ModelState.AddModelError("orderId", "Заказ не найден");
                return View("Index", searchModel);
            }

            //User
            if (action == "User_email")
            {
                if (searchModel.userEmail == null)
                {
                    ModelState.AddModelError("userEmail", "Введите E-mail пользователя");
                    return View("Index", searchModel);
                }

                string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
                ApplicationUser user = null;
                if (Regex.IsMatch(searchModel.userEmail, pattern, RegexOptions.IgnoreCase))
                {
                    user = await _context.Users.FirstOrDefaultAsync(c => c.Email == searchModel.userEmail);
                }
                else
                {
                    ModelState.AddModelError("userEmail", "Некорректный E-mail");
                    return View("Index", searchModel);
                }

                if (user != null)
                {
                    return RedirectToAction("Details", "User", new { userId = user.Id });
                }
                ModelState.AddModelError("userEmail", "Пользователь не найден");
                return View("Index", searchModel);
            }

            if (action == "User_number")
            {
                if (searchModel.userNumber == null)
                {
                    ModelState.AddModelError("userNumber", "Введите мобильный номер пользователя");
                    return View("Index", searchModel);
                }
                string pattern = @"((8|\+7)-?)?\(?\d{3,5}\)?-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}((-?\d{1})?-?\d{1})?";
                ApplicationUser user = null;
                if (Regex.IsMatch(searchModel.userNumber, pattern, RegexOptions.IgnoreCase))
                {
                    user = await _context.Users.FirstOrDefaultAsync(c => c.PhoneNumber == searchModel.userNumber);
                }
                else
                {
                    ModelState.AddModelError("userNumber", "Некорректный мобильный номер");
                    return View("Index", searchModel);
                }

                if (user != null)
                {
                    return RedirectToAction("Details", "User", new { userId = user.Id });
                }
                ModelState.AddModelError("userNumber", "Пользователь не найден");
                return View("Index", searchModel);
            }






            //Category
            if (action == "Category_id")
            {
                if (searchModel.categoryId == null)
                {
                    ModelState.AddModelError("categoryId", "Введите ID категории");
                    return View("Index", searchModel);
                }
                var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == searchModel.categoryId);
                if (category != null)
                {
                    return RedirectToAction("Details", "Category", new { id = category.Id });
                }
                ModelState.AddModelError("categoryId", "Категория не найдена");
                return View("Index", searchModel);
            }

            if (action == "Category_title")
            {
                if (searchModel.categoryTitle == null)
                {
                    ModelState.AddModelError("categoryTitle", "Введите название категории");
                    return View("Index", searchModel);
                }
                var category = await _context.Category.FirstOrDefaultAsync(c => c.Title == searchModel.categoryTitle);
                if (category != null)
                {
                    return RedirectToAction("Details", "Category", new { id = category.Id });
                }
                ModelState.AddModelError("categoryTitle", "Категория не найдена");
                return View("Index", searchModel);
            }





            //Item
            if (action == "Item_id")
            {
                if (searchModel.itemId == null)
                {
                    ModelState.AddModelError("itemId", "Введите ID товара");
                    return View("Index", searchModel);
                }
                var item = await _context.Item.FirstOrDefaultAsync(c => c.Id == searchModel.itemId);
                if (item != null)
                {
                    return RedirectToAction("Details", "Item", new { id = item.Id });
                }
                ModelState.AddModelError("itemId", "Товар не найден");
                return View("Index", searchModel);
            }
            if (action == "Item_title")
            {
                if (searchModel.itemTitle == null)
                {
                    ModelState.AddModelError("itemTitle", "Введите название товара");
                    return View("Index", searchModel);
                }
                var item = await _context.Item.FirstOrDefaultAsync(c => c.Title == searchModel.itemTitle);
                if (item != null)
                {
                    return RedirectToAction("Details", "Item", new { id = item.Id });
                }
                ModelState.AddModelError("itemTitle", "Товар не найден");
                return View("Index", searchModel);
            }

            
            return RedirectToAction(nameof(Index));
        }

        
    }

}
