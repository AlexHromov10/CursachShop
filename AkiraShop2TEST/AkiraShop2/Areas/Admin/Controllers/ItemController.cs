using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AkiraShop2.Data;
using AkiraShop2.Entities;
using AkiraShop2.Data.Extensions;
using Newtonsoft.Json;
using AkiraShop2.Entities.HelperEntities;
using Microsoft.AspNetCore.Hosting;
using AkiraShop.Data.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace AkiraShop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ItemController(ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
        }

        // GET: Admin/Item
        public async Task<IActionResult> Index(int categoryId, string categoryTitle)
        {
                List<Item> list = await (from item in _context.Item
                                         where item.CategoryId == categoryId
                                         select new Item
                                         {
                                             Amount=item.Amount,
                                             Id = item.Id,
                                             Title = item.Title,
                                             Description = item.Description,
                                             Price = item.Price,
                                             Json_Characterisitcs_exact = item.Json_Characterisitcs_exact,
                                             Image = item.Image,
                                             ManufacturerId = item.ManufacturerId,
                                             CategoryId = item.CategoryId
                                         }).ToListAsync();


                foreach (Item item in list)
                {
                    item.DeSerializeItem();
                }

                ViewBag.CategoryId = categoryId;
                ViewBag.CategoryTitle = categoryTitle;
                return View(list); 
        }

        // GET: Admin/Item/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Admin/Item/Create
        //public async Task<IActionResult> Create()
        public async Task<IActionResult> Create(int categoryId)
        {
            List<Manufacturer> manufacturers = await _context.Manufacturer.ToListAsync();
            Category match_category = _context.Category.FirstOrDefault(m => m.Id == categoryId);

            Item item = new Item
            {
                CategoryId = categoryId,
                Manufacturers = manufacturers.ConvertToSelectList(0)
            };

            item.ReadContentItem(match_category);
            item.Characteristics = item.CharacteristicsConvertToSelectList();
            ViewBag.CategoryId = item.CategoryId;
            ViewBag.CategoryTitle = match_category.Title;

            return View(item);
        }


        // POST: Admin/Item/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [ActionName("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price,CategoryId,ManufacturerId,CharactObjectEXACT,ImageFile,Amount")] Item item)
        {
            if (ModelState.IsValid && item.Amount != 0)
            {
                if (item.ImageFile != null)
                {
                    item.WriteContentItem(_hostingEnvironment, item.ImageFile);
                }
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new {categoryId = item.CategoryId});
            }
            else
            {
                List<Manufacturer> manufacturers = await _context.Manufacturer.ToListAsync();
                item.Manufacturers = manufacturers.ConvertToSelectList(0);

                Category match_category = _context.Category.FirstOrDefault(m => m.Id == item.CategoryId);
                item.ReadContentItem(match_category);
                item.Characteristics = item.CharacteristicsConvertToSelectList();
                ViewBag.CategoryId = item.CategoryId;
                ViewBag.CategoryTitle = match_category.Title;

                return View(item);
            }
        }

        // GET: Admin/Item/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Manufacturer> manufacturers = await _context.Manufacturer.ToListAsync();

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            Category match_category = _context.Category.FirstOrDefault(m => m.Id == item.CategoryId);

            item.Manufacturers = manufacturers.ConvertToSelectList(item.ManufacturerId);
            item.ReadContentItem(match_category);
            item.DeSerializeItem();
            item.Characteristics = item.CharacteristicsConvertToSelectList();

            ViewBag.CategoryId = item.CategoryId;
            ViewBag.CategoryTitle = match_category.Title;

            return View(item);
        }


            // POST: Admin/Item/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Image,ImageFile_EDIT,CharactObjectEXACT,Price,CategoryId,ManufacturerId,Amount")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            
            //if (ModelState.IsValid)
           // {
            try
            {
                if (item.Amount != 0)
                {
                    if (item.ImageFile_EDIT != null)
                    {
                        if (Path.GetExtension(item.ImageFile_EDIT.FileName) == ".png" || Path.GetExtension(item.ImageFile_EDIT.FileName) == ".jpg")
                        {
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Item");
                            var filePath = Path.Combine(uploads, item.Image);

                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);

                            }
                            item.WriteContentItem(_hostingEnvironment, item.ImageFile_EDIT);
                        }
                        else
                        {
                            List<Manufacturer> manufacturers = await _context.Manufacturer.ToListAsync();
                            item.Manufacturers = manufacturers.ConvertToSelectList(0);

                            Category match_category = _context.Category.FirstOrDefault(m => m.Id == item.CategoryId);
                            item.ReadContentItem(match_category);
                            item.Characteristics = item.CharacteristicsConvertToSelectList();
                            ViewBag.CategoryId = item.CategoryId;
                            ViewBag.CategoryTitle = match_category.Title;
                            return View(item);
                        }


                    }
                    else
                    {
                        item.SerializeItem();
                    }


                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    List<Manufacturer> manufacturers = await _context.Manufacturer.ToListAsync();
                    item.Manufacturers = manufacturers.ConvertToSelectList(0);

                    Category match_category = _context.Category.FirstOrDefault(m => m.Id == item.CategoryId);
                    item.ReadContentItem(match_category);
                    item.Characteristics = item.CharacteristicsConvertToSelectList();
                    ViewBag.CategoryId = item.CategoryId;
                    ViewBag.CategoryTitle = match_category.Title;
                    return View(item);
                }
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(item.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index),new { categoryId = item.CategoryId});
          //  }
            //return View(item);
        }

        // GET: Admin/Item/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (item == null)
            {
                return NotFound();
            }

            item.DeSerializeItem();
            ViewBag.CategoryId = item.CategoryId;
            
            return View(item);
        }

        // POST: Admin/Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.FindAsync(id);
            item.DeleteImage(_hostingEnvironment);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { categoryId = item.CategoryId });
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.Id == id);
        }
    }
}
