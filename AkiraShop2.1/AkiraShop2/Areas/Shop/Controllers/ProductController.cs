using AkiraShop2.Controllers;
using AkiraShop2.Data;
using AkiraShop2.Entities;
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
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public ProductController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
            _logger = logger;
        }

        //GET стартовый
        [HttpGet("Shop/Product/{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            Item item = await _context.Item.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            item.DeSerializeItem();

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                if (item.Amount > 0)
                {
                    Order cart = await _context.Order.Include(f => f.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == userId && i.Status == "CART");
                    OrderItem cartOrderTARGET = cart.OrderItems.FirstOrDefault(oi => oi.OrderItem_ItemId == item.Id);
                    if (cartOrderTARGET != null)
                    {
                        if (cart.OrderItems.FirstOrDefault(oi => oi.OrderItem_ItemId == item.Id).OrderItem_Amount >= item.Amount)
                        {
                            ViewBag.NotAvalible = item.Id;
                        }
                    }
                    
                }

                Order wishList = await _context.Order.Include(f => f.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == userId && i.Status == "WISH_LIST");
                OrderItem wishOrderTARGET = wishList.OrderItems.FirstOrDefault(oi => oi.OrderItem_ItemId == item.Id);
                if (wishOrderTARGET != null)
                {
                    ViewBag.NotAvalibleWish = item.Id;
                }

            }

            return View(item);
        }

        
        public async Task<IActionResult> AddToCard(int ID)
        {
            var item1 = await _context.Item.FindAsync(ID);
            if (item1.Amount > 0)
            {
                Order cart = await _context.Order.Include(f => f.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "CART");
                await cart.AddToCart(item1, _context);

            }

            return RedirectToAction(nameof(Index), new { id = ID});
        }

        public async Task<IActionResult> AddToWish(int ID)
        {
            var item1 = await _context.Item.FindAsync(ID);
            if (item1.Amount > 0)
            {
                Order cart = await _context.Order.Include(f => f.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "WISH_LIST");
                await cart.AddToWaitList(item1, _context);

            }

            return RedirectToAction(nameof(Index), new { id = ID });
        }


    }
}
