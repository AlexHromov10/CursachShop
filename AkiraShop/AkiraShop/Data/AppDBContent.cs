using AkiraShop.Data.Models;
using AkiraShop.Data.Models.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data
{
    public class AppDBContent : DbContext
    {
        public AppDBContent(DbContextOptions<AppDBContent> options) : base(options)
        {
                    
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.ApplyConfiguration(new ItemConfiguration());
        //}



        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
    }

    
}
