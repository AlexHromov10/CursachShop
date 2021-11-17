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
            category.DeSerializeCategory();
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
            List<int> errorIds = new List<int>();
            errorIds = category.CheckForNumeric();
            if (errorIds.Count == 0)
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
            else
            {
                foreach (var id in errorIds)
                {
                    ModelState.AddModelError("CharactObject["+id+"].charactName", "В числовой характеристики присутсвуют буквы!");
                }
                ViewBag.JumpToDivId = category.CharactObject[errorIds.First()].charactName;
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
            int jump_to = category.CharactObject.Count - 1;
            ViewBag.JumpToDivId = "DivId_charact(" + jump_to + ")";
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

            int jump_to = category.CharactObject.Count - 1;
            ViewBag.JumpToDivId = "DivId_charact("+ jump_to + ")";
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

            ViewBag.JumpToDivId = "DivId_charactVal(" + ids+")";
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
            ViewBag.JumpToDivId = "DivId_charactVal(" + ids + ")";
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Descriprions,Image,ImageFile_EDIT,CharactObject")] Category category, int? Charact_Add_Del, int? CharactVal_AddId, int? CharactVal_DelId)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (Charact_Add_Del == 1)
            {
                category.CharactObject.Add(new CategoryCharacteristics());

                //return RedirectToAction(nameof(Create), category );
                int jump_to = category.CharactObject.Count - 1;
                ViewBag.JumpToDivId = "DivId_charact(" + jump_to + ")";
                return View("Edit", category);
            }
            if (Charact_Add_Del == -1)
            {
                int last = category.CharactObject.Count - 1;
                if (last > 0)
                {
                    category.CharactObject.RemoveAt(last);
                }

                int jump_to = category.CharactObject.Count - 1;
                ViewBag.JumpToDivId = "DivId_charact(" + jump_to + ")";
                return View("Edit", category);
            }

            if (CharactVal_AddId != null)
            {

                ViewBag.JumpToDivId = "DivId_charactVal(" + CharactVal_AddId.Value + ")";
                category.CharactObject[CharactVal_AddId.Value].charactValues_Bool.charactValues.Add(string.Empty);

                return View("Edit", category);
            }
            if (CharactVal_DelId != null)
            {

                int last = category.CharactObject[CharactVal_DelId.Value].charactValues_Bool.charactValues.Count - 1;
                if (last > 0)
                {
                    category.CharactObject[CharactVal_DelId.Value].charactValues_Bool.charactValues.RemoveAt(last);
                }
                ViewBag.JumpToDivId = "DivId_charactVal(" + CharactVal_DelId.Value + ")";

                return View("Edit", category);
            }





            //if (ModelState.IsValid)
            // {

            List<int> errorIds = new List<int>();
            errorIds = category.CheckForNumeric();
            if (errorIds.Count == 0)
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
            else
            {
                foreach (var idd in errorIds)
                {
                    ModelState.AddModelError("CharactObject[" + id + "].charactName", "В числовой характеристики присутсвуют буквы!");
                }
                ViewBag.JumpToDivId = category.CharactObject[errorIds.First()].charactName;
            }
            return View(category);
        }

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
