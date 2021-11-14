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
using System.Security.Claims;

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

            List<Order> orders = await _context.Order.Include(o => o.OrderItems).Where(o => o.UserOrderId == userId).ToListAsync();

            Dictionary<string, List<Order>> result = new Dictionary<string, List<Order>>();

            List<Order> formed = new List<Order>();
            List<Order> delivered = new List<Order>();
            List<Order> CART_WISH = new List<Order>();
            foreach (var order in orders)
            {
                await order.InitOrder(_context);
                if (order.Status == "CART")
                {
                    List<Item> notAvalibleItems = order.order_checkForAvalibleAmount(order.items_with_amounts);
                    Order wait_list = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(i => i.UserOrderId == userId && i.Status == "WISH_LIST");
                    if (notAvalibleItems.Count != 0)
                    {
                        
                        await order.RemoveNotAvalibleItems(notAvalibleItems, wait_list, _context);
                    }
                    CART_WISH.Add(order);
                    CART_WISH.Add(wait_list);
                    
                }
                if (order.Status == "formed")
                {
                    formed.Add(order);
                }
                if (order.Status == "delivered")
                {
                    delivered.Add(order);
                }
            }
            result.Add("formed", formed);
            result.Add("delivered", delivered);
            result.Add("CART_WISH", CART_WISH);


            return View(result);
        }

        public async Task<IActionResult> Delete(int? orderId_index, int? orderId_formed)
        {
            int? orderId = null;
            if (orderId_index == null && orderId_formed == null)
            {
                return NotFound();
            }
            if (orderId_index != null)
            {
                orderId = orderId_index;
            }
            else
            {
                orderId = orderId_formed;
            }
            

            var order = await _context.Order.FirstOrDefaultAsync(o=>o.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();



            if (orderId_formed != null)
            {
                return RedirectToAction(nameof(FormedOrders));
            }
            return RedirectToAction(nameof(Index), new { userId = order.UserOrderId });


        }

        public async Task<IActionResult> FormedOrders()
        {
            List<Order> orders = await _context.Order.Include(o => o.OrderItems).Where(o => o.Status == "formed").ToListAsync();
            List<string> ids = orders.Select(i => i.UserOrderId).Distinct().ToList();
            var users = await _context.Users.Where(x => ids.Contains(x.Id)).ToListAsync();
            Dictionary<ApplicationUser, List<Order>> result = new Dictionary<ApplicationUser, List<Order>>();
            foreach (var user in users)
            {
                result.Add(user, orders.Where(o => o.UserOrderId == user.Id).ToList());
                if (result.Last().Value.Count != 0)
                {
                    foreach (var order in result.Last().Value)
                    {
                        await order.InitOrder(_context);
                    }
                }
                
            }
            return View(result);
        }
        public async Task<IActionResult> DeliverOrder(int? orderId_index, int? orderId_formed)
        {
            int? orderId = null;
            if (orderId_index == null && orderId_formed == null)
            {
                return NotFound();
            }
            if (orderId_index != null)
            {
                orderId = orderId_index;
            }
            else
            {
                orderId = orderId_formed;
            }

            Order order = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(o=>o.Id == orderId);
            if (order != null)
            {
                if (order.Status == "formed")
                {
                    order.Status = "delivered";
                    await _context.SaveChangesAsync();
                    if (orderId_formed != null)
                    {
                        return RedirectToAction(nameof(FormedOrders));
                    }
                    return RedirectToAction(nameof(Index), new { userId = order.UserOrderId });
                }
            }
            return NotFound();
        }
    }
}
