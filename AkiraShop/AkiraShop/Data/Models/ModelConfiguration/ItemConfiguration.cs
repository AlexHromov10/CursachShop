using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop.Data.Models.ModelConfiguration
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(prop => prop.id);

            builder.Property(prop => prop.itemName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(prop => prop.Desc)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(prop => prop.img)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(prop => prop.price)
                .IsRequired();
            

            builder.Property(prop => prop.categoryID)
                .HasMaxLength(1000);
        }   
    }
}
