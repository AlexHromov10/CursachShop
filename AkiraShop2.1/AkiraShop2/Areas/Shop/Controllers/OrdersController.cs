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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            List<Order> formed_orders = await _context.Order.Include(o => o.OrderItems).Where(o => o.Status == "formed" && o.UserOrderId == userId).ToListAsync();
            List<Order> delivered_orders = await _context.Order.Include(o => o.OrderItems).Where(o => o.Status == "delivered" && o.UserOrderId == userId).ToListAsync();

            foreach (var order in formed_orders)
            {
                await order.InitOrder(_context);
            }
            foreach (var order in delivered_orders)
            {
                await order.InitOrder(_context);
            }

            Dictionary<string, List<Order>> result = new Dictionary<string, List<Order>>();
            result.Add("formed",formed_orders);
            result.Add("delivered", delivered_orders);

            return View(result);
        }

        public async Task<IActionResult> Details(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }
            Order order = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
            await order.InitOrder(_context);
            return View(order);
        }

        /*
        public async Task<IActionResult> DeleteOrder(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }
            Order order_del = await _context.Order.Include(f => f.OrderItems).FirstOrDefaultAsync(i => i.Id == orderId);

            await order_del.order_Delete(_context);

            return RedirectToAction(nameof(Index));
        }
        */
    }
}
