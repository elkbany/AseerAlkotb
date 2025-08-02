using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("Wishlists");


         

            // Many-to-One: Wishlist --> WishlistItems
            builder.HasMany(w => w.WishlistItems)
                .WithOne(wi=>wi.Wishlist)
                .HasForeignKey(w => w.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);



            builder.HasOne(w => w.User)
                .WithOne(u => u.Wishlist)
               .HasForeignKey<Wishlist>("UserId")
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
