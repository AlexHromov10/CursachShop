using AkiraShop.Data;
using AkiraShop.Data.Interfaces;
using AkiraShop.Data.Models;
using AkiraShop.Data.Repository;
using AkiraShop.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AkiraShop.Controllers
{
    public class ItemsController : Controller
    {
        //readonly AppDBContent Context;
        readonly IItemsCategory categoryRepository;
        readonly IAllItems itemsRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        /*
        private readonly IAllItems _allItems;
        private readonly IItemsCategory _allCategories;
        
        public ItemsController(IAllItems iAllItems, IItemsCategory iItemsCat)
        {
            _allItems = iAllItems;
            _allCategories = iItemsCat;
        }
        */
        public ItemsController(IAllItems iITEMSrepository, IItemsCategory iCATrepository, IWebHostEnvironment ihost)
        {
            itemsRepository = iITEMSrepository;
            categoryRepository = iCATrepository;
            hostingEnvironment = ihost;
        }

   
        public ViewResult List()
        {   
            ViewBag.Title = "SHOP";


            ItemsListViewModel obj = new ItemsListViewModel();
            obj.AllItems = itemsRepository.AllItems;
            obj.AllCat = categoryRepository.AllCategories.ToList();

            //obj.currCategory = "Стиралка";


            return View(obj);
        }

        public ViewResult AddItems()
        {
            ViewBag.Title = "ADD ITEMS";

            ItemsListViewModel obj = new ItemsListViewModel();
            obj.AllItems = itemsRepository.AllItems.ToList();
            obj.AllCat = categoryRepository.AllCategories.ToList();
            //CreateItemModel obj = new CreateItemModel();
            //ViewBag.Categories = categoryRepository.AllCategories.ToList<string>();


                return View(obj);
        }



            //[Route("api/post_item")]
            [HttpPost]
        [ActionName("createItem")]
        [ValidateAntiForgeryToken]
        public IActionResult createItem(ItemsListViewModel newItem)
        {

            //if (ModelState.IsValid)
            //{

                
                Category match_category = categoryRepository.getObjectCategory(newItem.currCategory);

                if (newItem.ItemModel.Image != null)  
                {
                    var fileName = Path.GetFileNameWithoutExtension(Path.GetFileName(newItem.ItemModel.Image.FileName)) + "_" + Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(newItem.ItemModel.Image.FileName);
                    var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                    var filePath = Path.Combine(uploads, fileName);
                    newItem.ItemModel.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    newItem.ItemModel.Item.img = fileName; // Set the file name
                }



                //if (item.itemName.Contains )
                newItem.ItemModel.Item.categoryID = match_category.id;
                itemsRepository.InsertItem(newItem.ItemModel.Item);
                //categoryRepository.AddIdToCategory(item.id, match_category.id);

                return Ok(newItem.ItemModel.Item.img);
            //}

            //return Ok("No");

        }

    }
}
