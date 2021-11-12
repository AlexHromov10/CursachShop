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
                                       where order.UserOrderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && order.Status != "CART" && order.Status != "WISH_LIST"
                                       select new Order
                                       {
                                           Id = order.Id,
                                           UserOrderId = order.UserOrderId,
                                           Status = order.Status,
                                           OrderItems = order.OrderItems
                                       }).ToListAsync();

            foreach (var order in orders)
            {
                await order.InitOrder(_context);
            }

            return View(orders);
        }

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

    }
}
