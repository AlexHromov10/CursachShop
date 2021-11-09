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

            return View(item);
        }

        
        public async Task<IActionResult> AddToCard(int ID)
        {
            var item1 = await _context.Item.FindAsync(ID);
            if (item1.Amount > 0)
            {
                item1.DecreaseAmount();

                Order order = await _context.Order.FirstOrDefaultAsync(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "CART");
                if (order == null)
                {
                    return NotFound();
                }

                OrderItem orderItem = new OrderItem { OrderItem_ItemId = item1.Id, OrderItem_OrderId = order.Id };

                _context.Add(orderItem);

                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index), new { id = ID});
        }
    }
}
