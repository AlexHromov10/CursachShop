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

namespace AkiraShop2
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment ihost)
        {
            _context = context;
            _hostingEnvironment = ihost;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Category.ToListAsync();
            foreach(Category cat in categories)
            {
                cat.DeSerializeCategory();
            }
            return View(categories);
        }

        //NOT USING!!!!!///////////////////////
        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            Category category = new Category();
            category.CharactObject.Add(new CategoryCharacteristics());
            return View(category);
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Descriprions,ImageFile,CharactObject")] Category category)
        {
            if (category.CheckForNumeric())
            {
                if (ModelState.IsValid)
                {
                    if (category.ImageFile != null)
                    {
                        category.WriteContentCategory(_hostingEnvironment, category.ImageFile);

                        _context.Add(category);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                }
            }
            return View(category);


        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Далее три функции для добавления полей INPUT новых характеристик и их значений///////////////////
        [HttpPost]
        [ActionName("AddCharact")]
        [ValidateAntiForgeryToken]
        public IActionResult AddCharact(Category category)
        {
           
            category.CharactObject.Add(new CategoryCharacteristics());

            //return RedirectToAction(nameof(Create), category );
            
            return View("Create", new Category
            {
                ImageFile = category.ImageFile,
                CharactObject = category.CharactObject
            }); ;
            
        }

        [HttpPost]
        [ActionName("DelCharact")]
        [ValidateAntiForgeryToken]
        public IActionResult DelCharact(Category category)
        {
            int last = category.CharactObject.Count - 1;
            if(last > 0)
            {
                category.CharactObject.RemoveAt(last);
            }
            
            //category.CharactObject.RemoveAt(last);

            return View("Create", new Category
            {
                CharactObject = category.CharactObject
            });
        }

        [HttpPost]
        [ActionName("AddCharactValue")]
        [ValidateAntiForgeryToken]
        public IActionResult AddCharactValue(Category category, string AddCharactValueID)
        {
            int ids = Int32.Parse(AddCharactValueID.Substring(34, 1));

            ViewBag.JumpToDivId = "DivId("+ids+")";
           category.CharactObject[ids].charactValues_Bool.charactValues.Add(string.Empty);

            return View("Create", new Category
            {
                ImageFile = category.ImageFile,
                CharactObject = category.CharactObject
            });
        }

        [HttpPost]
        [ActionName("DelCharactValue")]
        [ValidateAntiForgeryToken]
        public IActionResult DelCharactValue(Category category, string DelCharactValueID)
        {
            int ids = Int32.Parse(DelCharactValueID.Substring(33, 1));

            int last = category.CharactObject[ids].charactValues_Bool.charactValues.Count - 1;
            if (last > 0)
            {
                category.CharactObject[ids].charactValues_Bool.charactValues.RemoveAt(last);
            }
            ViewBag.JumpToDivId = "DivId(" + ids + ")";
            return View("Create", new Category
            {
                ImageFile = category.ImageFile,
                CharactObject = category.CharactObject
            });
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            category.DeSerializeCategory();

            

            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Descriprions,Image,ImageFile_EDIT,CharactObject")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            // {
            
            if (category.CheckForNumeric())
            {
                try
                {
                    if (category.ImageFile_EDIT != null)
                    {
                        if (Path.GetExtension(category.ImageFile_EDIT.FileName) == ".png" || Path.GetExtension(category.ImageFile_EDIT.FileName) == ".jpg")
                        {
                            //var buff = Path.GetExtension(category.ImageFile_EDIT.FileName);
                            category.DeleteImage(_hostingEnvironment);
                            category.WriteContentCategory(_hostingEnvironment, category.ImageFile_EDIT);
                        }
                        else
                        {
                            return View(category);
                        }
                        
                    }
                    else
                    {
                        category.SerializeCategory();
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
          //  }
           return View(category);
        }


        //НЕ РАБОТАЕТ НЕ РАБОТАЕТ НЕ РАБОТАЕТ НЕ РАБОТАЕТ НЕ РАБОТАЕТ НЕ РАБОТАЕТ//
        [HttpPost]
        [ActionName("CharactEdit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CharactEdit(int id, [Bind("Id,Title,Descriprions,Image,ImageFile_EDIT,CharactObject")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            return Ok("asfasf");
        }
        //////////////////////////////////////////////////////////////////////////


        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);

            category.DeSerializeCategory();

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category.FindAsync(id);
            category.DeleteImage(_hostingEnvironment);

            List<Item> list = await (from item in _context.Item
                                     where item.CategoryId == category.Id
                                     select new Item
                                     {
                                         Image = item.Image,
                                     }).ToListAsync();


            foreach (Item item in list)
            {
                item.DeleteImage(_hostingEnvironment);
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
