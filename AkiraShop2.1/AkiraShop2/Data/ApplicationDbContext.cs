using AkiraShop2.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AkiraShop2.Data
{
    public class ApplicationUser:IdentityUser
    {
        [StringLength(250)]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [StringLength(250)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [StringLength(250)]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[0-9]{6}", ErrorMessage ="Некорректный индекс")]
        [Display(Name = "Индекс")]
        public string PostCode { get; set; }

        [RegularExpression(@"((8|\+7)-?)?\(?\d{3,5}\)?-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}((-?\d{1})?-?\d{1})?", ErrorMessage = "Некорректный номер телефона")]
        public override string PhoneNumber { get; set; }



        [ForeignKey("UserOrderId")]
        public virtual ICollection<Order> Orders { get; set; }



    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        


        public DbSet<Category> Category { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<Manufacturer> Manufacturer  { get;set;}
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
    }
}
