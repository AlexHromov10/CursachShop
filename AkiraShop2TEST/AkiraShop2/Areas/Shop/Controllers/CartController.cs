﻿using AkiraShop2.Controllers;
using AkiraShop2.Data;
using AkiraShop2.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AkiraShop2.Areas.Orders.Controllers
{
    [Area("Shop")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public CartController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
            _logger = logger;
        }

        //GET
        public async Task<IActionResult> Index()
        {

            //Order cart = _context.Order.FirstOrDefault(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "CART" && i.OrderItems != null);

            List<Order> cart = await (from order in _context.Order
                                        where order.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && order.Status == "CART"
                                        select new Order
                                        {
                                            Id = order.Id,
                                            UserOrderId = order.UserOrderId,
                                            Status = order.Status,
                                            OrderItems = order.OrderItems
                                        }).ToListAsync();

            if (cart != null)
            {
                foreach (var order in cart)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        Item item = _context.Item.FirstOrDefault(i => i.Id == orderItem.OrderItem_ItemId);
                        if (item != null)
                        {
                            order.ItemsForOrder.Add(item);
                        }
                    }
                }

                return View(cart);
            }
            else
            {
                return NotFound();
            }
            

        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int? itemId)
        {
            if (itemId == null)
            {
                return NotFound();
            }

            

            var item = await _context.Item.FindAsync(itemId);
            item.Amount++;
            //var user = await _context.Users.FindAsync();

            Order order = _context.Order.FirstOrDefault(i => i.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && i.Status == "CART");

            //OrderItem orderItem = new OrderItem { OrderItem_ItemId = itemId.Value, OrderItem_OrderId = order.Id };

            OrderItem orderItem =  _context.OrderItem.FirstOrDefault(i => i.OrderItem_ItemId == itemId.Value && i.OrderItem_OrderId == order.Id);

            _context.OrderItem.Remove(orderItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            Order cart = _context.Order.FirstOrDefault(i => i.Id == orderId);
            cart.Status = "formed";

            Order new_cart = new Order { UserOrderId = cart.UserOrderId, Status = "CART" };

            _context.Update(cart);
            _context.Add(new_cart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
