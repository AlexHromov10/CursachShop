using AkiraShop.Data;
using AkiraShop.Data.Interfaces;
using AkiraShop.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Controllers
{
    [Route("api/test")]
    public class TestController : Controller
    {
        readonly IItemsCategory categoryRepository;
        readonly IAllItems itemsRepository;

        public TestController(IAllItems iITEMSrepository, IItemsCategory iCATrepository)
        {
            itemsRepository = iITEMSrepository;
            categoryRepository = iCATrepository;
        }

        [HttpGet]
        public IActionResult GetItem()
        {

            var items = itemsRepository.AllItems.ToList();

            return Ok(items);
        }

        [HttpPost]
        public IActionResult CreateItem()
        {
            /*
            var item = new Item()
            {
                itemName = "aboba1",
                Desc = "cool",

                img = "https://www.qries.com/images/banner_logo.png",

                price = 10000,

                categoryID = "asgag"
            };

            Context.Add(item);
            Context.SaveChangesAsync();

            return Ok("created item!");
            */


            var cat1 = new Category()
            {
                id = new Guid("2f2e9caa-f929-486f-91da-53d1f0d0a45f"),
                categoryName = "holodosi",
                desc = "большиши холодыши, lol, lol",

            };

            var cat2 = new Category()
            {
                id = new Guid("8df14a14-6813-49fe-a0d6-7e12592d60ba"),
                categoryName = "teleki",
                desc = "крутышы теликишы, lol",
            };
            /*
            var cat2 = new Category()
            {
                categoryName = "holodosi",
                desc = "большиши холодыши",

            };
            */
            //categoryRepository.InsertCategory(cat1);
            //categoryRepository.InsertCategory(cat2);




            
            var cat3 = new Category()
            {
                id = new Guid(),
                categoryName = "morozilko",
                desc = "нормыши морозишы",

            };
            

            categoryRepository.InsertCategory(cat1);
            categoryRepository.InsertCategory(cat2);
            categoryRepository.InsertCategory(cat3);
            return Ok("created CATeglory!");
        }
    }
}
