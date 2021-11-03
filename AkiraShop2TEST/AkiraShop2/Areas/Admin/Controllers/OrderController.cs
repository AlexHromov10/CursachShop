using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AkiraShop2.Data;
using AkiraShop2.Entities;
using AkiraShop2.Entities.HelperEntities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace AkiraShop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
        }
        public async Task<IActionResult> Index(string userId)
        {
            ViewBag.UserEmail = _context.Users.First(i => i.Id == userId).Email;

            List<Order> orders = await (from order in _context.Order
                                        where order.UserOrderId == userId
                                        select new Order
                                        {
                                            Id = order.Id,
                                            UserOrderId = order.UserOrderId,
                                            Status = order.Status,
                                            OrderItems = order.OrderItems
                                        }).ToListAsync();

            
            foreach (var order in orders)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    Item item = _context.Item.First(i => i.Id == orderItem.OrderItem_ItemId);
                    if (item != null)
                    {
                        order.ItemsForOrder.Add(item);
                    }
                }
            }

            return View(orders);
        }
    }
}
