using AkiraShop2.Data;
using AkiraShop2.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AkiraShop2.Areas.Shop.Controllers
{
    [Area("Shop")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
        }

        //GET
        public async Task<IActionResult> Index()
        {
            List<Order> orders = await(from order in _context.Order
                                       where order.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && order.Status != "CART"
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            var orders = (from ordr in _context.Order
                          where ordr.Id == orderId
                          select new Order
                          {
                              Id = ordr.Id,
                              UserOrderId = ordr.UserOrderId,
                              Status = ordr.Status,
                              OrderItems = ordr.OrderItems
                          });

            foreach (var ordr in orders)
            {
                foreach (var orderItem in ordr.OrderItems)
                {
                    Item item = _context.Item.First(i => i.Id == orderItem.OrderItem_ItemId);
                    if (item != null)
                    {
                        item.Amount++;
                    }
                }
                _context.Remove(ordr);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
